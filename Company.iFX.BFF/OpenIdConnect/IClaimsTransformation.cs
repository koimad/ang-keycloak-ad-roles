using System.IdentityModel.Tokens.Jwt;

namespace Company.iFX.BFF.OpenIdConnect;

public interface IClaimsTransformation
{
    Task<Object> Transform(JwtPayload payload);
}