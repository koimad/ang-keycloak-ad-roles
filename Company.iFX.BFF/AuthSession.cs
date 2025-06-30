using Company.iFX.BFF.IdentityProviders;
using Company.iFX.BFF.OpenIdConnect;

using Microsoft.AspNetCore.Http;

namespace Company.iFX.BFF;

public class AuthSession : IAuthSession
{
    #region Members

    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IIdentityProvider _identityProvider;
    private readonly EndpointName _oidcProxyReservedEndpointName;
    private readonly IRedirectUriFactory _redirectUriFactory;

    #endregion

    #region Properties

    public static String TokenKey => $"{typeof(IIdentityProvider)}-token_key";

    public ISession Session { get; }

    private static String DateFormat => "yyyy-MM-dd HH:mm:ss.fff";

    private static String VerifierKey => $"{typeof(IIdentityProvider)}-verifier_key";

    private static String IdTokenKey => $"{typeof(IIdentityProvider)}-id_token_key";

    private static String RefreshTokenKey => $"{typeof(IIdentityProvider)}-refresh_token_key";

    private static String ExpiryKey => $"{typeof(IIdentityProvider)}-expires";

    private static String UserPreferredLandingPageKey => $"{typeof(IIdentityProvider)}-user_preferred_landing_page";

    #endregion

    #region Constructors

    public AuthSession(IHttpContextAccessor httpContextAccessor,
        IRedirectUriFactory redirectUriFactory,
        IIdentityProvider identityProvider,
        EndpointName oidcProxyReservedEndpointName)
    {
        if (httpContextAccessor.HttpContext?.Session == null)
        {
            throw new NotSupportedException("There is no session.");
        }

        _httpContextAccessor = httpContextAccessor;
        _redirectUriFactory = redirectUriFactory;
        _identityProvider = identityProvider;
        _oidcProxyReservedEndpointName = oidcProxyReservedEndpointName;
        Session = httpContextAccessor.HttpContext?.Session!;
    }

    #endregion

    #region Methods

    #region Private

    private DateTime? GetDateTime(String key)
    {
        String? date = Session.GetString(key);

        return date == null ? null : DateTime.ParseExact(date, DateFormat, null);
    }


    private async Task RemoveAsync(String key)
    {
        Session.Remove(key);
        await Session.CommitAsync();
    }


    private async Task SaveAsync(String key, String? value)
    {
        if (value == null && Session.Keys.Contains(key))
        {
            Session.Remove(key);
        }

        else if (value != null)
        {
            Session.SetString(key, value);
        }

        else
        {
            Session.Remove(key);
        }

        await Session.CommitAsync();
    }


    private async Task SetDateTimeAsync(String key, DateTime? value)
    {
        String? stringValue = value?.ToString(DateFormat);
        await SaveAsync(key, stringValue);
    }

    #endregion

    #region Public

    public String? GetAccessToken()
    {
        return GetAccessTokenFromSession(Session);
    }


    public static String? GetAccessTokenFromSession(ISession session)
    {
        return session.GetString(TokenKey);
    }


    public String? GetCodeVerifier()
    {
        return Session?.GetString(VerifierKey);
    }


    public DateTime? GetExpirationDate()
    {
        return GetDateTime(ExpiryKey);
    }


    public String? GetIdToken()
    {
        return Session?.GetString(IdTokenKey);
    }


    public String? GetRefreshToken()
    {
        return Session?.GetString(RefreshTokenKey);
    }


    public String? GetUserPreferredLandingPage()
    {
        return Session?.GetString(UserPreferredLandingPageKey);
    }


    public static Boolean HasAccessToken(ISession session)
    {
        return session.Keys.Contains(TokenKey);
    }


    public Boolean HasAccessToken()
    {
        return Session.Keys.Contains(TokenKey);
    }


    public Boolean HasIdToken()
    {
        return Session.Keys.Contains(IdTokenKey);
    }


    public Boolean HasRefreshToken()
    {
        return Session.Keys.Contains(RefreshTokenKey);
    }


    public async Task<AuthorizeRequest> InitiateAuthenticationSequence(String userPreferredLandingPage)
    {
        await SetUserPreferredLandingPageAsync(userPreferredLandingPage);

        String redirectUri = _redirectUriFactory.DetermineRedirectUri(
            _httpContextAccessor.HttpContext!,
            _oidcProxyReservedEndpointName.ToString()
        );

        AuthorizeRequest authorizeRequest = await _identityProvider.GetAuthorizeUrlAsync(redirectUri);

        if (!String.IsNullOrEmpty(authorizeRequest.CodeVerifier))
        {
            await SetCodeVerifierAsync(authorizeRequest.CodeVerifier);
        }

        return authorizeRequest;
    }


    public async Task ProlongExpirationDate(Int32 seconds)
    {
        DateTime? current = GetDateTime(ExpiryKey);

        if (current == null)
        {
            throw new NotSupportedException("Cannot prolong access token validity. Can only prolong based on expiry date. But the Expiry was not set in the session.");
        }

        await SetDateTimeAsync(ExpiryKey, current.Value.AddSeconds(seconds));
    }


    public async Task RemoveCodeVerifierAsync()
    {
        await RemoveAsync(VerifierKey);
    }


    public async Task RemoveUserPreferredLandingPageAsyncAsync()
    {
        await RemoveAsync(UserPreferredLandingPageKey);
    }


    public async Task SaveAsync(TokenResponse tokenResponse)
    {
        await SaveAsync(TokenKey, tokenResponse.access_token);
        await SaveAsync(IdTokenKey, tokenResponse.id_token);
        await SaveAsync(RefreshTokenKey, tokenResponse.refresh_token);
        await SetDateTimeAsync(ExpiryKey, tokenResponse.ExpiryDate);
    }


    public async Task SetCodeVerifierAsync(String codeVerifier)
    {
        await SaveAsync(VerifierKey, codeVerifier);
    }


    public async Task SetUserPreferredLandingPageAsync(String? userPreferredLandingPage)
    {
        if (String.IsNullOrEmpty(userPreferredLandingPage))
        {
            await RemoveAsync(UserPreferredLandingPageKey);
            return;
        }

        if (!LandingPage.TryParse(userPreferredLandingPage, out _))
        {
            throw new NotSupportedException($"Will not redirect user to {userPreferredLandingPage}");
        }

        await SaveAsync(UserPreferredLandingPageKey, userPreferredLandingPage);
    }


    public async Task UpdateAccessAndRefreshTokenAsync(TokenResponse tokenResponse)
    {
        await SaveAsync(TokenKey, tokenResponse.access_token);
        await SetDateTimeAsync(ExpiryKey, tokenResponse.ExpiryDate);

        if (!String.IsNullOrEmpty(tokenResponse.refresh_token))
        {
            await SaveAsync(RefreshTokenKey, tokenResponse.refresh_token);
        }
    }

    #endregion

    #endregion
}