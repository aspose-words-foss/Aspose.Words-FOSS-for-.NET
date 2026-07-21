// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/06/2019 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// A source that provides CSS tokens for the CSS parser.
    /// </summary>
    internal abstract class CssTokenSource
    {
        internal bool HasMoreTokens()
        {
            CssToken nextToken = Consume();
            Reconsume();
            return !nextToken.IsEof();
        }

        internal abstract CssToken Consume();

        internal abstract void Reconsume();

        internal void ConsumeOptionalWhitespace()
        {
            CssToken token = Consume();
            if (token.IsWhitespace())
            {
                Consume();
            }
            Reconsume();
        }

        /// <summary>
        /// Skips optional whitespace and consumes a token.
        /// </summary>
        internal CssToken ConsumeWhitespaceAndToken()
        {
            CssToken token = Consume();
            return token.IsWhitespace()
                ? Consume()
                : token;
        }
    }
}
