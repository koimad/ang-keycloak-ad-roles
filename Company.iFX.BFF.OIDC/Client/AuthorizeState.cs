using Company.iFX.BFF.OIDC.Client.Results;

namespace Company.iFX.BFF.OIDC.Client;
public class AuthorizeState : Result
{
    #region Properties

    public String? StartUrl { get; set; }

    public String? State { get; set; }

    public String? CodeVerifier { get; set; }

    public String? RedirectUri { get; set; }

    #endregion
}