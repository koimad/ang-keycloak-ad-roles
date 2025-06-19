using System.Net.Http.Headers;

using Yarp.ReverseProxy.Transforms;
using Yarp.ReverseProxy.Transforms.Builder;

namespace Company.iFX.BFF;

internal class HttpHeaderTransformation : ITransformProvider
{
    #region Methods

    #region Public

    public void Apply(TransformBuilderContext context)
    {
        context.AddRequestTransform(x =>
        {
            if (!AuthSession.HasAccessToken(x.HttpContext.Session))
            {
                return ValueTask.CompletedTask;
            }

            String? token = AuthSession.GetAccessTokenFromSession(x.HttpContext.Session);
            x.ProxyRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return ValueTask.CompletedTask;
        });
    }


    public void ValidateCluster(TransformClusterValidationContext context)
    {
    }


    public void ValidateRoute(TransformRouteValidationContext context)
    {
    }

    #endregion

    #endregion
}