// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/05/2006 by Dmitry Vorobyev

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies what type of compression to apply when saving page images into a TIFF file.
    /// </summary>
    public enum TiffCompression
    {
        /// <summary>
        /// Specifies no compression.
        /// </summary>
        None = 0,
        /// <summary>
        /// Specifies the RLE compression scheme.
        /// </summary>
        Rle = 1,
        /// <summary>
        /// Specifies the LZW compression scheme.
        /// In Java emulated by Deflate (Zip) compression.
        /// </summary>
        Lzw = 2,
        /// <summary>
        /// Specifies the CCITT3 compression scheme.
        /// </summary>
        Ccitt3 = 3,
        /// <summary>
        /// Specifies the CCITT4 compression scheme.
        /// </summary>
        Ccitt4 = 4
    }
}

