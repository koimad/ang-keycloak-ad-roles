using System.Security.Cryptography;

// ReSharper disable once CheckNamespace
namespace System.Text;

public static class StringExtensions
{
    #region Methods

    #region Public
    public static String ToSha256(this String input)
    {
        if (input.IsMissing())
        {
            return String.Empty;
        }

        using (SHA256 sha = SHA256.Create())
        {
            Byte[] bytes = Encoding.UTF8.GetBytes(input);
            Byte[] hash = sha.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }
    }


    public static String ToSha512(this String input)
    {
        if (input.IsMissing())
        {
            return String.Empty;
        }

        using (SHA512 sha = SHA512.Create())
        {
            Byte[] bytes = Encoding.UTF8.GetBytes(input);
            Byte[] hash = sha.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }
    }


    public static String EnsureTrailingSlash(this String url)
    {
        if (!url.EndsWith("/"))
        {
            return url + "/";
        }

        return url;
    }


    public static Boolean IsMissing(this String? value)
    {
        return String.IsNullOrWhiteSpace(value);
    }


    public static Boolean IsPresent(this String? value)
    {
        return !value.IsMissing();
    }


    public static String RemoveTrailingSlash(this String url)
    {
        if (url == null)
        {
            throw new ArgumentNullException(nameof(url));
        }

        if (url.EndsWith("/"))
        {
            url = url.Substring(0, url.Length - 1);
        }

        return url;
    }

    #endregion

    #endregion
}