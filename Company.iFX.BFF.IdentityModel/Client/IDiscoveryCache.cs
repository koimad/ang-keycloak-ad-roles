using Company.iFX.BFF.IdentityModel.Client.Messages;

namespace Company.iFX.BFF.IdentityModel.Client;

public interface IDiscoveryCache
{
    TimeSpan CacheDuration { get; set; }


    Task<DiscoveryDocumentResponse> GetAsync();


    void Refresh();
}