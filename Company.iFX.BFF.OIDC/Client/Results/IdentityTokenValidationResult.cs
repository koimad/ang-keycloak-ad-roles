using System.Security.Claims;

namespace Company.iFX.BFF.OIDC.Client.Results
{
    public class IdentityTokenValidationResult : Result
    {
        public ClaimsPrincipal? User { get; set; }
        
        public String? SignatureAlgorithm { get; set; }
    }
}