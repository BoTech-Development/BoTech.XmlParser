using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;
using BoTech.XmlParser.Attributes;
using BoTech.XmlParser.Error.ObjectDefinitions;
using BoTech.XmlParser.Models;

namespace BoTech.XmlParser;

public class XmlSerializer
{
    private readonly List<Type> _propertyAttributeTypes = new List<Type>() { typeof(XmlContent) };
    public string Serialize<T>(T obj)
    {
        XmlDocument document = new XmlNodeStructureGenerator().GenerateXmlStructure(obj);
        return document.GenerateXmlString();
    }
}