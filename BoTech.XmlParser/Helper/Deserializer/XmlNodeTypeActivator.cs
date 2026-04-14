using System.Reflection;
using BoTech.XmlParser.Models;
using BoTech.XmlParser.Services;

namespace BoTech.XmlParser.Helper.Deserializer;

public class XmlNodeTypeActivator
{
    public object? CreateInstanceAndPopulatePrimitivePropertiesForXmlNode(XmlNode node)
    {
        object? instance = CreateInstanceForXmlNode(node);
        if (instance != null)
        {
            PopulatePrimitivePropertiesForXmlNode(node, instance);
        }
        return instance;
    }

    private void PopulatePrimitivePropertiesForXmlNode(XmlNode node, object instance)
    {
        List<PropertyInfo> propertiesDefinedInTypeInNode = instance.GetType().GetProperties().ToList();
        foreach (XmlProperty xmlProperty in node.Properties)
        {
            PropertyInfo? propertyInfo = XmlNodePropertyValidator.GetPropertyInfoFromXmlPropertyAndDeclaredProperties(xmlProperty, propertiesDefinedInTypeInNode);
            if(propertyInfo == null) 
                throw new ArgumentException($"The property: {xmlProperty.GetNameOfThisPropertyInAXmlDocument()} is not defined in the type: {node.ReferencedType?.FullName}.");
            
            propertyInfo.SetValue(instance, TypeConverter.ConvertObjectToSpecificType(xmlProperty.Value, propertyInfo.PropertyType));
        }
    }
    private object? CreateInstanceForXmlNode(XmlNode node)
    {
        if(node.ReferencedType != null) 
            return Activator.CreateInstance(node.ReferencedType);
        else
            return null;
    }
}