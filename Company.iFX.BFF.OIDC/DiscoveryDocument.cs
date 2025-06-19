namespace Company.iFX.BFF.OIDC;

public class DiscoveryDocument
{
    #region Properties

    public String? issuer { get; set; }

    public String? authorization_endpoint { get; set; }

    public String? token_endpoint { get; set; }

    public String? userinfo_endpoint { get; set; }

    public String? jwks_uri { get; set; }

    public String? revocation_endpoint { get; set; }

    public String? end_session_endpoint { get; set; }
    
    public String? introspection_endpoint { get; set; }

    #endregion
}