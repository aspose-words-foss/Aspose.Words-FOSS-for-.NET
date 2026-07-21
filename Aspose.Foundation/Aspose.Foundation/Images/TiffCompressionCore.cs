// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/05/2006 by Dmitry Vorobyev

namespace Aspose.Images
{
    /// <summary>
    /// Specifies what type of compression to apply when saving pages to the TIFF format.
    /// </summary>
    public enum TiffCompressionCore
    {
        /// <summary>
        /// Specifies no compression.
        /// </summary>
        None = 1,
        /// <summary>
        /// Specifies the RLE compression scheme.
        /// </summary>
        Rle = 2,
        /// <summary>
        /// Specifies the LZW compression scheme.
        /// In Java emulated by Deflate (Zip) compression.
        /// </summary>
        Lzw = 5,
        /// <summary>
        /// Specifies the CCITT3 compression scheme.
        /// </summary>
        Ccitt3 = 3,
        /// <summary>
        /// Specifies the CCITT4 compression scheme.
        /// </summary>
        Ccitt4 = 4,

        // AN: The following values are used only during validating TIFF images.

        /// <summary>
        /// Specifies the Jpeg compression scheme.
        /// </summary>
        Jpeg = 6,
        /// <summary>
        /// Specifies the PackBits compression scheme.
        /// </summary>
        PackBits = 32773
    }
}