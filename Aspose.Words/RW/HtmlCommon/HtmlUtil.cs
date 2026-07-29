// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/11/2004 by Roman Korchagin

using System;
using System.Text;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Math;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.HtmlCommon
{
    /// <summary>
    /// Utility functions to deal with HTML conversion.
    /// </summary>
    internal static class HtmlUtil
    {
        /// <summary>
        /// Removes Word control characters from a string (all characters with codes U+0001..U+001F but tab U+0009
        /// and line feed U+000A, which are processed somewhere else).
        /// </summary>
        /// <param name="text">Text for processing.</param>
        internal static string RemoveControlCharacters(string text)
        {
            return RemoveControlCharsAndWhitespaces(text, true, false, false);
        }

        /// <summary>
        /// Trims whitespace from a string.
        /// White space can be leading (left), in the middle and trailing.
        /// Middle and trailing whitespace is processed in the same way - it is collapsed into one space character.
        /// Leading whitespace processing has two options. It can either trim it completely or leave one space.
        /// </summary>
        /// <param name="text">Text for processing.</param>
        /// <param name="isFullLeftTrim">Whether to completely trim leading whitespace or leave one space character.</param>
        internal static string RemoveWhitespaces(string text, bool isFullLeftTrim)
        {
            return RemoveControlCharsAndWhitespaces(text, false, true, isFullLeftTrim);
        }

        /// <summary>
        /// Removes Word control characters from a string (all characters with codes U+0001..U+001F but tab U+0009
        /// and line feed U+000A, which are processed somewhere else) and trims whitespace from a string.
        /// White space can be leading (left), in the middle and trailing.
        /// Middle and trailing whitespace is processed in the same way - it is collapsed into one space character.
        /// Leading whitespace processing has two options. It can either trim it completely or leave one space.
        /// </summary>
        /// <param name="text">Text for processing.</param>
        /// <param name="isFullLeftTrim">Whether to completely trim leading whitespace or leave one space character.</param>
        internal static string RemoveControlCharsAndWhitespaces(string text, bool isFullLeftTrim)
        {
            return RemoveControlCharsAndWhitespaces(text, true, true, isFullLeftTrim);
        }

        internal static bool ContainsAnythingButWhitespaces(string text, bool newlinesAreWhitespaces)
        {
            foreach (char c in text)
            {
                bool isWhitespace = (c == '\u000A')
                    ? newlinesAreWhitespaces
                    : IsWhitespace(c);
                if (!isWhitespace)
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsWhitespace(char c)
        {
            // http://www.w3.org/TR/html5/infrastructure.html#space-character
            return (c == '\u0009') ||
                   (c == '\u000A') ||
                   (c == '\u000C') ||
                   (c == '\u000D') ||
                   (c == '\u0020');
        }

        /// <summary>
        /// Validates an URI string read from a HTML attribute like "href" or "src".
        /// </summary>
        /// <remarks>
        /// <para>Browsers validate URIs as follows:</para>
        /// <para>1) remove leading and trailing space (U+0020) and control (U+0001..U+001F) characters;</para>
        /// <para>2) remove all control characters except for EOL (U+000A and U+000D) and TAB (U+0009).</para>
        /// <para>Additionally, we escape the double quote (U+0022) character.</para>
        /// </remarks>
        internal static string ValidateUri(string uri)
        {
            if (!StringUtil.HasChars(uri))
                return uri;

            int leading = 0;
            for (int i = 0; i < uri.Length; i++)
            {
                if (!IsControlCharacter(uri[i]) && !IsWhitespace(uri[i]))
                {
                    break;
                }
                ++leading;
            }

            int trailing = 0;
            for (int i = uri.Length - 1; i >= leading; i--)
            {
                if (!IsControlCharacter(uri[i]) && !IsWhitespace(uri[i]))
                {
                    break;
                }
                ++trailing;
            }

            int trimmedLength = uri.Length - leading - trailing;
            if (trimmedLength == 0)
                return string.Empty;

            // The specified length will be not enough if the URI contains quotation marks but it's a rare case.
            StringBuilder validatedUri = new StringBuilder(trimmedLength);
            for (int i = leading; i < (leading + trimmedLength); i++)
            {
                char c = uri[i];
                if (c == '\"')
                {
                    // The 'QUOTATION MARK' (U+0022) character causes problems in field codes and we have to escape it.
                    validatedUri.Append("%22");
                }
                else if ((c == '\n') || (c == '\r') || (c == '\t'))
                {
                    // These characters are removed from URIs upon validation.
                }
                else
                {
                    // Append all other characters as is.
                    validatedUri.Append(c);
                }
            }

            return validatedUri.ToString();
        }

        /// <summary>
        /// Converts a string into Word tab alignment.
        /// </summary>
        /// <param name="tabAlignment">Text representation of Word tab alignment.</param>
        /// <param name="result">Result tab alignment value.</param>
        /// <returns>
        /// <c>true</c>, if the tab alignment is converted sucessfully.
        /// <c>false</c> otherwise.
        /// </returns>
        internal static bool ParseTapStopAlignment(string tabAlignment, out TabAlignment result)
        {
            switch (tabAlignment.ToLowerInvariant())
            {
                case "left":
                    result = TabAlignment.Left;
                    return true;
                case "center":
                    result = TabAlignment.Center;
                    return true;
                case "right":
                    result = TabAlignment.Right;
                    return true;
                case "decimal":
                    result = TabAlignment.Decimal;
                    return true;
                case "bar":
                    result = TabAlignment.Bar;
                    return true;
                case "list":
                    result = TabAlignment.List;
                    return true;
                case "clear":
                    result = TabAlignment.Clear;
                    return true;
                default:
                    result = TabAlignment.Left;
                    return false;
            }
        }

        /// <summary>
        /// Converts a string into Word tab leader.
        /// </summary>
        /// <param name="tabLeader">Text representation of Word tab leader.</param>
        /// <param name="result">Result tab leader value.</param>
        /// <returns>
        /// <c>true</c>, if the tab leader is converted sucessfully.
        /// <c>false</c> otherwise.
        /// </returns>
        internal static bool ParseTapStopLeader(string tabLeader, out TabLeader result)
        {
            switch (tabLeader.ToLowerInvariant())
            {
                case "none":
                    result = TabLeader.None;
                    return true;
                case "dots":
                    result = TabLeader.Dots;
                    return true;
                case "dashes":
                    result = TabLeader.Dashes;
                    return true;
                case "line":
                    result = TabLeader.Line;
                    return true;
                case "heavy":
                    result = TabLeader.Heavy;
                    return true;
                case "middledot":
                    result = TabLeader.MiddleDot;
                    return true;
                default:
                    result = TabLeader.None;
                    return false;
            }
        }

        /// <summary>
        /// Converts a string into Word wrap type.
        /// </summary>
        /// <param name="wrapType">Text representation of Word wrap type.</param>
        /// <param name="result">Result wrap type value.</param>
        /// <returns>
        /// <c>true</c>, if the wrap type is converted sucessfully.
        /// <c>false</c> otherwise.
        /// </returns>
        internal static bool ParseWrapType(string wrapType, out WrapType result)
        {
            switch (wrapType.ToLowerInvariant())
            {
                case "none":
                    result = WrapType.None;
                    return true;
                case "inline":
                    result = WrapType.Inline;
                    return true;
                case "topbottom":
                    result = WrapType.TopBottom;
                    return true;
                case "square":
                    result = WrapType.Square;
                    return true;
                case "tight":
                    result = WrapType.Tight;
                    return true;
                case "through":
                    result = WrapType.Through;
                    return true;
                default:
                    result = WrapType.None;
                    return false;
            }
        }

        /// <summary>
        /// Converts a string into relative horizontal position.
        /// </summary>
        /// <param name="position">Text representation of relative horizontal position.</param>
        /// <param name="result">Result relative horizontal position value.</param>
        /// <returns>
        /// <c>true</c>, if the relative horizontal position is converted sucessfully.
        /// <c>false</c> otherwise.
        /// </returns>
        internal static bool ParseRelativeHorizontalPosition(string position, out RelativeHorizontalPosition result)
        {
            switch (position.ToLowerInvariant())
            {
                case "margin":
                    result = RelativeHorizontalPosition.Margin;
                    return true;
                case "page":
                    result = RelativeHorizontalPosition.Page;
                    return true;
                case "column":
                    result = RelativeHorizontalPosition.Column;
                    return true;
                case "character":
                    result = RelativeHorizontalPosition.Character;
                    return true;
                case "leftmargin":
                    result = RelativeHorizontalPosition.LeftMargin;
                    return true;
                case "rightmargin":
                    result = RelativeHorizontalPosition.RightMargin;
                    return true;
                case "insidemargin":
                    result = RelativeHorizontalPosition.InsideMargin;
                    return true;
                case "outsidemargin":
                    result = RelativeHorizontalPosition.OutsideMargin;
                    return true;
                default:
                    result = RelativeHorizontalPosition.Column;
                    return false;
            }
        }

        /// <summary>
        /// Converts a string into relative vertical position.
        /// </summary>
        /// <param name="position">Text representation of relative vertical position.</param>
        /// <param name="result">Result relative vertical position value.</param>
        /// <returns>
        /// <c>true</c>, if the relative vertical position is converted sucessfully.
        /// <c>false</c> otherwise.
        /// </returns>
        internal static bool ParseRelativeVerticalPosition(string position, out RelativeVerticalPosition result)
        {
            switch (position.ToLowerInvariant())
            {
                case "margin":
                    result = RelativeVerticalPosition.Margin;
                    return true;
                case "page":
                    result = RelativeVerticalPosition.Page;
                    return true;
                case "paragraph":
                    result = RelativeVerticalPosition.Paragraph;
                    return true;
                case "line":
                    result = RelativeVerticalPosition.Line;
                    return true;
                case "topmargin":
                    result = RelativeVerticalPosition.TopMargin;
                    return true;
                case "bottommargin":
                    result = RelativeVerticalPosition.BottomMargin;
                    return true;
                case "insidemargin":
                    result = RelativeVerticalPosition.InsideMargin;
                    return true;
                case "outsidemargin":
                    result = RelativeVerticalPosition.OutsideMargin;
                    return true;
                default:
                    result = RelativeVerticalPosition.Paragraph;
                    return false;
            }
        }

        /// <summary>
        /// Parses a non-negative integer according to HTML 5 rules.
        /// </summary>
        /// <param name="s">String to parse.</param>
        /// <returns>
        /// The parsed value, if parsing was successful;
        /// <c>int.MinValue</c> otherwise.
        /// </returns>
        /// <remarks>
        /// The algorithm is taken from here:
        /// http://www.w3.org/TR/html5/infrastructure.html#rules-for-parsing-non-negative-integers
        /// Returned values, if sucessfully parsed, are in range [0..2147483647]
        /// (in other words, the range is [0..int.MaxValue]).
        /// In case of an overflow the value of 2147483647 (int.MaxValue) is returned.
        /// </remarks>
        internal static int ParseNonNegativeInteger(string s)
        {
            if (s == null)
                return int.MinValue;

            int value = ParseInteger(s);
            return (value >= 0) ? value : int.MinValue;
        }

        /// <summary>
        /// Parses an integer according to HTML 5 rules.
        /// </summary>
        /// <param name="s">String to parse.</param>
        /// <returns>
        /// The parsed value, if parsing was successful;
        /// <c>int.MinValue</c> otherwise.
        /// </returns>
        /// <remarks>
        /// The algorithm is taken from here: http://www.w3.org/TR/html5/infrastructure.html#rules-for-parsing-integers
        /// Returned values are in range [-2147483647..2147483647] (in other words, the range is [-int.MaxValue..int.MaxValue]).
        /// In case of an overflow 2147483647 is returned if the result is positive,
        /// and -2147483647 is returned if the result is negative.
        /// </remarks>
        internal static int ParseInteger(string s)
        {
            if (s == null)
                return int.MinValue;

            // 1. Let input be the string being parsed.
            // 2. Let position be a pointer into input, initially pointing at the start of the string.
            // 3. Let sign have the value "positive".
            int i = 0;
            int sign = 1;

            // 4. Skip whitespace.
            while ((i < s.Length) && IsWhitespace(s[i]))
            {
                ++i;
            }

            // 5. If position is past the end of input, return an error.
            if (i >= s.Length)
            {
                return int.MinValue;
            }

            // 6. If the character indicated by position (the first character) is a "-" (U+002D) character:
            if (s[i] == '\x002D')
            {
                // 6.1. Let sign be "negative".
                sign = -1;

                // 6.2. Advance position to the next character.
                ++i;
            }
            // 6. Otherwise, if the character indicated by position (the first character) is a "+" (U+002B) character:
            else if (s[i] == '\x002B')
            {
                // 6.1. Advance position to the next character. (The "+" is ignored, but it is not conforming.)
                ++i;
            }

            // 6.2. (6.3.) If position is past the end of input, return an error.
            if (i >= s.Length)
            {
                return int.MinValue;
            }

            // 7. If the character indicated by position is not one of ASCII digits, then return an error.
            if (!StringUtil.IsDigit(s[i]))
            {
                return int.MinValue;
            }

            // 8. Collect a sequence of characters in the range ASCII digits, and interpret the resulting sequence
            // as a base-ten integer. Let value be that integer.
            long value = 0;
            while ((i < s.Length) && StringUtil.IsDigit(s[i]))
            {
                value *= 10;
                int digit = s[i] - '0';
                value += digit;
                ++i;

                bool overflow = value > int.MaxValue;
                if (overflow)
                {
                    return (sign > 0)
                        ? int.MaxValue
                        : -int.MaxValue;
                }
            }

            // 9. If sign is "positive", return value, otherwise return the result of subtracting value from zero.
            return (sign > 0)
                ? (int)value
                : -(int)value;
        }

        /// <summary>
        /// Parses an integer value according to HTML 5 rules and clips it to the specified range.
        /// Returns the min specified value in case of an error.
        /// </summary>
        internal static int ParseIntegerInRange(string s, int min, int max)
        {
            // Note that ParseInteger correctly works with null and empty strings.
            int value = ParseInteger(s);
            if (value < min)
            {
                // Parsing errors are covered by this branch.
                return min;
            }
            if (value > max)
            {
                return max;
            }
            return value;
        }

        internal static bool IsParagraphHasOnlyFloatingShapes(Paragraph para)
        {
            bool hasAtLeastOneFloatingShape = false;
            for (Node node = para.FirstChild; node != null; node = node.NextNonAnnotationSibling)
            {
                if (!(node is ShapeBase))
                    return false;

                ShapeBase shapeBase = (ShapeBase)node;
                if (shapeBase.WrapType != WrapType.None)
                    return false;

                hasAtLeastOneFloatingShape = true;
            }

            return hasAtLeastOneFloatingShape;
        }

        internal static bool IsParagraphHasOnlyPageOrColumnBreaks(Paragraph para)
        {
            bool hasAtLeastOnePageOrColumnBreak = false;
            for (Node node = para.FirstChild; node != null; node = node.NextNonAnnotationSibling)
            {
                if (!(node is Inline))
                    return false;

                string text = node.GetText();
                for (int i = 0; i < text.Length; i++)
                {
                    if ((text[i] != ControlChar.PageBreakChar) && (text[i] != ControlChar.ColumnBreakChar))
                        return false;

                    hasAtLeastOnePageOrColumnBreak = true;
                }
            }

            return hasAtLeastOnePageOrColumnBreak;
        }

        internal static bool IsParagraphHasOnlyEmptyRuns(Paragraph para)
        {
            for (Node node = para.FirstChild; node != null; node = node.NextSibling)
            {
                if (node.NodeType != NodeType.Run)
                    return false;

                Run run = (Run)node;
                if (StringUtil.HasChars(run.Text))
                    return false;

                if (run.RunPr.GetDirectAttr(FontAttr.Ruby) != null)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Sometime MS Word doesn't export frames.
        /// This functions determines if paragraph will have representable frame.
        /// </summary>
        internal static bool IsParagraphHasRepresentableFrame(Paragraph para, ParaPr expandedParaPr)
        {
            // WORDSNET-17638 Ignore frame if paragraph is in cell.
            // It correctly takes markup nodes like SDT and SmartTag into account.
            if (para.IsInCell)
            {
                return false;
            }

            // WORDSNET-8922, 10578 - We should ignore zero-width and zero-height frames if width and height
            // were explicitly specified.
            if ((expandedParaPr.HasFrameWidth && (expandedParaPr.FrameWidth == 0)) ||
                (expandedParaPr.HasFrameHeight && (expandedParaPr.FrameHeight == 0)))
            {
                return false;
            }

            return expandedParaPr.HasFrameAttributes;
        }

        /// <summary>
        /// Determines if paragraph mark is used to break the end of a paragraph for display.
        /// </summary>
        /// <param name="paragraph">Inspected paragraph.</param>
        /// <param name="expandedParaPr">Extended properties of inspected paragraph.</param>
        /// <param name="nextParagraph">Paragraph next to inspected paragraph.</param>
        /// <param name="expandedNextParaPr">Extended properties of paragraph next to inspected paragraph.</param>
        internal static bool IsParagraphBreakEndsParagraph(
            Paragraph paragraph,
            ParaPr expandedParaPr,
            Paragraph nextParagraph,
            ParaPr expandedNextParaPr)
        {
            if ((paragraph == null) || (nextParagraph == null))
                return true;

            // 1. Currently we export documents as if Show/Hidden is turned off in MS Word editor.
            //    FontAttr.SpecialHidden makes difference only when Show/Hidden is turned on, so we can ignore it here for now.
            // 2. FontAttr.Hidden doesn't have effect when paragraph is in frame.
            return !paragraph.ParagraphBreakFont.Hidden ||
                   IsParagraphHasRepresentableFrame(paragraph, expandedParaPr) ||
                   IsParagraphHasRepresentableFrame(nextParagraph, expandedNextParaPr);
        }

        internal static bool IsParagraphOrHeadingElement(string elementName)
        {
            return (elementName == "p") || IsHeadingElement(elementName);
        }

        internal static bool IsHeadingElement(string elementName)
        {
            switch (elementName)
            {
                case "h1":
                case "h2":
                case "h3":
                case "h4":
                case "h5":
                case "h6":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns string representation of list level's number in HTML.
        /// </summary>
        internal static string GetListLevelNumberString(int listLevelNumber)
        {
            return "%" + FormatterPal.IntToStr(listLevelNumber);
        }

        /// <summary>
        /// Converts HTML the frame text wrapping to WrapType enumeration.
        /// </summary>
        internal static WrapType ParseFrameWrapType(string wraptype)
        {
            switch (wraptype.ToLowerInvariant())
            {
                case "around":
                    return WrapType.Square;
                case "auto":
                    return WrapType.Inline;
                case "no-wrap-beside":
                    return WrapType.TopBottom;
                default:
                    return WrapType.None;
            }
        }

        /// <summary>
        /// Converts HTML relative vertical position of the frame to RelativeVerticalPosition enumeration.
        /// </summary>
        internal static bool ParseFrameRelativeVerticalPosition(
            string position,
            out RelativeVerticalPosition frameRelativeVerticalPosition)
        {
            switch (position.ToLowerInvariant())
            {
                case "page":
                    frameRelativeVerticalPosition = RelativeVerticalPosition.Page;
                    return true;
                case "paragraph":
                    frameRelativeVerticalPosition = RelativeVerticalPosition.Paragraph;
                    return true;
                default:
                    frameRelativeVerticalPosition = RelativeVerticalPosition.Margin;
                    return false;
            }
        }

        /// <summary>
        /// Converts HTML relative horizontal position of the frame to RelativeHorizontalPosition enumeration.
        /// </summary>
        internal static RelativeHorizontalPosition ParseFrameRelativeHorizontalPosition(string position)
        {
            switch (position.ToLowerInvariant())
            {
                case "page":
                    return RelativeHorizontalPosition.Page;
                case "column":
                    return RelativeHorizontalPosition.Column;
                default:
                    return RelativeHorizontalPosition.Margin;
            }
        }

        /// <summary>
        /// Converts HTML height rule of the frame to HeightRule enumeration.
        /// </summary>
        internal static HeightRule ParseFrameHeightRule(string heightRule)
        {
            switch (heightRule.ToLowerInvariant())
            {
                case "at-least":
                    return HeightRule.AtLeast;
                case "exactly":
                    return HeightRule.Exactly;
                default:
                    return HeightRule.Auto;
            }
        }

        /// <summary>
        /// Converts HTML horizontal alignment of the frame to HorizontalAlignment enumeration.
        /// </summary>
        internal static HorizontalAlignment ParseFrameHorizontalAligment(string aligment)
        {
            switch (aligment.ToLowerInvariant())
            {
                case "center":
                    return HorizontalAlignment.Center;
                case "inside":
                    return HorizontalAlignment.Inside;
                case "outside":
                    return HorizontalAlignment.Outside;
                case "right":
                    return HorizontalAlignment.Right;
                default:
                    return HorizontalAlignment.Default;
            }
        }

        /// <summary>
        /// Converts HTML vertical alignment of the frame to VerticalAlignment enumeration.
        /// </summary>
        internal static VerticalAlignment ParseFrameVerticalAlignment(string aligment)
        {
            switch (aligment.ToLowerInvariant())
            {
                case "center":
                    return VerticalAlignment.Center;
                case "bottom":
                    return VerticalAlignment.Bottom;
                case "top":
                    return VerticalAlignment.Top;
                case "inside":
                    return VerticalAlignment.Inside;
                case "outside":
                    return VerticalAlignment.Outside;
                default:
                    return VerticalAlignment.Default;
            }
        }

        /// <summary>
        /// Removes Word control characters from a string (all characters with codes U+0001..U+001F but tab U+0009
        /// and line feed U+000A, which are processed somewhere else) and trims whitespace from a string.
        /// White space can be leading (left), in the middle and trailing.
        /// Middle and trailing whitespace is processed in the same way - it is collapsed into one space character.
        /// Leading whitespace processing has two options. It can either trim it completely or leave one space.
        /// </summary>
        /// <param name="text">Text for processing.</param>
        /// <param name="isRemoveControlChars">Whether to remove control chars.</param>
        /// <param name="isRemoveWhitespaces">Whether to remove whitespace.</param>
        /// <param name="isFullLeftTrim">Whether to completely trim leading whitespace or leave one space character.</param>
        private static string RemoveControlCharsAndWhitespaces(
            string text,
            bool isRemoveControlChars,
            bool isRemoveWhitespaces,
            bool isFullLeftTrim)
        {
            // A flag indicating whether the current whitespace run is the leading whitespace run in the source text.
            bool isFirstWhitespaceRun = isFullLeftTrim;
            // A flag indicating whether the current whitespace character is the first character of the current whitespace run.
            bool isFirstWhitespaceChar = true;

            // Performance optimization. Delay instantiation of modified resulting string until we meet a character that
            // needs to be modified (removed or replaced).
            StringBuilder modifiedText = null;

            const char spaceCharacter = '\u0020';

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                // A flag indicating whether to output this character to the resulting string.
                bool writeCharacter = true;
                // A flag indicating whether to replace this character by the space character. The replacement will only take
                // place if the character is to be output to the resulting string (is not removed completely).
                bool writeSpaceInstead = false;

                // Check whether the current character is a control or a whitespace character. It is important to check
                // for control characters first, because the ranges of control and whitespace characters intersect and control
                // characters take precedence. For example, U+000C is both considered a control character and a whitespace
                // character. However, this algorithm processes it as a control character and removes it.
                bool isControlChar = isRemoveControlChars && IsControlCharacter(c);
                bool isWhitespace = !isControlChar && isRemoveWhitespaces && IsWhitespace(c);

                // Decide what to do with the current character: remove, replace by a space character, or copy unchanged.
                // Set flags accordingly.
                if (isWhitespace)
                {
                    if (!isFirstWhitespaceRun && isFirstWhitespaceChar)
                    {
                        // Collapse whitespace characters to single space. Replace the current character by the space character.
                        // If the current character is already the space character, it is instead copied unchanged.
                        writeSpaceInstead = c != spaceCharacter;
                    }
                    else
                    {
                        // Remove all collapsed whitespace characters except the first one.
                        writeCharacter = false;
                    }

                    // We've just processed the first character of this whitespace run.
                    isFirstWhitespaceChar = false;
                }
                else if (isControlChar)
                {
                    // Remove all control characters. Keep whitespace run flags unchanged.
                    writeCharacter = false;
                }
                else
                {
                    // We've met a non-whitespace character. Reset whitespace run flags.
                    isFirstWhitespaceRun = false;
                    isFirstWhitespaceChar = true;
                }

                // If the current character is modified (is not copied unchanged), we need to create a working copy
                // of the source text that can be altered.
                bool charIsModified = !writeCharacter || writeSpaceInstead;
                if (charIsModified && (modifiedText == null))
                {
                    modifiedText = new StringBuilder(text.Length);
                    // Copy unmodified characters from the source string up to the current character.
                    modifiedText.Append(text, 0, i);
                }

                // If the current character is not removed, output it to the resulting string.
                if (writeCharacter)
                {
                    if (writeSpaceInstead)
                    {
                        modifiedText.Append(spaceCharacter);
                    }
                    else if (modifiedText != null)
                    {
                        modifiedText.Append(c);
                    }
                    else
                    {
                        // Do nothing. So far we hasn't meet any characters that need to be removed or replaced.
                    }
                }
            }

            // Performance optimization. If no modifications were made, return the source string unchanged.
            return (modifiedText != null)
                ? modifiedText.ToString()
                : text;
        }

        private static bool IsControlCharacter(char c)
        {
            return (c >= '\u0001') && (c <= '\u001F') &&
                (c != '\u0009') &&
                (c != '\u000A');
        }

        /// <summary>
        /// Name of the custom document property that MS Word uses to store the "target" attribute value
        /// of the BASE element (the default target for hyperlinks).
        /// </summary>
        internal const string BaseTargetDocumentProperty = "Base Target";
    }
}
