using System.IdentityModel.Tokens.Jwt;

using Company.iFX.BFF.Cryptography;

using Jose;

namespace Company.iFX.BFF.Jwt.SignatureValidation;

internal abstract class SignatureValidator
{
    #region Methods

    #region Protected

    protected abstract Object CreateSigningObject(KeySet signingKey);


    protected virtual void Decode(String token, Object key)
    {
        JWT.Decode(token, key);
    }


    protected virtual JwtHeader? GetHeader(String token)
    {
        return JwtParser.ParseJwtHeader(token);
    }


    protected abstract KeySet? GetKeySet(IEnumerable<KeySet> keys, JwtHeader header);

    #endregion

    #region Public

    public Task<Boolean> Validate(String token, IEnumerable<KeySet> keys)
    {
        JwtHeader? header = GetHeader(token);

        if (header == null)
        {
            return Task.FromResult(false);
        }

        KeySet? keySet = GetKeySet(keys, header);

        if (keySet == null)
        {
            throw new KeySetNotFoundException("Failed to validate signature. Unable to find appropriate key to validate the signature with.");
        }

        // The signature is validated using JoseJWT. It uses an object to do so.
        // This could be anything ranging from a byte array to an RSA object.
        Object rsa = CreateSigningObject(keySet);

        try
        {
            Decode(token, rsa);
            return Task.FromResult(true);
        }
        catch (Exception)
        {
            return Task.FromResult(false);
        }
    }

    #endregion

    #endregion
}