using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Company.iFX.BFF.Middleware;

internal static class WebApplicationExtensions
{
    #region Methods

    #region Public

    public static void RegisterYarpMiddleware(this WebApplication app, IEnumerable<Type> types)
    {
        if (!types.Any())
        {
            return;
        }

        app.MapReverseProxy(x => x.Use((context, next) =>
        {
            List<Func<HttpContext, Task>> delegates = new List<Func<HttpContext, Task>> { _ => next() };

            foreach (Type type in types)
            {
                IYarpMiddleware instance = (IYarpMiddleware)x.ApplicationServices.GetRequiredService(type);
                Func<HttpContext, Task> previous = delegates.Last();
                delegates.Add(ctx => instance.Apply(ctx, previous));
            }

            return delegates.Last()(context);
        }));
    }

    #endregion

    #endregion
}