using Company.iFX.BFF.IdentityModel.Client.Messages;
using Company.iFX.BFF.OIDC.Client.Browser;

namespace Company.iFX.BFF.OIDC.Client.Requests;

public class LoginRequest
{
    #region Properties

    public DisplayMode BrowserDisplayMode { get; set; } = DisplayMode.Visible;

    public Int32 BrowserTimeout { get; set; } = 300;

    public Parameters FrontChannelExtraParameters { get; set; } = new Parameters();

    public Parameters BackChannelExtraParameters { get; set; } = new Parameters();

    #endregion
}