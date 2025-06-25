namespace Company.iFX.BFF.OIDC.Client;

internal class Pkce
{
    #region Properties

    public String? CodeVerifier { get; set; }
    public String? CodeChallenge { get; set; }

    #endregion
}