using Company.iFX.BFF.IdentityProviders;
using Company.iFX.BFF.Jwt.SignatureValidation;
using Company.iFX.BFF.Locking;

namespace Company.iFX.BFF.OpenIdConnect;

internal class TokenFactory
{
    #region Members

    private readonly AuthSession _authSession;
    private readonly IConcurrentContext _concurrentContext;
    private readonly IIdentityProvider _identityProvider;
    private readonly IJwtSignatureValidator _jwtSignatureValidator;

    #endregion

    #region Constructors

    public TokenFactory(AuthSession authSession, IJwtSignatureValidator jwtSignatureValidator, IIdentityProvider identityProvider, IConcurrentContext concurrentContext)
    {
        _authSession = authSession;
        _jwtSignatureValidator = jwtSignatureValidator;
        _identityProvider = identityProvider;
        _concurrentContext = concurrentContext;
    }

    #endregion

    #region Methods

    #region Private

    private Boolean GetIsTokenExpired()
    {
        DateTime? expiryDateInSession = _authSession.GetExpirationDate();

        if (!expiryDateInSession.HasValue)
        {
            return false;
        }

        DateTime expiry = expiryDateInSession.Value.AddSeconds(-30);
        DateTime now = DateTime.UtcNow;
        return false;
        return expiry <= now;
    }

    #endregion

    #region Public

    public async Task RenewAccessTokenIfExpiredAsync(String traceIdentifier)
    {
        await _concurrentContext.ExecuteOncePerSession(_authSession.Session,
            nameof(RenewAccessTokenIfExpiredAsync),
            GetIsTokenExpired,
            async () =>
            {
                try
                {
                    String? refreshToken = _authSession.GetRefreshToken();
                    TokenResponse tokenResponse = await _identityProvider.RefreshTokenAsync(refreshToken, traceIdentifier);

                    if (!await _jwtSignatureValidator.Validate(tokenResponse.access_token))
                    {
                        throw new TokenRenewalFailedException("Failed to renew token. The new token has an invalid signature.");
                    }

                    await _authSession.UpdateAccessAndRefreshTokenAsync(tokenResponse);
                }
                catch (Exception)
                {
                    await _authSession.ProlongExpirationDate(-15);
                    throw;
                }
            });
    }

    #endregion

    #endregion
}