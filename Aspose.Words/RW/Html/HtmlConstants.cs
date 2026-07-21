// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/11/2013 by Anton Savko

namespace Aspose.Words.RW.Html
{
    /// <summary>
    /// Common constants for reader/writer classes.
    /// </summary>
    internal static class HtmlConstants
    {
        /// <summary>
        /// Aspose Words vendor-specific extensions for CSS properties.
        /// </summary>
        internal const string AsposeVendorCssPrefix = "-aw-";

        internal const string IgnoreOnRoundtrip = AsposeVendorCssPrefix + "ignore";
        internal const string IgnoreOnRoundtripTrue = "true";

        // Prefixes used to generate names for comment and its cross-references.
        internal const string CommentReferenceIdPrefix = "_cmntref";
        internal const string CommentHyperlinkHrefPrefix = "#_cmnt";
        internal const string CommentIdPrefix = "_cmnt";
        internal const string CommentReferenceHyperlinkHrefPrefix = "#_cmntref";

        internal const string CommentAuthor = AsposeVendorCssPrefix + "comment-author";
        internal const string CommentDateTime = AsposeVendorCssPrefix + "comment-datetime";
        internal const string CommentInitial = AsposeVendorCssPrefix + "comment-initial";
        internal const string CommentParentId = AsposeVendorCssPrefix + "comment-parent-id";
        internal const string CommentDone = AsposeVendorCssPrefix + "comment-done";
        internal const string CommentDoneYes = "yes";

        // In case when comment is overlapped with other comment or comment’s boundaries are spread over
        // multiple paragraphs additional span element is used to preserve comment’s boundaries
        // (HTML doesn’t allow anchor tags to overlap each other or spread over multiple paragraphs).
        // These css properties can be applied to span element and store names of comments which they start or end.
        internal const string CommentStart = AsposeVendorCssPrefix + "comment-start";
        internal const string CommentEnd = AsposeVendorCssPrefix + "comment-end";

        // Prefixes used to generate names for footnote and its cross-references.
        internal const string FootnoteReferenceIdPrefix = "_ftnref";
        internal const string FootnoteHyperlinkHrefPrefix = "#_ftn";
        internal const string FootnoteIdPrefix = "_ftn";
        internal const string FootnoteReferenceHyperlinkHrefPrefix = "#_ftnref";

        // Prefixes used to generate names for endnote and its cross-references.
        internal const string EndnoteReferenceIdPrefix = "_ednref";
        internal const string EndnoteHyperlinkHrefPrefix = "#_edn";
        internal const string EndnoteIdPrefix = "_edn";
        internal const string EndnoteReferenceHyperlinkHrefPrefix = "#_ednref";

        // All footnotes (endnotes) in a document share formatting. When exported to HTML footnotes
        // (endnotes) are separated from other document’s content with horizontal rule element.
        // These css properties can be applied to horizontal rule element and
        // store footnotes' (endnotes') formatting roundtrip information.
        internal const string FootnoteType = AsposeVendorCssPrefix + "footnote-type";
        internal const string FootnoteNumberStyle = AsposeVendorCssPrefix + "footnote-numberstyle";
        internal const string FootnoteStartNumber = AsposeVendorCssPrefix + "footnote-startnumber";

        internal const string FootnoteIsAuto = AsposeVendorCssPrefix + "footnote-isauto";

        internal const string HeaderFooterType = AsposeVendorCssPrefix + "headerfooter-type";
        internal const string HeaderFooterTypeHeaderPrimary = "header-primary";
        internal const string HeaderFooterTypeHeaderFirst = "header-first";
        internal const string HeaderFooterTypeFooterPrimary = "footer-primary";
        internal const string HeaderFooterTypeFooterFirst = "footer-first";
        internal const string HeaderFooterTypeLinked = "linked";

        // In case when bookmark is overlapped with other bookmark or bookmark’s boundaries are spread over
        // multiple paragraphs additional span element is used to preserve bookmark’s boundaries
        // (HTML doesn’t allow anchor tags to overlap each other or spread over multiple paragraphs).
        // These css properties can be applied to span element and store names of bookmarks which they start or end.
        internal const string BookmarkStart = AsposeVendorCssPrefix + "bookmark-start";
        internal const string BookmarkEnd = AsposeVendorCssPrefix + "bookmark-end";

        internal const string HeaderFooterDifferentFirstPage = AsposeVendorCssPrefix + "different-first-page";
        internal const string HeaderFooterDifferentFirstPageTrue = "true";

        internal const string TabStopAlignment = AsposeVendorCssPrefix + "tabstop-align";
        internal const string TabStopPosition = AsposeVendorCssPrefix + "tabstop-pos";
        internal const string TabStopLeader = AsposeVendorCssPrefix + "tabstop-leader";
        internal const string TabStopReplacement = "\xa0\xa0\xa0\xa0\xa0\xa0\xa0\xa0\xa0\xa0\xa0\xa0\xa0\x20";

        internal const string FieldCode = AsposeVendorCssPrefix + "field-code";
        internal const string FieldStart = AsposeVendorCssPrefix + "field-start";
        internal const string FieldStartTrue = "true";
        internal const string FieldSeparator = AsposeVendorCssPrefix + "field-separator";
        internal const string FieldSeparatorTrue = "true";
        internal const string FieldEnd = AsposeVendorCssPrefix + "field-end";
        internal const string FieldEndTrue = "true";

        internal const string ImageWrapType = AsposeVendorCssPrefix + "wrap-type";
        internal const string ImageLeftPos = AsposeVendorCssPrefix + "left-pos";
        internal const string ImageTopPos = AsposeVendorCssPrefix + "top-pos";
        internal const string ImageRelativeHPos = AsposeVendorCssPrefix + "rel-hpos";
        internal const string ImageRelativeVPos = AsposeVendorCssPrefix + "rel-vpos";

        internal const string RevisionAuthor = AsposeVendorCssPrefix + "revision-author";
        internal const string RevisionDateTime = AsposeVendorCssPrefix + "revision-datetime";

        // Every document element which is content control can be marked in
        // output HTML with these css properties to store some roundtrip information.
        internal const string SdtTitle = AsposeVendorCssPrefix + "sdt-title";
        internal const string SdtTag = AsposeVendorCssPrefix + "sdt-tag";
        internal const string SdtContent = AsposeVendorCssPrefix + "sdt-content";

        internal const string SdtContentPlaceholder = "placeholder";

        internal const string OriginalFontFamily = AsposeVendorCssPrefix + "font-family";
        internal const string ListLabelFontWeight = AsposeVendorCssPrefix + "font-weight";
        internal const string ListLabelNumberFormat = AsposeVendorCssPrefix + "number-format";

        internal const string ListLevelNumber = AsposeVendorCssPrefix + "list-level-number";
        internal const string ListNumberFormat = AsposeVendorCssPrefix + "list-number-format";
        internal const string ListNumberStyles = AsposeVendorCssPrefix + "list-number-styles";
        internal const string ListNumberValues = AsposeVendorCssPrefix + "list-number-values";
        internal const string ListPaddingSimulation = AsposeVendorCssPrefix + "list-padding-sml";

        internal const string Import = AsposeVendorCssPrefix + "import";

        internal const string HeightRule = AsposeVendorCssPrefix + "height-rule";
        internal const string HeightRuleExactly = "exactly";

        internal const string StyleName = AsposeVendorCssPrefix + "style-name";
        internal const string StyleAliases = AsposeVendorCssPrefix + "style-aliases";
        internal const string StyleParent = AsposeVendorCssPrefix + "style-parent";

        internal const string OutlineLevel = AsposeVendorCssPrefix + "outline-level";

        // Obsolete data attributes which were used in previous versions of AW to store roundtrip information.
        // They have same meaning and application as their css counterparts.
        internal const string CommentAuthorDataAttribute = "data-comment-author";
        internal const string CommentDateTimeDataAttribute = "data-comment-datetime";
        internal const string CommentInitialDataAttribute = "data-comment-initial";

        internal const string FootnoteTypeAttribute = "data-footnote-type";
        internal const string FootnoteNumberStyleAttribute = "data-footnote-numberstyle";
        internal const string FootnoteStartNumberAttribute = "data-footnote-startnumber";

        internal const string FootnoteIsAutoAttribute = "data-footnote-isauto";
        internal const string HeaderFooterTypeAttribute = "data-headerfooter-type";

        /// <summary>
        /// The minimum length between list label and beginning of item text in html (in Word can be zero).
        /// It varies from browser to browser. In IE and Firefox browsers it's equal to 5pt and isn't depended from
        /// a font size but it depends on the font size in Chrome.
        /// </summary>
        internal const double ListLabelToTextWidth = 5;

        /// <summary>
        /// When HtmlSaveOptions.PrettyFormat is on,
        /// in output HTML pseudo-element list item has additional space between list label and text.
        /// Currently it's a constant. And it's equal to <see cref="ListLabelToTextWidth"/>,
        /// because previously <see cref="ListLabelToTextWidth"/> was used (by mistake).
        /// So leave this as is for now.
        /// </summary>
        internal const double FirstSpaceWidth = ListLabelToTextWidth;

        /// <summary>
        /// XML numeric character reference for U+00A0 (NO-BREAK SPACE).
        /// </summary>
        /// <remarks>
        /// We cannot use the '&amp;nbsp;' reference defined in HTML, because it is not valid in XML.
        /// This allows users to load our HTML into XmlDocument even when DTD validation is off. See WORDSNET-1011 
        /// </remarks>
        internal const string NoBreakSpaceHtmlNumericRef = "&#xa0;";

        // Border widths from the document model may be changed during export to HTML.
        // The following CSS properties are used to preserve original border widths during roundtrip.
        internal const string AsposeBorderWidth = AsposeVendorCssPrefix + "border-width";
        internal const string AsposeBorderTopWidth = AsposeVendorCssPrefix + "border-top-width";
        internal const string AsposeBorderRightWidth = AsposeVendorCssPrefix + "border-right-width";
        internal const string AsposeBorderBottomWidth = AsposeVendorCssPrefix + "border-bottom-width";
        internal const string AsposeBorderLeftWidth = AsposeVendorCssPrefix + "border-left-width";

        // Not all border line styles from the document model can be presented by CSS line styles.
        // The following CSS properties are used to preserve original border line styles during roundtrip.
        internal const string AsposeBorderStyle = AsposeVendorCssPrefix + "border-style";
        internal const string AsposeBorderTopStyle = AsposeVendorCssPrefix + "border-top-style";
        internal const string AsposeBorderRightStyle = AsposeVendorCssPrefix + "border-right-style";
        internal const string AsposeBorderBottomStyle = AsposeVendorCssPrefix + "border-bottom-style";
        internal const string AsposeBorderLeftStyle = AsposeVendorCssPrefix + "border-left-style";

        internal const string AsposeBorderColor = AsposeVendorCssPrefix + "border-color";
        internal const string AsposeBorderTopColor = AsposeVendorCssPrefix + "border-top-color";
        internal const string AsposeBorderRightColor = AsposeVendorCssPrefix + "border-right-color";
        internal const string AsposeBorderBottomColor = AsposeVendorCssPrefix + "border-bottom-color";
        internal const string AsposeBorderLeftColor = AsposeVendorCssPrefix + "border-left-color";

        // Shorthand versions of the properties above.
        internal const string AsposeBorderTop = AsposeVendorCssPrefix + "border-top";
        internal const string AsposeBorderRight = AsposeVendorCssPrefix + "border-right";
        internal const string AsposeBorderBottom = AsposeVendorCssPrefix + "border-bottom";
        internal const string AsposeBorderLeft = AsposeVendorCssPrefix + "border-left";

        internal const string AsposeBorder = AsposeVendorCssPrefix + "border";

        internal const string AsposePaddingTop = AsposeVendorCssPrefix + "padding-top";
        internal const string AsposePaddingRight = AsposeVendorCssPrefix + "padding-right";
        internal const string AsposePaddingBottom = AsposeVendorCssPrefix + "padding-bottom";
        internal const string AsposePaddingLeft = AsposeVendorCssPrefix + "padding-left";

        internal const string AsposeInsideH = AsposeVendorCssPrefix + "border-insideh";
        internal const string AsposeInsideV = AsposeVendorCssPrefix + "border-insidev";

        internal const string AsposeBorderInsideHWidth = AsposeVendorCssPrefix + "border-insideh-width";
        internal const string AsposeBorderInsideVWidth = AsposeVendorCssPrefix + "border-insidev-width";
        internal const string AsposeBorderInsideHStyle = AsposeVendorCssPrefix + "border-insideh-style";
        internal const string AsposeBorderInsideVStyle = AsposeVendorCssPrefix + "border-insidev-style";
        internal const string AsposeBorderInsideHColor = AsposeVendorCssPrefix + "border-insideh-color";
        internal const string AsposeBorderInsideVColor = AsposeVendorCssPrefix + "border-insidev-color";

        // Custom CSS properties that we use to preserve frame properties during HTML round trip.
        internal const string FrameWrapType = AsposeVendorCssPrefix + "element-wrap";
        internal const string FrameRelativeVerticalPosition = AsposeVendorCssPrefix + "element-anchor-vertical";
        internal const string FrameRelativeHorizontalPosition = AsposeVendorCssPrefix + "element-anchor-horizontal";
        internal const string FrameHeightRule = AsposeVendorCssPrefix + "height-rule";
        internal const string FrameWidth = AsposeVendorCssPrefix + "frame-width";
        internal const string FrameHeight = AsposeVendorCssPrefix + "frame-height";
        internal const string FrameTop = AsposeVendorCssPrefix + "element-top";
        internal const string FrameLeft = AsposeVendorCssPrefix + "element-left";
        internal const string FrameHorizontalDistanceFromText = AsposeVendorCssPrefix + "element-frame-hspace";
        internal const string FrameVerticalDistanceFromText = AsposeVendorCssPrefix + "element-frame-vspace";
        internal const string FrameLockAnchor = AsposeVendorCssPrefix + "element-anchor-lock";

        internal const string FrameLockAnchorLockedValue = "locked";

        // Custom CSS properties that we use to preserve page numbering properties.
        internal const string PageSetupNumberStyle = AsposeVendorCssPrefix + "page-numbers-style";
        internal const string PageSetupChapterSeparator = AsposeVendorCssPrefix + "page-numbers-chapter-separator";
        internal const string PageSetupHeadingLevelForChapter = AsposeVendorCssPrefix + "page-numbers-chapter-level";
        internal const string PageSetupStartingNumber = AsposeVendorCssPrefix + "page-numbers-start";

        // Custom CSS properties that we use to preserve page header and footer distances.
        internal const string PageSetupHeaderDistance = AsposeVendorCssPrefix + "header-distance";
        internal const string PageSetupFooterDistance = AsposeVendorCssPrefix + "footer-distance";
    }
}
