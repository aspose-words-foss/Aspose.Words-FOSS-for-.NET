// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/06/2021 by Ilya Navrotskiy

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Specifies the style for a gradient fill.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    public enum GradientStyle
    {
        /// <summary>
        /// No gradient.
        /// </summary>
        None = -1,

        /// <summary>
        /// Gradient running horizontally across an object.
        /// </summary>
        Horizontal = 1,

        /// <summary>
        /// Gradient running vertically down an object.
        /// </summary>
        Vertical = 2,

        /// <summary>
        /// Diagonal gradient moving from a bottom corner up to the opposite corner.
        /// </summary>
        DiagonalUp = 3,

        /// <summary>
        /// Diagonal gradient moving from a top corner down to the opposite corner.
        /// </summary>
        DiagonalDown = 4,

        /// <summary>
        /// Gradient running from a corner to the other three corners.
        /// </summary>
        FromCorner = 5,

        /// <summary>
        /// Gradient running from the center out to the corners.
        /// </summary>
        FromCenter = 6
    }
}
