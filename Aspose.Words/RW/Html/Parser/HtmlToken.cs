// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// The base class for different types of tokens returned by the <see cref="HtmlTokenizer"/>.
    /// </summary>
    internal abstract class HtmlToken
    {
        protected HtmlToken(HtmlTokenType type)
        {
            mType = type;
        }

        /// <summary>
        /// The type of the token.
        /// </summary>
        internal HtmlTokenType Type
        {
            get { return mType; }
        }

        private readonly HtmlTokenType mType;
    }
}
