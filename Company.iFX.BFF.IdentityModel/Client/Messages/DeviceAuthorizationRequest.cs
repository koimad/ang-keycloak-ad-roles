namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class DeviceAuthorizationRequest : ProtocolRequest
{
    #region Properties

    public String? Scope { get; set; }

    #endregion
}