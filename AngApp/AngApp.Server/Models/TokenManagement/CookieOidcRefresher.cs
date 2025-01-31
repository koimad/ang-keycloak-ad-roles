using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace AngApp.Server.Models.TokenManagement;

public class CookieOidcRefresher(IOptionsMonitor<OpenIdConnectOptions> oidcOptionsMonitor)
{
    #region Members

    private readonly OpenIdConnectProtocolValidator oidcTokenValidator = new OpenIdConnectProtocolValidator {
        // We no longer have the original nonce cookie which is deleted at the end of the authorization code flow having served its purpose.
        // Even if we had the nonce, it's likely expired. It's not intended for refresh requests. Otherwise, we'd use oidcOptions.ProtocolValidator.
        RequireNonce = false
    };

    #endregion

    #region Methods

    #region Private

    private static async Task AddClaimsFromUserInfoEndpointAsync(String userInfoEndpoint, String accessToken, String oidcScheme,
        JwtSecurityToken validatedIdToken, ClaimsIdentity identity, OpenIdConnectOptions options, CancellationToken cancellationToken)
    {
        HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, userInfoEndpoint);

        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
        requestMessage.Version = options.Backchannel.DefaultRequestVersion;
        
        HttpResponseMessage responseMessage = await options.Backchannel.SendAsync(requestMessage, cancellationToken);
        
        responseMessage.EnsureSuccessStatusCode();
        
        String userInfoResponse = await responseMessage.Content.ReadAsStringAsync(cancellationToken);

        String userInfoJson;
        MediaTypeHeaderValue? contentType = responseMessage.Content.Headers.ContentType;

        if (contentType?.MediaType?.Equals("application/json", StringComparison.OrdinalIgnoreCase) ?? false)
        {
            userInfoJson = userInfoResponse;
        }
        else if (contentType?.MediaType?.Equals("application/jwt", StringComparison.OrdinalIgnoreCase) ?? false)
        {
            JwtSecurityToken userInfoEndpointJwt = new JwtSecurityToken(userInfoResponse);
            userInfoJson = userInfoEndpointJwt.Payload.SerializeToJson();
        }
        else
        {
            return;
        }

        using JsonDocument user = JsonDocument.Parse(userInfoJson);

        options.ProtocolValidator.ValidateUserInfoResponse(new OpenIdConnectProtocolValidationContext {
            UserInfoEndpointResponse = userInfoResponse,
            ValidatedIdToken = validatedIdToken
        });

        foreach (ClaimAction action in options.ClaimActions)
        {
            action.Run(user.RootElement, identity, options.ClaimsIssuer ?? oidcScheme);
        }
    }

    #endregion

    #region Public

    public async Task ValidateOrRefreshCookieAsync(CookieValidatePrincipalContext validateContext, String oidcScheme)
    {
        String? accessTokenExpirationText = validateContext.Properties.GetTokenValue("expires_at");

        if (!DateTimeOffset.TryParse(accessTokenExpirationText, out DateTimeOffset accessTokenExpiration))
        {
            return;
        }

        OpenIdConnectOptions oidcOptions = oidcOptionsMonitor.Get(oidcScheme);
        DateTimeOffset now = oidcOptions.TimeProvider!.GetUtcNow();

        if (now + TimeSpan.FromMinutes(5) < accessTokenExpiration)
        {
            return;
        }

        OpenIdConnectConfiguration? oidcConfiguration = await oidcOptions.ConfigurationManager!.GetConfigurationAsync(validateContext.HttpContext.RequestAborted);
        String tokenEndpoint = oidcConfiguration.TokenEndpoint ?? throw new InvalidOperationException("Cannot refresh cookie. TokenEndpoint missing!");

        using HttpResponseMessage refreshResponse = await oidcOptions.Backchannel.PostAsync(tokenEndpoint,
            new FormUrlEncodedContent(new Dictionary<String, String?> {
                ["grant_type"] = "refresh_token",
                ["client_id"] = oidcOptions.ClientId,
                ["client_secret"] = oidcOptions.ClientSecret,
                ["scope"] = String.Join(" ", oidcOptions.Scope),
                ["refresh_token"] = validateContext.Properties.GetTokenValue("refresh_token")
            }));

        if (!refreshResponse.IsSuccessStatusCode)
        {
            validateContext.RejectPrincipal();
            return;
        }

        String refreshJson = await refreshResponse.Content.ReadAsStringAsync();
        OpenIdConnectMessage message = new OpenIdConnectMessage(refreshJson);

        TokenValidationParameters? validationParameters = oidcOptions.TokenValidationParameters.Clone();

        if (oidcOptions.ConfigurationManager is BaseConfigurationManager baseConfigurationManager)
        {
            validationParameters.ConfigurationManager = baseConfigurationManager;
        }
        else
        {
            validationParameters.ValidIssuer = oidcConfiguration.Issuer;
            validationParameters.IssuerSigningKeys = oidcConfiguration.SigningKeys;
        }

        TokenValidationResult? validationResult = await oidcOptions.TokenHandler.ValidateTokenAsync(message.IdToken, validationParameters);

        if (!validationResult.IsValid)
        {
            validateContext.RejectPrincipal();
            return;
        }

        JwtSecurityToken? validatedIdToken = JwtSecurityTokenConverter.Convert(validationResult.SecurityToken as JsonWebToken);
        validatedIdToken.Payload["nonce"] = null;

        oidcTokenValidator.ValidateTokenResponse(new OpenIdConnectProtocolValidationContext {
            ProtocolMessage = message,
            ClientId = oidcOptions.ClientId,
            ValidatedIdToken = validatedIdToken
        });

        if (oidcOptions.GetClaimsFromUserInfoEndpoint && !String.IsNullOrEmpty(oidcConfiguration.UserInfoEndpoint))
        {
            await AddClaimsFromUserInfoEndpointAsync(oidcConfiguration.UserInfoEndpoint, message.AccessToken, oidcScheme,
                validatedIdToken, validationResult.ClaimsIdentity, oidcOptions, validateContext.HttpContext.RequestAborted);
        }

        validateContext.ShouldRenew = true;
        validateContext.ReplacePrincipal(new ClaimsPrincipal(validationResult.ClaimsIdentity));

        Int32 expiresIn = Int32.Parse(message.ExpiresIn, NumberStyles.Integer, CultureInfo.InvariantCulture);
        DateTimeOffset expiresAt = now + TimeSpan.FromSeconds(expiresIn);

        validateContext.Properties.StoreTokens([
            new AuthenticationToken { Name = "access_token", Value = message.AccessToken },
            new AuthenticationToken { Name = "id_token", Value = message.IdToken },
            new AuthenticationToken { Name = "refresh_token", Value = message.RefreshToken },
            new AuthenticationToken { Name = "token_type", Value = message.TokenType },
            new AuthenticationToken { Name = "expires_at", Value = expiresAt.ToString("o", CultureInfo.InvariantCulture) }
        ]);
    }

    #endregion

    #endregion
}