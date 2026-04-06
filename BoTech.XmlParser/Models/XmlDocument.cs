namespace BoTech.XmlParser.Models;

public class XmlDocument
{
    public List<XmlNode> Nodes { get; set; } = new();

    public string GenerateXmlString()
    {
        string result = "";
        foreach (XmlNode node in Nodes)
        {
            result += node.Serialize() + "\n";
        }
        return result;
    }
}