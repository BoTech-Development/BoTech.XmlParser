namespace BoTech.XmlParser.Models.Deserializer;


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