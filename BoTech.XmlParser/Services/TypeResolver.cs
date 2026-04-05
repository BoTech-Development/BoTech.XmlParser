using System.Reflection;
using BoTech.XmlParser.Attributes;

namespace BoTech.XmlParser.Services;

public class TypeResolver
{
    private readonly List<Type> _instantiableTypes;
    public TypeResolver(Assembly callingAssembly)
    {
        _instantiableTypes = GetAllInstantiableTypesFromAssemblyIncludingReferencedAssemblies(callingAssembly);
    }

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
    public Type? GetAnotherTypeDefinedWithTheSameNameButIsNotGivenType(Type notThisTypeButTheSameName)
    {
        string typeName = notThisTypeButTheSameName.Name;
        foreach (var type in _instantiableTypes)
            if (type.Name == typeName && type.Namespace != notThisTypeButTheSameName.Namespace)
                return type;
        return null;
    }
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

    private List<Assembly> GetReferencedAssembliesFromAssembly(Assembly assembly)
    {
        List<Assembly> referencedAssemblies = new List<Assembly>();
        assembly.GetReferencedAssemblies().ToList().ForEach(assemblyName =>
            referencedAssemblies.Add(Assembly.Load(assemblyName))
        );
        return referencedAssemblies;
    }
}