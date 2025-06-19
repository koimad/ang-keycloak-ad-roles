using Company.iFX.BFF.ModuleInitializers;

using Yarp.ReverseProxy.Configuration;

// ReSharper disable once CheckNamespace
namespace Company.iFX.BFF;

public class ProxyConfig : IAppSettingsSection
{
    #region Properties

    public Mode Mode { get; set; }
    public String? EndpointName { get; set; }
    public String? ErrorPage { get; set; }
    public String? LandingPage { get; set; }
    public String? NameClaim { get; set; }
    public String? RoleClaim { get; set; }
    public IEnumerable<String> AllowedLandingPages { get; set; } = Array.Empty<String>();
    public Boolean EnableUserPreferredLandingPages { get; set; } = false;
    public Boolean? AlwaysRedirectToHttps { get; set; }
    public Boolean? AllowAnonymousAccess { get; set; }
    public Uri? CustomHostName { get; set; }
    public String? CookieName { get; set; }
    public TimeSpan? SessionIdleTimeout { get; set; }
    public YarpConfig? ReverseProxy { get; set; }

    #endregion

    #region Methods

    #region Private

    private static void AssignIfNotNull<T>(T? value, Action<T> @do)
    {
        if (value != null)
        {
            @do(value);
        }
    }

    #endregion

    #region Public

    public virtual void Apply(ProxyOptions options)
    {
        AssignIfNotNull(ErrorPage, options.SetAuthenticationErrorPage);
        AssignIfNotNull(LandingPage, options.SetLandingPage);
        AssignIfNotNull(CustomHostName, options.SetCustomHostName);
        AssignIfNotNull(CookieName, cookieName => options.CookieName = cookieName);
        AssignIfNotNull(NameClaim, nameClaim => options.NameClaim = nameClaim);
        AssignIfNotNull(RoleClaim, roleClaim => options.RoleClaim = roleClaim);

        options.Mode = Mode;
        options.EnableUserPreferredLandingPages = EnableUserPreferredLandingPages;
        options.AlwaysRedirectToHttps = !AlwaysRedirectToHttps.HasValue || AlwaysRedirectToHttps.Value;
        options.AllowAnonymousAccess = !AllowAnonymousAccess.HasValue || AllowAnonymousAccess.Value;
        options.EndpointName = EndpointName ?? "auth";
        options.SetAllowedLandingPages(AllowedLandingPages);

        if (SessionIdleTimeout.HasValue)
        {
            options.SessionIdleTimeout = SessionIdleTimeout.Value;
        }

        if (options.Mode != Mode.AuthenticateOnly)
        {
            IReadOnlyList<RouteConfig>? routes = ReverseProxy?.Routes.ToRouteConfig();
            IReadOnlyList<ClusterConfig>? clusters = ReverseProxy?.Clusters.ToClusterConfig();

            options.ConfigureYarp(routes, clusters);
        }
    }


    public virtual Boolean Validate(out IEnumerable<String> errors)
    {
        errors = Array.Empty<String>();
        return true;
    }

    #endregion

    #endregion
}