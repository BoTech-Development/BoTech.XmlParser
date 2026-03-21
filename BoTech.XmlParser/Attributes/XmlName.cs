namespace BoTech.XmlParser.Attributes;
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public class XmlName : Attribute
{   
    public readonly string Name;
    public XmlName(string theNameInTheXmlDocumentOfThisMember)
    {
        Name = theNameInTheXmlDocumentOfThisMember;
    }
}