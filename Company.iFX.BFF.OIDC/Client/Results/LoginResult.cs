using System.Security.Claims;

using Company.iFX.BFF.IdentityModel.Client.Messages;

namespace Company.iFX.BFF.OIDC.Client.Results;

public class LoginResult : Result
{
    #region Properties

    public ClaimsPrincipal? User { get; set; }

    public String AccessToken { get; set; } = String.Empty!;

    public String IdentityToken { get; set; } = String.Empty!;


    public String RefreshToken { get; set; } = String.Empty!;


    public DateTimeOffset? AccessTokenExpiration { get; set; }


    public DateTimeOffset? AuthenticationTime { get; set; }


    public DelegatingHandler? RefreshTokenHandler { get; set; }


    public TokenResponse TokenResponse { get; set; } = new TokenResponse();

    #endregion

    #region Constructors

    public LoginResult() { }


    public LoginResult(String error)
    {
        Error = error;
    }

    public LoginResult(String error, String errorDescription)
    {
        Error = error;
        ErrorDescription = errorDescription;
    }

    #endregion
}