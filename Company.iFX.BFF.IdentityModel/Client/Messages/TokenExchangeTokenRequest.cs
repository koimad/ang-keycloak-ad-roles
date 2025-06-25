namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class TokenExchangeTokenRequest : TokenRequest
{
    #region Properties

    public String? Resource { get; set; }

    public String? Audience { get; set; }

    public String? Scope { get; set; }

    public String? RequestedTokenType { get; set; }

    public String SubjectToken { get; set; } = String.Empty!;

    public String SubjectTokenType { get; set; } = String.Empty!;

    public String? ActorToken { get; set; }

    public String? ActorTokenType { get; set; }

    #endregion
}