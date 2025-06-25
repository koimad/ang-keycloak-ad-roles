namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class DynamicClientRegistrationRequest : ProtocolRequest
{
    #region Properties

    public String? Token { get; set; }

    public DynamicClientRegistrationDocument Document { get; set; } = new DynamicClientRegistrationDocument();

    #endregion
}