namespace Company.iFX.BFF.OpenIdConnect;

public class TokenRenewalFailedException(String errorMessage) : ApplicationException(errorMessage);