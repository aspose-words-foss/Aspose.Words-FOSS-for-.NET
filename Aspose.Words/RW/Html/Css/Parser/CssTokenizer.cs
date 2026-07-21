// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/06/2019 by Victor Chebotok

using System.Collections.Generic;
using System.Text;
using Aspose.Bidi;
using Aspose.Common;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// CSS tokenizer. Splits CSS text into CSS tokens.
    /// </summary>
    /// <remarks>
    /// See https://www.w3.org/TR/css-syntax-3/ and the latest editor's draft at https://drafts.csswg.org/css-syntax/
    /// </remarks>
    internal class CssTokenizer
    {
        /// <summary>
        /// Returns all CSS tokens of the provided CSS text.
        /// </summary>
        internal static IList<CssToken> GetTokens(string css)
        {
            List<CssToken> result = new List<CssToken>();
            CssTokenizer tokenizer = new CssTokenizer(css);
            while (tokenizer.MoveNext())
            {
                result.Add(tokenizer.CurrentToken);
            }
            return result;
        }

        /// <summary>
        /// Constructor. Initializes a new instance of the tokenizer to process the provided CSS text.
        /// </summary>
        /// <param name="css">CSS text to split into tokens.</param>
        internal CssTokenizer(string css)
        {
            mStream = new CssTokenizerStream(css);
        }

        /// <summary>
        /// Advances the tokenizer by one token.
        /// </summary>
        /// <returns>
        /// A value indicating whether another token has been parsed to <see cref="CurrentToken"/>.
        /// <c>true</c> - parsed another token. <c>false</c> - no more tokens, reached the end of the CSS text.
        /// </returns>
        internal bool MoveNext()
        {
            CurrentToken = CssToken.Eof;

            if (ConsumeWhitespaceWithOptionalComments())
            {
                CurrentToken = gWhitespace;
                return true;
            }

            if (mStream.IsAtEof())
            {
                return false;
            }

            mStream.ConsumeChar();

            if (IsNameStartCodePoint(mStream.Current))
            {
                mStream.ReconsumeChar();
                CurrentToken = ConsumeIdentLikeToken();
                return true;
            }

            if (StringUtil.IsDigit(mStream.Current))
            {
                mStream.ReconsumeChar();
                CurrentToken = ConsumeNumericToken();
                return true;
            }

            switch (mStream.Current)
            {
                case '"':
                case '\'':
                {
                    CurrentToken = ConsumeStringToken();
                    break;
                }
                case '#':
                {
                    CurrentToken = ConsumeHashToken();
                    break;
                }
                case '(':
                {
                    CurrentToken = gRoundBracketLeft;
                    break;
                }
                case ')':
                {
                    CurrentToken = gRoundBracketRight;
                    break;
                }
                case '+':
                {
                    if (IsNumber(mStream.Current, mStream.Next, mStream.Next2))
                    {
                        mStream.ReconsumeChar();
                        CurrentToken = ConsumeNumericToken();
                    }
                    else
                    {
                        CurrentToken = CreateDelimToken(mStream.Current);
                    }
                    break;
                }
                case ',':
                {
                    CurrentToken = gComma;
                    break;
                }
                case '-':
                {
                    if (IsNumber(mStream.Current, mStream.Next, mStream.Next2))
                    {
                        mStream.ReconsumeChar();
                        CurrentToken = ConsumeNumericToken();
                    }
                    else if (IsIdentifier(mStream.Current, mStream.Next, mStream.Next2))
                    {
                        mStream.ReconsumeChar();
                        CurrentToken = ConsumeIdentLikeToken();
                    }
                    else if ((mStream.Next == '-') && (mStream.Next2 == '>'))
                    {
                        mStream.ConsumeChar(); // '-'
                        mStream.ConsumeChar(); // '>'
                        CurrentToken = gCdc;
                    }
                    else
                    {
                        CurrentToken = CreateDelimToken(mStream.Current);
                    }
                    break;
                }
                case '.':
                {
                    if (IsNumber(mStream.Current, mStream.Next, mStream.Next2))
                    {
                        mStream.ReconsumeChar();
                        CurrentToken = ConsumeNumericToken();
                    }
                    else
                    {
                        CurrentToken = CreateDelimToken(mStream.Current);
                    }
                    break;
                }
                case ':':
                {
                    CurrentToken = gColon;
                    break;
                }
                case ';':
                {
                    CurrentToken = gSemicolon;
                    break;
                }
                case '<':
                {
                    if ((mStream.Next == '!') && (mStream.Next2 == '-') && (mStream.Next3 == '-'))
                    {
                        mStream.ConsumeChar(); // '!'
                        mStream.ConsumeChar(); // '-'
                        mStream.ConsumeChar(); // '-'
                        CurrentToken = gCdo;
                    }
                    else
                    {
                        CurrentToken = CreateDelimToken(mStream.Current);
                    }
                    break;
                }
                case '@':
                {
                    if (IsIdentifier(mStream.Next, mStream.Next2, mStream.Next3))
                    {
                        string name = ConsumeName();
                        CurrentToken = new CssTextToken(CssTokenType.AtKeyword, name);
                    }
                    else
                    {
                        CurrentToken = CreateDelimToken(mStream.Current);
                    }
                    break;
                }
                case '[':
                {
                    CurrentToken = gSquareBracketLeft;
                    break;
                }
                case ']':
                {
                    CurrentToken = gSquareBracketRight;
                    break;
                }
                case '\\':
                {
                    if (IsValidEscape(mStream.Current, mStream.Next))
                    {
                        mStream.ReconsumeChar();
                        CurrentToken = ConsumeIdentLikeToken();
                    }
                    else
                    {
                        CurrentToken = CreateDelimToken(mStream.Current);
                    }
                    break;
                }
                case '{':
                {
                    CurrentToken = gBlockBracketLeft;
                    break;
                }
                case '}':
                {
                    CurrentToken = gBlockBracketRight;
                    break;
                }
                default:
                {
                    CurrentToken = CreateDelimToken(mStream.Current);
                    break;
                }
            }

            return true;
        }

        /// <summary>
        /// The last parsed CSS token or <see cref="CssToken.Eof"/> if nothing has been parsed.
        /// </summary>
        internal CssToken CurrentToken { get; private set; }

        private CssToken ConsumeIdentLikeToken()
        {
            string name = ConsumeName();

            if (mStream.Next == '(')
            {
                if (StringUtil.AsciiLowerCase(name) == "url")
                {
                    mStream.ConsumeChar();
                    return ConsumeUrl();
                }
                else
                {
                    mStream.ConsumeChar();
                    return new CssTextToken(CssTokenType.Function, name);
                }
            }
            return new CssTextToken(CssTokenType.Ident, name);
        }

        private CssToken ConsumeUrl()
        {
            ConsumeWhitespace();

            if (mStream.IsAtEof())
            {
                return new CssTextToken(CssTokenType.Url, string.Empty);
            }

            if ((mStream.Next == '"') || (mStream.Next == '\''))
            {
                mStream.ConsumeChar();
                CssToken urlStringToken = ConsumeStringToken();
                if (urlStringToken.Type == CssTokenType.BadString)
                {
                    return ConsumeBadUrl();
                }

                ConsumeWhitespace();

                bool endedNormally = mStream.Next == ')';
                if (endedNormally || mStream.IsAtEof())
                {
                    if (endedNormally)
                    {
                        mStream.ConsumeChar();
                    }
                    return new CssTextToken(CssTokenType.Url, ((CssTextToken)urlStringToken).Text);
                }

                return ConsumeBadUrl();
            }

            mTextBuffer.Length = 0;
            while (true)
            {
                mStream.ConsumeChar();

                if (IsWhitespace(mStream.Current))
                {
                    ConsumeWhitespace();

                    bool endedNormally = mStream.Next == ')';
                    if (endedNormally || mStream.IsAtEof())
                    {
                        if (endedNormally)
                        {
                            mStream.ConsumeChar();
                        }
                        return new CssTextToken(CssTokenType.Url, mTextBuffer.ToString());
                    }

                    return ConsumeBadUrl();
                }

                if (IsNonPrintable(mStream.Current))
                {
                    return ConsumeBadUrl();
                }

                switch (mStream.Current)
                {
                    case ')':
                    case CssTokenizerStream.EofChar:
                    {
                        return new CssTextToken(CssTokenType.Url, mTextBuffer.ToString());
                    }
                    case '"':
                    case '\'':
                    case '(':
                    {
                        return ConsumeBadUrl();
                    }
                    case '\\':
                    {
                        if (IsValidEscape(mStream.Current, mStream.Next))
                        {
                            ConsumeEscapedCodePoint(mTextBuffer);
                        }
                        else
                        {
                            return ConsumeBadUrl();
                        }
                        break;
                    }
                    default:
                    {
                        mTextBuffer.Append(mStream.Current);
                        break;
                    }
                }
            }
        }

        private CssToken ConsumeBadUrl()
        {
            mTextBuffer.Length = 0;
            while (true)
            {
                mStream.ConsumeChar();
                if ((mStream.Current == ')') || (mStream.Current == CssTokenizerStream.EofChar))
                {
                    return gBadUrl;
                }
                if (IsValidEscape(mStream.Current, mStream.Next))
                {
                    ConsumeEscapedCodePoint(mTextBuffer);
                }
            }
        }

        private CssToken ConsumeHashToken()
        {
            if (IsNameCodePoint(mStream.Next) || IsValidEscape(mStream.Next, mStream.Next2))
            {
                bool isIdentifier = IsIdentifier(mStream.Next, mStream.Next2, mStream.Next3);
                return new CssHashToken(ConsumeName(), isIdentifier);
            }
            return CreateDelimToken(mStream.Current);
        }

        private CssToken ConsumeNumericToken()
        {
            bool isInteger = true;
            bool isSigned = false;
            bool isNegative = false;

            mTextBuffer.Length = 0;

            // Optional sign.
            if (mStream.Next == '-')
            {
                // This is used to resolve parsing errors and clamp too large and too small values to the valid range.
                isNegative = true;
            }
            if ((mStream.Next == '+') || (mStream.Next == '-'))
            {
                isSigned = true;
                mStream.ConsumeChar();
                mTextBuffer.Append(mStream.Current);
            }

            // Whole part.
            while (StringUtil.IsDigit(mStream.Next))
            {
                mStream.ConsumeChar();
                mTextBuffer.Append(mStream.Current);
            }

            // Optional fractional part.
            if ((mStream.Next == '.') && StringUtil.IsDigit(mStream.Next2))
            {
                // It's not an integer. It's a floating-point number.
                isInteger = false;

                mStream.ConsumeChar();
                mTextBuffer.Append(mStream.Current);
                mStream.ConsumeChar();
                mTextBuffer.Append(mStream.Current);

                while (StringUtil.IsDigit(mStream.Next))
                {
                    mStream.ConsumeChar();
                    mTextBuffer.Append(mStream.Current);
                }
            }

            // Optional exponent part. For example, "e123" or "E+456".
            bool hasNegativeExponent = false;
            char a = mStream.Next;
            char b = mStream.Next2;
            char c = mStream.Next3;
            if ((a == 'e') || (a == 'E'))
            {
                if (b == '-')
                {
                    // This is used to resolve parsing errors and clamp too large and too small values to the valid range.
                    hasNegativeExponent = true;
                }

                int charactersToConsume = 0;
                if (StringUtil.IsDigit(b))
                {
                    charactersToConsume = 2;
                }
                else if (((b == '+') || (b == '-')) && StringUtil.IsDigit(c))
                {
                    charactersToConsume = 3;
                }

                // If a valid exponent prefix found.
                if (charactersToConsume > 0)
                {
                    // It's not an integer. It's a number in the scientific notation.
                    isInteger = false;

                    for (int i = 0; i < charactersToConsume; i++)
                    {
                        mStream.ConsumeChar();
                        mTextBuffer.Append(mStream.Current);
                    }

                    while (StringUtil.IsDigit(mStream.Next))
                    {
                        mStream.ConsumeChar();
                        mTextBuffer.Append(mStream.Current);
                    }
                }
            }

            // Convert string to number. Take the exponent into consideration.
            double value = FormatterPal.TryParseDoubleInvariant(mTextBuffer.ToString());

            //Java parse double differently
            if (double.IsInfinity(value))
            {
                value = isNegative
                        ? double.MinValue
                        : double.MaxValue;
            }

            if (double.IsNaN(value))
            {
                // We get here if the exponent is too large and the number is either too large or too small.
                if (hasNegativeExponent)
                {
                    // The number is too small.
                    value = 0;
                }
                else
                {
                    // The number is too large.
                    value = isNegative
                        ? double.MinValue
                        : double.MaxValue;
                }
            }

            if (IsIdentifier(mStream.Next, mStream.Next2, mStream.Next3))
            {
                string unit = ConsumeName();
                return new CssDimensionToken(value, unit, isInteger, isSigned);
            }

            CssTokenType numberTokenType = CssTokenType.Number;
            if (mStream.Next == '%')
            {
                mStream.ConsumeChar();
                numberTokenType = CssTokenType.Percentage;
            }

            return new CssNumberToken(numberTokenType, value, isInteger, isSigned);
        }

        private static bool IsNumber(char a, char b, char c)
        {
            if (StringUtil.IsDigit(a))
            {
                return true;
            }

            switch (a)
            {
                case '+':
                case '-':
                    return StringUtil.IsDigit(b) || ((b == '.') && StringUtil.IsDigit(c));
                case '.':
                    return StringUtil.IsDigit(b);
                default:
                    return false;
            }
        }

        private static bool IsIdentifier(char a, char b, char c)
        {
            if (IsNameStartCodePoint(a))
            {
                return true;
            }

            if (a == '-')
            {
                return IsNameStartCodePoint(b) || IsValidEscape(b, c);
            }

            if (a == '\\')
            {
                return IsValidEscape(a, b);
            }

            return false;
        }

        private string ConsumeName()
        {
            mTextBuffer.Length = 0;
            while (mStream.ConsumeChar())
            {
                if (IsNameCodePoint(mStream.Current))
                {
                    mTextBuffer.Append(mStream.Current);
                }
                else if (IsValidEscape(mStream.Current, mStream.Next))
                {
                    ConsumeEscapedCodePoint(mTextBuffer);
                }
                else
                {
                    mStream.ReconsumeChar();
                    break;
                }
            }
            return mTextBuffer.ToString();
        }

        private static bool IsValidEscape(char a, char b)
        {
            return (a == '\\') && (b != CssTokenizerStream.EofChar) && (b != '\n');
        }

        private CssToken ConsumeStringToken()
        {
            mTextBuffer.Length = 0;
            char endingCharacter = mStream.Current;
            while (mStream.ConsumeChar() && (mStream.Current != endingCharacter))
            {
                switch (mStream.Current)
                {
                    case '\n':
                    {
                        mStream.ReconsumeChar();
                        return gBadString;
                    }
                    case '\\':
                    {
                        if (!mStream.IsAtEof())
                        {
                            if (mStream.Next == '\n')
                            {
                                mStream.ConsumeChar();
                            }
                            else
                            {
                                ConsumeEscapedCodePoint(mTextBuffer);
                            }
                        }
                        break;
                    }
                    default:
                    {
                        mTextBuffer.Append(mStream.Current);
                        break;
                    }
                }
            }

            return new CssTextToken(CssTokenType.String, mTextBuffer.ToString());
        }

        private void ConsumeEscapedCodePoint(StringBuilder stringBuilder)
        {
            if (mStream.IsAtEof())
            {
                stringBuilder.Append(UnicodeUtil.ReplacementChar);
                return;
            }

            mStream.ConsumeChar();

            if (StringUtil.IsHexDigit(mStream.Current))
            {
                mHexNumberBuffer.Length = 0;
                mHexNumberBuffer.Append(mStream.Current);
                // Consume as many hex digits as possible but no more than max length.
                while ((mHexNumberBuffer.Length < MaxEscapedCodePointLength) &&
                    StringUtil.IsHexDigit(mStream.Next))
                {
                    mStream.ConsumeChar();
                    mHexNumberBuffer.Append(mStream.Current);
                }
                if (IsWhitespace(mStream.Next))
                {
                    mStream.ConsumeChar();
                }
                ParseHexEscapeCode(stringBuilder, mHexNumberBuffer.ToString());
            }
            else
            {
                stringBuilder.Append(mStream.Current);
            }
        }

        /// <summary>
        /// Consumes optional sequence of whitespace characters.
        /// </summary>
        /// <returns>
        /// A value indicating whether any whitespace has been consumed.
        /// </returns>
        private bool ConsumeWhitespace()
        {
            bool anyWhitespaceConsumed = false;
            while (IsWhitespace(mStream.Next))
            {
                mStream.ConsumeChar();
                anyWhitespaceConsumed = true;
            }
            return anyWhitespaceConsumed;
        }

        /// <summary>
        /// Consumes optional whitespace sequences interleaved with optional comments.
        /// </summary>
        /// <returns>
        /// A value indicating whether any whitespace has been consumed.
        /// </returns>
        private bool ConsumeWhitespaceWithOptionalComments()
        {
            bool comsumedMoreWhitespace = true;
            bool anyWhitespaceConsumed = false;
            while (comsumedMoreWhitespace)
            {
                ConsumeComments();
                comsumedMoreWhitespace = ConsumeWhitespace();
                if (comsumedMoreWhitespace)
                {
                    anyWhitespaceConsumed = true;
                }
            }
            return anyWhitespaceConsumed;
        }

        /// <summary>
        /// Consumes zero or more consecutive comments: /* ... */
        /// </summary>
        private void ConsumeComments()
        {
            while ((mStream.Next == '/') && (mStream.Next2 == '*'))
            {
                mStream.ConsumeChar(); // '/'
                mStream.ConsumeChar(); // '*'

                // In case the comment is not closed properly we might end up skipping everything up to the EOF.
                while (!mStream.IsAtEof())
                {
                    mStream.ConsumeChar(); // This might be the '*' character of the closing sequence "*/".

                    if ((mStream.Current == '*') && (mStream.Next == '/'))
                    {
                        mStream.ConsumeChar(); // '/'

                        // Reached the closing sequence "*/" of the comment.
                        break;
                    }
                }
            }
        }

        private static bool IsWhitespace(char c)
        {
            return (c == ' ') || (c == '\n') || (c == '\t');
        }

        private static bool IsNameStartCodePoint(char c)
        {
            return StringUtil.IsLetter(c) || (c == '_') || (c >= '\u0080');
        }

        private static bool IsNameCodePoint(char c)
        {
            return IsNameStartCodePoint(c) || StringUtil.IsDigit(c) || (c == '-');
        }

        private static bool IsNonPrintable(char c)
        {
            // Note that zero code points (U+0000 NULL) are not on this list, because they are removed from the source CSS
            // during preprocessing and our tokenizer emits U+0000 NULL at end-of-file.
            return ((c >= '\u0001') && (c <= '\u0008')) ||
                (c == '\u000B') ||
                ((c >= '\u000E') && (c <= '\u001F')) ||
                (c == '\u007F');
        }

        /// <summary>
        /// Parses an escaped character and appends it to the provided string builder.
        /// </summary>
        /// <param name="stringBuilder">A string builder where the parsed character will be appended to.</param>
        /// <param name="escapedCode">The hexadecimal character code without the leading backslash character.</param>
        /// <returns>A character or a group of characters that corresponds to the hexadecimal code. When the escaped character
        /// is invalid in CSS (is zero, surrogate, or out of Unicode range), the replacement character is returned.</returns>
        /// <remarks>
        /// See https://www.w3.org/TR/css-syntax-3/#consume-an-escaped-code-point
        /// </remarks>
        private static void ParseHexEscapeCode(
            StringBuilder stringBuilder,
            string escapedCode)
        {
            int codePoint = FormatterPal.ParseHex(escapedCode);
            if ((codePoint == 0) || !UnicodeUtil.IsValidUtf32(codePoint) || UnicodeUtil.IsSurrogate(codePoint))
            {
                // Replace an invalid character.
                stringBuilder.Append(UnicodeUtil.ReplacementChar);
            }
            else
            {
                UnicodeUtil.AppendUtf32(stringBuilder, codePoint);
            }
        }

        private static CssToken CreateDelimToken(char c)
        {
            switch (c)
            {
                case '!':
                    return gExclamationMark;
                case '~':
                    return gTilde;
                case '=':
                    return gEqualsSign;
                case '>':
                    return gGreaterThanSign;
                case '|':
                    return gDash;
                case '+':
                    return gPlus;
                case '-':
                    return gMinus;
                case '*':
                    return gAsterisk;
                case '/':
                    return gSlash;
                default:
                    return new CssDelimToken(c);
            }
        }

        private readonly CssTokenizerStream mStream;
        private readonly StringBuilder mTextBuffer = new StringBuilder();

        /// <summary>
        /// The max length in hex digits of an escaped code point.
        /// </summary>
        private const int MaxEscapedCodePointLength = 6;

        /// <summary>
        /// A temporary buffer used to parse escaped code points.
        /// </summary>
        private readonly StringBuilder mHexNumberBuffer = new StringBuilder(MaxEscapedCodePointLength);

        // We pre-create commonly used tokens to re-use them and reduce memory consumption.
        private static readonly CssToken gWhitespace = new CssToken(CssTokenType.Whitespace);
        private static readonly CssToken gComma = new CssToken(CssTokenType.Comma);
        private static readonly CssToken gColon = new CssToken(CssTokenType.Colon);
        private static readonly CssToken gSemicolon = new CssToken(CssTokenType.Semicolon);
        private static readonly CssToken gCdo = new CssToken(CssTokenType.Cdo);
        private static readonly CssToken gCdc = new CssToken(CssTokenType.Cdc);
        private static readonly CssToken gRoundBracketLeft = new CssToken(CssTokenType.RoundBracketLeft);
        private static readonly CssToken gRoundBracketRight = new CssToken(CssTokenType.RoundBracketRight);
        private static readonly CssToken gSquareBracketLeft = new CssToken(CssTokenType.SquareBracketLeft);
        private static readonly CssToken gSquareBracketRight = new CssToken(CssTokenType.SquareBracketRight);
        private static readonly CssToken gBlockBracketLeft = new CssToken(CssTokenType.BlockBracketLeft);
        private static readonly CssToken gBlockBracketRight = new CssToken(CssTokenType.BlockBracketRight);
        private static readonly CssToken gBadUrl = new CssToken(CssTokenType.BadUrl);
        private static readonly CssToken gBadString = new CssToken(CssTokenType.BadString);
        private static readonly CssToken gExclamationMark = new CssDelimToken('!');
        private static readonly CssToken gTilde = new CssDelimToken('~');
        private static readonly CssToken gEqualsSign = new CssDelimToken('=');
        private static readonly CssToken gGreaterThanSign = new CssDelimToken('>');
        private static readonly CssToken gDash = new CssDelimToken('|');
        private static readonly CssToken gPlus = new CssDelimToken('+');
        private static readonly CssToken gMinus = new CssDelimToken('-');
        private static readonly CssToken gAsterisk = new CssDelimToken('*');
        private static readonly CssToken gSlash = new CssDelimToken('/');
    }
}
