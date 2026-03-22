using BoTech.XmlParser.Models;

namespace BoTech.XmlParser;

public class XmlSerializer
{
    public string Serialize<T>(T obj)
    {
        XmlDocument document = new XmlNodeStructureGenerator().GenerateXmlStructure(obj);
        return document.GenerateXmlString();
    }
}