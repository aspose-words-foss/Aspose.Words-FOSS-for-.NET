// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/02/2006 by Roman Korchagin

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Specifies how text is wrapped around a shape or picture.
    /// </summary>
    /// <seealso cref="ShapeBase.WrapType"/>
    public enum WrapType
    {
        /// <summary>
        /// No text wrapping around the shape. The shape is placed behind or in front of text.
        /// </summary>
        None = 3,
        /// <summary>
        /// The shape remains on the same layer as text and treated as a character.
        /// </summary>
        Inline = 0,
        /// <summary>
        /// The text stops at the top of the shape and restarts on the line below the shape.
        /// </summary>
        TopBottom = 1,
        /// <summary>
        /// Wraps text around all sides of the square bounding box of the shape.
        /// </summary>
        Square = 2,
        /// <summary>
        /// Wraps tightly around the edges of the shape, instead of wrapping around the bounding box.
        /// </summary>
        Tight = 4,
        /// <summary>
        /// Same as Tight, but wraps inside any parts of the shape that are open.
        /// </summary>
        Through = 5
    }
}
