using System.Reflection;
using BoTech.XmlParser.Attributes;

namespace BoTech.XmlParser;

public class XmlNameEvaluator
{
    private static readonly XmlNameEvaluator _instance = new();
    public static XmlNameEvaluator Instance => _instance;
    private  XmlNameEvaluator() {}

    private Dictionary<string, Type> _evaluatedXmlNamesInCombinationWithTheirTypes = new Dictionary<string, Type>();
    
    public void Clear() =>  _evaluatedXmlNamesInCombinationWithTheirTypes.Clear();
    
        
    public string GetXmlNameOrActualName(Type type)
    {
        XmlName nameDefinition;
        try
        {
            // First function throws exception when not found.
            nameDefinition = (XmlName)type.GetCustomAttributes()
                .First(atrribute => atrribute.GetType() == typeof(XmlName));
        }
        catch (Exception e)
        {
            return type.Name;
        }

        if (_evaluatedXmlNamesInCombinationWithTheirTypes.ContainsKey(nameDefinition.Name) &&
            !_evaluatedXmlNamesInCombinationWithTheirTypes[nameDefinition.Name].Equals(type))
            throw new ArgumentException(
                $"It is not allowed to describe different types with the same XmlName: The Name: {nameDefinition.Name}. FirstType: {_evaluatedXmlNamesInCombinationWithTheirTypes[nameDefinition.Name].FullName}. SecondType: {type.FullName}:");
        _evaluatedXmlNamesInCombinationWithTheirTypes.TryAdd(nameDefinition.Name, type);
        return nameDefinition.Name;
    }
    public string GetXmlNameOrActualName(PropertyInfo propertyInfo) => GetXmlNameOrActualName(propertyInfo.PropertyType);

}