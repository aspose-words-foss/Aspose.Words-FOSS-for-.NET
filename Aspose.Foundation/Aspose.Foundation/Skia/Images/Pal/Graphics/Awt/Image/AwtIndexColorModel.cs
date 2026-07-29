// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD

using System;

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal class AwtIndexColorModel: AwtColorModel
    {
        public AwtIndexColorModel(int bits, int size, byte[] r, byte[] g, byte[] b) : this(bits, size, r, g, b, null)
        {
        }

        public AwtIndexColorModel(int bits, int size, byte[] r, byte[] g, byte[] b, byte[] a) :
            base(bits, gOpaqueBits, AwtColorSpace.GetInstance(AwtColorSpaces.SRgb), false, false, AwtTransparency.Opaque, AwtColorModel.GetDefaultTransferType(bits))
        {
            if (bits < 1 || bits > 16)
                throw new ArgumentException("Number of bits must be between 1 and 16.");

            SetRgbs(size, r, g, b, a);
            CalculatePixelMask();
        }

        public override int GetRed(int pixel)
        {
            return (mRgb[pixel & mPixelMask] >> 16) & 0xff;
        }

        public override int GetGreen(int pixel)
        {
            return (mRgb[pixel & mPixelMask] >> 8) & 0xff;
        }

        public override int GetBlue(int pixel)
        {
            return mRgb[pixel & mPixelMask] & 0xff;
        }

        public override int GetAlpha(int pixel)
        {
            return (mRgb[pixel & mPixelMask] >> 24) & 0xff;
        }

        public override int GetRgb(int pixel)
        {
            return mRgb[pixel & mPixelMask];
        }

        private void SetRgbs(int size, byte[] r, byte[] g, byte[] b, byte[] a)
        {
            if (size < 1)
                throw new ArgumentException("Map size (" + size + ") must be >= 1");

            mMapSize = size;
            mRgb = new int[CalcRealMapSize(mPixelBits, size)];
            int alpha = 0xff;
            AwtTransparency transparency = AwtTransparency.Opaque;
            bool allgray = true;
            for (int i = 0; i < size; i++)
            {
                int rc = r[i] & 0xff;
                int gc = g[i] & 0xff;
                int bc = b[i] & 0xff;
                allgray = allgray && (rc == gc) && (gc == bc);
                if (a != null)
                {
                    alpha = a[i] & 0xff;
                    if (alpha != 0xff)
                    {
                        if (alpha == 0x00)
                        {
                            if (transparency == AwtTransparency.Opaque)
                                transparency = AwtTransparency.Bitmask;
                            if (mTransparentIndex < 0)
                                mTransparentIndex = i;

                        }
                        else
                            transparency = AwtTransparency.Translucent;

                        allgray = false;
                    }
                }
                mRgb[i] = (alpha << 24) | (rc << 16) | (gc << 8) | bc;
            }
            mAllGrayOpaque = allgray;
            SetTransparency(transparency);
        }

        private void CalculatePixelMask()
        {
            // Note that we adjust the mask so that our masking behavior here
            // is consistent with that of our native rendering loops.
            int maskbits = mPixelBits;
            if (maskbits == 3)
                maskbits = 4;

            else if (maskbits > 4 && maskbits < 8)
                maskbits = 8;

            mPixelMask = (1 << maskbits) - 1;
        }

        private int CalcRealMapSize(int bits, int size)
        {
            int newSize = Math.Max(1 << bits, size);
            return Math.Max(newSize, 256);
        }

        private void SetTransparency(AwtTransparency transparency)
        {
            if (mTransparency != transparency)
            {
                mTransparency = transparency;
                if (transparency == AwtTransparency.Opaque)
                {
                    mSupportsAlpha = false;
                    mNumComponents = 3;
                    mNBits = gOpaqueBits;
                }
                else
                {
                    mSupportsAlpha = true;
                    mNumComponents = 4;
                    mNBits = gAlphaBits;
                }
            }
        }

        private int[] mRgb;
        private int mMapSize;
        private int mPixelMask;
        private int mTransparentIndex = -1;
        private bool mAllGrayOpaque;

        private static readonly int[] gOpaqueBits = { 8, 8, 8 };
        private static readonly int[] gAlphaBits = { 8, 8, 8, 8 };
    }
}
#endif
