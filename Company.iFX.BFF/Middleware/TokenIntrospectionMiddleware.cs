using Company.iFX.BFF.IdentityProviders;
using Company.iFX.BFF.OpenIdConnect;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Company.iFX.BFF.Middleware;

public class TokenIntrospectionMiddleware : IYarpMiddleware
{
    #region Members

    private readonly IAuthSession _authSession;
    private readonly IIdentityProvider _identityProvider;
    private readonly ILogger<TokenIntrospectionMiddleware> _logger;

    #endregion

    #region Constructors

    public TokenIntrospectionMiddleware(ILogger<TokenIntrospectionMiddleware> logger, IIdentityProvider identityProvider, IAuthSession authSession)
    {
        _logger = logger;
        _identityProvider = identityProvider;
        _authSession = authSession;
    }

    #endregion

    #region Methods

    #region Public

    public async Task Apply(HttpContext context, Func<HttpContext, Task> next)
    {
        try
        {
            String? idToken = null;

            if (_authSession.HasAccessToken())
            {
                idToken = _authSession.GetAccessToken();
            }

            Boolean isValid = await _identityProvider.IntrospectTokenAsync(idToken);

            if (!isValid)
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync(@"{ ""reason"": ""token_validation_failed"" }");
                return;
            }
        }
        catch (TokenRenewalFailedException e)
        {
            _logger.LogError(e, "Error invoking Token Introspection Endpoint ");

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync(@"{ ""reason"": ""token_validation_failed"" }");
            return;
        }

        await next(context);
    }

    #endregion

    #endregion
}