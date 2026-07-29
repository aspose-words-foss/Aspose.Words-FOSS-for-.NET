// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/10/2009 by Alexey Noskov

namespace Aspose.Images
{
    /// <summary>
    /// Indicates the color space of the image.
    /// </summary>
    public enum TiffPhotometricInterpretationCore
    {
        /// <summary>
        /// For bilevel and grayscale images: 0 is imaged as white.
        /// </summary>
        WhiteIsZero = 0,
        /// <summary>
        /// For bilevel and grayscale images: 0 is imaged as black.
        /// </summary>
        BlackIsZero = 1,
        /// <summary>
        /// Defines a Red-Green-Blue color map for RGB images.
        /// </summary>
        Rgb = 2,
        /// <summary>
        /// Defines a Red-Green-Blue color map for palette color images.
        /// </summary>
        RgbPalette = 3,
        /// <summary>
        /// Transparency Mask.
        /// </summary>
        TransparencyMask = 4,
        /// <summary>
        /// Separated - usually CMYK.
        /// </summary>
        Cmyk = 5,
        /// <summary>
        /// Indicates that the image data is in the YCbCr color space.
        /// </summary>
        Ycbcr = 6,
        /// <summary>
        /// Indicates that the image data is in the Cielab color space.
        /// </summary>
        Cielab = 8,
        /// <summary>
        /// This value cannot be use in TIFF, it is used to specify default value while reading. 
        /// This is used to indicate if this field is specified.
        /// </summary>
        Unspecified = short.MaxValue
    }
}