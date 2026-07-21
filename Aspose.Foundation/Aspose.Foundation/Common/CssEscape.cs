// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/02/2016 by Victor Chebotok

using System.Text;

namespace Aspose.Common
{
    /// <summary>
    /// Utility class for escaping CSS text.
    /// </summary>
    public static class CssEscape
    {
        /// <summary>
        /// Escapes text so that it can be used as a CSS "string" inside double quotes.
        /// </summary>
        public static string EscapeDoubleQuotedString(string value)
        {
            return EscapeString(value, '\"');
        }

        /// <summary>
        /// Escapes text so that it can be used as a CSS "string" inside single quotes.
        /// </summary>
        public static string EscapeSingleQuotedString(string value)
        {
            return EscapeString(value, '\'');
        }

        /// <summary>
        /// Escapes text so that it can be used as a CSS "string".
        /// </summary>
        /// <param name="value">Css "string" to escape.</param>
        /// <param name="quoteChar">Quote char that will be escaped.</param>
        /// <remarks>
        /// Strings can either be written with double quotes or with single quotes. 
        /// Double quotes cannot occur inside double quotes, unless escaped (e.g., as '\"' or as '\22'). 
        /// Analogously for single quotes (e.g., "\'" or "\27").
        /// </remarks>
        private static string EscapeString(string value, char quoteChar)
        {
            Debug.Assert(value != null);
            Debug.Assert((quoteChar == '\"') || (quoteChar == '\''));

            StringBuilder result = null;
            int i = 0;

            while (i < value.Length)
            {
                char c = value[i];

                bool isInvalidChar = (c == '\\') || (c == quoteChar);
                bool isNewLineChar = (c == '\n') || (c == '\r') || (c == '\f');
                if (isInvalidChar || isNewLineChar)
                {
                    if (result == null)
                    {
                        result = new StringBuilder();
                        result.Append(value, 0, i);
                    }

                    result.Append('\\');
                }

                if (result != null)
                {
                    if (isNewLineChar)
                    {
                        // The line break characters must be replaced with their hexadecimal
                        // representations. That is, '\n' should be exported as '\\A' and '\r' as '\\D'
                        result.Append(FormatterPal.IntToStrX(c));

                        // If the line break character is followed by a character that is a valid hex digit (0-9, A-F, a-f), 
                        // an additional space character must be inserted after the escaped line break: "\nA" -> "\\A A". 
                        // This extra space character will be ignored during parsing.
                        char nextChar = ((value.Length - 1) > i) ? value[i + 1] : '\0';
                        if (StringUtil.IsHexDigit(nextChar) || (nextChar == ' '))
                            result.Append(' ');
                    }
                    else
                    {
                        result.Append(c);
                    }
                }

                i++;
            }

            return (result == null)
                ? value
                : result.ToString();
        }

        /// <summary>
        /// Escapes text so that it can be used as a CSS identifier.
        /// </summary>
        /// <param name="identifier">A text to escape. May be <c>null</c>.</param>
        /// <returns>
        /// A string containing an escaped version of the text. If the text is already a valid CSS identifier (no escaping
        /// is required), the original string is returned. If the original string is <c>null</c>, this method also returns
        /// <c>null</c>.
        /// </returns>
        public static string EscapeIdentifier(string identifier)
        {
            // Simple special cases.
            if (!StringUtil.HasChars(identifier))
            {
                return identifier;
            }
            if (identifier == "-")
            {
                return @"\-";
            }

            // Lazy initialization. Won't perform escaping unless we find a character that must be escaped.
            StringBuilder escapedIdentifier = null;

            // An optional single dash character is allowed at the start of an identifier. We skip the dash character so that
            // further processing will be the same: the character next to the dash is then considered the first character
            // of the identifier.
            int i = 0;
            if (identifier[i] == '-')
            {
                ++i;
            }

            // Process all characters one-by-one: copy valid characters and replace invalid ones with escape sequences.
            bool isFirstCharacter = true;
            bool afterEscapedCodePoint = false;
            while (i < identifier.Length)
            {
                char c = identifier[i];
                bool isHexDigit = StringUtil.IsHexDigit(c);

                // Decide if the current character is valid.
                bool isValidCharacter = (c >= '\u0080') ||
                    StringUtil.IsLetter(c) ||
                    (c == '_') ||
                    ((!isFirstCharacter) && (StringUtil.IsDigit(c) || (c == '-')));
                isFirstCharacter = false;

                // If the current character is invalid, start building an escape sequence.
                if (!isValidCharacter)
                {
                    // Lazy initialization. We found the first invalid character and it is the point where we actually
                    // start building an escaped version of the identifier.
                    if (escapedIdentifier == null)
                    {
                        escapedIdentifier = new StringBuilder();
                        // We may have skipped some valid characters and now we need to copy them to the result.
                        escapedIdentifier.Append(identifier, 0, i);
                    }
                    escapedIdentifier.Append('\\');
                    afterEscapedCodePoint = false;
                }

                // If we haven't started building an escaped version of the identifier yet, then all characters from the start
                // and up to the current character inclusive are valid and we should skip the current character too. Otherwise,
                // we need to append the current character to the escaped version of the identifier.
                if (escapedIdentifier != null)
                {
                    // Invalid characters that are hexadecimal digits are appended as code points.
                    if ((!isValidCharacter) && isHexDigit)
                    {
                        // Characters outside the ASCII range (with codes bigger than U+0080) are always allowed unescaped
                        // so 2 hexadecimal digits are enough for character codes.
                        Debug.Assert(c < '\u0080');
                        escapedIdentifier.Append(FormatterPal.IntToStrX2(c));
                    }
                    // Other characters are appended as is. Note that if the current character is invalid, we have written
                    // a backslash that will actually escape the character.
                    else
                    {
                        // If the current character is a hexadecimal digit and the character just before it is also
                        // a hexadecimal digit that belongs to an escaped code point, we must insert a whitespace delimeter
                        // so that the current character will not be concatenated with the preceding code point during parsing.
                        if (afterEscapedCodePoint && isHexDigit)
                        {
                            escapedIdentifier.Append(' ');
                        }

                        escapedIdentifier.Append(c);
                    }
                }

                afterEscapedCodePoint = (!isValidCharacter) && isHexDigit;
                ++i;
            }

            // If we didn't meet any invalid character, then the whole identifier is valid in CSS and it can be used as is.
            // Otherwise, return the escaped version of the identifier.
            return (escapedIdentifier == null)
                ? identifier
                : escapedIdentifier.ToString();
        }

        /// <summary>
        /// Checks whether the text contains only chars that are permitted for CSS identifier.
        /// It is performed by comparing the given text with the <see cref="EscapeIdentifier"/> function output.
        /// </summary>
        /// <param name="identifier">A text to check. Can't be <c>null</c> or empty.</param>
        /// <returns>
        /// Returns <c>false</c> if an escaped version of the text does not match the original text, 
        /// otherwise returns <c>true</c>.
        /// </returns>
        public static bool IsValidIdentifier(string identifier)
        {
            Debug.Assert(StringUtil.HasChars(identifier));
            return identifier == EscapeIdentifier(identifier);
        }
    }
}
