// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/07/2006 by Roman Korchagin

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// Defines the type of fill.
    /// </summary>
    /// <seealso cref="Fill.FillType"/>
    [CodePorting.Translator.Cs2Cpp.CppEnumEnableMetadata]
    internal enum FillTypeCore
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
        /// Fill with a picture that is centered in the shape.
        /// </summary>
        Picture = 3,
        /// <summary>
        /// Shade from start to end points.
        /// </summary>
        Shade = 4,
        /// <summary>
        /// Shade from bounding rectangle to end point.
        /// </summary>
        ShadeCenter = 5,
        /// <summary>
        /// Shade from shape outline to end point.
        /// </summary>
        ShadeShape = 6,
        /// <summary>
        /// Similar to Shade, but the fillAngle is additionally scaled by the aspect ratio of
        /// the shape. If shape is square, it is the same as Shade.
        /// </summary>
        ShadeScale = 7,
        /// <summary>
        /// Special type - shade to title ---  for PP.
        /// </summary>
        ShadeTitle = 8,
        /// <summary>
        /// Use the background fill color/pattern.
        /// </summary>
        Background = 9,
        /// <summary>
        /// Similar to Shade, but the fillAngle is additionally NOT scaled by the aspect ratio of
        /// the shape. If shape is square, it is the same as Shade.
        /// </summary>
        ShadeUnscale = 10
    }
}
