using Company.iFX.BFF.IdentityModel.Jwk;

namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class JsonWebKeySetResponse : ProtocolResponse
{
    #region Properties

    public JsonWebKeySet? KeySet { get; set; }

    #endregion

    #region Methods

    #region Protected

    protected override Task InitializeAsync(Object? initializationData = null)
    {
        if (HttpResponse?.IsSuccessStatusCode != true)
        {
            ErrorMessage = initializationData as String;
        }
        else
        {
            KeySet = new JsonWebKeySet(Raw!);
        }

        return Task.CompletedTask;
    }

    #endregion

    #endregion
}