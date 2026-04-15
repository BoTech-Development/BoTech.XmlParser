using BoTech.XmlParser.Models.Deserializer;

namespace BoTech.XmlParser.Services;
/// <summary>
/// This class can be used to desreliase or serialize the generic type definition in the xml document. <code>_gt=</code>  
/// </summary>
public class GenericTypeParser
{
    /// <summary>
    /// Parses a string containing serialized generic type information in XML format
    /// and constructs a tree of <see cref="GenericTypeInfo"/> objects representing the generic type structure.
    /// </summary>
    /// <param name="xmlString">
    /// A string in a specific serialized format, where each segment defines a generic type and its metadata,
    /// including type IDs, assigned IDs, types.
    /// </param>
    /// <returns>
    /// A root <see cref="GenericTypeInfo"/> object that represents the generic type structure,
    /// including its nested generic types, if any.
    /// </returns>
    public GenericTypeInfo ParseGenericTypeFromXmlString(string xmlString)
    {
        GenericTypeInfo? mainNode = null; // new GenericTypeInfo(-1, -1, null);
        string[] genericTypeDefinitions = xmlString.Split(",");
        foreach (string genericTypeDefinition in genericTypeDefinitions)
        {
            Dictionary<string, string> parameters =
                GetParametersFromParameterString(genericTypeDefinition.Replace("(", "").Replace(")", ""));
            if (parameters.ContainsKey("_tId") && parameters.ContainsKey("_aTId") &&
                parameters.ContainsKey("_tn") && parameters.ContainsKey("_nsp"))
            {
                GenericTypeInfo newInfo = new GenericTypeInfo(
                    int.Parse(parameters["_tId"]),
                    int.Parse(parameters["_aTId"]),
                    TypeResolver.Instance.GetTypeByNameFromReferencedAssemblies(parameters["_nsp"] + "." + parameters["_tn"]));
                if (mainNode == null) 
                    mainNode = newInfo;
                else
                    mainNode.AddSubGenericTypeByAssignedId(newInfo);
            }
        }
        return mainNode;
    }
    /// <summary>
    /// Takes a the type which contains generic type arguments, and creates the <see cref="GenericTypeInfo"/> tree.
    /// </summary>
    /// <param name="type">The generic type</param>
    /// <returns>The tree, which can be serialized.</returns>
    public GenericTypeInfo ParseGenericTypeInfoFromGenericType(Type type)
    {
        return ParseGenericTypeInfoFromGenericTypeRecursive(type, 1, out _);
    }
    /// <summary>
    /// Does the same as <see cref="ParseGenericTypeInfoFromGenericType"/>, but it outputs the next id of the next children.
    /// </summary>
    /// <param name="type">The current generic type or the root type.</param>
    /// <param name="currentId">The id of the created node.</param>
    /// <param name="nextId">The id of the children node.</param>
    /// <param name="parentInfo"></param>
    /// <returns></returns>
    private GenericTypeInfo ParseGenericTypeInfoFromGenericTypeRecursive(Type type, int currentId, out int nextId, GenericTypeInfo? parentInfo = null)
    {
        GenericTypeInfo currentGenericType; 
        if(parentInfo != null) // Its a sub node
        {
            currentId++;
            nextId = currentId;
            currentGenericType = new GenericTypeInfo(currentId, parentInfo.ThisId, type);
        }
        else // it's the root node
        {
            currentGenericType = new GenericTypeInfo(1, -1, type);
        }
        foreach (Type genericType in type.GetGenericArguments())
        {
            currentGenericType.SubGenericTypes.Add(ParseGenericTypeInfoFromGenericTypeRecursive(genericType, currentId, out nextId, currentGenericType));
            currentId = nextId;
        }
        nextId = 0;
        return currentGenericType;
    }
    /// <summary>
    /// Parses the parameter string into a dictionary of key-value pairs.
    /// The parameter string is expected to be a semicolon-separated list of key-value pairs
    /// where the key and value are separated by a colon.
    /// </summary>
    /// <param name="parameterString">The input parameter string to parse.</param>
    /// <returns>A dictionary containing the parsed key-value pairs.</returns>
    private Dictionary<string, string> GetParametersFromParameterString(string parameterString)
    {
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        string[] keyValuePairs = parameterString.Split(";");
        foreach (string keyValuePair in keyValuePairs)
        {
            string[] keyAndValue = keyValuePair.Split(":");
            parameters.Add(keyAndValue[0], keyAndValue[1]);
        }
        return parameters;
    }
}