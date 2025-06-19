using Company.iFX.BFF.Cryptography;

namespace Company.iFX.BFF.IdentityProviders;


public interface IIdentityProvider
{
    Task<AuthorizeRequest> GetAuthorizeUrlAsync(String redirectUri);


    Task<TokenResponse> GetTokenAsync(String redirectUri, String code, String? codeVerifier, String traceIdentifier);


    Task<IEnumerable<KeySet>> GetJwksAsync(Boolean invalidateCache = false);


    Task<TokenResponse> RefreshTokenAsync(String refreshToken, String traceIdentifier);


    Task RevokeAsync(String token, String traceIdentifier);


    Task<Uri> GetEndSessionEndpointAsync(String? idToken, String baseAddress);


    Task<Boolean> IntrospectTokenAsync(String? idToken);
}