using System.Text;

namespace Company.iFX.BFF.IdentityModel.Client;

public class DiscoveryEndpoint
{
    #region Members

    private const String? _http = "http";
    private const String? _https = "https";
    private const String? _malformedUrl = "Malformed URL";

    #endregion

    #region Properties

    public String Authority { get; }

    public String Url { get; }

    #endregion

    #region Constructors

    public DiscoveryEndpoint(String authority, String url)
    {
        Authority = authority;
        Url = url;
    }

    #endregion

    #region Methods

    #region Public

    public static Boolean IsSecureScheme(Uri url, DiscoveryPolicy policy)
    {
        if (policy.RequireHttps)
        {
            if (policy.AllowHttpOnLoopback)
            {
                String hostName = url.DnsSafeHost;

                foreach (String address in policy.LoopbackAddresses)
                {
                    if (String.Equals(hostName, address, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return String.Equals(url.Scheme, _https, StringComparison.OrdinalIgnoreCase);
        }

        return true;
    }


    public static Boolean IsValidScheme(Uri url)
    {
        if (String.Equals(url.Scheme, _http, StringComparison.OrdinalIgnoreCase) ||
            String.Equals(url.Scheme, _https, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }


    public static DiscoveryEndpoint ParseUrl(String input, String? path = null)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input));
        }

        if (String.IsNullOrEmpty(path))
        {
            path = OidcConstants.Discovery.DiscoveryEndpoint;
        }

        Boolean success = Uri.TryCreate(input, UriKind.Absolute, out Uri? uri);

        if (success == false)
        {
            throw new InvalidOperationException(_malformedUrl);
        }

        if (!IsValidScheme(uri!))
        {
            throw new InvalidOperationException(_malformedUrl);
        }

        String url = input.RemoveTrailingSlash();

        if (path!.StartsWith("/"))
        {
            path = path.Substring(1);
        }

        if (url.EndsWith(path, StringComparison.OrdinalIgnoreCase))
        {
            return new DiscoveryEndpoint(url.Substring(0, url.Length - path.Length - 1), url);
        }

        return new DiscoveryEndpoint(url, url.EnsureTrailingSlash() + path);
    }

    #endregion

    #endregion
}