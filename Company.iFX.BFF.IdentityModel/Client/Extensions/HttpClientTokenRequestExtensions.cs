using Company.iFX.BFF.IdentityModel;
using Company.iFX.BFF.IdentityModel.Client.Messages;
using Company.iFX.BFF.IdentityModel.Internal;

// ReSharper disable once CheckNamespace
namespace System.Net.Http;

public static class HttpClientTokenRequestExtensions
{
    #region Methods

    #region Internal

    public static async Task<TokenResponse> RequestTokenAsync(this HttpMessageInvoker client, ProtocolRequest request, CancellationToken cancellationToken = default)
    {
        request.Prepare();
        request.Method = HttpMethod.Post;

        HttpResponseMessage response;

        try
        {
            response = await client.SendAsync(request, cancellationToken).ConfigureAwait();
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return ProtocolResponse.FromException<TokenResponse>(ex);
        }

        return await ProtocolResponse.FromHttpResponseAsync<TokenResponse>(response).ConfigureAwait();
    }

    #endregion

    #region Public

    public static async Task<TokenResponse> RequestAuthorizationCodeTokenAsync(this HttpMessageInvoker client, AuthorizationCodeTokenRequest request, CancellationToken cancellationToken = default)
    {
        ProtocolRequest clone = request.Clone();

        clone.Parameters.AddRequired(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.AuthorizationCode);
        clone.Parameters.AddRequired(OidcConstants.TokenRequest.Code, request.Code);
        clone.Parameters.AddRequired(OidcConstants.TokenRequest.RedirectUri, request.RedirectUri);
        clone.Parameters.AddOptional(OidcConstants.TokenRequest.CodeVerifier, request.CodeVerifier);

        foreach (String resource in request.Resource)
        {
            clone.Parameters.AddRequired(OidcConstants.TokenRequest.Resource, resource, true);
        }

        return await client.RequestTokenAsync(clone, cancellationToken).ConfigureAwait();
    }


    public static async Task<TokenResponse> RequestBackchannelAuthenticationTokenAsync(this HttpMessageInvoker client, BackchannelAuthenticationTokenRequest request, CancellationToken cancellationToken = default)
    {
        ProtocolRequest clone = request.Clone();

        clone.Parameters.AddRequired(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Ciba);
        clone.Parameters.AddRequired(OidcConstants.TokenRequest.AuthenticationRequestId, request.AuthenticationRequestId);

        foreach (String resource in request.Resource)
        {
            clone.Parameters.AddRequired(OidcConstants.TokenRequest.Resource, resource, true);
        }

        return await client.RequestTokenAsync(clone, cancellationToken).ConfigureAwait();
    }


    public static async Task<TokenResponse> RequestClientCredentialsTokenAsync(this HttpMessageInvoker client, ClientCredentialsTokenRequest request, CancellationToken cancellationToken = default)
    {
        ProtocolRequest clone = request.Clone();

        clone.Parameters.AddRequired(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.ClientCredentials);
        clone.Parameters.AddOptional(OidcConstants.TokenRequest.Scope, request.Scope);

        foreach (String resource in request.Resource)
        {
            clone.Parameters.AddRequired(OidcConstants.TokenRequest.Resource, resource, true);
        }

        return await client.RequestTokenAsync(clone, cancellationToken).ConfigureAwait();
    }


    public static async Task<TokenResponse> RequestDeviceTokenAsync(this HttpMessageInvoker client, DeviceTokenRequest request, CancellationToken cancellationToken = default)
    {
        ProtocolRequest clone = request.Clone();

        clone.Parameters.AddRequired(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.DeviceCode);
        clone.Parameters.AddRequired(OidcConstants.TokenRequest.DeviceCode, request.DeviceCode);

        return await client.RequestTokenAsync(clone, cancellationToken).ConfigureAwait();
    }


    public static async Task<TokenResponse> RequestPasswordTokenAsync(this HttpMessageInvoker client, PasswordTokenRequest request, CancellationToken cancellationToken = default)
    {
        ProtocolRequest clone = request.Clone();

        clone.Parameters.AddRequired(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.Password);
        clone.Parameters.AddRequired(OidcConstants.TokenRequest.UserName, request.UserName);
        clone.Parameters.AddRequired(OidcConstants.TokenRequest.Password, request.Password, allowEmptyValue: true);
        clone.Parameters.AddOptional(OidcConstants.TokenRequest.Scope, request.Scope);

        foreach (String resource in request.Resource)
        {
            clone.Parameters.AddRequired(OidcConstants.TokenRequest.Resource, resource, true);
        }

        return await client.RequestTokenAsync(clone, cancellationToken).ConfigureAwait();
    }


    public static async Task<TokenResponse> RequestRefreshTokenAsync(this HttpMessageInvoker client, RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        ProtocolRequest clone = request.Clone();

        clone.Parameters.AddRequired(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.RefreshToken);
        clone.Parameters.AddRequired(OidcConstants.TokenRequest.RefreshToken, request.RefreshToken);
        clone.Parameters.AddOptional(OidcConstants.TokenRequest.Scope, request.Scope);

        foreach (String resource in request.Resource)
        {
            clone.Parameters.AddRequired(OidcConstants.TokenRequest.Resource, resource, true);
        }

        return await client.RequestTokenAsync(clone, cancellationToken).ConfigureAwait();
    }


    public static async Task<TokenResponse> RequestTokenAsync(this HttpMessageInvoker client, TokenRequest request, CancellationToken cancellationToken = default)
    {
        ProtocolRequest clone = request.Clone();

        if (!clone.Parameters.ContainsKey(OidcConstants.TokenRequest.GrantType))
        {
            clone.Parameters.AddRequired(OidcConstants.TokenRequest.GrantType, request.GrantType);
        }

        return await client.RequestTokenAsync(clone, cancellationToken).ConfigureAwait();
    }


    public static async Task<TokenResponse> RequestTokenExchangeTokenAsync(this HttpMessageInvoker client, TokenExchangeTokenRequest request, CancellationToken cancellationToken = default)
    {
        ProtocolRequest clone = request.Clone();

        clone.Parameters.AddRequired(OidcConstants.TokenRequest.GrantType, OidcConstants.GrantTypes.TokenExchange);
        clone.Parameters.AddRequired(OidcConstants.TokenRequest.SubjectToken, request.SubjectToken);
        clone.Parameters.AddRequired(OidcConstants.TokenRequest.SubjectTokenType, request.SubjectTokenType);

        clone.Parameters.AddOptional(OidcConstants.TokenRequest.Resource, request.Resource);
        clone.Parameters.AddOptional(OidcConstants.TokenRequest.Audience, request.Audience);
        clone.Parameters.AddOptional(OidcConstants.TokenRequest.Scope, request.Scope);
        clone.Parameters.AddOptional(OidcConstants.TokenRequest.RequestedTokenType, request.RequestedTokenType);
        clone.Parameters.AddOptional(OidcConstants.TokenRequest.ActorToken, request.ActorToken);
        clone.Parameters.AddOptional(OidcConstants.TokenRequest.ActorTokenType, request.ActorTokenType);

        return await client.RequestTokenAsync(clone, cancellationToken).ConfigureAwait();
    }


    public static async Task<TokenResponse> RequestTokenRawAsync(this HttpMessageInvoker client, String address, Parameters parameters, CancellationToken cancellationToken = default)
    {
        if (parameters == null)
        {
            throw new ArgumentNullException(nameof(parameters));
        }

        TokenRequest request = new TokenRequest { Address = address, Parameters = parameters };

        return await client.RequestTokenAsync(request, cancellationToken).ConfigureAwait();
    }

    #endregion

    #endregion
}