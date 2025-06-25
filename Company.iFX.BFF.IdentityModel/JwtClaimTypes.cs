
namespace Company.iFX.BFF.IdentityModel;

public static class JwtClaimTypes
{
    public const String Subject = "sub";

    public const String Name = "name";

    public const String GivenName = "given_name";

    public const String FamilyName = "family_name";

    public const String MiddleName = "middle_name";

    public const String NickName = "nickname";

    public const String PreferredUserName = "preferred_username";

    public const String Profile = "profile";

    public const String Picture = "picture";

    public const String WebSite = "website";

    public const String Email = "email";

    public const String EmailVerified = "email_verified";

    public const String Gender = "gender";

    public const String BirthDate = "birthdate";

    public const String ZoneInfo = "zoneinfo";

    public const String Locale = "locale";

    public const String PhoneNumber = "phone_number";

    public const String PhoneNumberVerified = "phone_number_verified";

    public const String Address = "address";

    public const String Audience = "aud";

    public const String Issuer = "iss";

    public const String NotBefore = "nbf";

    public const String Expiration = "exp";

    public const String UpdatedAt = "updated_at";

    public const String IssuedAt = "iat";

    public const String AuthenticationMethod = "amr";

    public const String SessionId = "sid";

    public const String AuthenticationContextClassReference = "acr";

    public const String AuthenticationTime = "auth_time";

    public const String AuthorizedParty = "azp";

    public const String AccessTokenHash = "at_hash";

    public const String AuthorizationCodeHash = "c_hash";

    public const String StateHash = "s_hash";

    public const String Nonce = "nonce";

    public const String JwtId = "jti";

    public const String Events = "events";

    public const String ClientId = "client_id";

    public const String Scope = "scope";

    public const String Actor = "act";

    public const String MayAct = "may_act";

    public const String Id = "id";

    public const String IdentityProvider = "idp";

    public const String Role = "role";

    public const String Roles = "roles";

    public const String ReferenceTokenId = "reference_token_id";

    public const String Confirmation = "cnf";

    public const String Algorithm = "alg";

    public const String JsonWebKey = "jwk";
    
    public const String TokenType = "typ";

    public const String DPoPHttpMethod = "htm";
    
    public const String DPoPHttpUrl = "htu";
    
    public const String DPoPAccessTokenHash = "ath";
    
    public static class JwtTypes
    {
        public const String AccessToken = "at+jwt";

        public const String AuthorizationRequest = "oauth-authz-req+jwt";
        
        public const String DPoPProofToken = "dpop+jwt";
    }

    public static class ConfirmationMethods
    {
        public const String JsonWebKey = "jwk";
        
        public const String JwkThumbprint = "jkt";
            
        public const String X509ThumbprintSha256 = "x5t#S256";
    }
}