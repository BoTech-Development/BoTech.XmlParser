using BoTech.XmlParser.Models;

namespace BoTech.XmlParser.Helper.Deserializer;

public class XmlNodeTypeActivator
{
    public object? CreateInstanceForXmlNode(XmlNode node)
    {
        if(node.ReferencedType != null) 
            return Activator.CreateInstance(node.ReferencedType);
        else
            return null;
    }
}