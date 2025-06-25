using Company.iFX.BFF.OIDC.Client.Results;

namespace Company.iFX.BFF.OIDC.Client;

public interface IIdentityTokenValidator
{
    Task<IdentityTokenValidationResult> ValidateAsync(String identityToken, OidcClientOptions options, CancellationToken cancellationToken = default);
}