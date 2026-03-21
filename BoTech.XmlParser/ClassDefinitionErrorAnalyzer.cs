using System.Collections;
using System.Reflection;
using BoTech.XmlParser.Attributes;
using BoTech.XmlParser.Error.ObjectDefinitions;

namespace BoTech.XmlParser;
[Obsolete]
public class ClassDefinitionErrorAnalyzer
{
    private List<Type> _analyzedTypes = new List<Type>();
    private Queue<PropertyInfo> _propertiesToAnalyze = new Queue<PropertyInfo>();
    
    /// <summary>
    /// Collect all errors for the checks that need to be carried out to verify whether they are serializable.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Dictionary<PropertyInfo, PropertyDefinitionError> CheckIfTypeAndAllSubTypesAreSerializableAndGetAllErrors(Type type)
    {
        _propertiesToAnalyze.Clear();
        BuildPropertiesToAnalyzeQueue(type,  null);
        //return CheckIfTypeAndAllSubTypesAreSerializableAndGetAllErrorsRecursive(type, new Dictionary<PropertyInfo, PropertyDefinitionError>());
        return new();
    }

    private void BuildPropertiesToAnalyzeQueue(Type type, object currentObject)
    {
        foreach (PropertyInfo propertyInfo in type.GetProperties().ToList())
        {
            // Add the current Property
            _propertiesToAnalyze.Enqueue(propertyInfo);
            // Lets add all other properties that are declared in the sub Types of the current Type.
            BuildPropertiesToAnalyzeQueueForPropertyType(propertyInfo.PropertyType);
            // When the Type of the Property is an interface or abstract class its is also necessary to check the actual implementation of this abstract class / interface.
            // But when the property is null nothing has to be done because, the serialized version of the property will not be included into the xml.
            object? propertyValue = propertyInfo.GetValue(currentObject);
            if(propertyValue != null)
                BuildPropertiesToAnalyzeQueueForPropertyType(propertyValue.GetType());
        } 
    }

    private void BuildPropertiesToAnalyzeQueueForPropertyType(Type type)
    {
        foreach (Type additionalTypeToAnalyze in GetGenericTypesOrTypeIgnoringIEnumerableTypes(type))
        {  
            if(_analyzedTypes.Contains(additionalTypeToAnalyze)) continue;
            _analyzedTypes.Add(additionalTypeToAnalyze);
            BuildPropertiesToAnalyzeQueue(additionalTypeToAnalyze, null);
        }
    }
    /*   /// <summary>
    /// Does the same as defined in the wrapper method: <see cref="CheckIfTypeAndAllSubTypesAreSerializableAndGetAllErrors"/>
    /// </summary>
    /// <param name="type"></param>
    /// <param name="collectedErrors"></param>
    /// <returns></returns>
    private Dictionary<PropertyInfo, PropertyDefinitionError>
        CheckIfTypeAndAllSubTypesAreSerializableAndGetAllErrorsRecursive(Type type,
            Dictionary<PropertyInfo, PropertyDefinitionError> collectedErrors)
    {
        
    }
    
    private List<Type> GetAllTypesToCheckFromAllProperties(List<PropertyInfo> properties)
    {
        List<Type> result = new List<Type>();
        foreach (PropertyInfo propertyInfo in properties)
        {
            result.AddRange(GetGenericTypesOrTypeIgnoringIEnumerableTypes(propertyInfo.PropertyType)); 
        }
        return result;
    }*/


    /// <summary>
    /// Creates a List which contains all Generic types, that not inherit from the IEnumerable Interface.
    /// </summary>
    /// <param name="propertyType">The Type of the Property in the class</param>
    /// <returns></returns>
    private List<Type> GetGenericTypesOrTypeIgnoringIEnumerableTypes(Type propertyType)
    {
        if(propertyType.IsPrimitive || propertyType.IsEnum || !propertyType.IsGenericType) return [propertyType];
        List<Type> types = new();
        if(!propertyType.GetInterfaces().Contains(typeof(IEnumerable)))
            types.Add(propertyType);
        // Evaluate all Generic types here:
        foreach (Type genericTypeArgument in propertyType.GetGenericArguments())
        {
            // When the Generic Type itself is a generic type too
            if (genericTypeArgument.IsGenericType)
                types.AddRange(
                    (genericTypeArgument));
            else
                types.Add(genericTypeArgument);
        }
        return types;
    }
  /*  /// <summary>
    /// Creates a List which contains all Generic types, that not inherit from the IEnumerable Interface.
    /// </summary>
    /// <param name="propertyType">The Type of the Property in the class</param>
    /// <returns></returns>
    private List<PropertyInfo> ListAllSubPropertiesThatNeedToCheckedByAProperty(PropertyInfo property)
    {
        Type propertyType = property.PropertyType;
        if(propertyType.IsPrimitive || propertyType.IsEnum || !propertyType.IsGenericType) return [property];
        List<PropertyInfo> propertiesToCheck = new();
        // When the property type is not a list, then it will be a custom generic object. So it is necessary to check all properties of this object.
        if(!propertyType.GetInterfaces().Contains(typeof(IEnumerable)))
            propertiesToCheck.Add(property); // add to the list of properties that still need to be checked.
        // Evaluate all Generic types here:
        foreach (Type genericTypeArgument in propertyType.GetGenericArguments())
        {
            // When the Generic Type itself is a generic type too
            if (genericTypeArgument.IsGenericType)
                types.AddRange(GetGenericsOrTypeIgnoringIEnumerableTypes(genericTypeArgument));
            else
                types.Add(genericTypeArgument);
        }
        return types;
    }*/
    
    
    /// <summary>
    /// Filters out the properties from the list that are not primitive
    /// </summary>
    /// <param name="properties">List to filter</param>
    /// <returns>The filtered List.</returns>
    private List<PropertyInfo> FilterAllPropertiesThatWillBeXmlContentProperties(List<PropertyInfo> properties)
    {
        return properties.Where(p => !(p.PropertyType.IsPrimitive || p.PropertyType == typeof(string))).ToList();
    }
    /// <summary>
    /// Lists all properties that are passed, but are not assigned to any attribute from the given list.
    /// </summary>
    /// <param name="properties">The given properties</param>
    /// <param name="withoutXmlAttributes">Filter.</param>
    /// <returns>A List of properties that are not marked with any attribute out of the <see cref="withoutXmlAttributes"/> list.</returns>
    private List<PropertyInfo> GetPropertiesWithoutXmlAttributesFromType(List<PropertyInfo> properties, List<Type> withoutXmlAttributes)
    {
        return properties.Where(p => p.CustomAttributes.Any(attribute => withoutXmlAttributes.Contains(attribute.GetType()) == false) == false).ToList();
    }
    //--------------------------------------------------------------------------------------------------------------------------------------------------------
    
    
    /// <summary>
    /// Does the same as defined in the wrapper method: <see cref="CheckIfTypeAndAllSubTypesAreSerializableAndGetAllErrors"/>
    /// </summary>
    /// <param name="type"></param>
    /// <param name="collectedErrors"></param>
    /// <returns></returns>
  /*  private Dictionary<PropertyInfo, PropertyDefinitionError>
        CheckIfTypeAndAllSubTypesAreSerializableAndGetAllErrorsRecursive(Type type,
            Dictionary<PropertyInfo, PropertyDefinitionError> collectedErrors)
    {
        List<PropertyInfo> properties =  type.GetProperties().ToList();
        collectedErrors = CheckIfXmlContentIsNecessary(properties, collectedErrors);
        foreach (PropertyInfo propertyInfo in properties)
        {
            collectedErrors =
                CheckIfTypeAndAllSubTypesAreSerializableAndGetAllErrorsRecursive(propertyInfo.PropertyType,
                    collectedErrors);
        }
        return collectedErrors;
    }

    private List<Type> GetAllTypesToCheckFromAllProperties(List<PropertyInfo> properties)
    {
        List<Type> result = new List<Type>();
        foreach (PropertyInfo propertyInfo in properties)
        {
            result.AddRange(GetGenericsOrTypeIgnoringIEnumerableTypes(propertyInfo.PropertyType)); 
        }
        return result;
    }
    /// <summary>
    /// Creates a List which contains all Generic types, that not inherit from the IEnumerable Interface.
    /// </summary>
    /// <param name="propertyType">The Type of the Property in the class</param>
    /// <returns></returns>
    private List<Type> GetGenericsOrTypeIgnoringIEnumerableTypes(Type propertyType)
    {
        if(propertyType.IsPrimitive || propertyType.IsEnum || !propertyType.IsGenericType) return [propertyType];
        List<Type> types = new();
        if(!propertyType.GetInterfaces().Contains(typeof(IEnumerable)))
            types.Add(propertyType);
        // Evaluate all Generic types here:
        foreach (Type genericTypeArgument in propertyType.GetGenericArguments())
        {
            // When the Generic Type itself is a generic type too
            if (genericTypeArgument.IsGenericType)
                types.AddRange(GetGenericsOrTypeIgnoringIEnumerableTypes(genericTypeArgument));
            else
                types.Add(genericTypeArgument);
        }
        return types;
    }
    /// <summary>
    /// Checks if any Property of the given Property List needs the XmlContent Attribute. When the Attribute is not already defined an Error will append to the Error list.
    /// </summary>
    /// <param name="propertiesDefinedInTheType">Defined Properties.</param>
    /// <param name="result">Error List</param>
    /// <returns>New Error List.</returns>
    private Dictionary<PropertyInfo, PropertyDefinitionError> CheckIfXmlContentIsNecessary(List<PropertyInfo> propertiesDefinedInTheType,
        Dictionary<PropertyInfo, PropertyDefinitionError> result)
    {
        List<PropertyInfo> filteredXmlContentProperties = FilterAllPropertiesThatWillBeXmlContentProperties(propertiesDefinedInTheType);
        List<PropertyInfo> filteredPropertiesWithoutXmlAttributes = GetPropertiesWithoutXmlAttributesFromType(
            propertiesDefinedInTheType, new List<Type>()
            {
                typeof(XmlContent)
            });
        foreach (PropertyInfo property in filteredPropertiesWithoutXmlAttributes)
        {
            if (filteredXmlContentProperties.Contains(property))
            {
                result.Add(property, new PropertyDefinitionError()
                    {
                        ErrorMessage = $"The XmlContent Attribute is missing for the Property:  {property.Name}, defined in the Type: {property.DeclaringType?.FullName}",
                        Error = PropertyDefinitionErrors.PropertyIsMissingXmlContentProperty
                    });
            }
        }
        return result;
    }
    /// <summary>
    /// Filters out the properties from the list that are not primitive
    /// </summary>
    /// <param name="properties">List to filter</param>
    /// <returns>The filtered List.</returns>
    private List<PropertyInfo> FilterAllPropertiesThatWillBeXmlContentProperties(List<PropertyInfo> properties)
    {
        return properties.Where(p => !(p.PropertyType.IsPrimitive || p.PropertyType == typeof(string))).ToList();
    }
    /// <summary>
    /// Lists all properties that are passed, but are not assigned to any attribute from the given list.
    /// </summary>
    /// <param name="properties">The given properties</param>
    /// <param name="withoutXmlAttributes">Filter.</param>
    /// <returns>A List of properties that are not marked with any attribute out of the <see cref="withoutXmlAttributes"/> list.</returns>
    private List<PropertyInfo> GetPropertiesWithoutXmlAttributesFromType(List<PropertyInfo> properties, List<Type> withoutXmlAttributes)
    {
        return properties.Where(p => p.CustomAttributes.Any(attribute => withoutXmlAttributes.Contains(attribute.GetType()) == false) == false).ToList();
    }*/
    
    
}