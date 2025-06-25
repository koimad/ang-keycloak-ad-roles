namespace Company.iFX.BFF.OIDC.Client;

public class TokenRefreshedEventArgs : EventArgs
{
    #region Properties

    public String AccessToken { get; }

    public String RefreshToken { get; }

    public Int32 ExpiresIn { get; }

    public String? IdentityToken { get; }

    #endregion

    #region Constructors

    public TokenRefreshedEventArgs(String accessToken, String refreshToken, Int32 expiresIn, String? identityToken = null)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        ExpiresIn = expiresIn;
        IdentityToken = identityToken;
    }

    #endregion
}