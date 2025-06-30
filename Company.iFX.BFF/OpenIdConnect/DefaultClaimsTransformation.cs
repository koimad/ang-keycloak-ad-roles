using System.IdentityModel.Tokens.Jwt;

namespace Company.iFX.BFF.OpenIdConnect;

public class DefaultClaimsTransformation : IClaimsTransformation
{
    #region Methods

    #region Public

    public Task<Object> Transform(JwtPayload payload)
    {
        return Task.FromResult<Object>(payload);
    }

    #endregion

    #endregion
}