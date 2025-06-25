using System.Security.Claims;

using Company.iFX.BFF.IdentityModel.Client.Messages;

namespace Company.iFX.BFF.OIDC.Client.Results;

public class ResponseValidationResult : Result
{
    #region Properties

    public AuthorizeResponse? AuthorizeResponse { get; set; } 
    public TokenResponse? TokenResponse { get; set; }
    public ClaimsPrincipal? User { get; set; }

    #endregion

    #region Constructors

    public ResponseValidationResult() { }


    public ResponseValidationResult(String error)
    {
        Error = error;
    }

    #endregion
}