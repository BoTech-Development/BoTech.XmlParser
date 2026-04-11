using System;
using System.Linq;
using System.Reflection;
using BoTech.XmlParser.Helper.Deserializer;
using BoTech.XmlParser.Models;
using BoTech.XmlParser.Models.Deserializer;

namespace BoTech.XmlParser;

public class XmlDeserializer
{
    private SemiParsedNodeTypeResolver _nodeTypeResolver;
    private SemiParsedNodeTypeValidator _nodeTypeValidator;
    /// <summary>
    /// Deserialize any XML.
    /// </summary>
    /// <param name="xml"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T? Deserialize<T>(string xml) where T : new()
    {
        GroupedXmlStringNode masterParentNode = CreateInitialNestedXmlStructureFromSplitXml(
            SplitXmlStringByLessThanCharAndRemoveSpacesAndNewLineSymbols(xml));
        XmlNode node = new GroupedXmlStringToXmlNodeConverter().ConvertGroupedXmlNodesToXmlNodes(masterParentNode);
        InitializeTypeHelper(Assembly.GetCallingAssembly());
        CheckAndResolveNodesRecursive(node);
        return default(T);
    }
    /// <summary>
    /// Inits all helper objects.
    /// </summary>
    /// <param name="callingAssembly"></param>
    private void InitializeTypeHelper(Assembly callingAssembly)
    {
        _nodeTypeResolver = new SemiParsedNodeTypeResolver(callingAssembly);
        _nodeTypeValidator = new SemiParsedNodeTypeValidator();
    }
    /// <summary>
    /// Checks the following:
    ///  1. Each type is instantiable via an empty constructor.
    ///  2. Each Property declared in the XmlNode is defined and settable in the referenced type.
    /// Resolves the following:
    ///  1. The referenced type for each XmlNode.
    /// </summary>
    /// <param name="node">The Main node.</param>
    private void CheckAndResolveNodesRecursive(XmlNode node)
    {
        _nodeTypeResolver.TryToResolveNodeTypesAndStoreThemInXmlNodes(node);
        _nodeTypeValidator.CheckNodeTypeForEmptyConstructors(node);
        foreach (XmlNode childNode in node.Children) CheckAndResolveNodesRecursive(childNode);
    }
    /// <summary>
    /// Splits the string with the smaller than char and removes all spaces and the <c>\n</c> symbol.
    /// </summary>
    /// <param name="xml"></param>
    /// <returns>An array that contains the trimmed / split strings-</returns>
    private string[] SplitXmlStringByLessThanCharAndRemoveSpacesAndNewLineSymbols(string xml)
    {
        string[] splittedXml = xml.Split("<");
        for (int i = 0; i < splittedXml.Length; i++) splittedXml[i] = splittedXml[i].Trim().Replace("\n", "");
        return splittedXml;
    }
    /// <summary>
    /// Creates the <see cref="GroupedXmlStringNode"/> structure. When an Xml Node has a body defined, all item of this body will be inserted as children to the current <see cref="GroupedXmlStringNode"/>.
    /// </summary>
    /// <param name="splitXml">The split xml by the <see cref="SplitXmlStringByLessThanCharAndRemoveSpacesAndNewLineSymbols"/> method. </param>
    /// <returns>The master parent node.</returns>
    private GroupedXmlStringNode CreateInitialNestedXmlStructureFromSplitXml(string[] splitXml)
    {
        GroupedXmlStringNode masterParentNode = new GroupedXmlStringNode(splitXml[1]);
        GroupedXmlStringNode currentParentNode = masterParentNode;
        Stack<GroupedXmlStringNode> nestedNodeStructure = new Stack<GroupedXmlStringNode>();
        bool startsWithEndXmlTag = false;
        bool hasXmlTagABody = false;
        foreach (string xmlPart in splitXml.Skip(2))
        {
            startsWithEndXmlTag = xmlPart.StartsWith("/");
            hasXmlTagABody = xmlPart.EndsWith("/>");
            if (!startsWithEndXmlTag && !hasXmlTagABody)
            {
                GroupedXmlStringNode newNode = GetAndCreateNewGroupedXmlStringNodeAndAddToParentNode(currentParentNode, xmlPart);
                nestedNodeStructure.Push(newNode);
                currentParentNode = newNode;
            }
            else if (!startsWithEndXmlTag && hasXmlTagABody)
            {
                GetAndCreateNewGroupedXmlStringNodeAndAddToParentNode(currentParentNode, xmlPart);
            }
            else if(startsWithEndXmlTag && !hasXmlTagABody && nestedNodeStructure.Count > 1)
            {
                nestedNodeStructure.Pop();
                currentParentNode = nestedNodeStructure.Peek();  
            }
        }
        return masterParentNode;
    }

    private GroupedXmlStringNode GetAndCreateNewGroupedXmlStringNodeAndAddToParentNode(GroupedXmlStringNode parentNode,
        string xmlPart)
    {
        GroupedXmlStringNode newNode = new GroupedXmlStringNode(xmlPart);
        parentNode.Children.Add(newNode);
        return newNode;
    }
}