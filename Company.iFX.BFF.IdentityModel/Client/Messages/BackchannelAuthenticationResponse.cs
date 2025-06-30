using System.Text.Json;

namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class BackchannelAuthenticationResponse : ProtocolResponse
{
    #region Properties

    public String? AuthenticationRequestId =>  Json?.TryGetString(OidcConstants.BackchannelAuthenticationResponse.AuthenticationRequestId);

    public Int32 ExpiresIn
    {
        get
        {
            String? value = TryGet(OidcConstants.BackchannelAuthenticationResponse.ExpiresIn);

            if (value != null)
            {
                if (Int32.TryParse(value, out Int32 theValue))
                {
                    return theValue;
                }
            }

            return 0;
        }
    }

    public Int32? Interval => Json?.TryGetInt(OidcConstants.BackchannelAuthenticationResponse.Interval);

    #endregion
}