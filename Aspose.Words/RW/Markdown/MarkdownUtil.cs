// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2019 by Ilya Navrotskiy

using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Common;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Markdown
{
    /// <summary>
    /// An utility class to work with a markdown.
    /// </summary>
    internal static class MarkdownUtil
    {
        /// <summary>
        /// Converts a specified level to a SetextHeading character.
        /// </summary>
        internal static char LevelToSetextHeadingChar(int level)
        {
            switch (level)
            {
                case 1:
                    return SetextHeadingBlock.Heading1Char;
                case 2:
                    return SetextHeadingBlock.Heading2Char;
                default:
                    throw new InvalidOperationException(string.Format("Invalid level for a SetextHeading: '{0}'", level));
            }
        }

        /// <summary>
        /// Converts a specified character to a corresponding <see cref="ListMarker"/> enumeration.
        /// </summary>
        internal static ListMarker ToListMarker(char c)
        {
            if (char.IsWhiteSpace(c))
                return ListMarker.None;

            switch (c)
            {
                case '.':
                    return ListMarker.Dot;
                case ')':
                    return ListMarker.Parenthesis;
                case '+':
                    return ListMarker.Plus;
                case '*':
                    return ListMarker.Asterisk;
                default:
                    return ListMarker.Minus;
            }
        }

        /// <summary>
        /// Converts a specified character to a corresponding <see cref="ListMarker"/> enumeration
        /// in respect of it is an ordered list.
        /// </summary>
        internal static ListMarker ToOrderedListMarker(char c)
        {
            if (char.IsWhiteSpace(c))
                return ListMarker.None;

            switch (c)
            {
                case ')':
                    return ListMarker.Parenthesis;
                default:
                    return ListMarker.Dot;
            }
        }

        /// <summary>
        /// Converts a specified character to a corresponding <see cref="ListMarker"/> enumeration
        /// in respect of it is a bullet list.
        /// </summary>
        internal static ListMarker ToBulletListMarker(char c)
        {
            if (char.IsWhiteSpace(c))
                return ListMarker.None;

            switch (c)
            {
                case '+':
                    return ListMarker.Plus;
                case '*':
                    return ListMarker.Asterisk;
                default:
                    return ListMarker.Minus;
            }
        }

        /// <summary>
        /// Converts a specified <see cref="ListMarker"/> enumeration to a corresponding char.
        /// </summary>
        internal static char ListMarkerToChar(ListMarker marker)
        {
            switch (marker)
            {
                case ListMarker.Dot:
                    return '.';
                case ListMarker.Parenthesis:
                    return ')';
                case ListMarker.Minus:
                    return '-';
                case ListMarker.Plus:
                    return '+';
                case ListMarker.Asterisk:
                    return '*';
                case ListMarker.None:
                    return ControlChar.SpaceChar;
                default:
                    throw new InvalidOperationException(string.Format("Unknown list marker: '{0}'", marker));
            }
        }

        /// <summary>
        /// Converts a specified style to a block type.
        /// </summary>
        internal static BlockType ToBlockType(Style style)
        {
            if (style != null)
            {
                if (style.Name.StartsWith(QuoteStyleName, StringComparison.Ordinal))
                    return BlockType.Quote;

                if (style.Name.StartsWith(ListStyleName, StringComparison.Ordinal))
                    return BlockType.BulletListItem;

                if (style.Name.StartsWith(IndentedCodeStyleName, StringComparison.Ordinal))
                    return BlockType.IndentedCode;

                if (style.Name.StartsWith(FencedCodeStyleName, StringComparison.Ordinal))
                    return BlockType.FencedCode;

                if (style.Name.StartsWith(SetextHeadingStyleName, StringComparison.Ordinal))
                    return BlockType.SetextHeading;

                if (style.Name.StartsWith(InlineCodeStyleName, StringComparison.Ordinal))
                    return BlockType.InlineCode;

                if (style.IsHeading)
                    return BlockType.AtxHeading;
            }

            return BlockType.Paragraph;
        }

        /// <summary>
        /// Returns true, if a specified character is one of the ordered list markers ('.', ')').
        /// </summary>
        internal static bool IsOrderedListMarker(char c)
        {
            return (c == '.') || (c == ')');
        }

        /// <summary>
        /// Returns true if a specified line can be a bullet list item.
        /// </summary>
        internal static bool IsBulletListItem(StringBuilder line)
        {
            Debug.Assert(line != null);

            // There must be at least list marker followed by whitespace character.
            if (line.Length < 2)
                return false;

            if (!IsBulletListMarker(line[0]))
                return false;

            if (!StringUtil.IsWhiteSpace(line[1]))
                return false;

            return true;
        }

        /// <summary>
        /// Returns true if a specified line can be an ordered list item.
        /// </summary>
        internal static bool IsOrderedListItem(StringBuilder line)
        {
            Debug.Assert(line != null);

            // The ordered list must start from a digits.
            int i = 0;
            while ((i < line.Length) && StringUtil.IsDigit(line[i]))
                i++;

            // The length of ordered list number must be less than 10.
            if ((i == 0) || (i >= 10))
                return false;

            // There must be at least ordered list marker followed by whitespace character.
            if ((i + 1) >= line.Length)
                return false;

            if (!IsOrderedListMarker(line[i]))
                return false;

            if (!StringUtil.IsWhiteSpace(line[i + 1]))
                return false;

            return true;
        }

        /// <summary>
        /// Returns true, if a text between specified characters is intraword.
        /// </summary>
        internal static bool IsIntraword(char prevChar, char nextChar)
        {
            return (char.IsLetterOrDigit(prevChar) && char.IsLetterOrDigit(nextChar));
        }

        /// <summary>
        /// Gets a flanking type.
        /// </summary>
        /// <remarks>
        /// See 6.4 Emphasis and strong emphasis at https://spec.commonmark.org for details.
        /// </remarks>
        internal static FlankingType GetFlankingType(char prevChar, char nextChar)
        {
            bool isLeftFlanking = IsLeftFlanking(prevChar, nextChar);
            bool isRightFlanking = IsRightFlanking(prevChar, nextChar);

            if (isLeftFlanking && isRightFlanking)
                return FlankingType.Both;

            if (isLeftFlanking)
                return FlankingType.Left;

            if (isRightFlanking)
                return FlankingType.Right;

            return FlankingType.None;
        }

        /// <summary>
        /// Normalizes InlineCode block.
        /// </summary>
        /// <remarks>
        ///  See chapter 6.3 of markdown specification at https://spec.commonmark.org
        ///  Note! Only spaces, and not Unicode whitespace in general, are stripped in this way
        /// (see `Example 333` of specification that is TestInlineCodeF()).
        /// </remarks>
        internal static string Normalize(string text)
        {
            // First, line endings are converted to spaces.
            string normalizedText = text.Replace(SoftLineBreakChar, ControlChar.SpaceChar);

            // If the resulting string both begins and ends with a space character, but does not consist entirely
            // of space characters, a single space character is removed from the front and back.
            if (!StringUtil.ContainsOnlyWhitespaces(normalizedText) &&
                (normalizedText[0] == ControlChar.SpaceChar) &&
                (normalizedText[normalizedText.Length - 1] == ControlChar.SpaceChar))
                normalizedText = normalizedText.Substring(1, normalizedText.Length - 2);

            return normalizedText;
        }

        /// <summary>
        /// Returns length of a specified character.
        /// </summary>
        internal static int GetLength(char c)
        {
            return (c == ControlChar.TabChar) ? TabSize : 1;
        }

        /// <summary>
        /// Returns length of a specified string considering tabs.
        /// </summary>
        internal static int GetLength(string value)
        {
            if (!StringUtil.HasChars(value))
                return 0;

            int length = 0;
            foreach (char c in value)
                length += GetLength(c);

            return length;
        }

        /// <summary>
        /// Gets the number of characters that should be taken from a specified text to make
        /// their total length become not less than a specified required length, considering tabs.
        /// </summary>
        /// <remarks>
        /// This is a reverse operation to <see cref="GetLength(string)"/>.
        /// </remarks>
        internal static int GetCharsCount(string text, int start, int requiredLength)
        {
            if (!StringUtil.HasChars(text))
                return 0;

            int startDiff = start;
            while ((requiredLength > 0) && (startDiff < text.Length))
            {
                requiredLength -= GetLength(text[startDiff]);
                startDiff++;
            }

            return startDiff - start;
        }

        /// <summary>
        /// Returns text with the backslash-escaped double quotes.
        /// </summary>
        internal static string EscapeDoubleQuotes(string text)
        {
            return (text == null)
                ? null
                : text.Replace(EscapedDoubleQuote, DoubleQuote).Replace(DoubleQuote, EscapedDoubleQuote);
        }

        /// <summary>
        /// Returns text with unescaped quotes and right parentheses.
        /// </summary>
        internal static string UnescapeQuotesAndParentheses(string text)
        {
            const string quote = "'";
            const string escapedQuote = @"\'";
            const string parenthesis = ")";
            const string escapedParenthesis = @"\)";

            if (text == null)
                return null;

            return text
                .Replace(escapedQuote, quote)
                .Replace(EscapedDoubleQuote, DoubleQuote)
                .Replace(escapedParenthesis, parenthesis);
        }

        /// <summary>
        /// Adds backslash-escaping for markup symbols.
        /// </summary>
        internal static string EscapeMarkupSymbols(string text)
        {
            return EscapeSymbols(text, gMarkupSymbolsForEscaping);
        }

        /// <summary>
        /// Removes backslash-escaping for markup symbols.
        /// </summary>
        internal static string UnescapeMarkupSymbols(string text)
        {
            return UnescapeSymbols(text, gEscapableMarkupSymbols);
        }

        /// <summary>
        /// Adds backslash-escaping for square brackets.
        /// </summary>
        internal static string EscapeSquareBrackets(string text)
        {
            return EscapeSymbols(text, gSquareBrackets);
        }

        /// <summary>
        /// Removes backslash-escaping for square brackets.
        /// </summary>
        internal static string UnescapeSquareBrackets(string text)
        {
            return UnescapeSymbols(text, gSquareBrackets);
        }

        /// <summary>
        /// Returns index of the first Whitespace character or -1 if not found.
        /// The search starts at a specified character position.
        /// </summary>
        internal static int IndexOfWhitespace(string text, int startIndex, int endIndex)
        {
            if (text != null)
            {
                for (int i = startIndex; i <= endIndex; i++)
                {
                    if (IsWhitespaceCharacter(text[i]))
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns index of the first non-Whitespace character or -1 if not found.
        /// The search starts at a specified character position.
        /// </summary>
        internal static int IndexOfNonWhitespace(string text, int startIndex, int endIndex)
        {
            if (text != null)
            {
                for (int i = startIndex; i <= endIndex; i++)
                {
                    if (!IsWhitespaceCharacter(text[i]))
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns index of the first occurrence of the specified unescaped character or -1 if not found.
        /// The search starts at a specified character position.
        /// </summary>
        internal static int IndexOfUnescaped(string text, char c, int startIndex, int endIndex)
        {
            while (startIndex < endIndex)
            {
                startIndex = text.IndexOf(c, startIndex, endIndex - startIndex + 1);

                if (startIndex == -1)
                    return -1;

                if (IsEscaped(text, startIndex))
                    startIndex++;
                else
                    return startIndex;
            }

            return -1;
        }

        /// <summary>
        /// Returns true, if a specified character is markdown ASCII whitespace.
        /// </summary>
        /// <remarks>
        /// According to spec at https://spec.commonmark.org/0.28/#whitespace
        /// Whitespace is a sequence of one or more whitespace characters.
        /// A whitespace character is a space (U+0020), tab (U+0009), newline (U+000A), line tabulation (U+000B),
        /// form feed (U+000C), or carriage return (U+000D).
        /// </remarks>
        internal static bool IsWhitespaceCharacter(char c)
        {
            return (
                (c == ControlChar.SpaceChar) ||
                (c == ControlChar.TabChar) ||
                (c == ControlChar.LineFeedChar) ||
                (c == ControlChar.LineBreakChar) ||
                (c == ControlChar.PageBreakChar) ||
                (c == ControlChar.ParagraphBreakChar) ||
                 // WORDSNET-18341 Also added SoftLineBreakChar as it actually
                 // will be converted to some of the whitespace characters.
                (c == SoftLineBreakChar));
        }

        /// <summary>
        /// Returns number after a specified substring inside a text.
        /// </summary>
        internal static int GetNumberAfterSubstring(string text, string substring)
        {
            int substringIdx = text.IndexOf(substring, StringComparison.InvariantCultureIgnoreCase);

            if (substringIdx == -1)
                return -1;

            int numberIdx = substringIdx + substring.Length;

            // One dot (.) is allowed just after substring to increase readability.
            if ((numberIdx < text.Length) && (text[numberIdx] == '.'))
                numberIdx++;

            int numberLength = 0;
            while (((numberIdx + numberLength) < text.Length) && StringUtil.IsDigit(text[numberIdx + numberLength]))
                numberLength++;

            if (numberLength == 0)
                return 0;

            string number = text.Substring(numberIdx, numberLength);

            return FormatterPal.ParseInt(number);
        }

        /// <summary>
        /// Returns true, if specified unescaped brackets are balanced.
        /// </summary>
        internal static bool AreBalanced(string text, int startIndex, int endIndex, char leftBracket, char rightBracket)
        {
            Debug.Assert(text != null);

            int leftBracketCounter = 0;
            int rightBracketCounter = 0;

            char prevChar = char.MinValue;
            for (int i = startIndex; i <= endIndex; i++)
            {
                char curChar = text[i];
                // Skip escaped characters.
                if (prevChar != ControlChar.BackslashChar)
                {
                    if (curChar == leftBracket)
                        leftBracketCounter++;
                    else if (curChar == rightBracket)
                        rightBracketCounter++;

                    if (leftBracketCounter < rightBracketCounter)
                        return false;
                }

                prevChar = curChar;
            }

            return (leftBracketCounter == rightBracketCounter);
        }

        /// <summary>
        /// Returns true, if specified unescaped brackets are balanced.
        /// </summary>
        internal static bool AreBalanced(Delimiter opening, Delimiter closing, char leftBracket, char rightBracket)
        {
            string text = opening.Text;
            int startIndex = opening.End + 1;
            int endIndex = closing.Start - 1;


            int leftBracketCounter = 0;
            int rightBracketCounter = 0;

            char prevChar = char.MinValue;
            for (int i = startIndex; i <= endIndex; i++)
            {
                // Skip characters inside linked delimiters with higher priority.
                if (GetPriority(i, opening, closing) > closing.Priority)
                {
                    prevChar = char.MinValue;
                    continue;
                }

                char curChar = text[i];
                // Skip escaped characters.
                if (prevChar != ControlChar.BackslashChar)
                {
                    if (curChar == leftBracket)
                        leftBracketCounter++;
                    else if (curChar == rightBracket)
                        rightBracketCounter++;

                    if (leftBracketCounter < rightBracketCounter)
                        return false;
                }

                prevChar = curChar;
            }

            return (leftBracketCounter == rightBracketCounter);
        }

        /// <summary>
        /// Returns true, if character at a specified index in text is escaped.
        /// </summary>
        internal static bool IsEscaped(string text, int index)
        {
            Debug.Assert(text != null);
            Debug.Assert((index >= 0) && (index < text.Length));

            int escapingStart = index - 1;
            while ((escapingStart >= 0) && (text[escapingStart] == ControlChar.BackslashChar))
                escapingStart--;

            int length = (index - escapingStart) + 1;
            return ((length % 2) == 1);
        }

        /// <summary>
        /// Returns true, if a specified text has the non-escaped specified characters.
        /// </summary>
        internal static bool HasNonEscapedCharacters(string text, int startIndex, int endIndex, params char[] values)
        {
            Debug.Assert(text != null);

            char prevChar = char.MinValue;
            for (int i = startIndex; i <= endIndex; i++)
            {
                char curChar = text[i];
                if (ArrayUtil.FindCharInArray(values, curChar) && (prevChar != ControlChar.BackslashChar))
                    return true;

                prevChar = curChar;
            }

            return false;
        }

        /// <summary>
        /// Returns true, if a specified character is escapable.
        /// </summary>
        internal static bool IsEscapableMarkupCharacter(char c)
        {
            return ArrayUtil.FindCharInArray(gEscapableMarkupCharacters, c);
        }

        /// <summary>
        /// Unescapes text.
        /// </summary>
        internal static StringBuilder UnEscape(string text)
        {
            StringBuilder unescapedText = new StringBuilder();
            if (text.Length == 0)
                return unescapedText;

            char prevChar = text[0];
            for (int i = 1; i < text.Length; i++)
            {
                char curChar = text[i];

                if ((prevChar != ControlChar.BackslashChar) || !IsEscapableMarkupCharacter(curChar))
                    unescapedText.Append(prevChar);

                prevChar = curChar;
            }

            unescapedText.Append(prevChar);

            return unescapedText;
        }

        /// <summary>
        /// Gets first linked delimiter of a specified type in a specified range.
        /// Range boundaries are not included.
        /// </summary>
        internal static Delimiter GetLinkedInRange(DelimiterType type, Delimiter a, Delimiter b)
        {
            Debug.Assert((a != null) && (b != null));

            Delimiter start = (a.IsBefore(b)) ? a : b;
            Delimiter end = (a == start) ? b : a;

            // The boundaries are not included.
            start = (Delimiter)start.NextNode;
            while (start != end)
            {
                if (!start.IsNotIncluded && start.IsLinked && (start.Type == type))
                    return start;

                start = (Delimiter)start.NextNode;
            }

            return null;
        }

        /// <summary>
        /// Returns left indent for a specified node in twips.
        /// </summary>
        internal static int GetLeftIndent(Node node)
        {
            if (node == null)
                return -1;

            switch (node.NodeType)
            {
                case NodeType.Paragraph:
                {
                    return (int)((Paragraph)node).FetchParaAttr(ParaAttr.LeftIndent, RevisionsView.Original);
                }

                case NodeType.Table:
                {
                    Row firstRow = ((Table)node).FirstRow;
                    return (firstRow != null) ? (int)((IRowAttrSource)firstRow).FetchRowAttr(TableAttr.LeftIndent) : 0;
                }

                // Skip bookmarks.
                case NodeType.BookmarkStart:
                case NodeType.BookmarkEnd:
                {
                    return GetLeftIndent(node.PreviousSibling);
                }

                default:
                    return -1;
            }
        }

        /// <summary>
        /// Returns true, if a specified character is markdown ASCII control character.
        /// </summary>
        /// <remarks>
        /// An ASCII control character is a character between U+0000–1F (both including) or U+007F.
        /// </remarks>
        internal static bool IsAsciiControlChar(char c)
        {
            return ((0 <= c) && (c <= '\u001F')) || (c == '\u007F');
        }

        /// <summary>
        /// Resolves whitespaces and line breaks by inserting, removing or converting them appropriately.
        /// </summary>
        internal static string ResolveWhitespacesAndLineBreaks(Block block, char softLineBreakCharReplacement)
        {
            List<StringBuilder> lines = new List<StringBuilder>();

            StringBuilder currentLine = new StringBuilder();
            foreach (char c in block.Text)
            {
                // We need to remove leading spaces in the beginning of each line.
                if ((currentLine.Length == 0) && IsWhitespaceCharacter(c))
                {
                    // For the non-first line we are obviously at the very beginning in this point.
                    if (lines.Count > 0)
                        continue;

                    // Otherwise, we need some checks to ensure we are at the beginning of the text line:
                    bool isSkip = true;
                    Block curBlock = block;
                    while (curBlock != null)
                    {
                        if (curBlock.GetPreviousSibling(
                                BlockType.Inline,
                                BlockType.Underline,
                                BlockType.BoldInline,
                                BlockType.ItalicInline,
                                BlockType.Strikethrough,
                                BlockType.InlineCode,
                                BlockType.Autolink,
                                BlockType.ImageDescription,
                                BlockType.FootnoteReference,
                                BlockType.LinkDestination) != null)
                        {
                            isSkip = false;
                            break;
                        }

                        curBlock = curBlock.Parent;
                        if (curBlock is InlineContainerBlock)
                        {
                            break;
                        }
                    }

                    if (isSkip)
                            continue;
                }

                if (c == SoftLineBreakChar)
                {
                    // Trim all whitespaces from end and place space character instead of soft line break char.
                    StringUtil.TrimEnd(currentLine);
                    currentLine.Append(softLineBreakCharReplacement);
                }
                else if (c == HardLineBreakSlashChar)
                {
                    // Don't trim anything and start a new line.
                    currentLine.Append(ControlChar.LineBreakChar);
                }
                else if (c == HardLineBreakSpacesChar)
                {
                    // Trim all whitespaces from end and start a new line.
                    StringUtil.TrimEnd(currentLine);
                    currentLine.Append(ControlChar.LineBreakChar);
                }
                else
                {
                    currentLine.Append(c);
                    continue;
                }

                lines.Add(currentLine);
                currentLine = new StringBuilder();
            }

            lines.Add(currentLine);

            StringBuilder sb = lines[0];
            for (int i = 1; i < lines.Count; i++)
                sb.Append(lines[i]);

            return sb.ToString();
        }

        /// <summary>
        /// Replaces specified characters within a specified string with
        /// a corresponding hexadecimal numeric character references.
        /// </summary>
        /// <remarks>
        /// See https://spec.commonmark.org/0.31.2/#hexadecimal-numeric-character-references
        /// </remarks>
        internal static string ReplaceWithHexRef(string s, string charactersToReplace)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                if (charactersToReplace.IndexOf(c) == -1)
                {
                    sb.Append(c);
                    continue;
                }

                byte[] bytes = Encoding.UTF8.GetBytes(new char[] {c});
                string hexCode = StringUtil.BytesToHex(bytes);

                sb.Append("&#x").Append(hexCode).Append(';');
            }

            return sb.ToString();
        }

        /// <summary>
        /// Returns priority of the delimiter that is located between specified two delimiters at a specified position.
        /// </summary>
        private static DelimiterPriority GetPriority(int position, Delimiter left, Delimiter right)
        {
            Debug.Assert(left.IsBefore(right));

            DelimiterPriority priority = DelimiterPriority.Lowest;

            Delimiter start = left;
            while (start != right)
            {
                if (!start.IsNotIncluded && start.IsLinked)
                {
                    if ((start.End < position) && (position < start.LinkedDelimiter.Start))
                        priority = start.Priority;
                }

                start = (Delimiter)start.NextNode;
            }

            return priority;
        }

        /// <summary>
        /// Adds backslash-escaping for specified symbols.
        /// </summary>
        private static string EscapeSymbols(string text, string[] symbols)
        {
            Debug.Assert(text != null);
            Debug.Assert(symbols != null);
            string result = text;
            foreach (string symbol in symbols)
            {
                string escaped = string.Format("{0}{1}", ControlChar.BackslashChar, symbol);
                result = result.Replace(symbol, escaped);
            }

            return result;
        }


        /// <summary>
        /// Removes backslash-escaping for specified symbols.
        /// </summary>
        private static string UnescapeSymbols(string text, string[] symbols)
        {
            Debug.Assert(text != null);
            Debug.Assert(symbols != null);
            string result = text;
            if (result.Contains(ControlChar.BackslashChar.ToString()))
            {
                foreach (string symbol in symbols)
                {
                    string escaped = string.Format("{0}{1}", ControlChar.BackslashChar, symbol);
                    result = result.Replace(escaped, symbol);
                }
            }
            return result;
        }

        /// <summary>
        /// Returns true, if a specified character is one of the bullet list markers (-, +, *).
        /// </summary>
        private static bool IsBulletListMarker(char c)
        {
            return (c == '-') || (c == '+') || (c == '*');
        }

        /// <summary>
        /// Returns true, if delimiter runs between specified characters is left-flanking.
        /// </summary>
        /// <remarks>
        /// See 6.4 Emphasis and strong emphasis at https://spec.commonmark.org for details.
        /// </remarks>
        private static bool IsLeftFlanking(char prevChar, char nextChar)
        {
            return (!IsWhiteSpace(nextChar) &&
                    (!IsPunctuation(nextChar) || IsWhiteSpace(prevChar) || IsPunctuation(prevChar)));
        }

        /// <summary>
        /// Returns true, if delimiter runs between specified characters is right-flanking.
        /// </summary>
        /// <remarks>
        /// See 6.4 Emphasis and strong emphasis at https://spec.commonmark.org for details.
        /// </remarks>
        private static bool IsRightFlanking(char prevChar, char nextChar)
        {
            return (!IsWhiteSpace(prevChar) &&
                    (!IsPunctuation(prevChar) || IsWhiteSpace(nextChar) || IsPunctuation(nextChar)));
        }

        /// <summary>
        /// Returns true, if a specified character is whitespace character.
        /// </summary>
        private static bool IsWhiteSpace(char c)
        {
            return char.IsWhiteSpace(c) || (c == SoftLineBreakChar);
        }

        /// <summary>
        /// Returns true, if a specified character is markdown punctuation character.
        /// </summary>
        /// <remarks>
        /// See details at https://spec.commonmark.org/0.29/#punctuation-character.
        /// </remarks>
        private static bool IsPunctuation(char c)
        {
            return (c == '$') ||
                   (c == '+') ||
                   (c == '<') ||
                   (c == '=') ||
                   (c == '>') ||
                   (c == '\\') ||
                   (c == '^') ||
                   (c == '`') ||
                   (c == '~') ||
                   (c == '|') ||
                   char.IsPunctuation(c);
        }

        /// <summary>
        /// Escapable markup symbols.
        /// </summary>
        /// <remarks>
        /// The following markup symbols can be backslash-escaped, as usual in Markdown:
        /// https://spec.commonmark.org/0.29/#example-496
        /// </remarks>
        private static readonly string[] gEscapableMarkupSymbols =
        {
            "'", ".", "+", "-", "=", "*", "_", "`", "~", ":", "!", "#", "$", "%", "&", "@", "/", "?", "^", "|", ";", "\\",
            "\"", "<", ">", "[", "]", "(", ")", "{", "}"
        };

        /// <summary>
        /// Escapable markup characters.
        /// </summary>
        /// <remarks>
        /// This should be the same characters as in gEscapableMarkupSymbols,
        /// but it is limited to the specified ones (bold and italic emphases delimiters) for a while.
        /// </remarks>
        private static readonly char[] gEscapableMarkupCharacters = {'*', '_'};

        /// <summary>
        /// Markup symbols for backslash-escaping.
        /// </summary>
        private static readonly string[] gMarkupSymbolsForEscaping = {"(", ")", "<", ">"};

        /// <summary>
        /// Square brackets.
        /// </summary>
        private static readonly string[] gSquareBrackets = {"[", "]"};

        /// <summary>
        /// A Tab size in spaces.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int TabSize = 4;

        internal const string QuoteStyleName = "Quote";
        internal const string ListStyleName = "List";
        internal const string SetextHeadingStyleName = "SetextHeading";
        internal const string HeadingStyleName = "Heading";
        internal const string FencedCodeStyleName = "FencedCode";
        internal const string IndentedCodeStyleName = "IndentedCode";
        internal const string FootnoteStyleName = "Footnote Text";
        internal const string InlineCodeStyleName = "InlineCode";

        /// <summary>
        /// Space character.
        /// </summary>
        internal const char SpaceChar = '\u0020';

        /// <summary>
        /// Soft line break.
        /// </summary>
        internal const char SoftLineBreakChar = '\uE00b';

        /// <summary>
        /// Hard line break character for backslash.
        /// </summary>
        internal const char HardLineBreakSlashChar = '\uE00d';

        /// <summary>
        /// Hard line break character for two spaces.
        /// </summary>
        internal const char HardLineBreakSpacesChar = '\uE00a';

        /// <summary>
        /// Hard line break string for backslash.
        /// </summary>
        internal static readonly string HardLineBreakSlash = HardLineBreakSlashChar.ToString();

        /// <summary>
        /// Double Quote character.
        /// </summary>
        private const string DoubleQuote = "\"";

        /// <summary>
        /// Backslash-escaped Double Quote character.
        /// </summary>
        private const string EscapedDoubleQuote = "\\\"";
    }
}
