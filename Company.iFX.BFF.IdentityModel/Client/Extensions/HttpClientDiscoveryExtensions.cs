using System.Text;

using Company.iFX.BFF.IdentityModel.Client.Messages;
using Company.iFX.BFF.IdentityModel.Internal;

namespace Company.iFX.BFF.IdentityModel.Client.Extensions;

public static class HttpClientDiscoveryExtensions
{
    #region Methods

    #region Public

    public static async Task<DiscoveryDocumentResponse> GetDiscoveryDocumentAsync(this HttpClient client, String? address = null, CancellationToken cancellationToken = default)
    {
        if (address == null && client.BaseAddress == null)
        {
            throw new ArgumentException("Either the address parameter or the HttpClient BaseAddress must not be null.");
        }

        return await client.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest { Address = address }, cancellationToken).ConfigureAwait();
    }


    public static async Task<DiscoveryDocumentResponse> GetDiscoveryDocumentAsync(this HttpMessageInvoker client, DiscoveryDocumentRequest request, CancellationToken cancellationToken = default)
    {
        String address;

        if (request.Address.IsPresent())
        {
            address = request.Address!;
        }
        else if (client is HttpClient httpClient && httpClient.BaseAddress != null)
        {
            address = httpClient.BaseAddress!.AbsoluteUri;
        }
        else
        {
            throw new ArgumentException("Either the DiscoveryDocumentRequest Address or the HttpClient BaseAddress must not be null.");
        }

        DiscoveryEndpoint parsed = DiscoveryEndpoint.ParseUrl(address, request.Policy.DiscoveryDocumentPath);
        String authority = parsed.Authority;
        String url = parsed.Url;

        if (request.Policy.Authority.IsMissing())
        {
            request.Policy.Authority = authority;
        }

        String? jwkUrl = "";

        if (!DiscoveryEndpoint.IsSecureScheme(new Uri(url), request.Policy))
        {
            return ProtocolResponse.FromException<DiscoveryDocumentResponse>(new InvalidOperationException("HTTPS required"), $"Error connecting to {url}. HTTPS required.");
        }

        try
        {
            ProtocolRequest clone = request.Clone();

            clone.Method = HttpMethod.Get;
            clone.Prepare();

            clone.RequestUri = new Uri(url);

            HttpResponseMessage response = await client.SendAsync(clone, cancellationToken).ConfigureAwait();

            if (!response.IsSuccessStatusCode)
            {
                return await ProtocolResponse.FromHttpResponseAsync<DiscoveryDocumentResponse>(response, $"Error connecting to {url}: {response.ReasonPhrase}").ConfigureAwait();
            }

            DiscoveryDocumentResponse disco = await ProtocolResponse.FromHttpResponseAsync<DiscoveryDocumentResponse>(response, request.Policy).ConfigureAwait();

            if (disco.IsError)
            {
                return disco;
            }

            try
            {
                jwkUrl = disco.JwksUri;

                if (jwkUrl != null)
                {
                    JsonWebKeySetRequest jwkClone = request.Clone<JsonWebKeySetRequest>();
                    jwkClone.Method = HttpMethod.Get;
                    jwkClone.Address = jwkUrl;
                    jwkClone.Prepare();

                    JsonWebKeySetResponse jwkResponse = await client.GetJsonWebKeySetAsync(jwkClone, cancellationToken).ConfigureAwait();

                    if (jwkResponse.IsError)
                    {
                        if (jwkResponse.Exception != null)
                        {
                            return ProtocolResponse.FromException<DiscoveryDocumentResponse>(jwkResponse.Exception, jwkResponse.Error);
                        }

                        if (jwkResponse.HttpResponse != null)
                        {
                            return await ProtocolResponse.FromHttpResponseAsync<DiscoveryDocumentResponse>(jwkResponse.HttpResponse, $"Error connecting to {jwkUrl}: {jwkResponse.HttpErrorReason}").ConfigureAwait();
                        }
                        
                        return ProtocolResponse.FromException<DiscoveryDocumentResponse>(
                            new ArgumentNullException(nameof(jwkResponse.HttpResponse)), "Unknown error retrieving JWKS - neither an exception nor an HttpResponse is available");
                    }

                    disco.KeySet = jwkResponse.KeySet;
                }

                return disco;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return ProtocolResponse.FromException<DiscoveryDocumentResponse>(ex, $"Error connecting to {jwkUrl}. {ex.Message}.");
            }
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            return ProtocolResponse.FromException<DiscoveryDocumentResponse>(ex, $"Error connecting to {url}. {ex.Message}.");
        }
    }

    #endregion

    #endregion
}