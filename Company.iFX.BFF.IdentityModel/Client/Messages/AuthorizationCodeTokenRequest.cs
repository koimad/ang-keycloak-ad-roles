namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class AuthorizationCodeTokenRequest : TokenRequest
{
    #region Properties

    public String Code { get; set; } = String.Empty;

    public String RedirectUri { get; set; } = String.Empty;

    public ICollection<String> Resource { get; set; } = new HashSet<String>();

    public String? CodeVerifier { get; set; }

    #endregion
}