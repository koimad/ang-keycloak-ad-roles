using System.Text.Json;
using System.Text.Json.Serialization;

namespace Company.iFX.BFF.IdentityModel.Jwk;

public class JsonWebKey
{
    #region Members

    private IList<String> _certificateClauses = new List<String>();
    private IList<String> _keyOps = new List<String>();

    #endregion

    #region Properties

    [JsonPropertyName(JsonWebKeyParameterNames.Alg)]
    public String? Alg { get; set; }

    [JsonPropertyName(JsonWebKeyParameterNames.Crv)]
    public String? Crv { get; set; }

    [JsonPropertyName(JsonWebKeyParameterNames.D)]
    public String? D { get; set; }

    [JsonPropertyName(JsonWebKeyParameterNames.DP)]
    public String? DP { get; set; }

    [JsonPropertyName(JsonWebKeyParameterNames.DQ)]
    public String? DQ { get; set; }

    [JsonPropertyName(JsonWebKeyParameterNames.E)]
    public String? E { get; set; }

    [JsonPropertyName(JsonWebKeyParameterNames.K)]
    public String? K { get; set; }

    [JsonPropertyName(JsonWebKeyParameterNames.KeyOps)]
    public IList<String>? KeyOps
    {
        get => _keyOps;
        set
        {
            if (value != null)
            {
                foreach (String keyOp in value)
                {
                    _keyOps.Add(keyOp);
                }
            }
        }
    }

    [JsonPropertyName(JsonWebKeyParameterNames.Kid)]
    public String? Kid { get; set; }

    [JsonPropertyName(JsonWebKeyParameterNames.Kty)]
    public String? Kty { get; set; }

    [JsonPropertyName(JsonWebKeyParameterNames.N)]
    public String? N { get; set; }

    [JsonPropertyName(JsonWebKeyParameterNames.Oth)]
    public IList<String>? Oth { get; set; }

    [JsonPropertyName(JsonWebKeyParameterNames.P)]
    public String? P { get; set; }

    [JsonPropertyName(JsonWebKeyParameterNames.Q)]
    public String? Q { get; set; }

    [JsonPropertyName(JsonWebKeyParameterNames.QI)]
    public String? QI { get; set; }

    [JsonPropertyName(JsonWebKeyParameterNames.Use)]
    public String? Use { get; set; }

    [JsonPropertyName(JsonWebKeyParameterNames.X)]
    public String? X { get; set; }

    [JsonPropertyName(JsonWebKeyParameterNames.X5c)]
    public IList<String>? X5c
    {
        get => _certificateClauses;
        set
        {
            if (value != null)
            {
                foreach (String clause in value)
                {
                    _certificateClauses.Add(clause);
                }
            }
        }
    }

    [JsonPropertyName(JsonWebKeyParameterNames.X5t)]
    public String? X5t { get; set; }

    [JsonPropertyName(JsonWebKeyParameterNames.X5tS256)]
    public String? X5tS256 { get; set; }

    [JsonPropertyName(JsonWebKeyParameterNames.X5u)]
    public String? X5u { get; set; }

    [JsonPropertyName(JsonWebKeyParameterNames.Y)]
    public String? Y { get; set; }

    public Int32 KeySize
    {
        get
        {
            if (Kty == JsonWebAlgorithmsKeyTypes.RSA)
            {
                return Base64Url.Decode(N).Length * 8;
            }

            if (Kty == JsonWebAlgorithmsKeyTypes.EllipticCurve)
            {
                return Base64Url.Decode(X).Length * 8;
            }

            if (Kty == JsonWebAlgorithmsKeyTypes.Octet)
            {
                return Base64Url.Decode(K).Length * 8;
            }

            return 0;
        }
    }

    public Boolean HasPrivateKey
    {
        get
        {
            if (Kty == JsonWebAlgorithmsKeyTypes.RSA)
            {
                return D != null && DP != null && DQ != null && P != null && Q != null && QI != null;
            }

            if (Kty == JsonWebAlgorithmsKeyTypes.EllipticCurve)
            {
                return D != null;
            }

            return false;
        }
    }

    #endregion

    #region Constructors

    [JsonConstructor]
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public JsonWebKey() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.


    public JsonWebKey(String json)
    {
        if (String.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentNullException(nameof(json));
        }

        JsonWebKey? key = JsonSerializer.Deserialize(json, JwkSourceGenerationContext.Default.JsonWebKey);

        if (key == null)
        {
            throw new InvalidOperationException("malformed key");
        }

        Copy(key);
    }

    #endregion

    #region Methods

    #region Private

    private void Copy(JsonWebKey key)
    {
        Alg = key.Alg;
        Crv = key.Crv;
        D = key.D;
        DP = key.DP;
        DQ = key.DQ;
        E = key.E;
        K = key.K;
        Kid = key.Kid;
        Kty = key.Kty;
        N = key.N;
        Oth = key.Oth;
        P = key.P;
        Q = key.Q;
        QI = key.QI;
        Use = key.Use;
        X5t = key.X5t;
        X5tS256 = key.X5tS256;
        X5u = key.X5u;
        X = key.X;
        Y = key.Y;

        _certificateClauses = key.X5c != null ? new List<String>(key.X5c) : new List<String>();
        
        _keyOps = key.KeyOps != null ? new List<String>(key.KeyOps) : new List<String>();
    }

    #endregion

    #endregion
}