using Jose;

using Microsoft.IdentityModel.Tokens;

namespace Company.iFX.BFF.Cryptography;

public class SymmetricKey(SymmetricSecurityKey key) : IEncryptionKey
{
    #region Methods

    #region Public

    public String Decrypt(String token)
    {
        JweToken? jweToken = JWE.Decrypt(token, key.Key);
        return jweToken.Plaintext;
    }


    public Byte[] ToByteArray()
    {
        return key.Key;
    }

    #endregion

    #endregion
}
