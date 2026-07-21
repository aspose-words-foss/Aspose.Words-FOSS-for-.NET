// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/01/2013 by Alexey Butalov

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents CSS token type.
    /// </summary>
    /// <remarks>
    /// See https://www.w3.org/TR/css-syntax-3/#tokenization
    /// </remarks>
    internal enum CssTokenType
    {
        /// <summary>
        /// Ident token. The token class is <see cref="CssTextToken"/>.
        /// </summary>
        Ident,

        /// <summary>
        /// Function token. The token class is <see cref="CssTextToken"/>.
        /// </summary>
        Function,

        /// <summary>
        /// At-keyword token. The token class is <see cref="CssTextToken"/>.
        /// </summary>
        AtKeyword,

        /// <summary>
        /// Hash token. The token class is <see cref="CssHashToken"/>.
        /// </summary>
        Hash,

        /// <summary>
        /// String token. The token class is <see cref="CssTextToken"/>.
        /// </summary>
        String,

        /// <summary>
        /// Bad string token. The token class is <see cref="CssToken"/>.
        /// </summary>
        BadString,

        /// <summary>
        /// URL token. The token class is <see cref="CssTextToken"/>.
        /// </summary>
        Url,

        /// <summary>
        /// Bad URL token. The token class is <see cref="CssToken"/>.
        /// </summary>
        BadUrl,

        /// <summary>
        /// Delim (delimiter) token. The token class is <see cref="CssDelimToken"/>.
        /// </summary>
        Delim,

        /// <summary>
        /// Number token. The token class is <see cref="CssNumberToken"/>.
        /// </summary>
        Number,

        /// <summary>
        /// Percentage token. The token class is <see cref="CssNumberToken"/>.
        /// </summary>
        Percentage,

        /// <summary>
        /// Dimension token. The token class is <see cref="CssDimensionToken"/>.
        /// </summary>
        Dimension,

        /// <summary>
        /// Whitespace token. The token class is <see cref="CssToken"/>.
        /// </summary>
        Whitespace,

        /// <summary>
        /// CDO (comment delimiter open) token. The token class is <see cref="CssToken"/>.
        /// </summary>
        Cdo,

        /// <summary>
        /// CDC (comment delimiter close) token. The token class is <see cref="CssToken"/>.
        /// </summary>
        Cdc,

        /// <summary>
        /// Colon token. The token class is <see cref="CssToken"/>.
        /// </summary>
        Colon,

        /// <summary>
        /// Semicolon token. The token class is <see cref="CssToken"/>.
        /// </summary>
        Semicolon,

        /// <summary>
        /// Comma token. The token class is <see cref="CssToken"/>.
        /// </summary>
        Comma,

        /// <summary>
        /// Left round bracket "(" token. The token class is <see cref="CssToken"/>.
        /// </summary>
        RoundBracketLeft,

        /// <summary>
        /// Right round bracket ")" token. The token class is <see cref="CssToken"/>.
        /// </summary>
        RoundBracketRight,

        /// <summary>
        /// Left block bracket "{" token. The token class is <see cref="CssToken"/>.
        /// </summary>
        BlockBracketLeft,

        /// <summary>
        /// Right round bracket "}" token. The token class is <see cref="CssToken"/>.
        /// </summary>
        BlockBracketRight,

        /// <summary>
        /// Left square bracket "[" token. The token class is <see cref="CssToken"/>.
        /// </summary>
        SquareBracketLeft,

        /// <summary>
        /// Right square bracket "]" token. The token class is <see cref="CssToken"/>.
        /// </summary>
        SquareBracketRight,

        /// <summary>
        /// End of file token. The token class is <see cref="CssToken"/>.
        /// </summary>
        Eof
    }
}
