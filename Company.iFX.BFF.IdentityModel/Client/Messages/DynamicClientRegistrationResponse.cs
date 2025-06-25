using Company.iFX.BFF.IdentityModel.Client.Extensions;

namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class DynamicClientRegistrationResponse : ProtocolResponse
{
    #region Properties

    public String? ErrorDescription => Json?.TryGetString("error_description");
    public String? ClientId => Json?.TryGetString(OidcConstants.RegistrationResponse.ClientId);
    public String? ClientSecret => Json?.TryGetString(OidcConstants.RegistrationResponse.ClientSecret);
    public String? RegistrationAccessToken => Json?.TryGetString(OidcConstants.RegistrationResponse.RegistrationAccessToken);
    public String? RegistrationClientUri => Json?.TryGetString(OidcConstants.RegistrationResponse.RegistrationClientUri);
    public Int64? ClientIdIssuedAt => Json?.TryGetInt(OidcConstants.RegistrationResponse.ClientIdIssuedAt);
    public Int64? ClientSecretExpiresAt => Json?.TryGetInt(OidcConstants.RegistrationResponse.ClientSecretExpiresAt);
    public String? SoftwareStatement => Json?.TryGetString(OidcConstants.RegistrationResponse.SoftwareStatement);

    #endregion
}