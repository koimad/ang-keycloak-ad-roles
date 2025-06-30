using Company.iFX.BFF.IdentityProviders;
using Company.iFX.BFF.ModuleInitializers.Configuration;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Company.iFX.BFF.ModuleInitializers;

public static class ModuleInitializer
{
    #region Members

    private static ProxyOptions _options = new();

    #endregion

    #region Methods

    #region Public

    public static IServiceCollection AddOidcProxy<TIdentityProvider>(this IServiceCollection serviceCollection, Action<ProxyOptions>? configureOptions = null)
        where TIdentityProvider : class, IIdentityProvider
    {
        return AddOidcProxy<TIdentityProvider, DefaultAppSettingsSection>(serviceCollection,
            new DefaultAppSettingsSection(),
            configureOptions);
    }


    public static IServiceCollection AddOidcProxy<TIdentityProvider, TAppSettingsSection>(this IServiceCollection serviceCollection, TAppSettingsSection appSettingsSection, Action<ProxyOptions>? configureOptions = null)
        where TIdentityProvider : class, IIdentityProvider
        where TAppSettingsSection : class, IAppSettingsSection
    {
        appSettingsSection.Apply(_options);
        configureOptions?.Invoke(_options);

        _options.RegisterIdentityProvider<TIdentityProvider, TAppSettingsSection>(appSettingsSection);

        IEnumerable<IBootstrap> configuration = _options.GetConfiguration();

        foreach (IBootstrap option in configuration)
        {
            option.Configure(_options, serviceCollection);
        }

        return serviceCollection;
    }


    public static void Reset() // Do not remove: Used for integration testing...
    {
        _options = new ProxyOptions();
    }


    public static WebApplication UseOidcProxy(this WebApplication app)
    {
        IEnumerable<IBootstrap> configuration = _options.GetConfiguration();

        foreach (IBootstrap option in configuration)
        {
            option.Configure(_options, app);
        }

        return app;
    }

    #endregion

    #endregion
}