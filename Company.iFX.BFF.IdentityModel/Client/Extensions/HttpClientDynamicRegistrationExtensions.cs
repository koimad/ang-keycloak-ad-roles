using System.Text;
using System.Text.Json;

using Company.iFX.BFF.IdentityModel.Client.Messages;
using Company.iFX.BFF.IdentityModel.Internal;

// ReSharper disable once CheckNamespace
namespace System.Net.Http;

public static class HttpClientDynamicRegistrationExtensions
{
    #region Methods

    #region Public
    public static async Task<DynamicClientRegistrationResponse> RegisterClientAsync(this HttpMessageInvoker client, DynamicClientRegistrationRequest request, CancellationToken cancellationToken = default)
    {
        ProtocolRequest clone = request.Clone();

        clone.Method = HttpMethod.Post;

        clone.Content = new StringContent(
            JsonSerializer.Serialize(request.Document, ClientMessagesSourceGenerationContext.Default.DynamicClientRegistrationDocument),
            Encoding.UTF8,
            "application/json");
        clone.Prepare();

        if (request.Token.IsPresent())
        {
            clone.SetBearerToken(request.Token!);
        }

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
            return ProtocolResponse.FromException<DynamicClientRegistrationResponse>(ex);
        }

        return await ProtocolResponse.FromHttpResponseAsync<DynamicClientRegistrationResponse>(response).ConfigureAwait();
    }

    #endregion

    #endregion
}