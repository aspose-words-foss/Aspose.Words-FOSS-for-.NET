// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/04/2021 by Mikhail Nepreteamov

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Aspose.Xml;

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Custom XML utilities.
    /// </summary>
    /// <dev>
    /// Several static methods taken from the <see cref="XmlMapping"/> class.
    /// </dev>
    internal static class CustomXmlUtil
    {
        /// <summary>
        /// Extracts XML nodes.
        /// </summary>
        /// <remarks>
        /// Look once again for caller duplicate.
        /// </remarks>
        internal static IList<XmlNode> ExtractXmlNodes(
            byte[] xmlData, string xPath, string prefixMappings, string storeItemId, XmlMappingContext ctx)
        {
            IList<XmlNode> xmlNodes = new List<XmlNode>();
            try
            {
                XmlDocument xml = GetXmlDocumentAndPlaceItToCache(xmlData, storeItemId, ctx);

                // WORDSJAVA-925 fixed for java only: CustomXmlNodesCache demands 2Gb heap to save docx to doc.
                // The cache is not used in java now.
#if !JAVA
                if ((ctx != null) && ctx.ContainsXPath(storeItemId, xPath))
                    return ctx.Get(storeItemId, xPath);
#endif
                IDictionary<string, string> namespaces = PrefixMappingsAsDictionary(prefixMappings);

                xmlNodes = XmlUtilPal.SelectNodes(xml.DocumentElement, xPath, namespaces);
                // andrnosk: WORDSNET-9940 Word extracts value if ng at first root element.
                if ((xmlNodes.Count == 0) && !xPath.StartsWith("/", StringComparison.Ordinal))
                {
                    xmlNodes =
                        XmlUtilPal.SelectNodes(xml.DocumentElement, string.Format(@"/{0}", xPath), namespaces);
                }

                // AM. Word extracts TermInfo/TermName element if Terms element was selected.
                // I don't fully understand this logic, seems to be some predefined XML element.
                if (IsPartnerControlsNamespace(xmlNodes))
                {
                    string partnerControlsPrefix = "";
                    // Look for ParentControl namespace prefix.
                    foreach (KeyValuePair<string, string> entry in namespaces)
                    {
                        if (PartnerControlsNamespace == entry.Value)
                        {
                            partnerControlsPrefix = entry.Key;
                            break;
                        }
                    }

                    string partnerControlsXPath =
                        string.Format(@"{0}/{1}:TermInfo/{1}:TermName", xPath, partnerControlsPrefix);
                    xmlNodes = XmlUtilPal.SelectNodes(xml.DocumentElement, partnerControlsXPath, namespaces);
                }
#if !JAVA
                if (ctx != null)
                    ctx.Add(storeItemId, xPath, xmlNodes);
#endif
                return xmlNodes;
            }
            catch
            {
                // Do not throw in case invalid XPath.
            }

            return xmlNodes;
        }

        /// <summary>
        /// Collects and concatenates text from xml nodes collection.
        /// </summary>
        /// <remarks>
        /// Mimic MSW behavior, collects text from certain CustomXml nodes.
        /// Please, check @"ImportDocx\Markup\TestSdtGetSetMulti.docx" for the details.
        /// </remarks>
        internal static string GetOverallInnerText(IList<XmlNode> xmlNodes)
        {
            if (xmlNodes.Count == 0)
                return "";

            if (!IsPartnerControlsNamespace(xmlNodes))
                return xmlNodes[0].InnerText;

            StringBuilder sb = new StringBuilder();

            int length = xmlNodes.Count;
            for (int i = 0; i < length; i++)
            {
                sb.Append(xmlNodes[i].InnerText);
                if (i != length - 1)
                    sb.Append("; ");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Shows if namespace of these nodes is PartnerControls.
        /// MSW behaves in a special way for this case, we must consider this.
        /// </summary>
        internal static bool IsPartnerControlsNamespace(IList<XmlNode> xmlNodes)
        {
            return (xmlNodes.Count > 0) && (xmlNodes[0].NamespaceURI == PartnerControlsNamespace);
        }

        /// <summary>
        /// Creates mapped XML node for given XPath.
        /// </summary>
        /// <dev>
        /// AM. Very draft solution, does not consider iterators and other complex XPath features.
        /// Will improve it on demand.
        /// </dev>
        internal static IList<XmlNode> CreateMappedXmlNode(string xpath, string prefixMappings)
        {
            MemoryStream tempStream = new MemoryStream();
            AnyXmlBuilder xmlBuilder = new AnyXmlBuilder(tempStream, true);

            IDictionary<string, string> namespaces = PrefixMappingsAsDictionary(prefixMappings);
            string[] pathParts = xpath.Split('/');

            xmlBuilder.StartDocument();
            foreach (string pathPart in pathParts)
            {
                if(string.IsNullOrEmpty(pathPart))
                    continue;

                xmlBuilder.StartElement(ParseElementName(pathPart));

                string xmlns = ParseNamespace(pathPart, namespaces);

                if(!string.IsNullOrEmpty(xmlns))
                    xmlBuilder.WriteAttributeString("xmlns", xmlns);
            }

            Array.Reverse(pathParts);
            foreach (string element in pathParts)
            {
                if(string.IsNullOrEmpty(element))
                    continue;

                xmlBuilder.EndElement(ParseElementName(element));
            }

            xmlBuilder.EndDocument();
            tempStream.Position = 0;

            XmlDocument xml = XmlUtilPal.LoadXml(tempStream, true);
            IList<XmlNode> xmlNodes = XmlUtilPal.SelectNodes(xml.DocumentElement, xpath, namespaces);

            return xmlNodes;
        }

        /// <summary>
        /// Returns structured document tag PrefixMappings as Dictionary.
        /// </summary>
        /// <remarks>
        /// AM. I didn't implement caching it will not be called often till public API implemented.
        /// </remarks>
        private static IDictionary<string, string> PrefixMappingsAsDictionary(string prefixMappings)
        {
            // Speed.
            if (!StringUtil.HasChars(prefixMappings))
                return null;

            // AM. It's kind of trick here. I don't want to parse mapping manually and
            // construct fake XML document with such mapping. XmlReader does the rest.
            AnyXmlReader xmlMapping = new AnyXmlReader(@"<?xml version='1.0' encoding='utf-8' standalone='yes'?>" +
                                                       "<map " + prefixMappings + @"/>", null);

            Dictionary<string, string> result = new Dictionary<string, string>();
            xmlMapping.MoveToElement();

            // Do not ignore xmlns prefix.
            while (xmlMapping.MoveToNextAttribute(false))
                result.Add(xmlMapping.LocalName, xmlMapping.Value);

            return result;
        }

        /// <summary>
        /// Gets an XML document and places it in the cache.
        /// </summary>
        private static XmlDocument GetXmlDocumentAndPlaceItToCache(
            byte[] xmlData, string storeItemId, XmlMappingContext ctx)
        {
            // Look for cached XmlDocument.
            if ((ctx != null) && ctx.ContainsXmlDocument(storeItemId))
                return ctx.Get(storeItemId);

            XmlDocument xml = XmlUtilPal.LoadXml(new MemoryStream(xmlData), true);

            if (ctx != null)
                ctx.Add(storeItemId, xml);

            return xml;
        }

        /// <summary>
        /// Parses element name from XPath part.
        /// </summary>
        private static string ParseElementName(string pathPart)
        {
            string elementName = "";

            // Extract element name.
            if (pathPart.Contains(":"))
                elementName = pathPart.Split(':')[1];

            // Remove possible iterator.
            if(elementName.Contains("["))
                elementName = elementName.Split('[')[0];

            return elementName;
        }

        /// <summary>
        /// Parses namespace from XPath part.
        /// </summary>
        private static string ParseNamespace(string pathPart, IDictionary<string, string> namespaces)
        {
            string ns = "";

            if (pathPart.Contains(":"))
                ns = pathPart.Split(':')[0];

            string value;
            if (namespaces.TryGetValue(ns, out value))
                return value;
            else
                return "";
        }

        /// <summary>
        /// PartnerControls namespace.
        /// </summary>
        private const string PartnerControlsNamespace =
            @"http://schemas.microsoft.com/office/infopath/2007/PartnerControls";
    }
}
