using System.Text.Json;

namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class DeviceAuthorizationResponse : ProtocolResponse
{
    #region Properties

    public String? DeviceCode => Json?.TryGetString(OidcConstants.DeviceAuthorizationResponse.DeviceCode);

    public String? UserCode => Json?.TryGetString(OidcConstants.DeviceAuthorizationResponse.UserCode);

    public String? VerificationUri => Json?.TryGetString(OidcConstants.DeviceAuthorizationResponse.VerificationUri);

    public String? VerificationUriComplete => Json?.TryGetString(OidcConstants.DeviceAuthorizationResponse.VerificationUriComplete);

    public Int32? ExpiresIn => Json?.TryGetInt(OidcConstants.DeviceAuthorizationResponse.ExpiresIn);

    public Int32 Interval => Json?.TryGetInt(OidcConstants.DeviceAuthorizationResponse.Interval) ?? 5;

    public String? ErrorDescription => Json?.TryGetString(OidcConstants.TokenResponse.ErrorDescription);

    #endregion
}