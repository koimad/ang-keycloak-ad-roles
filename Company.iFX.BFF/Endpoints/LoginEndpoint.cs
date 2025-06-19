using Company.iFX.BFF.IdentityProviders;
using Company.iFX.BFF.Logging;
using Company.iFX.BFF.ModuleInitializers;
using Company.iFX.BFF.OpenIdConnect;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Company.iFX.BFF.Endpoints;

internal static class LoginEndpoint
{
    #region Methods

    #region Private

    private static async Task SaveUserPreferredLandingPage(HttpContext context, AuthSession authSession, LandingPage[] allowedLandingPages)
    {
        String userPreferredLandingPage = context.Request.Query["landingpage"].ToString();

        if (context.Request.Path
            .RemoveQueryString()
            .Equals(userPreferredLandingPage, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new NotSupportedException($"Will not redirect user to {userPreferredLandingPage}");
        }

        if (allowedLandingPages.Any() && !String.IsNullOrEmpty(userPreferredLandingPage) && !allowedLandingPages.Any(x => x.Equals(userPreferredLandingPage)))
        {
            throw new NotSupportedException($"Will not redirect user to {userPreferredLandingPage}");
        }

        await authSession.SetUserPreferredLandingPageAsync(userPreferredLandingPage);
    }

    #endregion

    #region Public

    public static async Task Get(HttpContext context,
        [FromServices] EndpointName endpointName,
        [FromServices] AuthSession authSession,
        [FromServices] ProxyOptions options,
        [FromServices] ILogger logger,
        [FromServices] IRedirectUriFactory redirectUriFactory,
        [FromServices] IIdentityProvider identityProvider)
    {
        try
        {
            if (options.EnableUserPreferredLandingPages || options.AllowedUserPreferredLandingPages.Any())
            {
                try
                {
                    await SaveUserPreferredLandingPage(context, authSession, options.AllowedUserPreferredLandingPages);
                }
                catch (Exception e) when (e is ArgumentException or NotSupportedException)
                {
                    StringValues userPreferredLandingPage = context.Request.Query["landingpage"];

                    await logger.WarnAsync($"Suspicious activity detected. User provided an invalid landing page at " +
                                           $"login. Value provided: \"{userPreferredLandingPage}\"");

                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }
            }

            String redirectUri = redirectUriFactory.DetermineRedirectUri(context, endpointName.ToString());

            AuthorizeRequest authorizeRequest = await identityProvider.GetAuthorizeUrlAsync(redirectUri);

            if (!String.IsNullOrEmpty(authorizeRequest.CodeVerifier))
            {
                await authSession.SetCodeVerifierAsync(authorizeRequest.CodeVerifier);
            }

            await logger.InformAsync($"Redirect({authorizeRequest.AuthorizeUri})");

            context.Response.Redirect(authorizeRequest.AuthorizeUri.ToString());
        }
        catch (Exception e)
        {
            await logger.WarnAsync(e);
            throw;
        }
    }

    #endregion

    #endregion
}