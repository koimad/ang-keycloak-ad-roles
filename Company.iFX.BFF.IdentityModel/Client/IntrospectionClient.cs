using Company.iFX.BFF.IdentityModel.Client.Extensions;
using Company.iFX.BFF.IdentityModel.Client.Messages;

namespace Company.iFX.BFF.IdentityModel.Client;

public class IntrospectionClient
{
    #region Members

    private readonly Func<HttpMessageInvoker> _client;
    private readonly IntrospectionClientOptions _options;

    #endregion

    #region Constructors

    public IntrospectionClient(HttpMessageInvoker client, IntrospectionClientOptions options) : this(() => client, options) { }


    public IntrospectionClient(Func<HttpMessageInvoker> client, IntrospectionClientOptions options)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    #endregion

    #region Methods

    #region Public

    public void ApplyRequestParameters(TokenIntrospectionRequest request, Parameters? parameters)
    {
        request.Address = _options.Address;
        request.ClientId = _options.ClientId;
        request.ClientSecret = _options.ClientSecret;
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


    public Task<TokenIntrospectionResponse> Introspect(String token, String? tokenTypeHint = null, Parameters? parameters = null, CancellationToken cancellationToken = default)
    {
        TokenIntrospectionRequest request = new TokenIntrospectionRequest {
            Token = token,
            TokenTypeHint = tokenTypeHint
        };
        ApplyRequestParameters(request, parameters);

        return _client().IntrospectTokenAsync(request, cancellationToken);
    }

    #endregion

    #endregion
}