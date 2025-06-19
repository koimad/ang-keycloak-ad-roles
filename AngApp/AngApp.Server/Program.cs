using Company.iFX.BFF.OIDC;

using Company.iFX.BFF.ModuleInitializers;

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
        builder.Services.AddHttpContextAccessor();

        builder.Services.AddAntiforgery();

        OidcProxyConfig? config = builder.Configuration
            .GetSection("OidcProxy")
            .Get<OidcProxyConfig>();

        builder.Services.AddOidcProxy(config);

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

        app.UseOidcProxy()
            .UseHttpsRedirection()
            .UseAntiforgery()
            .UseDefaultFiles()
            .UseStaticFiles()
            .UseCors("All");

        app.Run();
    }

    #endregion

    #endregion
}