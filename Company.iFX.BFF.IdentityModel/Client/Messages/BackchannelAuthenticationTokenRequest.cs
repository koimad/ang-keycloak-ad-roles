
namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class BackchannelAuthenticationTokenRequest : TokenRequest
{
    #region Properties

    public String AuthenticationRequestId { get; set; } = String.Empty!;

    public ICollection<String> Resource { get; set; } = new HashSet<String>();

    #endregion
}