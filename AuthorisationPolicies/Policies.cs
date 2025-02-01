using Keycloak.AuthServices.Authorization;

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

    public static AuthorizationPolicy RequiresRealmModelsPolicy(String number)
    {
        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireRealmRoles($"model-{number}")
            //.RequireRole($"model-{number}")
            .Build();
    }

    public static AuthorizationPolicy RequiresResourceModelsPolicy(String number)
    {
        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireResourceRolesForClient("aspire-client", new[] {$"model-{number}"})
            //.RequireRole($"model-{number}")
            .Build();
    }

}