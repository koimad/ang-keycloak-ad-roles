using Company.iFX.BFF.IdentityModel;
using Company.iFX.BFF.IdentityModel.Client.Extensions;
using Company.iFX.BFF.IdentityModel.Client.Messages;
using Company.iFX.BFF.OIDC.Client.Browser;
using Company.iFX.BFF.OIDC.Client.Infrastructure;
using Company.iFX.BFF.OIDC.Client.Requests;
using Company.iFX.BFF.OIDC.Client.Results;

using Microsoft.Extensions.Logging;

using System.Security.Claims;
using System.Text;

using Boolean = System.Boolean;
using String = System.String;

namespace Company.iFX.BFF.OIDC.Client;

public class OidcClient
{
    #region Members

    private readonly AuthorizeClient _authorizeClient;
    private readonly ILogger _logger;
    private readonly ResponseProcessor _processor;

    private readonly Boolean _useDiscovery;

    #endregion

    #region Properties

    public OidcClientOptions Options { get; }

    #endregion

    #region Constructors

    public OidcClient(OidcClientOptions options)
    {
        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (options.ProviderInformation == null)
        {
            if (options.Authority.IsMissing())
            {
                throw new ArgumentException("No authority specified", nameof(Options.Authority));
            }

            _useDiscovery = true;
        }

        Options = options;
        _logger = options.LoggerFactory.CreateLogger<OidcClient>();
        _authorizeClient = new AuthorizeClient(options);
        _processor = new ResponseProcessor(options, EnsureProviderInformationAsync);
    }

    #endregion

    #region Methods

    #region Internal

    internal async Task EnsureConfigurationAsync(CancellationToken cancellationToken)
    {
        await EnsureProviderInformationAsync(cancellationToken);

        _logger.LogTrace("Effective options:");
        _logger.LogTrace(LogSerializer.Serialize(Options));
    }


    internal async Task EnsureProviderInformationAsync(CancellationToken cancellationToken)
    {
        _logger.LogTrace("EnsureProviderInformation");

        if (_useDiscovery)
        {
            if (Options.RefreshDiscoveryDocumentForLogin == false)
            {
                // discovery document has been loaded before - skip reload
                if (Options.ProviderInformation != null)
                {
                    _logger.LogDebug("Skipping refresh of discovery document.");

                    return;
                }
            }

            HttpClient discoveryClient = Options.CreateClient();

            DiscoveryDocumentResponse disco = await discoveryClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest {
                Address = Options.Authority,
                Policy = Options.Policy.Discovery
            }, cancellationToken).ConfigureAwait(false);

            if (disco.IsError)
            {
                _logger.LogError("Error loading discovery document: {errorType} - {error}", disco.ErrorType.ToString(), disco.Error);

                if (disco.ErrorType == ResponseErrorType.Exception)
                {
                    throw new InvalidOperationException("Error loading discovery document: " + disco.Error, disco.Exception);
                }

                throw new InvalidOperationException("Error loading discovery document: " + disco.Error);
            }

            _logger.LogDebug("Successfully loaded discovery document");
            _logger.LogDebug("Loaded keyset from {jwks_uri}", disco.JwksUri);
            IEnumerable<String?>? kids = disco.KeySet?.Keys?.Select(k => k.Kid);

            if (kids != null)
            {
                _logger.LogDebug($"Keyset contains the following kids: {String.Join(",", kids)}");
            }

            Options.ProviderInformation = new ProviderInformation {
                IssuerName = disco.Issuer,
                KeySet = disco.KeySet,

                AuthorizeEndpoint = disco.AuthorizeEndpoint,
                PushedAuthorizationRequestEndpoint = disco.PushedAuthorizationRequestEndpoint,
                TokenEndpoint = disco.TokenEndpoint,
                EndSessionEndpoint = disco.EndSessionEndpoint,
                UserInfoEndpoint = disco.UserInfoEndpoint,
                TokenEndPointAuthenticationMethods = disco.TokenEndpointAuthenticationMethodsSupported
            };
        }

        if (Options.ProviderInformation == null)
        {
            String error = "Provider Information  is missing";

            _logger.LogError(error);
            throw new InvalidOperationException(error);
        }

        if (Options.ProviderInformation.IssuerName.IsMissing())
        {
            String error = "Issuer name is missing in provider information";

            _logger.LogError(error);
            throw new InvalidOperationException(error);
        }

        if (Options.ProviderInformation.AuthorizeEndpoint.IsMissing())
        {
            String error = "Authorize endpoint is missing in provider information";

            _logger.LogError(error);
            throw new InvalidOperationException(error);
        }

        if (Options.ProviderInformation.TokenEndpoint.IsMissing())
        {
            String error = "Token endpoint is missing in provider information";

            _logger.LogError(error);
            throw new InvalidOperationException(error);
        }

        if (Options.ProviderInformation.KeySet == null)
        {
            if (Options.Policy.Discovery.RequireKeySet)
            {
                String error = "Key set is missing in provider information";

                _logger.LogError(error);
                throw new InvalidOperationException(error);
            }
        }
    }


    internal ClaimsPrincipal ProcessClaims(ClaimsPrincipal user, IEnumerable<Claim> userInfoClaims)
    {
        _logger.LogTrace("ProcessClaims");

        HashSet<Claim> combinedClaims = new HashSet<Claim>(new ClaimComparer(new ClaimOptions { IgnoreIssuer = true }));

        user.Claims.ToList().ForEach(c => combinedClaims.Add(c));
        userInfoClaims.ToList().ForEach(c => combinedClaims.Add(c));

        List<Claim> userClaims;

        if (Options.FilterClaims)
        {
            userClaims = combinedClaims.Where(c => !Options.FilteredClaims.Contains(c.Type)).ToList();
        }
        else
        {
            userClaims = combinedClaims.ToList();
        }

        return new ClaimsPrincipal(new ClaimsIdentity(userClaims, user.Identity.AuthenticationType, user.Identities.First().NameClaimType, user.Identities.First().RoleClaimType));
    }

    #endregion

    #region Public

    public virtual async Task<UserInfoResult> GetUserInfoAsync(String accessToken, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("GetUserInfoAsync");

        await EnsureConfigurationAsync(cancellationToken);

        if (accessToken.IsMissing())
        {
            throw new ArgumentNullException(nameof(accessToken));
        }

        if (Options.ProviderInformation == null)
        {
            String error = "Provider Information  is missing";

            _logger.LogError(error);
            throw new InvalidOperationException(error);
        }

        if (!Options.ProviderInformation.SupportsUserInfo)
        {
            String error = "Provider Information  is missing";
            _logger.LogError(error);
            throw new InvalidOperationException(error);

        }

        HttpClient userInfoClient = Options.CreateClient();

        UserInfoResponse userInfoResponse = await userInfoClient.GetUserInfoAsync(new UserInfoRequest {
            Address = Options.ProviderInformation.UserInfoEndpoint,
            Token = accessToken
        }, cancellationToken).ConfigureAwait(false);

        if (userInfoResponse.IsError)
        {
            return new UserInfoResult {
                Error = userInfoResponse.Error
            };
        }

        return new UserInfoResult {
            Claims = userInfoResponse.Claims
        };
    }


    public virtual async Task<LoginResult> LoginAsync(LoginRequest? request = null, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("LoginAsync");
        _logger.LogInformation("Starting authentication request.");

        request ??= new LoginRequest();

        await EnsureConfigurationAsync(cancellationToken);

        AuthorizeResult authorizeResult = await _authorizeClient.AuthorizeAsync(new AuthorizeRequest {
            DisplayMode = request.BrowserDisplayMode,
            Timeout = request.BrowserTimeout,
            ExtraParameters = request.FrontChannelExtraParameters
        }, cancellationToken);

        if (authorizeResult.IsError)
        {
            return new LoginResult(authorizeResult.Error, authorizeResult.ErrorDescription);
        }

        LoginResult result = await ProcessResponseAsync(
            authorizeResult.Data,
            authorizeResult.State,
            request.BackChannelExtraParameters,
            cancellationToken);

        if (!result.IsError)
        {
            _logger.LogInformation("Authentication request success.");
        }

        return result;
    }


    public virtual async Task<LogoutResult> LogoutAsync(LogoutRequest? request = null, CancellationToken cancellationToken = default)
    {
        request ??= new LogoutRequest();

        await EnsureConfigurationAsync(cancellationToken);

        BrowserResult result = await _authorizeClient.EndSessionAsync(request, cancellationToken);

        if (result.ResultType != BrowserResultType.Success)
        {
            return new LogoutResult(result.ResultType.ToString()) {
                Response = result.Response
            };
        }

        return new LogoutResult {
            Response = result.Response
        };
    }


    public virtual async Task<AuthorizeState> PrepareLoginAsync(Parameters? frontChannelParameters = null, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("PrepareLoginAsync");

        await EnsureConfigurationAsync(cancellationToken);
        return await _authorizeClient.CreateAuthorizeStateAsync(frontChannelParameters);
    }


    public virtual async Task<String> PrepareLogoutAsync(LogoutRequest? request = null, CancellationToken cancellationToken = default)
    {
        await EnsureConfigurationAsync(cancellationToken);

        if (Options.ProviderInformation == null)
        {
            String error = "Provider Information  is missing";

            _logger.LogError(error);
            throw new InvalidOperationException(error);
        }

        String? endpoint = Options.ProviderInformation.EndSessionEndpoint;

        if (endpoint.IsMissing())
        {
            throw new InvalidOperationException("Discovery document has no end session endpoint");
        }

        return _authorizeClient.CreateEndSessionUrl(endpoint, request);
    }


    public virtual async Task<LoginResult> ProcessResponseAsync(
        String data,
        AuthorizeState state,
        Parameters? backChannelParameters = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("ProcessResponseAsync");
        _logger.LogInformation("Processing response.");

        backChannelParameters = backChannelParameters ?? new Parameters();
        await EnsureConfigurationAsync(cancellationToken);

        _logger.LogDebug("Authorize response: {response}", data);
        AuthorizeResponse authorizeResponse = new AuthorizeResponse(data);

        if (authorizeResponse.IsError)
        {
            _logger.LogError(authorizeResponse.Error);
            return new LoginResult(authorizeResponse.Error, authorizeResponse.ErrorDescription);
        }

        ResponseValidationResult result = await _processor.ProcessResponseAsync(authorizeResponse, state, backChannelParameters, cancellationToken);

        if (result.IsError)
        {
            _logger.LogError(result.Error!);
            return new LoginResult(result.Error ?? String.Empty, result.ErrorDescription ?? String.Empty);
        }

        IEnumerable<Claim> userInfoClaims = Enumerable.Empty<Claim>();

        if (Options.LoadProfile)
        {
            UserInfoResult userInfoResult = await GetUserInfoAsync(result.TokenResponse!.AccessToken!, cancellationToken);

            if (userInfoResult.IsError)
            {
                String error = $"Error contacting userinfo endpoint: {userInfoResult.Error}";
                _logger.LogError(error);

                return new LoginResult(error);
            }

            userInfoClaims = userInfoResult.Claims ?? new List<Claim>();

            Claim? userInfoSub = userInfoClaims.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject);

            if (userInfoSub == null)
            {
                String error = "sub claim is missing from userinfo endpoint";
                _logger.LogError(error);

                return new LoginResult(error);
            }

            if (String.IsNullOrWhiteSpace(result.TokenResponse.IdentityToken))
            {
                if (!String.Equals(userInfoSub.Value, result.User.FindFirst(JwtClaimTypes.Subject).Value))
                {
                    String error = "sub claim from userinfo endpoint is different than sub claim from identity token.";
                    _logger.LogError(error);

                    return new LoginResult(error);
                }
            }
        }

        String? authTimeValue = result.User.FindFirst(JwtClaimTypes.AuthenticationTime)?.Value;
        DateTimeOffset? authTime = null;

        if (authTimeValue.IsPresent() && Int64.TryParse(authTimeValue, out Int64 seconds))
        {
            authTime = DateTimeOffset.FromUnixTimeSeconds(seconds);
        }

        ClaimsPrincipal user = ProcessClaims(result.User, userInfoClaims);

        LoginResult loginResult = new LoginResult {
            User = user,
            AccessToken = result.TokenResponse!.AccessToken!,
            RefreshToken = result.TokenResponse.RefreshToken!,
            AccessTokenExpiration = DateTimeOffset.Now.AddSeconds(result.TokenResponse.ExpiresIn),
            IdentityToken = result.TokenResponse!.IdentityToken!,
            AuthenticationTime = authTime,
            TokenResponse = result.TokenResponse
        };

        if (loginResult.RefreshToken.IsPresent())
        {
            loginResult.RefreshTokenHandler = new RefreshTokenDelegatingHandler(
                this,
                loginResult.AccessToken,
                loginResult.RefreshToken,
                loginResult.TokenResponse.TokenType,
                Options.RefreshTokenInnerHttpHandler);
        }

        return loginResult;
    }


    public virtual async Task<RefreshTokenResult> RefreshTokenAsync(
        String refreshToken,
        Parameters? backChannelParameters = null,
        String? scope = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("RefreshTokenAsync");

        await EnsureConfigurationAsync(cancellationToken);
        backChannelParameters = backChannelParameters ?? new Parameters();

        HttpClient client = Options.CreateClient();

        TokenResponse response = await client.RequestRefreshTokenAsync(new RefreshTokenRequest {
            Address = Options.ProviderInformation.TokenEndpoint,
            ClientId = Options.ClientId,
            ClientSecret = Options.ClientSecret,
            ClientAssertion = await Options.GetClientAssertionAsync(),
            ClientCredentialStyle = Options.TokenClientCredentialStyle,
            RefreshToken = refreshToken,
            Parameters = backChannelParameters,
            Scope = scope
        }, cancellationToken).ConfigureAwait(false);

        if (response.IsError)
        {
            return new RefreshTokenResult {
                Error = response.Error,
                ErrorDescription = response.ErrorDescription
            };
        }

        TokenResponseValidationResult validationResult = await _processor.ValidateTokenResponseAsync(response, null, Options.Policy.RequireIdentityTokenOnRefreshTokenResponse, cancellationToken);

        if (validationResult.IsError)
        {
            return new RefreshTokenResult { Error = validationResult.Error };
        }

        return new RefreshTokenResult {
            IdentityToken = response.IdentityToken,
            AccessToken = response.AccessToken,
            RefreshToken = response.RefreshToken,
            ExpiresIn = response.ExpiresIn,
            AccessTokenExpiration = DateTimeOffset.Now.AddSeconds(response.ExpiresIn)
        };
    }

    #endregion

    #endregion
}