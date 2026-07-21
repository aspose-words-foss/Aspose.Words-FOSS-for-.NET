// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/02/2006 by Roman Korchagin

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Specifies what side(s) of the shape or picture the text wraps around.
    /// </summary>
    /// <seealso cref="ShapeBase.WrapSide"/>
    public enum WrapSide
    {
        /// <summary>
        /// The document text wraps on both sides of the shape.
        /// </summary>
        Both = 0,
        /// <summary>
        /// The document text wraps on the left side of the shape only. There is a text free area on the right of the shape.
        /// </summary>
        Left = 1,
        /// <summary>
        /// The document text wraps on the right side of the shape only. There is a text free area on the left side of the shape.
        /// </summary>
        Right = 2,
        /// <summary>
        /// The document text wraps on the side of the shape that is farthest from the page margin, leaving text free area on the other side of the shape.
        /// </summary>
        Largest = 3,
        /// <summary>
        /// Default value is <see cref="Both"/>.
        /// </summary>
        Default = Both
    }
}
