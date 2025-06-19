using Microsoft.AspNetCore.Http;

namespace Company.iFX.BFF;

// ReSharper disable once InconsistentNaming
public static class ISessionExtensions
{
    #region Methods

    #region Public

    public static String? GetAccessToken(this ISession session)
    {
        return session?.GetString(AuthSession.TokenKey);
    }

    #endregion

    #endregion
}