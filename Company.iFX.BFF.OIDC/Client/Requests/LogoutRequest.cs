using Company.iFX.BFF.OIDC.Client.Browser;

namespace Company.iFX.BFF.OIDC.Client.Requests;

public class LogoutRequest
{
    #region Properties

    public DisplayMode BrowserDisplayMode { get; set; } = DisplayMode.Visible;

    public Int32 BrowserTimeout { get; set; } = 300;

    public String? IdTokenHint { get; set; }

    public String? State { get; set; }

    #endregion
}