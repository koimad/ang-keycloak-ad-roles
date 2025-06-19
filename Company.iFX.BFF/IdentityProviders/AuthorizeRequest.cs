using Microsoft.AspNetCore.Http;

namespace Company.iFX.BFF.IdentityProviders;

public class AuthorizeRequest
{
    #region Properties

    public Uri AuthorizeUri { get; }

    public String? CodeVerifier { get; }

    #endregion

    #region Constructors

    public AuthorizeRequest(Uri authorizeUri)
    {
        AuthorizeUri = authorizeUri;
    }


    public AuthorizeRequest(Uri authorizeUri, String codeVerifier)
    {
        AuthorizeUri = authorizeUri;
        CodeVerifier = codeVerifier;
    }

    #endregion

    #region Methods

    #region Public

    public IResult ToResult()
    {
        return Results.Redirect(AuthorizeUri.ToString());
    }


    public override String ToString()
    {
        return AuthorizeUri.ToString();
    }


    public Uri ToUri()
    {
        return AuthorizeUri;
    }

    #endregion

    #endregion
}