using Company.iFX.BFF.IdentityProviders;
using Company.iFX.BFF.Logging;
using Company.iFX.BFF.ModuleInitializers;
using Company.iFX.BFF.OpenIdConnect;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Company.iFX.BFF.Endpoints;

internal static class LogoutEndpoint
{
    #region Methods

    #region Public

    public static async Task<IResult> Get(HttpContext context,
        [FromServices] AuthSession authSession,
        [FromServices] ILogger logger,
        [FromServices] ProxyOptions proxyOptions,
        [FromServices] IRedirectUriFactory redirectUriFactory,
        [FromServices] IIdentityProvider identityProvider)
    {
        if (!authSession.HasAccessToken())
        {
            return Results.BadRequest();
        }

        await logger.InformAsync("Revoking access_token.");
        String? accessToken = context.Session.GetAccessToken();

        try
        {
            await identityProvider.RevokeAsync(accessToken!, context.TraceIdentifier);
        }
        catch (Exception e) when (e is ApplicationException || e is ArgumentException)
        {
            await logger.WarnAsync($"Unexpected: Failed to revoke access_token during end-session. Access_token will be removed from the Company.iFX.BFF http-session. " +
                                   $"This event is only visible in the logs. The following error occurred: {e}");
        }

        await logger.InformAsync("Revoking refresh_token.");
        String? refreshToken = authSession.GetRefreshToken();

        try
        {
            await identityProvider.RevokeAsync(refreshToken!, context.TraceIdentifier);
        }
        catch (Exception e) when (e is ApplicationException || e is ArgumentException)
        {
            await logger.WarnAsync($"Unexpected: Failed to revoke refresh_token during end-session. Refresh_token will be removed from the Company.iFX.BFF http-session. " +
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

        await logger.InformAsync($"Redirect to {endSessionEndpoint}");

        return Results.Redirect(endSessionEndpoint.ToString());
    }

    #endregion

    #endregion
}