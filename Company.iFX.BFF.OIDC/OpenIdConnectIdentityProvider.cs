using System.Diagnostics;
using System.Net;
using System.Web;

using Company.iFX.BFF.Cryptography;
using Company.iFX.BFF.IdentityModel;
using Company.iFX.BFF.IdentityModel.Client.Extensions;
using Company.iFX.BFF.IdentityModel.Client.Messages;
using Company.iFX.BFF.IdentityProviders;
using Company.iFX.BFF.Logging;
using Company.iFX.BFF.OIDC.Client;
using Company.iFX.BFF.OpenIdConnect;

using Microsoft.Extensions.Caching.Memory;


using TokenResponse = Company.iFX.BFF.IdentityProviders.TokenResponse;

namespace Company.iFX.BFF.OIDC;

public class OpenIdConnectIdentityProvider : IIdentityProvider
{
    #region Members

    private readonly IMemoryCache _cache;
    private readonly OpenIdConnectConfig _configuration;
    private readonly HttpClient _httpClient;
    private readonly ILogger _logger;

    #endregion

    #region Properties

    protected virtual String DiscoveryEndpointAddress => $"{_configuration.Authority.TrimEnd('/')}/" + $"{_configuration.DiscoveryEndpoint.TrimStart('/')}";

    #endregion

    #region Constructors

    public OpenIdConnectIdentityProvider(ILogger logger, IMemoryCache cache, HttpClient httpClient, OpenIdConnectConfig configuration)
    {
        _logger = logger;
        _cache = cache;
        _httpClient = httpClient;
        _configuration = configuration;
    }

    #endregion

    #region Methods

    #region Protected

    protected virtual async Task<Uri> BuildEndSessionUri(String? idToken, String redirectUri)
    {
        DiscoveryDocument openIdConfiguration = await GetDiscoveryDocument();

        String? endSessionUrEndpoint = openIdConfiguration.end_session_endpoint;

        if (endSessionUrEndpoint == null)
        {
            throw new NotSupportedException("Invalid OpenId configuration. OpenId Configuration MUST contain a value for end_session_ endpoint. (https://openid.net/specs/openid-connect-session-1_0-17.html#OPMetadata)");
        }

        String urlEncodedRedirectUri = HttpUtility.UrlEncode(redirectUri);
        String endSessionUrl = $"{endSessionUrEndpoint}?id_token_hint={idToken}&post_logout_redirect_uri={urlEncodedRedirectUri}";
        return new Uri(endSessionUrl);
    }


    protected virtual Parameters? GetFrontChannelParameters()
    {
        return null;
    }


    protected virtual async Task<DiscoveryDocument?> ObtainDiscoveryDocument(String endpointAddress)
    {
        DiscoveryDocumentResponse? discoveryDocument = await _httpClient.GetDiscoveryDocumentAsync(endpointAddress);

        return new DiscoveryDocument {
            authorization_endpoint = discoveryDocument.AuthorizeEndpoint,
            end_session_endpoint = discoveryDocument.EndSessionEndpoint,
            issuer = discoveryDocument.Issuer,
            jwks_uri = discoveryDocument.JwksUri,
            revocation_endpoint = discoveryDocument.RevocationEndpoint,
            token_endpoint = discoveryDocument.TokenEndpoint,
            userinfo_endpoint = discoveryDocument.UserInfoEndpoint,
            introspection_endpoint = discoveryDocument.IntrospectionEndpoint
        };
    }

    #endregion

    #region Public

    public virtual async Task<AuthorizeRequest> GetAuthorizeUrlAsync(String redirectUri)
    {
        Scopes scopes = new Scopes(_configuration.Scopes);

        OidcClient client = new OidcClient(new OidcClientOptions {
            Authority = _configuration.Authority,
            ClientId = _configuration.ClientId,
            ClientSecret = _configuration.ClientSecret,
            RedirectUri = redirectUri,
            Scope = String.Join(" ", scopes),
            DisablePushedAuthorization = _configuration.DisablePushedAuthorization
        });

        AuthorizeState? request = await client.PrepareLoginAsync(GetFrontChannelParameters());

        Debug.Assert(request.StartUrl != null);
        Debug.Assert(request.CodeVerifier != null);

        return new AuthorizeRequest(new Uri(request.StartUrl), request.CodeVerifier);
    }


    public async Task<DiscoveryDocument> GetDiscoveryDocument()
    {
        String endpointAddress = DiscoveryEndpointAddress;

        if (_cache.TryGetValue(DiscoveryEndpointAddress, out Object? discoveryDocument))
        {
            return (DiscoveryDocument)discoveryDocument!;
        }

        discoveryDocument = await ObtainDiscoveryDocument(endpointAddress);

        if (discoveryDocument == null)
        {
            throw new ApplicationException($"Unable to login. Unable to find a well-known/openid-configuration document at {endpointAddress}");
        }

        _cache.Set(endpointAddress, discoveryDocument, TimeSpan.FromHours(1));
        return (DiscoveryDocument)discoveryDocument;
    }


    public async Task<Uri> GetEndSessionEndpointAsync(String? idToken, String baseAddress)
    {
        String logOutRedirectEndpoint = _configuration.PostLogoutRedirectEndpoint.StartsWith('/')
            ? _configuration.PostLogoutRedirectEndpoint
            : $"/{_configuration.PostLogoutRedirectEndpoint}";

        String redirectUrl = $"{baseAddress}{logOutRedirectEndpoint}";

        return await BuildEndSessionUri(idToken, redirectUrl);
    }


    public async Task<IEnumerable<KeySet>> GetJwksAsync(Boolean invalidateCache = false)
    {
        DiscoveryDocument openIdConfiguration = await GetDiscoveryDocument();
        String? jwksUri = openIdConfiguration.jwks_uri;

        if (!invalidateCache && _cache.TryGetValue(jwksUri, out Object? keySet) && keySet != null)
        {
            return (JsonWebKeySet)keySet;
        }

        JsonWebKeySetResponse response = await _httpClient.GetJsonWebKeySetAsync(jwksUri);

        if (response.IsError)
        {
            throw new ApplicationException($"Unable to JSON Web Key Set. " +
                                           $"OIDC server responded {response.HttpStatusCode}: {response.Raw}");
        }

        keySet = JsonWebKeySet.Create(response);
        _cache.Set(jwksUri, keySet, TimeSpan.FromHours(1));
        return keySet as IEnumerable<KeySet> ?? Array.Empty<KeySet>();
    }


    public virtual async Task<TokenResponse> GetTokenAsync(String redirectUri,
        String code,
        String? codeVerifier,
        String traceIdentifier)
    {
        DiscoveryDocument wellKnown = await GetDiscoveryDocument();

        if (wellKnown.token_endpoint == null)
        {
            throw new ApplicationException(
                "Unable to exchange code for access_token. The well-known/openid-configuration document does not contain a token endpoint.");
        }

        IdentityModel.Client.Messages.TokenResponse response = await _httpClient.RequestTokenAsync(new AuthorizationCodeTokenRequest {
            Address = wellKnown.token_endpoint,
            GrantType = OidcConstants.GrantTypes.AuthorizationCode,
            ClientId = _configuration.ClientId,
            ClientSecret = _configuration.ClientSecret,

            Parameters = {
                { OidcConstants.TokenRequest.Code, code },
                { OidcConstants.TokenRequest.RedirectUri, redirectUri },
                { OidcConstants.TokenRequest.CodeVerifier, codeVerifier }
            }
        });

        if (response.IsError)
        {
            throw new ApplicationException($"Unable to retrieve token. OIDC server responded {response.HttpStatusCode}: {response.Raw}");
        }

        await _logger.InformAsync("Queried /token endpoint and obtained id_, access_, and refresh_tokens.");

        DateTime expiryDate = DateTime.UtcNow.AddSeconds(response.ExpiresIn);

        return new TokenResponse(response.AccessToken, response.IdentityToken, response.RefreshToken, expiryDate);
    }


    public async Task<Boolean> IntrospectTokenAsync(String? idToken)
    {
        Boolean result = false;

        if (!String.IsNullOrWhiteSpace(idToken))
        {
            DiscoveryDocument openIdConfiguration = await GetDiscoveryDocument();
            String? introspectionUri = openIdConfiguration.introspection_endpoint;

            TokenIntrospectionResponse response = await _httpClient.IntrospectTokenAsync(new TokenIntrospectionRequest {
                Address = introspectionUri,
                ClientId = _configuration.ClientId,
                ClientSecret = _configuration.ClientSecret,
                Token = idToken
            });
            result = response.IsActive;
        }

        return result;
    }


    public virtual async Task<TokenResponse> RefreshTokenAsync(String refreshToken, String traceIdentifier)
    {
        DiscoveryDocument openIdConfiguration = await GetDiscoveryDocument();
        Scopes scopes = new Scopes(_configuration.Scopes);

        IdentityModel.Client.Messages.TokenResponse response = await _httpClient.RequestRefreshTokenAsync(new RefreshTokenRequest {
            Address = openIdConfiguration.token_endpoint,
            GrantType = OidcConstants.GrantTypes.RefreshToken,
            RefreshToken = refreshToken,
            ClientId = _configuration.ClientId,
            ClientSecret = _configuration.ClientSecret,
            Scope = String.Join(' ', scopes)
        });

        if (response.IsError)
        {
            throw new TokenRenewalFailedException($"Unable to retrieve token. OIDC server responded {response.HttpStatusCode}: {response.Raw}");
        }

        await _logger.InformAsync("Queried /token endpoint (refresh grant) and obtained id_, access_, and refresh_tokens.");

        DateTime expiresIn = DateTime.UtcNow.AddSeconds(response.ExpiresIn);

        return new TokenResponse(response.AccessToken, response.IdentityToken, response.RefreshToken, expiresIn);
    }


    public virtual async Task RevokeAsync(String token, String traceIdentifier)
    {
        DiscoveryDocument openIdConfiguration = await GetDiscoveryDocument();

        TokenRevocationResponse response = await _httpClient.RevokeTokenAsync(new TokenRevocationRequest {
            Address = openIdConfiguration.revocation_endpoint,
            Token = token,
            ClientId = _configuration.ClientId,
            ClientSecret = _configuration.ClientSecret
        });

        if (response.HttpStatusCode != HttpStatusCode.OK)
        {
            throw new ApplicationException($"Unable to revoke tokens. OIDC server responded {response.HttpStatusCode}: \r\n{response.Raw}");
        }

        await _logger.InformAsync("Token revoked.");
    }

    #endregion

    #endregion
}