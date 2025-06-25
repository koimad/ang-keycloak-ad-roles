using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;

using Company.iFX.BFF.IdentityModel.Internal;

namespace Company.iFX.BFF.IdentityModel;

public static class Identity
{
    #region Properties

    public static ClaimsIdentity Anonymous
    {
        get
        {
            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Name, String.Empty)
            };

            return new ClaimsIdentity(claims);
        }
    }

    #endregion

    #region Methods

    #region Public

    public static ClaimsIdentity Create(String authenticationType, params Claim[] claims)
    {
        return new ClaimsIdentity(claims, authenticationType, JwtClaimTypes.Name, JwtClaimTypes.Role);
    }


    public static ClaimsIdentity CreateFromCertificate(X509Certificate2 certificate, String authenticationType, Boolean includeAllClaims)
    {
        List<Claim> claims = new List<Claim>();
        String issuer = certificate.Issuer;

        claims.Add(new Claim("issuer", issuer));

        String thumbprint = certificate.Thumbprint;
        claims.Add(new Claim(ClaimTypes.Thumbprint, thumbprint, ClaimValueTypes.Base64Binary, issuer));

        String name = certificate.SubjectName.Name;

        if (name.IsPresent())
        {
            claims.Add(new Claim(ClaimTypes.X500DistinguishedName, name, ClaimValueTypes.String, issuer));
        }

        if (includeAllClaims)
        {
            name = certificate.SerialNumber;

            if (name.IsPresent())
            {
                claims.Add(new Claim(ClaimTypes.SerialNumber, name, ClaimValueTypes.String, issuer));
            }

            name = certificate.GetNameInfo(X509NameType.DnsName, false);

            if (name.IsPresent())
            {
                claims.Add(new Claim(ClaimTypes.Dns, name, ClaimValueTypes.String, issuer));
            }

            name = certificate.GetNameInfo(X509NameType.SimpleName, false);

            if (name.IsPresent())
            {
                claims.Add(new Claim(ClaimTypes.Name, name, ClaimValueTypes.String, issuer));
            }

            name = certificate.GetNameInfo(X509NameType.EmailName, false);

            if (name.IsPresent())
            {
                claims.Add(new Claim(ClaimTypes.Email, name, ClaimValueTypes.String, issuer));
            }

            name = certificate.GetNameInfo(X509NameType.UpnName, false);

            if (name.IsPresent())
            {
                claims.Add(new Claim(ClaimTypes.Upn, name, ClaimValueTypes.String, issuer));
            }

            name = certificate.GetNameInfo(X509NameType.UrlName, false);

            if (name.IsPresent())
            {
                claims.Add(new Claim(ClaimTypes.Uri, name, ClaimValueTypes.String, issuer));
            }
        }

        return new ClaimsIdentity(claims, authenticationType);
    }

    #endregion

    #endregion
}