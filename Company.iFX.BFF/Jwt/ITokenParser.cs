using System.IdentityModel.Tokens.Jwt;

namespace Company.iFX.BFF.Jwt;

public interface ITokenParser
{
    JwtPayload? ParseJwtPayload(String? accessToken);


    JwtPayload? ParseIdToken(String? idToken);
}