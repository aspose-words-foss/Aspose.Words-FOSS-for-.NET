// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// The special token that indicates that no other tokens will be returned by the tokenizer.
    /// </summary>
    internal class HtmlEndOfFileToken : HtmlToken
    {
        internal HtmlEndOfFileToken()
            : base(HtmlTokenType.EndOfFile)
        {
            // Empty constructor.
        }
    }
}