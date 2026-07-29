// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/12/2022 by Vadim Saltykov

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Specifies relatively to what the width of a shape or a text frame is calculated horizontally.
    /// </summary>
    /// <seealso cref="ShapeBase.RelativeHorizontalSize"/>
    /// <dev>
    /// Do not renumber. The values are taken from the RTF specification.
    /// </dev>
    public enum RelativeHorizontalSize
    {
        /// <summary>
        /// Specifies that the width is calculated relatively to the space between the left and the right margins.
        /// </summary>
        Margin = 0,
        /// <summary>
        /// Specifies that the width is calculated relatively to the page width.
        /// </summary>
        Page = 1,
        /// <summary>
        /// Specifies that the width is calculated relatively to the left margin area size.
        /// </summary>
        LeftMargin = 2,
        /// <summary>
        /// Specifies that the width is calculated relatively to the right margin area size.
        /// </summary>
        RightMargin = 3,
        /// <summary>
        /// Specifies that the width is calculated relatively to the inside margin area size,
        /// to the left margin area size for odd pages and to the right margin area size for even pages.
        /// </summary>
        InnerMargin = 4,
        /// <summary>
        /// Specifies that the width is calculated relatively to the outside margin area size,
        /// to the right margin area size for odd pages and to the left margin area size for even pages.
        /// </summary>
        OuterMargin = 5,
        /// <summary>
        /// Default value is <see cref="Margin"/>.
        /// </summary>
        Default = Page
    }
}
