// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/07/2007 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Aspose.Common;

namespace Aspose.Xml
{
    /// <summary>
    /// Provides useful methods to read any XML file (DOCX, WordML, ODF or whatever).
    /// Do not put any MS Word specific methods here.
    ///
    /// This class is implemented as a wrapper over XmlTextReader.
    /// Wrapping reduces the interface exposed to the client classes and makes easier porting to Java.
    /// </summary>
    public class AnyXmlReader
    {
        /// <summary>
        /// Initializes XML reader, positions to the first content node.
        /// </summary>
        public AnyXmlReader(Stream stream) : this(stream, true, false)
        {
        }

        /// <summary>
        /// Initializes XML reader.
        /// </summary>
        /// <param name="stream">The stream containing the XML data to read.</param>
        /// <param name="moveToContent">Indicates that reader should skip ahead to the next content node.</param>
        /// <param name="useFullElementName">Reader will use full element name (e.g. "xml:space") instead of local name.</param>
        public AnyXmlReader(Stream stream, bool moveToContent, bool useFullElementName)
        {
            mUseFullElementName = useFullElementName;
            // Some DOCX documents might require reading the same part twice (for example same footer is referenced
            // twice), therefore the stream's position must be reset to start. I am putting this here, not in DOCX
            // because it covers more cases and it looks like always have to read from stream start.
            stream.Position = 0;

            mReader = XmlUtilPal.CreateXmlTextReader(stream);

            if (moveToContent)
                mReader.MoveToContent();

            mUnderlyingStream = stream;
        }

        public AnyXmlReader(Stream stream, bool moveToContent)
            : this(stream, moveToContent, false)
        {
            // Ctor.
        }

        /// <summary>
        /// Ctor to read from a string, possibly a fragment.
        /// </summary>
        /// <param name="xml">This can be an XML document or fragment.</param>
        /// <param name="namespaces">Key is prefix, value is namespace url. Needed to properly load
        /// fragments that contain elements with prefixes. Optional, can be null.</param>
        /// <param name="useFullElementName">Reader will use full element name (e.g. "xml:space") instead of local name.</param>
        public AnyXmlReader(string xml, IDictionary<string, string> namespaces, bool useFullElementName)
        {
            mReader = XmlUtilPal.CreateXmlTextReader(xml, namespaces);
            mReader.MoveToContent();
            mUseFullElementName = useFullElementName;
        }

        public AnyXmlReader(string xml, IDictionary<string, string> namespaces)
            : this(xml, namespaces, false)
        {
            // Ctor.
        }

        /// <summary>
        /// Reads to the start of the next element in the stream.
        /// Stops reading when reaches the end of the specified element or the end of file.
        /// Used to read child elements of the specified element.
        ///
        /// Reader should already be positioned on the specified element
        /// (or on an attribute of the element).
        /// </summary>
        /// <param name="parentElement">The local name of an element,
        /// end of which limits the scope of the operation.</param>
        /// <returns>Returns true if the element was read.
        /// Returns false if reached the end of the specified element or the end of file.</returns>
        public bool ReadChild(string parentElement)
        {
            return ReadChildWithTextValues(parentElement, AnyXmlTextHandling.None);
        }

        /// <summary>
        /// Reads to the start of the next element in the stream.
        /// Stops reading when reaches the end of the specified element, or the end of file.
        ///
        /// Used to read child elements of the specified element.
        ///
        /// Reader should already be positioned on the specified element
        /// (or on an attribute of the element).
        /// </summary>
        /// <param name="parentElement">The local name of an element,
        /// end of which limits the scope of the operation.</param>
        /// <param name="textHandling">Specifies how to return text and whitespace nodes to the handler.</param>
        /// <returns>Returns true if the element was read. Returns false if reached the end of the specified element or the end of file.</returns>
        public bool ReadChildWithTextValues(string parentElement, AnyXmlTextHandling textHandling)
        {
            // Move to element in case we are positioned on one of its attributes.
            mReader.MoveToElement();

            // Signal the caller to stop if we have an empty element.
            if (IsEmptyElement && (GetNameInternal() == parentElement))
                return false;

            // This loop skips XML nodes that are not elements (comments etc).
#if JAVA
            while (mReader.read() && mReader.getDepth() < 1800) // WORDSNET-26597 Java limit of depth is around 2000
#else
            while (mReader.Read())
#endif
            {
                switch (mReader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        // Found a child element, return it to the caller.
                        return true;
                    }
                    case XmlNodeType.EndElement:
                    {
                        // If reached the end of the parent element, signal the caller to stop.
                        if (IsEndElement(parentElement))
                            return false;
                        break;
                    }
                    case XmlNodeType.CDATA:
                    case XmlNodeType.Text:
                    {
                        // Found a text value, return it to the caller.
                        if ((textHandling & AnyXmlTextHandling.Text) != 0)
                            return true;
                        break;
                    }
                    case XmlNodeType.SignificantWhitespace:
                    {
                        // Found a text value, return it to the caller.
                        if ((textHandling & AnyXmlTextHandling.SignificantWhitespace) != 0)
                            return true;
                        break;
                    }
                    case XmlNodeType.Whitespace:
                    {
                        // If the caller requests, we have to return ignorable whitespace too, because in ODT such whitespace can be significant.
                        if ((textHandling & AnyXmlTextHandling.Whitespace) != 0)
                            return true;
                        break;
                    }
                    case XmlNodeType.ProcessingInstruction:
                    {
                        // ProcessingInstruction is significant for C14Transform algorithm.
                        if ((textHandling & AnyXmlTextHandling.ProcessingInstruction) != 0)
                            return true;
                        break;
                    }
                    case XmlNodeType.EntityReference:
                    {
                        // EntityReference is significant for C14Transform algorithm.
                        if ((textHandling & AnyXmlTextHandling.EntityReference) != 0)
                        {
                            if (mReader.CanResolveEntity)
                                mReader.ResolveEntity();

                            return true;
                        }
                        break;
                    }
                    default:
                        // Do nothing and it will skip.
                        break;
                }
            }

            // If reached end of file, signal the caller to stop.
            return false;
        }

        /// <summary>
        /// Skips the current element content and positions reader to the element's end.
        /// Reader should already be positioned on the specified element
        /// (or on an attribute of the element).
        ///
        /// This method is different from XmlTextReader.Skip in the way it positions the cursor.
        /// Skip positions to start of next element, but this method positions to the end of the skipped element.
        /// This is needed because we read elements using ReadChild and it will read next node when called.
        /// </summary>
        public virtual void IgnoreElement()
        {
            // Move to element in case we are positioned on one of its attributes.
            mReader.MoveToElement();

            // There is nothing to skip if the element is empty.
            if (IsEmptyElement)
                return;

            // This is the name of the element we are skipping.
            string tagName = GetNameInternal();
            while (!IsEndElement(tagName))
            {
                mReader.Read();

                // This skips child elements nicely.
                if (mReader.NodeType == XmlNodeType.Element)
                    mReader.Skip();
            }
        }

        public bool IsEndElement(string elemName)
        {
            return (mReader.NodeType == XmlNodeType.EndElement) && (GetNameInternal() == elemName);
        }

        /// <summary>
        /// Reads a value of the attribute with the specified name.
        /// If the attribute is not found returns the default value.
        /// </summary>
        public string ReadAttribute(string name, string defaultValue)
        {
            string result = defaultValue;

            // RK We are using MoveToNextAttribute, not GetAttribute because we are looking up using local name.
            // GetAttribute takes either a qualified name or a local name and namespace uri.
            // In the future I actually want to rework all xml reading code to specify the namespace everywhere.
            while (mReader.MoveToNextAttribute())
            {
                if (GetNameInternal() == name)
                {
                    result = mReader.Value;
                    break;
                }
            }

            // RK Reset the position so we can read attributes of this element again.
            mReader.MoveToElement();

            return result;
        }

        /// <summary>
        /// Reads a value of the attribute with the specified name.
        /// If the attribute is not found returns the default value.
        /// </summary>
        public int ReadIntAttribute(string localName, int defaultValue)
        {
            string s = ReadAttribute(localName, null);
            return (s != null) ? FormatterPal.XmlToInt(s) : defaultValue;
        }

        /// <summary>
        /// Reads a value of the attribute with the specified name.
        /// If the attribute is not found returns the default value.
        /// </summary>
        public uint ReadUIntAttribute(string localName, uint defaultValue)
        {
            string s = ReadAttribute(localName, null);
            return (s != null) ? FormatterPal.XmlToUInt(s) : defaultValue;
        }

        /// <summary>
        /// Reads a value of the attribute with the specified name.
        /// If the attribute is not found returns the default value.
        /// </summary>
        public long ReadLongAttribute(string localName, long defaultValue)
        {
            string s = ReadAttribute(localName, null);
            return (s != null) ? FormatterPal.XmlToLong(s) : defaultValue;
        }

        /// <summary>
        /// Reads a value of the attribute with the specified name.
        /// If the attribute is not found returns the default value.
        /// </summary>
        public double ReadDoubleAttribute(string localName, double defaultValue)
        {
            string s = ReadAttribute(localName, null);
            return (s != null) ? FormatterPal.ParseDouble(s) : defaultValue;
        }

        /// <summary>
        /// Reads a value of the attribute with the specified name.
        /// If the attribute is not found returns the default value.
        /// </summary>
        public bool ReadBoolAttribute(string localName, bool defaultValue)
        {
            string s = ReadAttribute(localName, null);
            if (s == null)
                return defaultValue;
            //XML Schema Part 2: Datatypes
            //3.2.2.1 Lexical representation
            // An instance of a datatype that is defined as boolean can have the following literals {true, false, 1, 0}.
            // Practice shows {t} can be used instead of {true} also.
            return ConvertToBool(s);
        }

        public static bool ConvertToBool(string s)
        {
            return s == "1" || s == "true" || s == "t";
        }

        /// <summary>
        /// Reads a value of the attribute with the specified name.
        /// If the attribute is not found returns the default value.
        /// </summary>
        public Guid ReadGuidAttribute(string localName, Guid defaultValue)
        {
            string s = ReadAttribute(localName, null);
            return StringUtil.HasChars(s) ? new Guid(s) : defaultValue;
        }

        /// <summary>
        /// Moves to the element that contains the current attribute node.
        /// </summary>
        public bool MoveToElement()
        {
            return mReader.MoveToElement();
        }

        /// <summary>
        /// Moves to the next attribute of the current element.
        ///
        /// RK Note, this method skips attributes with the "xmlns" prefix e.g "xmlns:w" because at the moment
        /// our readers look at the local name only and we don't want "xmlns:w" to be parsed as "w".
        /// In the future I should fix parsers to somehow take namespace into account too.
        /// </summary>
        public bool MoveToNextAttribute()
        {
            return MoveToNextAttribute(true);
        }

        public bool MoveToNextAttribute(bool ignoreXmlns)
        {
            while (mReader.MoveToNextAttribute())
            {
                if ((mReader.Prefix != "xmlns") || (!ignoreXmlns))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Reads the contents of an element or text node as a string.
        /// </summary>
        public string ReadString()
        {
            return mReader.ReadString();
        }

        /// <summary>
        /// Reads the contents of an element or text node as a string. Converts _x000D_ to \r character and x000B to \v.
        /// </summary>
        /// <remarks>
        /// AM. I think this replacement (_x000D to CR and x000B to Vertical Tab) might be common routine for string reading/writing.
        /// But still not sure and this method is called in certain places only.
        /// It should not be called often that's why implementation is so straight.
        /// If I am right one day we should integrate such replacement into common ReadString().
        /// </remarks>
        public string ReadStringConvertSpecialCharacters(bool isLpwstr)
        {
            string result = (isLpwstr) ? mReader.ReadString().Replace("\r", "") : mReader.ReadString();
            return result.Replace("_x000d_", "\r").Replace("_x000D_", "\r").Replace("_x000b_", "\v").Replace("_x000B_", "\v");
        }

        public int ReadStringAsInt()
        {
            return FormatterPal.XmlToInt(ReadString());
        }

        /// <summary>
        /// Recognizes only case-insensitive "true" and "false".
        /// </summary>
        public bool ReadStringAsBool()
        {
            return FormatterPal.ParseBoolTrueFalse(ReadString());
        }

        /// <summary>
        /// Reads the contents of an element or text node as <b>double</b>.
        /// </summary>
        public double ReadStringAsDouble()
        {
            return FormatterPal.ParseDouble(ReadString());
        }

        /// <summary>
        /// Reads the contents, including markup, representing the current node and all its children.
        /// Skips ignorable whitespace after the end till the next node.
        /// </summary>
        public string ReadOuterXml()
        {
            return ReadOuterXml(null, null);
        }

        /// <summary>
        /// Reads the contents, including markup, representing the current node and all its children.
        /// Skips ignorable whitespace after the end till the next node.
        /// Allows providing the <see cref="IXmlUpdater"/> interface to update XML on reading.
        /// Also, allows providing the <see cref="XmlTextReaderNamespaceStorage"/> which populates with
        /// used XML namespaces while reading.
        /// </summary>
        public string ReadOuterXml(IXmlUpdater xmlUpdater, XmlTextReaderNamespaceStorage namespaceStorage)
        {
            // WORDSJAVA-944 Charts in Docx xml markup: xml attribute order changed.
            string result = XmlUtilPal.ReadOuterXml(mReader, xmlUpdater, namespaceStorage);

            // Ideally, we only need to skip ignorable whitespace after the end till the next node.
            // But if there is a line break, then Java reports it and further indentation as text, whereas .NET skips it as whitespace.
            // This is a reasonably safe autoportable hack that has no effect in .NET but helps Java to skip till the next node correctly.
            while ((mReader.NodeType == XmlNodeType.Whitespace) ||
                    (mReader.NodeType == XmlNodeType.Text) ||
                    (mReader.NodeType == XmlNodeType.Comment) || // WORDSNET-7199
                    (mReader.NodeType == XmlNodeType.SignificantWhitespace)) // WORDSNET-13242
            {
                mReader.Read();
            }

            return result;
        }

        /// <summary>
        /// Gets a collection of all namespaces defined locally at the current node.
        /// </summary>
        public IDictionary<string, string> GetNameSpacesDefinedLocally()
        {
#if JAVA
            // WORKAROUND: Java doesn't support XmlTextReader.GetNamespacesInScope()

            Dictionary<string, string> namespaces = new Dictionary<string, string>();

            while (mReader.MoveToNextAttribute())
            {
                if (mReader.Prefix == "xmlns")
                {
                    namespaces.Add(mReader.LocalName, mReader.Value);
                }
            }
            mReader.MoveToElement();
            return namespaces;
#else
            return mReader.GetNamespacesInScope(XmlNamespaceScope.Local);
#endif
        }

        /// <summary>
        /// Get name space by prefix.
        /// </summary>
        public string LookupNamespace(string prefix)
        {
            return mReader.LookupNamespace(prefix);
        }

        /// <summary>
        /// Reads the next node from the reader stream.
        /// </summary>
        public void ReadNextNode()
        {
#if JAVA
            mReader.read();//ExEnd of empty element
            mReader.read();//ExSpace of empty element
            // Check if the current element is an EndElement.
            // If yes, then there is no need to read again to avoid an infinite loop (see WORDSJAVA-2886).
            if(!isEndElement(mReader.getLocalName()))
                mReader.read();//ExEnd of parent element
#else
            WhitespaceHandling savedWhitespaceHandling = mReader.WhitespaceHandling;

            mReader.WhitespaceHandling = WhitespaceHandling.None;
            mReader.MoveToElement();
            mReader.Read();

            mReader.WhitespaceHandling = savedWhitespaceHandling;
#endif
        }

        /// <summary>
        /// Gets the namespace prefix associated with the current node.
        /// </summary>
        public string Prefix
        {
            get { return mReader.Prefix; }
        }

        /// <summary>
        /// Gets the local name of the current node.
        /// </summary>
        public string LocalName
        {
            get { return mReader.LocalName; }
        }

        /// <summary>
        /// Gets the name of the current node including prefix.
        /// </summary>
        public string Name
        {
            get { return mReader.Name; }
        }

        public bool IsEmptyElement
        {
            get { return mReader.IsEmptyElement; }
        }

        /// <summary>
        /// Gets the text value of the current node.
        /// </summary>
        public string Value
        {
            get { return mReader.Value; }
        }

        /// <summary>
        /// Parses the text value of the current node into an integer.
        /// </summary>
        public int ValueAsInt
        {
            get { return FormatterPal.XmlToInt(Value); }
        }

        /// <summary>
        /// Parses the text value of the current node into an unsigned integer.
        /// </summary>
        public uint ValueAsUInt
        {
            get { return FormatterPal.XmlToUInt(Value); }
        }

        /// <summary>
        /// Parses the text value of the current node into a long.
        /// </summary>
        public long ValueAsLong
        {
            get { return FormatterPal.XmlToLong(Value); }
        }

        public double ValueAsDouble
        {
            get { return FormatterPal.ParseDouble(Value); }
        }

        public virtual bool ValueAsBool
        {
            get { return ConvertToBool(Value); }
        }

        /// <summary>
        /// WORDSNET-4391 Int64 values appear in places where we expect Int32, so we truncate values to the lower 32 bits during this resiliency.
        /// </summary>
        public int ValueAsTruncatedInt
        {
            get { return (int)(FormatterPal.ParseInt64(mReader.Value) & 0xFFFFFFFF); }
        }

        public string NamespaceURI
        {
            get { return mReader.NamespaceURI; }
        }

        public XmlTextReader UnderlyingReader
        {
            get { return mReader; }
        }

        /// <summary>
        /// Underlying stream used by the reader.
        /// </summary>
        /// <remarks>
        /// May be null.
        /// </remarks>
        public Stream UnderlyingStream
        {
            get { return mUnderlyingStream; }
        }

        public XmlNodeType NodeType
        {
            get { return mReader.NodeType; }
        }

        private string GetNameInternal()
        {
            return mUseFullElementName
                ? Name
                : LocalName;
        }

        /// <summary>
        /// Underlying stream used by the reader.
        /// </summary>
        private readonly Stream mUnderlyingStream;

        /// <summary>
        /// Inner XmlTextReader. It must be in the mode that returns all whitespace (significant and ignorable).
        /// </summary>
        private readonly XmlTextReader mReader;

        private readonly bool mUseFullElementName;
    }
}
