namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class TokenResponse : ProtocolResponse
{
    #region Properties

    public String AccessToken => TryGet(OidcConstants.TokenResponse.AccessToken)!;

    public String IdentityToken => TryGet(OidcConstants.TokenResponse.IdentityToken)!;

    public String Scope => TryGet(OidcConstants.TokenResponse.Scope)!;

    public String IssuedTokenType => TryGet(OidcConstants.TokenResponse.IssuedTokenType)!;

    public String TokenType => TryGet(OidcConstants.TokenResponse.TokenType)!;

    public String RefreshToken => TryGet(OidcConstants.TokenResponse.RefreshToken)!;

    public String ErrorDescription => TryGet(OidcConstants.TokenResponse.ErrorDescription)!;

    public Int32 ExpiresIn
    {
        get
        {
            String? value = TryGet(OidcConstants.TokenResponse.ExpiresIn);

            if (value != null)
            {
                if (Int32.TryParse(value, out Int32 theValue))
                {
                    return theValue;
                }
            }

            return 0;
        }
    }

    #endregion
}