namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class ClientCredentialsTokenRequest : TokenRequest
{
    #region Properties

    public String? Scope { get; set; }

    public ICollection<String> Resource { get; set; } = new HashSet<String>();

    #endregion
}