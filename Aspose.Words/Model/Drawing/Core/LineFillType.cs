// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/05/2015 by Andrey Noskov

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Specifies how to "fill" the line contour.
    /// </summary>
    /// <seealso cref="Stroke.LineFillType"/>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    internal enum LineFillType
    {
        /// <summary>
        /// Fill with a solid color.
        /// </summary>
        Solid = 0,
        /// <summary>
        /// Fill with a pattern (bitmap).
        /// </summary>
        Pattern = 1,
        /// <summary>
        /// A texture (pattern with its own color map).
        /// </summary>
        Texture = 2,
        /// <summary>
        /// Center a picture in the shape.
        /// </summary>
        Picture = 3
    }
}
