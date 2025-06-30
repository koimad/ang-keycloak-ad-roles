using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;

using Company.iFX.BFF.Cryptography;

namespace Company.iFX.BFF.Jwt;

public sealed class JweParser(IEncryptionKey encryptionKey) : JwtParser
{
    #region Methods

    #region Public

    public static Boolean IsJwe(String token)
    {
        return !String.IsNullOrEmpty(token) && token.Split('.', StringSplitOptions.RemoveEmptyEntries).Length == 5;
    }


    public override JwtPayload? ParseJwtPayload(String? encryptedAccessToken)
    {
        if (String.IsNullOrEmpty(encryptedAccessToken))
        {
            throw new AuthenticationException("Authentication failed. There's no token present.");
        }

        try
        {
            String accessToken = encryptionKey.Decrypt(encryptedAccessToken);
            return base.ParseJwtPayload(accessToken);
        }
        catch (Exception e)
        {
            throw new AuthenticationException($"Authentication failed. Unable to decrypt JWE. Use the options.ConfigureJweParser(..) method to configure JWE handling correctly or implement the '{typeof(ITokenParser)}' class.", e);
        }
    }

    #endregion

    #endregion
}