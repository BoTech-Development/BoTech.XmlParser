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

    public string XmlName = "";
    /// <summary>
    /// 
    /// </summary>
    public List<XmlProperty> XmlProperties = new List<XmlProperty>();

    public List<XmlProperty> InternalSerializerProperties = new List<XmlProperty>();
    public bool IsPropertyIdentifier { get; private set; } = false;

    public void InitializeThisAndChildren()
    {
        CreateXmlPropertiesFromGroupedXmlString();
        InitializeChildren();
    }

    private void InitializeChildren()
    {
        foreach (GroupedXmlStringNode child in Children) child.InitializeThisAndChildren();
    }
    public void CreateXmlPropertiesFromGroupedXmlString()
    {
        if (HasGroupedXmlStringPropertiesDefined(GroupedXmlString))
        {
            string remainingProperties = StoreXmlNameOfGroupedXmlStringWithPropertiesAndReturnRemaining(GroupedXmlString);
            remainingProperties = remainingProperties.Replace(">", "");
            CreatePropertiesFromPropertyXmlString(remainingProperties);
        }
        else
        {
            IsPropertyIdentifier = IsGroupedXmlStringPropertyIdentifier(GroupedXmlString);
            if (!IsPropertyIdentifier)
            {
                XmlName = GetXmlNameForGroupedXmlStringWithoutProperties(GroupedXmlString);
            }
        }
    }

    private void CreatePropertiesFromPropertyXmlString(string propertyXmlString)
    {
        List<string> propertiesAsStrings = SeparatePropertiesFromPropertyXmlString(propertyXmlString);//propertyXmlString.Split(" ");
        XmlProperty createdProperty;
        foreach (string propertyAsString in propertiesAsStrings)
        {
            createdProperty = CreateXmlPropertyFromXmlPropertyString(propertyAsString);
            if(IsXmlPropertyAnInternalProperty(createdProperty))
                InternalSerializerProperties.Add(createdProperty);
            else
                XmlProperties.Add(createdProperty);
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
    private string StoreXmlNameOfGroupedXmlStringWithPropertiesAndReturnRemaining(string groupedXmlString)
    {
        int indexOfFirstSpace = groupedXmlString.IndexOf(' ');
        if(indexOfFirstSpace == -1) throw new InvalidOperationException("Expected a space char in the string:"  + groupedXmlString);
        XmlName = groupedXmlString[..indexOfFirstSpace];
        return groupedXmlString[(indexOfFirstSpace + 1)..]; 
    }
    private static string GetXmlNameForGroupedXmlStringWithoutProperties(string groupedXmlString) =>
        groupedXmlString.Replace(">", "");
    private static bool IsGroupedXmlStringPropertyIdentifier(string groupedXmlString) => groupedXmlString.Split(".").Length == 2;
    
    private static bool HasGroupedXmlStringPropertiesDefined(string groupedXmlString) => groupedXmlString.Contains(' ');
}