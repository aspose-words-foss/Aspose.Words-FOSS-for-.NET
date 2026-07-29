// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// The token that represents text that is not a part of HTML markup. 
    /// </summary>
    internal class HtmlTextToken : HtmlToken
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="text">The text of the token.</param>
        internal HtmlTextToken(string text)
            : base(HtmlTokenType.Text)
        {
            Debug.Assert(text != null);

            mText = text;
        }

        /// <summary>
        /// Gets the text of the token.
        /// </summary>
        internal string Text
        {
            get { return mText; }
        }

        private readonly string mText;
    }
}
