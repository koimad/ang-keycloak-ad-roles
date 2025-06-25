using System.Text;

namespace Company.iFX.BFF.OIDC.Client.Results;

public abstract class Result
{
    #region Properties

    public Boolean IsError => Error.IsPresent();

    public String? Error { get; set; }

    public String? ErrorDescription { get; set; } 

    #endregion
}