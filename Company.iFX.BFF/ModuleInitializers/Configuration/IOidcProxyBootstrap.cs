using Company.iFX.BFF.OpenIdConnect;

namespace Company.iFX.BFF.ModuleInitializers.Configuration;

public interface IOidcProxyBootstrap : IBootstrap
{
    IOidcProxyBootstrap WithCallbackHandler<TCallbackHandler>() where TCallbackHandler : IAuthenticationCallbackHandler;


    IOidcProxyBootstrap WithClaimsTransformation<TClaimsTransformation>() where TClaimsTransformation : IClaimsTransformation;


    IOidcProxyBootstrap WithRedirectUriFactory<TRedirectUriFactory>() where TRedirectUriFactory : IRedirectUriFactory;
}