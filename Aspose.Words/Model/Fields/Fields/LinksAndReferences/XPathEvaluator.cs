// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/10/2015 by Edward Voronov

using System.Collections.Generic;
using System.Text;
using System.Xml;
using Aspose.Xml;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Evaluates xpath expression for INCLUDETEXT field and formats result in MS Word manner.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppSkipEntity("XPath is not supported in C++ yet.")]
    internal static class XPathEvaluator
    {
        internal static string Evaluate(XmlDocument document, string xpath, string namespaceMappings)
        {
#if !JAVA
            Debug.Assert(document.PreserveWhitespace);
#endif

            IDictionary<string, string> mappings = ExtractNamespaceMappings(namespaceMappings);

            IList<XmlNode> nodes;
            try
            {
                nodes = XmlUtilPal.SelectNodes(document.DocumentElement, xpath, mappings);
            }
            catch
            {
                return string.Empty;
            }

            StringBuilder builder = new StringBuilder();

            foreach (XmlNode node in nodes)
                builder.Append(AcceptXmlNode(node).Trim());

            return builder.ToString();
        }

        private static string AcceptXmlNode(XmlNode node)
        {
            if (XmlUtilPal.IsNodeType(node, XmlNodeType.Whitespace))
                return string.Empty;

            if (XmlUtilPal.IsNodeType(node, XmlNodeType.Text))
                return AcceptXmlNode((XmlText)node);

            if (XmlUtilPal.IsNodeType(node, XmlNodeType.Element))
                return AcceptXmlNode((XmlElement)node);

            if (XmlUtilPal.IsNodeType(node, XmlNodeType.Attribute))
                return AcceptXmlNode((XmlAttribute)node);

            else
                    Debug.Assert(false, string.Format("Unexpected node type: {0}.", node.NodeType));

            return string.Empty;
        }

        private static string AcceptXmlNode(XmlAttribute attribute)
        {
            return attribute.Value;
        }

        private static string AcceptXmlNode(XmlText text)
        {
            return text.Data;
        }

        private static string AcceptXmlNode(XmlElement element)
        {
            StringBuilder builder = new StringBuilder();
            XmlElement previousChildAsElement = null;
            for (int i = 0; i < element.ChildNodes.Count; i++)
            {
                XmlNode childNode = element.ChildNodes.Item(i);
                string childResult = AcceptXmlNode(childNode);
                if (string.IsNullOrEmpty(childResult))
                    continue;

                if (previousChildAsElement != null)
                {
                    if (XmlUtilPal.IsNodeType(childNode, XmlNodeType.Element))
                        builder.Append(Separator);

                    if (XmlUtilPal.IsNodeType(childNode, XmlNodeType.Text) && IsLastChildWhitespace(previousChildAsElement))
                        builder.Append(Separator);
                }

                builder.Append(childResult);

                previousChildAsElement = childNode as XmlElement;
            }

            return builder.ToString();
        }

        private static bool IsLastChildWhitespace(XmlNode element)
        {
            XmlNode lastChild = element.LastChild;

            if (lastChild == null)
                return false;

            if (XmlUtilPal.IsNodeType(lastChild, XmlNodeType.Whitespace))
                return true;

            if (XmlUtilPal.IsNodeType(lastChild, XmlNodeType.Element))
                return IsLastChildWhitespace(lastChild);

            return false;
        }

        private static IDictionary<string, string> ExtractNamespaceMappings(string namespaceMappings)
        {
            if (string.IsNullOrEmpty(namespaceMappings))
                return null;

            try
            {
                XmlDocument document = XmlUtilPal.LoadXml(string.Format(@"<root {0} />", namespaceMappings), false);

                Debug.Assert(document.DocumentElement != null);

                XmlAttributeCollection attributes = document.DocumentElement.Attributes;

                if (attributes.Count == 0)
                    return null;

                Dictionary<string, string> result = new Dictionary<string, string>(attributes.Count);

                for (int i = 0; i < attributes.Count; i++)
                    result.Add(attributes[i].LocalName, attributes[i].Value);

                return result;
            }
            catch
            {
                return null;
            }
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const char Separator = ControlChar.SpaceChar;
    }
}
