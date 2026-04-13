using System.Reflection;
using BoTech.XmlParser.Attributes;
using BoTech.XmlParser.Helper.Serializer;

namespace BoTech.XmlParser.Services;

public class TypeResolver
{
    private static TypeResolver? _instance = null;
    public static TypeResolver Instance
    {
        get
        {
            if (_instance == null) throw  new InvalidOperationException("This instance has not been initialized.");
            return _instance;
        }
    }
    private readonly List<Type> _instantiableTypes;
    private readonly List<Assembly> _referencedAssemblies;
    private TypeResolver(Assembly callingAssembly)
    {
        _referencedAssemblies = GetReferencedAssembliesFromAssembly(callingAssembly);
        _referencedAssemblies.Add(callingAssembly);
        _instantiableTypes = GetAllInstantiableTypesFromAssemblyIncludingReferencedAssemblies(_referencedAssemblies);
    }
    /// <summary>
    /// Creates a new instance of the singleton
    /// </summary>
    /// <param name="callingAssembly">The Assembly that called this Serialisation or Deserialisation method.</param>
    public static void CreateInstance(Assembly callingAssembly){ _instance = new TypeResolver(callingAssembly); }
    /// <summary>
    /// Resets the singleton (deletes the current Instance). Please use this method for releasing Memory.
    /// </summary>
    public static void Clear()
    {
        _instance = null;
    }

    public Type? GetTypeByNameFromReferencedAssemblies(string fullName)
    {
        Type? result = null;
        foreach (Assembly referencedAssembly in _referencedAssemblies)
        {
             if((result = referencedAssembly.GetType(fullName)) != null) return result;
        }
        return null;
    }
    /// <summary>
    /// This Method check if another type in the calling Assembly or a referenced Assembly has the same XmlName, as the given Type.
    /// </summary>
    /// <param name="xmlName">The name of the given Type.</param>
    /// <param name="currentType"></param>
    /// <returns>The type which has the same name or null.</returns>
    public Type? HasAnotherTypeTheSameXmlName(XmlName xmlName, MemberInfo currentType)
    {
        foreach (Type instantiableType in _instantiableTypes)
            if (instantiableType.Name != currentType.Name)
            {
                try
                {
                    XmlName name = XmlNameEvaluator.TryToGetXmlNameFromMemberInfo(instantiableType);
                    if (name.Name == xmlName.Name) return instantiableType;
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
                }
            }
        return null;
    }
    /// <summary>
    /// This Method tries to find another type which has the same Name. That means there a two classes declared in a referenced Assembly or in the calling assembly which have the same name.
    /// </summary>
    /// <param name="notThisTypeButTheSameName"></param>
    /// <returns>The type with the same name or null.</returns>
    public Type? GetAnotherTypeDefinedWithTheSameNameButIsNotGivenType(Type notThisTypeButTheSameName)
    {
        string typeName = notThisTypeButTheSameName.Name;
        foreach (var type in _instantiableTypes)
            if (type.Name == typeName && type.Namespace != notThisTypeButTheSameName.Namespace)
                return type;
        return null;
    }

    public Type TryToGetTypeByNameInXmlDocument(string nameInXmlDocument, string namespaceInXmlDocument)
    {
        foreach (var type in _instantiableTypes)
        {
            if (HasTypeTheSameXmlDocumentName(type, nameInXmlDocument))
            {
                if(HasTypeTheSameNamespace(type, namespaceInXmlDocument)) 
                    return type;
            }
        }
        throw new ArgumentException($"The Type with the name: {nameInXmlDocument} and the namespace: {namespaceInXmlDocument} was not found.");
    }

    private bool HasTypeTheSameNamespace(Type type, string namespaceInXmlDocument)
    {
        if(namespaceInXmlDocument == "") return true;
        if(type.Namespace == namespaceInXmlDocument) return true;
        return false;
    }
    private bool HasTypeTheSameXmlDocumentName(Type type, string nameInXmlDocument)
    {
        XmlName? definedXmlNameInType = XmlNameEvaluator.GetXmlNameOrNullFromMemberInfo(type);
        if(definedXmlNameInType != null && definedXmlNameInType.Name == nameInXmlDocument) return true;
        if(type.Name == nameInXmlDocument) return true;
        return false;
    }
    /// <summary>
    /// Fetches all Instantiable types from  all Referenced Assemblies by the calling Assembly.
    /// </summary>
    /// <param name="referencedAssemblies"></param>
    /// <returns></returns>
    private List<Type> GetAllInstantiableTypesFromAssemblyIncludingReferencedAssemblies(List<Assembly> referencedAssemblies)
    {
        List<Type> result = new List<Type>();
        foreach (Assembly referencedAssembly in referencedAssemblies)
        {
            result.AddRange(referencedAssembly.GetExportedTypes());
        }
        return result;
    }
    /// <summary>
    /// Loads and returns all Assemblies that are referenced by the current Assembly.
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    private List<Assembly> GetReferencedAssembliesFromAssembly(Assembly assembly)
    {
        List<Assembly> referencedAssemblies = new List<Assembly>();
        assembly.GetReferencedAssemblies().ToList().ForEach(assemblyName =>
            referencedAssemblies.Add(Assembly.Load(assemblyName))
        );
        return referencedAssemblies;
    }
}