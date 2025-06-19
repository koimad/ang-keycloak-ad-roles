using Microsoft.AspNetCore.Http;

namespace Company.iFX.BFF.OpenIdConnect;

public interface IRedirectUriFactory
{
    String DetermineHostName(HttpContext context);


    String DetermineRedirectUri(HttpContext context, PathString endpointName);
}