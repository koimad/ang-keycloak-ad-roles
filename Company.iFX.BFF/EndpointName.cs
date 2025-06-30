namespace Company.iFX.BFF;

public class EndpointName
{
    #region Members

    private readonly String _value;

    #endregion

    #region Constructors

    public EndpointName(String value)
    {
        _value = value;
    }

    #endregion

    #region Methods

    #region Public

    public override String ToString()
    {
        return $"/{_value}";
    }

    #endregion

    #endregion
}