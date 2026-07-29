// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/04/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// The mode in which an HTML document should be processed and displayed.
    /// </summary>
    internal enum HtmlDocumentMode
    {
        /// <remarks>
        /// The document should be processed according to the HTML and CSS specifications. Also known as 'no-quirks' mode.
        /// </remarks>
        Standards,

        /// <summary>
        /// The document should be processed according to the HTML and CSS specifications, except for calculation of height of table
        /// cells containing only images. Also known as 'almost-standards' mode.
        /// </summary>
        LimitedQuirks,

        /// <summary>
        /// A number of layout and other quirks should be used during processing the document. Also known as 'compatibility' mode.
        /// </summary>
        Quirks
    }
}
