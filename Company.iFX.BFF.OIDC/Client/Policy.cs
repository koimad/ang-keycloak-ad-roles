using Company.iFX.BFF.IdentityModel;
using Company.iFX.BFF.IdentityModel.Client;

namespace Company.iFX.BFF.OIDC.Client;

public class Policy
{
    #region Properties

    public DiscoveryPolicy Discovery { get; set; } = new DiscoveryPolicy();

    public Boolean RequireAccessTokenHash { get; set; } = false;

    public Boolean RequireIdentityTokenOnRefreshTokenResponse { get; set; } = false;

    public Boolean RequireIdentityTokenSignature { get; set; } = false;

    public Boolean ValidateTokenIssuerName { get; set; } = true;

    public ICollection<String> ValidSignatureAlgorithms { get; set; } = new HashSet<String> {
        OidcConstants.Algorithms.Asymmetric.RS256,
        OidcConstants.Algorithms.Asymmetric.RS384,
        OidcConstants.Algorithms.Asymmetric.RS512,

        OidcConstants.Algorithms.Asymmetric.PS256,
        OidcConstants.Algorithms.Asymmetric.PS384,
        OidcConstants.Algorithms.Asymmetric.PS512,

        OidcConstants.Algorithms.Asymmetric.ES256,
        OidcConstants.Algorithms.Asymmetric.ES384,
        OidcConstants.Algorithms.Asymmetric.ES512
    };

    #endregion
}