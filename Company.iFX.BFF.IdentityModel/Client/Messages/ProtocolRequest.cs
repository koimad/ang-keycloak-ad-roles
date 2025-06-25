using System.Net.Http.Headers;
using System.Text;

using Company.iFX.BFF.IdentityModel.Client.Extensions;

namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class ProtocolRequest : HttpRequestMessage
{
    #region Properties
    public String? Address { get; set; }

    public String? ClientId { get; set; }

    public String? ClientSecret { get; set; }

    public ClientAssertion? ClientAssertion { get; set; } 

    public ClientCredentialStyle ClientCredentialStyle { get; set; } = ClientCredentialStyle.AuthorizationHeader;

    public BasicAuthenticationHeaderStyle AuthorizationHeaderStyle { get; set; } = BasicAuthenticationHeaderStyle.Rfc6749;

    public String? DPoPProofToken { get; set; }

    public Parameters Parameters { get; set; } = new Parameters();

    #endregion

    #region Constructors

    public ProtocolRequest()
    {
        Headers.Accept.Clear();
        Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    #endregion

    #region Methods

    #region Public

    public ProtocolRequest Clone()
    {
        return Clone<ProtocolRequest>();
    }


    /// <summary>
    ///     Clones this instance.
    /// </summary>
    public T Clone<T>()
        where T : ProtocolRequest, new()
    {
        T clone = new T {
            RequestUri = RequestUri,
            Version = Version,
            Method = Method,

            Address = Address,
            AuthorizationHeaderStyle = AuthorizationHeaderStyle,
            ClientAssertion = ClientAssertion,
            ClientCredentialStyle = ClientCredentialStyle,
            ClientId = ClientId,
            ClientSecret = ClientSecret,
            DPoPProofToken = DPoPProofToken,
            Parameters = new Parameters()
        };


        foreach (KeyValuePair<String, String> item in Parameters)
        {
            clone.Parameters.Add(item);
        }
        

        clone.Headers.Clear();

        foreach (KeyValuePair<String, IEnumerable<String>> header in Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (Options.Any())
        {
            foreach (KeyValuePair<String, Object?> property in Options)
            {
                clone.Options.TryAdd(property.Key, property.Value);
            }
        }

        return clone;
    }


    public void Prepare()
    {
        if (ClientId.IsPresent())
        {
            if (ClientCredentialStyle == ClientCredentialStyle.AuthorizationHeader)
            {
                if (AuthorizationHeaderStyle == BasicAuthenticationHeaderStyle.Rfc6749)
                {
                    this.SetBasicAuthenticationOAuth(ClientId, ClientSecret ?? "");
                }
                else if (AuthorizationHeaderStyle == BasicAuthenticationHeaderStyle.Rfc2617)
                {
                    this.SetBasicAuthentication(ClientId, ClientSecret ?? "");
                }
                else
                {
                    throw new InvalidOperationException("Unsupported basic authentication header style");
                }
            }
            else if (ClientCredentialStyle == ClientCredentialStyle.PostBody)
            {
                Parameters.AddRequired(OidcConstants.TokenRequest.ClientId, ClientId);
                Parameters.AddOptional(OidcConstants.TokenRequest.ClientSecret, ClientSecret);
            }
            else
            {
                throw new InvalidOperationException("Unsupported client credential style");
            }
        }

        if (ClientAssertion != null && !String.IsNullOrWhiteSpace(ClientAssertion.Type) && !String.IsNullOrWhiteSpace(ClientAssertion.Value))
        {
            if (ClientCredentialStyle == ClientCredentialStyle.AuthorizationHeader && !String.IsNullOrEmpty(ClientId))
            {
                throw new InvalidOperationException("CredentialStyle.AuthorizationHeader and client assertions are not compatible");
            }

            Parameters.AddOptional(OidcConstants.TokenRequest.ClientAssertionType, ClientAssertion.Type);
            Parameters.AddOptional(OidcConstants.TokenRequest.ClientAssertion, ClientAssertion.Value);
        }

        if (Address.IsPresent())
        {
            RequestUri = new Uri(Address!, UriKind.RelativeOrAbsolute);
        }

        if (DPoPProofToken.IsPresent())
        {
            Headers.Add(OidcConstants.HttpHeaders.DPoP, DPoPProofToken);
        }

        if (Parameters.Any())
        {
            Content = new FormUrlEncodedContent(Parameters);
        }
    }

    #endregion

    #endregion
}