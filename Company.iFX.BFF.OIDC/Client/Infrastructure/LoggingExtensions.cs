using System.Diagnostics;
using System.Security.Claims;

using Microsoft.Extensions.Logging;

namespace Company.iFX.BFF.OIDC.Client.Infrastructure
{
    internal static class LoggingExtensions
    {
        public static void LogClaims(this ILogger logger, IEnumerable<Claim> claims)
        {
            foreach (Claim claim in claims)
            {
                logger.LogDebug($"Claim: {claim.Type}: {claim.Value}");
            }
        }

        public static void LogClaims(this ILogger logger, ClaimsPrincipal user)
        {
            logger.LogClaims(user.Claims);
        }
    }
}