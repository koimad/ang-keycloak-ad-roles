using Yarp.ReverseProxy.Configuration;

// ReSharper disable once CheckNamespace
namespace Company.iFX.BFF;

public class YarpConfig
{
    #region Properties

    public Dictionary<String, RouteConfig> Routes { get; set; } = new();
    public Dictionary<String, ClusterConfig> Clusters { get; set; } = new();

    #endregion
}