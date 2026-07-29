// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2013 by Sergey Merkulov
#if NETSTANDARD

using Aspose.Images.Pal;
using SkiaSharp;
using System;
using System.Runtime.InteropServices;

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

        public SKBitmap Apply(SKBitmap sourceBitmap)
        {
            sourceBitmap = BitmapPal.ConvertToDefaultColorType(sourceBitmap);
            SKImageInfo info = new SKImageInfo(sourceBitmap.Width, sourceBitmap.Height, SKColorType.Gray8, SKAlphaType.Opaque);
            SKBitmap destination = new SKBitmap(info);
            // Do the job.
            ProcessFilter(sourceBitmap, destination, info);
            return destination;
        }

        /// <summary>
        /// Process the filter on the specified image.
        /// </summary>
        /// <param name="sourceData">Source image data.</param>
        /// <param name="destinationData">Destination image data.</param>
        /// <remarks>
        /// Using the <see cref="SKBitmap.SetPixels(IntPtr)"> method may cause memory leaks, so it is preferable to use
        /// the <see cref="SKBitmap.InstallPixels(SKImageInfo, IntPtr, int, SKBitmapReleaseDelegate)"> method.
        /// InstallPixels also optionally takes a delegate that will be invoked on the supplied pixel array pointer when
        /// the bitmap is disposed of. This can be used to do things like unpin arrays, release GC handles,
        /// free memory on the non-GC heap and so on. This lets you pass ownership of a memory buffer to an SkBitmap instance,
        /// and not have to worry about keeping it alive or cleaning it up yourself.
        /// </remarks>
        protected void ProcessFilter(SKBitmap sourceImage, SKBitmap destinationImage, SKImageInfo info)
        {
            int rc = (int)(0x10000 * mRedCoefficient);
            int gc = (int)(0x10000 * mGreenCoefficient);
            int bc = (int)(0x10000 * mBlueCoefficient);

            byte[] grayData = new byte[destinationImage.Width * destinationImage.Height];
            SKColor[] srcPixels = sourceImage.Pixels;
            // Do the job.
            for (int i = 0; i < srcPixels.Length; i++)
            {
                SKColor c = srcPixels[i];
                grayData[i] = (byte)((rc * c.Red + gc * c.Green + bc * c.Blue) >> 16);
            }

            GCHandle dataPin = GCHandle.Alloc(grayData, GCHandleType.Pinned);
            destinationImage.InstallPixels(info, dataPin.AddrOfPinnedObject(), info.RowBytes, (addr, ctx) => dataPin.Free());
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
