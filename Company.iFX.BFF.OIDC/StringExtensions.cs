namespace Company.iFX.BFF.OIDC;

internal static class StringExtensions
{
    #region Methods

    #region Public

    public static Byte[] Base64UrlDecode(this String input)
    {
        if (String.IsNullOrEmpty(input))
        {
            return Array.Empty<Byte>();
        }

        String output = input.Replace('-', '+').Replace('_', '/');

        switch (output.Length % 4)
        {
            case 0: break;

            case 2: output += "=="; break;

            case 3: output += "="; break;

            default: throw new ArgumentException("Illegal base64url string.");
        }

        return Convert.FromBase64String(output);
    }

    #endregion

    #endregion
}