using System.Text;

using Company.iFX.BFF.IdentityModel;
using Company.iFX.BFF.IdentityModel.Client;
using Company.iFX.BFF.IdentityModel.Client.Extensions;
using Company.iFX.BFF.IdentityModel.Client.Messages;
using Company.iFX.BFF.OIDC.Client.Browser;
using Company.iFX.BFF.OIDC.Client.Infrastructure;
using Company.iFX.BFF.OIDC.Client.Requests;
using Company.iFX.BFF.OIDC.Client.Results;

using Microsoft.Extensions.Logging;

namespace Company.iFX.BFF.OIDC.Client;

public class AuthorizeClient
{
    #region Members

    private readonly CryptoHelper _crypto;
    private readonly ILogger<AuthorizeClient> _logger;
    private readonly OidcClientOptions _options;

    #endregion

    #region Constructors

    public AuthorizeClient(OidcClientOptions options)
    {
        _options = options;
        _logger = options.LoggerFactory.CreateLogger<AuthorizeClient>();
        _crypto = new CryptoHelper(options);
    }

    #endregion

    #region Methods

    #region Private

    private async Task<PushedAuthorizationResponse> PushAuthorizationRequestAsync(String state, String codeChallenge, Parameters frontChannelParameters)
    {
        HttpClient http = _options.CreateClient();

        PushedAuthorizationRequest par = new PushedAuthorizationRequest {
            Address = _options.ProviderInformation.PushedAuthorizationRequestEndpoint,
            ClientId = _options.ClientId,

            ClientSecret = _options.ClientSecret,
            ClientAssertion = await _options.GetClientAssertionAsync(),
            Parameters = CreateAuthorizeParameters(state, codeChallenge, frontChannelParameters)
        };

        if (par.ClientAssertion?.Value != null)
        {
            par.ClientCredentialStyle = ClientCredentialStyle.PostBody;
        }

        return await http.PushAuthorizationAsync(par);
    }

    #endregion

    #region Public

    public async Task<AuthorizeResult> AuthorizeAsync(AuthorizeRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("AuthorizeAsync");

        if (_options.Browser == null)
        {
            throw new InvalidOperationException("No browser configured.");
        }

        AuthorizeResult result = new AuthorizeResult {
            State = await CreateAuthorizeStateAsync(request.ExtraParameters)
        };

        if (result.State.IsError)
        {
            result.Error = result.State.Error;
            result.ErrorDescription = result.State.ErrorDescription;
            return result;
        }

        BrowserOptions browserOptions = new BrowserOptions(result.State.StartUrl, _options.RedirectUri) {
            Timeout = TimeSpan.FromSeconds(request.Timeout),
            DisplayMode = request.DisplayMode
        };

        BrowserResult browserResult = await _options.Browser.InvokeAsync(browserOptions, cancellationToken);

        if (browserResult.ResultType == BrowserResultType.Success)
        {
            result.Data = browserResult.Response;
            return result;
        }

        result.Error = browserResult.Error ?? browserResult.ResultType.ToString();
        result.ErrorDescription = browserResult.ErrorDescription;
        return result;
    }


    public Parameters CreateAuthorizeParameters(String state, String codeChallenge, Parameters frontChannelParameters)
    {
        _logger.LogTrace("CreateAuthorizeParameters");

        Parameters parameters = new Parameters {
            { OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.Code },
            { OidcConstants.AuthorizeRequest.State, state },
            { OidcConstants.AuthorizeRequest.CodeChallenge, codeChallenge },
            { OidcConstants.AuthorizeRequest.CodeChallengeMethod, OidcConstants.CodeChallengeMethods.Sha256 }
        };

        if (_options.ClientId.IsPresent())
        {
            parameters.Add(OidcConstants.AuthorizeRequest.ClientId, _options.ClientId);
        }

        if (_options.Scope.IsPresent())
        {
            parameters.Add(OidcConstants.AuthorizeRequest.Scope, _options.Scope);
        }

        if (_options.Resource.Any())
        {
            foreach (String resource in _options.Resource)
            {
                parameters.Add(OidcConstants.AuthorizeRequest.Resource, resource);
            }
        }

        if (_options.RedirectUri.IsPresent())
        {
            parameters.Add(OidcConstants.AuthorizeRequest.RedirectUri, _options.RedirectUri);
        }

        if (frontChannelParameters != null)
        {
            foreach (KeyValuePair<String, String> entry in frontChannelParameters)
            {
                parameters.Add(entry.Key, entry.Value);
            }
        }

        return parameters;
    }


    public async Task<AuthorizeState> CreateAuthorizeStateAsync(Parameters? frontChannelParameters)
    {
        _logger.LogTrace("CreateAuthorizeStateAsync");

        Pkce pkce = _crypto.CreatePkceData();

        AuthorizeState state = new AuthorizeState {
            State = _crypto.CreateState(_options.StateLength),
            RedirectUri = _options.RedirectUri,
            CodeVerifier = pkce.CodeVerifier
        };

        if (_options.ProviderInformation.PushedAuthorizationRequestEndpoint.IsPresent() &&
            !_options.DisablePushedAuthorization)
        {
            _logger.LogDebug("The IdentityProvider contains a pushed authorization request endpoint. Automatically pushing authorization parameters. Use DisablePushedAuthorization to opt out.");
            PushedAuthorizationResponse parResponse = await PushAuthorizationRequestAsync(state.State, pkce.CodeChallenge, frontChannelParameters);

            if (parResponse.IsError)
            {
                _logger.LogError("Failed to push authorization parameters");

                state.Error = parResponse.Error;
                state.ErrorDescription = "Failed to push authorization parameters";
                return state;
            }

            state.StartUrl = CreateAuthorizeUrl(parResponse.RequestUri, _options.ClientId);
        }
        else
        {
            state.StartUrl = CreateAuthorizeUrl(state.State, pkce.CodeChallenge, frontChannelParameters);
        }

        _logger.LogDebug(LogSerializer.Serialize(state));

        return state;
    }


    public String CreateAuthorizeUrl(String state, String codeChallenge,
        Parameters frontChannelParameters)
    {
        _logger.LogTrace("CreateAuthorizeUrl");

        Parameters parameters = CreateAuthorizeParameters(state, codeChallenge, frontChannelParameters);
        RequestUrl request = new RequestUrl(_options.ProviderInformation.AuthorizeEndpoint);

        return request.Create(parameters);
    }


    public String CreateAuthorizeUrl(String requestUri, String clientId)
    {
        _logger.LogTrace("CreateAuthorizeUrl with requestUri from PAR");

        Parameters parameters = new Parameters {
            { OidcConstants.AuthorizeRequest.ClientId, clientId },
            { OidcConstants.AuthorizeRequest.RequestUri, requestUri }
        };
        RequestUrl request = new RequestUrl(_options.ProviderInformation.AuthorizeEndpoint);

        return request.Create(parameters);
    }


    public String CreateEndSessionUrl(String endpoint, LogoutRequest request)
    {
        _logger.LogTrace("CreateEndSessionUrl");

        return new RequestUrl(endpoint).CreateEndSessionUrl(
            request.IdTokenHint,
            _options.PostLogoutRedirectUri,
            request.State);
    }


    public async Task<BrowserResult> EndSessionAsync(LogoutRequest request, CancellationToken cancellationToken = default)
    {
        String? endpoint = _options.ProviderInformation.EndSessionEndpoint;

        if (endpoint.IsMissing())
        {
            throw new InvalidOperationException("Discovery document has no end session endpoint");
        }

        String url = CreateEndSessionUrl(endpoint, request);

        BrowserOptions browserOptions = new BrowserOptions(url, _options.PostLogoutRedirectUri ?? String.Empty) {
            Timeout = TimeSpan.FromSeconds(request.BrowserTimeout),
            DisplayMode = request.BrowserDisplayMode
        };

        return await _options.Browser.InvokeAsync(browserOptions, cancellationToken);
    }

    #endregion

    #endregion
}