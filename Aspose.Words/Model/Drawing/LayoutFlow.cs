// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/07/2006 by Roman Korchagin

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Determines the flow of the text layout in a textbox. 
    /// </summary>
    /// <seealso cref="TextBox.LayoutFlow"/>
    /// <dev>
    /// Do not renumber. The values are from the RTF spec.
    /// </dev>
    public enum LayoutFlow
    {
        /// <summary>
        /// Text is displayed horizontally.
        /// </summary>
        Horizontal = 0,
        /// <summary>
        /// Ideographic text is displayed vertically.
        /// </summary>
        TopToBottomIdeographic = 1,
        /// <summary>
        /// Text is displayed vertically.
        /// </summary>
        BottomToTop = 2,
        /// <summary>
        /// Text is displayed vertically.
        /// </summary>
        TopToBottom = 3,
        /// <summary>
        /// Ideographic text is displayed horizontally.
        /// </summary>
        HorizontalIdeographic = 4,
        /// <summary>
        /// Text is displayed vertically.
        /// </summary>
        Vertical = 5
    }
}
