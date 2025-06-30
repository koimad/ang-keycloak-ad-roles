using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Encodings.Web;

using Company.iFX.BFF.Jwt;
using Company.iFX.BFF.ModuleInitializers;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Company.iFX.BFF.Middleware;

public sealed class OidcProxyAuthenticationHandler : AuthenticationHandler<OidcProxyAuthenticationSchemeOptions>
{
    #region Members

    public const String SchemaName = "Company.iFX.BFF";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ProxyOptions _proxyOptions;
    private readonly ITokenParser _tokenParser;

    #endregion

    #region Constructors

    public OidcProxyAuthenticationHandler(ITokenParser tokenParser, ProxyOptions proxyOptions,
        IHttpContextAccessor httpContextAccessor, IOptionsMonitor<OidcProxyAuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder)
    {
        _tokenParser = tokenParser;
        _proxyOptions = proxyOptions;
        _httpContextAccessor = httpContextAccessor;
    }

    #endregion

    #region Methods

    #region Protected

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            if (_httpContextAccessor.HttpContext == null)
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            String? token = _httpContextAccessor.HttpContext.Session.GetAccessToken();

            if (String.IsNullOrEmpty(token))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            JwtPayload? payload = _tokenParser.ParseJwtPayload(token);

            if (payload == null)
            {
                throw new AuthenticationFailureException("Failed to authenticate. The access_token jwt does not have a payload.");
            }

            Claim[] claims = payload
                .Select(x => new Claim(x.Key, x.Value?.ToString() ?? String.Empty))
                .ToArray();

            if (!claims.Any())
            {
                throw new AuthenticationFailureException("Failed to authenticate. The access_token jwt does not contain any claims.");
            }

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, SchemaName, _proxyOptions.NameClaim, _proxyOptions.RoleClaim);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            AuthenticationTicket ticket = new AuthenticationTicket(claimsPrincipal, SchemaName);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch (Exception e)
        {
            return Task.FromResult(AuthenticateResult.Fail(e));
        }
    }

    #endregion

    #endregion
}