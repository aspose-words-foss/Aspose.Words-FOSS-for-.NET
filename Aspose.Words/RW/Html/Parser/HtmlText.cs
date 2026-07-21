// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/04/2013 by Victor Chebotok

using System.Text;
using Aspose.Bidi;
using Aspose.Words.RW.HtmlCommon;

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// Processes text tokens during construction of HTML trees.
    /// </summary>
    /// <remarks>
    /// The HTML tree constructor works with text strings, but HTML Specification describes processing of text
    /// in terms of characters. This class helps to manipulate characters of text strings.
    /// </remarks>
    internal class HtmlText
    {
        internal HtmlText()
        {
            mText = new StringBuilder();
        }

        internal HtmlText(string text)
        {
            mText = new StringBuilder(text);
        }

        internal void Clear()
        {
            mText.Length = 0;
        }

        internal void Append(HtmlText text)
        {
            mText.Append(text.mText);
        }

        internal void RemoveLeadingLineFeedChar()
        {
            if ((mText.Length > 0) && (mText[0] == '\n'))
            {
                mText.Remove(0, 1);
            }
        }

        internal bool ContainsAnythingButWhitespace()
        {
            for (int i = 0; i < mText.Length; i++)
            {
                if (!HtmlUtil.IsWhitespace(mText[i]))
                {
                    return true;
                }
            }
            return false;
        }

        internal bool ContainsAnythingButWhitespaceOrNull()
        {
            for (int i = 0; i < mText.Length; i++)
            {
                char c = mText[i];
                if ((c != '\0') && (!HtmlUtil.IsWhitespace(c)))
                {
                    return true;
                }
            }
            return false;
        }

        internal void TrimLeadingWhitespace()
        {
            int leadingWhitespaceCount = CountLeadingWhitespace();
            if (leadingWhitespaceCount > 0)
            {
                mText.Remove(0, leadingWhitespaceCount);
            }
        }

        internal void StripNullCharacters()
        {
            mText.Replace("\0", null);
        }

        internal void ReplaceNullCharacters()
        {
            mText.Replace('\0', UnicodeUtil.ReplacementChar);
        }

        internal string GetLeadingWhitespace()
        {
            int leadingWhitespaceCount = CountLeadingWhitespace();
            if (leadingWhitespaceCount > 0)
            {
                char[] whitespace = new char[leadingWhitespaceCount];
                mText.CopyTo(0, whitespace, 0, leadingWhitespaceCount);
                return new string(whitespace);
            }
            else
            {
                return string.Empty;
            }
        }

        internal string GetAllWhitespaces()
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < mText.Length; i++)
            {
                char c = mText[i];
                if (HtmlUtil.IsWhitespace(c))
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        internal string GetAsString()
        {
            return mText.ToString();
        }

        internal bool IsEmpty
        {
            get { return mText.Length == 0; }
        }

        private int CountLeadingWhitespace()
        {
            int result = 0;
            while ((result < mText.Length) && HtmlUtil.IsWhitespace(mText[result]))
            {
                ++result;
            }
            return result;
        }

        private readonly StringBuilder mText;
    }
}
