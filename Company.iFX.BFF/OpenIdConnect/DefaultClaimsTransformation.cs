using System.IdentityModel.Tokens.Jwt;

namespace Company.iFX.BFF.OpenIdConnect;

internal class DefaultClaimsTransformation : IClaimsTransformation
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