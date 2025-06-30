using System.IdentityModel.Tokens.Jwt;

using Company.iFX.BFF.Cryptography;

using Jose;

namespace Company.iFX.BFF.Jwt.SignatureValidation;

public class Hs256SignatureValidator(SymmetricKey symmetricKey) : SignatureValidator
{
    #region Methods

    #region Protected

    protected override Object CreateSigningObject(KeySet signingKey)
    {
        return symmetricKey.ToByteArray();
    }


    protected override void Decode(String token, Object key)
    {
        JWT.Decode(token, key, JwsAlgorithm.HS256);
    }


    protected override KeySet? GetKeySet(IEnumerable<KeySet> keySets, JwtHeader header)
    {
        return keySets.SingleOrDefault();
    }

    #endregion

    #endregion
}