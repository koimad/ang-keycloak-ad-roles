using System.Net.Http.Headers;

using static Company.iFX.BFF.IdentityModel.OidcConstants;

namespace Company.iFX.BFF.IdentityModel.Client.Extensions;

public static class AuthorizationHeaderExtensions
{
    #region Methods

    #region Public

    public static void SetBasicAuthentication(this HttpClient client, String userName, String password)
    {
        client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(userName, password);
    }


    public static void SetBasicAuthentication(this HttpRequestMessage request, String userName, String password)
    {
        request.Headers.Authorization = new BasicAuthenticationHeaderValue(userName, password);
    }


    public static void SetBasicAuthenticationOAuth(this HttpClient client, String userName, String password)
    {
        client.DefaultRequestHeaders.Authorization = new BasicAuthenticationOAuthHeaderValue(userName, password);
    }


    public static void SetBasicAuthenticationOAuth(this HttpRequestMessage request, String userName, String password)
    {
        request.Headers.Authorization = new BasicAuthenticationOAuthHeaderValue(userName, password);
    }


    public static void SetBearerToken(this HttpClient client, String token)
    {
        client.SetToken(AuthenticationSchemes.AuthorizationHeaderBearer, token);
    }


    public static void SetBearerToken(this HttpRequestMessage request, String token)
    {
        request.SetToken(AuthenticationSchemes.AuthorizationHeaderBearer, token);
    }


    public static void SetDPoPToken(this HttpRequestMessage request, String accessToken, String proofToken)
    {
        request.SetToken(AuthenticationSchemes.AuthorizationHeaderDPoP, accessToken);

        if (request.Headers.Contains(OidcConstants.HttpHeaders.DPoP))
        {
            request.Headers.Remove(OidcConstants.HttpHeaders.DPoP);
        }

        request.Headers.Add(OidcConstants.HttpHeaders.DPoP, proofToken);
    }


    public static void SetToken(this HttpClient client, String scheme, String token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
    }


    public static void SetToken(this HttpRequestMessage request, String scheme, String token)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue(scheme, token);
    }

    #endregion

    #endregion
}