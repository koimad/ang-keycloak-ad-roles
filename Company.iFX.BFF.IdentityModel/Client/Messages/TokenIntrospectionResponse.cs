using System.Security.Claims;
using System.Text.Json;

using Company.iFX.BFF.IdentityModel.Client.Extensions;

namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class TokenIntrospectionResponse : ProtocolResponse
{
    #region Members

    private const String _active = "active";
    private const String? _jsonIsNull = "Json is null";

    #endregion

    #region Properties

    public Boolean IsActive => Json?.TryGetBoolean(_active) ?? false;

    public IEnumerable<Claim> Claims { get; protected set; } = Enumerable.Empty<Claim>();

    #endregion

    #region Methods

    #region Protected

    protected override Task InitializeAsync(Object? initializationData = null)
    {
        if (!IsError)
        {
            if (Json == null)
            {
                throw new InvalidOperationException(_jsonIsNull); // TODO better exception
            }

            String? issuer = Json?.TryGetString(OidcConstants.AuthorizeResponse.Issuer);
            List<Claim> claims = Json?.ToClaims(issuer, OidcConstants.AuthorizeResponse.Scope).ToList() ?? new List<Claim>();

            JsonElement? scope = Json?.TryGetValue(OidcConstants.AuthorizeResponse.Scope);

            if (scope?.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement item in scope?.EnumerateArray() ?? Enumerable.Empty<JsonElement>())
                {
                    claims.Add(new Claim(OidcConstants.AuthorizeResponse.Scope, item.ToString(), ClaimValueTypes.String, issuer));
                }
            }
            else
            {
                String scopeString = scope.ToString() ?? "";

                String[] scopes = scopeString.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (String scopeValue in scopes)
                {
                    claims.Add(new Claim(OidcConstants.AuthorizeResponse.Scope, scopeValue, ClaimValueTypes.String, issuer));
                }
            }

            Claims = claims;
        }

        return Task.CompletedTask;
    }

    #endregion

    #endregion
}