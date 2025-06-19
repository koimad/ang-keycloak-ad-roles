using System.Collections.Immutable;

using Yarp.ReverseProxy.Configuration;

// ReSharper disable once CheckNamespace
namespace Company.iFX.BFF;

public static class YarpConfigExtensions
{
    #region Methods

    #region Private

    private static RouteConfig ToRouteConfig(this KeyValuePair<String, RouteConfig> source)
    {
        return new RouteConfig {
            RouteId = source.Key,
            Match = source.Value.Match,
            Order = source.Value.Order,
            ClusterId = source.Value.ClusterId,
            AuthorizationPolicy = source.Value.AuthorizationPolicy,
            CorsPolicy = source.Value.CorsPolicy,
            MaxRequestBodySize = source.Value.MaxRequestBodySize,
            Metadata = source.Value.Metadata,
            Transforms = source.Value.Transforms
        };
    }

    #endregion

    #region Public

    public static IReadOnlyList<ClusterConfig> ToClusterConfig(this Dictionary<String, ClusterConfig> source)
    {
        return source.Select(x => x.ToClusterConfig()).ToImmutableArray();
    }


    public static ClusterConfig ToClusterConfig(this KeyValuePair<String, ClusterConfig> source)
    {
        return new ClusterConfig {
            ClusterId = source.Key,
            LoadBalancingPolicy = source.Value.LoadBalancingPolicy,
            SessionAffinity = source.Value.SessionAffinity,
            HealthCheck = source.Value.HealthCheck,
            HttpClient = source.Value.HttpClient,
            HttpRequest = source.Value.HttpRequest,
            Destinations = source.Value.Destinations,
            Metadata = source.Value.Metadata
        };
    }


    public static IReadOnlyList<RouteConfig> ToRouteConfig(this Dictionary<String, RouteConfig> source)
    {
        return source.Select(x => x.ToRouteConfig()).ToImmutableArray();
    }

    #endregion

    #endregion
}