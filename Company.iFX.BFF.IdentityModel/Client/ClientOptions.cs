using Company.iFX.BFF.IdentityModel.Client.Messages;

namespace Company.iFX.BFF.IdentityModel.Client;

public class TokenClientOptions : ClientOptions { }

public class IntrospectionClientOptions : ClientOptions { }

public abstract class ClientOptions
{
    #region Properties

    public String Address { get; set; } = String.Empty!;

    public String ClientId { get; set; } = String.Empty!;

    public String? ClientSecret { get; set; }

    public ClientAssertion? ClientAssertion { get; set; } = new ClientAssertion();

    public ClientCredentialStyle ClientCredentialStyle { get; set; } = ClientCredentialStyle.PostBody;

    public BasicAuthenticationHeaderStyle AuthorizationHeaderStyle { get; set; } = BasicAuthenticationHeaderStyle.Rfc6749;

    public Parameters Parameters { get; set; } = new();

    #endregion
}