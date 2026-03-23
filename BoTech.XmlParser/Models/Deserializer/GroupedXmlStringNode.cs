using System.Collections.Generic;

namespace BoTech.XmlParser.Models.Deserializer;

public class GroupedXmlStringNode(string groupedXmlString)
{
    /// <summary>
    /// The XML string which contains all properties and the class name or the xmlName.
    /// </summary>
    public string GroupedXmlString { get; init; } = groupedXmlString;
    public List<GroupedXmlStringNode> Children = new List<GroupedXmlStringNode>();
}