namespace Company.iFX.BFF.OIDC.Client.Infrastructure
{
    internal static class OidcClientOptionsExtensions
    {
        public static HttpClient CreateClient(this OidcClientOptions options)
        {
            if (options.HttpClientFactory != null)
            {
                return options.HttpClientFactory(options);
            }

            HttpClient client = options.BackchannelHandler != null ? new HttpClient(options.BackchannelHandler) : new HttpClient();

            client.Timeout = options.BackchannelTimeout;
            return client;
        }
    }
}