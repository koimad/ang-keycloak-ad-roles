using System.Text.Json;
using System.Text.Json.Serialization;

namespace Company.iFX.BFF.IdentityModel.Jwk;

public class JsonWebKeySet
{
    #region Properties

    [JsonPropertyName(JsonWebKeyParameterNames.Keys)]
    public List<JsonWebKey> Keys { get; set; }

    [JsonIgnore]
    public String? RawData { get; set; }

    #endregion

    #region Constructors

    [JsonConstructor]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public JsonWebKeySet() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.


    public JsonWebKeySet(String json)
    {
        if (String.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentNullException(nameof(json));
        }

        JsonWebKeySet? jwebKeys = null;
        jwebKeys = JsonSerializer.Deserialize<JsonWebKeySet>(json, JwkSourceGenerationContext.Default.JsonWebKeySet);

        if (jwebKeys == null)
        {
            throw new InvalidOperationException("invalid JSON web keys");
        }

        Keys = jwebKeys.Keys;
        RawData = json;
    }

    #endregion
}