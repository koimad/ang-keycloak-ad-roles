using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Http;

using ILogger = Company.iFX.BFF.Logging.ILogger;

namespace Company.iFX.BFF;

public class DefaultAuthenticationCallbackHandler : IAuthenticationCallbackHandler
{
    #region Members

    private readonly ILogger _logger;

    #endregion

    #region Constructors

    public DefaultAuthenticationCallbackHandler(ILogger logger)
    {
        _logger = logger;
    }

    #endregion

    #region Methods

    #region Public

    public virtual async Task<IResult> OnAuthenticated(HttpContext context, JwtPayload? jwtPayload, String defaultLandingPage, String? userPreferredLandingPage)
    {
        String landingPage = userPreferredLandingPage ?? defaultLandingPage;

        await _logger.InformAsync($"Redirect({landingPage})");
        return Results.Redirect(landingPage);
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