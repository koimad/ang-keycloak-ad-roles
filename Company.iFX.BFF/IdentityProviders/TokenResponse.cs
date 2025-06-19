namespace Company.iFX.BFF.IdentityProviders;

public class TokenResponse(String? access_token, String? id_token, String? refresh_token, DateTime expiryDate)
{
    #region Properties

    public String? access_token { get; } = access_token;

    public String? id_token { get; } = id_token;

    public String? refresh_token { get; } = refresh_token;

    public DateTime ExpiryDate { get; } = expiryDate;

    #endregion
}