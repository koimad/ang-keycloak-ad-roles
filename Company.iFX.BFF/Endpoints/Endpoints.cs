using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Company.iFX.BFF.Endpoints;

internal static class Endpoints
{
    #region Methods

    #region Public

    public static void MapAuthenticationEndpoints(this WebApplication app, String endpointName)
    {
        app.MapGet($"/{endpointName}/profile", ProfileEndpoint.Get);

        app.MapGet($"/{endpointName}/login", LoginEndpoint.Get);

        app.MapGet($"/{endpointName}/login/callback", CallbackEndpoint.Get);

        app.MapGet($"/{endpointName}/login/callback/error", () => Results.Text("Login failed."));

        app.MapGet($"/{endpointName}/logout", LogoutEndpoint.Get);
    }

    #endregion

    #endregion
}