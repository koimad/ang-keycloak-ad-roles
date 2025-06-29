using System.Net;

using Company.iFX.BFF.IdentityModel.Client.Messages;

using Microsoft.AspNetCore.Http;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Company.iFX.BFF.IdentityModel.Middleware;

public class TokenIntrospectionMiddleware : IMiddleware
{
    #region Members

    private readonly ILogger<TokenIntrospectionMiddleware> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
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

        String? header = context.Request.Headers["Authorization"].FirstOrDefault();

        if (!String.IsNullOrEmpty(header) && header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            String token = header.Substring("Bearer ".Length).Trim();

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
                _logger.LogError("Token introspection failed: {Error}", response.Error);
                return;
            }
            
            if (!response.IsActive)
            {
                _logger.LogWarning("Token is not active (revoked or expired). Token: {Token}", token);
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Token is not active (revoked or expired).");
                return;
            }
        }

        await next(context);
    }

    #endregion

    #endregion
}