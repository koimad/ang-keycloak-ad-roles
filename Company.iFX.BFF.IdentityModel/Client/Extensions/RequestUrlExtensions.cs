using System.Text;

using Company.iFX.BFF.IdentityModel.Client.Messages;

namespace Company.iFX.BFF.IdentityModel.Client.Extensions;

public static class RequestUrlExtensions
{
    #region Methods

    #region Public

    public static String Create(this RequestUrl request, Parameters parameters)
    {
        return request.Create(parameters);
    }


    public static String CreateAuthorizeUrl(this RequestUrl request,
        String clientId,
        String? responseType = null,
        String? scope = null,
        String? redirectUri = null,
        String? state = null,
        String? nonce = null,
        String? loginHint = null,
        String? acrValues = null,
        String? prompt = null,
        String? responseMode = null,
        String? codeChallenge = null,
        String? codeChallengeMethod = null,
        String? display = null,
        Int32? maxAge = null,
        String? uiLocales = null,
        String? idTokenHint = null,
        String? requestUri = null,
        Parameters? extra = null)
    {
        Parameters values = new Parameters { { OidcConstants.AuthorizeRequest.ClientId, clientId } };

        if (requestUri.IsPresent())
        {
            values.AddRequired(OidcConstants.AuthorizeRequest.RequestUri, requestUri);
        }
        else
        {
            values.AddRequired(OidcConstants.AuthorizeRequest.ResponseType, responseType);
            values.AddOptional(OidcConstants.AuthorizeRequest.Scope, scope);
            values.AddOptional(OidcConstants.AuthorizeRequest.RedirectUri, redirectUri);
            values.AddOptional(OidcConstants.AuthorizeRequest.State, state);
            values.AddOptional(OidcConstants.AuthorizeRequest.Nonce, nonce);
            values.AddOptional(OidcConstants.AuthorizeRequest.LoginHint, loginHint);
            values.AddOptional(OidcConstants.AuthorizeRequest.AcrValues, acrValues);
            values.AddOptional(OidcConstants.AuthorizeRequest.Prompt, prompt);
            values.AddOptional(OidcConstants.AuthorizeRequest.ResponseMode, responseMode);
            values.AddOptional(OidcConstants.AuthorizeRequest.CodeChallenge, codeChallenge);
            values.AddOptional(OidcConstants.AuthorizeRequest.CodeChallengeMethod, codeChallengeMethod);
            values.AddOptional(OidcConstants.AuthorizeRequest.Display, display);
            values.AddOptional(OidcConstants.AuthorizeRequest.MaxAge, maxAge?.ToString());
            values.AddOptional(OidcConstants.AuthorizeRequest.UiLocales, uiLocales);
            values.AddOptional(OidcConstants.AuthorizeRequest.IdTokenHint, idTokenHint);
            values.AddOptional(OidcConstants.AuthorizeRequest.RequestUri, requestUri);
        }

        return request.Create(values.Merge(extra));
    }


    public static String CreateEndSessionUrl(this RequestUrl request,
        String? idTokenHint = null,
        String? postLogoutRedirectUri = null,
        String? state = null,
        Parameters? extra = null)
    {
        Parameters values = new Parameters();

        values.AddOptional(OidcConstants.EndSessionRequest.IdTokenHint, idTokenHint);
        values.AddOptional(OidcConstants.EndSessionRequest.PostLogoutRedirectUri, postLogoutRedirectUri);
        values.AddOptional(OidcConstants.EndSessionRequest.State, state);

        return request.Create(values.Merge(extra));
    }

    #endregion

    #endregion
}