using Company.iFX.BFF.IdentityModel.Client.Extensions;
using Company.iFX.BFF.IdentityModel.Client.Messages;

namespace Company.iFX.BFF.IdentityModel.Client;

public class TokenClient
{
    #region Members

    private readonly Func<HttpMessageInvoker> _client;
    private readonly TokenClientOptions _options;

    #endregion

    #region Constructors

    public TokenClient(HttpMessageInvoker client, TokenClientOptions options) : this(() => client, options) { }


    public TokenClient(Func<HttpMessageInvoker> client, TokenClientOptions options)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    #endregion

    #region Methods

    #region Internal

    internal void ApplyRequestParameters(TokenRequest request, Parameters? parameters)
    {
        request.Address = _options.Address;
        request.ClientId = _options.ClientId;
        request.ClientSecret = _options.ClientSecret!;
        request.ClientAssertion = _options.ClientAssertion!;
        request.ClientCredentialStyle = _options.ClientCredentialStyle;
        request.AuthorizationHeaderStyle = _options.AuthorizationHeaderStyle;
        request.Parameters = new Parameters(_options.Parameters);

        if (parameters != null)
        {
            foreach (KeyValuePair<String, String> parameter in parameters)
            {
                request.Parameters.Add(parameter);
            }
        }
    }

    #endregion

    #region Public

    public Task<TokenResponse> RequestAuthorizationCodeTokenAsync(String code, String redirectUri, String? codeVerifier = null, Parameters? parameters = null, CancellationToken cancellationToken = default)
    {
        AuthorizationCodeTokenRequest request = new AuthorizationCodeTokenRequest {
            Code = code,
            RedirectUri = redirectUri,
            CodeVerifier = codeVerifier
        };
        ApplyRequestParameters(request, parameters);

        return _client().RequestAuthorizationCodeTokenAsync(request, cancellationToken);
    }


    public Task<TokenResponse> RequestClientCredentialsTokenAsync(String? scope = null, Parameters? parameters = null, CancellationToken cancellationToken = default)
    {
        ClientCredentialsTokenRequest request = new ClientCredentialsTokenRequest {
            Scope = scope
        };
        ApplyRequestParameters(request, parameters);

        return _client().RequestClientCredentialsTokenAsync(request, cancellationToken);
    }


    public Task<TokenResponse> RequestDeviceTokenAsync(String deviceCode, Parameters? parameters = null, CancellationToken cancellationToken = default)
    {
        DeviceTokenRequest request = new DeviceTokenRequest {
            DeviceCode = deviceCode
        };
        ApplyRequestParameters(request, parameters);

        return _client().RequestDeviceTokenAsync(request, cancellationToken);
    }


    public Task<TokenResponse> RequestPasswordTokenAsync(String userName, String? password = null, String? scope = null, Parameters? parameters = null, CancellationToken cancellationToken = default)
    {
        PasswordTokenRequest request = new PasswordTokenRequest {
            UserName = userName,
            Password = password,
            Scope = scope
        };
        ApplyRequestParameters(request, parameters);

        return _client().RequestPasswordTokenAsync(request, cancellationToken);
    }


    public Task<TokenResponse> RequestRefreshTokenAsync(String refreshToken, String? scope = null, Parameters? parameters = null, CancellationToken cancellationToken = default)
    {
        RefreshTokenRequest request = new RefreshTokenRequest {
            RefreshToken = refreshToken,
            Scope = scope
        };
        ApplyRequestParameters(request, parameters);

        return _client().RequestRefreshTokenAsync(request, cancellationToken);
    }


    public Task<TokenResponse> RequestTokenAsync(String grantType, Parameters? parameters = null, CancellationToken cancellationToken = default)
    {
        TokenRequest request = new TokenRequest {
            GrantType = grantType
        };
        ApplyRequestParameters(request, parameters);

        return _client().RequestTokenAsync(request, cancellationToken);
    }

    #endregion

    #endregion
}