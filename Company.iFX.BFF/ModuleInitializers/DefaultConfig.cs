namespace Company.iFX.BFF.ModuleInitializers;

public class DefaultAppSettingsSection : IAppSettingsSection
{
    #region Methods

    #region Public

    public void Apply(ProxyOptions options)
    {
    }


    public Boolean Validate(out IEnumerable<String> errors)
    {
        errors = Array.Empty<String>();
        return true;
    }

    #endregion

    #endregion
}