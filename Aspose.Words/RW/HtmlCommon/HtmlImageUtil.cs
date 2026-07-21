// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/10/2016 by Nikolay Sezganov

using System;
using System.Drawing;
using System.IO;
using Aspose.Images;
using Aspose.Images.Pal;
using Aspose.IO;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Saving;
#if NETSTANDARD
using Image = SkiaSharp.SKBitmap;
#endif

namespace Aspose.Words.RW.HtmlCommon
{
    /// <summary>
    /// Utility functions to work with images in HTML import/export.
    /// </summary>
    internal static class HtmlImageUtil
    {
        /// <summary>
        /// Checks an image on possibility of inserting to the document model.
        /// </summary>
        internal static bool IsInsertableImage(MemoryStream imageStream)
        {

            if (imageStream == null)
                return false;

            if (IsJpeg(imageStream))
                return true;

            if (IsPng(imageStream))
                return true;

            if (IsGif(imageStream))
                return true;

            if (IsSvg(imageStream))
                return true;

            try
            {
                byte[] imageData = imageStream.ToArray();
                return IsSupportedImage(imageData) || ImageUtil.IsPictFile(imageData);
            }
            catch (EndOfStreamException)
            {
                // Format detection failed. This image format is not supported.
                return false;
            }
        }

        /// <summary>
        /// Indicates whether the image is visible in IE when attached to an MHTML file.
        /// </summary>
        internal static bool IsSupportedAttachedImageFormat(MemoryStream imageStream)
        {
            if (imageStream == null)
            {
                return false;
            }

            return IsJpeg(imageStream) ||
                IsPng(imageStream) ||
                IsGif(imageStream) ||
                IsIco(imageStream) ||
                IsBmp(imageStream);
        }

        /// <summary>
        /// Checks whether image format is supported by MSW as is or after conversion.
        /// </summary>
        internal static bool IsSupportedImage(byte[] imageBytes)
        {
            return GetSupportedImageBytes(imageBytes) != null;
        }

        /// <summary>
        /// Makes sure the image is fully supported by our document model.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///   Some image formats are not fully supported by our document model. For example: GIF, TIFF, BMP (see WORDSNET-1878).
        ///   </para>
        ///   <para>
        ///   If the format of the original image is fully supported, the source image bytes are returned as is.
        ///   Otherwise, the image is converted to PNG and the method returns PNG image bytes.
        ///   </para>
        /// </remarks>
        internal static byte[] GetSupportedImageBytes(byte[] imageBytes)
        {
            try
            {
                return ImageDataCore.GetImageBytes(imageBytes);
            }
            catch (CantCreateBitmapException)
            {
                return null;
            }
        }

        /// <summary>
        /// Checks an image on possibility of inserting to the document model.
        /// </summary>
        internal static bool IsInsertableImage(byte[] imageBytes)
        {
            if ((imageBytes == null) || (imageBytes.Length == 0))
                return false;

            using (MemoryStream ms = new MemoryStream(imageBytes))
            {
                return IsInsertableImage(ms);
            }
        }

        /// <summary>
        /// Returns byte array with the radiobutton image in PNG format.
        /// </summary>
        internal static byte[] CreateRadioButtonImage(Document doc, bool selected)
        {
            throw new NotSupportedException("FOSS");
        }

        private static bool IsGif(MemoryStream imageStream)
        {
            try
            {
                imageStream.Position = 0;
                if (ImageUtil.IsGif(imageStream))
                    return true;
            }
            catch (EndOfStreamException)
            {
                // Format detection failed. This is not a GIF image.
            }

            return false;
        }

        private static bool IsPng(MemoryStream imageStream)
        {
            try
            {
                imageStream.Position = 0;
                if (ImageUtil.IsPng(imageStream))
                    return true;
            }
            catch (EndOfStreamException)
            {
                // Format detection failed. This is not a PNG image.
            }

            return false;
        }

        private static bool IsJpeg(MemoryStream imageStream)
        {
            try
            {
                imageStream.Position = 0;
                if (ImageUtil.IsJpeg(imageStream))
                    return true;
            }
            catch (EndOfStreamException)
            {
                // Format detection failed. This is not a JPEG image.
            }

            return false;
        }

        private static bool IsBmp(MemoryStream imageStream)
        {
            try
            {
                imageStream.Position = 0;
                return ImageUtil.IsBmp(imageStream);
            }
            catch (EndOfStreamException)
            {
                // Format detection failed. This is not a BMP image.
                return false;
            }
        }

        private static bool IsIco(MemoryStream imageStream)
        {
            try
            {
                imageStream.Position = 0;
                return ImageUtil.IsIco(imageStream);
            }
            catch (EndOfStreamException)
            {
                // Format detection failed. This is not an ICO image.
                return false;
            }
        }

        private static bool IsSvg(MemoryStream imageStream)
        {
            try
            {
                imageStream.Position = 0;
                return ImageUtil.IsSvg(imageStream);
            }
            catch (EndOfStreamException)
            {
                // Format detection failed. This is not a SVG image.
                return false;
            }
        }
    }
}
