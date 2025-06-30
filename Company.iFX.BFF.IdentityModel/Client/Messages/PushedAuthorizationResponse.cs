using System.Text.Json;

namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class PushedAuthorizationResponse : ProtocolResponse
{
    #region Properties

    public Int32? ExpiresIn => Json?.TryGetInt(OidcConstants.PushedAuthorizationRequestResponse.ExpiresIn);


    public String? RequestUri =>  Json?.TryGetString(OidcConstants.PushedAuthorizationRequestResponse.RequestUri);

    #endregion
}