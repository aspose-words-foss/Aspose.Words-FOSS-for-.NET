// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2013 by Sergey Merkulov
#if NETSTANDARD || NET

using System;
using Aspose.IO;
using Aspose.JavaAttributes;
using SkiaSharp;

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
        public SKBitmap Apply(SKBitmap sourceImage)
        {
            CheckSourceFormat(sourceImage.ColorType);

            // Prepare source data.
            PrepareSourceData(sourceImage);
            byte[] bytes = sourceImage.Bytes;


            // Prepare destination image bytes and BitWriter associated with it.
            int destDataStride = sourceImage.Width / 8 + (sourceImage.Width % 8 > 0 ? 1 : 0);
            byte[] destBytes = new byte[Height * destDataStride];
            BitWriter writer = new BitWriter(destBytes);

            int ptr = 0;
            int offset = Stride - Width;

            // Do the job:
            // For each line
            for (Y = 0; Y < Height; Y++)
            {
                writer.ByteIndex = Y * destDataStride;
                // For each pixel
                for (X = 0; X < Width; X++, ptr++)
                {
                    ProcessPixel(bytes, writer, ptr);
                }
                ptr += offset;
            }

            return BitonalConverter.CreateBitmapFrom1bppBytes(destBytes, sourceImage.Width, sourceImage.Height);
        }

        private static void CheckSourceFormat(SKColorType pixelFormat)
        {
            if (pixelFormat == SKColorType.Gray8)
                return;

            throw new ArgumentException("Unsupported pixel format");
        }

        private void PrepareSourceData(SKBitmap sourceImage)
        {
            // get image size
            Width = sourceImage.Width;
            Height = sourceImage.Height;
            Stride = Width;
        }

        private void ProcessPixel(byte[] bytes, BitWriter writer, int ptr)
        {
            // pixel value
            int v = (bytes[ptr] & 0xFF);
            // error value
            int error;

            // fill the next destination pixel
            if (v >= (mThreshold & 0xFF))
            {
                bytes[ptr] = (byte)255;
                error = v - 255;
            }
            else
            {
                bytes[ptr] = (byte)0;
                error = v;
            }

            // Set correct bit in result 1bpp image.
            if ((bytes[ptr] & 0xFF) == 255)
                writer.WriteOneInCurrentBit();
            writer.MoveToNextBit();

            // do error diffusion
            Diffuse(error, bytes, ptr);
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

        //Java-deleted: not used in java
        /// <summary>
        /// Processing image's pixel size in bytes.
        /// </summary>
        //protected int PixelSize;

        private byte mThreshold = (byte)128;
    }
}

#endif
