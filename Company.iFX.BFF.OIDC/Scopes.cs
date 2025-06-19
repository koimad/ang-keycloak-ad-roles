namespace Company.iFX.BFF.OIDC;

public class Scopes : List<String>
{
    #region Constructors

    public Scopes(IEnumerable<String> scopes)
    {
        AddRange(scopes.Select(x => x.ToLowerInvariant()));

        const String openId = "openid";

        if (!Contains(openId))
        {
            Add(openId);
        }

        const String offlineAccessScope = "offline_access";

        if (!Contains(offlineAccessScope))
        {
            Add(offlineAccessScope);
        }
    }

    #endregion
}