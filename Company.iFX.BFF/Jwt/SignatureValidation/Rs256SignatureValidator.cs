using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;

using Company.iFX.BFF.Cryptography;

using Jose;

namespace Company.iFX.BFF.Jwt.SignatureValidation;

internal class Rs256SignatureValidator : SignatureValidator
{
    #region Methods

    #region Protected

    protected override Object CreateSigningObject(KeySet signingKey)
    {
        RSAParameters rsaParameters = new RSAParameters {
            Exponent = signingKey.Exponent,
            Modulus = signingKey.Modulus
        };

        RSA rsa = RSA.Create();
        rsa.ImportParameters(rsaParameters);
        return rsa;
    }


    protected override void Decode(String token, Object key)
    {
        JWT.Decode(token, key, JwsAlgorithm.RS256);
    }


    protected override KeySet? GetKeySet(IEnumerable<KeySet> keySets, JwtHeader header)
    {
        KeySet[] keys = keySets
            .Where(x => x.Kid == header.Kid)
            .ToArray();

        return keys.Length == 1
            ? keys.Single()
            : null;
    }

    #endregion

    #endregion
}