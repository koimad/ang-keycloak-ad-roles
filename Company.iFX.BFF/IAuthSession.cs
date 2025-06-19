using Company.iFX.BFF.IdentityProviders;

namespace Company.iFX.BFF;

public interface IAuthSession
{
    Boolean HasIdToken();

    Boolean HasAccessToken();

    String? GetIdToken();


    String? GetAccessToken();


    String? GetUserPreferredLandingPage();


    Task<AuthorizeRequest> InitiateAuthenticationSequence(String userPreferredLandingPage);
}