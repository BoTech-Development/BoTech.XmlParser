namespace BoTech.XmlParser.Models;

public class XmlProperty(string name, string? value)
{
    /// <summary>
    /// Name before the equals char.
    /// </summary>
    public string Name { get; private set; } = name;

    /// <summary>
    /// Value of the property.
    /// </summary>
    public string? Value { get; private set; } = value;
}