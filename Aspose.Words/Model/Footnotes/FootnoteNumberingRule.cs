// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/07/2007 by Roman Korchagin

namespace Aspose.Words.Notes
{
    /// <summary>
    /// Determines when automatic footnote or endnote numbering restarts.
    /// </summary>
    /// <seealso cref="FootnoteOptions"/>
    /// <seealso cref="EndnoteOptions"/>
    public enum FootnoteNumberingRule
    {
        /// <summary>
        /// Numbering continuous throughout the document.
        /// </summary>
        Continuous = 0,
        /// <summary>
        /// Numbering restarts at each section.
        /// </summary>
        RestartSection = 1,
        /// <summary>
        /// Numbering restarts at each page. Valid for footnotes only.
        /// </summary>
        RestartPage = 2,

        /// <summary>
        /// Equals <see cref="Continuous"/>.
        /// </summary>
        Default = Continuous
    }
}
