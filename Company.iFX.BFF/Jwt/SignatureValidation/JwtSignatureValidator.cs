using System.IdentityModel.Tokens.Jwt;

using Company.iFX.BFF.Cryptography;
using Company.iFX.BFF.IdentityProviders;

using Microsoft.Extensions.Logging;

namespace Company.iFX.BFF.Jwt.SignatureValidation;

public class JwtSignatureValidator(
    IIdentityProvider identityProvider,
    ILogger<JwtSignatureValidator> logger,
    IEncryptionKey? jwtEncryptionKey,
    Rs256SignatureValidator rs256SignatureValidator,
    Hs256SignatureValidator hs256SignatureValidator) : IJwtSignatureValidator
{
    #region Methods

    #region Private

    private SignatureValidator? CreateValidator(JwtHeader header)
    {
        return header.Alg switch {
            "RS256" => rs256SignatureValidator,
            "HS256" => hs256SignatureValidator,
            _       => null
        };
    }

    #endregion

    #region Public

    public virtual async Task<Boolean> Validate(String? token)
    {
        if (String.IsNullOrEmpty(token))
        {
            return false;
        }

        if (JweParser.IsJwe(token))
        {
            if (jwtEncryptionKey is null)
            {
                return false;
            }

            try
            {
                jwtEncryptionKey.Decrypt(token);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        JwtHeader? header = JwtParser.ParseJwtHeader(token);

        if (header == null)
        {
            logger.LogWarning("Unable to determine how to validate the access_token. The JWT does not have a header. The signature has not been verified and the JWT is considered to be invalid.");
            return false; 
        }

        SignatureValidator? signatureValidator = CreateValidator(header);

        if (signatureValidator == null)
        {
            logger.LogError("Unable to determine how to validate the access_token. Unable to find a validator for the algorithm specified in the JWT header. The signature has not been verified and the JWT is considered to be invalid.");
            return false;
        }

        IEnumerable<KeySet> keys = await identityProvider.GetJwksAsync();

        try
        {
            return await signatureValidator.Validate(token, keys);
        }
        catch (KeySetNotFoundException)
        {
            // If the signature verification fails because the key wasn't found, it is very well possible the keys have
            // rotated in that case it is common practice to re-obtain the key-set and verify again.
            keys = await identityProvider.GetJwksAsync(true);
            return await signatureValidator.Validate(token, keys);
        }
    }

    #endregion

    #endregion
}