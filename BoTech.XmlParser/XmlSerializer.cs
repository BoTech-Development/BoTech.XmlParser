using BoTech.XmlParser.Models;

namespace BoTech.XmlParser;

public class XmlSerializer
{
    public string Serialize<T>(T obj)
    {
        XmlDocument document = new XmlNodeStructureGenerator().GenerateXmlStructure(obj);
        string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";
        xml += document.GenerateXmlString();
        return xml;
    }
}