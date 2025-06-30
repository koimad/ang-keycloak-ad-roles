using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using System.IdentityModel.Tokens.Jwt;

namespace Company.iFX.BFF;

public class DefaultAuthenticationCallbackHandler : IAuthenticationCallbackHandler
{
    #region Members

    private readonly ILogger _logger;

    #endregion

    #region Constructors

    public DefaultAuthenticationCallbackHandler(ILogger<DefaultAuthenticationCallbackHandler> logger)
    {
        _logger = logger;
    }

    #endregion

    #region Methods

    #region Public

    public virtual Task<IResult> OnAuthenticated(HttpContext context, JwtPayload? jwtPayload, String defaultLandingPage, String? userPreferredLandingPage)
    {
        String landingPage = userPreferredLandingPage ?? defaultLandingPage;
        _logger.LogInformation($"Redirect({landingPage})");
        return Task.FromResult(Results.Redirect(landingPage));
    }


    public virtual Task<IResult> OnAuthenticationFailed(HttpContext context, String defaultLandingPage, String? userPreferredLandingPage)
    {
        IResult result = Results.Unauthorized();
        return Task.FromResult(result);
    }


    public virtual Task OnError(HttpContext context, Exception e)
    {
        return Task.CompletedTask;
    }

    #endregion

    #endregion
}