using System.Text.RegularExpressions;

namespace Company.iFX.BFF;

internal partial record LandingPage
{
    private readonly String _value;

    public static Boolean TryParse(String url, out LandingPage landingPage)
    {
        landingPage = new LandingPage();

        if (String.IsNullOrEmpty(url))
        {
            return false;
        }

        if (url == "/")
        {
            return true;
        }

        Regex regex = LandingPageRegex();

        if (!regex.IsMatch(url)
            || url.Contains("://")
            || url.Contains("javascript:", StringComparison.InvariantCultureIgnoreCase)
            || url.Contains("javascript%3a", StringComparison.InvariantCultureIgnoreCase))
        {
            return false;
        }

        Boolean isValidUri = Uri.TryCreate(url, UriKind.Relative, out _);

        if (!isValidUri)
        {
            return false;
        }

        landingPage = new LandingPage(url);
        return true;
    }


    public LandingPage()
    {
        _value = "/";
    }


    private LandingPage(String landingPage)
    {
        _value = landingPage;
    }


    public override String ToString()
    {
        return String.IsNullOrEmpty(_value) ? "/" : _value;
    }


    public Boolean Equals(String url)
    {
        return ToString().Equals(url, StringComparison.InvariantCultureIgnoreCase);
    }


    [GeneratedRegex("^\\/[a-zA-Z0-9.]")]
    private static partial Regex LandingPageRegex();
}