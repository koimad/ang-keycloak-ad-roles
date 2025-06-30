using System.Security.Claims;
using System.Text.Json;

// ReSharper disable once CheckNamespace
namespace System.Text.Json;

public static class JsonElementExtensions
{
    #region Methods

    #region Private

    private static String Stringify(JsonElement item)
    {
        String value = item.ValueKind == JsonValueKind.String ? item.ToString() : item.GetRawText();

        return value;
    }

    #endregion

    #region Public

    public static IEnumerable<Claim> ToClaims(this JsonElement json, String? issuer = null, params String[] excludeKeys)
    {
        List<Claim> claims = new List<Claim>();
        List<String> excludeList = excludeKeys.ToList();

        foreach (JsonProperty x in json.EnumerateObject())
        {
            if (excludeList.Contains(x.Name))
            {
                continue;
            }

            if (x.Value.ValueKind == JsonValueKind.Array)
            {
                foreach (JsonElement item in x.Value.EnumerateArray())
                {
                    claims.Add(new Claim(x.Name, Stringify(item), ClaimValueTypes.String, issuer));
                }
            }
            else
            {
                claims.Add(new Claim(x.Name, Stringify(x.Value), ClaimValueTypes.String, issuer));
            }
        }

        return claims;
    }


    public static Boolean? TryGetBoolean(this JsonElement json, String name)
    {
        String? value = json.TryGetString(name);

        if (Boolean.TryParse(value, out Boolean result))
        {
            return result;
        }

        return null;
    }


    public static Int32? TryGetInt(this JsonElement json, String name)
    {
        String? value = json.TryGetString(name);

        if (value != null)
        {
            if (Int32.TryParse(value, out Int32 intValue))
            {
                return intValue;
            }
        }

        return null;
    }


    public static String? TryGetString(this JsonElement json, String name)
    {
        JsonElement value = json.TryGetValue(name);
        return value.ValueKind == JsonValueKind.Undefined ? null : value.ToString();
    }


    public static IEnumerable<String> TryGetStringArray(this JsonElement json, String name)
    {
        List<String> values = new List<String>();

        JsonElement array = json.TryGetValue(name);

        if (array.ValueKind == JsonValueKind.Array)
        {
            foreach (JsonElement item in array.EnumerateArray())
            {
                values.Add(item.ToString());
            }
        }

        return values;
    }


    public static JsonElement TryGetValue(this JsonElement json, String name)
    {
        if (json.ValueKind == JsonValueKind.Undefined)
        {
            return default;
        }

        return json.TryGetProperty(name, out JsonElement value) ? value : default;
    }

    #endregion

    #endregion
}