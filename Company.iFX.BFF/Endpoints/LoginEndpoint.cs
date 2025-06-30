using Company.iFX.BFF.IdentityProviders;
using Company.iFX.BFF.ModuleInitializers;
using Company.iFX.BFF.OpenIdConnect;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Company.iFX.BFF.Endpoints;

public static class LoginEndpoint
{
    #region Methods

    #region Public

    public static async Task Get(HttpContext context,
        [FromServices] EndpointName endpointName,
        [FromServices] AuthSession authSession,
        [FromServices] ILoggerFactory loggerFactory,
        [FromServices] IRedirectUriFactory redirectUriFactory,
        [FromServices] IIdentityProvider identityProvider)
    {
        ILogger logger = loggerFactory.CreateLogger(typeof(LoginEndpoint));

        try
        {
            String redirectUri = redirectUriFactory.DetermineRedirectUri(context, endpointName.ToString());

            AuthorizeRequest authorizeRequest = await identityProvider.GetAuthorizeUrlAsync(redirectUri);

            if (!String.IsNullOrEmpty(authorizeRequest.CodeVerifier))
            {
                await authSession.SetCodeVerifierAsync(authorizeRequest.CodeVerifier);
            }

            logger.LogInformation($"Redirect({authorizeRequest.AuthorizeUri})");

            context.Response.Redirect(authorizeRequest.AuthorizeUri.ToString());
        }
        catch (Exception e)
        {
            logger.LogError(e,"Error calling login endpoint");
            throw;
        }
    }

    #endregion

    #endregion
}