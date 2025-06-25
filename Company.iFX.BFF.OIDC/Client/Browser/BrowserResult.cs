using Company.iFX.BFF.OIDC.Client.Results;

namespace Company.iFX.BFF.OIDC.Client.Browser;

public class BrowserResult : Result
{
    #region Properties

    public BrowserResultType ResultType { get; set; }

    public String? Response { get; set; }

    #endregion
}