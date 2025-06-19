using Microsoft.Extensions.DependencyInjection;

using Company.iFX.BFF.ModuleInitializers;

namespace Company.iFX.BFF.OIDC;

public static class ModuleInitializer
{
    #region Methods

    #region Public

    public static IServiceCollection AddOidcProxy(this IServiceCollection serviceCollection,
        OidcProxyConfig config,
        Action<ProxyOptions>? configureOptions = null)
    {
        return AddOidcProxy<OpenIdConnectIdentityProvider>(serviceCollection, config, configureOptions);
    }


    public static IServiceCollection AddOidcProxy<TIdentityProvider>(this IServiceCollection serviceCollection,
        OidcProxyConfig config,
        Action<ProxyOptions>? configureOptions = null) where TIdentityProvider : OpenIdConnectIdentityProvider
    {
        if (config == null)
        {
            throw new ArgumentNullException(nameof(config), "Failed to initialise Company.iFX.BFF. Config cannot be null. " +
                                                            $"Invoke `builder.Services.AddOidcProxy(..)` with an instance of `{nameof(OidcProxyConfig)}`.");
        }

        if (config.Oidc == null)
        {
            throw new ArgumentException("Failed to initialise Company.iFX.BFF. " +
                                        $"Invoke `builder.Services.AddOidcProxy(..)` with an instance of `{nameof(OidcProxyConfig)}` " +
                                        $"and provide a value for {nameof(OidcProxyConfig)}.{nameof(config.Oidc)}.");
        }

        if (!config.Validate(out var errors))
        {
            throw new NotSupportedException(String.Join(", ", errors));
        }

        serviceCollection.AddSingleton(config.Oidc);

        return serviceCollection.AddOidcProxy<TIdentityProvider>(options =>
        {
            config.Apply(options);
            configureOptions?.Invoke(options);
        });
    }

    #endregion

    #endregion
}