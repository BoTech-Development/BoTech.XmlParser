using System;
using System.Linq;
using BoTech.XmlParser.Models;
using BoTech.XmlParser.Models.Deserializer;

namespace BoTech.XmlParser;

public class XmlDeserializer
{
    /// <summary>
    /// Deserialize any xml.
    /// </summary>
    /// <param name="xml"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T? Deserialize<T>(string xml) where T : new()
    {
        GroupedXmlStringNode masterParentNode = CreateInitialNestedXmlStructureFromSplitXml(
            SplitXmlStringByLessThanCharAndRemoveSpacesAndNewLineSymbols(xml));
        return default(T);
    }
    /// <summary>
    /// Splits the string with the smaller than char and removes all spaces and the <c>\n</c> symbol.
    /// </summary>
    /// <param name="xml"></param>
    /// <returns>An array which contains the trimmed / split strings-</returns>
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