namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class PushedAuthorizationRequest : ProtocolRequest
{
    #region Properties

    public String? ResponseType { get; set; }

    public String? Scope { get; set; }

    public String? RedirectUri { get; set; }

    public String? State { get; set; }

    public String? Nonce { get; set; }

    public String? LoginHint { get; set; }

    public String? AcrValues { get; set; }

    public String? Prompt { get; set; }

    public String? ResponseMode { get; set; }

    public String? CodeChallenge { get; set; }

    public String? CodeChallengeMethod { get; set; }

    public String? Display { get; set; }

    public Int32? MaxAge { get; set; }

    public String? UiLocales { get; set; }

    public String? IdTokenHint { get; set; }

    public ICollection<String> Resource { get; set; } = new HashSet<String>();

    public String? DPoPKeyThumbprint { get; set; }

    public String? Request { get; set; }

    #endregion

    #region Methods

    #region Public

    public Parameters MergeInto(Parameters targetParameters)
    {
        return targetParameters;
    }

    #endregion

    #endregion
}