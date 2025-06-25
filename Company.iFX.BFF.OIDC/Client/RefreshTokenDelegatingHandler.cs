using System.Net;
using System.Net.Http.Headers;
using System.Text;

using Company.iFX.BFF.OIDC.Client.Results;

using Microsoft.Extensions.Logging;

namespace Company.iFX.BFF.OIDC.Client;

public class RefreshTokenDelegatingHandler : DelegatingHandler
{
    #region Members

    private String _accessToken;
    private readonly String _accessTokenType;

    private Boolean _disposed;
    private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
    private readonly ILogger _logger;
    private readonly OidcClient _oidcClient;
    private String _refreshToken;

    #endregion

    #region Events

    public event EventHandler<TokenRefreshedEventArgs> TokenRefreshed = delegate { };

    #endregion

    #region Properties

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(5);

    public String AccessToken
    {
        get
        {
            if (_lock.Wait(Timeout))
            {
                try
                {
                    return _accessToken;
                }
                finally
                {
                    _lock.Release();
                }
            }

            return null;
        }
    }

    public String? RefreshToken
    {
        get
        {
            if (_lock.Wait(Timeout))
            {
                try
                {
                    return _refreshToken;
                }
                finally
                {
                    _lock.Release();
                }
            }

            return null;
        }
    }

    #endregion

    #region Constructors

    public RefreshTokenDelegatingHandler(OidcClient oidcClient, String accessToken, String refreshToken, String tokenType = "Bearer", HttpMessageHandler? innerHandler = null)
    {
        _oidcClient = oidcClient ?? throw new ArgumentNullException(nameof(oidcClient));
        _accessToken = accessToken;
        _accessTokenType = tokenType;

        _logger = _oidcClient.Options.LoggerFactory.CreateLogger<RefreshTokenDelegatingHandler>();

        if (refreshToken.IsMissing())
        {
            throw new ArgumentNullException(nameof(refreshToken));
        }

        _refreshToken = refreshToken;

        if (innerHandler != null)
        {
            InnerHandler = innerHandler;
        }
    }

    #endregion

    #region Methods

    #region Private

    private async Task<String?> GetAccessTokenAsync(CancellationToken cancellationToken)
    {
        if (await _lock.WaitAsync(Timeout, cancellationToken).ConfigureAwait(false))
        {
            try
            {
                return _accessToken;
            }
            finally
            {
                _lock.Release();
            }
        }

        return null;
    }


    private async Task<Boolean> RefreshTokensAsync(CancellationToken cancellationToken)
    {
        if (await _lock.WaitAsync(Timeout, cancellationToken).ConfigureAwait(false))
        {
            if (_refreshToken.IsMissing())
            {
                return false;
            }

            try
            {
                RefreshTokenResult response = await _oidcClient.RefreshTokenAsync(
                    _refreshToken,
                    null,
                    null,
                    cancellationToken).ConfigureAwait(false);

                if (!response.IsError)
                {
                    _accessToken = response.AccessToken;

                    if (!response.RefreshToken.IsMissing())
                    {
                        _refreshToken = response.RefreshToken;
                    }

                    await Task.Run(() =>
                    {
                        foreach (Delegate @delegate in TokenRefreshed.GetInvocationList())
                        {
                            EventHandler<TokenRefreshedEventArgs> del = (EventHandler<TokenRefreshedEventArgs>)@delegate;

                            try
                            {
                                del(this, new TokenRefreshedEventArgs(response.AccessToken, response.RefreshToken, response.ExpiresIn, response.IdentityToken));
                            }
                            catch { }
                        }
                    }, cancellationToken).ConfigureAwait(false);

                    return true;
                }

                _logger.LogError("Failed on RefreshTokensAsync: {error} - {description}", response.Error, response.ErrorDescription);
            }
            finally
            {
                _lock.Release();
            }
        }

        return false;
    }

    #endregion

    #region Protected

    protected override void Dispose(Boolean disposing)
    {
        if (disposing && !_disposed)
        {
            _disposed = true;
            _lock.Dispose();
        }

        base.Dispose(disposing);
    }


    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        String? accessToken = await GetAccessTokenAsync(cancellationToken);

        if (accessToken.IsMissing())
        {
            if (await RefreshTokensAsync(cancellationToken) == false)
            {
                return new HttpResponseMessage(HttpStatusCode.Unauthorized) { RequestMessage = request };
            }
        }

        request.Headers.Authorization = new AuthenticationHeaderValue(_accessTokenType, AccessToken);
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode != HttpStatusCode.Unauthorized)
        {
            return response;
        }

        if (await RefreshTokensAsync(cancellationToken) == false)
        {
            return response;
        }

        response.Dispose(); // This 401 response will not be used for anything so is disposed to unblock the socket.

        request.Headers.Authorization = new AuthenticationHeaderValue(_accessTokenType, AccessToken);
        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #endregion
}