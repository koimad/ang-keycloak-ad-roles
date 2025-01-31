// ReSharper disable once CheckNamespace
namespace BlazorOpenIdConnect.Client.Models;

public class Band(Int32 id, String name)
{
    #region Properties

    public Int32 Id { get; set; } = id;
    public String Name { get; set; } = name;

    #endregion
}