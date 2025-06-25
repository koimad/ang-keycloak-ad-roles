namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class PasswordTokenRequest : TokenRequest
{
    #region Properties

    public String UserName { get; set; } = String.Empty!;

    public String? Password { get; set; }

    public String? Scope { get; set; }

    public ICollection<String> Resource { get; set; } = new HashSet<String>();

    #endregion
}