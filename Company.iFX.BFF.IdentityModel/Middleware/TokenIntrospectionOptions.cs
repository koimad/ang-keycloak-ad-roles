namespace Company.iFX.BFF.IdentityModel.Middleware;

public class TokenIntrospectionOptions
{
    #region Properties

    public String Address { get; set; } = String.Empty;
    public String ClientId { get; set; } = String.Empty;
    public String ClientSecret { get; set; } = String.Empty;

    #endregion
}