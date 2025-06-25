using System.Text.Json.Serialization;

using Company.iFX.BFF.IdentityModel;
using Company.iFX.BFF.IdentityModel.Client;
using Company.iFX.BFF.IdentityModel.Client.Messages;
using Company.iFX.BFF.OIDC.Client.Browser;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Company.iFX.BFF.OIDC.Client;

public class OidcClientOptions
{
    #region Properties

    public String? Authority { get; set; }


    public ProviderInformation? ProviderInformation { get; set; } 


    public String? ClientId { get; set; }

    public String? ClientSecret { get; set; }


    public ClientAssertion ClientAssertion { get; set; } = new ClientAssertion();


    [JsonIgnore]
    public Func<Task<ClientAssertion>> GetClientAssertionAsync { get; set; }

    public String? Scope { get; set; }

    public ICollection<String> Resource { get; set; } = new HashSet<String>();

    public String? RedirectUri { get; set; }

    public String? PostLogoutRedirectUri { get; set; }

    public Int32 StateLength { get; set; } = 16;

    [JsonIgnore]
    public IBrowser? Browser { get; set; }

    public TimeSpan? BrowserTimeout { get; set; }

    public TimeSpan ClockSkew { get; set; } = TimeSpan.FromMinutes(5);

    public Boolean RefreshDiscoveryDocumentForLogin { get; set; } = true;

    public Boolean RefreshDiscoveryOnSignatureFailure { get; set; } = false;

    public Boolean LoadProfile { get; set; } = true;

    public Boolean FilterClaims { get; set; } = true;

    [JsonIgnore]
    public HttpMessageHandler? RefreshTokenInnerHttpHandler { get; set; }

    [JsonIgnore]
    public HttpMessageHandler? BackchannelHandler { get; set; }

    public TimeSpan BackchannelTimeout { get; set; } = TimeSpan.FromSeconds(30);

    [JsonIgnore]
    public Func<OidcClientOptions, HttpClient>? HttpClientFactory { get; set; }

    public ClientCredentialStyle TokenClientCredentialStyle { get; set; } = ClientCredentialStyle.PostBody;

    public Policy Policy { get; set; } = new Policy();

    [JsonIgnore]
    public ILoggerFactory LoggerFactory { get; set; } = new NullLoggerFactory();

    [JsonIgnore]
    public IIdentityTokenValidator? IdentityTokenValidator { get; set; }

    public ICollection<String> FilteredClaims { get; set; } = new HashSet<String> {
        JwtClaimTypes.Issuer,
        JwtClaimTypes.Expiration,
        JwtClaimTypes.NotBefore,
        JwtClaimTypes.Audience,
        JwtClaimTypes.Nonce,
        JwtClaimTypes.IssuedAt,
        JwtClaimTypes.AuthenticationTime,
        JwtClaimTypes.AuthorizationCodeHash,
        JwtClaimTypes.AccessTokenHash,
        JwtClaimTypes.StateHash
    };

    public Boolean DisablePushedAuthorization { get; set; } = false;

    #endregion

    #region Constructors

    public OidcClientOptions()
    {
        GetClientAssertionAsync ??= () => Task.FromResult(ClientAssertion);
    }

    #endregion
}