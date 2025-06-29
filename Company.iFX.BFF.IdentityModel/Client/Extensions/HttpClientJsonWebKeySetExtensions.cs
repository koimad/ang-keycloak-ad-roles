using System.Net.Http.Headers;

using Company.iFX.BFF.IdentityModel.Client.Messages;
using Company.iFX.BFF.IdentityModel.Internal;

namespace Company.iFX.BFF.IdentityModel.Client.Extensions;

public static class HttpClientJsonWebKeySetExtensions
{
    #region Methods

    #region Public

    public static async Task<JsonWebKeySetResponse> GetJsonWebKeySetAsync(this HttpMessageInvoker client, String? address = null, CancellationToken cancellationToken = default)
    {
        return await client.GetJsonWebKeySetAsync(new JsonWebKeySetRequest {
            Address = address
        }, cancellationToken).ConfigureAwait();
    }


    public static async Task<JsonWebKeySetResponse> GetJsonWebKeySetAsync(this HttpMessageInvoker client, JsonWebKeySetRequest request, CancellationToken cancellationToken = default)
    {
        ProtocolRequest clone = request.Clone();

        clone.Method = HttpMethod.Get;
        clone.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/jwk-set+json"));
        clone.Prepare();

        HttpResponseMessage response;

        try
        {
            response = await client.SendAsync(clone, cancellationToken).ConfigureAwait();

            if (!response.IsSuccessStatusCode)
            {
                return await ProtocolResponse.FromHttpResponseAsync<JsonWebKeySetResponse>(response, $"Error connecting to {clone.RequestUri!.AbsoluteUri}: {response.ReasonPhrase}").ConfigureAwait();
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return ProtocolResponse.FromException<JsonWebKeySetResponse>(ex, $"Error connecting to {clone.RequestUri!.AbsoluteUri}. {ex.Message}.");
        }

        return await ProtocolResponse.FromHttpResponseAsync<JsonWebKeySetResponse>(response).ConfigureAwait();
    }

    #endregion

    #endregion
}