using System.Text;

namespace Company.iFX.BFF.OIDC.Client;


public class ProviderInformation
{
    #region Properties

    public String? IssuerName { get; set; }

    public Company.iFX.BFF.IdentityModel.Jwk.JsonWebKeySet? KeySet { get; set; }

    public String? TokenEndpoint { get; set; }

    public String? AuthorizeEndpoint { get; set; }

    public String? PushedAuthorizationRequestEndpoint { get; set; }

    public String? EndSessionEndpoint { get; set; }

    public String? UserInfoEndpoint { get; set; }

    public IEnumerable<String> TokenEndPointAuthenticationMethods { get; set; } = new String[] { };


    public Boolean SupportsUserInfo => UserInfoEndpoint.IsPresent();

    public Boolean SupportsEndSession => EndSessionEndpoint.IsPresent();

    #endregion
}