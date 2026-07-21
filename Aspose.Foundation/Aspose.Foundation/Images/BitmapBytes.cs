// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/05/2011 by Alexey Titov

using Aspose.JavaAttributes;
#if !NETSTANDARD
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
#else
using SkiaSharp;
#endif

namespace Aspose.Images
{
    /// <summary>
    /// Helper class containing bytes of a bitmap and size of the bitmap.
    /// </summary>
    [JavaDelete("Not needed in java.")]
    [AndroidManual("Not needed in java. But it needs to be ported on Android manually.")]
    internal class BitmapBytes
    {
#if !NETSTANDARD
        public BitmapBytes(BitmapData bitmapData)
        {
            mWidth = bitmapData.Width;
            mHeight = bitmapData.Height;
            mStride = bitmapData.Stride;

            // Copy image data to binary array
            int imageSize = bitmapData.Stride * bitmapData.Height;
            mBytes = new byte[imageSize];
            Marshal.Copy(bitmapData.Scan0, mBytes, 0, imageSize);
        }
#else
        public BitmapBytes(SKBitmap bitmapData)
        {
            mWidth = bitmapData.Width;
            mHeight = bitmapData.Height;
            mStride = CalculateStride(bitmapData);
            mBytes = bitmapData.Bytes;
        }

        private static int CalculateStride(SKBitmap bitmapData)
        {
            // we always work with Bitmap.Config.ARGB_8888
            // so we know each pixel is stored on 4 bytes.
            return 4 * bitmapData.Width; // must be power of 2?
        }
#endif

        public byte[] Bytes
        {
            get { return mBytes; }
        }

        public int Width
        {
            get { return mWidth; }
        }

        public int Height
        {
            get { return mHeight; }
        }

        public int Stride
        {
            get { return mStride; }
        }

        private readonly byte[] mBytes;
        private readonly int mHeight;
        private readonly int mStride;
        private readonly int mWidth;
    }
}
