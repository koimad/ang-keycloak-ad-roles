namespace Company.iFX.BFF.IdentityModel.Middleware;

public class TokenIntrospectionOptions
{
    #region Properties

    public String Address { get; set; } = "";
    public String ClientId { get; set; } = "";
    public String ClientSecret { get; set; } = "";

    #endregion
}