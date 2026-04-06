using System.Collections.Generic;

namespace BoTech.XmlParser.Models.Deserializer;

public class GroupedXmlStringNode(string groupedXmlString)
{
    /// <summary>
    /// The XML string which contains all properties and the class name or the xmlName.
    /// </summary>
    public string GroupedXmlString { get; init; } = groupedXmlString;
    /// <summary>
    /// All Children defined as XmlContent in the XmlString.
    /// </summary>
    public List<GroupedXmlStringNode> Children = new List<GroupedXmlStringNode>();
}