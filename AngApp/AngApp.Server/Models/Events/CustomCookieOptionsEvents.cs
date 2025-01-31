using System.Security.Claims;

using Microsoft.AspNetCore.Authentication.Cookies;

namespace AngApp.Server.Models.Events;

public class CustomCookieOptionsEvents : CookieAuthenticationEvents
{
    #region Methods

    #region Public

    public override Task SigningIn(CookieSigningInContext context)
    {
        Console.WriteLine();
        Console.WriteLine("Claims received by the Cookie handler");

        if (context.Principal?.Claims != null)
        {
            foreach (Claim claim in context.Principal.Claims)
            {
                Console.WriteLine($"{claim.Type} - {claim.Value}");
            }
        }

        Console.WriteLine();

        return base.SigningIn(context);
    }

    #endregion

    #endregion
}