// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Specifies the type of an arrow at a line end.
    /// </summary>
    /// <seealso cref="Stroke.StartArrowType"/>
    /// <seealso cref="Stroke.EndArrowType"/>
    public enum ArrowType
    {
        /// <summary>
        /// The line does not have an arrow at the end.
        /// </summary>
        None = 0,
        /// <summary>
        /// The arrow is a solid triangle.
        /// </summary>
        Arrow = 1,
        /// <summary>
        /// The arrow is a "stealth" arrow.
        /// </summary>
        Stealth = 2,
        /// <summary>
        /// The line end is a solid diamond.
        /// </summary>
        Diamond = 3,
        /// <summary>
        /// The line end is a solid oval.
        /// </summary>
        Oval = 4,
        /// <summary>
        /// The arrow is an open arrow.
        /// </summary>
        Open = 5,

        /// <summary>
        /// Same as <see cref="None"/>.
        /// </summary>
        Default = None
    }
}
