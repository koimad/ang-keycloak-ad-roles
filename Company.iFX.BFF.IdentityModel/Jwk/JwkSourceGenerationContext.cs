using System.Text.Json.Serialization;

namespace Company.iFX.BFF.IdentityModel.Jwk;

[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Metadata,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    )]
[JsonSerializable(typeof(JsonWebKey))]
[JsonSerializable(typeof(JsonWebKeySet))]
public partial class JwkSourceGenerationContext : JsonSerializerContext
{
}
