// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// The token that represents the text of a comment node.
    /// </summary>
    internal class HtmlCommentToken : HtmlToken
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text">The text of the comment.</param>
        internal HtmlCommentToken(string text)
            : base(HtmlTokenType.Comment)
        {
            Debug.Assert(text != null);

            mText = text;
        }

        /// <summary>
        /// Gets the text of the comment.
        /// </summary>
        internal string Text
        {
            get { return mText; }
        }

        private readonly string mText;
    }
}