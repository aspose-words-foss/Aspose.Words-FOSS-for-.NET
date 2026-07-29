// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/06/2012 by Alexey Noskov

namespace Aspose.Words.Drawing.Charts.Core.SimpleTypes
{
    /// <summary>
    /// This enum specifies the possible ways to place a picture on a data point, series, wall, or floor.
    /// Corresponds ST_PictureFormat simple type (5.7.3.35).
    /// </summary>
    internal enum PictureFormat
    {
        /// <summary>
        /// Specifies that the picture shall be stacked.
        /// </summary>
        Stack,
        /// <summary>
        /// Specifies that the picture shall be stacked after being scaled so that it's height is one Picture Stack Unit. 
        /// Does not apply to walls or floor.
        /// </summary>
        StackScale,
        /// <summary>
        /// Specifies that the picture shall be anisotropic stretched to fill the data point, series, wall or floor.
        /// </summary>
        Stretch
    }
}
