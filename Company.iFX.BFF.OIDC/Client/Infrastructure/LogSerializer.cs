using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Company.iFX.BFF.OIDC.Client.Infrastructure;

public static class LogSerializer
{
    #region Members

    private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = true
    };

    public static Boolean Enabled = true;

    #endregion

    #region Constructors

    static LogSerializer()
    {
        _jsonOptions.Converters.Add(new JsonStringEnumConverter());
    }

    #endregion

    #region Methods

    #region Private

    private static String Serialize<T>(T logObject)
    {
        return Enabled ? JsonSerializer.Serialize(logObject, (JsonTypeInfo<T>)SourceGenerationContext.Default.GetTypeInfo(typeof(T))) : "Logging has been disabled";
    }

    #endregion

    #region Public

    public static String Serialize(OidcClientOptions opts)
    {
        return Serialize<OidcClientOptions>(opts);
    }


    public static String Serialize(AuthorizeState state)
    {
        return Serialize<AuthorizeState>(state);
    }


    [RequiresUnreferencedCode("The log serializer uses reflection in a way that is incompatible with trimming")]
    public static String Serialize(Object logObject)
    {
        return Enabled ? JsonSerializer.Serialize(logObject, _jsonOptions) : "Logging has been disabled";
    }

    #endregion

    #endregion
}