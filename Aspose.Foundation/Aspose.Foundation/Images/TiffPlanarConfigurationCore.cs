// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/10/2009 by Alexey Noskov

namespace Aspose.Images
{
    /// <summary>
    /// Specifies how the components of each pixel are stored.
    /// </summary>
    public enum TiffPlanarConfigurationCore
    {
        /// <summary>
        /// Chunky format. The component values for each pixel are stored contiguously.
        /// The order of the components within the pixel is specified by PhotometricInterpretation.
        /// </summary>
        Chunky = 1,
        /// <summary>
        /// Planar format. The components are stored in separate "component planes."
        /// </summary>
        Planar = 2
    }
}