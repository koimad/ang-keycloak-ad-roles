using System.Security.Claims;
using System.Text.Json;

namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class UserInfoResponse : ProtocolResponse
{
    #region Properties

    public IEnumerable<Claim> Claims { get; private set; } = new List<Claim>();

    #endregion

    #region Methods

    #region Protected

    protected override Task InitializeAsync(Object? initializationData = null)
    {
        if (!IsError && Json.HasValue)
        {
            Claims = Json.Value.ToClaims();
        }
        else
        {
            Claims = Enumerable.Empty<Claim>();
        }

        return Task.CompletedTask;
    }

    #endregion

    #endregion
}