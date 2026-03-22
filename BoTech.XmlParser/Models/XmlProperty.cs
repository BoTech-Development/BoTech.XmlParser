namespace BoTech.XmlParser.Models;

public class XmlProperty(string name, string actualName, string? value)
{
    /// <summary>
    /// Name before the equals char.
    /// </summary>
    public string Name { get; init; } = name;
    /// <summary>
    /// The actual name defined by the <see cref="XmlName"/> attribute.
    /// </summary>
    public string ActualName { get; init; } = actualName;
    /// <summary>
    /// Value of the property.
    /// </summary>
    public string? Value { get; init; } = value;
}