namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class TokenIntrospectionRequest : ProtocolRequest
{
    #region Properties

    public String Token { get; set; } = String.Empty!;

    public String? TokenTypeHint { get; set; }

    #endregion
}