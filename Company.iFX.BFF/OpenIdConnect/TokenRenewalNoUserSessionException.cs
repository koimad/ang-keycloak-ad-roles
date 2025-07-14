namespace Company.iFX.BFF.OpenIdConnect;

public class TokenRenewalNoUserSessionException(String errorMessage) : ApplicationException(errorMessage);