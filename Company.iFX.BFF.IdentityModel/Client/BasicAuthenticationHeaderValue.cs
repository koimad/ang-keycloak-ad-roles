using System.Net.Http.Headers;
using System.Text;

namespace Company.iFX.BFF.IdentityModel.Client;

public class BasicAuthenticationHeaderValue : AuthenticationHeaderValue
{
    #region Members

    private static readonly String _scheme = "Basic";

    #endregion

    #region Constructors

    public BasicAuthenticationHeaderValue(String userName, String? password) : base(_scheme, EncodeCredential(userName, password)) { }

    #endregion

    #region Methods

    #region Public

    public static String EncodeCredential(String userName, String? password)
    {
        if (String.IsNullOrWhiteSpace(userName))
        {
            throw new ArgumentNullException(nameof(userName));
        }

        password ??= String.Empty;

        Encoding encoding = Encoding.UTF8;
        String credential = $"{userName}:{password}";

        return Convert.ToBase64String(encoding.GetBytes(credential));
    }

    #endregion

    #endregion
}