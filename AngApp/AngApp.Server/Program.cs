using System.Net;

using AngApp.Server.Models.Events;
using AngApp.Server.Models.Extensions;

using AuthorisationPolicies;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Yarp.ReverseProxy.Transforms;

namespace AngApp.Server;

public class Program
{
    #region Members

    private static readonly String _defaultSchemaName = "Keycloak";

    #endregion

    #region Methods


    #region Public

    public static void Main(String[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        builder.Services.AddScoped<CustomTokenStorageOidcEvents>();
        builder.Services.AddScoped<CustomCookieOptionsEvents>();

        builder.Services.AddAuthorization();
        
        builder.Services.AddHttpContextAccessor();

        WebApplication app = builder.Build();

        app.UseDefaultFiles();
        app.UseStaticFiles();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment()) { }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.Run();
    }

    #endregion

    #endregion
}