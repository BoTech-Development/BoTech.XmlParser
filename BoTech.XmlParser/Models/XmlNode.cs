using System.Reflection;
using BoTech.XmlParser.Attributes;
using BoTech.XmlParser.Helper.Serializer;

namespace BoTech.XmlParser.Models;

public class XmlNode
{
    /// <summary>
    /// The name of the class or the defined xml name.
    /// </summary>
    public string ClassName { get; init; } 
    /// <summary>
    /// The actual name defined by the <see cref="XmlName"/> attribute.
    /// </summary>
    public string XmlName { get; init; }
    /// <summary>
    /// All Properties that are defined in the start node tag.
    /// </summary>
    public List<XmlProperty> Properties { get; init; } = new();
    /// <summary>
    /// A list of internal properties used for e.g. exact identification of the type which is described by the current node.
    /// </summary>
    public List<XmlProperty> InternalSerializerProperties { get; init; } = new List<XmlProperty>();
    /// <summary>
    /// All inner children.
    /// </summary>
    public List<XmlNode> Children { get; init; } = new();
    /// <summary>
    /// The Name of the class and the property name which contains an object. => Example: <BoForm.FormContent>{Object Notation goes here}</BoForm.FormContent>
    /// </summary>
    public string NameOfParentClassAndProperty { get; init; } = string.Empty;
    /// <summary>
    /// Indicates whether the current instance is used as a property identifier node.
    /// A property identifier node is determined by the presence of a non-empty
    /// parent class and property name combination.
    /// </summary>
    public bool IsPropertyIdentifier => NameOfParentClassAndProperty != "";
    /// <summary>
    /// The corresponding type to this XmlNode
    /// </summary>
    public Type? ReferencedType { get; set; } = null;
    /// <summary>
    /// The referenced object to this XmlNode. This Property is set by the <see cref="XmlDeserializer"/>
    /// </summary>
    public object? Value { get; set; } = null;

    private XmlNode(string className, string xmlName, string nameOfParentClassAndProperty, Type? referencedType)
    {
        ClassName = className;
        XmlName = xmlName;
        ReferencedType = referencedType;
        NameOfParentClassAndProperty = nameOfParentClassAndProperty;
    }
    
    public static XmlNode CreateXmlNodeFromTypeAndProperty(Type referencedType, string xmlName, PropertyInfo? parentPropertyInfo) 
        => new XmlNode(referencedType.Name, 
                        xmlName,
                       GetPropertyIdentifierFromProperty(parentPropertyInfo), 
                       referencedType);
    public static XmlNode CreateEmptyNodeWithXmlNode(string xmlName) 
        => new XmlNode("", 
                        xmlName,
                        string.Empty,
                        null);
    public static XmlNode CreatePropertyIdentifierXmlNode(string nameOfParentClassAndProperty) 
        => new XmlNode("PROPERTYIDENTIFIER",
            "PROPERTYIDENTIFIER",
                      nameOfParentClassAndProperty,
            null);
    public static XmlNode CreateRootXmlNode() 
        => new XmlNode("ROOT",
            "ROOT",
            "",
            null);
    public static XmlNode CreatePropertyIdentifierXmlNode(PropertyInfo? parentPropertyInfo) 
        => new XmlNode("PROPERTYIDENTIFIER",
            "PROPERTYIDENTIFIER",
                       GetPropertyIdentifierFromProperty(parentPropertyInfo), 
            null);
    
    private static string GetPropertyIdentifierFromProperty(PropertyInfo? parentPropertyInfo)
    {
        string nameOfParentClassAndProperty = "";
        if(parentPropertyInfo != null && parentPropertyInfo.DeclaringType != null)
            nameOfParentClassAndProperty =  $"{parentPropertyInfo.DeclaringType.Name}.{parentPropertyInfo.Name}";
        return nameOfParentClassAndProperty;
    }
    public string GetNameOfThisNodeInAXmlDocument() => XmlName == "" ? ClassName : XmlName;

    public string GetDeclaredNamespaceIfExist()
    {
        if(InternalSerializerProperties.Count == 0) return "";
        string? declaredNamespace = InternalSerializerProperties.FirstOrDefault(property => property.GetNameOfThisPropertyInAXmlDocument() == "_nsp")?.Value;
        if(declaredNamespace == null) return "";
        return declaredNamespace;
    }
    /// <summary>
    /// Serializes the current node and all children using the <see cref="XmlNodeSerializer"/>.
    /// </summary>
    /// <returns>A string representation of this node.</returns>
    public string Serialize() => new XmlNodeSerializer().SerializeNodeAndChildren(this);
}