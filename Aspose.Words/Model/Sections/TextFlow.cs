// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/03/2007 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Specifies orientation of text in a section.
    /// This has similar values, but not same as <see cref="TextOrientation"/>.
    /// </summary>
    internal enum TextFlow
    {
        /// <summary>
        /// Text flows left to right and top to bottom (lr-tb).
        /// 
        /// Can be selected in MS Word.
        /// </summary>
        Horizontal = 0,
        /// <summary>
        /// Text flows top to bottom and right to left, vertical (tb-rl).
        /// 
        /// Far East characters appear vertical, other text is rotated 90 degrees to the right 
        /// to appear from top to bottom. 
        /// 
        /// Can be selected in MS Word.
        /// </summary>
        Vertical = 1,
        /// <summary>
        /// Cannot be selected in MS Word.
        /// MS says: Text flows left to right and bottom to top (lr-bt).
        /// </summary>
        LrBt = 2,
        /// <summary>
        /// Cannot be selected in MS Word.
        /// MS says: Text flows right to left and top to bottom (rl-tb).
        /// </summary>
        RlTb = 3,
        /// <summary>
        /// Text flows left to right and top to bottom, vertical (lr-tb-v).
        /// 
        /// Text is arranged horizontally, but Far East characters are rotated 90 degrees to the left.
        /// 
        /// Can be selected in MS Word.
        /// </summary>
        HorizontalRotatedFarEast = 4,
        /// <summary>
        /// Cannot be selected in MS Word.
        /// MS says: Text flows vertically, non-vertical font (tb-rl-v)?
        /// </summary>
        VNonV = 5,
        /// <summary>
        /// Text flows top to bottom and left to right, vertical (tb-lr-v).
        /// 
        /// Text is arranged vertically, but Far East characters are rotated 90 degrees to the left.
        /// 
        /// Cannot be selected in MS Word.
        /// </summary>
        VerticalRotatedFarEast = 7

    }
}
