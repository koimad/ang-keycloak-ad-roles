using Company.iFX.BFF.OpenIdConnect;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Company.iFX.BFF.Middleware;

public class TokenRenewalMiddleware : IYarpMiddleware
{
    #region Members

    private readonly ILogger<TokenRenewalMiddleware> _logger;
    private readonly TokenFactory _tokenFactory;

    #endregion

    #region Constructors

    public TokenRenewalMiddleware(TokenFactory tokenFactory, ILogger<TokenRenewalMiddleware> logger)
    {
        _tokenFactory = tokenFactory;
        _logger = logger;
    }

    #endregion

    #region Methods

    #region Public

    public async Task Apply(HttpContext context, Func<HttpContext, Task> next)
    {
        try
        {
            await _tokenFactory.RenewAccessTokenIfExpiredAsync(context.TraceIdentifier);
        }
        catch (TokenRenewalNoUserSessionException e)
        {
            _logger.LogError(e, "Error Invoking Token Renewal Endpoint No Valid Session Found to Renew");

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(@"{ ""reason"": ""token_renewal_failed_no_user_session_found"" }");
            return;
        }
        catch (TokenRenewalFailedException e)
        {
            _logger.LogError(e,"Error Invoking Token Renewal Endpoint" );

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(@"{ ""reason"": ""token_renewal_failed"" }");
            return;
        }

        await next(context);
    }

    #endregion

    #endregion
}