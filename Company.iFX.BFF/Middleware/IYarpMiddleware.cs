using Microsoft.AspNetCore.Http;

namespace Company.iFX.BFF.Middleware;

public interface IYarpMiddleware
{
    Task Apply(HttpContext context, Func<HttpContext, Task> next);
}