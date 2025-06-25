using Company.iFX.BFF.IdentityModel.Client.Messages;
using Company.iFX.BFF.IdentityModel.Internal;

namespace Company.iFX.BFF.IdentityModel.Client.Extensions;

public static class HttpClientTokenRevocationExtensions
{
    #region Methods

    #region Public

    public static async Task<TokenRevocationResponse> RevokeTokenAsync(this HttpMessageInvoker client, TokenRevocationRequest request, CancellationToken cancellationToken = default)
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
            return ProtocolResponse.FromException<TokenRevocationResponse>(ex);
        }

        return await ProtocolResponse.FromHttpResponseAsync<TokenRevocationResponse>(response).ConfigureAwait();
    }

    #endregion

    #endregion
}