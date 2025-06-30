using Company.iFX.BFF.IdentityProviders;
using Company.iFX.BFF.ModuleInitializers;
using Company.iFX.BFF.OpenIdConnect;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Company.iFX.BFF.Endpoints;

public static class LogoutEndpoint
{
    #region Methods

    #region Public

    public static async Task<IResult> Get(HttpContext context,
        [FromServices] AuthSession authSession,
        [FromServices] ILoggerFactory loggerFactory,
        [FromServices] ProxyOptions proxyOptions,
        [FromServices] IRedirectUriFactory redirectUriFactory,
        [FromServices] IIdentityProvider identityProvider)
    {
        ILogger logger = loggerFactory.CreateLogger(typeof(LogoutEndpoint));

        if (!authSession.HasAccessToken())
        {
            return Results.BadRequest();
        }

        logger.LogInformation("Revoking access_token.");
        String? accessToken = context.Session.GetAccessToken();

        try
        {
            await identityProvider.RevokeAsync(accessToken!, context.TraceIdentifier);
        }
        catch (Exception e) when (e is ApplicationException || e is ArgumentException)
        {
            logger.LogWarning($"Unexpected: Failed to revoke access_token during end-session. Access_token will be removed from the Company.iFX.BFF http-session. " +
                                   $"This event is only visible in the logs. The following error occurred: {e}");
        }

        logger.LogInformation("Revoking refresh_token.");
        String? refreshToken = authSession.GetRefreshToken();

        try
        {
            await identityProvider.RevokeAsync(refreshToken!, context.TraceIdentifier);
        }
        catch (Exception e) when (e is ApplicationException || e is ArgumentException)
        {
            logger.LogWarning($"Unexpected: Failed to revoke refresh_token during end-session. Refresh_token will be removed from the Company.iFX.BFF http-session. " +
                                   $"This event is only visible in the logs. The following error occurred: {e}");
        }

        String? idToken = null;

        if (authSession.HasIdToken())
        {
            idToken = authSession.GetIdToken();
        }

        context.Session.Clear();
        context.Response.Cookies.Delete(proxyOptions.CookieName);

        String baseAddress = $"{redirectUriFactory.DetermineHostName(context)}";

        Uri endSessionEndpoint = await identityProvider.GetEndSessionEndpointAsync(idToken, baseAddress);

        logger.LogInformation($"Redirect to {endSessionEndpoint}");

        return Results.Redirect(endSessionEndpoint.ToString());
    }

    #endregion

    #endregion
}