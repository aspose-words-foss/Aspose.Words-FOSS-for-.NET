// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2013 by Sergey Merkulov
#if !NETSTANDARD
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Aspose.IO;
using Aspose.JavaAttributes;

namespace Aspose.Images.Filters
{
    /// <summary>
    /// Base class for error diffusion dithering.
    /// Main code and idea are taken from AForge.NET framework.
    /// </summary>
    /// <remarks><para>The class is the base class for binarization algorithms based on
    /// <a href="http://en.wikipedia.org/wiki/Error_diffusion">error diffusion</a>.</para>
    /// <para>Each pixel is binarized based not only
    /// on its own value, but on values of some surrounding pixels. During pixel's binarization, its <b>binarization
    /// error</b> is distributed (diffused) to some neighbor pixels with some coefficients. This error diffusion
    /// updates neighbor pixels changing their values, what affects their upcoming binarization. Error diffuses
    /// only on unprocessed yet neighbor pixels, which are right and bottom pixels usually (in the case if image
    /// processing is done from upper left corner to bottom right corner). <b>Binarization error</b> equals
    /// to processing pixel value, if it is below threshold value, or pixel value minus 255 otherwise.</para>
    /// <para>The filter accepts 8 bpp grayscale images for processing.</para>
    /// </remarks>
    [JavaManual("Manually ported to java")]
    internal abstract class ErrorDiffusionDithering
    {
        /// <summary>
        /// Perform color dithering for the specified image.
        /// <see cref="sourceImage"/> must be 8 bpp grayscale image.
        /// The output bitmap is 1 bpp image. 
        /// </summary>
        public Bitmap Apply(Bitmap sourceImage)
        {
            CheckSourceFormat(sourceImage.PixelFormat);

            // Prepare source data.
            BitmapData sourceData = PrepareSourceData(sourceImage);
            byte[] bytes = new byte[sourceData.Height * sourceData.Stride];
            Marshal.Copy(sourceData.Scan0, bytes, 0, bytes.Length);

            // Create destination image.
            Bitmap destImage = new Bitmap(sourceImage.Width, sourceImage.Height, PixelFormat.Format1bppIndexed);
            destImage.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);
            
            // Lock destination image.
            BitmapData destData = destImage.LockBits(new Rectangle(0, 0, Width, Height),
                ImageLockMode.WriteOnly, destImage.PixelFormat);
            // Prepare destination image bytes and BitWriter associated with it.
            byte[] destBytes = new byte[Height*destData.Stride];
            BitWriter writer = new BitWriter(destBytes);
            
            int ptr = 0;
            int offset = Stride - Width;

            // Do the job:
            // For each line
            for (Y = 0; Y < Height; Y++)
            {
                writer.ByteIndex = Y*destData.Stride;
                // For each pixel
                for (X = 0; X < Width; X++, ptr++)
                {
                    ProcessPixel(bytes, writer, ptr);
                }
                ptr += offset;
            }

            FinishProcess(sourceImage, sourceData, destImage, destData, destBytes);

            return destImage;
        }

        private static void CheckSourceFormat(PixelFormat pixelFormat)
        {
            if (pixelFormat == PixelFormat.Format8bppIndexed)
                return;

            throw new InvalidOperationException("Unsupported pixel format");
        }

        private BitmapData PrepareSourceData(Bitmap sourceImage)
        {
            BitmapData sourceData = sourceImage.LockBits(
                new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                ImageLockMode.ReadOnly, sourceImage.PixelFormat);

            // get image size
            Width = sourceData.Width;
            Height = sourceData.Height;
            Stride = sourceData.Stride;
            PixelSize = Bitmap.GetPixelFormatSize(sourceImage.PixelFormat) / 8;
            return sourceData;
        }

        private void ProcessPixel(byte[] bytes, BitWriter writer, int ptr)
        {
            // pixel value
            int v = bytes[ptr];
            // error value
            int error;
            
            // fill the next destination pixel
            if (v >= mThreshold)
            {
                bytes[ptr] = 255;
                error = v - 255;
            }
            else
            {
                bytes[ptr] = 0;
                error = v;
            }

            // Set correct bit in result 1bpp image.
            if(bytes[ptr]==255)
                writer.WriteOneInCurrentBit();
            writer.MoveToNextBit();

            // do error diffusion
            Diffuse(error, bytes, ptr);
        }
        
        private static void FinishProcess(Bitmap sourceImage, BitmapData sourceData, Bitmap destImage, BitmapData destData, byte[] destBytes)
        {
            sourceImage.UnlockBits(sourceData);
            Marshal.Copy(destBytes, 0, destData.Scan0, destBytes.Length);
            destImage.UnlockBits(destData);
        }

        /// <summary>
        /// Do error diffusion.
        /// </summary>
        /// <param name="error">Current error value.</param>
        /// <param name="bytes">Pixels data.</param>
        /// <param name="ptr">Pointer to current processing pixel.</param>
        protected abstract void Diffuse(int error, byte[] bytes, int ptr);

        /// <summary>
        /// Threshold value.
        /// </summary>
        /// <remarks>Default value is 128.</remarks>
        public byte ThresholdValue
        {
            get { return mThreshold; }
            set { mThreshold = value; }
        }

        /// <summary>
        /// Current processing X coordinate.
        /// </summary>
        protected int X;

        /// <summary>
        /// Current processing Y coordinate.
        /// </summary>
        protected int Y;

        /// <summary>
        /// Processing image's width.
        /// </summary>
        protected int Width;

        /// <summary>
        /// Processing image's height.
        /// </summary>
        protected int Height;

        /// <summary>
        /// Processing image's stride (line size).
        /// </summary>
        protected int Stride;

        /// <summary>
        /// Processing image's pixel size in bytes.
        /// </summary>
        protected int PixelSize;

        private byte mThreshold = 128;
    }
}
#endif
