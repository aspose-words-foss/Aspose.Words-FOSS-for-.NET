// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2013 by Sergey Merkulov
#if !NETSTANDARD

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Aspose.Drawing;
using Aspose.JavaAttributes;

namespace Aspose.Images.Filters
{
    /// <summary>
    /// Base class for image grayscaling.
    /// Main code and idea are taken from AForge.NET framework.
    /// </summary>
    /// <remarks>
    /// <para>The filter accepts 24, 32 bpp color images and produces
    /// 8 bpp grayscale image.</para>
    /// </remarks>
    [JavaManual("Manually ported to java")]
    internal class Grayscale
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Grayscale"/> class.
        /// </summary>
        /// <param name="cr">Red coefficient.</param>
        /// <param name="cg">Green coefficient.</param>
        /// <param name="cb">Blue coefficient.</param>
        public Grayscale(double cr, double cg, double cb)
        {
            mRedCoefficient = cr;
            mGreenCoefficient = cg;
            mBlueCoefficient = cb;
        }

        /// <summary>
        /// Grayscale image using R-Y algorithm.
        /// </summary>
        /// 
        /// <remarks><para>The instance uses <b>R-Y</b> algorithm to convert color image
        /// to grayscale. The conversion coefficients are:
        /// <list type="bullet">
        /// <item>Red: 0.5;</item>
        /// <item>Green: 0.419;</item>
        /// <item>Blue: 0.081.</item>
        /// </list></para>
        /// </remarks>
        public static Grayscale RMY()
        {
            return new Grayscale(0.5000, 0.4190, 0.0810);
        }

        public Bitmap Apply(Bitmap sourceBitmap)
        {
            CheckSourceFormat(sourceBitmap.PixelFormat);

            // Prepare source data.
            BitmapData sourceData = sourceBitmap.LockBits(
                new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
                ImageLockMode.ReadOnly,
                sourceBitmap.PixelFormat);

            // Prepare destination bitmap and data.
            Bitmap destBitmap = new Bitmap(sourceBitmap.Width, sourceBitmap.Height, PixelFormat.Format8bppIndexed);
            destBitmap.SetResolution(sourceBitmap.HorizontalResolution, sourceBitmap.VerticalResolution);
            SetGrayscalePalette(destBitmap);
            BitmapData destData = destBitmap.LockBits(
                new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            // Do the job.
            ProcessFilter(sourceData, destData);

            // Unlock bitmaps.
            sourceBitmap.UnlockBits(sourceData);
            destBitmap.UnlockBits(destData);

            return destBitmap;
        }

        /// <summary>
        /// Set palette of the 8 bpp indexed image to grayscale.
        /// </summary>
        /// <param name="image">Image to initialize.</param>
        /// <remarks>The method initializes palette of
        /// <see cref="System.Drawing.Imaging.PixelFormat">Format8bppIndexed</see>
        /// image with 256 gradients of gray color.</remarks>
        public static void SetGrayscalePalette(Bitmap image)
        {
            // Check pixel format.
            if (image.PixelFormat != PixelFormat.Format8bppIndexed)
                throw new InvalidOperationException("Source image is not 8 bpp image.");

            // Get palette.
            ColorPalette cp = image.Palette;
            // Init palette.
            for (int i = 0; i < 256; i++)
            {
                cp.Entries[i] = Color.FromArgb(i, i, i);
            }
            // Set palette back.
            image.Palette = cp;
        }

        private static void CheckSourceFormat(PixelFormat pixelFormat)
        {
            if (pixelFormat == PixelFormat.Format24bppRgb || pixelFormat == PixelFormat.Format32bppRgb ||
                pixelFormat == PixelFormat.Format32bppArgb)
                return;

            throw new InvalidOperationException("Unsupported pixel format");
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// 
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// 
        protected void ProcessFilter(BitmapData sourceData, BitmapData destinationData)
        {
            // Get width and height.
            int width = sourceData.Width;
            int height = sourceData.Height;

            int pixelSize = (sourceData.PixelFormat == PixelFormat.Format24bppRgb) ? 3 : 4;
            int srcOffset = sourceData.Stride - width * pixelSize;
            int dstOffset = destinationData.Stride - width;

            int rc = (int)(0x10000 * mRedCoefficient);
            int gc = (int)(0x10000 * mGreenCoefficient);
            int bc = (int)(0x10000 * mBlueCoefficient);
            
            int sourcePointer = 0;
            int destPointer = 0;

            // Get source image bytes.
            byte[] srcData = new byte[sourceData.Height * sourceData.Stride];
            Marshal.Copy(sourceData.Scan0, srcData, 0, srcData.Length);

            // Prepare destination buffer.
            byte[] destBuffer = new byte[destinationData.Height * destinationData.Stride];

            // Do the job.
            // For each line
            for (int y = 0; y < height; y++)
            {
                // For each pixel
                for (int x = 0; x < width; x++, sourcePointer += pixelSize, destPointer++)
                {
                    destBuffer[destPointer] =
                        (byte)
                        ((rc * srcData[sourcePointer + DrColor.RIndex] + gc * srcData[sourcePointer + DrColor.GIndex] +
                          bc * srcData[sourcePointer + DrColor.BIndex]) >> 16);
                }
                sourcePointer += srcOffset;
                destPointer += dstOffset;
            }

            Marshal.Copy(destBuffer, 0, destinationData.Scan0, destBuffer.Length);
        }

        /// <summary>
        /// Portion of red channel's value to use during conversion from RGB to grayscale.
        /// </summary>
        private readonly double mRedCoefficient;

        /// <summary>
        /// Portion of green channel's value to use during conversion from RGB to grayscale.
        /// </summary>
        private readonly double mGreenCoefficient;

        /// <summary>
        /// Portion of blue channel's value to use during conversion from RGB to grayscale.
        /// </summary>
        private readonly double mBlueCoefficient;
    }
}
#endif
