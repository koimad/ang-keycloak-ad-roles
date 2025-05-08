using System.Net;

using AngApp.Server.Models.Events;
using AngApp.Server.Models.Extensions;
using AngApp.Server.Models.User;

using AuthorisationPolicies;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Yarp.ReverseProxy.Transforms;
using Microsoft.AspNetCore.Http;

namespace AngApp.Server;

public class Program
{
    #region Members

    private static readonly String _defaultSchemaName = "Keycloak";

    #endregion

    #region Methods

    private static String ValidateUri(HttpContext httpContext, String? uri)
    {
        String basePath = String.IsNullOrEmpty(httpContext.Request.PathBase)
            ? "/"
            : httpContext.Request.PathBase;

        if (String.IsNullOrEmpty(uri))
        {
            return basePath;
        }

        if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
        {
            return new Uri(uri, UriKind.Absolute).PathAndQuery;
        }

        if (uri[0] != '/')
        {
            return $"{basePath}{uri}";
        }

        return uri;
    }


    #region Public



    public static void Main(String[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        builder.Services.AddScoped<CustomTokenStorageOidcEvents>();
        builder.Services.AddScoped<CustomCookieOptionsEvents>();

        builder.Services.AddHttpContextAccessor();

        builder.Services.AddAntiforgery();
        
        builder.Services
            .AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = _defaultSchemaName;
            })
            .AddOpenIdConnect(_defaultSchemaName, oidcOptions =>
            {
                oidcOptions.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                oidcOptions.Authority = "http://localhost:8080/realms/Aspirations";
                oidcOptions.MetadataAddress = "http://localhost:8080/realms/Aspirations/.well-known/openid-configuration";
                oidcOptions.ResponseType = OpenIdConnectResponseType.Code;
                oidcOptions.Scope.Add(OpenIdConnectScope.OpenIdProfile);
                oidcOptions.Scope.Add("roles");

                oidcOptions.UsePkce = true;
                oidcOptions.ClientId = "aspire-client";
                oidcOptions.ClientSecret = "JjBVXaN6yedHopvSPMbbMPaFdg2usL9w";

                oidcOptions.CallbackPath = new PathString("/signin-oidc");
                oidcOptions.SignedOutCallbackPath = new PathString("/signout-callback-oidc");
                oidcOptions.MapInboundClaims = false;

                oidcOptions.TokenValidationParameters.NameClaimType = JwtRegisteredClaimNames.Name;
                oidcOptions.TokenValidationParameters.RoleClaimType = "roles";

                oidcOptions.RequireHttpsMetadata = false;
                oidcOptions.EventsType = typeof(CustomTokenStorageOidcEvents);

                oidcOptions.GetClaimsFromUserInfoEndpoint = true;
                oidcOptions.ClaimActions.MapJsonKey("roles", "roles");
                oidcOptions.SaveTokens = true;

            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, cookieOptions =>
            {
                cookieOptions.Cookie.Name = "Keycloak";
                cookieOptions.Cookie.MaxAge = TimeSpan.FromMinutes(60);
                cookieOptions.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                cookieOptions.SlidingExpiration = true;
                cookieOptions.AccessDeniedPath = new PathString("/AccessDenied");
                cookieOptions.LogoutPath = new PathString("/signout-callback-oidc");
                cookieOptions.EventsType = typeof(CustomCookieOptionsEvents);
            });

        builder.Services.ConfigureCookieOidcRefresh(CookieAuthenticationDefaults.AuthenticationScheme, _defaultSchemaName);


        builder.Services.AddReverseProxy()
            .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
            .AddTransforms(transformBuilder =>
            {

                transformBuilder.AddPathPrefix("/externalapi");

                transformBuilder.AddRequestTransform(async transformContext =>
                {
                    String? accessToken = await transformContext.HttpContext.GetTokenAsync("access_token");
                    transformContext.ProxyRequest.Headers.Authorization = new("Bearer", accessToken);

                });

                transformBuilder.AddResponseTransform(async transformContext =>
                {
                    if (transformContext.ProxyResponse.StatusCode == HttpStatusCode.Unauthorized || transformContext.ProxyResponse.StatusCode == HttpStatusCode.Forbidden)
                    {
                        transformContext.HttpContext.Response.StatusCode = (Int32)HttpStatusCode.Unauthorized;
                    }
                });
            });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: "All",
                policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        ;
                });
        });

        WebApplication app = builder.Build();

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.UseAntiforgery();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.MapGet("/login", (String? returnUrl, HttpContext httpContext) =>
        {
            // ensure the returnUrl is valid & safe.  
            returnUrl = ValidateUri(httpContext, returnUrl);

            ChallengeHttpResult t = TypedResults.Challenge(new AuthenticationProperties { RedirectUri = returnUrl });

            return t;

        }).AllowAnonymous();

        app.MapGet("/logout", (String? returnUrl, HttpContext httpContext) =>
        {
            returnUrl = ValidateUri(httpContext, returnUrl);

            return TypedResults.SignOut(
                new AuthenticationProperties
                    { RedirectUri = returnUrl },
                [CookieAuthenticationDefaults.AuthenticationScheme, _defaultSchemaName]);
        });

        app.MapGet("/userinfo", (HttpContext httpContext) =>
        {
            UserProfile userProfile = new UserProfile(httpContext.User.FindFirst(JwtRegisteredClaimNames.Name)!.Value,
                httpContext.User.FindFirst(JwtRegisteredClaimNames.Email)?.Value,
                httpContext.User.Claims
                    .Where(c => c.Type == "roles")
                    .Select(c => c.Value)
                    .ToList(),
                httpContext.User.FindFirst(JwtRegisteredClaimNames.UniqueName)?.Value);
            
            return userProfile; ;

        }).RequireAuthorization();

        app.MapReverseProxy();

        app.UseCors("All");

        app.Run();
    }

    #endregion

    #endregion
}