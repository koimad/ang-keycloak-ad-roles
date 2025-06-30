using Company.iFX.BFF.Locking;
using Company.iFX.BFF.Locking.Distributed.Redis;
using Company.iFX.BFF.Locking.InMemory;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

using RedLockNet;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;

using StackExchange.Redis;

namespace Company.iFX.BFF.ModuleInitializers.Configuration;

public class SessionBootstrap : IBootstrap
{
    #region Members

    private ConnectionMultiplexer? _connectionMultiplexer;

    #endregion

    #region Methods

    #region Public

    public void Configure(ProxyOptions options, IServiceCollection services)
    {
        services
            .AddDistributedMemoryCache()
            .AddMemoryCache()
            .AddSession(o =>
            {
                o.IdleTimeout = options.SessionIdleTimeout;
                o.Cookie.HttpOnly = true;
                o.Cookie.IsEssential = true;
                o.Cookie.Name = options.CookieName;
            });

        if (_connectionMultiplexer == null)
        {
            services.AddTransient<IConcurrentContext, InMemoryConcurrentContext>();
        }
        else
        {
            services
                .AddDataProtection()
                .PersistKeysToStackExchangeRedis(_connectionMultiplexer, options.CookieName);

            services.AddStackExchangeRedisCache(redisCacheOptions =>
            {
                redisCacheOptions.Configuration = _connectionMultiplexer.Configuration;
                redisCacheOptions.InstanceName = options.CookieName;
            });

            services
                .AddTransient<IConcurrentContext, RedisConcurrentContext>()
                .AddTransient<IDistributedLockFactory>(_ => RedLockFactory.Create(new List<RedLockMultiplexer> { _connectionMultiplexer }));
        }
    }


    public void Configure(ProxyOptions options, WebApplication app)
    {
        app.UseSession();

        app.Use(async (context, next) =>
        {
            await context.Session.LoadAsync();
            await next();
        });
    }


    public SessionBootstrap WithRedis(ConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
        return this;
    }

    #endregion

    #endregion
}