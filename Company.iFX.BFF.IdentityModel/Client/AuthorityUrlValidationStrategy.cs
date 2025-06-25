using System.Text;

namespace Company.iFX.BFF.IdentityModel.Client;

public sealed class AuthorityUrlValidationStrategy : IAuthorityValidationStrategy
{
    private const String _endpointIsEmpty = "endpoint is empty";
    private const String _endpointIsNotAValidUrl = "Endpoint is not a valid URL";
    private const String? _authorityMustBeAUrl = "Authority must be a URL.";

    #region Methods

    #region Public

    public AuthorityValidationResult IsEndpointValid(String endpoint, ICollection<String> allowedAuthorities)
    {
        if (String.IsNullOrEmpty(endpoint))
        {
            return AuthorityValidationResult.CreateError(_endpointIsEmpty);
        }

        if (!Uri.TryCreate(endpoint.RemoveTrailingSlash(), UriKind.Absolute, out Uri? endpointUrl))
        {
            return AuthorityValidationResult.CreateError(_endpointIsNotAValidUrl);
        }

        foreach (String authority in allowedAuthorities)
        {
            if (!Uri.TryCreate(authority.RemoveTrailingSlash(), UriKind.Absolute, out Uri? authorityUrl))
            {
                throw new ArgumentOutOfRangeException(_authorityMustBeAUrl, nameof(allowedAuthorities));
            }

            String expectedString = authorityUrl.ToString();
            String testString = endpointUrl.ToString();

            if (testString.StartsWith(expectedString, StringComparison.Ordinal))
            {
                return AuthorityValidationResult.SuccessResult;
            }
        }

        String expectedBaseAddresses = String.Join(",", allowedAuthorities);
        return AuthorityValidationResult.CreateError($"Invalid base address for endpoint {endpoint}. Valid base addresses: {expectedBaseAddresses}.");
    }


    public AuthorityValidationResult IsIssuerNameValid(String issuerName, String expectedAuthority)
    {
        if (!Uri.TryCreate(expectedAuthority.RemoveTrailingSlash(), UriKind.Absolute, out Uri? expectedAuthorityUrl))
        {
            throw new ArgumentOutOfRangeException(nameof(expectedAuthority),"Authority must be a valid URL.");
        }

        if (String.IsNullOrWhiteSpace(issuerName))
        {
            return AuthorityValidationResult.CreateError("Issuer name is missing");
        }

        if (!Uri.TryCreate(issuerName.RemoveTrailingSlash(), UriKind.Absolute, out Uri? issuerUrl))
        {
            return AuthorityValidationResult.CreateError("Issuer name is not a valid URL");
        }

        if (expectedAuthorityUrl.Equals(issuerUrl))
        {
            return AuthorityValidationResult.SuccessResult;
        }

        return AuthorityValidationResult.CreateError("Issuer name does not match authority: " + issuerName);
    }

    #endregion

    #endregion
}