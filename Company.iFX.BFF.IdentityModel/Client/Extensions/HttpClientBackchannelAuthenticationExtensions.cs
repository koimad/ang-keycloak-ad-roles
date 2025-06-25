using Company.iFX.BFF.IdentityModel.Client.Messages;
using Company.iFX.BFF.IdentityModel.Internal;

namespace Company.iFX.BFF.IdentityModel.Client.Extensions;

public static class HttpClientBackchannelAuthenticationExtensions
{
    #region Methods

    #region Public

    public static async Task<BackchannelAuthenticationResponse> RequestBackchannelAuthenticationAsync(this HttpMessageInvoker client, BackchannelAuthenticationRequest request, CancellationToken cancellationToken = default)
    {
        ProtocolRequest clone = request.Clone();

        if (!String.IsNullOrWhiteSpace(request.RequestObject))
        {
            clone.Parameters.AddOptional(OidcConstants.BackchannelAuthenticationRequest.Request, request.RequestObject);
        }
        else
        {
            clone.Parameters.AddRequired(OidcConstants.AuthorizeRequest.Scope, request.Scope);
            clone.Parameters.AddOptional(OidcConstants.BackchannelAuthenticationRequest.ClientNotificationToken, request.ClientNotificationToken);
            clone.Parameters.AddOptional(OidcConstants.BackchannelAuthenticationRequest.AcrValues, request.AcrValues);
            clone.Parameters.AddOptional(OidcConstants.BackchannelAuthenticationRequest.LoginHintToken, request.LoginHintToken);
            clone.Parameters.AddOptional(OidcConstants.BackchannelAuthenticationRequest.LoginHint, request.LoginHint);
            clone.Parameters.AddOptional(OidcConstants.BackchannelAuthenticationRequest.IdTokenHint, request.IdTokenHint);
            clone.Parameters.AddOptional(OidcConstants.BackchannelAuthenticationRequest.BindingMessage, request.BindingMessage);
            clone.Parameters.AddOptional(OidcConstants.BackchannelAuthenticationRequest.UserCode, request.UserCode);

            if (request.RequestedExpiry.HasValue)
            {
                clone.Parameters.AddOptional(OidcConstants.BackchannelAuthenticationRequest.RequestedExpiry, request.RequestedExpiry.ToString());
            }

            foreach (String resource in request.Resource)
            {
                clone.Parameters.AddRequired(OidcConstants.TokenRequest.Resource, resource, true);
            }
        }

        clone.Method = HttpMethod.Post;
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
            return ProtocolResponse.FromException<BackchannelAuthenticationResponse>(ex);
        }

        return await ProtocolResponse.FromHttpResponseAsync<BackchannelAuthenticationResponse>(response).ConfigureAwait();
    }

    #endregion

    #endregion
}