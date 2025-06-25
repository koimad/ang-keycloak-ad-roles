namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class TokenRequest : ProtocolRequest
{
    #region Properties

    public String GrantType { get; set; } = String.Empty;

    #endregion
}