
namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class RefreshTokenRequest : TokenRequest
{
    #region Properties

    public String RefreshToken { get; set; } = String.Empty!;

    public String? Scope { get; set; }

    public ICollection<String> Resource { get; set; } = new HashSet<String>();

    #endregion
}