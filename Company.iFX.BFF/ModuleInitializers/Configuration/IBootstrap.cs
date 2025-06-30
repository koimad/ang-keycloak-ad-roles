using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Company.iFX.BFF.ModuleInitializers.Configuration;

public interface IBootstrap
{
    void Configure(ProxyOptions options, IServiceCollection services);


    void Configure(ProxyOptions options, WebApplication app);
}