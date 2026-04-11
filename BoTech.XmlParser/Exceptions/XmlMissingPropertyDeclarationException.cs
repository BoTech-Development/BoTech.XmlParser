namespace BoTech.XmlParser.Exceptions;

public class XmlMissingPropertyDeclarationException : Exception
{
    public XmlMissingPropertyDeclarationException()
    {
        
    }
    public XmlMissingPropertyDeclarationException(string message) : base(message)
    {
        
    }
    public XmlMissingPropertyDeclarationException(string message, Exception inner) : base(message, inner)
    {
        
    }
}