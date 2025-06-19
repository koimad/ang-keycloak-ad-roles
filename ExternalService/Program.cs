using System.Security.Claims;

using AuthorisationPolicies;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace ExternalService;

public class Program
{
    #region Methods

    #region Private

    private static Boolean OnValidateLifeTime(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
    {
        return true;
    }

    #endregion

    #region Public

    public static Func<HttpContext, String?> ForwardReferenceToken(String introspectionScheme = "introspection")
    {
        String? Select(HttpContext context)
        {
            (String scheme, String credential) = GetSchemeAndCredential(context);

            if (scheme.Equals("Bearer", StringComparison.OrdinalIgnoreCase) && !credential.Contains("."))
            {
                return introspectionScheme;
            }

            return null;
        }

        return Select;
    }


    public static (String, String) GetSchemeAndCredential(HttpContext context)
    {
        String? header = context.Request.Headers["Authorization"].FirstOrDefault();

        if (String.IsNullOrEmpty(header))
        {
            return ("", "");
        }

        String[] parts = header.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
        {
            return ("", "");
        }

        return (parts[0], parts[1]);
    }


    public static void Main(String[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddAuthorization()
            .AddAuthorizationBuilder();

        builder.Services.AddAuthentication(option => { option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; })
            .AddJwtBearer(options =>
            {
                options.Authority = "http://localhost:8080/realms/Aspirations";
                options.MetadataAddress = "http://localhost:8080/realms/Aspirations/.well-known/openid-configuration";
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters.NameClaimType = ClaimTypes.Name;
                options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
                options.TokenValidationParameters.ValidateIssuer = true;
                options.TokenValidationParameters.ValidIssuer = "http://localhost:8080/realms/Aspirations";
                options.TokenValidationParameters.ValidateIssuerSigningKey = true;
                options.TokenValidationParameters.ValidTypes = new[] { "JWT" };
                
                options.TokenValidationParameters.ValidAudiences = new[] { "aspire-client" };
                options.TokenValidationParameters.ValidateAudience = true;
                options.TokenValidationParameters.ValidateLifetime = true;
                options.TokenValidationParameters.LifetimeValidator = OnValidateLifeTime;
                options.TokenValidationParameters.ClockSkew = TimeSpan.Zero;

                //options.ForwardDefaultSelector = ForwardReferenceToken("Introspection");

                options.Audience = "aspire-client";
                options.MapInboundClaims = true;

                options.Events = new JwtBearerEvents {
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
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine(context.SecurityToken.ValidTo);
                        return Task.CompletedTask;
                    }
                };
            })
            //.AddOAuth2Introspection("Introspection", options =>
            //{
            //    options.Authority = "http://localhost:8080/realms/Aspirations";
            //    options.ClientId = "aspire-client";
            //    options.ClientSecret = "5DljDk3brbgltSEB3xQwxWhyxIU9iQD2";
            //    options.SkipTokensWithDots = false;
            //    options.RoleClaimType = "roles"; // ClaimTypes.Role;
            //    options.NameClaimType = "name"; // ClaimTypes.Name;
            //    options.SaveToken = true;

            //    options.Events = new OAuth2IntrospectionEvents {
            //        OnSendingRequest = context =>
            //        {
            //            Console.WriteLine(context.TokenIntrospectionRequest.Token);
            //            return Task.CompletedTask;
            //        },
            //        OnAuthenticationFailed = context =>
            //        {
            //            Console.WriteLine(context.Error);
            //            return Task.CompletedTask;
            //        },
            //        OnTokenValidated = context =>
            //        {
            //            Console.WriteLine(context.SecurityToken);
            //            return Task.CompletedTask;
            //        }
            //    };
            //})
            ;

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("All",
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