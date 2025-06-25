using System.Text.Json;
using System.Text.Json.Serialization;

using Company.iFX.BFF.IdentityModel.Jwk;

namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class DynamicClientRegistrationDocument
{
    #region Properties

    [JsonPropertyName(OidcConstants.ClientMetadata.RedirectUris)]
    public ICollection<Uri> RedirectUris { get; set; } = new HashSet<Uri>();

    [JsonPropertyName(OidcConstants.ClientMetadata.ResponseTypes)]
    public ICollection<String> ResponseTypes { get; set; } = new HashSet<String>();

    [JsonPropertyName(OidcConstants.ClientMetadata.GrantTypes)]
    public ICollection<String> GrantTypes { get; set; } = new HashSet<String>();

    [JsonPropertyName(OidcConstants.ClientMetadata.ApplicationType)]
    public String? ApplicationType { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.Contacts)]
    public ICollection<String> Contacts { get; set; } = new HashSet<String>();

    [JsonPropertyName(OidcConstants.ClientMetadata.ClientName)]
    public String? ClientName { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.LogoUri)]
    public Uri? LogoUri { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.ClientUri)]
    public Uri? ClientUri { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.PolicyUri)]
    public Uri? PolicyUri { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.TosUri)]
    public Uri? TosUri { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.JwksUri)]
    public Uri? JwksUri { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.Jwks)]
    public JsonWebKeySet? Jwks { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.SectorIdentifierUri)]
    public Uri? SectorIdentifierUri { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.SubjectType)]
    public String? SubjectType { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.Scope)]
    public String? Scope { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.PostLogoutRedirectUris)]
    public ICollection<Uri> PostLogoutRedirectUris { get; set; } = new HashSet<Uri>();

    [JsonPropertyName(OidcConstants.ClientMetadata.FrontChannelLogoutUri)]
    public String? FrontChannelLogoutUri { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.FrontChannelLogoutSessionRequired)]
    public Boolean? FrontChannelLogoutSessionRequired { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.BackchannelLogoutUri)]
    public String? BackChannelLogoutUri { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.BackchannelLogoutSessionRequired)]
    public Boolean? BackchannelLogoutSessionRequired { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.SoftwareStatement)]
    public String? SoftwareStatement { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.SoftwareId)]
    public String? SoftwareId { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.SoftwareVersion)]
    public String? SoftwareVersion { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.IdentityTokenSignedResponseAlgorithm)]
    public String? IdentityTokenSignedResponseAlgorithm { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.IdentityTokenEncryptedResponseAlgorithm)]
    public String? IdentityTokenEncryptedResponseAlgorithm { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.IdentityTokenEncryptedResponseEncryption)]
    public String? IdentityTokenEncryptedResponseEncryption { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.UserinfoSignedResponseAlgorithm)]
    public String? UserinfoSignedResponseAlgorithm { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.UserInfoEncryptedResponseAlgorithm)]
    public String? UserInfoEncryptedResponseAlgorithm { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.UserinfoEncryptedResponseEncryption)]
    public String? UserinfoEncryptedResponseEncryption { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.RequestObjectSigningAlgorithm)]
    public String? RequestObjectSigningAlgorithm { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.RequestObjectEncryptionAlgorithm)]
    public String? RequestObjectEncryptionAlgorithm { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.RequestObjectEncryptionEncryption)]
    public String? RequestObjectEncryptionEncryption { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.RequireSignedRequestObject)]
    public Boolean? RequireSignedRequestObject { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.TokenEndpointAuthenticationMethod)]
    public String? TokenEndpointAuthenticationMethod { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.TokenEndpointAuthenticationSigningAlgorithm)]
    public String? TokenEndpointAuthenticationSigningAlgorithm { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.DefaultMaxAge)]
    public Int32? DefaultMaxAge { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.RequireAuthenticationTime)]
    public Boolean? RequireAuthenticationTime { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.DefaultAcrValues)]
    public ICollection<String> DefaultAcrValues { get; set; } = new HashSet<String>();

    [JsonPropertyName(OidcConstants.ClientMetadata.InitiateLoginUri)]
    public Uri? InitiateLoginUri { get; set; }

    [JsonPropertyName(OidcConstants.ClientMetadata.RequestUris)]
    public ICollection<Uri> RequestUris { get; set; } = new HashSet<Uri>();

    [JsonExtensionData]
    public IDictionary<String, JsonElement>? Extensions { get; set; } = new Dictionary<String, JsonElement>(StringComparer.Ordinal);

    #endregion

    #region Methods

    #region Public

    public Boolean ShouldSerializeContacts()
    {
        return Contacts.Any();
    }


    public Boolean ShouldSerializeDefaultAcrValues()
    {
        return DefaultAcrValues.Any();
    }


    public Boolean ShouldSerializeGrantTypes()
    {
        return GrantTypes.Any();
    }


    public Boolean ShouldSerializeRequestUris()
    {
        return RequestUris.Any();
    }


    public Boolean ShouldSerializeResponseTypes()
    {
        return ResponseTypes.Any();
    }

    #endregion

    #endregion
}