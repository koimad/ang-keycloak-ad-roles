using System.Net;
using System.Text;

namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class AuthorizeResponse
{
    #region Properties

    public String Raw { get; }

    public Dictionary<String, String> Values { get; } = new Dictionary<String, String>();

    public String Code => TryGet(OidcConstants.AuthorizeResponse.Code)!;

    public String AccessToken => TryGet(OidcConstants.AuthorizeResponse.AccessToken)!;

    public String IdentityToken => TryGet(OidcConstants.AuthorizeResponse.IdentityToken)!;

    public String Error => TryGet(OidcConstants.AuthorizeResponse.Error)!;

    public String Scope => TryGet(OidcConstants.AuthorizeResponse.Scope)!;

    public String TokenType => TryGet(OidcConstants.AuthorizeResponse.TokenType)!;

    public String State => TryGet(OidcConstants.AuthorizeResponse.State)!;

    public String SessionState => TryGet(OidcConstants.AuthorizeResponse.SessionState)!;

    public String Issuer => TryGet(OidcConstants.AuthorizeResponse.Issuer)!;

    public String ErrorDescription => TryGet(OidcConstants.AuthorizeResponse.ErrorDescription)!;

    public Boolean IsError => Error.IsPresent();

    public Int32 ExpiresIn
    {
        get
        {
            String? value = TryGet(OidcConstants.AuthorizeResponse.ExpiresIn);
            Int32.TryParse(value, out Int32 theValue);

            return theValue;
        }
    }

    #endregion

    #region Constructors

    public AuthorizeResponse(String raw)
    {
        Raw = raw;
        ParseRaw();
    }

    #endregion

    #region Methods

    #region Private

    private void ParseRaw()
    {
        String[] fragments;
        if (Raw.Contains("?"))
        {
            fragments = Raw.Split('?');

            Int32 additionalHashFragment = fragments[1].IndexOf('#');

            if (additionalHashFragment >= 0)
            {
                fragments[1] = fragments[1].Substring(0, additionalHashFragment);
            }
        }
        else if (Raw.Contains("#"))
        {
            fragments = Raw.Split('#');
        }
        else
        {
            fragments = new[] { "", Raw };
        }

        String[] qparams = fragments[1].Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (String param in qparams)
        {
            String[] parts = param.Split('=');

            if (parts.Length == 2)
            {
                Values.Add(parts[0], parts[1]);
            }
            else
            {
                throw new InvalidOperationException("Malformed callback URL.");
            }
        }
    }

    #endregion

    #region Public

    public String? TryGet(String type)
    {
        if (Values.TryGetValue(type, out String? value))
        {
            return WebUtility.UrlDecode(value);
        }

        return null;
    }

    #endregion

    #endregion
}