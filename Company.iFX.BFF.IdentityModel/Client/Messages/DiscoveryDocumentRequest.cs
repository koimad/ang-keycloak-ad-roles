namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class DiscoveryDocumentRequest : ProtocolRequest
{
    #region Properties

    public DiscoveryPolicy Policy { get; set; } = new DiscoveryPolicy();

    #endregion
}