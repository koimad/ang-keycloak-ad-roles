using System.Text;
using System.Text.Json;

namespace Company.iFX.BFF.IdentityModel.Jwk;

public static class JsonWebKeyExtensions
{
    #region Methods

    #region Public

    public static String ToJwkString(this JsonWebKey key)
    {
        String json = JsonSerializer.Serialize(key, JwkSourceGenerationContext.Default.JsonWebKey);
        return Base64Url.Encode(Encoding.UTF8.GetBytes(json));
    }

    #endregion

    #endregion
}