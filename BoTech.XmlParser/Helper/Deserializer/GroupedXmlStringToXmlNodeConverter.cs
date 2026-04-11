using BoTech.XmlParser.Models;
using BoTech.XmlParser.Models.Deserializer;

namespace BoTech.XmlParser.Helper.Deserializer;

public class GroupedXmlStringToXmlNodeConverter
{
    public XmlNode ConvertGroupedXmlNodesToXmlNodes(GroupedXmlStringNode groupedXmlStringNode)
    {
        XmlNode parentNode = XmlNode.CreateRootXmlNode();
        ConvertGroupedXmlNodesToXmlNodesRecursive(groupedXmlStringNode, parentNode);
        return parentNode.Children[0]; // return not the ROOT node
    }

    private void ConvertGroupedXmlNodesToXmlNodesRecursive(GroupedXmlStringNode groupedXmlStringNodes,
        XmlNode parentNode)
    {
        XmlNode currentNode = ParseXmlPropertiesAnsXmlNameFromGroupedXmlString(groupedXmlStringNodes, parentNode);
        foreach (GroupedXmlStringNode groupedXmlStringNode in groupedXmlStringNodes.Children) ConvertGroupedXmlNodesToXmlNodesRecursive(groupedXmlStringNode, currentNode);
    }
    private XmlNode ParseXmlPropertiesAnsXmlNameFromGroupedXmlString(GroupedXmlStringNode groupedXmlStringNode, XmlNode parentNode)
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
    private bool IsXmlPropertyAnInternalProperty(XmlProperty property) => property.ActualName == "_nsp" || property.ActualName.Contains("_gt-");
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
    private (string remainingString, string xmlName) GetXmlNameOfGroupedXmlStringWithPropertiesAndReturnRemaining(string groupedXmlString)
    {
        int indexOfFirstSpace = groupedXmlString.IndexOf(' ');
        if(indexOfFirstSpace == -1) throw new InvalidOperationException("Expected a space char in the string:"  + groupedXmlString);
        return (groupedXmlString[(indexOfFirstSpace + 1)..],groupedXmlString[..indexOfFirstSpace]);
    }
    private static string GetXmlNameForGroupedXmlStringWithoutProperties(string groupedXmlString) =>
        groupedXmlString.Replace(">", "");
    private static bool IsGroupedXmlStringPropertyIdentifier(string groupedXmlString) => groupedXmlString.Split(".").Length == 2;
    
    private static bool HasGroupedXmlStringPropertiesDefined(string groupedXmlString) => groupedXmlString.Contains(' ');
}