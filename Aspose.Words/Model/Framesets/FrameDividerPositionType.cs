// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/09/2010 by Alexey Morozov
namespace Aspose.Words.Framesets
{
    /// <summary>
    /// Specifies the units of a frame divider position.
    /// </summary>
    /// <remarks>
    /// Values correspond to [MS-DOC] 2.9.99 FssdUnits.
    /// </remarks>
    internal enum FrameDividerPositionType
    {
        /// <summary>
        /// No units are specified.
        /// </summary>
        None = 0x00,
        
        /// <summary>
        /// The value is in pixels.
        /// </summary>
        Pixel = 0x01,
        
        /// <summary>
        /// The value is a percentage of the size of the parent frame.
        /// </summary>
        Percentage = 0x02,
        
        /// <summary>
        /// The value is a relative position.
        /// </summary>
        Relative = 0x03
    }
}
