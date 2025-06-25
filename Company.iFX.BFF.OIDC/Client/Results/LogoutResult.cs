namespace Company.iFX.BFF.OIDC.Client.Results;

public class LogoutResult : Result
{
    #region Properties

    public String? Response { get; set; }

    #endregion

    #region Constructors

    public LogoutResult() { }


    public LogoutResult(String error)
    {
        Error = error;
    }

    #endregion
}