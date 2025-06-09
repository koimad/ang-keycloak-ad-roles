using System.Security.Claims;
using AuthorisationPolicies;
using BlazorOpenIdConnect.Client.Models;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Keycloak.AuthServices.Authorization;

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

        builder.Services.AddAuthorization()
            .AddKeycloakAuthorization(options =>
            {
                //options.EnableRolesMapping = RolesClaimTransformationSource.Realm;
                //options.RoleClaimType = KeycloakConstants.RoleClaimType;
            })
            .AddAuthorizationBuilder();
            
        
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = "http://localhost:8080/realms/Aspirations";
                options.MetadataAddress = "http://localhost:8080/realms/Aspirations/.well-known/openid-configuration";
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters.NameClaimType = ClaimTypes.Name;
                options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
                options.TokenValidationParameters.ValidateIssuer = true;
                options.TokenValidationParameters.ValidateIssuerSigningKey = true;
                options.TokenValidationParameters.ValidateAudience = true;
                options.TokenValidationParameters.ValidateLifetime = true;
                options.Audience = "aspire-client";
                options.MapInboundClaims = true;
                
                options.Events = new JwtBearerEvents()
                {
                    OnForbidden = context =>
                    {
                        // add headers since the default middleware does not add them
                        context.Response.Headers.Append("Access-Control-Allow-Origin", $"{context.Request.Headers["Origin"]}");
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        // add headers since the default middleware does not add them
                        context.Response.Headers.Append("Access-Control-Allow-Origin", $"{context.Request.Headers["Origin"]}");
                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: "All",
                policy =>
                {
                    policy.WithOrigins("https://127.0.0.1:7297", "https://127.0.0.1:52947", "https://127.0.0.1:7052",
                            "https://localhost:7297", "https://localhost:52947", "https://localhost:7052")
                        .WithMethods("GET")
                        .WithHeaders("authorization", "*")
                        ;
                });
        });

        WebApplication app = builder.Build();

        app.UseHttpsRedirection();

        app.UseAuthorization();
        
        app.UseCors("All");

        app.MapGet("externalapi/bands", (HttpContext httpContext) => Results.Ok(GetBands()))
            .RequireAuthorization(Policies.RequiresModelsRolePolicy());

        app.MapGet("externalapi/model1", (HttpContext httpContext) => Results.Ok($"Hello {httpContext?.User?.Identity?.Name} from Model 1, the time is {DateTime.Now:F}"))
            .RequireAuthorization(Policies.RequiresResourceModelsPolicy("one"))
            ;

        app.MapGet("externalapi/model2", (HttpContext httpContext) => Results.Ok($"Hello {httpContext?.User?.Identity?.Name} from Model 2, the time is {DateTime.Now:F}"))
            .RequireAuthorization(Policies.RequiresResourceModelsPolicy("two"))
            ;

        app.MapGet("externalapi/model3", (HttpContext httpContext) => Results.Ok($"Hello {httpContext?.User?.Identity?.Name} from Model 3, the time is {DateTime.Now:F}"))
            .RequireAuthorization(Policies.RequiresRealmModelsPolicy("three"))
            ;

        app.MapGet("externalapi/model4", (HttpContext httpContext) => Results.Ok($"Hello {httpContext?.User?.Identity?.Name} from Model 4, the time is {DateTime.Now:F}"))
            .RequireAuthorization(Policies.RequiresRealmModelsPolicy("four"))
            ;

        app.Run();
    }

    #endregion

    #endregion
}