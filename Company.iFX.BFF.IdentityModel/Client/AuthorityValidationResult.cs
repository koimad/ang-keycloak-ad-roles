namespace Company.iFX.BFF.IdentityModel.Client;

public record AuthorityValidationResult
{
    #region Members

    private const String? _errorMessageMustBeProvidedIfSuccessFalse = "A message must be provided if success=false.";
    private const String _success = "success";
    public static readonly AuthorityValidationResult SuccessResult = new(true, null);

    #endregion

    #region Properties

    public String ErrorMessage { get; }

    public Boolean Success { get; }

    #endregion

    #region Constructors

    private AuthorityValidationResult(Boolean success, String? message)
    {
        if (!success && String.IsNullOrEmpty(message))
        {
            throw new ArgumentException(_errorMessageMustBeProvidedIfSuccessFalse, nameof(message));
        }

        ErrorMessage = message!;
        Success = success;
    }

    #endregion

    #region Methods

    #region Public

    public static AuthorityValidationResult CreateError(String message)
    {
        return new AuthorityValidationResult(false, message);
    }


    public override String ToString()
    {
        return Success ? _success : ErrorMessage;
    }

    #endregion

    #endregion
}