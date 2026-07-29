// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/06/2012 by Vyacheslav Durin
#if NETSTANDARD

using System;
using System.Drawing;
using System.IO;
using Aspose.Common;
using Aspose.Fonts;
using Aspose.IO;
using SkiaSharp;

namespace Aspose.Images.Pal
{
    /// <summary>
    /// This class is to be ported manually to Java.
    /// </summary>
    public static class BitmapUtilPal
    {
        /// <summary>
        /// Converts a metafile as a byte array into a bitmap and saves it to a byte array.
        /// </summary>
        /// <param name="imageBytes">Metafile bytes.</param>
        /// <param name="resolution">Metafile resolution.</param>
        /// <param name="emulateRasterOperations"></param>
        /// <returns>Bitmap as a bytes array.</returns>
        public static byte[] ConvertMetafileToBitmapUsingGdiPlus(byte[] imageBytes, SizeF resolution,
            bool emulateRasterOperations)
        {
            using (MemoryStream stream = new MemoryStream(imageBytes))
                return ConvertMetafileToBitmapUsingGdiPlus(stream, resolution, emulateRasterOperations, null);
        }

        /// <summary>
        /// Converts a metafile as a stream into a bitmap and saves it to a byte array.
        /// </summary>
        /// <param name="imageStream">Metafile stream.</param>
        /// <param name="resolution">Metafile resolution.</param>
        /// <param name="emulateRasterOperations"></param>
        /// <param name="fontProvider"></param>
        /// <returns>Bitmap as a bytes array.</returns>
        public static byte[] ConvertMetafileToBitmapUsingGdiPlus(Stream imageStream, SizeF resolution,
            bool emulateRasterOperations, IFontProvider fontProvider)
        {
            Debug.Assert(resolution.Height != 0 && resolution.Width != 0);

            byte[] resultImgBytes = null;

#if JAVA
            if (JavaDebug.ON)
                Log.i("AW", "PalBitmapUtil#convertMetafileToBitmapUsingGdiPlus res=" + resolution);
#endif
            using (BitmapPal bitmap = new BitmapPal(imageStream))
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Copy bitmap as a png file into a byte array.
                bitmap.Save(memoryStream, FileFormat.Png);
                resultImgBytes = StreamUtil.CopyStreamToByteArray(memoryStream);
            }

            if (resultImgBytes == null || resultImgBytes.Length <= 0)
                resultImgBytes = StreamUtil.CopyStreamToByteArray(imageStream);

            return resultImgBytes;
        }

#if DEBUG
        /// <summary>
        /// Used for graphics debugging. Appropriate extension will be calculated from
        /// imageBytes and added to fileName.
        /// </summary>
        public static void SaveImageBytes(byte[] imageBytes, string fileName)
        {
            FileFormat type = ImageUtil.GetImageType(imageBytes);
            string ext = FileFormatCore.ToExt(type);

            using (FileStream stream = new FileStream(fileName + "." + ext, FileMode.Create))
                stream.Write(imageBytes, 0, imageBytes.Length);
        }

        /// <summary>
        /// Used for graphics debugging.
        /// </summary>
        public static void SaveNativeImageToPngFile(SKBitmap image, string fileName)
        {
            using (FileStream stream = new FileStream(fileName + ".png", FileMode.Create))
            {
                BitmapPal bitmap = new BitmapPal(image);
                bitmap.SavePng(stream);
            }
        }
#endif
    }
}
#endif
