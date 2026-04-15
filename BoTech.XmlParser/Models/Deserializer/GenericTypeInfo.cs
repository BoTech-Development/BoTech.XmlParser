namespace BoTech.XmlParser.Models.Deserializer;


public class GenericTypeInfo(int thisId, int assignedToId, Type type)
{
    /// <summary>
    /// A unique ID for this GenericType.
    /// </summary>
    public int ThisId { get; init; } = thisId;
    /// <summary>
    /// The ID of the node that this GenericType where this GenericType is stored in the <see cref="SubGenericTypes"/> list.    
    /// </summary>
    public int AssignedToId { get; init; } = assignedToId;
    /// <summary>
    /// When the User defines a generic type in the xml string, the Type argument is set here. <br/>
    /// For example: User defined type: <c>List&lt;string&gt;</c>, This Property is set to <c>typeof(string)</c>
    /// </summary>
    public Type Type { get; init; } = type;
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
            thisAsString += "," + gti.ParseToString();
        }
        return thisAsString;
    }
    /// <summary>
    /// Adds a sub generic type by using the TypeId and the AssignedToId.
    /// </summary>
    /// <param name="subGenericType">The sub node to add.</param>
    /// <returns>True when it works.</returns>
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
    /// <summary>
    /// When this node is the most parent node in the tree, this method returns true.
    /// </summary>
    /// <returns></returns>
    public bool IsThisNodeMostParent() => ThisId == 1 && AssignedToId == -1;
    /// <summary>
    /// Applies the node structure to the <see cref="genericType"/> argument and returns the new generic <see cref="Type"/>.
    /// </summary>
    /// <param name="genericType">The Type that has no generic type arguments defined.</param>
    /// <returns>The new Type which has all generic arguments defined.</returns>
    /// <exception cref="InvalidOperationException">Occurs when you try to invoke this method on a sub node.</exception>
    public Type InjectGenericTypeArgumentsFromTreeInGenericType(Type genericType)
    {
        if (IsThisNodeMostParent())
        {
            Type[] genericTypeArguments = new Type[SubGenericTypes.Count];
            for (int i = 0; i < SubGenericTypes.Count; i++)
            {
                genericTypeArguments[i] = SubGenericTypes[i].MakeGenericTypeArgumentFromSubNodesOrReturnThisType();
            }
            return genericType.MakeGenericType(genericTypeArguments);
            //return genericType.MakeGenericType(MakeGenericTypeArgumentFromSubNodesOrReturnThisType();
        }
        throw new InvalidOperationException("You can only call this method on the root node.");
    }
    /// <summary>
    /// Internal method that makes the generic type argument from the sub nodes.
    /// </summary>
    /// <returns>
    /// The new Type
    /// </returns>
    private Type MakeGenericTypeArgumentFromSubNodesOrReturnThisType()
    {
        if (SubGenericTypes.Count > 0)
        {
            List<Type> genericTypeParamsForThisType = new();
            foreach (GenericTypeInfo gti in SubGenericTypes)
                genericTypeParamsForThisType.AddRange(gti.MakeGenericTypeArgumentFromSubNodesOrReturnThisType());
            return Type.MakeGenericType(genericTypeParamsForThisType.ToArray());
        }
        return Type;
    }
}