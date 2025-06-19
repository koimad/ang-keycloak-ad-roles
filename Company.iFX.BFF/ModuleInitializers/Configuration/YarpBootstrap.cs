using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using Company.iFX.BFF.Middleware;

namespace Company.iFX.BFF.ModuleInitializers.Configuration;

internal class YarpBootstrap : IBootstrap
{
    #region Members

    private readonly IList<Action<IReverseProxyBuilder>> _configuration = new List<Action<IReverseProxyBuilder>>();
    private readonly List<Type> _yarpMiddlewareRegistrations = new List<Type>();

    #endregion

    #region Methods

    #region Public

    public void AddYarpMiddleware(IEnumerable<Type> handlerType)
    {
        foreach (Type type in handlerType)
        {
            if (!_yarpMiddlewareRegistrations.Contains(type))
            {
                _yarpMiddlewareRegistrations.Add(type);
            }
        }
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