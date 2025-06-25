
namespace Company.iFX.BFF.OIDC.Client.Results;

public class RefreshTokenResult : Result
{
    #region Properties

    public virtual String? IdentityToken { get; set; }


    public virtual String? AccessToken { get; set; }


    public virtual String? RefreshToken { get; set; }


    public virtual Int32 ExpiresIn { get; set; }


    public virtual DateTimeOffset AccessTokenExpiration { get; set; }

    #endregion
}