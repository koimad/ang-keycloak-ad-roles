using System.Text.Json.Serialization;

namespace Company.iFX.BFF.IdentityModel.Client.Messages;

[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    GenerationMode = JsonSourceGenerationMode.Metadata,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(DynamicClientRegistrationDocument))]
public partial class ClientMessagesSourceGenerationContext : JsonSerializerContext
{
}
