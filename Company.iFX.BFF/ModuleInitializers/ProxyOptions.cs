using Company.iFX.BFF.Cryptography;
using Company.iFX.BFF.IdentityProviders;
using Company.iFX.BFF.Jwt;
using Company.iFX.BFF.Jwt.SignatureValidation;
using Company.iFX.BFF.Middleware;
using Company.iFX.BFF.ModuleInitializers.Configuration;
using Company.iFX.BFF.OpenIdConnect;

using Microsoft.Extensions.DependencyInjection;

using StackExchange.Redis;

using Yarp.ReverseProxy.Configuration;

namespace Company.iFX.BFF.ModuleInitializers;

public class ProxyOptions
{
    #region Members

    private readonly AuthorizationBootstrap _authorizationBootstrap = new();

    private Action<IOidcProxyBootstrap> _configureCallbackHandler = _ => { };

    private Action<IOidcProxyBootstrap> _configureClaimsTransformation = _ => { };

    private Action<YarpBootstrap> _configureYarpBootstrap = _ => { };

    private readonly List<Type> _customYarpMiddleware = [typeof(TokenIntrospectionMiddleware), typeof(TokenRenewalMiddleware)]; // Order is important need to renew if required before calling introspection endpoint  

    private IOidcProxyBootstrap? _oidcProxyBootstrap;

    private readonly SessionBootstrap _sessionBootstrap = new();

    private readonly YarpBootstrap _yarpBootstrap = new();

    #endregion

    #region Properties

    public LandingPage[] AllowedUserPreferredLandingPages { get; set; } = Array.Empty<LandingPage>();

    public Uri? CustomHostName { get; set; }

    public LandingPage ErrorPage { get; set; } = new LandingPage();

    public LandingPage LandingPage { get; set; } = new LandingPage();

    public String EndpointName { get; set; } = "auth";


    public Mode Mode { get; set; }


    public String CookieName { get; set; } = "oidcproxy.cookie";


    public TimeSpan SessionIdleTimeout { get; set; } = TimeSpan.FromMinutes(20);


    public Boolean AlwaysRedirectToHttps { get; set; } = true;

    public Boolean EnableUserPreferredLandingPages { get; set; } = false;
    public String NameClaim { get; set; } = "sub";

    public String RoleClaim { get; set; } = "role";

    #endregion

    #region Methods

    #region Public

    public void AddAuthenticationCallbackHandler<TAuthenticationCallbackHandler>() where TAuthenticationCallbackHandler : class, IAuthenticationCallbackHandler
    {
        _configureCallbackHandler = oidcProxyBootstrap => oidcProxyBootstrap.WithCallbackHandler<TAuthenticationCallbackHandler>();
    }


    public void AddClaimsTransformation<TClaimsTransformation>() where TClaimsTransformation : class, IClaimsTransformation
    {
        _configureClaimsTransformation = oidcProxyBootstrap => oidcProxyBootstrap.WithClaimsTransformation<TClaimsTransformation>();
    }


    public void AddRedirectUriFactory<TRedirectUriFactory>() where TRedirectUriFactory : class, IRedirectUriFactory
    {
        _configureCallbackHandler = oidcProxyBootstrap => oidcProxyBootstrap.WithRedirectUriFactory<TRedirectUriFactory>();
    }


    public void AddTokenParser<TTokenParser>() where TTokenParser : class, ITokenParser
    {
        _authorizationBootstrap.WithTokenParser<TTokenParser>();
    }


    public void AddYarpMiddleware<THandler>() where THandler : IYarpMiddleware
    {
        _customYarpMiddleware.Add(typeof(THandler));
    }


    public void ConfigureRedisBackBone(ConnectionMultiplexer connectionMultiplexer)
    {
        _sessionBootstrap.WithRedis(connectionMultiplexer);
    }


    public void ConfigureYarp(Action<IReverseProxyBuilder> configuration)
    {
        _configureYarpBootstrap = yarpBootstrap => yarpBootstrap.ConfigureProxyBuilder(configuration);
    }


    public void ConfigureYarp(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
    {
        if (!routes.Any())
        {
            throw new ArgumentException("Failed to initialise Company.iFX.BFF. Invoke `builder.Services.AddOidcProxy(..)` and provide a value for config.ReverseProxy.Routes");
        }

        if (!clusters.Any())
        {
            throw new ArgumentException("Failed to initialise Company.iFX.BFF. Invoke `builder.Services.AddOidcProxy(..)` and provide a value for config.ReverseProxy.Clusters");
        }

        _configureYarpBootstrap = yarpBootstrap => yarpBootstrap
            .ConfigureProxyBuilder(b => b.LoadFromMemory(routes, clusters));
    }


    public IEnumerable<IBootstrap> GetConfiguration()
    {
        if (_oidcProxyBootstrap == null)
        {
            throw new NotSupportedException("Unable to bootstrap Company.iFX.BFF. No IdentityProviders configured.");
        }

        _configureCallbackHandler.Invoke(_oidcProxyBootstrap);
        _configureClaimsTransformation.Invoke(_oidcProxyBootstrap);

        _configureYarpBootstrap.Invoke(_yarpBootstrap);

        _yarpBootstrap.AddYarpMiddleware(_customYarpMiddleware);

        List<IBootstrap?> bootstraps = new List<IBootstrap?>();

        if (Mode != Mode.AuthenticateOnly)
        {
            bootstraps.Add(_yarpBootstrap);
        }

        bootstraps.Add(_sessionBootstrap);
        bootstraps.Add(_oidcProxyBootstrap);
        bootstraps.Add(_authorizationBootstrap);

        return bootstraps
            .Where(x => x != null)
            .Cast<IBootstrap>()
            .ToArray();
    }


    public void RegisterIdentityProvider<TIdentityProvider, TIdentityProviderConfig>(TIdentityProviderConfig config)
        where TIdentityProvider : class, IIdentityProvider
        where TIdentityProviderConfig : class
    {
        if (_oidcProxyBootstrap != null)
        {
            throw new NotSupportedException("Unable to bootstrap Company.iFX.BFF. Configuring multiple IdentityProviders is not supported.");
        }

        _oidcProxyBootstrap = new OidcProxyBootstrap<TIdentityProvider, TIdentityProviderConfig>(config);
    }


    public void RegisterSignatureValidator<TJwtSignatureValidator>() where TJwtSignatureValidator : class, IJwtSignatureValidator
    {
        _authorizationBootstrap.WithSignatureValidator<TJwtSignatureValidator>();
    }


    public void SetAllowedLandingPages(IEnumerable<String>? landingPages)
    {
        if (landingPages == null || !landingPages.Any())
        {
            return;
        }

        List<LandingPage> allowedLandingPages = new List<LandingPage>();

        foreach (String landingPage in landingPages)
        {
            if (!LandingPage.TryParse(landingPage, out LandingPage value))
            {
                const String errorMessage = "Cannot initialize Company.iFX.BFF. Invalid landing page. The path to the landing page must be relative.";

                throw new NotSupportedException(errorMessage);
            }

            allowedLandingPages.Add(value);
        }

        AllowedUserPreferredLandingPages = allowedLandingPages.ToArray();
    }


    public void SetAuthenticationErrorPage(String errorPage)
    {
        const String errorMessage = "Cannot initialize Company.iFX.BFF. Invalid error page. The path to the error page must be relative and may not have a querystring.";

        if (!LandingPage.TryParse(errorPage, out LandingPage value))
        {
            throw new NotSupportedException(errorMessage);
        }

        if (errorPage.Contains('?') || errorPage.Contains('#'))
        {
            throw new NotSupportedException(errorMessage);
        }

        ErrorPage = value;
    }


    public void SetCustomHostName(Uri hostname)
    {
        if (!String.IsNullOrEmpty(hostname.Query))
        {
            throw new NotSupportedException($"Cannot initialize Company.iFX.BFF. Error configuring custom hostname. {hostname} is not a valid hostname. A custom hostname may not have a querystring.");
        }

        CustomHostName = hostname;
    }


    public void SetLandingPage(String landingPage)
    {
        if (!LandingPage.TryParse(landingPage, out LandingPage value))
        {
            const String errorMessage = "Cannot initialize Company.iFX.BFF. Invalid landing page. The path to the landing page must be relative.";

            throw new NotSupportedException(errorMessage);
        }

        LandingPage = value;
    }


    public void UseEncryptionKey(IEncryptionKey key)
    {
        _authorizationBootstrap.WithEncryptionKey(key);
    }


    public void UseSigningKey(SymmetricKey key)
    {
        _authorizationBootstrap.WithSigningKey(key);
    }

    #endregion

    #endregion
}