// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Specifies orientation of text in shapes.
    /// </summary>
    /// <dev>
    /// Type of vertical text (20.1.10.82 ST_TextVerticalType (Vertical Text Types)).
    /// Enum elements have the same names as their corresponding <see cref="TextOrientation"/> elements.
    /// </dev>
    public enum ShapeTextOrientation
    {
        /// <summary>
        /// Text is arranged horizontally (lr-tb).
        /// </summary>
        Horizontal,

        /// <summary>
        /// Text is rotated 90 degrees to the right to appear from top to bottom (tb-rl).
        /// </summary>
        Downward,

        /// <summary>
        /// Text is rotated 90 degrees to the left to appear from bottom to top (bt-lr).
        /// </summary>
        Upward,

        /// <summary>
        /// Far East characters appear vertical, other text is rotated 90 degrees
        /// to the right to appear from top to bottom (tb-rl-v).
        /// </summary>
        VerticalFarEast,

        /// <summary>
        /// Far East characters appear vertical, other text is rotated 90 degrees
        /// to the right to appear from top to bottom vertically, then left to right horizontally  (tb-lr-v).
        /// </summary>
        VerticalRotatedFarEast,

        /// <summary>
        /// Text is vertical, with one letter on top of the other.
        /// </summary>
        WordArtVertical,

        /// <summary>
        /// Text is vertical, with one letter on top of the other, then right to left horizontally.
        /// </summary>
        WordArtVerticalRightToLeft
    }
}
