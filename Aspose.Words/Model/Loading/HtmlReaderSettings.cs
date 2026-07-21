// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2021 by Victor Chebotok

using Aspose.Words.Settings;

namespace Aspose.Words.Loading
{
    /// <summary>
    /// Settings applied when loading HTML content from documents of various HTML-based formats.
    /// </summary>
    internal class HtmlReaderSettings
    {
        /// <summary>
        /// Indicates whether VML images are recognized and imported from HTML.
        /// </summary>
        internal bool SupportVml;

        /// <summary>
        /// Indicates whether we should stick to MS Word's rules when applying CSS formatting to document model.
        /// </summary>
        internal bool ApplyFormattingAsMsWord;

        /// <summary>
        /// Indicates whether certain 'input' elements are imported as Form Fild/SDT nodes.
        /// </summary>
        internal HtmlControlType PreferredControlType;

        /// <summary>
        /// Version of MS Word that Aspose.Words should mimic when loading HTML.
        /// </summary>
        internal MsWordVersion MswVersion = MsWordVersion.Word2019;

        /// <summary>
        /// Indicates whether to ignore &lt;noscript&gt; HTML elements.
        /// </summary>
        internal bool IgnoreNoscriptElements;

        /// <summary>
        /// Indicates whether to convert loaded SVG images to the EMF format.
        /// </summary>
        internal bool ConvertSvgToEmf;

        /// <summary>
        /// Specifies the original file name for CHM documents. May be empty or <c>null</c>.
        /// </summary>
        internal string OriginalChmFileName;

        /// <summary>
        /// Instructs the reader to try to parse the document as XHTML first.
        /// </summary>
        /// <remarks>
        /// <para>
        ///  If parsing of XHTML is disabled by setting this option to <c>false</c> or if parsing of document as XHTML fails,
        ///  the reader parses the document as HTML 5.
        /// </para>
        /// <para>
        /// Currently, this option doesn't affect reading of HTML fragments. Fragments are always parsed as HTML 5.
        /// </para>
        /// </remarks>
        internal bool ParseAsXhtml;

        /// <summary>
        /// Indicates whether to mimic MS Word's behavior and use <see cref="HtmlBlock"/> nodes to preserve borders and margins
        /// of block-level elements like BODY, DIV, or BLOCKQUOTE.
        /// </summary>
        internal bool UseHtmlBlocks;

        /// <summary>
        /// Indicates whether to support certain self-closing non-HTML tags.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is a non-standard feature but it improves performance when loading some MOBI documents.
        /// </para>
        /// <para>
        /// According to the HTML5 specification, all tags that are not part of HTML cannot be self-closing. When such tags are
        /// used extensively, however, the resulting HTML tree may become too deeply nested and hard to process. This happens,
        /// for example, for "mbp:pagebreak" tags that are used in MOBI documents to break them into pages.
        /// </para>
        /// </remarks>
        internal bool SupportSelfClosingNonHtmlTags;

        /// <summary>
        /// Indicates whether to support @font-face CSS rules and whether to load fonts declared in these rules.
        /// </summary>
        internal bool SupportFontFaceRules;

        /// <summary>
        /// Indicates whether to keep bookmarks that are created for HTML elements with "id" attributes but are not referenced
        /// by any hyperlinks in this HTML file.
        /// </summary>
        /// <remarks>
        /// When loading formats like EPUB or CHM, such bookmarks should be kept, because they may be referenced from other HTML
        /// files (topics) of the source document.
        /// </remarks>
        internal bool KeepUnreferencedIdBookmarks;

        /// <summary>
        /// Format-specific processor of hyperlink hrefs and bookmark names.
        /// </summary>
        /// <remarks>
        /// Used to resolve ambiguities when loading formats like EPUB or CHM and combining multiple HTML files (topics)
        /// into a single resulting document.
        /// </remarks>
        internal IHyperlinkProcessor HyperlinkProcessor;
    }
}
