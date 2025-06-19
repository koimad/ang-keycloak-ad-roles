using System.Text.RegularExpressions;

namespace Company.iFX.BFF.OIDC;

public class OpenIdConnectConfig
{
    #region Properties

    public String ClientId { get; set; } = String.Empty;

    public String ClientSecret { get; set; } = String.Empty;

    public String Authority { get; set; } = String.Empty;

    public String DiscoveryEndpoint { get; set; } = "/.well-known/openid-configuration";

    public String[] Scopes { get; set; } = Array.Empty<String>();

    public String PostLogoutRedirectEndpoint { get; set; } = "/";

    public Boolean DisablePushedAuthorization { get; set; } = false;

    #endregion

    #region Methods

    #region Public

    public virtual Boolean Validate(out IEnumerable<String> errors)
    {
        List<String> results = new List<String>();

        if (String.IsNullOrEmpty(ClientId))
        {
            results.Add("Unable to start Company.iFX.BFF Invalid client_id. " +
                        "Configure the client_id in the appsettings.json or program.cs file and try again. ");
        }

        if (String.IsNullOrEmpty(ClientSecret))
        {
            results.Add("Unable to start Company.iFX.BFF. Invalid client_secret. " +
                        "Configure the client_secret in the appsettings.json or program.cs file and try again. " );
        }

        String urlRegex = @"^https?:\/\/";

        if (!Regex.IsMatch(Authority, urlRegex))
        {
            results.Add("Unable to start Company.iFX.BFF. Invalid authority. " +
                        "Configure the authority in the appsettings.json or program.cs file and try again. ");
        }

        errors = results;
        return !results.Any();
    }

    #endregion

    #endregion
}