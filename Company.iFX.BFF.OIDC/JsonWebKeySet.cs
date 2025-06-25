using Company.iFX.BFF.Cryptography;
using Company.iFX.BFF.IdentityModel.Client.Messages;

namespace Company.iFX.BFF.OIDC;

internal class JsonWebKeySet : List<KeySet>
{
    #region Constructors

    private JsonWebKeySet(IEnumerable<KeySet> keySets)
    {
        AddRange(keySets);
    }

    #endregion

    #region Methods

    #region Internal

    internal static JsonWebKeySet Create(JsonWebKeySetResponse jwksResponse)
    {
        KeySet[]? keySets = jwksResponse.KeySet?.Keys
            .Select(x =>
            {
                Byte[] exponent = x.E.Base64UrlDecode();
                Byte[] modulus = x.N.Base64UrlDecode();

                return new KeySet(exponent, modulus, x.Kid);
            })
            .ToArray();

        return new(keySets);
    }

    #endregion

    #endregion
}