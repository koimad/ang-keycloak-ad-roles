using Company.iFX.BFF.IdentityModel;
using Company.iFX.BFF.IdentityModel.Client.Messages;
using Company.iFX.BFF.IdentityModel.Internal;

// ReSharper disable once CheckNamespace
namespace System.Net.Http;

public static class HttpClientDeviceFlowExtensions
{
    #region Methods

    #region Public

    public static async Task<DeviceAuthorizationResponse> RequestDeviceAuthorizationAsync(this HttpMessageInvoker client, DeviceAuthorizationRequest request, CancellationToken cancellationToken = default)
    {
        ProtocolRequest clone = request.Clone();

        clone.Parameters.AddOptional(OidcConstants.AuthorizeRequest.Scope, request.Scope);
        clone.Method = HttpMethod.Post;
        clone.Prepare();

        clone.Content ??= new FormUrlEncodedContent(new List<KeyValuePair<String, String>>());

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
            return ProtocolResponse.FromException<DeviceAuthorizationResponse>(ex);
        }

        return await ProtocolResponse.FromHttpResponseAsync<DeviceAuthorizationResponse>(response).ConfigureAwait();
    }

    #endregion

    #endregion
}