namespace Company.iFX.BFF.OIDC.Client.Browser;

public class BrowserOptions
{
    #region Properties

    public String StartUrl { get; }

    public String EndUrl { get; }

    public DisplayMode DisplayMode { get; set; } = DisplayMode.Visible;

    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(5);

    #endregion

    #region Constructors

    public BrowserOptions(String startUrl, String endUrl)
    {
        StartUrl = startUrl;
        EndUrl = endUrl;
    }

    #endregion
}