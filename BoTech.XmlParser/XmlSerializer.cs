using System.Reflection;
using BoTech.XmlParser.Helper.Serializer;
using BoTech.XmlParser.Models;

namespace BoTech.XmlParser;

public class XmlSerializer
{
    /// <summary>
    /// Serialize any object to an XML string.
    /// </summary>
    /// <param name="obj">The object (no abstract class).</param>
    /// <typeparam name="T">The generic Type.</typeparam>
    /// <returns>A Xml string.</returns>
    public string Serialize<T>(T obj)
    {
        XmlDocument document = new XmlNodeStructureGenerator().GenerateXmlStructure(obj, Assembly.GetCallingAssembly());
        string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";
        xml += document.GenerateXmlString();
        return xml;
    }
}