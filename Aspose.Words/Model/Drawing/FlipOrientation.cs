// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/02/2006 by Roman Korchagin

using System;
using System.Diagnostics.CodeAnalysis;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Possible values for the orientation of a shape.
    /// </summary>
    /// <seealso cref="ShapeBase.FlipOrientation"/>
    [Flags]
    [SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames",
        Justification = "Public API.")]
    public enum FlipOrientation
    {
        /// <summary>
        /// Coordinates are not flipped.
        /// </summary>
        None = 0x00,
        /// <summary>
        /// Flip along the y-axis, reversing the x-coordinates.
        /// </summary>
        Horizontal = 0x01,
        /// <summary>
        /// Flip along the x-axis, reversing the y-coordinates.
        /// </summary>
        Vertical = 0x02,
        /// <summary>
        /// Flip along both the y- and x-axis.
        /// </summary>
        Both = 0x03
    }
}
