using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using Jose;

namespace Company.iFX.BFF.Cryptography;

public sealed class SslCertificate(X509Certificate2 certificate) : IEncryptionKey
{
    #region Methods

    #region Public

    public String Decrypt(String token)
    {
        RSA? privateKey = certificate.GetRSAPrivateKey();

        if (privateKey == null)
        {
            throw new NotSupportedException("Failed to decrypt JWE. The provided certificate does not have a private key. " +
                                            "The private key of the certificate is required to decrypt the JWE. Provide a certificate with a private key.");
        }

        JweToken? jweToken = JWE.Decrypt(token, privateKey);
        return jweToken.Plaintext;
    }

    #endregion

    #endregion
}