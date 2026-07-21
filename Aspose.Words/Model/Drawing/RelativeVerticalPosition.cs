// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/02/2006 by Roman Korchagin

using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Specifies to what the vertical position of a shape or text frame is relative.
    /// </summary>
    /// <seealso cref="ShapeBase.RelativeVerticalPosition"/>
    /// <dev>
    /// Do not renumber. The values are taken from the RTF specification.
    /// </dev>
    [CppEnumEnableMetadata]
    public enum RelativeVerticalPosition
    {
        /// <summary>
        /// Specifies that the vertical positioning shall be relative to the page margins.
        /// </summary>
        Margin = 0,
        /// <summary>
        /// The object is positioned relative to the top edge of the page.
        /// </summary>
        Page = 1,
        /// <summary>
        /// The object is positioned relative to the top of the paragraph that contains the anchor.
        /// </summary>
        Paragraph = 2,
        /// <summary>
        /// Undocumented.
        /// </summary>
        Line = 3,
        /// <summary>
        /// Specifies that the vertical positioning shall be relative to the top margin of the current page.
        /// </summary>
        TopMargin = 4,  // The value taken from a test RTF file.
        /// <summary>
        /// Specifies that the vertical positioning shall be relative to the bottom margin of the current page.
        /// </summary>
        BottomMargin = 5,   // The value taken from a test RTF file.
        /// <summary>
        /// Specifies that the vertical positioning shall be relative to the inside margin of the current page.
        /// </summary>
        InsideMargin = 6,   // The value taken from a test RTF file.
        /// <summary>
        /// Specifies that the vertical positioning shall be relative to the outside margin of the current page.
        /// </summary>
        OutsideMargin = 7,  // The value taken from a test RTF file.
        /// <summary>
        /// Default value is <see cref="Margin"/>. 
        /// </summary>
        TableDefault = Margin,          // AM. Better naming for this value should be Default and it's applied to both table and paragraph.
                                        // We decided to don't change this for a while because it's public and we're not 100% sure yet.
        /// <summary>
        /// Default value is <see cref="Paragraph"/>. 
        /// </summary>
        TextFrameDefault = Paragraph    // AM. Better naming for this value should be ShapeDefault and it is drawing objects default.
                                        // We decided to don't change this for a while because it's public and we're not 100% sure yet.
    }
}
