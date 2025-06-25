using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;


namespace Company.iFX.BFF.IdentityModel;

public static class Principal
{
    #region Properties
    public static ClaimsPrincipal Anonymous => new ClaimsPrincipal(Identity.Anonymous);

    #endregion

    #region Methods

    #region Public

    public static ClaimsPrincipal Create(String authenticationType, params Claim[] claims)
    {
        return new ClaimsPrincipal(Identity.Create(authenticationType, claims));
    }


    public static ClaimsPrincipal CreateFromCertificate(X509Certificate2 certificate, String authenticationType = "X.509", Boolean includeAllClaims = false)
    {
        return new ClaimsPrincipal(Identity.CreateFromCertificate(certificate, authenticationType, includeAllClaims));
    }

    #endregion

    #endregion
}