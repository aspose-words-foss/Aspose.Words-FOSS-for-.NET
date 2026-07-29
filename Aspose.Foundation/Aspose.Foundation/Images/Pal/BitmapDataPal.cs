// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/12/2016 by Alexey Butalov

#if !NETSTANDARD
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Aspose.JavaAttributes;

namespace Aspose.Images.Pal
{
    /// <summary>
    /// Represents a one-dimensional array of pixel data in the 32bppArgb-bitmap. 
    /// </summary>
    /// <remarks>
    /// Maybe it is possible to optimize this class to store data in int[] instead of byte[]. This way 
    /// pixel color could be accessed in one operation instead of 4. Need to check it in profiler.
    /// </remarks>
    [JavaManual("Manual porting by design.")]
    public class BitmapDataPal
    {
        internal BitmapDataPal(Bitmap bitmap, BitmapData bitmapData)
        {
            mBitmap = bitmap;
            mBitmapData = bitmapData;
            mArgbData = new byte[mBitmapData.Stride * mBitmapData.Height];
            Marshal.Copy(mBitmapData.Scan0, mArgbData, 0, mArgbData.Length);
        }

        public void UnlockBits()
        {
            Marshal.Copy(mArgbData, 0, mBitmapData.Scan0, mArgbData.Length);
            mBitmap.UnlockBits(mBitmapData);
        }

        public int GetPixelCount()
        {
            return mBitmapData.Width * mBitmapData.Height;
        }

        public byte[] ArgbData
        {
            get { return mArgbData; }
        }

        public int ArgbDataLength
        {
            get { return mArgbData.Length; }
        }

        // PixelFormat Format32bppArgb has the following unusual sequence
        // byte 0: Blue value of pixel 1
        // Byte 1: Green value of pixel 1
        // Byte 2: Red value of pixel 1
        // Byte 3: Alpha value of pixel 1

        public byte GetA(int i) { return mArgbData[i * 4 + 3]; }

        public void SetA(int i, byte value)
        {
            mArgbData[i * 4 + 3] = value;
        }

        public byte GetR(int i) { return mArgbData[i * 4 + 2]; }

        public byte GetG(int i) { return mArgbData[i * 4 + 1]; }

        public byte GetB(int i) { return mArgbData[i * 4]; }

        public void SetArgb(int i, byte a, byte r, byte g, byte b)
        {
            i *= 4;
            mArgbData[i + 3] = a;
            mArgbData[i + 2] = r;   
            mArgbData[i + 1] = g;
            mArgbData[i + 0] = b;
        }

        private readonly Bitmap mBitmap;
        private readonly BitmapData mBitmapData;
        private readonly byte[] mArgbData;
    }
}
#endif
