using Company.iFX.BFF.IdentityModel.Client.Messages;
using Company.iFX.BFF.OIDC.Client.Browser;

namespace Company.iFX.BFF.OIDC.Client.Requests;

public class AuthorizeRequest
{
    #region Members

    public Parameters ExtraParameters = new Parameters();

    #endregion

    #region Properties

    public DisplayMode DisplayMode { get; set; } = DisplayMode.Visible;
    public Int32 Timeout { get; set; } = 300;

    #endregion
}