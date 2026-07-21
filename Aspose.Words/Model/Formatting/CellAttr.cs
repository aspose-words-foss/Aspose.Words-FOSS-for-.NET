// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2005 by Roman Korchagin

using Aspose.Words.Tables;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Attributes that can be defined for a cell.
    ///
    /// Note the constant values make sure the attributes are written
    /// into a binary file in a specific order and the order is important.
    /// </summary>
    [CppConstexpr]
    internal static class CellAttr
    {
        /// <summary>
        /// int, twips, no default
        /// </summary>
        internal const int Width = 3010;
        /// <summary>
        /// <see cref="Aspose.Words.Tables.PreferredWidth"/>
        /// </summary>
        internal const int PreferredWidth = 3020;
        /// <summary>
        /// enum
        /// </summary>
        internal const int VerticalMerge = 3030;
        /// <summary>
        /// enum
        /// </summary>
        internal const int HorizontalMerge = 3040;
        /// <summary>
        /// enum, <see cref="TextOrientation"/>
        /// </summary>
        internal const int Orientation = 3050;
        /// <summary>
        /// enum
        /// </summary>
        internal const int VerticalAlignment = 3060;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int TopPadding = 3070;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int BottomPadding = 3080;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int LeftPadding = 3090;
        /// <summary>
        /// int, twips
        /// </summary>
        internal const int RightPadding = 3100;
        /// <summary>
        /// Border
        /// </summary>
        internal const int BorderTop = 3110;
        /// <summary>
        /// Border
        /// </summary>
        internal const int BorderLeft = 3120;
        /// <summary>
        /// Border
        /// </summary>
        internal const int BorderBottom = 3130;
        /// <summary>
        /// Border
        /// </summary>
        internal const int BorderRight = 3140;
        /// <summary>
        /// Border
        /// </summary>
        internal const int BorderDiagonalDown = 3150;
        /// <summary>
        /// Border
        /// </summary>
        internal const int BorderDiagonalUp = 3160;
        /// <summary>
        /// Shading
        /// </summary>
        internal const int Shading = 3170;
        /// <summary>
        /// bool
        /// </summary>
        internal const int WrapText = 3180;
        /// <summary>
        /// bool
        /// </summary>
        internal const int FitText = 3190;

        /// <summary>
        /// Border. Only used in conditional formatting in table styles.
        /// </summary>
        internal const int BorderHorizontal = 3200;

        /// <summary>
        /// Border. Only used in conditional formatting in table styles.
        /// </summary>
        internal const int BorderVertical = 3210;

        /// <summary>
        /// Bool. Specifies that table cell content is rendered with no height if all cells in the row are empty; 
        /// however, cells have a visible height if they have nonzero cell borders, cell margins, or cell spacing.
        /// </summary>
        internal const int HideMark = 3220;

        /// <summary>
        /// Specifies grid span for cell. 
        /// </summary>
        /// <remarks>
        /// The attribute is originally set by DOCX/WML import.
        /// Table grid calculation, <see cref="Table.AutoFit"/> or DOCX/WML export may modify the attribute.
        /// Modifying the attributes directly bypassing the above scenarios may change the table structure and should be avoided.
        /// </remarks>
        internal const int Sys_CellSpan = 3900;
    }
}
