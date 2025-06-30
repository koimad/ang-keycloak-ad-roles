using Company.iFX.BFF.Endpoints;
using Company.iFX.BFF.IdentityProviders;
using Company.iFX.BFF.Jwt.SignatureValidation;
using Company.iFX.BFF.Middleware;
using Company.iFX.BFF.OpenIdConnect;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Company.iFX.BFF.ModuleInitializers.Configuration;

public class OidcProxyBootstrap<TIdentityProvider, TIdentityProviderConfig> : IOidcProxyBootstrap
    where TIdentityProvider : class, IIdentityProvider
    where TIdentityProviderConfig : class
{
    #region Members

    private Type _callbackHandlerType = typeof(DefaultAuthenticationCallbackHandler);

    private Type _claimsTransformationType = typeof(DefaultClaimsTransformation);
    private readonly TIdentityProviderConfig _config;

    private Type _redirectUrlFactory = typeof(RedirectUriFactory);

    #endregion

    #region Constructors

    public OidcProxyBootstrap(TIdentityProviderConfig config)
    {
        _config = config;
    }

    #endregion

    #region Methods

    #region Public

    public void Configure(ProxyOptions options, IServiceCollection services)
    {
        services
            .AddTransient<AnonymousAccessMiddleware>();

        services
            .AddSingleton<EndpointName>(_ => new EndpointName(options.EndpointName))
            .AddTransient<TokenRenewalMiddleware>()
            .AddTransient<TokenIntrospectionMiddleware>()
            .AddTransient<IIdentityProvider, TIdentityProvider>()
            .AddTransient<TIdentityProviderConfig>(_ => _config)
            .AddTransient(_ => options)
            .AddHttpClient<TIdentityProvider>();

        services
            .AddHttpContextAccessor()
            .AddTransient<TokenFactory>()
            .AddTransient<AuthSession>()
            .AddTransient<IAuthSession, AuthSession>()
            ;

        services
            .AddTransient<Rs256SignatureValidator>();

        services
            .AddTransient(typeof(IAuthenticationCallbackHandler), _callbackHandlerType)
            .AddTransient(typeof(IClaimsTransformation), _claimsTransformationType)
            .AddTransient(typeof(IRedirectUriFactory), _redirectUrlFactory);
    }


    public void Configure(ProxyOptions options, WebApplication app)
    {
        app.UseMiddleware<AnonymousAccessMiddleware>();
        app.MapAuthenticationEndpoints(options.EndpointName);
    }


    public IOidcProxyBootstrap WithCallbackHandler<TCallbackHandler>() where TCallbackHandler : IAuthenticationCallbackHandler
    {
        _callbackHandlerType = typeof(TCallbackHandler);
        return this;
    }


    public IOidcProxyBootstrap WithClaimsTransformation<TClaimsTransformation>() where TClaimsTransformation : IClaimsTransformation
    {
        _claimsTransformationType = typeof(TClaimsTransformation);
        return this;
    }


    public IOidcProxyBootstrap WithRedirectUriFactory<TRedirectUriFactory>() where TRedirectUriFactory : IRedirectUriFactory
    {
        _redirectUrlFactory = typeof(TRedirectUriFactory);
        return this;
    }

    #endregion

    #endregion
}