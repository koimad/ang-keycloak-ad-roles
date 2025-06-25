using System.Net;
using System.Security.Claims;
using System.Text;

using Company.iFX.BFF.IdentityModel;
using Company.iFX.BFF.IdentityModel.Client.Extensions;
using Company.iFX.BFF.IdentityModel.Client.Messages;
using Company.iFX.BFF.OIDC.Client.Infrastructure;
using Company.iFX.BFF.OIDC.Client.Results;

using Microsoft.Extensions.Logging;

namespace Company.iFX.BFF.OIDC.Client;

public class ResponseProcessor
{
    #region Members

    private readonly CryptoHelper _crypto;
    private readonly ILogger<ResponseProcessor> _logger;
    private readonly OidcClientOptions _options;
    private readonly Func<CancellationToken, Task> _refreshKeysAsync;

    #endregion

    #region Constructors

    public ResponseProcessor(OidcClientOptions options, Func<CancellationToken, Task> refreshKeysAsync)
    {
        _options = options;
        _refreshKeysAsync = refreshKeysAsync;
        _logger = options.LoggerFactory.CreateLogger<ResponseProcessor>();

        _crypto = new CryptoHelper(options);
    }

    #endregion

    #region Methods

    #region Private

    private async Task<ResponseValidationResult> ProcessCodeFlowResponseAsync(
        AuthorizeResponse authorizeResponse,
        AuthorizeState state,
        Parameters backChannelParameters,
        CancellationToken cancellationToken)
    {
        _logger.LogTrace("ProcessCodeFlowResponseAsync");

        // redeem code for tokens
        TokenResponse tokenResponse = await RedeemCodeAsync(authorizeResponse.Code, state, backChannelParameters, cancellationToken);

        if (tokenResponse.IsError)
        {
            return new ResponseValidationResult($"Error redeeming code: {tokenResponse.Error ?? "no error code"} / {tokenResponse.ErrorDescription ?? "no description"}");
        }

        if (tokenResponse.HttpStatusCode != HttpStatusCode.OK)
        {
            return new ResponseValidationResult($"Error redeeming code: {tokenResponse.Raw}");
        }

        // validate token response
        TokenResponseValidationResult? tokenResponseValidationResult = await ValidateTokenResponseAsync(tokenResponse, state, false, cancellationToken);

        if (tokenResponseValidationResult.IsError)
        {
            return new ResponseValidationResult($"Error validating token response: {tokenResponseValidationResult.Error}");
        }

        return new ResponseValidationResult {
            AuthorizeResponse = authorizeResponse,
            TokenResponse = tokenResponse,
            User = tokenResponseValidationResult?.IdentityTokenValidationResult?.User ?? Principal.Create(_options.Authority!)
        };
    }


    private async Task<TokenResponse> RedeemCodeAsync(String code, AuthorizeState state, Parameters backChannelParameters, CancellationToken cancellationToken)
    {
        _logger.LogTrace("RedeemCodeAsync");

        HttpClient client = _options.CreateClient();

        TokenResponse tokenResult = await client.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest {
            Address = _options.ProviderInformation.TokenEndpoint,
            ClientId = _options.ClientId,
            ClientSecret = _options.ClientSecret,
            ClientAssertion = await _options.GetClientAssertionAsync(),
            ClientCredentialStyle = _options.TokenClientCredentialStyle,

            Code = code,
            RedirectUri = state.RedirectUri!,
            CodeVerifier = state.CodeVerifier,
            Parameters = backChannelParameters ?? new Parameters()
        }, cancellationToken).ConfigureAwait(false);

        return tokenResult;
    }

    #endregion

    #region Public

    public async Task<ResponseValidationResult> ProcessResponseAsync(
        AuthorizeResponse authorizeResponse,
        AuthorizeState state,
        Parameters backChannelParameters,
        CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("ProcessResponseAsync");

        if (String.IsNullOrEmpty(authorizeResponse.Code))
        {
            return new ResponseValidationResult("Missing authorization code.");
        }

        if (String.IsNullOrEmpty(authorizeResponse.State))
        {
            return new ResponseValidationResult("Missing state.");
        }

        if (!String.Equals(state.State, authorizeResponse.State, StringComparison.Ordinal))
        {
            return new ResponseValidationResult("Invalid state.");
        }

        return await ProcessCodeFlowResponseAsync(authorizeResponse, state, backChannelParameters, cancellationToken);
    }

#pragma warning disable IDE0060
    public async Task<TokenResponseValidationResult> ValidateTokenResponseAsync(TokenResponse response, AuthorizeState state, Boolean requireIdentityToken, CancellationToken cancellationToken = default)
    {
        _logger.LogTrace("ValidateTokenResponse");

        if (response.AccessToken.IsMissing())
        {
            return new TokenResponseValidationResult("Access token is missing on token response.");
        }

        if (requireIdentityToken)
        {
            if (response.IdentityToken.IsMissing())
            {
                return new TokenResponseValidationResult("Identity token is missing on token response.");
            }
        }

        if (response.IdentityToken.IsPresent())
        {
            IIdentityTokenValidator validator;

            if (_options.IdentityTokenValidator == null)
            {
                if (_options.Policy.RequireIdentityTokenSignature == false)
                {
                    validator = new NoValidationIdentityTokenValidator();
                }
                else
                {
                    throw new InvalidOperationException(
                        "No IIdentityTokenValidator is configured. Either explicitly set a validator on the options, or set OidcClientOptions.Policy.RequireIdentityTokenSignature to false to skip validation.");
                }
            }
            else
            {
                validator = _options.IdentityTokenValidator;
            }

            IdentityTokenValidationResult validationResult = await validator.ValidateAsync(response.IdentityToken, _options, cancellationToken);

            if (validationResult.Error == "invalid_signature")
            {
                await _refreshKeysAsync(cancellationToken);
                validationResult = await _options.IdentityTokenValidator!.ValidateAsync(response.IdentityToken, _options, cancellationToken);
            }

            if (validationResult.IsError)
            {
                return new TokenResponseValidationResult(validationResult.Error ?? "Identity token validation error");
            }

            // validate at_hash
            if (!String.Equals(validationResult.SignatureAlgorithm, "none", StringComparison.OrdinalIgnoreCase))
            {
                Claim? atHash = validationResult.User!.FindFirst(JwtClaimTypes.AccessTokenHash);

                if (atHash == null)
                {
                    if (_options.Policy.RequireAccessTokenHash)
                    {
                        return new TokenResponseValidationResult("at_hash is missing.");
                    }
                }
                else
                {
                    if (!_crypto.ValidateHash(response.AccessToken!, atHash.Value, validationResult.SignatureAlgorithm!))
                    {
                        return new TokenResponseValidationResult("Invalid access token hash.");
                    }
                }
            }

            return new TokenResponseValidationResult(validationResult);
        }

        return new TokenResponseValidationResult(null as IdentityTokenValidationResult);
    }
#pragma warning restore IDE0060
    #endregion

    #endregion
}