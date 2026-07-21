// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/01/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a CSS parser token.
    /// </summary>
    internal class CssToken
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="tokenType">Token type.</param>
        internal CssToken(CssTokenType tokenType)
        {
            Type = tokenType;
        }

        /// <summary>
        /// Token type.
        /// </summary>
        internal CssTokenType Type { get; }

        internal bool IsDelim(char delimChar)
        {
            return (Type == CssTokenType.Delim) && (((CssDelimToken)this).Value == delimChar);
        }

        internal bool IsEof()
        {
            return Type == CssTokenType.Eof;
        }

        internal bool IsWhitespace()
        {
            return Type == CssTokenType.Whitespace;
        }

        internal static readonly CssToken Eof = new CssToken(CssTokenType.Eof);
    }
}
