// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/07/2010 by Alex Novickov

namespace Aspose.Words.Saving
{
    /// <summary>
    /// Specifies the pixel format for the generated images of document pages.
    /// </summary>
    public enum ImagePixelFormat
    {
        /// <summary>
        /// 16 bits per pixel, RGB.
        /// </summary>
        Format16BppRgb555 = 0,
        /// <summary>
        /// 16 bits per pixel, RGB.
        /// </summary>
        Format16BppRgb565 = 1,
        /// <summary>
        /// 16 bits per pixel, ARGB.
        /// </summary>
        Format16BppArgb1555 = 2,
        /// <summary>
        /// 24 bits per pixel, RGB.
        /// </summary>
        Format24BppRgb = 3,
        /// <summary>
        /// 32 bits per pixel, RGB.
        /// </summary>
        Format32BppRgb = 4,
        /// <summary>
        /// 32 bits per pixel, ARGB.
        /// </summary>
        Format32BppArgb = 5,
        /// <summary>
        /// 32 bits per pixel, ARGB, premultiplied alpha.
        /// </summary>
        Format32BppPArgb = 6,
        /// <summary>
        /// 48 bits per pixel, RGB.
        /// </summary>
        Format48BppRgb = 7,
        /// <summary>
        /// 64 bits per pixel, ARGB.
        /// </summary>
        Format64BppArgb = 8,
        /// <summary>
        /// 64 bits per pixel, ARGB, premultiplied alpha.
        /// </summary>
        Format64BppPArgb = 9,
        /// <summary>
        /// 1 bit per pixel, Indexed.
        /// </summary>
        Format1bppIndexed = 10
    }
}
