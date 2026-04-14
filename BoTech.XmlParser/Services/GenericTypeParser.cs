using BoTech.XmlParser.Models.Deserializer;

namespace BoTech.XmlParser.Services;

public class GenericTypeParser
{
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

    public GenericTypeInfo ParseGenericTypeInfoFromGenericType(Type type)
    {
        return ParseGenericTypeInfoFromGenericTypeRecursive(type, 1, out _);
    }

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