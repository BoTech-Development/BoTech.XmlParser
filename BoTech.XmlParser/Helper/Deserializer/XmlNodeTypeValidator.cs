using System.Reflection;
using BoTech.XmlParser.Exceptions;
using BoTech.XmlParser.Models;

namespace BoTech.XmlParser.Helper.Deserializer;

public class XmlNodeTypeValidator
{
    /// <summary>
    /// Validates if the given XmlNode's referenced type contains a parameterless constructor.
    /// Throws an exception if the referenced type does not have an empty constructor.
    /// </summary>
    /// <param name="node">The XmlNode to validate for a parameterless constructor in its referenced type.</param>
    /// <exception cref="XmlMissingConstructorException">
    /// Thrown when the type referenced by the XmlNode does not include a parameterless (empty) constructor, which is
    /// required for the deserialization process to instantiate the type.
    /// </exception>
    public void CheckNodeTypeForEmptyConstructors(XmlNode node)
    {
        if (node.ReferencedType != null)
            if (HasTypeEmptyConstructor(node.ReferencedType) == false)
                throw new XmlMissingConstructorException(
                    $"The type: {node.ReferencedType.Name} has no empty constructor! To instantiate it by the deserializer, you need to add a parameterless constructor.");
    }
    private bool HasTypeEmptyConstructor(Type type) => type.GetConstructor(Type.EmptyTypes) != null;
    
}