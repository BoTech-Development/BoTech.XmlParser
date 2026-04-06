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
    private TypeResolver(Assembly callingAssembly)
    {
        _instantiableTypes = GetAllInstantiableTypesFromAssemblyIncludingReferencedAssemblies(callingAssembly);
    }
    /// <summary>
    /// Creates a new instance of the singleton
    /// </summary>
    /// <param name="callingAssembly">The Assembly that called this Serialisation or Deserialisation method.</param>
    public static void CreateInstance(Assembly callingAssembly){ _instance = new TypeResolver(callingAssembly); }
    /// <summary>
    /// Resets the singleton (deletes the current Instance). Please use this method for releasing Memory.
    /// </summary>
    public void Clear()
    {
        _instance = null;
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
    /// <summary>
    /// Fetches all Instantiable types from the given Assembly and all Referenced Assemblies by this Assembly.
    /// </summary>
    /// <param name="assembly"></param>
    /// <returns></returns>
    private List<Type> GetAllInstantiableTypesFromAssemblyIncludingReferencedAssemblies(Assembly assembly)
    {
        List<Type> result = new List<Type>();
        result.AddRange(assembly.GetExportedTypes());
        List<Assembly> referencedAssemblies = GetReferencedAssembliesFromAssembly(assembly);//AppDomain.CurrentDomain.GetAssemblies().ToList();
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