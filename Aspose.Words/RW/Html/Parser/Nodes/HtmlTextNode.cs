// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/04/2013 by Victor Chebotok

using System.Text;
using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// Represents a text node of an HTML document tree.
    /// </summary>
    internal class HtmlTextNode : HtmlNode
    {
        /// <summary>
        /// Constructor. Initializes a new instance of the class.
        /// </summary>
        /// <param name="text">The text of the node.</param>
        internal HtmlTextNode(string text)
        {
            Debug.Assert(StringUtil.HasChars(text));
            mText = text;
        }

        /// <summary>
        /// Serializes the node to XML text.
        /// </summary>
        /// <param name="result">
        /// The text buffer to which the HTML text of this node will be appended.
        /// </param>
        /// <param name="serializationContext">Serialization context.</param>
        /// <remarks>
        /// This method follows the algorithm described here: http://www.w3.org/TR/html5/syntax.html#serializing-html-fragments
        /// </remarks>
        internal override void SerializeToXml(StringBuilder result, XmlSerializationContext serializationContext)
        {
            for (int i = 0; i < mText.Length; i++)
            {
                char c = mText[i];
                switch (c)
                {
                    case '&':
                        result.Append("&amp;");
                        break;
                    case '\u00A0':
                        // The "&nbsp;" entity is not defined in XML, so a numeric character reference is used instead.
                        result.Append(HtmlConstants.NoBreakSpaceHtmlNumericRef);
                        break;
                    case '<':
                        result.Append("&lt;");
                        break;
                    case '>':
                        result.Append("&gt;");
                        break;
                    default:
                        result.Append(c);
                        break;
                }
            }
        }

        internal override bool IsBoxOrContainsBoxes()
        {
            // Text nodes are not boxes and cannot contain other boxes.
            return false;
        }

        internal override bool ContainsText()
        {
            return mTextHasNonIgnoredCharacters;
        }

        /// <summary>
        /// Gets the text of the node.
        /// </summary>
        internal string Text
        {
            get { return mText; }
        }

        protected override bool BuildNodeBoxLayout(bool isStart, bool preformattedWhitespace)
        {
            if (!isStart)
            {
                // This should never happen since text nodes cannot have children.
                return true;
            }

            // Text nodes cannot be empty; they always contain some characters (whitespace or usual).
            // If whitespace is meaningful (preformatted), any text will be meaningful.
            // If whitespace is ignored, only text that contains characters other than whitespace will be meaningful.
            if (preformattedWhitespace)
            {
                Debug.Assert(StringUtil.HasChars(mText));
                mTextHasNonIgnoredCharacters = true;
            }
            else
            {
                mTextHasNonIgnoredCharacters = HtmlUtil.ContainsAnythingButWhitespaces(mText, true);
            }

            // The return value doesn't really matter here because text nodes cannot have children and therefore cannot be
            // "subtree roots". Let's return "true" to indicate that no special processing is needed.
            return true;
        }

        private readonly string mText;

        private bool mTextHasNonIgnoredCharacters;
    }
}
