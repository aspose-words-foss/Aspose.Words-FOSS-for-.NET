// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/03/2013 by Alexey Morozov

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Aspose.Xml;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// Implements C14N transform. See http://www.w3.org/TR/2001/REC-xml-c14n-20010315.
    /// </summary>
    internal class C14Transform : Transform
    {
        protected override void Read(AnyXmlReader reader)
        {
            // Nothing to read
        }

        /// <summary>
        /// Applies transformation to the specified stream.
        /// </summary>
        internal override MemoryStream Apply(Stream xml)
        {
            MemoryStream stream = new MemoryStream();
            if (xml.Length == 0)
                return stream;

            AnyXmlBuilder builder = new AnyXmlBuilder(stream, new UTF8Encoding(false), false);
            AnyXmlReader reader = new AnyXmlReader(xml, false);
            string curNodeName = reader.LocalName;
            while (reader.ReadChildWithTextValues(curNodeName, AnyXmlTextHandlingConsts.All))
                ProcessNode(reader, builder, null);

            builder.Flush();
            return stream;
        }

        internal override void Write(AnyXmlBuilder writer)
        {
            writer.StartElement("Transform");
            writer.WriteAttributeString("Algorithm", Algorithm);
            writer.EndElement("Transform");
        }

        /// <summary>
        /// Propagates specified namespace to topmost processing element nodes.
        /// </summary>
        internal void PropagateNamespace(string ns)
        {
            mPropagatingNamespace = ns;
        }

        /// <summary>
        /// Writes canonical form of the XML node to a stream.
        /// </summary>
        private bool ProcessNode(AnyXmlReader reader, AnyXmlBuilder builder,
            List<XmlAttribute> parentNamespaces)
        {
            bool hasData = true;
            XmlNodeType nodeType = reader.NodeType;
            switch (nodeType)
            {
                case XmlNodeType.Element:
                    ProcessElementNode(reader, builder, parentNamespaces);
                    break;
                case XmlNodeType.Text:
                    hasData = ProcessTextNode(reader, builder);
                    break;
                case XmlNodeType.SignificantWhitespace:
                case XmlNodeType.Whitespace:
                    hasData = ProcessWhitespaceNode(reader, builder);
                    break;
                case XmlNodeType.ProcessingInstruction:
                    ProcessPiNode(reader, builder);
                    break;
                case XmlNodeType.CDATA:
                    hasData = ProcessCDataNode(reader, builder);
                    break;
                case XmlNodeType.EntityReference:
                    hasData = ProcessEntityReferenceNode(reader, builder, parentNamespaces);
                    break;
                default:
                    break;
            }

            // Set the type of previously processed node, which has non-zero data length.
            if (hasData)
                mPrevNodeType = nodeType;

            return hasData;
        }

        /// <summary>
        /// Writes canonical form of the XML element node to a stream.
        /// </summary>
        private void ProcessElementNode(AnyXmlReader reader, AnyXmlBuilder builder,
            List<XmlAttribute> parentNamespaces)
        {
            Debug.Assert(reader.NodeType == XmlNodeType.Element);

            if (mPrevNodeType == XmlNodeType.ProcessingInstruction)
                builder.WriteRaw("\n");

            string elementName = reader.Name;
            builder.StartElement(elementName);

            string elementLocalName = reader.LocalName;

            // Process attributes (namespaces and regular attributes).
            List<XmlAttribute> namespaces = ProcessAttributes(reader, builder, parentNamespaces);

            // We need all namespaces starting from the root node up to the current node.
            if (parentNamespaces != null)
                namespaces.AddRange(parentNamespaces);

            // Process nested element nodes.
            while (reader.ReadChildWithTextValues(elementLocalName, AnyXmlTextHandlingConsts.All))
                ProcessNode(reader, builder, namespaces);

            builder.FullEndElement(elementName);
        }

        /// <summary>
        /// Writes canonical form for attributes of the specified element node to a stream.
        /// </summary>
        private List<XmlAttribute> ProcessAttributes(AnyXmlReader reader, AnyXmlBuilder builder,
            List<XmlAttribute> parentNamespaces)
        {
            // Collect attributes and namespaces separately to order them according to spec - 2.2 Document Order.
            List<XmlAttribute> namespaces = new List<XmlAttribute>();
            List<XmlAttribute> attributes = new List<XmlAttribute>();

            XmlAttribute emptyNamespace = null;
            while (reader.MoveToNextAttribute(false))
            {
                XmlAttribute xmlAttribute = new XmlAttribute(reader);
                if (xmlAttribute.IsNamespace)
                {
                    // IN. It is not entirely clear, when Word removes empty xmlns="". Spec says, that:
                    // "The latter condition eliminates unnecessary occurrences of xmlns="" in the canonical form..."
                    // But actually, it looks like when such namespace is a single namespace of element (no any other
                    // namespaces defined for that element node), then Word preserves it.
                    if (!string.IsNullOrEmpty(xmlAttribute.Value))
                        namespaces.Add(xmlAttribute);
                    else
                        emptyNamespace = xmlAttribute;
                }
                else
                {
                    attributes.Add(xmlAttribute);
                }
            }

            // Add propagated namespace to the topmost element nodes, if specified.
            if ((mPropagatingNamespace != null) && (parentNamespaces == null))
            {
                XmlAttribute propagatedNamespace = new XmlAttribute("xmlns", mPropagatingNamespace);
                if (!propagatedNamespace.Exists(namespaces))
                    namespaces.Add(propagatedNamespace);
            }

            // See 2.3 Processing Model - Namespace Nodes.
            RemoveInheritedNamespaces(namespaces, parentNamespaces);

            // See explanation above.
            if ((emptyNamespace != null) && (namespaces.Count == 0))
                namespaces.Add(emptyNamespace);

            XmlAttributesComparer comparer = new XmlAttributesComparer();
            namespaces.Sort(comparer);
            WriteAttributes(namespaces, builder);

            attributes.Sort(comparer);
            WriteAttributes(attributes, builder);

            return namespaces;
        }

        /// <summary>
        /// Writes canonical form of the XML text node to a stream.
        /// </summary>
        private static bool ProcessTextNode(AnyXmlReader reader, AnyXmlBuilder builder)
        {
            Debug.Assert(reader.NodeType == XmlNodeType.Text);

            return WriteFilteredString(builder, reader.Value);
        }

        /// <summary>
        /// Writes canonical form of the XML whitespace node to a stream.
        /// </summary>
        private bool ProcessWhitespaceNode(AnyXmlReader reader, AnyXmlBuilder builder)
        {
            Debug.Assert(reader.NodeType == XmlNodeType.Whitespace || reader.NodeType == XmlNodeType.SignificantWhitespace);

            if (reader.UnderlyingReader.Depth == 0)
                return false;

            if (mPrevNodeType == XmlNodeType.ProcessingInstruction)
                builder.WriteRaw("\n");

            return WriteFilteredString(builder, reader.Value);
        }

        /// <summary>
        /// Writes canonical form of the XML processing instruction node to a stream.
        /// </summary>
        private void ProcessPiNode(AnyXmlReader reader, AnyXmlBuilder builder)
        {
            Debug.Assert(reader.NodeType == XmlNodeType.ProcessingInstruction);

            if (mPrevNodeType != XmlNodeType.Document)
                builder.WriteRaw("\n");

            // MSDN at https://msdn.microsoft.com/en-us/library/w36h547w(v=vs.71).aspx says, that if data
            // is empty string, then it writes a ProcessingInstruction with no data content, for example <?name?>.
            // But actually, it writes an extra space before closing '?>' element in such case.
            // To avoid that, I have to write instruction name as a raw string.
            string data = CanonicalizeText(reader.Value);
            if (StringUtil.HasChars(data))
                builder.WriteProcessingInstruction(reader.LocalName, data);
            else
                builder.WriteRaw(string.Format("<?{0}?>", reader.LocalName));
        }

        /// <summary>
        /// Writes canonical form of the XML CDATA node to a stream.
        /// </summary>
        private bool ProcessCDataNode(AnyXmlReader reader, AnyXmlBuilder builder)
        {
            Debug.Assert(reader.NodeType == XmlNodeType.CDATA);

            string text = CanonicalizeText(reader.Value);
            if (text.Length > 0)
            {
                if (mPrevNodeType == XmlNodeType.ProcessingInstruction)
                    builder.WriteRaw("\n");

                builder.WriteRaw(text);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Writes canonical form of the XML entity reference node to a stream.
        /// </summary>
        private bool ProcessEntityReferenceNode(AnyXmlReader reader, AnyXmlBuilder builder,
            List<XmlAttribute> parentNamespaces)
        {
            Debug.Assert(reader.NodeType == XmlNodeType.EntityReference);

            string value = reader.Value;
            if (value != null)
            {
                builder.WriteRaw(value);
                return true;
            }

            string entityName = reader.LocalName;
            bool isEmpty = true;
            while(reader.ReadChildWithTextValues(entityName, AnyXmlTextHandlingConsts.All))
                if (ProcessNode(reader, builder, parentNamespaces))
                    isEmpty = false;

            return isEmpty;
        }

        /// <summary>
        /// Filters out invalid XML characters and writes specified value. Replaces "\r\n" with "\n".
        /// </summary>
        private static bool WriteFilteredString(AnyXmlBuilder builder, string value)
        {
            string text = builder.FilterInvalidXmlChars(value.Replace("\r\n", "\n"));
            builder.WriteString(text);

            return text.Length > 0;
        }

        /// <summary>
        /// Canonicalizes the string as described in W3C Canonical XML specification (see https://www.w3.org/TR/2001/REC-xml-c14n-20010315).
        /// </summary>
        private static string CanonicalizeText(string text)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
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
                    case '\r':
                    {
                        // Convert "\r\n" to "\n".
                        if (((i + 1) == text.Length) || (text[i + 1] != '\n'))
                            sb.Append(c);
                        break;
                    }
                    default:
                        sb.Append(c);
                        break;
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// Removes equal to parent (inherited) namespaces.
        /// </summary>
        private static void RemoveInheritedNamespaces(List<XmlAttribute> namespaces,
            List<XmlAttribute> parentNamespaces)
        {
            if (parentNamespaces == null)
                return;

            foreach (XmlAttribute parentNamespace in parentNamespaces)
            {
                RemoveEqualNamespaces(parentNamespace, namespaces);

                if (namespaces.Count == 0)
                    break;
            }
        }

        /// <summary>
        /// Removes namespaces equal to the specified reference namespace.
        /// </summary>
        private static void RemoveEqualNamespaces(XmlAttribute referenceNamespace, List<XmlAttribute> namespaces)
        {
            Debug.Assert(referenceNamespace.IsNamespace);

            for (int i = namespaces.Count - 1; i >= 0; i--)
            {
                XmlAttribute curNamespace = namespaces[i];
                if (curNamespace.Equals(referenceNamespace))
                    namespaces.RemoveAt(i);
            }
        }

        /// <summary>
        /// Writes collection of attributes to a stream.
        /// </summary>
        private static void WriteAttributes(List<XmlAttribute> attributes, AnyXmlBuilder builder)
        {
            foreach (XmlAttribute attribute in attributes)
                builder.WriteAttributeString(attribute.Name, attribute.Value);
        }

        internal const string Algorithm = @"http://www.w3.org/TR/2001/REC-xml-c14n-20010315";

        /// <summary>
        /// The type of previously processed node, which has non-zero data length.
        /// It means that something had to be written to the output stream for that node.
        /// </summary>
        private XmlNodeType mPrevNodeType = XmlNodeType.Document;

        /// <summary>
        /// A namespace that will be propagated to topmost processing element nodes.
        /// </summary>
        private string mPropagatingNamespace;
    }

    /// <summary>
    /// The helper class for comparing XML attribute nodes
    /// in accordance with W3C Canonical XML specification.
    /// </summary>
    internal class XmlAttributesComparer : IComparer<XmlAttribute>
    {
        public int Compare(XmlAttribute x, XmlAttribute y)
        {
            return x.CompareTo(y);
        }
    }

    /// <summary>
    /// Represents XML attribute.
    /// </summary>
    internal class XmlAttribute
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal XmlAttribute(AnyXmlReader reader)
        {
            mName = reader.Name;
            mValue = reader.Value;
            mNamespaceUri = reader.NamespaceURI;
            mPrefix = reader.Prefix;
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        internal XmlAttribute(string name, string value)
        {
            mName = name;
            mValue = value;
        }

        /// <summary>
        /// Returns true, if there is equal to this instance XmlAttribute exists in a specified array.
        /// </summary>
        internal bool Exists(IEnumerable<XmlAttribute> items)
        {
            foreach (XmlAttribute attr in items)
                if (Equals(attr))
                    return true;

            return false;
        }

        /// <summary>
        /// Returns true, if this instance is equal to a specified XML attribute.
        /// </summary>
        internal bool Equals(XmlAttribute otherXmlAttribute)
        {
            return CompareTo(otherXmlAttribute) == 0;
        }

        /// <summary>
        /// Compares this instance with a specified XmlAttribute object and indicates whether this instance
        /// precedes, follows, or appears in the same position in the sort order as the specified XmlAttribute.
        /// </summary>
        internal int CompareTo(XmlAttribute otherXmlAttribute)
        {
            return IsNamespace
                ? CompareAsNamespace(otherXmlAttribute)
                : CompareAsAttribute(otherXmlAttribute);
        }

        /// <summary>
        /// <para>
        /// Compares this instance with a specified XmlAttribute object and indicates whether this instance
        /// precedes, follows, or appears in the same position in the sort order as the specified XmlAttribute.
        /// </para>
        /// <para>Uses rules for comparing namespaces in accordance with W3C Canonical XML specification.</para>
        /// </summary>
        /// <remarks>See 2.2, 2.3 and also 3. - Examples of specification.</remarks>
        private int CompareAsNamespace(XmlAttribute otherNamespace)
        {
            int namesCompare = string.CompareOrdinal(Name, otherNamespace.Name);
            return (namesCompare != 0)
                ? namesCompare
                : string.CompareOrdinal(Value, otherNamespace.Value);
        }

        /// <summary>
        /// <para>
        /// Compares this instance with a specified XmlAttribute object and indicates whether this instance
        /// precedes, follows, or appears in the same position in the sort order as the specified XmlAttribute.
        /// </para>
        /// <para>Uses rules for comparing regular attributes (not namespaces) in accordance with W3C Canonical XML specification.</para>
        /// </summary>
        /// <remarks>See 2.2, 2.3 and also 3. - Examples of specification.</remarks>
        private int CompareAsAttribute(XmlAttribute otherAttr)
        {
            if (NamespaceUri == otherAttr.NamespaceUri)
                return string.CompareOrdinal(Name, otherAttr.Name);

            if (StringUtil.HasChars(NamespaceUri))
            {
                if (!StringUtil.HasChars(otherAttr.NamespaceUri))
                    return 1;
                else
                    return string.CompareOrdinal(NamespaceUri, otherAttr.NamespaceUri);
            }
            else
            {
                if (StringUtil.HasChars(otherAttr.NamespaceUri))
                    return -1;
            }

            return 0;
        }

        /// <summary>
        /// Attribute name.
        /// </summary>
        internal string Name
        {
            get { return mName; }
        }

        /// <summary>
        /// Attribute value.
        /// </summary>
        internal string Value
        {
            get { return mValue; }
        }

        /// <summary>
        /// Attribute namespace URI.
        /// </summary>
        internal string NamespaceUri
        {
            get { return mNamespaceUri; }
        }

        /// <summary>
        /// Returns true if this attribute is a namespace.
        /// </summary>
        internal bool IsNamespace
        {
            get
            {
                const string xmlns = "xmlns";
                return (mPrefix == xmlns) || (mName == xmlns);
            }
        }

        private readonly string mName;
        private readonly string mValue;
        private readonly string mNamespaceUri;
        private readonly string mPrefix;
    }
}
