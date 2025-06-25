namespace Company.iFX.BFF.IdentityModel.Client;

public interface IAuthorityValidationStrategy
{
    AuthorityValidationResult IsIssuerNameValid(String issuerName, String expectedAuthority);


    AuthorityValidationResult IsEndpointValid(String endpoint, ICollection<String> expectedAuthority);
}