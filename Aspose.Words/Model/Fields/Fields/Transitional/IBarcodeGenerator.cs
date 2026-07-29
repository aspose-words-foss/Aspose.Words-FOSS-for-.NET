// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/03/2015 by Vadim Polienko

using System.IO;
using Aspose.JavaAttributes;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Public interface for barcode custom generator. Implementation should be provided by user.
    /// </summary>
    /// <remarks>
    /// Generator instance should be passed through the <see cref="FieldOptions.BarcodeGenerator"/> property.
    /// </remarks>
    [JavaManual]
    public interface IBarcodeGenerator
    {
        /// <summary>
        /// Generate barcode image using the set of parameters (for DisplayBarcode field).
        /// </summary>
        /// <param name="parameters">The set of parameters</param>
        /// <returns>Stream with image data representing generated barcode.</returns>
        /// <remarks>Supported image formats are Bmp, Emf, Gif, Jpeg, Png, Tiff, Wmf, Pict, Ico, WebP, Svg.</remarks>
        [JavaThrows(true)]
        Stream GetBarcodeImage(BarcodeParameters parameters);

        /// <summary>
        /// Generate barcode image using the set of parameters (for old-fashioned Barcode field).
        /// </summary>
        /// <param name="parameters">The set of parameters</param>
        /// <returns>Stream with image data representing generated barcode.</returns>
        /// <remarks>Supported image formats are Bmp, Emf, Gif, Jpeg, Png, Tiff, Wmf, Pict, Ico, WebP, Svg.</remarks>
        [JavaThrows(true)]
        Stream GetOldBarcodeImage(BarcodeParameters parameters);
    }
}
