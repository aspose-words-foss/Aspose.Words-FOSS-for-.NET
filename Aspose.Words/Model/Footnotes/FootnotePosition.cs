// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/07/2007 by Roman Korchagin

using System.Diagnostics.CodeAnalysis;

namespace Aspose.Words.Notes
{
    /// <summary>
    /// Defines the footnote position.
    /// </summary>
    /// <seealso cref="FootnoteOptions"/>
    [SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue",
        Justification = "Public API, as designed.")]
    public enum FootnotePosition
    {
        /// <summary>
        /// Footnotes are output at the bottom of each page.
        /// </summary>
        BottomOfPage = 1,
        /// <summary>
        /// Footnotes are output beneath text on each page.
        /// </summary>
        BeneathText = 2
    }
}
