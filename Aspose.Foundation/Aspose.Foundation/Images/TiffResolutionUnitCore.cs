// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/10/2009 by Alexey Noskov

namespace Aspose.Images
{
    /// <summary>
    /// Specifies what unit of measurement is used for XResolution and YResolution.
    /// </summary>
    public enum TiffResolutionUnitCore
    {
        /// <summary>
        /// No absolute unit of measurement. Used for images that may have a non-square aspect ratio, but no meaningful absolute dimensions.
        /// </summary>
        None = 1,
        /// <summary>
        /// Inch.
        /// </summary>
        Inch = 2,
        /// <summary>
        /// Centimeter.
        /// </summary>
        Centimeter = 3,
    }
}