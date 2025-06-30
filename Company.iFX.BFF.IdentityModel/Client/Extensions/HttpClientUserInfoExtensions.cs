using System.Text;

using Company.iFX.BFF.IdentityModel.Client.Messages;
using Company.iFX.BFF.IdentityModel.Internal;

// ReSharper disable once CheckNamespace
namespace System.Net.Http;

public static class HttpClientUserInfoExtensions
{
    #region Members

    private const String _applicationJson = "application/json";

    #endregion

    #region Methods

    #region Public

    public static async Task<UserInfoResponse> GetUserInfoAsync(this HttpMessageInvoker client, UserInfoRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Token.IsMissing())
        {
            throw new ArgumentNullException(nameof(request.Token));
        }

        ProtocolRequest clone = request.Clone();

        clone.Method = HttpMethod.Get;
        clone.SetBearerToken(request.Token!);
        clone.Prepare();

        HttpResponseMessage response;

        try
        {
            response = await client.SendAsync(clone, cancellationToken).ConfigureAwait();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return ProtocolResponse.FromException<UserInfoResponse>(ex);
        }

        Boolean skipJsonParsing = response.Content?.Headers.ContentType?.MediaType != _applicationJson;
        return await ProtocolResponse.FromHttpResponseAsync<UserInfoResponse>(response, skipJson: skipJsonParsing).ConfigureAwait();
    }

    #endregion

    #endregion
}