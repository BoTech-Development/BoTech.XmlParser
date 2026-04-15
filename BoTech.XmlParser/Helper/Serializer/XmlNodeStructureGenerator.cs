using System.Collections;
using System.Reflection;
using BoTech.XmlParser.Models;
using BoTech.XmlParser.Services;

namespace BoTech.XmlParser.Helper.Serializer;

public class XmlNodeStructureGenerator
{
    /// <summary>
    /// Creates the <see cref="XmlDocument"/> XmlNode structure.
    /// </summary>
    /// <param name="obj">Object instance for creating the structure.</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public XmlDocument GenerateXmlStructure<T>(T obj, Assembly callingAssembly)
    {
        TypeResolver.CreateInstance(callingAssembly);
        XmlDocument doc = new XmlDocument();
        XmlNode parentNode = XmlNode.CreateRootXmlNode();
        GenerateXmlNodeStructure(obj, parentNode);
        doc.Nodes.AddRange(parentNode.Children);
        TypeResolver.Clear();
        return doc;
    }

    /// <summary>
    /// Creates the XmlNode structure recursive.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="parentNode"></param>
    /// <param name="parentProperty">The parent property must be set to the property info that was considered last before the recursive call. So the property that references the current object.</param>
    private void GenerateXmlNodeStructure(object? obj, XmlNode parentNode, PropertyInfo? parentProperty = null)
    {
        if (obj == null) return;
        // Add the Xml Property Node if needed
        if (parentProperty != null)
        {
            XmlNode newPropertyIdentifier = XmlNode.CreatePropertyIdentifierXmlNode(parentProperty);
            parentNode.Children.Add(newPropertyIdentifier);
            parentNode = newPropertyIdentifier;
        }
        Type type = obj.GetType();
        XmlNode node = XmlNode.CreateXmlNodeFromTypeAndProperty(type, XmlNameEvaluator.GetXmlNameOrActualName(type), parentProperty);
        
        XmlProperty? genericTypeHint = GenerateAdditionalGenericTypeHints(obj);
        if(genericTypeHint != null)
            node.InternalSerializerProperties.Add(genericTypeHint);
        if(IsTheNamespaceDeclarationNeededForTheXml(type))
            node.InternalSerializerProperties.Add(new XmlProperty("_nsp", "_nsp", type.Namespace));
        
        GenerateXmlNodesForAllProperties(type, node, obj);
        parentNode.Children.Add(node);
    }

    /// <summary>
    /// It is necessary to declare the namespace when there are multiple Types with the same name defined.
    /// </summary>
    /// <param name="type"></param>
    /// <returns>True when the given type has the same name as another already analyzed Type.</returns>
    private bool IsTheNamespaceDeclarationNeededForTheXml(Type type) =>
        TypeResolver.Instance.GetAnotherTypeDefinedWithTheSameNameButIsNotGivenType(type) != null;
    /// <summary>
    /// Adds all properties / values that are declared / contained in the given type to the XmlNode tree.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="node"></param>
    /// <param name="obj"></param>
    private void GenerateXmlNodesForAllProperties(Type type, XmlNode node, object obj)
    {
        PropertyInfo[] properties = type.GetProperties();
        bool hasTypeMultipleObjectBasedPropertiesDefined = TypeResolver.HasTypeMultipleObjectBasedPropertiesDefined(type);
        foreach (PropertyInfo property in properties)
        {
            object? propertyValue = property.GetValue(obj);
            if(propertyValue == null) continue;
            if (TypeResolver.IsTypePrimitive(property.PropertyType))
            {
                AddPrimitiveXmlAttributeToNode(node, property, propertyValue);
            }
            else if (TypeResolver.IsParsable(property.PropertyType)) // When the object implements IParsable.
            {
                string? parsedObject = propertyValue.ToString();
                if(parsedObject != null)
                    AddPrimitiveXmlAttributeToNode(node, property, parsedObject);
            }
            else if (property.PropertyType.IsEnum)
            {
                AddPrimitiveXmlAttributeToNode(node, property, propertyValue);
            }
            else if (TypeResolver.IsTypeCollection(property.PropertyType))
            {
                AddCollectionContentToXmlNode(node, propertyValue, property, hasTypeMultipleObjectBasedPropertiesDefined);
            }
            else //if(property.PropertyType.IsGenericType || propertyValue.GetType().IsGenericType) // User defined generic type
            {
                if(hasTypeMultipleObjectBasedPropertiesDefined)
                    GenerateXmlNodeStructure(propertyValue, node, property);
                else
                    GenerateXmlNodeStructure(propertyValue, node, null);
            }
          
        }
    }

    private XmlProperty? GenerateAdditionalGenericTypeHints(object propertyValue)
    {
        if (!propertyValue.GetType().IsGenericType) return null;
        string genericTypesString = new GenericTypeParser().ParseGenericTypeInfoFromGenericType(propertyValue.GetType())
            .ParseToString();
        return new XmlProperty("", "_gt", genericTypesString);
        
       /* Type[] genericTypes =  propertyValue.GetType().GetGenericArguments();
        List<XmlProperty> additionalGenericPropertyHints = new();
        for(int i = 0; i < genericTypes.Length; i++) additionalGenericPropertyHints.Add(new XmlProperty(  "",$"_gt-{i}", "_fn:" + genericTypes[i].FullName + "_asm:" + genericTypes[i].Assembly.FullName));
        return additionalGenericPropertyHints;*/
    }
    /// <summary>
    /// Adds each object stored in the given collection to the XmlNode tree.
    /// </summary>
    /// <param name="parentNode"></param>
    /// <param name="property"></param>
    /// <param name="propertyValue"></param>
    /// <param name="doesNextNodeNeedPropertyIdentifier">True when all sub nodes should have the Property Identifier (for e.g. <BoForm.Imports>{SubNode goes here}</BoForm.Imports> declared.</param>
    /// <exception cref="NotSupportedException">When the collection type is not supported.</exception>
    private void AddCollectionContentToXmlNode(XmlNode parentNode, object propertyValue, PropertyInfo property, bool doesNextNodeNeedPropertyIdentifier)
    {
        Type[] interfaces = property.PropertyType.GetInterfaces();
        PropertyInfo? transferInfo = doesNextNodeNeedPropertyIdentifier ? property : null;
        if (interfaces.Contains(typeof(IEnumerable<>)) || interfaces.Contains(typeof(IEnumerable)))
        {
            foreach (object? item in (IEnumerable)propertyValue)
            {
                GenerateXmlNodeStructure(item, parentNode, transferInfo);
            }
        }
        else if (interfaces.Contains(typeof(ICollection<>))|| interfaces.Contains(typeof(ICollection)))
        {
            foreach (object? item in (ICollection)propertyValue)
            {
                GenerateXmlNodeStructure(item, parentNode, transferInfo);
            }
        }
        else if (interfaces.Contains(typeof(IList<>))|| interfaces.Contains(typeof(IList)))
        {
            foreach (object? item in (IList)propertyValue)
            {
                GenerateXmlNodeStructure(item, parentNode, transferInfo);
            }
        }
        else
        {
            throw new NotSupportedException($"The Collection type {property.PropertyType} is not supported. Please ensure to inherit at least from one of the following interfaces: System.Collections.Generic.IList<T>,  System.Collections.Generic.ICollection<T>, System.Collections.Generic.IEnumerable<>! ");
        }
    }
    /// <summary>
    /// Adds a new XmlProperty to the given Node.
    /// </summary>
    /// <param name="node">The Node</param>
    /// <param name="property">Will be used to determine the name of the property that should be displayed in xml.</param>
    /// <param name="propertyValue">The string value</param>
    private void AddPrimitiveXmlAttributeToNode(XmlNode node, PropertyInfo property, object propertyValue) => node.Properties.Add(new XmlProperty(property.PropertyType, property.Name, XmlNameEvaluator.GetXmlNameOrActualName(property),propertyValue.ToString()));
}