using System.Security.Cryptography.X509Certificates;

namespace Company.iFX.BFF.IdentityModel.X509Certificates;

public class X509CertificatesLocation
{
    private readonly StoreLocation _location;

    public X509CertificatesLocation(StoreLocation location)
    {
        _location = location;
    }

    public X509CertificatesName My => new X509CertificatesName(_location, StoreName.My);
    public X509CertificatesName AddressBook => new X509CertificatesName(_location, StoreName.AddressBook);
    public X509CertificatesName TrustedPeople => new X509CertificatesName(_location, StoreName.TrustedPeople);
    public X509CertificatesName TrustedPublisher => new X509CertificatesName(_location, StoreName.TrustedPublisher);
    public X509CertificatesName CertificateAuthority => new X509CertificatesName(_location, StoreName.CertificateAuthority);
}