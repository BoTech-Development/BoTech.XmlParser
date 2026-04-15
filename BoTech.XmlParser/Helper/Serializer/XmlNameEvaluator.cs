using System.Reflection;
using BoTech.XmlParser.Attributes;
using BoTech.XmlParser.Services;

namespace BoTech.XmlParser.Helper.Serializer;

public static class XmlNameEvaluator
{
    /// <summary>
    /// Tries to create a name for the XmlTag. At the same time this method checks if the XmlName (if exists) is correctly defined.
    /// </summary>
    /// <param name="type">The type for which the name is to be created.</param>
    /// <returns>The XmlName or the actual Name of the given Type.</returns>
    /// <exception cref="ArgumentException">Will be thrown when the definition is not correct. Please catch this Exception not in this library.</exception>
    public static string GetXmlNameOrActualName(Type type)
    {
        XmlName nameDefinition = GetXmlNameFromMemberInfoOrPredefinedName(type, type.Name);
        Type? otherTypeWithTheSameXmlName = null;
        if((otherTypeWithTheSameXmlName = TypeResolver.Instance.HasAnotherTypeTheSameXmlName(nameDefinition, type)) != null)
            throw new ArgumentException(
                $"It is not allowed to describe different Classes with the same XmlName: The Name: {nameDefinition.Name}. First Class: {type.FullName}. Second Class: {otherTypeWithTheSameXmlName.FullName}.");
        return nameDefinition.Name;
    }
    /// <summary>
    /// Tries to create a name for the Property XmlTag. At the same time this method checks if the XmlName (if exists) is correctly defined.
    /// </summary>
    /// <param name="propertyInfo">The property for which the name is to be created.</param>
    /// <returns>The XmlName or the actual Name of the Property.</returns>
    /// <exception cref="ArgumentException">Will be thrown when the definition is not correct. Please catch this Exception not in this library.</exception>
    public static string GetXmlNameOrActualName(PropertyInfo propertyInfo)
    {
        XmlName nameDefinition = GetXmlNameFromMemberInfoOrPredefinedName(propertyInfo, propertyInfo.Name);
        PropertyInfo? otherPropertyInfoWithTheSameXmlName = GetAnotherPropertyDefinedInTheTypeWithTheSameXmlName(nameDefinition, propertyInfo, propertyInfo.DeclaringType);
        if(otherPropertyInfoWithTheSameXmlName != null)
            throw new ArgumentException(
                $"It is not allowed to describe different Properties with the same XmlName: The Name: {nameDefinition.Name}. First Member: {propertyInfo}. Second Member: {otherPropertyInfoWithTheSameXmlName}, In the class: {propertyInfo.DeclaringType.FullName}.");
        return nameDefinition.Name;
    }
    private static XmlName GetXmlNameFromMemberInfoOrPredefinedName(MemberInfo info, string predefinedActualName)
    {
        XmlName? nameDefinition = TryToGetXmlNameFromMemberInfo(info);
        if(nameDefinition == null) nameDefinition = new XmlName(predefinedActualName);
        return nameDefinition;
    }

    private static PropertyInfo? GetAnotherPropertyDefinedInTheTypeWithTheSameXmlName(XmlName currentXmlName,
        PropertyInfo currentProperty, Type currentType)
    {
        foreach (PropertyInfo propertyInfo in currentType.GetProperties())
        {
            if(propertyInfo.Name == currentProperty.Name) continue;
            XmlName? nameDefinition = TryToGetXmlNameFromMemberInfo(propertyInfo);
            if(nameDefinition != null && nameDefinition.Name == currentXmlName.Name) return propertyInfo;
        }
        return null;
    }
    /// <summary>
    /// This Method tries to find the XmlName Attribute for a specific member.
    /// </summary>
    /// <param name="info"></param>
    /// <returns></returns>
    public static XmlName? GetXmlNameOrNullFromMemberInfo(MemberInfo info)
    {
        return TryToGetXmlNameFromMemberInfo(info);
    }
    /// <summary>
    /// This Method tries to find the XmlName Attribute for a specific member.
    /// </summary>
    /// <param name="info">The Member</param>
    /// <returns>The XmlName.</returns>
    /// <exception cref="InvalidOperationException">Will be thrown when a XmlName is not defined for the given Member.</exception>
    public static XmlName? TryToGetXmlNameFromMemberInfo(MemberInfo info)
    {
        CustomAttributeData? data = null;
        foreach (CustomAttributeData customAttributeData in info.CustomAttributes)
        {
            if(customAttributeData.AttributeType == typeof(XmlName)) data = customAttributeData;
        }
        if (data == null) return null; //throw new InvalidOperationException($"The XmlName Attribute is not defined for the given Member: {info.Name}.");
        object? value = data.ConstructorArguments[0].Value;
        if(value != null)
            return new XmlName(value.ToString());
        return null;
        //throw new InvalidOperationException($"The XmlName Attribute is not defined for the given Member: {info.Name}.");
    }

}