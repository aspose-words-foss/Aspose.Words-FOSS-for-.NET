// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 31/07/2010 by Viktor Sazhaev

using System;
using System.Diagnostics.CodeAnalysis;

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies how the document is split into parts when saving to <see cref="SaveFormat.Html"/>,
    /// <see cref="SaveFormat.Epub"/> or <see cref="SaveFormat.Azw3"/> format.
    /// </summary>
    /// <remarks>
    /// <p><see cref="DocumentSplitCriteria" /> is a set of flags which can be combined. For instance you can split the document
    /// at page breaks and heading paragraphs in the same export operation.</p>
    /// 
    /// <p>Different criteria can partially overlap. For instance, <b>Heading 1</b> style is frequently given 
    /// <see cref="ParagraphFormat.PageBreakBefore" /> property so it falls under two criteria: <see cref="PageBreak" /> and 
    /// <see cref="HeadingParagraph" />. Some section breaks can cause page breaks and so on. 
    /// In typical cases specifying only one flag is the most practical option.</p>
    /// 
    /// <seealso cref="HtmlSaveOptions.DocumentSplitCriteria"/>
    /// </remarks>
    [Flags]
    [SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames",
        Justification = "Public API.")]
    public enum DocumentSplitCriteria
    {
        /// <summary>
        /// The document is not split.
        /// </summary>
        None = 0x0000,
        /// <summary>
        /// The document is split into parts at explicit page breaks.
        /// A page break can be specified by a <see cref="ControlChar.PageBreak"/> character, 
        /// a section break specifying start of new section on a new page,
        /// or a paragraph that has its <see cref="ParagraphFormat.PageBreakBefore" /> property set to <c>true</c>.
        /// </summary>
        PageBreak = 0x0001,
        /// <summary>
        /// The document is split into parts at column breaks.
        /// A column break can be specified by a <see cref="ControlChar.ColumnBreak"/> character or
        /// a section break specifying start of new section in a new column.
        /// </summary>
        ColumnBreak = 0x0002,
        /// <summary>
        /// The document is split into parts at a section break of any type.
        /// </summary>
        SectionBreak = 0x0004,
        /// <summary>
        /// The document is split into parts at a paragraph formatted using a heading style <b>Heading 1</b>, <b>Heading 2</b> etc. 
        /// Use together with <see cref="HtmlSaveOptions.DocumentSplitHeadingLevel"/> to specify the heading levels 
        /// (from 1 to the specified level) at which to split.
        /// </summary>
        HeadingParagraph = 0x0008
    }
}
