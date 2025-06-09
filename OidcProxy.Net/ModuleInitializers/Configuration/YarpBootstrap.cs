using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using OidcProxy.Net.Middleware;

namespace OidcProxy.Net.ModuleInitializers.Configuration;

internal class YarpBootstrap : IBootstrap
{
    #region Members

    private readonly IList<Action<IReverseProxyBuilder>> _configuration = new List<Action<IReverseProxyBuilder>>();
    private readonly List<Type> _yarpMiddlewareRegistrations = [typeof(TokenRenewalMiddleware)];

    #endregion

    #region Methods

    #region Public

    public void AddYarpMiddleware(IEnumerable<Type> handlerType)
    {
        _yarpMiddlewareRegistrations.AddRange(handlerType);
    }


    public void Configure(ProxyOptions options, IServiceCollection services)
    {
        IReverseProxyBuilder proxyBuilder = services
            .AddReverseProxy()
            .AddTransforms<HttpHeaderTransformation>();

        foreach (Action<IReverseProxyBuilder> config in _configuration)
        {
            config.Invoke(proxyBuilder);
        }
    }


    public void Configure(ProxyOptions options, WebApplication app)
    {
        app.RegisterYarpMiddleware(_yarpMiddlewareRegistrations);
    }


    public void ConfigureProxyBuilder(Action<IReverseProxyBuilder> configuration)
    {
        _configuration.Add(configuration);
    }

    #endregion

    #endregion
}