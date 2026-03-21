using System.Reflection;

namespace BoTech.XmlParser.Models;

public class XmlObjectNode(string className, string? namespaceOfClass, PropertyInfo parentPropertyInfo) : XmlNode(className, namespaceOfClass,  parentPropertyInfo)
{
    /// <summary>
    /// The Name of the class and the property name which contains an object. => Example: <BoForm.FormContent>{Object Notation goes here}</BoForm.FormContent>
    /// </summary>
    public string NameOfClassAndProperty { get; set; } = string.Empty;
}