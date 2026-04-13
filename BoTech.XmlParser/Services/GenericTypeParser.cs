using System.Reflection;

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

public class GenericTypeInfo
{
    /// <summary>
    /// A unique ID for this GenericType.
    /// </summary>
    public int ThisId { get; set; }
    /// <summary>
    /// The ID of the node that this GenericType where this GenericType is stored in the <see cref="SubGenericTypes"/> list.    
    /// </summary>
    public int AssignedToId { get; set; }
    /// <summary>
    /// When the User defines a generic type in the xml string, the Type argument is set here. <br/>
    /// For example: User defined type: <c>List&lt;string&gt;</c>, This Property is set to <c>typeof(string)</c>
    /// </summary>
    public Type? Type { get; set; } = typeof(object);
    /// <summary>
    /// When the User defines a generic type with a depth: For example: <c>List&lt;List&lt;string&gt;&gt;</c>, the sub generic types are set here.
    /// </summary>
    public List<GenericTypeInfo> SubGenericTypes = new List<GenericTypeInfo>();
    /// <summary>
    /// String will contain the following properties: <see cref="ThisId"/>, <see cref="AssignedToId"/>, <see cref="Type"/>, <see cref="SubGenericTypes"/> <br/>
    /// Parsed in the following format:<br/>
    /// (_tId:{ThisId};_aTId:{AssignedToId};_tn:{Type.Name};_nsp:{Type.Namespace})&amp;...All SubGenericTypes...
    /// </summary>
    /// <returns></returns>
    public string ParseToString()
    {
        string thisAsString = $"(_tId:{ThisId};_aTId:{AssignedToId};_tn:{Type.Name};_nsp:{Type.Namespace})";
        foreach (GenericTypeInfo gti in SubGenericTypes)
        {
            thisAsString += "&" + gti.ParseToString();
        }
        return thisAsString;
    }

    public bool AddSubGenericTypeByAssignedId(GenericTypeInfo subGenericType)
    {
        if (subGenericType.AssignedToId == -1 || ThisId == subGenericType.AssignedToId)
        {
            SubGenericTypes.Add(subGenericType);
            return true;
        }
        else
        {
          
            foreach (GenericTypeInfo gti in SubGenericTypes)
            {
                if (gti.AddSubGenericTypeByAssignedId(subGenericType)) return true;
            }
            return false;
        }
    }
}