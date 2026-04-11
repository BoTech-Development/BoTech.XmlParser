using System.Reflection;
using BoTech.XmlParser.Exceptions;
using BoTech.XmlParser.Models;

namespace BoTech.XmlParser.Helper.Deserializer;

public class SemiParsedNodeTypeValidator
{
    public void CheckNodeTypeForEmptyConstructors(XmlNode node)
    {
        if(node.ReferencedType != null)
            if (HasTypeEmptyConstructor(node.ReferencedType) == false)
                throw new XmlMissingConstructorException(
                    $"The type: {node.ReferencedType.Name} has no empty constructor! To instantiate it by the deserializer, you need to add a parameterless constructor.");
    }
    private bool HasTypeEmptyConstructor(Type type) => type.GetConstructor(Type.EmptyTypes) != null;
    
}