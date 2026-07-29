// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/03/2011 by Andrey Kamimura

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies how headers and footers are exported to HTML, MHTML or EPUB.
    /// </summary>
    /// <seealso cref="HtmlSaveOptions.ExportHeadersFootersMode"/>
    public enum ExportHeadersFootersMode
    {
        /// <summary>
        /// Headers and footers are not exported.
        /// </summary>
        None = 0,
        /// <summary>
        /// Primary headers and footers are exported at the beginning and the end of each section.
        /// </summary>
        PerSection = 1,
        /// <summary>
        /// Primary header of the first section is exported at the beginning of the document and primary footer is at the end.
        /// </summary>
        FirstSectionHeaderLastSectionFooter = 2,
        /// <summary>
        /// First page header and footer are exported at the beginning and the end of each section.
        /// </summary>
        FirstPageHeaderFooterPerSection = 3
    }
}
