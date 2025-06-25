using System.Security.Claims;
using System.Text;
using System.Text.Json;

using Company.iFX.BFF.IdentityModel;
using Company.iFX.BFF.OIDC.Client.Results;

namespace Company.iFX.BFF.OIDC.Client;

public class NoValidationIdentityTokenValidator : IIdentityTokenValidator
{
    #region Methods

    #region Public

    public Task<IdentityTokenValidationResult> ValidateAsync(String identityToken, OidcClientOptions options, CancellationToken cancellationToken = default)
    {
        String[] parts = identityToken.Split('.');

        if (parts.Length != 3)
        {
            IdentityTokenValidationResult error = new IdentityTokenValidationResult {
                Error = "invalid_jwt"
            };

            return Task.FromResult(error);
        }

        String payload = Encoding.UTF8.GetString(Base64Url.Decode(parts[1]));

        Dictionary<String, JsonElement>? values = JsonSerializer.Deserialize(
            payload, SourceGenerationContext.Default.DictionaryStringJsonElement);

        List<Claim> claims = new List<Claim>();

        foreach (KeyValuePair<String, JsonElement> element in values)
        {
            if (element.Value.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement item in element.Value.EnumerateArray())
                {
                    claims.Add(new Claim(element.Key, item.ToString()));
                }
            }
            else
            {
                claims.Add(new Claim(element.Key, element.Value.ToString()));
            }
        }

        IdentityTokenValidationResult result = new IdentityTokenValidationResult {
            SignatureAlgorithm = "none",
            User = new ClaimsPrincipal(new ClaimsIdentity(claims, "none", "name", "role"))
        };

        return Task.FromResult(result);
    }

    #endregion

    #endregion
}