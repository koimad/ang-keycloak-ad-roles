using System.Text.Json;

using Company.iFX.BFF.IdentityModel.Client.Extensions;

namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class MtlsEndpointAliases
{
    #region Properties

    public JsonElement? Json { get; }

    public String? TokenEndpoint => Json?.TryGetString(OidcConstants.Discovery.TokenEndpoint);

    public String? RevocationEndpoint => Json?.TryGetString(OidcConstants.Discovery.RevocationEndpoint);

    public String? DeviceAuthorizationEndpoint => Json?.TryGetString(OidcConstants.Discovery.DeviceAuthorizationEndpoint);

    public String? IntrospectionEndpoint => Json?.TryGetString(OidcConstants.Discovery.IntrospectionEndpoint);

    #endregion

    #region Constructors

    public MtlsEndpointAliases(JsonElement? json)
    {
        Json = json;
    }

    #endregion
}