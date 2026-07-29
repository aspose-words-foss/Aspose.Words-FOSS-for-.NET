// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/07/2007 by Roman Korchagin

namespace Aspose.Words.Notes
{
    /// <summary>
    /// Defines the footnote or endnote position.
    /// </summary>
    internal enum FootnoteLocation
    {
        /// <summary>
        /// Footnotes are output at the bottom of each page. Valid for footnotes only.
        /// </summary>
        BottomOfPage = 1,
        /// <summary>
        /// Footnotes are output beneath text on each page. Valid for footnotes only.
        /// </summary>
        BeneathText = 2,
        /// <summary>
        /// Endnotes are output at the end of the section. Valid for endnotes only.
        /// </summary>
        EndOfSection = 0,
        /// <summary>
        /// Endnotes are output at the end of the document. Valid for endnotes only.
        /// </summary>
        EndOfDocument = 3,
    }
}
