
namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class BackchannelAuthenticationRequest : ProtocolRequest
{
    #region Properties

    public String Scope { get; set; } = String.Empty;

    public String? ClientNotificationToken { get; set; }

    public String? AcrValues { get; set; }

    public String? LoginHintToken { get; set; }

    public String? IdTokenHint { get; set; }

    public String? LoginHint { get; set; }

    public String? BindingMessage { get; set; }

    public String? UserCode { get; set; }

    public Int32? RequestedExpiry { get; set; }

    public String? RequestObject { get; set; }

    public ICollection<String> Resource { get; set; } = new HashSet<String>();

    #endregion
}