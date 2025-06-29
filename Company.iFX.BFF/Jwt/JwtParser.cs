using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Company.iFX.BFF.Jwt;

public class JwtParser : ITokenParser
{
    #region Methods

    #region Private

    private static String? Decode(String? token)
    {
        if (String.IsNullOrEmpty(token))
        {
            return null;
        }

        String urlEncodedMiddleSection = GetSection(token, 1);

        String middleSection = urlEncodedMiddleSection
            .Replace('-', '+')
            .Replace('_', '/');

        String base64 = middleSection
            .PadRight(middleSection.Length + (4 - middleSection.Length % 4) % 4, '=');

        Byte[] bytes = Convert.FromBase64String(base64);
        return Encoding.UTF8.GetString(bytes);
    }


    private static String GetSection(String token, Int32 section)
    {
        String[] chunks = token.Split(".");

        return chunks.Length != 3 ? throw new NotSupportedException($"Invalid token: {token}") : chunks[section];
    }


    private static JwtPayload? ParsePayload(String? token)
    {
        if (String.IsNullOrEmpty(token))
        {
            return null;
        }

        String? json = Decode(token);

        return String.IsNullOrEmpty(json)
            ? null
            : JwtPayload.Deserialize(json);
    }

    #endregion

    #region Public

    public virtual JwtPayload? ParseIdToken(String? idToken)
    {
        return ParsePayload(idToken);
    }


    public static JwtHeader? ParseJwtHeader(String? token)
    {
        if (String.IsNullOrEmpty(token))
        {
            return null;
        }

        String[] parts = token.Split('.', StringSplitOptions.RemoveEmptyEntries);

        return parts.Length != 3 && parts.Length != 5 // A JWT has 3 parts, a JWE has 5.
            ? null
            : JwtHeader.Base64UrlDeserialize(parts[0]);
    }


    public virtual JwtPayload? ParseJwtPayload(String? token)
    {
        return ParsePayload(token);
    }

    #endregion

    #endregion
}