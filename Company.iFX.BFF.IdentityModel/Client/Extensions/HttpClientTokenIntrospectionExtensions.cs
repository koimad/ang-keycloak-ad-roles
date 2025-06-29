using Company.iFX.BFF.IdentityModel;
using Company.iFX.BFF.IdentityModel.Client.Messages;
using Company.iFX.BFF.IdentityModel.Internal;

// ReSharper disable once CheckNamespace
namespace System.Net.Http;

public static class HttpClientTokenIntrospectionExtensions
{
    #region Methods

    #region Public

    public static async Task<TokenIntrospectionResponse> IntrospectTokenAsync(this HttpMessageInvoker client, TokenIntrospectionRequest request, CancellationToken cancellationToken = default)
    {
        ProtocolRequest clone = request.Clone();

        clone.Method = HttpMethod.Post;
        clone.Parameters.AddRequired(OidcConstants.TokenIntrospectionRequest.Token, request.Token);
        clone.Parameters.AddOptional(OidcConstants.TokenIntrospectionRequest.TokenTypeHint, request.TokenTypeHint);
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
            return ProtocolResponse.FromException<TokenIntrospectionResponse>(ex);
        }

        return await ProtocolResponse.FromHttpResponseAsync<TokenIntrospectionResponse>(response).ConfigureAwait();
    }

    #endregion

    #endregion
}