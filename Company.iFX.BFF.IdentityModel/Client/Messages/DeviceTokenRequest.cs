namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class DeviceTokenRequest : TokenRequest
{
    #region Properties

    public String DeviceCode { get; set; } = String.Empty!;

    #endregion
}