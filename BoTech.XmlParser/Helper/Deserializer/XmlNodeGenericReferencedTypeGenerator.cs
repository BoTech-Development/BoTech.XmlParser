using BoTech.XmlParser.Models;
using BoTech.XmlParser.Models.Deserializer;
using BoTech.XmlParser.Services;

namespace BoTech.XmlParser.Helper.Deserializer;

public class XmlNodeGenericReferencedTypeGenerator
{
    public Type? GenerateGenericReferencedTypeFromXmlNode(XmlNode node)
    {
        XmlProperty? genericTypeXmlProperty = null;
        foreach (XmlProperty property in node.InternalSerializerProperties)
        {
            if (property.GetNameOfThisPropertyInAXmlDocument() == "_gt")
            {
                genericTypeXmlProperty = property;
            }
        }
        if (genericTypeXmlProperty == null || genericTypeXmlProperty.Value == null) return node.ReferencedType;
        GenericTypeParser parser = new GenericTypeParser(); 
        GenericTypeInfo info = parser.ParseGenericTypeFromXmlString(genericTypeXmlProperty.Value);
        return info.InjectGenericTypeArgumentsFromTreeInGenericType(node.ReferencedType!);
    }
}