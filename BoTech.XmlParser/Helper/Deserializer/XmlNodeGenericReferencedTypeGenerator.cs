using BoTech.XmlParser.Models;

namespace BoTech.XmlParser.Helper.Deserializer;

public class XmlNodeGenericReferencedTypeGenerator
{
    public Type? GenerateGenericReferencedTypeFromXmlNode(XmlNode node)
    {
        XmlProperty genericTypeXmlProperty;
        try
        {
            genericTypeXmlProperty = node.InternalSerializerProperties.First(property =>
                property.GetNameOfThisPropertyInAXmlDocument() == "_gt");
            
        }
        catch (Exception e)
        {
            Console.WriteLine("Info: No Generic Type Property found.");
        }
        return node.ReferencedType;
        
    }
}