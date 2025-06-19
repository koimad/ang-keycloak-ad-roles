using Company.iFX.BFF.Cryptography;
using Company.iFX.BFF.Jwt;
using Company.iFX.BFF.Jwt.SignatureValidation;
using Company.iFX.BFF.Middleware;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Company.iFX.BFF.ModuleInitializers.Configuration;

internal class AuthorizationBootstrap : IBootstrap
{
    #region Members

    private Action<IServiceCollection> _applyHs256SignatureValidator = s => s.AddTransient<Hs256SignatureValidator>(_ => null!);

    private Action<IServiceCollection> _applyJwtParser = s => s.AddTransient<ITokenParser, JwtParser>()
        .AddSingleton<IEncryptionKey>(_ => null!);

    private Action<IServiceCollection> _applyJwtValidator = s => s.AddTransient<IJwtSignatureValidator, JwtSignatureValidator>();

    #endregion

    #region Methods

    #region Public

    public void Configure(ProxyOptions options, IServiceCollection services)
    {
        services
            .AddAuthorization()
            .AddAuthentication(OidcProxyAuthenticationHandler.SchemaName)
            .AddScheme<OidcProxyAuthenticationSchemeOptions, OidcProxyAuthenticationHandler>(OidcProxyAuthenticationHandler.SchemaName, null);

        _applyJwtParser(services);
        _applyJwtValidator(services);
        _applyHs256SignatureValidator(services);
    }


    public void Configure(ProxyOptions options, WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }


    public AuthorizationBootstrap WithEncryptionKey(IEncryptionKey key)
    {
        _applyJwtParser = s => s
            .AddTransient<ITokenParser, JweParser>()
            .AddSingleton(key);

        return this;
    }


    public AuthorizationBootstrap WithSignatureValidator<T>() where T : class, IJwtSignatureValidator
    {
        _applyJwtValidator = s => s.AddTransient<IJwtSignatureValidator, T>();

        return this;
    }


    public AuthorizationBootstrap WithSigningKey(SymmetricKey key)
    {
        _applyHs256SignatureValidator = s => s
            .AddTransient<Hs256SignatureValidator>(_ => new Hs256SignatureValidator(key));

        return this;
    }


    public AuthorizationBootstrap WithTokenParser<TTokenParser>() where TTokenParser : class, ITokenParser
    {
        _applyJwtParser = s => s
            .AddTransient<ITokenParser, TTokenParser>();

        return this;
    }

    #endregion

    #endregion
}