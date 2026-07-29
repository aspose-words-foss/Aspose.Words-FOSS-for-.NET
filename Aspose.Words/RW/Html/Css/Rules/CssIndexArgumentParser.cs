// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/03/2013 by Victor Chebotok

using System.Collections.Generic;
using System.Text;
using Aspose.Common;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Parses arguments of CSS pseudo-classes :nth-child(), :nth-last-child(), :nth-of-type(), and :nth-last-of-type().
    /// </summary>
    /// <remarks>
    /// The argument has the form 'an+b'; its syntax is described here: http://www.w3.org/TR/css3-selectors/#nth-child-pseudo
    /// </remarks>
    internal static class CssIndexArgumentParser
    {
        /// <summary>
        /// Parses the tokens into a pseudo-class argument.
        /// </summary>
        /// <param name="tokens">Tokens of the argument, which will be parsed.</param>
        /// <returns>The parsed argument or <c>null</c> in case of an error.</returns>
        internal static CssIndexArgument Parse(IList<CssToken> tokens)
        {
            if ((tokens == null) || (tokens.Count == 0))
            {
                // Parsing error. The list of argument tokens is empty.
                return null;
            }

            StringBuilder argumentText = new StringBuilder();

            // Rebuild the text of the argument.
            // Check that the argument consists of valid tokens.
            // Replace whitespace characters with the space (U+0020) character.
            for (int i = 0; i < tokens.Count; i++)
            {
                CssToken token = tokens[i];
                if (token.IsDelim('+') || token.IsDelim('-'))
                {
                    char delimChar = ((CssDelimToken)token).Value;
                    argumentText.Append(delimChar);
                    continue;
                }
                switch (token.Type)
                {
                    case CssTokenType.Ident:
                    {
                        argumentText.Append(((CssTextToken)token).Text);
                        break;
                    }
                    case CssTokenType.Dimension:
                    case CssTokenType.Number:
                    {
                        CssNumberToken numberToken = (CssNumberToken)token;
                        if (!numberToken.IsInteger)
                        {
                            // Parsing error. All numbers must be integers. No floating-point or scientific numbers allowed.
                            return null;
                        }

                        if (numberToken.IsSigned && numberToken.Value > 0)
                        {
                            argumentText.Append('+');
                        }
                        argumentText.Append(FormatterPal.DoubleToStr(numberToken.Value));

                        if (token.Type == CssTokenType.Dimension)
                        {
                            argumentText.Append(((CssDimensionToken)numberToken).Unit);
                        }
                        break;
                    }
                    case CssTokenType.Whitespace:
                    {
                        argumentText.Append(' ');
                        break;
                    }
                    default:
                    {
                        // Parsing error. Unexpected token.
                        return null;
                    }
                }
            }

            // Trim leading whitespace.
            int leadingSpaceCount = 0;
            for (int i = 0; i < argumentText.Length; i++)
            {
                if (argumentText[i] != ' ')
                {
                    break;
                }
                ++leadingSpaceCount;
            }
            if (leadingSpaceCount > 0)
            {
                argumentText.Remove(0, leadingSpaceCount);
            }

            // Trim trailing whitespace.
            int trailingSpaceCount = 0;
            for (int i = argumentText.Length - 1; i >= 0; i--)
            {
                if (argumentText[i] != ' ')
                {
                    break;
                }
                ++trailingSpaceCount;
            }
            if (trailingSpaceCount > 0)
            {
                argumentText.Remove(argumentText.Length - trailingSpaceCount, trailingSpaceCount);
            }

            // Convert the argument text to lowercase. Arguments are case-insensitive.
            string trimmedArgumentText = StringUtil.AsciiLowerCase(argumentText.ToString());

            if (trimmedArgumentText.Length == 0)
            {
                // Parsing error. Argument is empty (or contains only whitespace characters).
                return null;
            }

            // Check if the argument is any of the aliases. 
            if (trimmedArgumentText == "even")
            {
                // 'even' is the same as '2n'.
                return new CssIndexArgument(2, 0);
            }
            if (trimmedArgumentText == "odd")
            {
                // 'odd' is the same as '2n+1'.
                return new CssIndexArgument(2, 1);
            }

            int indexOfN = trimmedArgumentText.IndexOf('n');
            if (indexOfN >= 0)
            {
                // The argument is a number in the form 'an+b'. Parse the 'a' and 'b' values.
                string stepString = trimmedArgumentText.Substring(0, indexOfN);
                string offsetString = trimmedArgumentText.Substring(indexOfN + 1);

                int step = (stepString.Length > 0)
                    ? ParseInt(stepString, false, true)
                    : 1;
                if (step == int.MinValue)
                {
                    // Error. Cannot parse step value.
                    return null;
                }

                int offset = (offsetString.Length > 0)
                    ? ParseInt(offsetString, true, false)
                    : 0;
                if (offset == int.MinValue)
                {
                    // Error. Cannot parse offset value.
                    return null;
                }

                return new CssIndexArgument(step, offset);
            }
            else
            {
                // The argument is a number with optional sign.
                int offset = ParseInt(trimmedArgumentText, false, false);
                if (offset == int.MinValue)
                {
                    // Error. Cannot parse offset value.
                    return null;
                }
                return new CssIndexArgument(0, offset);
            }
        }

        /// <summary>
        /// Parses a string into an integer value.
        /// </summary>
        /// <returns>Parsed value or <see cref="int.MinValue"/> in case of an error.</returns>
        private static int ParseInt(string s, bool allowWhitespaceAroundSign, bool allowOnlySign)
        {
            Debug.Assert(s != null);

            // Character index at which the number value starts. Will be calculated as leading whitespace and sign characters
            // are parsed.
            int numberOffset = 0;

            // Skip whitespace characters before the sign character, if whitespace characters are allowed around the sign
            // character.
            if (allowWhitespaceAroundSign)
            {
                // When the argument's text was rebuilt, all whitespace characters were replaced with spaces, so only spaces
                // are skipped here.
                while ((numberOffset < s.Length) && (s[numberOffset] == ' '))
                {
                    ++numberOffset;
                }
            }

            if (numberOffset >= s.Length)
            {
                // Parsing error. Integer value is empty (or contains only whitespace characters).
                return int.MinValue;
            }

            // Parse the sign character if any exists.
            int sign = 1;
            if (s[numberOffset] == '+')
            {
                ++numberOffset;
            }
            else if (s[numberOffset] == '-')
            {
                sign = -1;
                ++numberOffset;
            }

            // Skip whitespace characters after the sign character, if whitespace characters are allowed around the sign
            // character.
            if (allowWhitespaceAroundSign)
            {
                // When the argument's text was rebuilt, all whitespace characters were replaced with spaces, so only spaces
                // are skipped here.
                while ((numberOffset < s.Length) && (s[numberOffset] == ' '))
                {
                    ++numberOffset;
                }
            }

            if (numberOffset >= s.Length)
            {
                if (allowOnlySign)
                {
                    // The number consists only of a sign character. For example, if the argument text is '-n',
                    // the step value consists only of a minus sign, and the step is equal to -1 (minus one).
                    return sign;
                }
                // Parsing error. Integer value consists only of a sign character.
                return int.MinValue;
            }

            // Extract the integer part from the string.
            string number = s.Substring(numberOffset);

            // The integer parsing routine accepts not only digits but also whitespace and sign characters,
            // so we perform an additional check to ensure that the string contains nothing but digits
            // before trying to parse it.
            if (StringUtil.IsDecimal(number))
            {
                int value = FormatterPal.TryParseInt(number);
                if (value >= 0)
                {
                    return sign * value;
                }
            }
            // Parsing error. Integer value contains characters other than digits.
            return int.MinValue;
        }
    }
}
