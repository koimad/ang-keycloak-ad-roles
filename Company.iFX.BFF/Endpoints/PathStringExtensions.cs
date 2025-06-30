using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Http;

namespace Company.iFX.BFF.Endpoints;

public static class PathStringExtensions
{
    #region Methods

    #region Public

    public static PathString RemoveQueryString(this PathString path)
    {
        String value = path.Value ?? String.Empty;

        if (value.Contains('?'))
        {
            value = value.Split('?')[0];
        }

        return value.ToLowerInvariant();
    }


    public static PathString TrimEnd(this PathString path, String pathSection)
    {
        String regex = $"{pathSection.Replace("/", @"\/")}[\\/]?$";

        String value = path.Value ?? String.Empty;

        String endpointName = Regex.Replace(value, regex, String.Empty);
        return endpointName.ToLowerInvariant();
    }

    #endregion

    #endregion
}