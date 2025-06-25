namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class TokenRevocationRequest : ProtocolRequest
{
    #region Properties

    public String Token { get; set; } = default!;


    public String TokenTypeHint { get; set; } = default!;

    #endregion
}