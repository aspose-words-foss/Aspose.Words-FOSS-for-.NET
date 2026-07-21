// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/02/2006 by Roman Korchagin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Specifies to what the horizontal position of a shape or text frame is relative.
    /// </summary>
    /// <seealso cref="ShapeBase.RelativeHorizontalPosition"/>
    /// <dev>
    /// Do not renumber. The values are taken from the RTF specification.
    /// </dev>
    [CppEnumEnableMetadata]
    public enum RelativeHorizontalPosition
    {
        /// <summary>
        /// Specifies that the horizontal positioning shall be relative to the page margins.
        /// </summary>
        Margin = 0,
        /// <summary>
        /// The object is positioned relative to the left edge of the page.
        /// </summary>
        Page = 1,
        /// <summary>
        /// The object is positioned relative to the left side of the column.
        /// </summary>
        Column = 2,
        /// <summary>
        /// The object is positioned relative to the left side of the paragraph.
        /// </summary>
        Character = 3,
        /// <summary>
        /// Specifies that the horizontal positioning shall be relative to the left margin of the page.
        /// </summary>
        LeftMargin = 4, // The value taken from a test RTF file.
        /// <summary>
        /// Specifies that the horizontal positioning shall be relative to the right margin of the page.
        /// </summary>
        RightMargin = 5,    // The value taken from a test RTF file.
        /// <summary>
        /// Specifies that the horizontal positioning shall be relative to the inside margin of the 
        /// current page (the left margin on odd pages, right on even pages).
        /// </summary>
        InsideMargin = 6,   // The value taken from a test RTF file.
        /// <summary>
        /// Specifies that the horizontal positioning shall be relative to the outside margin of the 
        /// current page (the right margin on odd pages, left on even pages).
        /// </summary>
        OutsideMargin = 7,  // The value taken from a test RTF file.
        /// <summary>
        /// Default value is <see cref="Column"/>.
        /// </summary>
        Default = Column,
    }
}
