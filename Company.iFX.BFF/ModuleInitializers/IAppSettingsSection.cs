namespace Company.iFX.BFF.ModuleInitializers;

public interface IAppSettingsSection
{
    public Boolean Validate(out IEnumerable<String> errors);


    public void Apply(ProxyOptions options);
}