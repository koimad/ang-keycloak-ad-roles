using System.Net.Http.Headers;
using System.Text;

namespace Company.iFX.BFF.IdentityModel.Client;

public class BasicAuthenticationOAuthHeaderValue : AuthenticationHeaderValue
{
    #region Members

    private const String _scheme = "Basic";

    #endregion

    #region Constructors

    public BasicAuthenticationOAuthHeaderValue(String userName, String password) : base(_scheme, EncodeCredential(userName, password)) { }

    #endregion

    #region Methods

    #region Private

    private static String UrlEncode(String value)
    {
        if (String.IsNullOrEmpty(value))
        {
            return String.Empty;
        }

        return Uri.EscapeDataString(value).Replace("%20", "+");
    }

    #endregion

    #region Public

    public static String EncodeCredential(String userName, String? password)
    {
        if (String.IsNullOrWhiteSpace(userName))
        {
            throw new ArgumentNullException(nameof(userName));
        }

        if (password == null)
        {
            password = String.Empty;
        }

        Encoding encoding = Encoding.UTF8;
        String credential = $"{UrlEncode(userName)}:{UrlEncode(password)}";

        return Convert.ToBase64String(encoding.GetBytes(credential));
    }

    #endregion

    #endregion
}