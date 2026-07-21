// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// A bookmarked position in the <see cref="HtmlStream"/>. 
    /// </summary>
    /// <remarks>
    /// The position is used by <see cref="HtmlTokenizer"/> to reparse characters in case of an error.
    /// </remarks>
    internal class HtmlStreamBookmark
    {
        internal HtmlStreamBookmark(HtmlStream stream, int position)
        {
            mStream = stream;
            mPosition = position;
        }

        internal HtmlStream Stream
        {
            get { return mStream; }
        }

        internal int Position
        {
            get { return mPosition; }
        }

        private readonly HtmlStream mStream;

        private readonly int mPosition;
    }
}
