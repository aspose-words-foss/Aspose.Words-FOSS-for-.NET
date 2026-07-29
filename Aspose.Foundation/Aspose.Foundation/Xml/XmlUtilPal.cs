// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/08/2010 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using Aspose.Collections.Generic;
using Aspose.JavaAttributes;

namespace Aspose.Xml
{
    /// <summary>
    /// This class is to be ported manually to Java.
    /// </summary>
    [JavaManual("Manual porting by design.")]
    public static class XmlUtilPal
    {
        /// <summary>
        /// Creates and returns a <see cref="XmlTextWriter"/>.
        /// This has to be abstracted because we need some special code for creating on Mono,
        /// but we don't have XmlTextWriter(StreamWriter) ctor on Java.
        /// </summary>
        public static XmlTextWriter CreateXmlTextWriter(Stream stream, Encoding encoding)
        {
            // AM: Create XmlTextWriter such way in order to explicitly set newline characters for Mono.
            StreamWriter sw = new StreamWriter(stream, encoding);
            sw.NewLine = "\r\n";
            return new XmlTextWriter(sw);
        }

        public static XmlTextReader CreateXmlTextReader(Stream stream)
        {
            XmlTextReader reader = new XmlTextReader(stream);
            // WORDSNET-21674 Investigate and fix XXE vulnerabilities in Aspose.Words.Net.
            // By default in .NetFramework 4.5 and earlier, the XmlTextReader uses the XmlUrlResolver which
            // will parse Dtds included in the XML document. To prohibit this, set the XmlResolver = null.
            reader.XmlResolver = null;
            return reader;
        }

        public static XmlTextReader CreateXmlTextReader(Stream stream, XmlResolver resolver)
        {
            XmlTextReader reader = new XmlTextReader(stream);
            reader.EntityHandling = EntityHandling.ExpandEntities;
            reader.XmlResolver = resolver;
            return reader;
        }

        /// <summary>
        /// Creates and returns a <see cref="XmlTextReader"/> for reading XML from a string.
        /// The string can be a complete document or a fragment.
        /// If the string is a fragment, you can specify optional hashtable of prefixes to namespaces mapping.
        /// </summary>
        public static XmlTextReader CreateXmlTextReader(string xml, IDictionary<string, string> namespaces)
        {
            NameTable nt = new NameTable();

            XmlNamespaceManager nsMgr = new XmlNamespaceManager(nt);
            if (namespaces != null)
            {
                foreach (KeyValuePair<string, string> entry in namespaces)
                    nsMgr.AddNamespace(entry.Key, entry.Value);
            }

            XmlParserContext context = new XmlParserContext(nt, nsMgr, null, XmlSpace.Default);

            XmlTextReader reader = new XmlTextReader(xml, XmlNodeType.Document, context);
            // WORDSNET-21674 Investigate and fix XXE vulnerabilities in Aspose.Words.Net.
            // By default in .NetFramework 4.5 and earlier, the XmlTextReader uses the XmlUrlResolver which
            // will parse Dtds included in the XML document. To prohibit this, set the XmlResolver = null.
            reader.XmlResolver = null;
            return reader;
        }

        /// <summary>
        /// Loads an XML string into a <see cref="XmlDocument"/>.
        /// </summary>
        [JavaThrows(true)]  // IO Exceptions
        public static XmlDocument LoadXml(string xml, bool preserveWhitespace)
        {
            XmlDocument xmlDoc = new XmlDocument();
            // WORDSNET-21674 Investigate and fix XXE vulnerabilities in Aspose.Words.Net.
            // By default in .NetFramework 4.5 and earlier, the XmlDocument uses the XmlUrlResolver which
            // will parse Dtds included in the XML document. To prohibit this, set the XmlResolver = null.
            xmlDoc.XmlResolver = null;
            xmlDoc.PreserveWhitespace = preserveWhitespace;
            xmlDoc.LoadXml(xml);
            return xmlDoc;
        }

        /// <summary>
        /// Loads a stream into a <see cref="XmlDocument"/>.
        /// </summary>
        [JavaThrows(true)]  // IO Exceptions
        public static XmlDocument LoadXml(Stream stream, bool preserveWhitespace)
        {
            XmlDocument xmlDoc = new XmlDocument();
            // WORDSNET-21674 Investigate and fix XXE vulnerabilities in Aspose.Words.Net.
            // By default in .NetFramework 4.5 and earlier, the XmlDocument uses the XmlUrlResolver which
            // will parse Dtds included in the XML document. To prohibit this, set the XmlResolver = null.
            xmlDoc.XmlResolver = null;
            xmlDoc.PreserveWhitespace = preserveWhitespace;
            xmlDoc.Load(stream);
            return xmlDoc;
        }

        [JavaThrows(true)]  // IO Exceptions
        public static void SaveXml(XmlDocument doc, Stream stream)
        {
            doc.Save(stream);
        }

        [JavaThrows(true)]  // IO Exceptions
        public static string GetOuterXml(XmlDocument doc)
        {
            return doc.OuterXml;
        }

        [JavaThrows(true)]  // IO Exceptions
        public static string GetOuterXml(XmlNode node)
        {
            return node.OuterXml;
        }

        /// <summary>
        /// Reads the contents, including markup, representing the current node and all its children.
        /// Skips ignorable whitespace after the end till the next node.
        /// </summary>
        public static string ReadOuterXml(XmlTextReader reader)
        {
            return ReadOuterXml(reader, null, null);
        }

        /// <summary>
        /// Reads the contents, including markup, representing the current node and all its children.
        /// Skips ignorable whitespace after the end till the next node.
        /// Allows providing the <see cref="IXmlUpdater"/> interface to update XML on reading.
        /// Also, allows providing the <see cref="XmlTextReaderNamespaceStorage"/> which populates with
        /// used XML namespaces while reading.
        /// </summary>
        public static string ReadOuterXml(XmlTextReader reader, IXmlUpdater xmlUpdater,
            XmlTextReaderNamespaceStorage namespaceStorage)
        {
            //@todo 3 sk read outer xml for attribute nodes.
            if (reader.NodeType != XmlNodeType.Element)
                return "";

            if (namespaceStorage == null)
                namespaceStorage = new XmlTextReaderNamespaceStorage(null);

            StringBuilder builder = new StringBuilder();
            ReadOuterXml(reader, builder, xmlUpdater, namespaceStorage);
            reader.Read();
            return builder.ToString();
        }

        private static void ReadOuterXml(XmlTextReader reader, StringBuilder builder,
            IXmlUpdater xmlUpdater, XmlTextReaderNamespaceStorage namespaceStorage)
        {
            ReadStartElement(reader, builder, namespaceStorage, xmlUpdater);

            if (reader.IsEmptyElement)
                return;

            while (true)
            {
                reader.Read();

                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        // calls himself recursively
                        ReadOuterXml(reader, builder,
                            xmlUpdater, new XmlTextReaderNamespaceStorage(namespaceStorage));
                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
                        builder.Append("</").Append(reader.Name).Append('>');
                        return;
                    }
                    case XmlNodeType.SignificantWhitespace:
                    case XmlNodeType.Whitespace:
                    {
                        builder.Append(reader.Value);
                        break;
                    }
                    case XmlNodeType.Text:
                    {
                        builder.Append(SecurityElement.Escape(reader.Value));
                        break;
                    }
                    case XmlNodeType.Comment:
                    {
                        // do nothing
                        break;
                    }
                    case XmlNodeType.CDATA:
                    {
                        // JAVA never gets here. StAX parser with coalescing setting (our default config) reports CDATA as CHARACTERS.
                        builder.Append("<![CDATA[").Append(reader.Value).Append("]]>");
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException(null, "Unexpected XML node type: " + reader.NodeType.ToString());
                }
            }
        }

        private static void ReadStartElement(XmlTextReader reader, StringBuilder builder,
            XmlTextReaderNamespaceStorage storage, IXmlUpdater xmlUpdater)
        {
            builder.Append('<');
            builder.Append(reader.Name);

            // storage.Namepaces contains namespace declarations
            // ordinary attributes
            SortedStringListGeneric<string> attributes = new SortedStringListGeneric<string>();

            // overridden prefixes are not supported at the moment
            // check prefix of element itself, if it is defined
            if (!string.IsNullOrEmpty(reader.Prefix) && !storage.IsPrefixDefined(reader.Prefix))
                storage.Namespaces.Add(reader.Prefix, reader.NamespaceURI);

            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    if (reader.Prefix == "xmlns")
                    {
                        // define a local namespace
                        if (!storage.IsPrefixDefined(reader.LocalName))
                            storage.Namespaces.Add(reader.LocalName, reader.Value);
                        continue;
                    }

                    if (reader.Name == "xmlns" && !storage.IsPrefixDefined(""))
                    {
                        // default namespace
                        storage.Namespaces.Add("", reader.Value);
                        continue;
                    }

                    attributes.Add(reader.Name, reader.Value);

                    if (!string.IsNullOrEmpty(reader.Prefix) && !storage.IsPrefixDefined(reader.Prefix))
                        storage.Namespaces.Add(reader.Prefix, reader.NamespaceURI);
                }
                while (reader.MoveToNextAttribute());
            }

            if (attributes.Count > 0)
            {
                foreach (KeyValuePair<string, string> e in attributes)
                    builder.AppendFormat(" {0}=\"{1}\"", e.Key, EscapeAttributeValue(e.Value));
            }
            if (storage.Namespaces.Count > 0)
            {
                foreach (KeyValuePair<string, string> e in storage.Namespaces)
                {
                    string namespaceUrl = xmlUpdater != null
                        ? xmlUpdater.ReplaceNamespace(e.Value)
                        : e.Value;
                    if (string.IsNullOrEmpty(e.Key))
                        builder.AppendFormat(" xmlns=\"{0}\"", EscapeAttributeValue(namespaceUrl));
                    else
                        builder.AppendFormat(" xmlns:{0}=\"{1}\"", e.Key, EscapeAttributeValue(namespaceUrl));
                }
            }

            reader.MoveToElement();
            if (reader.IsEmptyElement)
                builder.Append(" />");
            else
                builder.Append('>');
        }

        /// <summary>
        /// Replaces invalid XML characters in a string with their valid XML equivalent as <see cref="SecurityElement.Escape"/>
        /// does.
        /// </summary>
        /// <remarks>
        /// Carriage returns and line feeds do not need to be escaped in an attribute value in order for the XML
        /// to be well-formed.
        /// But we escape \r and \n too, because <see cref="XmlReader.ReadOuterXml"/> escapes them (if it is created
        /// via the ctor, rather than the <see cref="XmlReader.Create"/> factory method).
        /// </remarks>
        private static string EscapeAttributeValue(string value)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                switch (c)
                {
                    case '&':
                        sb.Append("&amp;");
                        break;
                    case '<':
                        sb.Append("&lt;");
                        break;
                    case '>':
                        sb.Append("&gt;");
                        break;
                    case '\'':
                        sb.Append("&apos;");
                        break;
                    case '\"':
                        sb.Append("&quot;");
                        break;
                    case '\r':
                        sb.Append("&#xD;");
                        break;
                    case '\n':
                        sb.Append("&#xA;");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Selects nodes using an XPath query.
        /// Namespaces is optional (can be null) and contains prefixes mapped to namespaces.
        /// </summary>
        public static IList<XmlNode> SelectNodes(XmlElement element, string xPath, IDictionary<string, string> namespaces)
        {
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(new NameTable());

            if (namespaces != null)
            {
                foreach (KeyValuePair<string, string> entry in namespaces)
                    nsMgr.AddNamespace(entry.Key, entry.Value);
            }
            else
            {
                CollectNamespaces(nsMgr, element, xPath);
            }

            XmlNodeList xmlNodes = element.SelectNodes(xPath, nsMgr);

            List<XmlNode> result = new List<XmlNode>();
            // Bind life time of selected nodes to avoid NullReferenceException in C++.
            foreach (XmlNode xmlNode in xmlNodes)
                result.Add(xmlNode);

            return result;
        }

        /// <summary>
        /// Select single node using an XPath query.
        /// Namespaces is optional (can be null) and contains prefixes mapped to namespaces.
        /// </summary>
        public static XmlNode SelectSingleNode(XmlElement element, string xPath, IDictionary<string, string> namespaces)
        {
            Debug.Assert(element != null);
            Debug.Assert(StringUtil.HasChars(xPath));

            XmlNode node;

            if (namespaces != null)
            {
                XmlNamespaceManager nsMgr = new XmlNamespaceManager(new NameTable());
                foreach (KeyValuePair<string, string> entry in namespaces)
                    nsMgr.AddNamespace(entry.Key, entry.Value);

                node = element.SelectSingleNode(xPath, nsMgr);
            }
            else
            {
                node = element.SelectSingleNode(xPath);
            }

            return node;
        }

         public static bool IsNodeType(XmlNode node, XmlNodeType xmlNodeType)
        {
            return node.NodeType == xmlNodeType;
        }

    /// <summary>
        /// Collects nodes from <paramref name="xPath"/> inside the <paramref name="element"/>
        /// and adds their name spaces to the <paramref name="nsMgr"/>.
        /// </summary>
        private static void CollectNamespaces(XmlNamespaceManager nsMgr, XmlElement element, string xPath)
        {
            // Index of next name space.
            int nsIdx = 0;

            // Add name space of the root element.
            AddNamespace(nsMgr, element, nsIdx++);

            // Remove indexers from the xPath and split it by nodes.
            string[] pathNodes = gRegexIndexer.Replace(xPath, string.Empty).
                Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string pathNode in pathNodes)
            {
                string pathNodeWithoutNamespace = gRegexNamespace.Replace(pathNode, string.Empty);

                // Speed. Avoid to collect namespaces for xpath part without namespace specification.
                if (pathNode == pathNodeWithoutNamespace)
                    continue;

                XmlNodeList xmlNodes = element.GetElementsByTagName(pathNodeWithoutNamespace);
                if (xmlNodes.Count != 0)
                    AddNamespace(nsMgr, xmlNodes[0], nsIdx++);
            }
        }

        /// <summary>
        /// Adds name space of <paramref name="xmlNode"/> into <paramref name="nsMgr"/> with 'ns' prefix.
        /// </summary>
        private static void AddNamespace(XmlNamespaceManager nsMgr, XmlNode xmlNode, int nsIdx)
        {
            string nsUri = GetNamespaceUri(xmlNode);

            if (StringUtil.HasChars(nsUri))
            {
                string nsPrefix = string.Format("ns{0}", nsIdx);
                nsMgr.AddNamespace(nsPrefix, nsUri);
            }
        }

        /// <summary>
        /// Returns name space Uri with empty prefix for the specified <paramref name="xmlNode"/>.
        /// </summary>
        private static string GetNamespaceUri(XmlNode xmlNode)
        {
            if (xmlNode.Attributes != null)
            {
                XmlAttribute xmlns = xmlNode.Attributes["xmlns"];
                if (xmlns != null)
                    return xmlns.Value;
            }

            return null;
        }

        private const string RegexPatternIndexer = @"(\[\d+\])";
        private static readonly Regex gRegexIndexer = new Regex(RegexPatternIndexer,
            RegexOptions.Compiled | RegexOptions.CultureInvariant);

        private const string RegexPatternNamespace = @"(ns\d+:)";
        private static readonly Regex gRegexNamespace = new Regex(RegexPatternNamespace,
            RegexOptions.Compiled | RegexOptions.CultureInvariant);
    }
}
