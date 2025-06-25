using Company.iFX.BFF.IdentityModel.Client.Messages;
using Company.iFX.BFF.IdentityModel.Internal;

namespace Company.iFX.BFF.IdentityModel.Client;

public class RequestUrl
{
    #region Members

    private readonly String _baseUrl;

    #endregion

    #region Constructors

    public RequestUrl(String baseUrl)
    {
        _baseUrl = baseUrl;
    }

    #endregion

    #region Methods

    #region Public

    public String Create(Parameters? parameters)
    {
        if (parameters == null || !parameters.Any())
        {
            return _baseUrl;
        }

        return QueryHelpers.AddQueryString(_baseUrl, parameters);
    }

    #endregion

    #endregion
}