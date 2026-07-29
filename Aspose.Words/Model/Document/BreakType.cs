// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Specifies type of a break inside a document.
    /// </summary>
    public enum BreakType
    {
        /// <summary>
        /// Break between paragraphs.
        /// </summary>
        ParagraphBreak,
        /// <summary>
        /// Explicit page break.
        /// </summary>
        PageBreak,
        /// <summary>
        /// Explicit column break.
        /// </summary>
        ColumnBreak,
        /// <summary>
        /// Specifies start of new section on the same page as the previous section.
        /// </summary>
        SectionBreakContinuous,
        /// <summary>
        /// Specifies start of new section in the new column.
        /// </summary>
        SectionBreakNewColumn,
        /// <summary>
        /// Specifies start of new section on a new page.
        /// </summary>
        SectionBreakNewPage,
        /// <summary>
        /// Specifies start of new section on a new even page.
        /// </summary>
        SectionBreakEvenPage,
        /// <summary>
        /// Specifies start of new section on a odd page.
        /// </summary>
        SectionBreakOddPage,
        /// <summary>
        /// Explicit line break.
        /// </summary>
        LineBreak,
    }
}
