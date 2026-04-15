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
    public string PropertyName { get; init; }
    /// <summary>
    /// The actual name defined by the <see cref="XmlName"/> attribute.
    /// </summary>
    public string XmlName { get; init; }
    /// <summary>
    /// Value of the property.
    /// </summary>
    public string? Value { get; init; }

    public XmlProperty(Type valueType, string propertyName, string xmlName, string? value)
    {
        ValueType = valueType;
        PropertyName = propertyName;
        XmlName = xmlName;
        Value = value;
    }
    public XmlProperty(string propertyName, string xmlName, string? value)
    {
        PropertyName = propertyName;
        XmlName = xmlName;
        Value = value;
    }
    /// <summary>
    /// Gets the actual name.
    /// </summary>
    /// <returns></returns>
    public string GetNameOfThisPropertyInAXmlDocument() => XmlName == "" ? PropertyName : XmlName;
}