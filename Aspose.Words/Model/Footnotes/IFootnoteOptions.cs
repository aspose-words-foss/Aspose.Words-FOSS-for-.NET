// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/08/2017 by Alexander Zhiltsov

namespace Aspose.Words.Notes
{
    /// <summary>
    /// Common interface for the <see cref="FootnoteOptions"/> and <see cref="EndnoteOptions"/> classes.
    /// </summary>
    internal interface IFootnoteOptions
    {
        /// <summary>
        /// Specifies the number format for automatically numbered footnotes or endnotes.
        /// </summary>
        NumberStyle NumberStyle { get; set; }

        /// <summary>
        /// Determines when automatic numbering restarts.
        /// </summary>
        FootnoteNumberingRule RestartRule { get; set; }

        /// <summary>
        /// Specifies the starting number or character for the first automatically numbered footnotes or endnotes.
        /// </summary>
        int StartNumber { get; set; }

        /// <summary>
        /// Specifies the footnotes or endnotes position.
        /// </summary>
        FootnoteLocation Location { get; set; }
    }
}
