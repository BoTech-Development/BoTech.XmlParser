using BoTech.XmlParser.Models;
using BoTech.XmlParser.Models.Deserializer;

namespace BoTech.XmlParser.Helper.Deserializer;

public class GroupedXmlStringToXmlNodeConverter
{
    /// <summary>
    /// Converts a GroupedXmlStringNode to a XmlNode.
    /// </summary>
    /// <param name="groupedXmlStringNode">
    /// An object that defines the hierarchy of the XmlNode.
    /// </param>
    /// <returns></returns>
    public XmlNode ConvertGroupedXmlNodesToXmlNodes(GroupedXmlStringNode groupedXmlStringNode)
    {
        XmlNode parentNode = XmlNode.CreateRootXmlNode();
        ConvertGroupedXmlNodesToXmlNodesRecursive(groupedXmlStringNode, parentNode);
        return parentNode.Children[0]; // return not the ROOT node
    }
    /// <summary>
    /// Recursive Method that converts a GroupedXmlStringNode to a XmlNode.
    /// </summary>
    /// <param name="groupedXmlStringNodes"></param>
    /// <param name="parentNode"></param>
    private void ConvertGroupedXmlNodesToXmlNodesRecursive(GroupedXmlStringNode groupedXmlStringNodes,
        XmlNode parentNode)
    {
        XmlNode currentNode = ParseXmlPropertiesAndXmlNameFromGroupedXmlString(groupedXmlStringNodes, parentNode);
        foreach (GroupedXmlStringNode groupedXmlStringNode in groupedXmlStringNodes.Children) ConvertGroupedXmlNodesToXmlNodesRecursive(groupedXmlStringNode, currentNode);
    }
    /// <summary>
    /// Parse the XmlProperties and the XmlName from the GroupedXmlStringNode.
    /// </summary>
    /// <param name="groupedXmlStringNode"></param>
    /// <param name="parentNode"></param>
    /// <returns>Creates the new XmlNode</returns>
    private XmlNode ParseXmlPropertiesAndXmlNameFromGroupedXmlString(GroupedXmlStringNode groupedXmlStringNode, XmlNode parentNode)
    {
        XmlNode currentNode;
        if (HasGroupedXmlStringPropertiesDefined(groupedXmlStringNode.GroupedXmlString))
        {
            (string remainingProperties, string xmlName) = GetXmlNameOfGroupedXmlStringWithPropertiesAndReturnRemaining(groupedXmlStringNode.GroupedXmlString);
            currentNode = XmlNode.CreateEmptyNodeWithXmlNode(xmlName);
            remainingProperties = remainingProperties.Replace(">", "");
            CreatePropertiesFromPropertyXmlString(remainingProperties, currentNode);
        }
        else
        {
            if (IsGroupedXmlStringPropertyIdentifier(groupedXmlStringNode.GroupedXmlString))
            {
                currentNode = 
                    XmlNode.CreatePropertyIdentifierXmlNode(
                        GetXmlNameForGroupedXmlStringWithoutProperties(groupedXmlStringNode.GroupedXmlString));
            }
            else
            {
                currentNode =
                    XmlNode.CreateEmptyNodeWithXmlNode(
                        GetXmlNameForGroupedXmlStringWithoutProperties(groupedXmlStringNode.GroupedXmlString));
            }
        }
        parentNode.Children.Add(currentNode);
        return currentNode;
    }
    /// <summary>
    /// Parses the given property XML string and adds the resulting properties
    /// to the specified XmlNode's property collections.
    /// </summary>
    /// <param name="propertyXmlString">
    /// A string containing properties in an XML format to be parsed.
    /// </param>
    /// <param name="currentNode">
    /// The XmlNode to which the parsed properties will be added.
    /// </param>
    private void CreatePropertiesFromPropertyXmlString(string propertyXmlString, XmlNode currentNode)
    {
        List<string> propertiesAsStrings = SeparatePropertiesFromPropertyXmlString(propertyXmlString);//propertyXmlString.Split(" ");
        XmlProperty createdProperty;
        foreach (string propertyAsString in propertiesAsStrings)
        {
            createdProperty = CreateXmlPropertyFromXmlPropertyString(propertyAsString);
            if(IsXmlPropertyAnInternalProperty(createdProperty))
                currentNode.InternalSerializerProperties.Add(createdProperty);
            else
                currentNode.Properties.Add(createdProperty);
        }
    }
    /// <summary>
    /// Separates individual properties from a property XML string.
    /// </summary>
    /// <param name="propertyXmlString">
    /// A string representing the property XML from which individual properties will be extracted.
    /// </param>
    /// <returns>
    /// A list of strings, where each string represents an individual property extracted from the input XML string.
    /// </returns>
    private List<string> SeparatePropertiesFromPropertyXmlString(string propertyXmlString)
    {
        List<string> separatedProperties = new List<string>();
        bool hadReadFirstQuotationMark = false;
        int endIndex = 0;
        for (int c = 0; c < propertyXmlString.Length; c++)
        {
            if (propertyXmlString[c] == '"')
            {
                if (hadReadFirstQuotationMark)
                {
                    endIndex = c;
                    if (c + 1 < propertyXmlString.Length)
                    {
                        endIndex = c + 1;
                    }
                    separatedProperties.Add(propertyXmlString[0..endIndex].Trim());
                    propertyXmlString = propertyXmlString.Remove(0, endIndex);
                    hadReadFirstQuotationMark = false;
                    c = 0;
                }
                else
                {
                    hadReadFirstQuotationMark = true;
                }
            }
        }
        return separatedProperties;
    }
    /// <summary>
    /// Returns true when the property is an internal property such as the namspace declaration or generic type declaration.
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    private bool IsXmlPropertyAnInternalProperty(XmlProperty property)
    {
        string nameOfPropertyInXmlDocument = property.GetNameOfThisPropertyInAXmlDocument();
        return nameOfPropertyInXmlDocument == "_nsp" || nameOfPropertyInXmlDocument == "_gt";
    }

    /// <summary>
    /// Creates an XmlProperty instance from a provided property string.
    /// </summary>
    /// <param name="propertyXmlString">
    /// A string representation of a property in an XML document. The string must include a name and a value separated by an '=' character.
    /// </param>
    /// <returns>
    /// An XmlProperty object representing the parsed name and value.
    /// </returns>
    private XmlProperty CreateXmlPropertyFromXmlPropertyString(string propertyXmlString)
    {
        int propertyEqualsCharIndex = propertyXmlString.IndexOf('=');
        if (propertyEqualsCharIndex < propertyXmlString.Length)
        {
            string propertyName = propertyXmlString[..propertyEqualsCharIndex];
            string propertyValue =  propertyXmlString[(propertyEqualsCharIndex + 1)..];
            if(propertyValue.StartsWith('"')) propertyValue = propertyValue[1..];
            if(propertyValue.EndsWith('"')) propertyValue = propertyValue[..^1];
            return new XmlProperty("",propertyName, propertyValue);
        }
        Console.WriteLine(propertyXmlString); 
        return new XmlProperty("","", "");
    }
    /// <summary>
    /// Extracts the XML name and remaining properties from a grouped XML string that contains properties.
    /// </summary>
    /// <param name="groupedXmlString">
    /// A string containing both the XML name and its associated properties, separated by a space.
    /// </param>
    /// <returns>
    /// A tuple containing the remaining string of properties and the extracted XML name.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the input string does not contain a space character, indicating missing properties or XML name.
    /// </exception>
    private (string remainingString, string xmlName) GetXmlNameOfGroupedXmlStringWithPropertiesAndReturnRemaining(
        string groupedXmlString)
    {
        int indexOfFirstSpace = groupedXmlString.IndexOf(' ');
        if(indexOfFirstSpace == -1) throw new InvalidOperationException("Expected a space char in the string:"  + groupedXmlString);
        return (groupedXmlString[(indexOfFirstSpace + 1)..],groupedXmlString[..indexOfFirstSpace]);
    }
    /// <summary>
    /// Extracts the XML name from a grouped XML string that does not contain properties.
    /// </summary>
    /// <param name="groupedXmlString">
    /// A string representing the grouped XML structure without any properties defined.
    /// </param>
    /// <returns>
    /// The XML name extracted from the provided grouped XML string.
    /// </returns>
    private static string GetXmlNameForGroupedXmlStringWithoutProperties(string groupedXmlString) =>
        groupedXmlString.Replace(">", "");
    /// <summary>
    /// Determines if the given grouped XML string represents a property identifier.
    /// </summary>
    /// <param name="groupedXmlString">
    /// A grouped XML string to be evaluated.
    /// </param>
    /// <returns>
    /// A boolean value indicating whether the grouped XML string represents a property identifier.
    /// </returns>
    private static bool IsGroupedXmlStringPropertyIdentifier(string groupedXmlString) => groupedXmlString.Split(".").Length == 2;
    /// <summary>
    /// Determines whether the provided grouped XML string has properties defined.
    /// </summary>
    /// <param name="groupedXmlString">
    /// The grouped XML string to be checked for properties.
    /// </param>
    /// <returns>
    /// True if properties are defined in the grouped XML string; otherwise, false.
    /// </returns>
    private static bool HasGroupedXmlStringPropertiesDefined(string groupedXmlString) => groupedXmlString.Contains(' ');
}