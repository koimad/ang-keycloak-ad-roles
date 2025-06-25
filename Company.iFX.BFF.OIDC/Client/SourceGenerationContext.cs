using System.Text.Json;
using System.Text.Json.Serialization;

namespace Company.iFX.BFF.OIDC.Client;

[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Metadata,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(AuthorizeState))]
[JsonSerializable(typeof(Dictionary<String, JsonElement>))]
[JsonSerializable(typeof(OidcClientOptions))]
public partial class SourceGenerationContext : JsonSerializerContext { }