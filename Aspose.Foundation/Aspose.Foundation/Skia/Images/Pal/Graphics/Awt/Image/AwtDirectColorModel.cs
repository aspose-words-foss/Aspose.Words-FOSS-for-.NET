// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD

using System;

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal class AwtDirectColorModel : AwtPackedColorModel
    {
        public AwtDirectColorModel(AwtColorSpace space, int bits, int rmask, int gmask, int bmask, int amask, bool isAlphaPremultiplied, AwtDataBufferType transferType) :
            base(space, bits, rmask, gmask, bmask, amask, isAlphaPremultiplied, amask == 0 ? AwtTransparency.Opaque : AwtTransparency.Translucent, transferType)
        {
            if (IsLinearRgbSpace(mColorSpace))
            {
                mIsLinearRgb = true;
                if (mMaxBits <= 8)
                {
                    mLRgbPrecision = 8;
                    mToSRgb8LUT = AwtColorModel.GetLinearRgb8TosRgb8LUT();
                    mFromSRgb8LUT8 = AwtColorModel.GetsRgb8ToLinearRgb8LUT();
                }
                else
                {
                    mLRgbPrecision = 16;
                    mToSRgb8LUT = AwtColorModel.GetLinearRgb16TosRgb8LUT();
                    mFromsRgb8LUT16 = AwtColorModel.GetsRgb8ToLinearRgb16LUT();
                }
            }
            else if (!mIsSrgb)
            {
                for (int i = 0; i < 3; i++)
                {
                    // super constructor checks that space is TYPE_RGB
                    // check here that min/max are all 0.0/1.0
                    if ((space.GetMinValue(i) != 0.0f) || (space.GetMaxValue(i) != 1.0f))
                    {
                        throw new ArgumentException("Illegal min/max RGB component value");
                    }
                }
            }
            SetFields();
        }

        public override int GetRed(int pixel)
        {
            if (mIsSrgb)
                return GetsRgbComponentFromSRgb(pixel, 0);

            else if (mIsLinearRgb)
                return GetsRgbComponentFromLinearRgb(pixel, 0);

            float[] rgb = GetDefaultRgbComponents(pixel);
            return (int)(rgb[0] * 255.0f + 0.5f);
        }

        public override int GetGreen(int pixel)
        {
            if (mIsSrgb)
                return GetsRgbComponentFromSRgb(pixel, 1);

            else if (mIsLinearRgb)
                return GetsRgbComponentFromLinearRgb(pixel, 1);

            float[] rgb = GetDefaultRgbComponents(pixel);
            return (int)(rgb[1] * 255.0f + 0.5f);
        }

        public override int GetBlue(int pixel)
        {
            if (mIsSrgb)
                return GetsRgbComponentFromSRgb(pixel, 2);

            else if (mIsLinearRgb)
                return GetsRgbComponentFromLinearRgb(pixel, 2);

            float[] rgb = GetDefaultRgbComponents(pixel);
            return (int)(rgb[2] * 255.0f + 0.5f);
        }

        public override int GetAlpha(int pixel)
        {
            if (!mSupportsAlpha) return 255;
            int a = ((pixel & mMaskArray[3]) >> mMaskOffsets[3]);
            if (mScaleFactors[3] != 1.0f)
                a = (int)(a * mScaleFactors[3] + 0.5f);

            return a;
        }

        public override int GetRgb(object inData)
        {
            int pixel = 0;
            switch (mTransferType)
            {
                case AwtDataBufferType.Byte:
                    byte[] bdata = (byte[])inData;
                    pixel = bdata[0] & 0xff;
                    break;
                case AwtDataBufferType.Ushort:
                    short[] sdata = (short[])inData;
                    pixel = sdata[0] & 0xffff;
                    break;
                case AwtDataBufferType.Int:
                    int[] idata = (int[])inData;
                    pixel = idata[0];
                    break;
                default:
                    throw new NotImplementedException("This method has not been implemented for transferType " + mTransferType);
            }
            return GetRgb(pixel);
        }

        public override int GetRgb(int pixel)
        {
            if (mIsSrgb || mIsLinearRgb)
            {
                return (GetAlpha(pixel) << 24)
                        | (GetRed(pixel) << 16)
                        | (GetGreen(pixel) << 8)
                        | (GetBlue(pixel) << 0);
            }
            float[] rgb = GetDefaultRgbComponents(pixel);
            return (GetAlpha(pixel) << 24)
                    | (((int)(rgb[0] * 255.0f + 0.5f)) << 16)
                    | (((int)(rgb[1] * 255.0f + 0.5f)) << 8)
                    | (((int)(rgb[2] * 255.0f + 0.5f)) << 0);
        }

        public int[] GetComponents(int pixel, int[] components, int offset)
        {
            if (components == null)
                components = new int[offset + mNumComponents];

            for (int i = 0; i < mNumComponents; i++)
                components[offset + i] = (pixel & mMaskArray[i]) >> mMaskOffsets[i];

            return components;
        }

        private int GetsRgbComponentFromLinearRgb(int pixel, int idx)
        {
            int c = ((pixel & mMaskArray[idx]) >> mMaskOffsets[idx]);
            if (mIsAlphaPremultiplied)
            {
                float factor = (float)((1 << mLRgbPrecision) - 1);
                int a = ((pixel & mMaskArray[3]) >> mMaskOffsets[3]);
                c = (a == 0) ? 0 : (int)(((c * mScaleFactors[idx]) * factor / (a * mScaleFactors[3])) + 0.5f);
            }
            else if (mNBits[idx] != mLRgbPrecision)
            {
                if (mLRgbPrecision == 16)
                    c = (int)((c * mScaleFactors[idx] * 257.0f) + 0.5f);

                else
                    c = (int)((c * mScaleFactors[idx]) + 0.5f);
            }
            // now range of c is 0-255 or 0-65535, depending on lRGBprecision
            return mToSRgb8LUT[c] & 0xff;
        }

        private int GetsRgbComponentFromSRgb(int pixel, int idx)
        {
            int c = ((pixel & mMaskArray[idx]) >> mMaskOffsets[idx]);
            if (mIsAlphaPremultiplied)
            {
                int a = ((pixel & mMaskArray[3]) >> mMaskOffsets[3]);
                c = (a == 0) ? 0 : (int)(((c * mScaleFactors[idx]) * 255.0f / (a * mScaleFactors[3])) + 0.5f);
            }
            else if (mScaleFactors[idx] != 1.0f)
                c = (int)((c * mScaleFactors[idx]) + 0.5f);

            return c;
        }
        
        private float[] GetDefaultRgbComponents(int pixel)
        {
            int[] components = GetComponents(pixel, null, 0);
            float[] norm = GetNormalizedComponents(components, 0, null, 0);
            // Note that getNormalizedComponents returns non-premultiplied values
            return mColorSpace.ToRgb(norm);
        }

        private void SetFields()
        {
            // Set the private fields
            // REMIND: Get rid of these from the native code
            mRedMask = mMaskArray[0];
            mRedOffset = mMaskOffsets[0];
            mGreenMask = mMaskArray[1];
            mGreenOffset = mMaskOffsets[1];
            mBlueMask = mMaskArray[2];
            mBlueOffset = mMaskOffsets[2];
            if (mNBits[0] < 8)
                mRedScale = (1 << mNBits[0]) - 1;

            if (mNBits[1] < 8)
                mGreenScale = (1 << mNBits[1]) - 1;

            if (mNBits[2] < 8)
                mBlueScale = (1 << mNBits[2]) - 1;

            if (mSupportsAlpha)
            {
                mAlphaMask = mMaskArray[3];
                mAlphaOffset = mMaskOffsets[3];
                if (mNBits[3] < 8)
                    mAlphaScale = (1 << mNBits[3]) - 1;
            }
        }

        private int mRedMask;
        private int mGreenMask;
        private int mBlueMask;
        private int mAlphaMask;
        private int mRedOffset;
        private int mGreenOffset;
        private int mBlueOffset;
        private int mAlphaOffset;
        private int mRedScale;
        private int mGreenScale;
        private int mBlueScale;
        private int mAlphaScale;
        private readonly bool mIsLinearRgb;
        private readonly int mLRgbPrecision;
        private readonly byte[] mToSRgb8LUT;
        private readonly byte[] mFromSRgb8LUT8;
        private readonly short[] mFromsRgb8LUT16;
    }
}
#endif
