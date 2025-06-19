using System.IdentityModel.Tokens.Jwt;

using Company.iFX.BFF.IdentityProviders;
using Company.iFX.BFF.Jwt;
using Company.iFX.BFF.Jwt.SignatureValidation;
using Company.iFX.BFF.Logging;
using Company.iFX.BFF.ModuleInitializers;
using Company.iFX.BFF.OpenIdConnect;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Company.iFX.BFF.Endpoints;

internal static class CallbackEndpoint
{
    #region Methods

    #region Public

    public static async Task<IResult> Get(HttpContext context,
        [FromServices] AuthSession authSession,
        [FromServices] ILogger logger,
        [FromServices] IRedirectUriFactory redirectUriFactory,
        [FromServices] ProxyOptions proxyOptions,
        [FromServices] IIdentityProvider identityProvider,
        [FromServices] ITokenParser tokenParser,
        [FromServices] IJwtSignatureValidator jwtSignatureValidator,
        [FromServices] IAuthenticationCallbackHandler authenticationCallbackHandler)
    {
        try
        {
            String? userPreferredLandingPage = authSession.GetUserPreferredLandingPage();

            String? code = context.Request.Query["code"].SingleOrDefault();

            if (String.IsNullOrEmpty(code))
            {
                await logger.InformAsync("Unable to obtain access token. Querystring parameter 'code' has no value.");

                String redirectUri = $"{proxyOptions.ErrorPage}{context.Request.QueryString}";
                return await authenticationCallbackHandler.OnAuthenticationFailed(context, redirectUri, userPreferredLandingPage);
            }

            PathString endpointName = context.Request.Path.RemoveQueryString().TrimEnd("/login/callback");
            String redirectUrl = redirectUriFactory.DetermineRedirectUri(context, endpointName);

            String? codeVerifier = authSession.GetCodeVerifier();

            await logger.InformAsync("Exchanging code for access_token.");
            TokenResponse tokenResponse = await identityProvider.GetTokenAsync(redirectUrl, code, codeVerifier, context.TraceIdentifier);

            if (!await jwtSignatureValidator.Validate(tokenResponse.access_token))
            {
                return await authenticationCallbackHandler.OnAuthenticationFailed(context,
                    proxyOptions.LandingPage.ToString(),
                    userPreferredLandingPage);
            }

            await authSession.SaveAsync(tokenResponse);

            await logger.InformAsync($"Redirect({proxyOptions.LandingPage})");

            JwtPayload? jwtPayload = tokenParser.ParseJwtPayload(tokenResponse.access_token);

            return await authenticationCallbackHandler.OnAuthenticated(context,
                jwtPayload,
                proxyOptions.LandingPage.ToString(),
                userPreferredLandingPage);
        }
        catch (Exception e)
        {
            await logger.ErrorAsync(e);
            await authenticationCallbackHandler.OnError(context, e);
            throw;
        }
        finally
        {
            await authSession.RemoveCodeVerifierAsync();
        }
    }

    #endregion

    #endregion
}