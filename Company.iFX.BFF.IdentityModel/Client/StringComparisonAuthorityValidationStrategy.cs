using System.Text;

namespace Company.iFX.BFF.IdentityModel.Client;

public sealed class StringComparisonAuthorityValidationStrategy : IAuthorityValidationStrategy
{
    #region Members

    private readonly StringComparison _stringComparison;

    #endregion

    #region Constructors

    public StringComparisonAuthorityValidationStrategy(StringComparison stringComparison = StringComparison.Ordinal)
    {
        _stringComparison = stringComparison;
    }

    #endregion

    #region Methods

    #region Public

    public AuthorityValidationResult IsEndpointValid(String endpoint, ICollection<String> allowedAuthorities)
    {
        if (String.IsNullOrEmpty(endpoint))
        {
            return AuthorityValidationResult.CreateError("endpoint is empty");
        }

        foreach (String? authority in allowedAuthorities)
        {
            if (!String.IsNullOrWhiteSpace(authority))
            {
                if (endpoint.StartsWith(authority, _stringComparison))
                {
                    return AuthorityValidationResult.SuccessResult;
                }
            }
        }

        String expectedBaseAddresses = String.Join(",", allowedAuthorities);
        return AuthorityValidationResult.CreateError($"Invalid base address for endpoint {endpoint}. Valid base addresses: {expectedBaseAddresses}.");
    }


    public AuthorityValidationResult IsIssuerNameValid(String issuerName, String expectedAuthority)
    {
        if (String.IsNullOrWhiteSpace(issuerName))
        {
            return AuthorityValidationResult.CreateError("Issuer name is missing");
        }

        if (String.Equals(issuerName.RemoveTrailingSlash(), expectedAuthority.RemoveTrailingSlash(), _stringComparison))
        {
            return AuthorityValidationResult.SuccessResult;
        }

        return AuthorityValidationResult.CreateError("Issuer name does not match authority: " + issuerName);
    }

    #endregion

    #endregion
}