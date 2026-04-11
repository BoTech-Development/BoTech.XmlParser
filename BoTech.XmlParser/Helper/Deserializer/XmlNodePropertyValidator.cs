using System.Reflection;
using BoTech.XmlParser.Exceptions;
using BoTech.XmlParser.Helper.Serializer;
using BoTech.XmlParser.Models;

namespace BoTech.XmlParser.Helper.Deserializer;

public class XmlNodePropertyValidator
{
    public void CheckIfDeclaredPropertiesAreValid(XmlNode node)
    {
        if(node.ReferencedType != null)
            CheckIfAllPropertiesAreDeclaredInType(node.ReferencedType, node.Properties);
    }
    private void CheckIfAllPropertiesAreDeclaredInType(Type type, List<XmlProperty> xmlProperties)
    {
        List<PropertyInfo> propertiesDeclaredInType = type.GetProperties().ToList();
        foreach (XmlProperty xmlProperty in xmlProperties)
        {
            if (!propertiesDeclaredInType.Exists(propertyInfo => XmlNameEvaluator.GetXmlNameOrActualName(propertyInfo) == xmlProperty.GetNameOfThisPropertyInAXmlDocument()))
                throw new XmlMissingPropertyDeclarationException(
                    $"The property: {xmlProperty.GetNameOfThisPropertyInAXmlDocument()} is not declared in the type: {type.FullName}.");
        }
    }
}