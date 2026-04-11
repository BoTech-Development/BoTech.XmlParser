namespace BoTech.XmlParser.Exceptions;

public class XmlMissingConstructorException : Exception
{
    public XmlMissingConstructorException()
    {
        
    }
    public XmlMissingConstructorException(string message) : base(message)
    {
        
    }

    public XmlMissingConstructorException(string message, Exception inner) : base(message, inner)
    {
        
    }
}