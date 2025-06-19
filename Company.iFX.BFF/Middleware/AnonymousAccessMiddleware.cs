using Company.iFX.BFF.IdentityProviders;
using Company.iFX.BFF.Logging;

using Microsoft.AspNetCore.Http;

namespace Company.iFX.BFF.Middleware;

internal class AnonymousAccessMiddleware : IMiddleware
{
    #region Members

    private readonly IAuthSession _authSession;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger _logger;
    private readonly EndpointName _oidcProxyReservedEndpointName;

    #endregion

    #region Constructors

    public AnonymousAccessMiddleware(EndpointName oidcProxyReservedEndpointName, IAuthSession authSession, ILogger logger, IHttpContextAccessor httpContextAccessor)
    {
        _oidcProxyReservedEndpointName = oidcProxyReservedEndpointName;
        _authSession = authSession;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    #endregion

    #region Methods

    #region Public

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        String currentPath = context.Request.Path + context.Request.QueryString;

        if (currentPath.StartsWith(_oidcProxyReservedEndpointName.ToString(), StringComparison.InvariantCultureIgnoreCase))
        {
            await next(context);
            return;
        }

        ISession? session = _httpContextAccessor.HttpContext?.Session;

        if (session?.GetAccessToken() != null)
        {
            await next(context);
            return;
        }

        AuthorizeRequest authorizeRequest = await _authSession.InitiateAuthenticationSequence(currentPath);

        await _logger.InformAsync($"Redirect({authorizeRequest.AuthorizeUri})");

        context.Response.Redirect(authorizeRequest.AuthorizeUri.ToString());
    }

    #endregion

    #endregion
}