// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Specifies orientation of text on a page, in a table cell or a text frame.
    /// </summary>
    [CppEnumEnableMetadata]
    public enum TextOrientation
    {
        /// <summary>
        /// Text is arranged horizontally (lr-tb).
        /// </summary>
        Horizontal = 0,

        /// <summary>
        /// Text is rotated 90 degrees to the right to appear from top to bottom (tb-rl).
        /// </summary>
        Downward = 1,

        /// <summary>
        /// Text is rotated 90 degrees to the left to appear from bottom to top (bt-lr).
        /// </summary>
        Upward = 3,

        /// <summary>
        /// Text is arranged horizontally, but Far East characters are rotated 90 degrees to the left (lr-tb-v).
        /// </summary>
        HorizontalRotatedFarEast = 4,

        /// <summary>
        /// Far East characters appear vertical, other text is rotated 90 degrees
        /// to the right to appear from top to bottom (tb-rl-v).
        /// </summary>
        VerticalFarEast = 5,

        /// <summary>
        /// Far East characters appear vertical, other text is rotated 90 degrees
        /// to the right to appear from top to bottom vertically, then left to right horizontally  (tb-lr-v).
        /// </summary>
        /// <dev> IN: Set this value to 7 because MS Word uses this value in DOC format. </dev>
        VerticalRotatedFarEast = 7
    }
}
