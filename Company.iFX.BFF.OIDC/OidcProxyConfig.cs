using Company.iFX.BFF;

namespace Company.iFX.BFF.OIDC;

public class OidcProxyConfig : ProxyConfig
{
    #region Properties

    public OpenIdConnectConfig Oidc { get; set; } = new();

    #endregion

    #region Methods

    #region Public

    public override Boolean Validate(out IEnumerable<String> errors)
    {
        Boolean hasProxyConfig = base.Validate(out IEnumerable<String> baseErrors);
        Boolean hasValidOidcConfig = Oidc.Validate(out IEnumerable<String> oidcErrors);

        errors = baseErrors.Union(oidcErrors);

        return hasProxyConfig && hasValidOidcConfig;
    }

    #endregion

    #endregion
}