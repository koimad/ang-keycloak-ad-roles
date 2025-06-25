using System.Security.Cryptography.X509Certificates;

namespace Company.iFX.BFF.IdentityModel.X509Certificates;

public class X509CertificatesName
{
    #region Members

    private readonly StoreLocation _location;
    private readonly StoreName _name;

    #endregion

    #region Properties

    public X509CertificatesFinder Thumbprint => new X509CertificatesFinder(_location, _name, X509FindType.FindByThumbprint);
    public X509CertificatesFinder SubjectDistinguishedName => new X509CertificatesFinder(_location, _name, X509FindType.FindBySubjectDistinguishedName);
    public X509CertificatesFinder SerialNumber => new X509CertificatesFinder(_location, _name, X509FindType.FindBySerialNumber);
    public X509CertificatesFinder IssuerName => new X509CertificatesFinder(_location, _name, X509FindType.FindByIssuerName);

    #endregion

    #region Constructors

    public X509CertificatesName(StoreLocation location, StoreName name)
    {
        _location = location;
        _name = name;
    }

    #endregion
}