// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/08/2024 by Ilya Navrotskiy

using System;
using System.Text;
using Aspose.Common;
using Aspose.Words.RW.Html.Parser;
using Aspose.Words.RW.Markdown.Reader;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown HTML inline block.
    /// </summary>
    internal class HtmlTagBlock : Block
    {
        /// <summary>
        /// Initializes a new instance of <see cref="HtmlTagBlock"/>.
        /// </summary>
        internal HtmlTagBlock(HtmlTagToken htmlTagToken)
        {
            ArgumentUtil.CheckNotNull(htmlTagToken, "htmlTagToken");
            mHtmlTagToken = htmlTagToken;
        }

        /// <summary>
        /// Writes the block into a model using specified context.
        /// </summary>
        internal override void Write(MarkdownReaderContext context)
        {
            // There is no children, so just Open and Close.
            if (mHtmlTagToken.IsStart)
                context.Open(this);
            else
                context.Close(this);
        }

        /// <summary>
        /// Tries to parse a line of text starting from a specified position.
        /// </summary>
        internal override bool TryParse(string txtLine, int start)
        {
            return false;
        }

        /// <summary>
        /// Tries to append a specified block to this block.
        /// </summary>
        internal override bool TryAppend(Block block)
        {
            return false;
        }

#if DEBUG
        public override string ToString()
        {
            return string.Format("HtmlTagBlock: '{0}'", TagName);
        }
#endif

        /// <summary>
        /// Gets a string representing tag name of this <see cref="HtmlTagBlock"/>.
        /// </summary>
        /// The returning value is without any space characters and opening/closing characters of HTML elements,
        /// like 'i', 'b', 'br' and so on.
        internal string TagName
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get
            {
                string tagName = mHtmlTagToken.Name.Replace(MarkdownUtil.SoftLineBreakChar, ' ');
                return StringUtil.RemoveAllWhitespaces(tagName).ToString();
            }
        }

        /// <summary>
        /// Gets a text of this <see cref="HtmlTagBlock"/>.
        /// </summary>
        internal override string Text
        {
            get { return RawText; }
        }

        /// <summary>
        /// Gets string representing a raw text for the HTML inline block.
        /// </summary>
        /// <remarks>
        /// Actually, the right way to get raw text will be implementing a new corresponding
        /// property in <see cref="HtmlTagToken"/>. But for the moment just try to recover it
        /// from the HTML tag parts.
        /// </remarks>
        internal string RawText
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get
            {
                StringBuilder sb = mHtmlTagToken.IsStart ? new StringBuilder("<") : new StringBuilder("</");

                sb.Append(mHtmlTagToken.Name);

                if (StringUtil.HasChars(AttributesText))
                {
                    if (mHtmlTagToken.Name == "http:" || mHtmlTagToken.Name == "http")
                        sb.Append("//");
                    else if (!mHtmlTagToken.Name.EndsWith(":", StringComparison.InvariantCulture))
                        sb.Append(' ');

                    sb.Append(AttributesText);
                }

                if (mHtmlTagToken.IsStart && mHtmlTagToken.IsSelfClosing)
                    sb.Append('/');

                sb.Append('>');

                return sb.ToString();
            }
        }

        /// <summary>
        /// Returns level of the Heading, if tag is one of the 'h0'-'h9', or -1 otherwise.
        /// </summary>
        internal int HeadingTagLevel
        {
            get
            {
                if ((TagName.Length == 2) && TagName.StartsWith("h", StringComparison.InvariantCultureIgnoreCase))
                {
                    int level = FormatterPal.TryParseInt(TagName.Substring(1));
                    if (level != int.MinValue)
                        return level;
                }

                return -1;
            }
        }

        /// <summary>
        /// Returns true, if HTML element is 'p' or one of the 'h0'-'h9'.
        /// </summary>
        internal bool IsParaOrHeading
        {
            get { return (TagName == "p") || (HeadingTagLevel != -1); }
        }

        /// <summary>
        /// Returns true, if HTML element is 'table'.
        /// </summary>
        internal bool IsTable
        {
            get { return (TagName == "table"); }
        }

        /// <summary>
        /// Returns true, if HTML element is 'td'.
        /// </summary>
        internal bool IsCell
        {
            get { return (TagName == "td"); }
        }

        /// <summary>
        /// Gets block type.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.HtmlTag; }
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.Inline; }
        }

        /// <summary>
        /// Gets the value indicating whether this is a start (<c>true</c>) or end (<c>false</c>) tag.
        /// </summary>
        internal bool IsStart
        {
            get { return mHtmlTagToken.IsStart; }
        }

        /// <summary>
        /// Gets the value indicating whether this HTML tag is 'self-closing'.
        /// </summary>
        internal bool IsSelfClosing
        {
            get { return mHtmlTagToken.IsSelfClosing; }
        }

        /// <summary>
        /// Gets a string representing attributes collection.
        /// </summary>
        private string AttributesText
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (HtmlAttribute attribute in mHtmlTagToken.Attributes)
                {
                    sb.Append(attribute.Name);
                    if (StringUtil.HasChars(attribute.Name) && StringUtil.HasChars(attribute.Value))
                        sb.Append('=');

                    if (StringUtil.HasChars(attribute.Value))
                        sb.Append('\"');
                    sb.Append(attribute.Value);
                    if (StringUtil.HasChars(attribute.Value))
                        sb.Append('\"');

                    sb.Append(' ');
                }

                // Remove space separator after very last attribute.
                if (mHtmlTagToken.Attributes.Count > 0)
                    sb.Length--;

                return sb.ToString();
            }
        }

        private readonly HtmlTagToken mHtmlTagToken;
    }
}
