using Company.iFX.BFF.IdentityModel.Client.Extensions;
using Company.iFX.BFF.IdentityModel.Client.Messages;
using Company.iFX.BFF.IdentityModel.Internal;

namespace Company.iFX.BFF.IdentityModel.Client;
public class DiscoveryCache : IDiscoveryCache
{
    #region Members

    private readonly String _authority;
    private readonly Func<HttpMessageInvoker> _getHttpClient;
    private AsyncLazy<DiscoveryDocumentResponse>? _lazyResponse;
    private DateTime _nextReload = DateTime.MinValue;

    private readonly DiscoveryPolicy _policy;

    #endregion

    #region Properties

    public TimeSpan CacheDuration { get; set; } = TimeSpan.FromHours(24);

    #endregion

    #region Constructors

    public DiscoveryCache(String authority, DiscoveryPolicy? policy = null)
    {
        _authority = authority;
        _policy = policy ?? new DiscoveryPolicy();
        _getHttpClient = () => new HttpClient();
    }


    public DiscoveryCache(String authority, Func<HttpMessageInvoker> httpClientFunc, DiscoveryPolicy? policy = null)
    {
        _authority = authority;
        _policy = policy ?? new DiscoveryPolicy();
        _getHttpClient = httpClientFunc ?? throw new ArgumentNullException(nameof(httpClientFunc));
    }

    #endregion

    #region Methods

    #region Private

    private async Task<DiscoveryDocumentResponse> GetResponseAsync()
    {
        DiscoveryDocumentResponse result = await _getHttpClient().GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest {
            Address = _authority,
            Policy = _policy
        }).ConfigureAwait();

        if (result.IsError)
        {
            Refresh();
            _nextReload = DateTime.MinValue;
        }
        else
        {
            _nextReload = DateTime.UtcNow.Add(CacheDuration);
        }

        return result;
    }

    #endregion

    #region Public

    public Task<DiscoveryDocumentResponse> GetAsync()
    {
        if (_nextReload <= DateTime.UtcNow)
        {
            Refresh();
        }

        return _lazyResponse!.Value;
    }


    public void Refresh()
    {
        _lazyResponse = new AsyncLazy<DiscoveryDocumentResponse>(GetResponseAsync);
    }

    #endregion

    #endregion
}