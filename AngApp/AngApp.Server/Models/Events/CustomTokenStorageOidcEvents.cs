using System.Security.Claims;
using System.Text.Json;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace AngApp.Server.Models.Events;

public class CustomTokenStorageOidcEvents: OpenIdConnectEvents
{
    public override Task TokenValidated(TokenValidatedContext context)
    {
        return base.TokenValidated(context);
    }


    public override Task TicketReceived(TicketReceivedContext context)
    {
        return base.TicketReceived(context);
    }


    public override Task TokenResponseReceived(TokenResponseReceivedContext context)
    {
        return base.TokenResponseReceived(context);
    }


    public override Task AuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
    {
        return base.AuthorizationCodeReceived(context);
    }




    public override Task UserInformationReceived(UserInformationReceivedContext context)
    {
        Console.WriteLine();
        Console.WriteLine("Claims from the ID token");
        foreach (Claim claim in context.Principal.Claims)
        {
            Console.WriteLine($"{claim.Type} - {claim.Value}");
        }
        Console.WriteLine();
        Console.WriteLine("Claims from the UserInfo endpoint");
        foreach (JsonProperty property in context.User.RootElement.EnumerateObject())
        {
            Console.WriteLine($"{property.Name} - {property.Value}");
        }
        return base.UserInformationReceived(context);
    }
}
