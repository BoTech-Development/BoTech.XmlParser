using System.Reflection;
using BoTech.XmlParser.Exceptions;
using BoTech.XmlParser.Helper.Serializer;
using BoTech.XmlParser.Models;

namespace BoTech.XmlParser.Helper.Deserializer;

public class XmlNodePropertyValidator
{
    /// <summary>
    /// Validates if all properties in the XmlNode are declared in the ReferencedType.
    /// </summary>
    /// <param name="node">
    /// The node to check it for.
    /// </param>
    public void CheckIfDeclaredPropertiesAreValid(XmlNode node)
    {
        if(node.ReferencedType != null)
            CheckIfAllPropertiesAreDeclaredInType(node.ReferencedType, node.Properties);
    }
    /// <summary>
    /// Checks if all properties in the XmlProperties are declared in the given Type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="xmlProperties"></param>
    /// <exception cref="XmlMissingPropertyDeclarationException"></exception>
    /// <exception cref="ArgumentException"></exception>
    private void CheckIfAllPropertiesAreDeclaredInType(Type type, List<XmlProperty> xmlProperties)
    {
        List<PropertyInfo> propertiesDeclaredInType = type.GetProperties().ToList();
        PropertyInfo? correspondingProperty = null;
        foreach (XmlProperty xmlProperty in xmlProperties)
        {
            correspondingProperty = GetPropertyInfoFromXmlPropertyAndDeclaredProperties(xmlProperty, propertiesDeclaredInType);
            if(correspondingProperty == null)
                throw new XmlMissingPropertyDeclarationException(
                    $"The property: {xmlProperty.GetNameOfThisPropertyInAXmlDocument()} is not declared in the type: {type.FullName}.");
            if(!correspondingProperty.CanWrite || correspondingProperty.GetSetMethod() == null)
                throw new ArgumentException($"The property: '{xmlProperty.GetNameOfThisPropertyInAXmlDocument()}' in the type {type.FullName} is not writable.");
        }
    }
    /// <summary>
    /// Returns the PropertyInfo from the XmlProperty and the properties declared in the type.
    /// </summary>
    /// <param name="xmlProperty"></param>
    /// <param name="propertiesDeclaredInType"></param>
    /// <returns></returns>
    public static PropertyInfo? GetPropertyInfoFromXmlPropertyAndDeclaredProperties(XmlProperty xmlProperty,
        List<PropertyInfo> propertiesDeclaredInType)
    {
        return propertiesDeclaredInType.Find(propertyInfo =>
            XmlNameEvaluator.GetXmlNameOrActualName(propertyInfo) == xmlProperty.GetNameOfThisPropertyInAXmlDocument());

    }

}