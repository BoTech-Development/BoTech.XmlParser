using System.Reflection;
using BoTech.XmlParser.Attributes;

namespace BoTech.XmlParser.Models;

public class XmlNode
{
    /// <summary>
    /// Will be declared in xml when the same class name is declared in another xmlnode
    /// </summary>
    public string? NamespaceOfReferencedClass { get; init; }
    /// <summary>
    /// The name of the class or the defined xml name.
    /// </summary>
    public string ClassName { get; init; } 
    /// <summary>
    /// The actual name defined by the <see cref="XmlName"/> attribute.
    /// </summary>
    public string ActualName { get; init; }
    /// <summary>
    /// All Properties that are defined in the start node tag.
    /// </summary>
    public List<XmlProperty> Properties { get; init; } = new();
    /// <summary>
    /// All inner children.
    /// </summary>
    public List<XmlNode> Children { get; init; } = new();
    /// <summary>
    /// The Name of the class and the property name which contains an object. => Example: <BoForm.FormContent>{Object Notation goes here}</BoForm.FormContent>
    /// </summary>
    public string NameOfParentClassAndProperty { get; init; } = string.Empty;

    private const int CountOfEmptyCharsOfATab = 4;

    public XmlNode(string className, string actualName, string? namespaceOfClass, PropertyInfo? parentPropertyInfo)
    {
        NamespaceOfReferencedClass = namespaceOfClass;
        ClassName = className;
        ActualName = actualName;
        if(parentPropertyInfo != null && parentPropertyInfo.DeclaringType != null)
            NameOfParentClassAndProperty =  $"{parentPropertyInfo.DeclaringType.Name}.{parentPropertyInfo.Name}";
    }
    public string Serialize(int countOfTabs)
    {
        string tabs = GenerateTabString(countOfTabs);
        string name = ActualName == ClassName ? ClassName : ActualName;
        string xml = tabs + $"<{name}";
        xml += SerializeProperties();
        if (Children.Count == 0)
            xml += "/>\n";
        else
        {
            xml += ">\n";
            xml += SerializeChildren(countOfTabs + 1);
            xml += tabs + $"</{name}>\n";
        }
        return xml;
    }
    private string GenerateTabString(int countOfTabs) => new string(' ', countOfTabs * CountOfEmptyCharsOfATab);
    private string SerializeProperties()
    {
        string propertyXml = "";
        foreach (XmlProperty property in Properties)
        {
            propertyXml += $" {property.Name}=\"{property.Value}\"";
        }
        return propertyXml;
    }

    private string SerializeChildren(int countOfTabs)
    {
        if (IsPropertyIdentifierNeededForThisXmlNode())
        {
            Dictionary<string, List<XmlNode>> groupedNodesByParentClass = GroupXmlNodesByThePropertyIdentifier();
            string tabs = GenerateTabString(countOfTabs);
            return SerializeChildrenWithPropertyIdentifier(groupedNodesByParentClass, countOfTabs, tabs);
        }
        else
        {
            return SerializeChildrenWithoutPropertyIdentifier(countOfTabs);
        }
    }
    /// <summary>
    /// Groups the <see cref="Children"/> Property by their Parent property identifier.
    /// </summary>
    /// <returns>A Dictionary which contains the identifiers as a key.</returns>
    private Dictionary<string, List<XmlNode>> GroupXmlNodesByThePropertyIdentifier()
    {
        Dictionary<string, List<XmlNode>> groupedNodesByParentClass = new();
        foreach (XmlNode xmlNodeIdentifier in Children)
        {
            if(groupedNodesByParentClass.ContainsKey(xmlNodeIdentifier.NameOfParentClassAndProperty))
                groupedNodesByParentClass[xmlNodeIdentifier.NameOfParentClassAndProperty].AddRange(xmlNodeIdentifier.Children);
            else
                groupedNodesByParentClass.Add(xmlNodeIdentifier.NameOfParentClassAndProperty, xmlNodeIdentifier.Children);
        }
        return groupedNodesByParentClass;
    } 
    private string SerializeChildrenWithPropertyIdentifier(Dictionary<string,List<XmlNode>> groupedNodesByParentClass,  int countOfTabs, string tabs)
    {
        string childrenXml = "";
        foreach (KeyValuePair<string, List<XmlNode>> pair in groupedNodesByParentClass)
        {
            childrenXml += tabs + $"<{pair.Key}>\n";
            foreach (XmlNode child in pair.Value)
            {
                childrenXml += child.Serialize(countOfTabs + 1);
            }
            childrenXml += tabs + $"</{pair.Key}>\n";
        }
        return childrenXml;
    }
    private string SerializeChildrenWithoutPropertyIdentifier(int countOfTabs)
    {
        string childrenXml = "";
        foreach (XmlNode child in Children)
        {
            childrenXml += child.Serialize(countOfTabs);
        }
        return childrenXml;
    }
    /// <summary>
    /// Returns true when the xml node like: <Form.FormContent></Form.FormContent> is needed.
    /// </summary>
    /// <returns></returns>
    private bool IsPropertyIdentifierNeededForThisXmlNode() => Children.Any(child => child.NameOfParentClassAndProperty != "");
    
}