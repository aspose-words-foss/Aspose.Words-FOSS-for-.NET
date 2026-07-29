// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/12/2022 by Vadim Saltykov

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Specifies relatively to what the height of a shape or a text frame is calculated vertically.
    /// </summary>
    /// <seealso cref="ShapeBase.RelativeVerticalSize"/>
    /// <dev>
    /// Do not renumber. The values are taken from the RTF specification.
    /// </dev>
    public enum RelativeVerticalSize
    {
        /// <summary>
        /// Specifies that the height is calculated relatively to the space between the top and the bottom margins.
        /// </summary>
        Margin = 0,
        /// <summary>
        /// Specifies that the height is calculated relatively to the page height.
        /// </summary>
        Page = 1,
        /// <summary>
        /// Specifies that the height is calculated relatively to the top margin area size.
        /// </summary>
        TopMargin = 2,
        /// <summary>
        /// Specifies that the height is calculated relatively to the bottom margin area size.
        /// </summary>
        BottomMargin = 3,
        /// <summary>
        /// Specifies that the height is calculated relatively to the inside margin area size,
        /// to the top margin area size for odd pages and to the bottom margin area size for even pages.
        /// </summary>
        InnerMargin = 4,
        /// <summary>
        /// Specifies that the height is calculated relatively to the outside margin area size,
        /// to the bottom margin area size for odd pages and to the top margin area size for even pages.
        /// </summary>
        OuterMargin = 5,
        /// <summary>
        /// Default value is <see cref="Margin"/>.
        /// </summary>
        Default = Page
    }
}
