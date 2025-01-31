using AngApp.Server.Models.TokenManagement;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace AngApp.Server.Models.Extensions;

public static class CookieOidcServiceCollectionExtensions
{
    #region Methods

    #region Public

    public static IServiceCollection ConfigureCookieOidcRefresh(this IServiceCollection services, String cookieScheme, String oidcScheme)
    {
        services.AddSingleton<CookieOidcRefresher>();

        services.AddOptions<CookieAuthenticationOptions>(cookieScheme).Configure<CookieOidcRefresher>((cookieOptions, refresher) =>
        {
            cookieOptions.Events.OnValidatePrincipal = context =>
            {
                return refresher.ValidateOrRefreshCookieAsync(context, oidcScheme);
            };
        });

        services.AddOptions<OpenIdConnectOptions>(oidcScheme).Configure(oidcOptions =>
        {
            // Request a refresh_token.
            oidcOptions.Scope.Add(OpenIdConnectScope.OfflineAccess);
            // Store the refresh_token.
            oidcOptions.SaveTokens = true;
        });
        return services;
    }

    #endregion

    #endregion
}