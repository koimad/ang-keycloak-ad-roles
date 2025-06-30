using Company.iFX.BFF.ModuleInitializers;

using Microsoft.AspNetCore.Http;

namespace Company.iFX.BFF.OpenIdConnect;

public class RedirectUriFactory : IRedirectUriFactory
{
    #region Members

    private readonly ProxyOptions _options;

    #endregion

    #region Constructors

    public RedirectUriFactory(ProxyOptions options)
    {
        _options = options;
    }

    #endregion

    #region Methods

    #region Public

    public String DetermineHostName(HttpContext context)
    {
        Uri hostName = _options.CustomHostName == null
            ? new Uri($"{context.Request.Scheme}://{context.Request.Host}")
            : new Uri($"{_options.CustomHostName.Scheme}://{_options.CustomHostName.Authority}");

        return hostName.Scheme == "http" && _options.AlwaysRedirectToHttps
            ? $"https://{hostName.Authority}"
            : hostName.ToString().TrimEnd('/');
    }


    public String DetermineRedirectUri(HttpContext context, PathString endpointName)
    {
        String hostName = DetermineHostName(context);
        return $"{hostName}{endpointName}/login/callback";
    }

    #endregion

    #endregion
}