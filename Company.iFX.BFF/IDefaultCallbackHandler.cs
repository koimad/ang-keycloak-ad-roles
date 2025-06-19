using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Http;

namespace Company.iFX.BFF;

public interface IAuthenticationCallbackHandler
{
    Task<IResult> OnAuthenticationFailed(HttpContext context, String defaultLandingPage, String? userPreferredLandingPage);


    Task<IResult> OnAuthenticated(HttpContext context, JwtPayload? jwtPayload, String defaultLandingPage, String? userPreferredLandingPage);


    Task OnError(HttpContext context, Exception e);
}