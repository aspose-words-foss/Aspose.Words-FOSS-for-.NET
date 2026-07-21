// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2013 by Victor Chebotok

using System.IO;
using System.Text;
using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// Represents a complete HTML document.
    /// </summary>
    internal class HtmlDocument
    {
        /// <summary>
        /// Constructor. Initializes a new instance of the class.
        /// </summary>
        /// <param name="root">The root node of the document's HTML tree. May not be <c>null</c>.</param>
        /// <param name="mode">The quirks mode of the document.</param>
        internal HtmlDocument(HtmlElementNode root, HtmlDocumentMode mode)
        {
            Debug.Assert(root != null);

            Root = root;
            Mode = mode;
            DefaultLanguage = GetDefaultLanguageFromMetaElements(Root);
        }

        /// <summary>
        /// Loads an HTML document from a stream with the specified encoding.
        /// If the document has BOM, it will take precedence over the specified encoding.
        /// </summary>
        /// <param name="stream">The input stream. May not be <c>null</c>.</param>
        /// <param name="encoding">The input encoding. May not be <c>null</c>.</param>
        /// <param name="supportedFeatures">
        /// Features that are considered supported when loading conditional parts (IE conditional expressions).
        /// Setting this parameter to <c>null</c> disables support for conditional parts, like in modern browers.
        /// </param>
        /// <param name="isScriptingEnabled">Indicates whether support for scripts is enabled when parsing HTML.</param>
        /// <param name="supportSelfClosingNonHtmlTags">
        /// Indicates whether to support certain self-closing non-HTML tags.
        /// </param>
        internal static HtmlDocument Load(
            Stream stream,
            Encoding encoding,
            Features supportedFeatures,
            bool isScriptingEnabled,
            bool supportSelfClosingNonHtmlTags)
        {
            Debug.Assert(stream != null);
            Debug.Assert(encoding != null);

            return Load(
                new StreamReader(stream, encoding),
                supportedFeatures,
                isScriptingEnabled,
                supportSelfClosingNonHtmlTags);
        }

        /// <summary>
        /// Loads an HTML document from a file encoded in UTF-8.
        /// </summary>
        /// <param name="filePath">Path to an HTML file to load from.</param>
        /// <remarks>
        /// Support for IE conditional expressions is disabled.
        /// Scripting is disabled.
        /// Self-closing non-HTML tags are not supported.
        /// </remarks>
        internal static HtmlDocument LoadFromFile(string filePath)
        {
            Debug.Assert(StringUtil.HasChars(filePath));

            using (StreamReader reader = new StreamReader(filePath, Encoding.UTF8))
            {
                return Load(reader, null, false, false);
            }
        }

        /// <summary>
        /// Loads the HTML document from the specified string.
        /// </summary>
        /// <param name="html">
        /// String containing the HTML document to load. May not be <c>null</c>.
        /// </param>
        /// <remarks>
        /// Support for IE conditional expressions is disabled.
        /// Scripting is disabled.
        /// Self-closing non-HTML tags are not supported.
        /// </remarks>
        internal static HtmlDocument Load(string html)
        {
            return Load(html, null, false, false);
        }

        /// <summary>
        /// Loads the HTML document from the specified UTF-8 stream.
        /// </summary>
        /// <param name="stream">
        /// Stream containing the HTML document to load. May not be <c>null</c>.
        /// </param>
        /// <remarks>
        /// Support for IE conditional expressions is disabled.
        /// Scripting is disabled.
        /// Self-closing non-HTML tags are not supported.
        /// </remarks>
        internal static HtmlDocument Load(Stream stream)
        {
            return Load(stream, Encoding.UTF8, null, false, false);
        }

        /// <summary>
        /// Loads the HTML document from the specified string.
        /// </summary>
        /// <param name="html">
        /// String containing the HTML document to load. May not be <c>null</c>.
        /// </param>
        /// <param name="supportedFeatures">
        /// Features that are considered supported when loading conditional parts (IE conditional expressions).
        /// Setting this parameter to <c>null</c> disables support for conditional parts, like in modern browers.
        /// </param>
        /// <param name="isScriptingEnabled">Indicates whether support for scripts is enabled when parsing HTML.</param>
        /// <param name="supportSelfClosingNonHtmlTags">
        /// Indicates whether to support certain self-closing non-HTML tags.
        /// </param>
        internal static HtmlDocument Load(
            string html,
            Features supportedFeatures,
            bool isScriptingEnabled,
            bool supportSelfClosingNonHtmlTags)
        {
            Debug.Assert(html != null);

            using (StringReader reader = new StringReader(html))
            {
                return Load(reader, supportedFeatures, isScriptingEnabled, supportSelfClosingNonHtmlTags);
            }
        }

        /// <summary>
        /// Loads an XHTML document from a stream.
        /// </summary>
        internal static HtmlDocument LoadXhtml(Stream stream)
        {
            HtmlDocument result = XhtmlTreeConstructor.Construct(stream);

            // Recalculate boxes of HTML elements.
            result.Root.BuildBoxLayout();

            return result;
        }

        /// <summary>
        /// Gets the root node of the document's HTML tree.
        /// </summary>
        internal HtmlElementNode Root { get; }

        /// <summary>
        /// Gets the quirks mode of the document.
        /// </summary>
        internal HtmlDocumentMode Mode { get; }

        /// <summary>
        /// Gets the default language of the document, which is set through the &lt;meta&gt; element.
        /// If no default language is set for the document, this value is an empty string.
        /// </summary>
        /// <remarks>
        /// See http://www.w3.org/TR/html5/document-metadata.html#attr-meta-http-equiv-content-language
        /// </remarks>
        internal string DefaultLanguage { get; }

        /// <summary>
        /// Loads the HTML document from the specified TextReader.
        /// </summary>
        /// <param name="reader">The TextReader used to feed the HTML data into the document. May not be <c>null</c>.</param>
        /// <param name="supportedFeatures">
        /// Features that are considered supported when reading IE conditional comments.
        /// Setting this parameter to <c>null</c> disables support for conditional comments, like in modern browers.
        /// </param>
        /// <param name="isScriptingEnabled">Indicates whether support for scripts is enabled when parsing HTML.</param>
        /// <param name="supportSelfClosingNonHtmlTags">
        /// Indicates whether to support certain self-closing non-HTML tags.
        /// </param>
        private static HtmlDocument Load(
            TextReader reader,
            Features supportedFeatures,
            bool isScriptingEnabled,
            bool supportSelfClosingNonHtmlTags)
        {
            Debug.Assert(reader != null);

            HtmlDocument result = HtmlTreeConstructor.Construct(
                new HtmlTokenizer(reader.ReadToEnd(), supportedFeatures),
                isScriptingEnabled,
                supportSelfClosingNonHtmlTags);

            // Recalculate boxes of HTML elements.
            result.Root.BuildBoxLayout();

            return result;
        }

        /// <summary>
        /// Gets the default language of the document from &lt;meta&gt; elements.
        /// </summary>
        /// <param name="root">The element from which tree traversal is to be started.</param>
        /// <returns>The default language of the document. If no default language is set, an empty string is returned.</returns>
        /// <remarks>
        /// The method performs the non-recursive depth-first tree traversal and searches for &lt;meta&gt; elements that declare
        /// the document language (pragma-set default language). The language declared by the last such element is returned.
        /// </remarks>
        private static string GetDefaultLanguageFromMetaElements(HtmlElementNode root)
        {
            Debug.Assert(root != null);
            string result = string.Empty;

            HtmlTreeEnumerator enumerator = new HtmlTreeEnumerator(root);
            while (enumerator.MoveNext())
            {
                HtmlElementNode elementNode = enumerator.Current as HtmlElementNode;
                if (enumerator.IsStart && (elementNode != null))
                {
                    string furtherResult = GetPragmaSetDefaultLanguageValue(elementNode);
                    if (furtherResult != string.Empty)
                    {
                        result = furtherResult;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets language declaration from the element if it is a &lt;meta&gt; element that declares the document language
        /// (a pragma-set default language).
        /// </summary>
        /// <param name="element">An element of the HTML tree.</param>
        /// <returns>
        /// The language declared by the element. If the element is not a &lt;meta&gt; element or if it does not
        /// contain a valid language declaration, an empty string is returned.
        /// </returns>
        /// <remarks>
        /// The algorithm is described here:
        /// http://www.w3.org/TR/html5/document-metadata.html#attr-meta-http-equiv-content-language
        /// </remarks>
        private static string GetPragmaSetDefaultLanguageValue(HtmlElementNode element)
        {
            if (element.Name != "meta")
            {
                return string.Empty;
            }

            HtmlAttribute httpEquivAttribute = element.Attributes["http-equiv"];
            if ((httpEquivAttribute == null) ||
                (!StringUtil.EqualsIgnoreCase(httpEquivAttribute.Value, "content-language")))
            {
                return string.Empty;
            }

            HtmlAttribute contentAttribute = element.Attributes["content"];
            if (contentAttribute == null)
            {
                return string.Empty;
            }

            string content = contentAttribute.Value;
            if (StringUtil.Contains(content, ",", false))
            {
                return string.Empty;
            }

            int startIndex = 0;
            while ((startIndex < content.Length) && HtmlUtil.IsWhitespace(content[startIndex]))
            {
                ++startIndex;
            }

            int length = 0;
            while (((startIndex + length) < content.Length) && (!HtmlUtil.IsWhitespace(content[startIndex + length])))
            {
                ++length;
            }

            return content.Substring(startIndex, length);
        }
    }
}
