using System.Net;

using Company.iFX.BFF.IdentityModel.Client.Messages;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Company.iFX.BFF.IdentityModel.Middleware;

public class TokenIntrospectionMiddleware : IMiddleware
{
    #region Members

    private const String _authorizationHeader = "Authorization";
    private const String _bearer = "Bearer ";
    private const String? _tokenIntrospectionFailedError = "Token introspection failed: {Error}";
    private const String _tokenIsNotActiveRevokedOrExpired = "Token is not active (revoked or expired).";
    private const String? _tokenIsNotActiveRevokedOrExpiredTokenToken = "Token is not active (revoked or expired). Token: {Token}";
    private readonly IHttpClientFactory _httpClientFactory;

    private readonly ILogger<TokenIntrospectionMiddleware> _logger;
    private readonly TokenIntrospectionOptions _options;

    #endregion

    #region Constructors

    public TokenIntrospectionMiddleware(ILogger<TokenIntrospectionMiddleware> logger, IHttpClientFactory httpClientFactory, IOptions<TokenIntrospectionOptions> options)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _options = options.Value;
    }

    #endregion

    #region Methods

    #region Public

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _logger.LogDebug("TokenIntrospectionMiddleware invoked for {Path}", context.Request.Path);

        String? header = context.Request.Headers[_authorizationHeader].FirstOrDefault();

        if (!String.IsNullOrEmpty(header) && header.StartsWith(_bearer, StringComparison.OrdinalIgnoreCase))
        {
            String token = header.Substring(_bearer.Length).Trim();

            HttpClient client = _httpClientFactory.CreateClient();

            TokenIntrospectionRequest tokenRequest = new TokenIntrospectionRequest {
                Address = _options.Address,
                ClientId = _options.ClientId,
                ClientSecret = _options.ClientSecret,
                Token = token
            };

            TokenIntrospectionResponse response = await client.IntrospectTokenAsync(tokenRequest);

            if (response.IsError)
            {
                context.Response.StatusCode = (Int32)HttpStatusCode.InternalServerError;
                _logger.LogError(_tokenIntrospectionFailedError, response.Error);
                return;
            }

            if (!response.IsActive)
            {
                _logger.LogWarning(_tokenIsNotActiveRevokedOrExpiredTokenToken, token);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync(_tokenIsNotActiveRevokedOrExpired);
                return;
            }
        }

        await next(context);
    }

    #endregion

    #endregion
}