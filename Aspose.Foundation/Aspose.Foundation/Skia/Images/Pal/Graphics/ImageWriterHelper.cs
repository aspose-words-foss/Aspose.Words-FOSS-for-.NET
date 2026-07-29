// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/08/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD

using System.IO;
using Aspose.Images.Pal.Graphics.Encoder;
using Aspose.Images.Pal.Graphics.Encoder.Bmp;
using SkiaSharp;

namespace Aspose.Images.Pal.Graphics
{
    internal class ImageWriterHelper
    {
        public static void SaveGifCore(BitmapPal image, Stream stream)
        {
            GifEncoder encoder = new GifEncoder();
            encoder.Start(stream);
            encoder.AddFrame(image.GetNativeBitmap());
            encoder.Finish();
        }

        public static void SaveTiffCore(BitmapPal image, Stream stream, TiffCompressionCore compression, ImageBinarizationMethodCore binarizationMethod, byte threshold)
        {
            using (TiffWriterPal tiffWriter = new TiffWriterPal())
            {
                tiffWriter.SaveFirstFrame(stream, compression, binarizationMethod, threshold, System.Drawing.Imaging.PixelFormat.Format32bppArgb, image, false);
            }
        }

        public static void SaveBmpCore(BitmapPal image, Stream stream)
        {
            // Only default color type is supported.
            image.ConvertToDefaultColorType();

            BmpImageWriter writer = new BmpImageWriter();
            writer.SetOutput(stream);
            writer.Write(image);
            writer.Flush();
        }

        /// <summary>
        /// Save JPEG image into stream using the given quality.
        /// </summary>
        public static void SaveJpegCore(BitmapPal image, Stream stream, int jpegQuality)
        {
            SaveCore(image, SKEncodedImageFormat.Jpeg, jpegQuality, stream);
            ImageUtil.SaveJpegResolutionJfif(stream, image.HorizontalResolution, image.VerticalResolution);
        }

        /// <summary>
        /// Save PNG image into stream.
        /// </summary>
        public static void SavePngCore(BitmapPal image, Stream stream)
        {
            // Temporary stream is required to write resolution to PNG since it is written as a separate structure
            // in the middle of the PNG file.
            using (MemoryStream tmpStream = new MemoryStream())
            {
                SaveCore(image, SKEncodedImageFormat.Png, 100, tmpStream);
                ImageUtil.SavePngResolution(tmpStream, stream, image.HorizontalResolution, image.VerticalResolution);
            }
        }

        /// <summary>
        /// Save WebP image into stream using the given quality.
        /// </summary>
        public static void SaveWebPCore(BitmapPal image, Stream stream)
        {
            SaveCore(image, SKEncodedImageFormat.Webp, 100, stream);
        }

        private static void SaveCore(BitmapPal image, SKEncodedImageFormat imageFormat, int quality, Stream dstStream)
        {
            SKBitmap nativeBmp = image.GetNativeBitmap();
            if (nativeBmp != null)
            {
                nativeBmp.Encode(dstStream, imageFormat, quality);
            }
            else
            {
                byte[] noImageBytes = ImageUtil.GetNoImageBytes();
                dstStream.Write(noImageBytes, 0, noImageBytes.Length);
            }
        }
    }
}
#endif
