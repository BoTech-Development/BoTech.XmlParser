using BoTech.XmlParser.Models.Deserializer;

namespace BoTech.XmlParser.Services;

public class GenericTypeParser
{
    public GenericTypeInfo ParseGenericTypeFromXmlString(string xmlString)
    {
        GenericTypeInfo mainNode = new GenericTypeInfo();
        int currentDepth = 0;
        string[] genericTypeDefinitions = xmlString.Split("&");
        foreach (string genericTypeDefinition in genericTypeDefinitions)
        {
            Dictionary<string, string> parameters =
                GetParametersFromParameterString(genericTypeDefinition.Replace("(", "").Replace(")", ""));
            if (parameters.ContainsKey("_tId") && parameters.ContainsKey("_aTId") &&
                parameters.ContainsKey("_tn") && parameters.ContainsKey("_nsp"))
            {
                GenericTypeInfo newInfo = new GenericTypeInfo();
                newInfo.ThisId = int.Parse(parameters["_tId"]);
                newInfo.AssignedToId = int.Parse(parameters["_aTId"]);
                newInfo.Type = TypeResolver.Instance.GetTypeByNameFromReferencedAssemblies(parameters["_nsp"] + "." + parameters["_tn"]);
                mainNode.AddSubGenericTypeByAssignedId(newInfo);
            }
        }
        return mainNode.SubGenericTypes[0];
    }

    public GenericTypeInfo ParseGenericTypeInfoFromGenericType(Type type)
    {
        return ParseGenericTypeInfoFromGenericTypeRecursive(type, 0, out _);
    }

    private GenericTypeInfo ParseGenericTypeInfoFromGenericTypeRecursive(Type type, int currentId, out int nextId, GenericTypeInfo? parentInfo = null)
    {
        GenericTypeInfo currentGenericType = new GenericTypeInfo()
        {
            Type = type,
            ThisId = -1,
            AssignedToId = -1
        };
        if(parentInfo != null)
        {
            currentId++;
            nextId = currentId;
            currentGenericType.ThisId = currentId;
            currentGenericType.AssignedToId = parentInfo.ThisId;
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