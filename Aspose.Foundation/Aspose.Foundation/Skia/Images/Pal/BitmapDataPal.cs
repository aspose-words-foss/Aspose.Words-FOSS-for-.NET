// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/08/2017 by Alexey Noskov

#if NETSTANDARD || NET

using Aspose.Drawing;
using SkiaSharp;

namespace Aspose.Images.Pal
{
    /// <summary>
    /// Represents a one-dimensional array of pixel data in the 32bppArgb-bitmap.
    /// </summary>
    public class BitmapDataPal
    {
        internal BitmapDataPal(SKBitmap bitmap)
        {
            mBitmap = bitmap;
            mPixelData = mBitmap.Pixels;
        }

        public void UnlockBits()
        {
            if (mArgbData != null)
            {
                for (int i = 0; i < mPixelData.Length; i++)
                {
                    int j = i * 4;
                    mPixelData[i] = new SKColor(mArgbData[j + 2], mArgbData[j + 1], mArgbData[j + 0], mArgbData[j + 3]);
                }
            }

            mBitmap.Pixels = mPixelData;
        }

        /// <summary>
        /// Gets or sets ARGB value of the specified pixel.
        /// </summary>
        internal int this[int i]
        {
            get { return GetArgb(i); }
            set { SetArgb(i, value); }
        }

        public int GetPixelCount()
        {
            return mPixelData.Length;
        }

        public void SetA(int i, int alpha)
        {
            int r = GetR(i);
            int g = GetG(i);
            int b = GetB(i);
            SetArgb(i, alpha, r, g, b);
        }

        public byte GetA(int i)
        {
            if (mArgbData != null)
                return mArgbData[GetArgbIndexA(i)];

            return mPixelData[i].Alpha;
        }

        public byte GetR(int i)
        {
            if (mArgbData != null)
                return mArgbData[GetArgbIndexR(i)];

            return mPixelData[i].Red;
        }

        public byte GetG(int i)
        {
            if (mArgbData != null)
                return mArgbData[GetArgbIndexG(i)];

            return mPixelData[i].Green;
        }

        public byte GetB(int i)
        {
            if (mArgbData != null)
                return mArgbData[GetArgbIndexB(i)];

            return mPixelData[i].Blue;
        }

        public void SetArgb(int i, int a, int r, int g, int b)
        {
            if (mArgbData != null)
            {
                mArgbData[GetArgbIndexA(i)] = (byte)a;
                mArgbData[GetArgbIndexR(i)] = (byte)r;
                mArgbData[GetArgbIndexG(i)] = (byte)g;
                mArgbData[GetArgbIndexB(i)] = (byte)b;
                return;
            }

            SKColor color = new SKColor((byte)r, (byte)g, (byte)b, (byte)a);
            mPixelData[i] = color;
        }

        private int GetArgb(int i)
        {
            if (mArgbData != null)
                return new DrColor(
                    mArgbData[GetArgbIndexA(i)],
                    mArgbData[GetArgbIndexR(i)],
                    mArgbData[GetArgbIndexG(i)],
                    mArgbData[GetArgbIndexB(i)]).ToArgb();

            DrColor c = new DrColor(mPixelData[i].Alpha, mPixelData[i].Red, mPixelData[i].Green, mPixelData[i].Blue);
            return c.ToArgb();
        }

        private void SetArgb(int i, int argb)
        {
            if (mArgbData != null)
            {
                DrColor color = new DrColor(argb);
                mArgbData[GetArgbIndexA(i)] = (byte)color.A;
                mArgbData[GetArgbIndexR(i)] = (byte)color.R;
                mArgbData[GetArgbIndexG(i)] = (byte)color.G;
                mArgbData[GetArgbIndexB(i)] = (byte)color.B;
                return;
            }

            mPixelData[i] = new SKColor((uint)argb);
        }

        public byte[] ArgbData
        {
            get
            {
                // This is not the most eficient way to get array of bytes, but the other option is to operate with pointers.
                // According to this article working with pixels via mBitmap.Pixels is not the worst option.
                // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/bitmaps/pixel-bits
                if (mArgbData == null)
                {
                    mArgbData = new byte[mPixelData.Length * 4];
                    for (int i = 0; i < mPixelData.Length; i++)
                    {
                        int j = i * 4;
                        mArgbData[j + 3] = mPixelData[i].Alpha;
                        mArgbData[j + 2] = mPixelData[i].Red;
                        mArgbData[j + 1] = mPixelData[i].Green;
                        mArgbData[j + 0] = mPixelData[i].Blue;
                    }
                }
                return mArgbData;
            }
        }

        private int GetArgbIndexB(int i)
        {
            return i * 4;
        }

        private int GetArgbIndexG(int i)
        {
            return i * 4 + 1;
        }

        private int GetArgbIndexR(int i)
        {
            return i * 4 + 2;
        }

        private int GetArgbIndexA(int i)
        {
            return i * 4 + 3;
        }

        private readonly SKBitmap mBitmap;
        private readonly SKColor[] mPixelData;
        private byte[] mArgbData;
    }
}
#endif
