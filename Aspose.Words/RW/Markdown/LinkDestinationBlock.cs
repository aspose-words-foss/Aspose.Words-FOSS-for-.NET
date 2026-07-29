// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/07/2020 by Ilya Navrotskiy

using System;
using Aspose.Images;
using Aspose.Words.RW.HtmlCommon;
using Aspose.Words.RW.Markdown.Reader;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// Represents a markdown LinkDestination block (with title).
    /// </summary>
    internal class LinkDestinationBlock : ContainerBlock
    {
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
            Debug.Assert(IsOpened);

            Add(block);
            return true;
        }

        /// <summary>
        /// Writes the block into a model using specified context.
        /// </summary>
        internal override void Write(MarkdownReaderContext context)
        {
            // A LinkDestination should be never written independently, but only along with a LinkText or ImageDescription,
            // which are always go before it. So, nothing to do here.
        }

        /// <summary>
        /// Returns true, if text between startIndex and endIndex can be a valid Link destination.
        /// </summary>
        internal static bool IsValid(string text, int startIndex, int endIndex)
        {
            int openAngleBracket = MarkdownUtil.IndexOfUnescaped(text, '<', startIndex, endIndex);
            int closeAngleBracket = (openAngleBracket == -1)
                ? -1
                : MarkdownUtil.IndexOfUnescaped(text, '>', openAngleBracket, endIndex);

            int startNonWhitespace = MarkdownUtil.IndexOfNonWhitespace(text, startIndex, endIndex);

            // Check URI.
            int uriStart = ((openAngleBracket > -1) && (closeAngleBracket > -1) && (openAngleBracket <= startNonWhitespace))
                ? openAngleBracket
                : startNonWhitespace;
            if (uriStart == -1)
                // The empty URI is allowed.
                return true;

            int uriEnd = ((openAngleBracket > -1) && (closeAngleBracket > -1) && (openAngleBracket <= startNonWhitespace))
                ? closeAngleBracket
                : MarkdownUtil.IndexOfWhitespace(text, uriStart, endIndex);

            if (uriEnd == -1)
                uriEnd = endIndex;

            if (!IsValidUri(text, uriStart, uriEnd))
                return false;

            // Check Title.
            int titleStart = MarkdownUtil.IndexOfNonWhitespace(text, uriEnd + 1, endIndex);
            // Uri and title must have at least one space between each other.
            if ((titleStart == uriEnd + 1) && (MarkdownUtil.IndexOfWhitespace(text, startIndex, endIndex) < 0))
                return false;
            if (titleStart == -1)
                // The empty title is allowed.
                return true;

            if (!IsValidTitle(text, titleStart, endIndex))
                return false;

            return true;
        }

        /// <summary>
        /// Returns true, if text between specified delimiters can be a valid Link destination.
        /// </summary>
        internal static bool IsValid(Delimiter opening, Delimiter closing)
        {
            Debug.Assert((opening != null) && (closing != null));
            Debug.Assert(opening.IsBefore(closing));

            return IsValidSvgDataUrl(opening.Text.Substring(opening.End + 1, (closing.Start - 1) - opening.End)) ||
                   IsValid(opening.Text, opening.End + 1, closing.Start - 1);
        }

        /// <summary>
        /// Returns true, if a specified string is valid Link URI.
        /// </summary>
        private static bool IsValidUri(string text, int uriStart, int uriEnd)
        {
            // There are two rules for the URI. First one is when URI is enclosed in '<' and '>'
            // and the second one when it is not.
            if (IsInAngleBrackets(text, uriStart, uriEnd))
            {
                if (MarkdownUtil.HasNonEscapedCharacters(text, uriStart + 1, uriEnd - 1, '<', '>'))
                    return false;

                return text.IndexOf(MarkdownUtil.SoftLineBreakChar, uriStart, uriEnd-uriStart ) == -1;
            }
            else
            {
                if (text.Length == 0)
                    return false;

                if (text[uriStart] == '<')
                    return false;

                for (int i = uriStart; i < uriEnd; i++)
                {
                    char ch = text[i];
                    if (MarkdownUtil.IsAsciiControlChar(ch))
                        return false;

                    if (ch == MarkdownUtil.SpaceChar)
                        return false;

                    if (ch == MarkdownUtil.SoftLineBreakChar)
                        return false;

                    if ((ch == MarkdownUtil.HardLineBreakSlashChar) || (ch == MarkdownUtil.HardLineBreakSpacesChar))
                        return false;
                }

                return MarkdownUtil.AreBalanced(text, uriStart, uriEnd, '(', ')');
            }
        }

        /// <summary>
        /// Returns true, if a specified text started and ended with the specified positions can be a valid Link title.
        /// </summary>
        private static bool IsValidTitle(string text, int titleStart, int titleEnd)
        {
            Debug.Assert(text != null);

            // Trim whitespace characters from the end.
            while ((titleEnd >= 0) && MarkdownUtil.IsWhitespaceCharacter(text[titleEnd]))
            {
                titleEnd--;
            }

            int length = (titleEnd - titleStart) + 1;

            // A link title can be empty.
            if (length < 1)
                return true;

            // If link title is not empty, then it must be wrapped into opening and closing delimiters.
            if (length < 2)
                return false;

            // Check link title has opening delimiter.
            char openingDelimiter = text[titleStart];
            if (!IsTitleOpening(openingDelimiter))
                return false;

            // Check closing delimiter is not escaped.
            if (text[titleEnd - 1] == ControlChar.BackslashChar)
                return false;

            // Check link title has closing delimiter.
            char closingDelimiter = text[titleEnd];
            if (!IsTitleClosing(closingDelimiter))
                return false;

            // Check that closing delimiter corresponds to an opening one.
            char expectedClosingDelimiter = (openingDelimiter == '(') ? ')' : openingDelimiter;
            if (closingDelimiter != expectedClosingDelimiter)
                return false;

            // At last, check there is no unescaped opening or closing delimiters.
            if (MarkdownUtil.HasNonEscapedCharacters(text, titleStart + 1, titleEnd - 1, openingDelimiter, closingDelimiter))
                return false;

            return true;
        }

        /// <summary>
        /// Returns true, if the text is in the angle brackets.
        /// </summary>
        private static bool IsInAngleBrackets(string text, int startIndex, int endIndex)
        {
            Debug.Assert(text != null);

            int length = (endIndex - startIndex) + 1;
            // There should be at least `<` and `>`.
            if (length < 2)
                return false;

            // Check closing `>` is not escaped.
            if (text[endIndex - 1] == ControlChar.BackslashChar)
                return false;

            return ((text[startIndex] == '<') && (text[endIndex] == '>'));
        }

        /// <summary>
        /// Returns true, if a specified character is valid link title opening delimiter.
        /// </summary>
        private static bool IsTitleOpening(char c)
        {
            return (c == '\"') || (c == '\'') || (c == '(');
        }

        /// <summary>
        /// Returns true, if a specified character is valid link title closing delimiter.
        /// </summary>
        private static bool IsTitleClosing(char c)
        {
            return (c == '\"') || (c == '\'') || (c == ')');
        }

        /// <summary>
        /// Returns true, if this link destination is an Svg image embedded as a data Url.
        /// </summary>
        internal bool IsSvgDataUrl
        {
            get { return IsValidSvgDataUrl(base.Text); }
        }

        /// <summary>
        /// Returns true, if a specified text is valid Svg data Url.
        /// </summary>
        private static bool IsValidSvgDataUrl(string text)
        {
            DataUrl dataUrl = DataUrl.Parse(text);
            if (dataUrl != null)
            {
                byte[] dataBytes = dataUrl.GetDataInUtf8();
                if (dataBytes.Length > 0)
                    return ImageUtil.IsSvg(dataBytes);
            }

            return false;
        }

        /// <summary>
        /// Returns unescaped URI.
        /// </summary>
        private string GetUnescapedUri()
        {
            string text = base.Text;
            int endIndex = text.Length - 1;

            if (IsValidSvgDataUrl(text))
                return text.Replace("\v", "%20");

            int openAngleBracket = MarkdownUtil.IndexOfUnescaped(text, '<', 0, endIndex);
            int closeAngleBracket = (openAngleBracket == -1)
                ? -1
                : MarkdownUtil.IndexOfUnescaped(text, '>', openAngleBracket, endIndex);
            int startNonWhitespace = MarkdownUtil.IndexOfNonWhitespace(text, 0, endIndex);

            if ((openAngleBracket > -1) && (closeAngleBracket > -1) && (openAngleBracket <= startNonWhitespace))
            {
                text = text.Substring(openAngleBracket + 1, (closeAngleBracket - openAngleBracket) - 1);
                return text;
            }

            // Skip leading whitespace characters in Uri.
            int uriStart = startNonWhitespace;
            // Uri can be empty.
            if (uriStart == -1)
                return "";

            int uriEnd = MarkdownUtil.IndexOfWhitespace(text, uriStart, endIndex) - 1;
            if (uriEnd < 0)
                uriEnd = endIndex;

            // Valid URI can be surrounded with '<' and '>'.
            // Remove them to get `pure` text of URI.
            if (IsInAngleBrackets(text, uriStart, uriEnd))
            {
                uriStart++;
                uriEnd--;
            }

            return text.Substring(uriStart, (uriEnd - uriStart) + 1);
        }

        /// <summary>
        /// A type of the block.
        /// </summary>
        internal override BlockType Type
        {
            get { return BlockType.LinkDestination; }
        }

        /// <summary>
        /// Gets block level.
        /// </summary>
        internal override MarkdownBlockLevel BlockLevel
        {
            get { return MarkdownBlockLevel.Inline; }
        }

        /// <summary>
        /// Gets block text.
        /// </summary>
        internal override string Text
        {
            get
            {
                // The spec editor as well as GitLab are both ignore a link destination text inside ImageDescription.
                return (GetParent(BlockType.ImageDescription) != null) ? "" : base.Text;
            }
        }

        /// <summary>
        /// Gets a string value representing an URI of the link destination.
        /// </summary>
        internal string Uri
        {
            get
            {
                URI uri = new URI(MarkdownUtil.UnescapeMarkupSymbols(GetUnescapedUri()));
                return uri.ToString(true);
            }
        }

        /// <summary>
        /// Gets a string value representing a Title of the link destination.
        /// </summary>
        internal string Title
        {
            get
            {
                // Remove Uri string from the destination text.
                string text = base.Text;
                if (IsValidSvgDataUrl(text))
                    return "";
                string uri = GetUnescapedUri();
                if (uri.Length == 0)
                    return "";
                int uriPos = text.IndexOf(uri, StringComparison.Ordinal);
                if (uriPos < 0)
                    return "";
                text = text.Substring(uriPos + uri.Length - 1);

                int endIndex = text.Length - 1;
                // Skip leading whitespace characters in Uri.
                int uriStart = MarkdownUtil.IndexOfNonWhitespace(text, 0, endIndex);
                // In case of Uri is empty, the title is empty too.
                if (uriStart == -1)
                    return "";

                int uriEnd = MarkdownUtil.IndexOfWhitespace(text, uriStart, endIndex) - 1;
                // There can be only Uri without Title in this destination.
                if (uriEnd < 0)
                    return "";

                int titleStart = MarkdownUtil.IndexOfNonWhitespace(text, uriEnd + 1, endIndex);
                // The title can be empty.
                if (titleStart == -1)
                    return "";

                // Skip whitespace characters from the end.
                while (MarkdownUtil.IsWhitespaceCharacter(text[endIndex]))
                    endIndex--;

                // Valid title is always inside surrounding opening and closing delimiters `"`, `'`, `(`.
                // Remove them to get `pure` text of title.
                string title = text.Substring(titleStart + 1, endIndex - titleStart - 1);

                // WORDSNET-18341 Line breaks are allowed in link text. Preserve it by
                // replacing with LineBreak, otherwise it will be removed in Block.Write().
                title = title.Replace(MarkdownUtil.SoftLineBreakChar, ControlChar.LineBreakChar);

                return MarkdownUtil.UnescapeQuotesAndParentheses(title).Trim();
            }
        }

        /// <summary>
        /// Opening delimiter for LinkTextBlock.
        /// </summary>
        [CppConstexpr]
        internal const char OpeningDelimiter = LinkDestinationOpeningDelimiter.Character;

        /// <summary>
        /// Closing delimiter for LinkTextBlock.
        /// </summary>
        [CppConstexpr]
        internal const char ClosingDelimiter = LinkDestinationClosingDelimiter.Character;
    }
}
