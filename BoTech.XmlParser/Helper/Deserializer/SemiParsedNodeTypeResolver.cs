using System.Reflection;
using BoTech.XmlParser.Models;
using BoTech.XmlParser.Services;

namespace BoTech.XmlParser.Helper.Deserializer;

public class SemiParsedNodeTypeResolver
{
    public SemiParsedNodeTypeResolver(Assembly callingAssembly)
    {
        TypeResolver.CreateInstance(callingAssembly);
    }
    public void TryToResolveNodeTypesAndStoreThemInXmlNodes(XmlNode semiParsedNode)
    {
        string nameOfNodeInAXmlDocument = semiParsedNode.GetNameOfThisNodeInAXmlDocument();
        if(nameOfNodeInAXmlDocument != "?xml" && !semiParsedNode.IsPropertyIdentifier)
        {
            semiParsedNode.ReferencedType = TypeResolver.Instance.TryToGetTypeByNameInXmlDocument(
                nameOfNodeInAXmlDocument,
                semiParsedNode.GetDeclaredNamespaceIfExist());
        }
    }
}