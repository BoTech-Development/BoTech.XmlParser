namespace BoTech.XmlParser.Models;

public class XmlProperty
{
    /// <summary>
    /// The Type of the defined property.
    /// </summary>
    public Type? ValueType { get; init; }
    /// <summary>
    /// Name before the equals char.
    /// </summary>
    public string Name { get; init; }
    /// <summary>
    /// The actual name defined by the <see cref="XmlName"/> attribute.
    /// </summary>
    public string ActualName { get; init; }
    /// <summary>
    /// Value of the property.
    /// </summary>
    public string? Value { get; init; }

    public XmlProperty(Type valueType,string name, string actualName, string? value)
    {
        ValueType = valueType;
        Name = name;
        ActualName = actualName;
        Value = value;
    }
    public XmlProperty(string name, string actualName, string? value)
    {
        Name = name;
        ActualName = actualName;
        Value = value;
    }
}