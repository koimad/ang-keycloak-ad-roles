using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using AuthorisationPolicies;

using BlazorOpenIdConnect.Client.Models;

using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace ExternalService;

public class Program
{
    #region Methods

    #region Public

    public static IEnumerable<Band> GetBands()
    {
        return [
            new Band(1, "Nirvana (from external API)"),
            new Band(2, "Queens of the Stone Age (from external API)"),
            new Band(3, "Fred Again. (from external API)"),
            new Band(4, "Underworld (from external API)")
        ];
    }


    public static void Main(String[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "http://localhost:8080/realms/Aspirations";
                options.MetadataAddress = "http://localhost:8080/realms/Aspirations/.well-known/openid-configuration";
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters.NameClaimType = ClaimTypes.Name;
                options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
                options.TokenValidationParameters.ValidateIssuer = true;
                options.TokenValidationParameters.ValidateAudience = true;
                options.TokenValidationParameters.ValidateLifetime = true;
                options.Audience = "aspire-client";
                options.MapInboundClaims = true;
            });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: "All",
                policy =>
                {
                    policy.WithOrigins("https://127.0.0.1:7297", "https://127.0.0.1:52947", "https://127.0.0.1:7052")
                        .WithMethods("GET")
                        .WithHeaders("authorization")
                        ;
                });
        });

 
        WebApplication app = builder.Build();

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.UseCors("All");

        app.MapGet("externalapi/bands", (HttpContext httpContext) => Results.Ok(GetBands()))
            .RequireAuthorization(Policies.RequiresModelsRolePolicy());

        app.MapGet("externalapi/model1", (HttpContext httpContext) => Results.Ok($"Hello From Model 1, the time is {DateTime.Now:F}"))
            .RequireAuthorization()
            ;

        app.MapGet("externalapi/model2", (HttpContext httpContext) => Results.Ok($"Hello From Model 2, the time is {DateTime.Now:F}"))
            .RequireAuthorization(Policies.RequiresModelsPolicy("two"))
            ;

        app.MapGet("externalapi/model3", (HttpContext httpContext) => Results.Ok($"Hello From Model 3, the time is {DateTime.Now:F}"))
            .RequireAuthorization(Policies.RequiresModelsPolicy("three"))
            ;

        app.MapGet("externalapi/model4", (HttpContext httpContext) => Results.Ok($"Hello From Model 4, the time is {DateTime.Now:F}"))
            .RequireAuthorization(Policies.RequiresModelsPolicy("four"))
            ;

        app.Run();
    }

    #endregion

    #endregion
}