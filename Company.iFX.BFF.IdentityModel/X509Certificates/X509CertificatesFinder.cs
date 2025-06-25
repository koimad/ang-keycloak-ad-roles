using System.Security.Cryptography.X509Certificates;

namespace Company.iFX.BFF.IdentityModel.X509Certificates;

public class X509CertificatesFinder
{
    #region Members

    private readonly X509FindType _findType;
    private readonly StoreLocation _location;
    private readonly StoreName _name;

    #endregion

    #region Constructors

    public X509CertificatesFinder(StoreLocation location, StoreName name, X509FindType findType)
    {
        _location = location;
        _name = name;
        _findType = findType;
    }

    #endregion

    #region Methods

    #region Public

    public IEnumerable<X509Certificate2> Find(Object findValue, Boolean validOnly = true)
    {
        using (X509Store store = new X509Store(_name, _location))
        {
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certColl = store.Certificates.Find(_findType, findValue, validOnly);
            return certColl;
        }
    }

    #endregion

    #endregion
}