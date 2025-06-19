namespace Company.iFX.BFF.Jwt.SignatureValidation;

public class KeySetNotFoundException : ApplicationException
{
    #region Constructors

    public KeySetNotFoundException(String message) : base(message) { }

    #endregion
}