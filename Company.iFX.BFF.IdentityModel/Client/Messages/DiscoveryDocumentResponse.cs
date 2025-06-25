using System.Text;
using System.Text.Json;

using Company.iFX.BFF.IdentityModel.Client.Extensions;
using Company.iFX.BFF.IdentityModel.Jwk;

namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class DiscoveryDocumentResponse : ProtocolResponse
{
    #region Properties

    public DiscoveryPolicy Policy { get; set; } = new DiscoveryPolicy();

    public JsonWebKeySet? KeySet { get; set; }

    public MtlsEndpointAliases? MtlsEndpointAliases { get; internal set; }

    public String? Issuer => TryGetString(OidcConstants.Discovery.Issuer);
    public String? AuthorizeEndpoint => TryGetString(OidcConstants.Discovery.AuthorizationEndpoint);
    public String? TokenEndpoint => TryGetString(OidcConstants.Discovery.TokenEndpoint);
    public String? UserInfoEndpoint => TryGetString(OidcConstants.Discovery.UserInfoEndpoint);
    public String? IntrospectionEndpoint => TryGetString(OidcConstants.Discovery.IntrospectionEndpoint);
    public String? RevocationEndpoint => TryGetString(OidcConstants.Discovery.RevocationEndpoint);
    public String? DeviceAuthorizationEndpoint => TryGetString(OidcConstants.Discovery.DeviceAuthorizationEndpoint);
    public String? BackchannelAuthenticationEndpoint => TryGetString(OidcConstants.Discovery.BackchannelAuthenticationEndpoint);

    public String? JwksUri => TryGetString(OidcConstants.Discovery.JwksUri);
    public String? EndSessionEndpoint => TryGetString(OidcConstants.Discovery.EndSessionEndpoint);
    public String? CheckSessionIframe => TryGetString(OidcConstants.Discovery.CheckSessionIframe);
    public String? RegistrationEndpoint => TryGetString(OidcConstants.Discovery.RegistrationEndpoint);
    public String? PushedAuthorizationRequestEndpoint => TryGetString(OidcConstants.Discovery.PushedAuthorizationRequestEndpoint);
    public Boolean? FrontChannelLogoutSupported => TryGetBoolean(OidcConstants.Discovery.FrontChannelLogoutSupported);
    public Boolean? FrontChannelLogoutSessionSupported => TryGetBoolean(OidcConstants.Discovery.FrontChannelLogoutSessionSupported);
    public IEnumerable<String> GrantTypesSupported => TryGetStringArray(OidcConstants.Discovery.GrantTypesSupported);
    public IEnumerable<String> CodeChallengeMethodsSupported => TryGetStringArray(OidcConstants.Discovery.CodeChallengeMethodsSupported);
    public IEnumerable<String> ScopesSupported => TryGetStringArray(OidcConstants.Discovery.ScopesSupported);
    public IEnumerable<String> SubjectTypesSupported => TryGetStringArray(OidcConstants.Discovery.SubjectTypesSupported);
    public IEnumerable<String> ResponseModesSupported => TryGetStringArray(OidcConstants.Discovery.ResponseModesSupported);
    public IEnumerable<String> ResponseTypesSupported => TryGetStringArray(OidcConstants.Discovery.ResponseTypesSupported);
    public IEnumerable<String> ClaimsSupported => TryGetStringArray(OidcConstants.Discovery.ClaimsSupported);
    public IEnumerable<String> TokenEndpointAuthenticationMethodsSupported => TryGetStringArray(OidcConstants.Discovery.TokenEndpointAuthenticationMethodsSupported);
    public IEnumerable<String> BackchannelTokenDeliveryModesSupported => TryGetStringArray(OidcConstants.Discovery.BackchannelTokenDeliveryModesSupported);
    public Boolean? BackchannelUserCodeParameterSupported => TryGetBoolean(OidcConstants.Discovery.BackchannelUserCodeParameterSupported);
    public Boolean? RequirePushedAuthorizationRequests => TryGetBoolean(OidcConstants.Discovery.RequirePushedAuthorizationRequests);

    #endregion

    #region Methods

    #region Private

    private String Validate(DiscoveryPolicy policy)
    {
        if (policy.ValidateIssuerName)
        {
            IAuthorityValidationStrategy strategy = policy.AuthorityValidationStrategy ?? DiscoveryPolicy.DefaultAuthorityValidationStrategy;

            AuthorityValidationResult issuerValidationResult = strategy.IsIssuerNameValid(Issuer!, policy.Authority);

            if (!issuerValidationResult.Success)
            {
                return issuerValidationResult.ErrorMessage;
            }
        }

        String error = ValidateEndpoints(Json, policy);

        if (error.IsPresent())
        {
            return error;
        }

        return String.Empty;
    }


    private Boolean ValidateIssuerName(String issuer, String authority, IAuthorityValidationStrategy validationStrategy)
    {
        return validationStrategy.IsIssuerNameValid(issuer, authority).Success;
    }

    #endregion

    #region Protected

    protected override Task InitializeAsync(Object? initializationData = null)
    {
        if (HttpResponse?.IsSuccessStatusCode != true)
        {
            ErrorMessage = initializationData as String;
            return Task.CompletedTask;
        }

        Policy = initializationData as DiscoveryPolicy ?? new DiscoveryPolicy();

        String validationError = Validate(Policy);

        if (validationError.IsPresent())
        {
            Json = null;
            ErrorType = ResponseErrorType.PolicyViolation;
            ErrorMessage = validationError;
        }

        MtlsEndpointAliases = new MtlsEndpointAliases(Json?.TryGetValue(OidcConstants.Discovery.MtlsEndpointAliases));

        return Task.CompletedTask;
    }

    #endregion

    #region Public

    public Boolean? TryGetBoolean(String name)
    {
        return Json?.TryGetBoolean(name);
    }


    public String? TryGetString(String name)
    {
        return Json?.TryGetString(name);
    }


    public IEnumerable<String> TryGetStringArray(String name)
    {
        return Json?.TryGetStringArray(name) ?? Array.Empty<String>();
    }


    public JsonElement? TryGetValue(String name)
    {
        return Json?.TryGetValue(name);
    }


    public String ValidateEndpoints(JsonElement? json, DiscoveryPolicy policy)
    {
        if (json == null)
        {
            throw new ArgumentNullException(nameof(json));
        }

        HashSet<String> allowedHosts = new HashSet<String>(policy.AdditionalEndpointBaseAddresses.Select(e => new Uri(e).Authority)) {
            new Uri(policy.Authority).Authority
        };

        HashSet<String> allowedAuthorities = new HashSet<String>(policy.AdditionalEndpointBaseAddresses) { policy.Authority };

        foreach (JsonProperty element in json?.EnumerateObject()!)
        {
            if (element.Name.EndsWith("endpoint", StringComparison.OrdinalIgnoreCase) ||
                element.Name.Equals(OidcConstants.Discovery.JwksUri, StringComparison.OrdinalIgnoreCase) ||
                element.Name.Equals(OidcConstants.Discovery.CheckSessionIframe, StringComparison.OrdinalIgnoreCase))
            {
                String endpoint = element.Value.ToString();

                Boolean isValidUri = Uri.TryCreate(endpoint, UriKind.Absolute, out Uri? uri);

                if (!isValidUri)
                {
                    return $"Malformed endpoint: {endpoint}";
                }

                if (!DiscoveryEndpoint.IsValidScheme(uri!))
                {
                    return $"Malformed endpoint: {endpoint}";
                }

                if (!DiscoveryEndpoint.IsSecureScheme(uri!, policy))
                {
                    return $"Endpoint does not use HTTPS: {endpoint}";
                }

                if (policy.ValidateEndpoints)
                {
                    // if endpoint is on exclude list, don't validate
                    if (policy.EndpointValidationExcludeList.Contains(element.Name))
                    {
                        continue;
                    }

                    Boolean isAllowed = false;

                    foreach (String host in allowedHosts)
                    {
                        if (String.Equals(host, uri!.Authority))
                        {
                            isAllowed = true;
                        }
                    }

                    if (!isAllowed)
                    {
                        return $"Endpoint is on a different host than authority: {endpoint}";
                    }

                    IAuthorityValidationStrategy strategy = policy.AuthorityValidationStrategy ?? DiscoveryPolicy.DefaultAuthorityValidationStrategy;
                    AuthorityValidationResult endpointValidationResult = strategy.IsEndpointValid(endpoint, allowedAuthorities);

                    if (!endpointValidationResult.Success)
                    {
                        return endpointValidationResult.ErrorMessage;
                    }
                }
            }
        }

        if (policy.RequireKeySet)
        {
            if (String.IsNullOrWhiteSpace(JwksUri))
            {
                return "Keyset is missing";
            }
        }

        return String.Empty;
    }


    public Boolean ValidateIssuerName(String issuer, String authority)
    {
        return DiscoveryPolicy.DefaultAuthorityValidationStrategy.IsIssuerNameValid(issuer, authority).Success;
    }


    public Boolean ValidateIssuerName(String issuer, String authority, StringComparison nameComparison)
    {
        return new StringComparisonAuthorityValidationStrategy(nameComparison).IsIssuerNameValid(issuer, authority).Success;
    }

    #endregion

    #endregion
}