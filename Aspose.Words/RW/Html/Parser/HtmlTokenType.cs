// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// Types of tokens returned by <see cref="HtmlTokenizer"/>.
    /// </summary>
    internal enum HtmlTokenType
    {
        /// <summary>
        /// The text that is not a part of HTML markup.
        /// </summary>
        Text,

        /// <summary>
        /// The text from a comment node.
        /// </summary>
        Comment,
        
        /// <summary>
        /// The DOCTYPE node.
        /// </summary>
        Doctype,

        /// <summary>
        /// The tag node.
        /// </summary>
        Tag,

        /// <summary>
        /// The special token that indicates that the end of HTML is reached and no other tokens will be returned.
        /// </summary>
        EndOfFile
    }
}
