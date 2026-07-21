// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/06/2012 by Vyacheslav Durin
#if !NETSTANDARD
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using Aspose.Fonts;
using Aspose.IO;
using Aspose.JavaAttributes;

namespace Aspose.Images.Pal
{
    /// <summary>
    /// This class is to be ported manually to Java.
    /// </summary>
    [JavaManual("Manual porting by design.")]
    public static class BitmapUtilPal
    {
        /// <summary>
        /// Converts a metafile as a byte array into a bitmap and saves it to a byte array.
        /// </summary>
        /// <param name="imageBytes">Metafile bytes.</param>
        /// <param name="resolution">Metafile resolution.</param>
        /// <param name="emulateRasterOperations">Used only in Java.</param>
        /// <returns>Bitmap as a bytes array.</returns>
        public static byte[] ConvertMetafileToBitmapUsingGdiPlus(byte[] imageBytes, SizeF resolution, bool emulateRasterOperations)
        {
            using (MemoryStream stream = new MemoryStream(imageBytes))
            {
                return ConvertMetafileToBitmapUsingGdiPlus(stream, resolution, emulateRasterOperations, null);
            }
        }

        /// <summary>
        /// Converts a metafile as a stream into a bitmap and saves it to a byte array.
        /// </summary>
        /// <param name="imageStream">Metafile stream.</param>
        /// <param name="resolution">Metafile resolution.</param>
        /// <param name="emulateRasterOperations">Used only in Java.</param>
        /// <param name="fontProvider">Used only in Java.</param>
        /// <returns>Bitmap as a bytes array.</returns>
        public static byte[] ConvertMetafileToBitmapUsingGdiPlus(Stream imageStream, SizeF resolution, bool emulateRasterOperations, IFontProvider fontProvider)
        {
            Debug.Assert(resolution.Height != 0 && resolution.Width != 0);
            using (Image newImage = Image.FromStream(imageStream))
            {
                int newResolution = ImageConstants.PrintResolution;
                int newWidth;
                int newHeight;

                // WORDSNET-22288 The metafile could have very big values for width and height. 
                // Using these values when creating a bitmap could result in a very big bitmap.
                // This loop attempts to find suitable bitmap width and height that will not crash the system.
                while (true)
                {
                    float resizeRatioVertical = newResolution / resolution.Height;
                    float resizeRatioHorizontal = newResolution / resolution.Width;

                    newWidth = (int)(newImage.Size.Width * resizeRatioHorizontal);
                    newHeight = (int)(newImage.Size.Height * resizeRatioVertical);

                    if (ImageUtil.IsBitmapTooBigForSystem(newWidth, newHeight))
                        newResolution /= 2;
                    else
                        break;
                }

                using (Bitmap bitmap = new Bitmap(newWidth, newHeight))
                {
                    bitmap.SetResolution(newResolution, newResolution);

                    // Create a bitmap using image bytes.
                    using (Graphics g = GraphicsFromImage(bitmap))
                    {
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        g.DrawImage(newImage, 0, 0);
                    }

                    // Copy bitmap as a png file into a byte array.
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        bitmap.Save(memoryStream, ImageFormat.Png);
                        return StreamUtil.CopyStreamToByteArray(memoryStream);
                    }
                }
            }
        }

        /// <summary>
        /// Creates Graphics object from an image.
        /// It can also prepare the most suitable/stable options if placed here.
        /// </summary>
        public static Graphics GraphicsFromImage(Image image)
        {
            Graphics g = Graphics.FromImage(image);
            return g;
        }

    }
}
#endif
