using System.IdentityModel.Tokens.Jwt;

using Company.iFX.BFF.Jwt;
using Company.iFX.BFF.OpenIdConnect;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Company.iFX.BFF.Endpoints;

public static class ProfileEndpoint
{
    #region Methods

    #region Public

    public static async Task<IResult> Get(HttpContext context,
        [FromServices] AuthSession authSession,
        [FromServices] ITokenParser tokenParser,
        [FromServices] IClaimsTransformation claimsTransformation)
    {
        if (!authSession.HasIdToken())
        {
            return Results.Unauthorized();
        }

        context.Response.Headers.CacheControl = "no-cache, no-store, must-revalidate";

        String? idToken = authSession.GetIdToken();
        JwtPayload? payload = tokenParser.ParseIdToken(idToken);

        if (payload == null)
        {
            return Results.Ok(new Object());
        }

        Object claims = await claimsTransformation.Transform(payload);
        return Results.Ok(claims);
    }

    #endregion

    #endregion
}