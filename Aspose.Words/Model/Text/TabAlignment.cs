// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using System.Diagnostics.CodeAnalysis;

namespace Aspose.Words
{
    /// <summary>
    /// Specifies the alignment/type of a tab stop.
    /// </summary>
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags",
        Justification = "Not used as a flag.")]
    public enum TabAlignment
    {
        /// <summary>
        /// Left-aligns the text after the tab stop.
        /// </summary>
        Left = 0,
        /// <summary>
        /// Centers the text around the tab stop.
        /// </summary>
        Center = 1,
        /// <summary>
        /// Right-aligns the text at the tab stop.
        /// </summary>
        Right = 2,
        /// <summary>
        /// Aligns the text at the decimal dot.
        /// </summary>
        Decimal = 3,
        /// <summary>
        /// Draws a vertical bar at the tab stop position.
        /// </summary>
        Bar = 4,
        /// <summary>
        /// The tab is a delimiter between the number/bullet and text in a list item.
        /// </summary>
        List = 6,
        /// <summary>
        /// Clears any tab stop in this position.
        /// </summary>
        Clear = 7
    }
}
