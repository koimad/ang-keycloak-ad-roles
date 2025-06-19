namespace Company.iFX.BFF.Jwt.SignatureValidation;

public interface IJwtSignatureValidator
{
    Task<Boolean> Validate(String? token);
}