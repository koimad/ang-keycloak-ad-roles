namespace Company.iFX.BFF.IdentityModel.Client;

public class DiscoveryPolicy
{
    #region Members

    public static readonly IAuthorityValidationStrategy DefaultAuthorityValidationStrategy = new StringComparisonAuthorityValidationStrategy();

    public ICollection<String> LoopbackAddresses = new HashSet<String> { "localhost", "127.0.0.1" };

    #endregion

    #region Properties

    public String Authority { get; set; } = default!;

    public String? DiscoveryDocumentPath { get; set; }

    public IAuthorityValidationStrategy AuthorityValidationStrategy { get; set; } = DefaultAuthorityValidationStrategy;

    public Boolean RequireHttps { get; set; } = true;

    public Boolean AllowHttpOnLoopback { get; set; } = true;

    public Boolean ValidateIssuerName { get; set; } = true;

    public Boolean ValidateEndpoints { get; set; } = true;

    public ICollection<String> EndpointValidationExcludeList { get; set; } = new HashSet<String>();

    public ICollection<String> AdditionalEndpointBaseAddresses { get; set; } = new HashSet<String>();

    public Boolean RequireKeySet { get; set; } = true;

    #endregion
}