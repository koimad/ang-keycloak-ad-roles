using Microsoft.AspNetCore.Authorization;

namespace AuthorisationPolicies;

public class Policies
{
    public const String IsFromUK = "IsFromUnitiedKingdom";


    public static AuthorizationPolicy IsFromUnitiedKingdomPolicy()
    {
        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireRole("aspire-editor")
            //.RequireClaim("country", "United Kingdom")
            .Build();
    }


    public static AuthorizationPolicy RequiresModelsRolePolicy()
    {
        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireRole("aspire-editor")
            .Build();
    }

    public static AuthorizationPolicy RequiresModelsPolicy(String number)
    {
        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireRole($"model-{number}")
            .Build();
    }


}