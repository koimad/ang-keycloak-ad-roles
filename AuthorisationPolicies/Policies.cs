using Keycloak.AuthServices.Authorization;

using Microsoft.AspNetCore.Authorization;

namespace AuthorisationPolicies;

public class Policies
{

    public static AuthorizationPolicy RequiresRealmModelsPolicy(String number)
    {
        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            //.RequireRealmRoles($"model-{number}")
            .RequireRole($"model-{number}")
            .Build();
    }

    public static AuthorizationPolicy RequiresResourceModelsPolicy(String number)
    {
        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            //.RequireResourceRolesForClient("aspire-client", new[] {$"model-{number}"})
            .RequireRole($"model-{number}")
            .Build();
    }

}