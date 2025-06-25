namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class UserInfoRequest : ProtocolRequest
{
    #region Properties

    public String? Token { get; set; }

    #endregion
}