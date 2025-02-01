using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

using AuthorisationPolicies;

using BlazorOpenIdConnect.Client.Models;

using Keycloak.AuthServices.Authentication;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

using Keycloak.AuthServices.Authentication;
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

        // Add services to the container.
        builder.Services.AddAuthorization()
            .AddKeycloakAuthorization()
            ;
        
        builder.Services.AddKeycloakWebApiAuthentication(options =>
        {
            options.AuthServerUrl = "http://localhost:8080";
            options.Realm = "Aspirations";
            options.Resource = "aspire-client";
            options.SslRequired = "none";
            options.VerifyTokenAudience = true;
        }, jwtOptions =>
        {
            jwtOptions.TokenValidationParameters.ValidateIssuerSigningKey = true;
            jwtOptions.TokenValidationParameters.ValidateLifetime = true;
            
            jwtOptions.Events = new JwtBearerEvents()
            {
                OnForbidden = context =>
                {
                    // add headers since the default middleware does not add them
                    context.Response.Headers.Append("Access-Control-Allow-Origin", $"{context.Request.Headers["Origin"]}");
                    return Task.CompletedTask;
                }
            };
        });

        //builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        //    .AddJwtBearer(options =>
        //    {
        //        options.Authority = "http://localhost:8080/realms/Aspirations";
        //        options.MetadataAddress = "http://localhost:8080/realms/Aspirations/.well-known/openid-configuration";
        //        options.RequireHttpsMetadata = false;
        //        options.TokenValidationParameters.NameClaimType = ClaimTypes.Name;
        //        options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
        //        options.TokenValidationParameters.ValidateIssuer = true;
        //        options.TokenValidationParameters.ValidateIssuerSigningKey = true;
        //        options.TokenValidationParameters.ValidateAudience = true;
        //        options.TokenValidationParameters.ValidateLifetime = true;
        //        options.Audience = "aspire-client";
        //        //options.MapInboundClaims = true;

        //        options.Events = new JwtBearerEvents() {
        //            OnForbidden = context =>
        //            {
        //                // add headers since the default middleware does not add them
        //                context.Response.Headers.Append("Access-Control-Allow-Origin", $"{context.Request.Headers["Origin"]}");
        //                return Task.CompletedTask;
        //            }
        //        };
        //    });

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
        
        app.UseCors("All");

        app.MapGet("externalapi/bands", (HttpContext httpContext) => Results.Ok(GetBands()))
            .RequireAuthorization(Policies.RequiresModelsRolePolicy());

        app.MapGet("externalapi/model1", (HttpContext httpContext) =>
            {
                return Results.Ok($"Hello {httpContext?.User?.Identity?.Name} from Model 1, the time is {DateTime.Now:F}");
            })
            .RequireAuthorization(Policies.RequiresRealmModelsPolicy("one"))
            ;

        app.MapGet("externalapi/model2", (HttpContext httpContext) => Results.Ok($"Hello {httpContext?.User?.Identity?.Name} from Model 2, the time is {DateTime.Now:F}"))
            .RequireAuthorization(Policies.RequiresRealmModelsPolicy("two"))
            ;

        app.MapGet("externalapi/model3", (HttpContext httpContext) => Results.Ok($"Hello {httpContext?.User?.Identity?.Name} from Model 3, the time is {DateTime.Now:F}"))
            .RequireAuthorization(Policies.RequiresResourceModelsPolicy("three"))
            ;

        app.MapGet("externalapi/model4", (HttpContext httpContext) => Results.Ok($"Hello {httpContext?.User?.Identity?.Name} from Model 4, the time is {DateTime.Now:F}"))
            .RequireAuthorization(Policies.RequiresResourceModelsPolicy("four"))
            ;

        app.Run();
    }

    #endregion

    #endregion
}