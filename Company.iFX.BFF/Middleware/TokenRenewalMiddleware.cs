using Company.iFX.BFF.Logging;
using Company.iFX.BFF.OpenIdConnect;

using Microsoft.AspNetCore.Http;

namespace Company.iFX.BFF.Middleware;

internal class TokenRenewalMiddleware : IYarpMiddleware
{
    #region Members

    private readonly ILogger _logger;
    private readonly TokenFactory _tokenFactory;

    #endregion

    #region Constructors

    public TokenRenewalMiddleware(TokenFactory tokenFactory, ILogger logger)
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
        catch (TokenRenewalFailedException e)
        {
            await _logger.ErrorAsync(e);

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(@"{ ""reason"": ""token_renewal_failed"" }");
            return;
        }

        await next(context);
    }

    #endregion

    #endregion
}