// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2024 by Victor Chebotok

using Aspose.JavaAttributes;

namespace Aspose.Words.Loading
{
    /// <summary>
    /// Format-specific processor of hyperlink hrefs and bookmark names.
    /// </summary>
    internal interface IHyperlinkProcessor
    {
        /// <summary>
        /// Indicates whether the specified "href" is a valid address for a hyperlink in the document model.
        /// </summary>
        [JavaThrows(true)]
        bool IsValidHyperlinkHref(string href);

        /// <summary>
        /// Maps a hyperlink href loaded from an individual HTML file into a href that can be used in the resulting document
        /// without conflicting with hrefs imported from other HTML files.
        /// </summary>
        string MapHyperlinkHref(string href);

        /// <summary>
        /// Maps a bookmark name loaded from an individual HTML file into a unique bookmark name that can be used
        /// in the resulting document.
        /// </summary>
        string MapBookmarkName(string name);
    }
}
