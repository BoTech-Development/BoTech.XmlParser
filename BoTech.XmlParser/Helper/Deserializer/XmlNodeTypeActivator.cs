using System.Reflection;
using BoTech.XmlParser.Helper.Serializer;
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

    public void PopulateContentPropertiesForXmlNode(XmlNode node)
    {
        if(node.ReferencedType == null) return;
        List<PropertyInfo> propertiesDefinedInTypeInNode = node.ReferencedType.GetProperties().ToList();
        foreach (PropertyInfo propertyInfo in propertiesDefinedInTypeInNode)
        {
            if (!TypeResolver.IsTypePrimitive(propertyInfo.PropertyType) && !propertyInfo.PropertyType.IsEnum)
            {
                if (TypeResolver.IsTypeCollection(propertyInfo.PropertyType))
                {
                    object? collectionContent = Activator.CreateInstance(propertyInfo.PropertyType);
                    if(collectionContent == null) 
                        throw new InvalidOperationException($"Collection type {propertyInfo.PropertyType.Name} could not be instantiated.");
                    MethodInfo? addMethod = collectionContent.GetType().GetMethod("Add");
                    if (addMethod == null)
                        throw new InvalidOperationException($"Collection type {collectionContent.GetType().Name} does not have Add().");
                    foreach (object? contentInstance in GetAllInstancesForCollectionContentProperty(node, propertyInfo))
                    {
                        addMethod.Invoke(collectionContent, new[] { contentInstance });
                    }
                    propertyInfo.SetValue(node.Value, collectionContent);
                    //propertyInfo.SetValue(node.Value,Activator.CreateInstance(propertyInfo.PropertyType));
                  /*  IEnumerable<object?> collectionContent = (IEnumerable<object?>)propertyInfo.GetValue(node.Value);
                    foreach (object? contentInstance in GetAllInstancesForCollectionContentProperty(node, propertyInfo))
                    {
                        collectionContent = collectionContent.Append(contentInstance);
                    }*/
                        
                }
                else
                {
                    object? contentInstance = GetInstanceForContentProperty(node, propertyInfo);
                    if(contentInstance != null)
                        propertyInfo.SetValue(node.Value,contentInstance);
                }
            }
        }
    }
    private List<object?> GetAllInstancesForCollectionContentProperty(XmlNode nodeWhereTheContentPropertyIsDefined, PropertyInfo propertyInfo)
    {
        List<object?> result = new List<object?>();
        if (nodeWhereTheContentPropertyIsDefined.Children.Exists(child => child.IsPropertyIdentifier))
        {
            // Find content Node by Property Identifier
            string propertyIdentifier = nodeWhereTheContentPropertyIsDefined.GetNameOfThisNodeInAXmlDocument() + "." + XmlNameEvaluator.GetXmlNameOrActualName(propertyInfo);
            XmlNode? propertyIdentifierNode = nodeWhereTheContentPropertyIsDefined.Children.Find(child =>
                child.IsPropertyIdentifier && child.NameOfParentClassAndProperty == propertyIdentifier);
            if (propertyIdentifierNode != null && propertyIdentifierNode.Children.Count > 1)
            {
                foreach (XmlNode child in propertyIdentifierNode.Children) result.Add(child.Value);
                return result;  
            }
            
        }
        foreach (XmlNode child in nodeWhereTheContentPropertyIsDefined.Children) result.Add(child.Value);
        return result; 
    }
    private object? GetInstanceForContentProperty(XmlNode nodeWhereTheContentPropertyIsDefined,
        PropertyInfo propertyInfo)
    {
        XmlNode? contentNode;
        if (nodeWhereTheContentPropertyIsDefined.Children.Exists(child => child.IsPropertyIdentifier))
        {
            // Find content Node by Property Identifier
            string propertyIdentifier = nodeWhereTheContentPropertyIsDefined.GetNameOfThisNodeInAXmlDocument() + "." + XmlNameEvaluator.GetXmlNameOrActualName(propertyInfo);
            XmlNode? propertyIdentifierNode = nodeWhereTheContentPropertyIsDefined.Children.Find(child =>
                child.IsPropertyIdentifier && child.NameOfParentClassAndProperty == propertyIdentifier);
            if(propertyIdentifierNode != null && propertyIdentifierNode.Children.Count == 1)
                return propertyIdentifierNode.Children.First().Value;
        }
        else if((contentNode = nodeWhereTheContentPropertyIsDefined.Children.Find(child => !child.IsPropertyIdentifier)) != null)
        {
            return contentNode.Value;
        }
        return null;
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