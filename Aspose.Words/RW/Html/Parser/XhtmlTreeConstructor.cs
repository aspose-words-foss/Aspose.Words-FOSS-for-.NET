// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/09/2021 by Artem Shabarshin

using System.IO;
using System.Xml;
using Aspose.Xml;

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// Constructs an HTML document tree according to the XHTML specification.
    /// </summary>
    /// <remarks>
    /// See https://www.w3.org/TR/xhtml1/
    /// </remarks>
    internal class XhtmlTreeConstructor
    {
        /// <summary>
        /// Parses an XHTML document and returns the HTML tree.
        /// </summary>
        /// <param name="stream">The stream with data of the XHTML document.</param>
        /// <returns>The root element of the document's HTML tree.</returns>
        internal static HtmlDocument Construct(Stream stream)
        {
            XhtmlTreeConstructor constructor = new XhtmlTreeConstructor();
            constructor.Run(stream);
            return new HtmlDocument(constructor.mRootElement, HtmlDocumentMode.Standards);
        }

        /// <summary>
        /// Parses XHTML from the stream into an HTML tree.
        /// </summary>
        private void Run(Stream stream)
        {

            // Let the parser expand entities for us. Note that support for external entities is turned off,
            // like in Adobe Digital Editions. This is done by specifying a null XML resolver when creating the reader.
            XhtmlXmlResolver xmlResolver = new XhtmlXmlResolver();
            XmlTextReader reader = XmlUtilPal.CreateXmlTextReader(stream, xmlResolver);

            while (reader.Read())
            {
                xmlResolver.SetCurrentNodeType(reader.NodeType);
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                    {
                        HtmlAttributeCollection attributes = new HtmlAttributeCollection();
                        if (reader.HasAttributes)
                        {
                            while (reader.MoveToNextAttribute())
                            {
                                attributes.Add(new HtmlAttribute(reader.Name, reader.Value));
                            }
                        }
                        reader.MoveToElement();
                        ProcessStartTag(reader.Name, reader.NamespaceURI, reader.IsEmptyElement, attributes);
                        break;
                    }
                    case XmlNodeType.EndElement:
                    {
#if JAVA
                        //Java XmlTextReader doesn't understand self-closing tags by default. Let's help him a little.
                        if (IsSelfClosingTag(reader.Name))
                            break;
#endif

                        ProcessEndTag(reader.Name, reader.NamespaceURI);
                        break;
                    }
                    case XmlNodeType.Text:
                    case XmlNodeType.Whitespace:
                    case XmlNodeType.CDATA:
                    case XmlNodeType.SignificantWhitespace:
                    {
                        // Currently, we had to replace all CR control characters because HTML parser
                        // doesn't handle this character in accordance with parsing HTML documents.
                        string text = reader.Value.Replace("\r\n", "\n").Replace("\r", "\n");
                        ProcessText(text);
                        break;
                    }
                    default:
                    {
                        // Discard other types of nodes.
                        break;
                    }
                }
            }
        }

        private void ProcessStartTag(
            string name,
            string ns,
            bool isSelfClosing,
            HtmlAttributeCollection attributes)
        {
            // A XHTML document must have only one root. An XML exception will be thrown by the XML parser otherwise.
            if (mRootElement == null)
            {
                // According to the specification, the first tag in a XHTML document must be 'html' but
                // we don't check this here. HTML reader can resolve it better.
                HtmlElementNode htmlElement = CreateElement(name, ns, attributes);
                mRootElement = htmlElement;
                mOpenedElements.Add(htmlElement);
            }
            else if (!mOpenedElements.IsEmpty)
            {
                InsertHtmlElement(name, ns, attributes);
            }

            if (isSelfClosing)
            {
                ProcessEndTag(name, ns);
            }
        }

        private void ProcessEndTag(string name, string ns)
        {
            Debug.Assert(!mOpenedElements.IsEmpty);

#if JAVA
            // In Java, some tags are handled differently than in .NET.
            // Therefore, situations with empty mOpenedElements can occur
            if (mOpenedElements.IsEmpty)
                return;
#endif

            HtmlElementNode poppedElement = mOpenedElements.GetLast();
            Debug.Assert(poppedElement.Name == name);
            Debug.Assert(poppedElement.Namespace == ns);
            mOpenedElements.RemoveLast();
        }

        private void InsertHtmlElement(
            string name,
            string ns,
            HtmlAttributeCollection attributes)
        {
            InsertElement(name, ns, attributes);
        }

        private void InsertElement(
            string name,
            string ns,
            HtmlAttributeCollection attributes)
        {
            HtmlElementNode element = CreateElement(name, ns, attributes);

            InsertIntoCurrentNode(element);
            mOpenedElements.Add(element);
        }

        private void ProcessText(string text)
        {
            // Discard content outside or directly inside HTML node.
            if (!mOpenedElements.IsEmpty && (mOpenedElements.GetLast().Name != "html"))
            {
                HtmlTextNode textNode = new HtmlTextNode(text);
                InsertIntoCurrentNode(textNode);
            }
        }

        private void InsertIntoCurrentNode(HtmlNode node)
        {
            Debug.Assert(node != null);

            HtmlElementNode currentNode = mOpenedElements.GetLast();
            currentNode.Children.Add(node);
        }

        private static HtmlElementNode CreateElement(
            string name,
            string ns,
            HtmlAttributeCollection attributes)
        {
            return new HtmlElementNode(name, ns, attributes);
        }

        /// <summary>
        /// The root element of the document's HTML tree.
        /// </summary>
        private HtmlElementNode mRootElement;

        /// <summary>
        /// The list of HTML elements that are still opened. The most recently opened element comes last.
        /// </summary>
        private readonly HtmlOpenedElementList mOpenedElements = new HtmlOpenedElementList();

        private bool IsSelfClosingTag(string inputTag)
        {
            for (int i = 0; i < mSelfClosingTag.Length; i++)
            {
                if (inputTag.Equals(mSelfClosingTag[i], System.StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private string[]  mSelfClosingTag =  new string[] {"area", "base", "br", "col", "embed", "hr", "img",
                                                           "input", "link", "meta", "param", "source", "track",
                                                           "wbr", "script", "command", "keygen", "menuitem"};
    }
}
