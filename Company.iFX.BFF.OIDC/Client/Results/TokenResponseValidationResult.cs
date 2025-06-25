namespace Company.iFX.BFF.OIDC.Client.Results;

public class TokenResponseValidationResult : Result
{
    #region Properties

    public IdentityTokenValidationResult? IdentityTokenValidationResult { get; set; }

    #endregion

    #region Constructors

    public TokenResponseValidationResult(String error)
    {
        Error = error;
    }


    public TokenResponseValidationResult(IdentityTokenValidationResult? result)
    {
        
        IdentityTokenValidationResult = result;
    }

    #endregion
}