
using System.Security.Cryptography.X509Certificates;

namespace Company.iFX.BFF.IdentityModel.X509Certificates;

public static class X509
{
    public static X509CertificatesLocation CurrentUser => new X509CertificatesLocation(StoreLocation.CurrentUser);
    public static X509CertificatesLocation LocalMachine => new X509CertificatesLocation(StoreLocation.LocalMachine);
}