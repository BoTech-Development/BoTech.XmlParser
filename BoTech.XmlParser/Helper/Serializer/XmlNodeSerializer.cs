using BoTech.XmlParser.Models;

namespace BoTech.XmlParser.Helper.Serializer;

public class XmlNodeSerializer
{
    /// <summary>
    /// The number of empty chars (Space characters) of a tab.
    /// </summary>
    private const int CountOfEmptyCharsOfATab = 4;
    /// <summary>
    /// Serializes the given Node to xml and all SubNodes.
    /// </summary>
    /// <param name="node">The given Node</param>
    /// <returns>The xml.</returns>
    public string SerializeNodeAndChildren(XmlNode node)
    {
        return SerializeRecursive(node,0);
    }
    /// <summary>
    /// Serialize all XmlNodes recursive, by using a specific tab count to make the XmlDocument easier to read. 
    /// </summary>
    /// <param name="currentNode"></param>
    /// <param name="countOfTabs">Set 0 when you call this method the first time. Each Node get its own tab count.</param>
    /// <returns>valid xml.</returns>
    private string SerializeRecursive(XmlNode currentNode, int countOfTabs)
    {
        string tabs = GenerateTabString(countOfTabs);
        string name = currentNode.GetNameOfThisNodeInAXmlDocument();
        string xml = tabs + $"<{name}";
        xml += SerializeProperties(currentNode);
        if (currentNode.Children.Count == 0)
            xml += "/>\n";
        else
        {
            xml += ">\n";
            xml += SerializeChildren(currentNode,countOfTabs + 1);
            xml += tabs + $"</{name}>\n";
        }
        return xml;
    }
    /// <summary>
    /// Creates a string that contains as many spaces as specified multiplied by the defined <see cref="CountOfEmptyCharsOfATab"/>.
    /// </summary>
    /// <param name="countOfTabs">The count of spaces</param>
    /// <returns>The new string.</returns>
    private string GenerateTabString(int countOfTabs) => new string(' ', countOfTabs * CountOfEmptyCharsOfATab);
    /// <summary>
    /// Serializes all Properties of the given Node to XML including the internal properties.
    /// </summary>
    /// <param name="currentNode">the given node.</param>
    /// <returns>A string that contains only the serialized properties.</returns>
    private string SerializeProperties(XmlNode currentNode)
    {
        string propertyXml = "";
        foreach (XmlProperty property in currentNode.Properties)
        {
            propertyXml += $" {property.ActualName}=\"{property.Value}\"";
        }
        foreach (XmlProperty property in currentNode.InternalSerializerProperties)
        {
            propertyXml += $" {property.ActualName}=\"{property.Value}\"";
        }
        return propertyXml;
    }

    /// <summary>
    /// Serializes the children of the given Node to XML.
    /// </summary>
    /// <param name="currentNode">The parent node whose children are to be serialized.</param>
    /// <param name="countOfTabs">The number of tabs for indentation in the serialized XML.</param>
    /// <returns>A string containing the serialized representation of the children nodes.</returns>
    private string SerializeChildren(XmlNode currentNode, int countOfTabs)
    {
        if (IsPropertyIdentifierNeededForThisXmlNode(currentNode))
        {
            Dictionary<string, List<XmlNode>> groupedNodesByParentClass = GroupXmlNodesByThePropertyIdentifier(currentNode);
            string tabs = GenerateTabString(countOfTabs);
            return SerializeChildrenWithPropertyIdentifier(groupedNodesByParentClass, countOfTabs, tabs);
        }
        else
        {
            return SerializeChildrenWithoutPropertyIdentifier(currentNode, countOfTabs);
        }
    }
    /// <summary>
    /// Groups the <see cref="XmlNode.Children"/> Property by their Parent property identifier.
    /// </summary>
    /// <returns>A Dictionary which contains the identifiers as a key.</returns>
    private Dictionary<string, List<XmlNode>> GroupXmlNodesByThePropertyIdentifier(XmlNode currentNode)
    {
        Dictionary<string, List<XmlNode>> groupedNodesByParentClass = new();
        foreach (XmlNode xmlNodeIdentifier in currentNode.Children)
        {
            if(groupedNodesByParentClass.ContainsKey(xmlNodeIdentifier.NameOfParentClassAndProperty))
                groupedNodesByParentClass[xmlNodeIdentifier.NameOfParentClassAndProperty].AddRange(xmlNodeIdentifier.Children);
            else
                groupedNodesByParentClass.Add(xmlNodeIdentifier.NameOfParentClassAndProperty, xmlNodeIdentifier.Children);
        }
        return groupedNodesByParentClass;
    }

    /// <summary>
    /// Serializes the child nodes to XML, grouping them by a property identifier.
    /// </summary>
    /// <param name="groupedNodesByParentClass">A dictionary containing child nodes grouped by their parent class property identifier.</param>
    /// <param name="countOfTabs">The number of tabs to indent the XML elements.</param>
    /// <param name="tabs">A string representing the indentation for the current XML level.</param>
    /// <returns>A formatted XML string representing the serialized child nodes.</returns>
    private string SerializeChildrenWithPropertyIdentifier(Dictionary<string, List<XmlNode>> groupedNodesByParentClass,
        int countOfTabs, string tabs)
    {
        string childrenXml = "";
        foreach (KeyValuePair<string, List<XmlNode>> pair in groupedNodesByParentClass)
        {
            childrenXml += tabs + $"<{pair.Key}>\n";
            foreach (XmlNode child in pair.Value)
            {
                childrenXml += SerializeRecursive(child, countOfTabs + 1);
            }
            childrenXml += tabs + $"</{pair.Key}>\n";
        }
        return childrenXml;
    }

    /// <summary>
    /// Serializes the children of the given XmlNode without adding property identifiers.
    /// </summary>
    /// <param name="currentNode">The XmlNode whose children should be serialized.</param>
    /// <param name="countOfTabs">The number of tab spaces to prepend to each child for formatting.</param>
    /// <returns>The serialized xml string of the children.</returns>
    private string SerializeChildrenWithoutPropertyIdentifier(XmlNode currentNode, int countOfTabs)
    {
        string childrenXml = "";
        foreach (XmlNode child in currentNode.Children)
        {
            childrenXml += SerializeRecursive(child, countOfTabs);
        }
        return childrenXml;
    }
    /// <summary>
    /// Returns true when the xml node like: <Form.FormContent></Form.FormContent> is needed.
    /// </summary>
    /// <returns></returns>
    private bool IsPropertyIdentifierNeededForThisXmlNode(XmlNode currentNode) => currentNode.Children.Any(child => child.NameOfParentClassAndProperty != "");
}