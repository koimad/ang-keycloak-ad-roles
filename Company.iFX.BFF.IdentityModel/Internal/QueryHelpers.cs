using System.Text;
using System.Text.Encodings.Web;

namespace Company.iFX.BFF.IdentityModel.Internal;

internal static class QueryHelpers
{
    #region Methods

    #region Public

    public static String AddQueryString(String uri, String name, String value)
    {
        if (uri == null)
        {
            throw new ArgumentNullException(nameof(uri));
        }

        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        return AddQueryString(uri, new[] { new KeyValuePair<String, String>(name, value) });
    }


    public static String AddQueryString(String uri, IDictionary<String, String> queryString)
    {
        if (uri == null)
        {
            throw new ArgumentNullException(nameof(uri));
        }

        if (queryString == null)
        {
            throw new ArgumentNullException(nameof(queryString));
        }

        return AddQueryString(uri, (IEnumerable<KeyValuePair<String, String>>)queryString);
    }


    public static String AddQueryString(String uri, IEnumerable<KeyValuePair<String, String>> queryString)
    {
        if (uri == null)
        {
            throw new ArgumentNullException(nameof(uri));
        }

        if (queryString == null)
        {
            throw new ArgumentNullException(nameof(queryString));
        }

        Int32 anchorIndex = uri.IndexOf('#');
        String uriToBeAppended = uri;
        String anchorText = "";

        // If there is an anchor, then the query string must be inserted before its first occurrence.
        if (anchorIndex != -1)
        {
            anchorText = uri.Substring(anchorIndex);
            uriToBeAppended = uri.Substring(0, anchorIndex);
        }

        Int32 queryIndex = uriToBeAppended.IndexOf('?');
        Boolean hasQuery = queryIndex != -1;

        StringBuilder sb = new StringBuilder();
        sb.Append(uriToBeAppended);

        foreach (KeyValuePair<String, String> parameter in queryString)
        {
            sb.Append(hasQuery ? '&' : '?');
            sb.Append(UrlEncoder.Default.Encode(parameter.Key));
            sb.Append('=');
            sb.Append(UrlEncoder.Default.Encode(parameter.Value));
            
            hasQuery = true;
        }

        sb.Append(anchorText);
        
        return sb.ToString();
    }

    #endregion

    #endregion
}