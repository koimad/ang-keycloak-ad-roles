using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text;

namespace Company.iFX.BFF.IdentityModel.Client.Messages;

public class Parameters : List<KeyValuePair<String, String>>
{
    #region Properties

    public IEnumerable<String> this[String index]
    {
        get { return this.Where(i => i.Key.Equals(index)).Select(i => i.Value); }
    }

    #endregion

    #region Constructors

    public Parameters() { }


    public Parameters(IEnumerable<KeyValuePair<String, String>> values)
        : base(values) { }

    #endregion

    #region Methods

    #region Public

    public void Add(String key, String value, ParameterReplaceBehavior parameterReplace = ParameterReplaceBehavior.None)
    {
        if (key.IsMissing())
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (parameterReplace == ParameterReplaceBehavior.None)
        {
            Add(new KeyValuePair<String, String>(key, value));
            return;
        }

        List<KeyValuePair<String, String>> existingItems = this.Where(i => i.Key == key).ToList();

        if (existingItems.Count > 1 && parameterReplace == ParameterReplaceBehavior.Single)
        {
            throw new InvalidOperationException("More than one item found to replace.");
        }

        existingItems.Clear();

        Add(new KeyValuePair<String, String>(key, value));
    }


    public void AddOptional(String key, String? value, Boolean allowDuplicates = false)
    {
        if (key.IsMissing())
        {
            throw new ArgumentNullException(nameof(key));
        }

        if (value.IsMissing())
        {
            return;
        }

        if (allowDuplicates == false)
        {
            if (ContainsKey(key))
            {
                throw new InvalidOperationException($"Duplicate parameter: {key}");
            }
        }

        Add(key, value!);
    }


    public void AddRequired(String key, String? value, Boolean allowDuplicates = false, Boolean allowEmptyValue = false)
    {
        if (key.IsMissing())
        {
            throw new ArgumentNullException(nameof(key));
        }

        Boolean valuePresent = value.IsPresent();
        Boolean parameterPresent = ContainsKey(key);

        if (!valuePresent && !parameterPresent && !allowEmptyValue)
        {
            throw new ArgumentException("Parameter is required", key);
        }

        if (valuePresent && parameterPresent && !allowDuplicates)
        {
            if (this[key].Contains(value))
            {
                return;
            }

            throw new InvalidOperationException($"Duplicate parameter: {key}");
        }

        if (valuePresent || allowEmptyValue)
        {
            Add(key, value!);
        }
    }


    public Boolean ContainsKey(String key)
    {
        return this.Any(k => String.Equals(k.Key, key));
    }


    [RequiresUnreferencedCode("The FromObject method uses reflection in a way that is incompatible with trimming.")]
    public static Parameters? FromObject(Object? values)
    {
        if (values == null)
        {
            return null;
        }

        if (values is Dictionary<String, String> dictionary)
        {
            return new Parameters(dictionary);
        }

        dictionary = new Dictionary<String, String>();

        foreach (PropertyInfo prop in values.GetType().GetRuntimeProperties())
        {
            String? value = prop.GetValue(values) as String;

            if (value.IsPresent())
            {
                dictionary.Add(prop.Name, value!);
            }
        }

        return new Parameters(dictionary);
    }


    public IEnumerable<String> GetValues(String name)
    {
        return this[name];
    }


    public Parameters Merge(Parameters? additionalValues = null)
    {
        if (additionalValues != null)
        {
            IEnumerable<KeyValuePair<String, String>> merged =
                this.Concat(additionalValues.Where(add => !ContainsKey(add.Key)))
                    .Select(s => new KeyValuePair<String, String>(s.Key, s.Value));

            return new Parameters(merged.ToList());
        }

        return this;
    }

    #endregion

    #endregion
}