// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD

using System;

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal class AwtComponentColorModel : AwtColorModel
    {

        public AwtComponentColorModel(AwtColorSpace colorSpace, bool hasAlpha, bool isAlphaPremultiplied, AwtTransparency transparency, AwtDataBufferType transferType) :
            this(colorSpace, null, hasAlpha, isAlphaPremultiplied, transparency, transferType)
        {
        }

        public AwtComponentColorModel(AwtColorSpace colorSpace, int[] bits, bool hasAlpha, bool isAlphaPremultiplied, AwtTransparency transparency, AwtDataBufferType transferType) :
            base(BitsHelper(transferType, colorSpace.NumComponents, hasAlpha),
                    BitsArrayHelper(bits, transferType, colorSpace.NumComponents, hasAlpha),
                    colorSpace,
                    hasAlpha, isAlphaPremultiplied, transparency, transferType)
        {
            switch (transferType)
            {
                case AwtDataBufferType.Byte:
                case AwtDataBufferType.Ushort:
                case AwtDataBufferType.Int:
                    mSigned = false;
                    mNeedScaleInit = true;
                    break;
                case AwtDataBufferType.Short:
                    mSigned = true;
                    mNeedScaleInit = true;
                    break;
                case AwtDataBufferType.Float:
                case AwtDataBufferType.Double:
                    mSigned = true;
                    mNeedScaleInit = false;
                    mNoUnnorm = true;
                    mNonStdScale = false;
                    break;
                default:
                    throw new ArgumentException("This constructor is not compatible with transferType " + transferType);
            }
            SetupLUTs();
        }

        public override int GetRed(int pixel) { throw new NotImplementedException(); }

        public override int GetGreen(int pixel) { throw new NotImplementedException(); }

        public override int GetBlue(int pixel) { throw new NotImplementedException(); }

        public override int GetAlpha(int pixel) { throw new NotImplementedException(); }

        public override int GetRed(object inData) { return GetRgbComponent(inData, 0); }

        public override int GetGreen(object inData) { return GetRgbComponent(inData, 1); }

        public override int GetBlue(object inData) { return GetRgbComponent(inData, 2); }

        public override int GetAlpha(object inData)
        {
            if (!mSupportsAlpha)
                return 255;

            int alpha = 0;
            int aIdx = mNumColorComponents;
            int mask = (1 << mNBits[aIdx]) - 1;

            switch (mTransferType)
            {
                case AwtDataBufferType.Short:
                    short[] sdata = (short[])inData;
                    alpha = (int)((sdata[aIdx] / 32767.0f) * 255.0f + 0.5f);
                    return alpha;
                case AwtDataBufferType.Float:
                    float[] fdata = (float[])inData;
                    alpha = (int)(fdata[aIdx] * 255.0f + 0.5f);
                    return alpha;
                case AwtDataBufferType.Double:
                    double[] ddata = (double[])inData;
                    alpha = (int)(ddata[aIdx] * 255.0 + 0.5);
                    return alpha;
                case AwtDataBufferType.Byte:
                    byte[] bdata = (byte[])inData;
                    alpha = bdata[aIdx] & mask;
                    break;
                case AwtDataBufferType.Ushort:
                    short[] usdata = (short[])inData;
                    alpha = usdata[aIdx] & mask;
                    break;
                case AwtDataBufferType.Int:
                    int[] idata = (int[])inData;
                    alpha = idata[aIdx];
                    break;
                default:
                    throw new NotImplementedException("This method has not been implemented for transferType " + mTransferType);
            }

            if (mNBits[aIdx] == 8)
                return alpha;

            else
                return (int)((((float)alpha) / ((float)((1 << mNBits[aIdx]) - 1))) * 255.0f + 0.5f);
        }

        public override int GetRgb(object inData)
        {
            if (mNeedScaleInit)
                InitScale();

            if (mIsSRgbStdScale || mIsLinearRgbStdScale)
                return (GetAlpha(inData) << 24) | (GetRed(inData) << 16) | (GetGreen(inData) << 8) | (GetBlue(inData));

            else if (mColorSpaceType == AwtColorSpaceType.Gray)
            {
                int gray = GetRed(inData); // Red sRGB component should equal
                                           // green and blue components
                return (GetAlpha(inData) << 24) | (gray << 16) | (gray << 8) | gray;
            }
            float[] norm = GetNormalizedComponents(inData, null, 0);
            // Note that getNormalizedComponents returns non-premult values
            float[] rgb = mColorSpace.ToRgb(norm);
            return (GetAlpha(inData) << 24) | (((int)(rgb[0] * 255.0f + 0.5f)) << 16) | (((int)(rgb[1] * 255.0f + 0.5f)) << 8) | (((int)(rgb[2] * 255.0f + 0.5f)) << 0);
        }

        private int GetRgbComponent(object inData, int idx)
        {
            if (mNeedScaleInit)
                InitScale();

            if (mIsSRgbStdScale)
                return ExtractComponent(inData, idx, 8);

            else if (mIsLinearRgbStdScale)
            {
                int lutidx = ExtractComponent(inData, idx, 16);
                return mToSRgb8LUT[lutidx] & 0xff;
            }
            else if (mIsIccGrayStdScale)
            {
                int lutidx = ExtractComponent(inData, 0, 16);
                return mToSRgb8LUT[lutidx] & 0xff;
            }

            // Not CS_sRGB, CS_LINEAR_RGB, or any TYPE_GRAY IccColorSpace
            float[] norm = GetNormalizedComponents(inData, null, 0);
            // Note that getNormalizedComponents returns non-premultiplied values
            float[] rgb = mColorSpace.ToRgb(norm);
            return (int)(rgb[idx] * 255.0f + 0.5f);
        }

        private int ExtractComponent(object inData, int idx, int precision)
        {
            // Extract component idx from inData.  The precision argument
            // should be either 8 or 16.  If it's 8, this method will return
            // an 8-bit value.  If it's 16, this method will return a 16-bit
            // value for transferTypes other than TYPE_BYTE.  For TYPE_BYTE,
            // an 8-bit value will be returned.

            // This method maps the input value corresponding to a
            // normalized ColorSpace component value of 0.0 to 0, and the
            // input value corresponding to a normalized ColorSpace
            // component value of 1.0 to 2^n - 1 (where n is 8 or 16), so
            // it is appropriate only for ColorSpaces with min/max component
            // values of 0.0/1.0.  This will be true for sRGB, the built-in
            // Linear RGB and Linear Gray spaces, and any other ICC grayscale
            // spaces for which we have precomputed LUTs.

            bool needAlpha = (mSupportsAlpha && mIsAlphaPremultiplied);
            int alp = 0;
            int comp;
            int mask = (1 << mNBits[idx]) - 1;

            switch (mTransferType)
            {
                // Note: we do no clamping of the pixel data here - we
                // assume that the data is scaled properly
                case AwtDataBufferType.Short:
                    {
                        short[] sdata = (short[])inData;
                        float scalefactor = (float)((1 << precision) - 1);
                        if (needAlpha)
                        {
                            short s = sdata[mNumColorComponents];
                            if (s != (short)0)
                                return (int)((((float)sdata[idx]) / ((float)s)) * scalefactor + 0.5f);

                            else
                                return 0;
                        }
                        else
                            return (int)((sdata[idx] / 32767.0f) * scalefactor + 0.5f);
                    }
                case AwtDataBufferType.Float:
                    {
                        float[] fdata = (float[])inData;
                        float scalefactor = (float)((1 << precision) - 1);
                        if (needAlpha)
                        {
                            float f = fdata[mNumColorComponents];
                            if (f != 0.0f)
                                return (int)(((fdata[idx] / f) * scalefactor) + 0.5f);
                            else
                                return 0;
                        }
                        else
                            return (int)(fdata[idx] * scalefactor + 0.5f);
                    }
                case AwtDataBufferType.Double:
                    {
                        double[] ddata = (double[])inData;
                        double scalefactor = (double)((1 << precision) - 1);
                        if (needAlpha)
                        {
                            double d = ddata[mNumColorComponents];
                            if (d != 0.0)
                                return (int)(((ddata[idx] / d) * scalefactor) + 0.5);
                            else
                                return 0;
                        }
                        else
                            return (int)(ddata[idx] * scalefactor + 0.5);
                    }
                case AwtDataBufferType.Byte:
                    byte[] bdata = (byte[])inData;
                    comp = bdata[idx] & mask;
                    precision = 8;
                    if (needAlpha)
                        alp = bdata[mNumColorComponents] & mask;
                    break;
                case AwtDataBufferType.Ushort:
                    short[] usdata = (short[])inData;
                    comp = usdata[idx] & mask;
                    if (needAlpha)
                        alp = usdata[mNumColorComponents] & mask;
                    break;
                case AwtDataBufferType.Int:
                    int[] idata = (int[])inData;
                    comp = idata[idx];
                    if (needAlpha)
                        alp = idata[mNumColorComponents];
                    break;
                default:
                    throw new
                            NotImplementedException("This method has not " +
                            "been implemented for transferType " + mTransferType);
            }
            if (needAlpha)
            {
                if (alp != 0)
                {
                    float scalefactor = (float)((1 << precision) - 1);
                    float fcomp = ((float)comp) / ((float)mask);
                    float invalp = ((float)((1 << mNBits[mNumColorComponents]) - 1)) / ((float)alp);
                    return (int)(fcomp * invalp * scalefactor + 0.5f);
                }
                else
                    return 0;
            }
            else
            {
                if (mNBits[idx] != precision)
                {
                    float scalefactor = (float)((1 << precision) - 1);
                    float fcomp = ((float)comp) / ((float)mask);
                    return (int)(fcomp * scalefactor + 0.5f);
                }
                return comp;
            }
        }

        private void InitScale()
        {
            mNeedScaleInit = false; // only needs to called once
            if (mNonStdScale || mSigned)
                mNoUnnorm = true;
            else
                mNoUnnorm = false;

            float[] lowVal;
            float[] highVal;
            switch (mTransferType)
            {
                case AwtDataBufferType.Byte:
                    {
                        byte[] bpixel = new byte[mNumComponents];
                        for (int i = 0; i < mNumColorComponents; i++)
                            bpixel[i] = 0;

                        if (mSupportsAlpha)
                            bpixel[mNumColorComponents] = (byte)((1 << mNBits[mNumColorComponents]) - 1);

                        lowVal = GetNormalizedComponents(bpixel, null, 0);
                        for (int i = 0; i < mNumColorComponents; i++)
                            bpixel[i] = (byte)((1 << mNBits[i]) - 1);

                        highVal = GetNormalizedComponents(bpixel, null, 0);
                    }
                    break;
                case AwtDataBufferType.Ushort:
                    {
                        short[] uspixel = new short[mNumComponents];
                        for (int i = 0; i < mNumColorComponents; i++)
                            uspixel[i] = 0;

                        if (mSupportsAlpha)
                            uspixel[mNumColorComponents] = (short)((1 << mNBits[mNumColorComponents]) - 1);

                        lowVal = GetNormalizedComponents(uspixel, null, 0);
                        for (int i = 0; i < mNumColorComponents; i++)
                            uspixel[i] = (short)((1 << mNBits[i]) - 1);

                        highVal = GetNormalizedComponents(uspixel, null, 0);
                    }
                    break;
                case AwtDataBufferType.Int:
                    {
                        int[] ipixel = new int[mNumComponents];
                        for (int i = 0; i < mNumColorComponents; i++)
                            ipixel[i] = 0;

                        if (mSupportsAlpha)
                            ipixel[mNumColorComponents] = ((1 << mNBits[mNumColorComponents]) - 1);

                        lowVal = GetNormalizedComponents(ipixel, null, 0);
                        for (int i = 0; i < mNumColorComponents; i++)
                            ipixel[i] = ((1 << mNBits[i]) - 1);

                        highVal = GetNormalizedComponents(ipixel, null, 0);
                    }
                    break;
                case AwtDataBufferType.Short:
                    {
                        short[] spixel = new short[mNumComponents];
                        for (int i = 0; i < mNumColorComponents; i++)
                            spixel[i] = 0;

                        if (mSupportsAlpha)
                            spixel[mNumColorComponents] = 32767;

                        lowVal = GetNormalizedComponents(spixel, null, 0);
                        for (int i = 0; i < mNumColorComponents; i++)
                            spixel[i] = 32767;

                        highVal = GetNormalizedComponents(spixel, null, 0);
                    }
                    break;
                default:
                    throw new ArgumentException("Invalid AwtDataBufferType.");
            }
            mNonStdScale = false;
            for (int i = 0; i < mNumColorComponents; i++)
            {
                if ((lowVal[i] != 0.0f) || (highVal[i] != 1.0f))
                {
                    mNonStdScale = true;
                    break;
                }
            }
            if (mNonStdScale)
            {
                mNoUnnorm = true;
                mIsSRgbStdScale = false;
                mIsLinearRgbStdScale = false;
                mIsLinearGrayStdScale = false;
                mIsIccGrayStdScale = false;
                mCompOffset = new float[mNumColorComponents];
                mCompScale = new float[mNumColorComponents];
                for (int i = 0; i < mNumColorComponents; i++)
                {
                    mCompOffset[i] = lowVal[i];
                    mCompScale[i] = 1.0f / (highVal[i] - lowVal[i]);
                }
            }
        }

        private float[] GetNormalizedComponents(object pixel, float[] normComponents, int normOffset)
        {
            if (normComponents == null)
                normComponents = new float[mNumComponents + normOffset];

            switch (mTransferType)
            {
                case AwtDataBufferType.Byte:
                    byte[] bpixel = (byte[])pixel;
                    for (int c = 0, nc = normOffset; c < mNumComponents; c++, nc++)
                        normComponents[nc] = ((float)(bpixel[c] & 0xff)) / ((float)((1 << mNBits[c]) - 1));

                    break;
                case AwtDataBufferType.Ushort:
                    short[] uspixel = (short[])pixel;
                    for (int c = 0, nc = normOffset; c < mNumComponents; c++, nc++)
                        normComponents[nc] = ((float)(uspixel[c] & 0xffff)) / ((float)((1 << mNBits[c]) - 1));

                    break;
                case AwtDataBufferType.Int:
                    int[] ipixel = (int[])pixel;
                    for (int c = 0, nc = normOffset; c < mNumComponents; c++, nc++)
                        normComponents[nc] = ((float)ipixel[c]) / ((float)((1 << mNBits[c]) - 1));

                    break;
                case AwtDataBufferType.Short:
                    short[] spixel = (short[])pixel;
                    for (int c = 0, nc = normOffset; c < mNumComponents; c++, nc++)
                        normComponents[nc] = ((float)spixel[c]) / 32767.0f;

                    break;
                case AwtDataBufferType.Float:
                    float[] fpixel = (float[])pixel;
                    for (int c = 0, nc = normOffset; c < mNumComponents; c++, nc++)
                        normComponents[nc] = fpixel[c];

                    break;
                case AwtDataBufferType.Double:
                    double[] dpixel = (double[])pixel;
                    for (int c = 0, nc = normOffset; c < mNumComponents; c++, nc++)
                        normComponents[nc] = (float)dpixel[c];

                    break;
                default:
                    throw new NotImplementedException("This method has not been implemented for transferType " + mTransferType);
            }

            if (mSupportsAlpha && mIsAlphaPremultiplied)
            {
                float alpha = normComponents[mNumColorComponents + normOffset];
                if (alpha != 0.0f)
                {
                    float invAlpha = 1.0f / alpha;
                    for (int c = normOffset; c < mNumColorComponents + normOffset; c++)
                        normComponents[c] *= invAlpha;
                }
            }

            if (mMin != null)
            {
                for (int c = 0; c < mNumColorComponents; c++)
                    normComponents[c + normOffset] = mMin[c] + mDiffMinMax[c] * normComponents[c + normOffset];

            }
            return normComponents;
        }
                
        private void SetupLUTs()
        {
            if (mIsSrgb)
            {
                mIsSRgbStdScale = true;
                mNonStdScale = false;
            }
            else if (AwtColorModel.IsLinearRgbSpace(mColorSpace))
            {
                // Note that the built-in Linear RGB space has a normalized
                // range of 0.0 - 1.0 for each coordinate.  Usage of these
                // LUTs makes that assumption.
                mIsLinearRgbStdScale = true;
                mNonStdScale = false;
                if (mTransferType == AwtDataBufferType.Byte)
                {
                    mToSRgb8LUT = AwtColorModel.GetLinearRgb8TosRgb8LUT();
                    mFromsRgb8LUT8 = AwtColorModel.GetsRgb8ToLinearRgb8LUT();
                }
                else
                {
                    mToSRgb8LUT = AwtColorModel.GetLinearRgb16TosRgb8LUT();
                    mFromsRgb8LUT16 = AwtColorModel.GetsRgb8ToLinearRgb16LUT();
                }
            }
            else if ((mColorSpaceType == AwtColorSpaceType.Gray) && (mColorSpace is AwtIccColorSpace) && (mColorSpace.GetMinValue(0) == 0.0f) && (mColorSpace.GetMaxValue(0) == 1.0f))
            {
                // Note that a normalized range of 0.0 - 1.0 for the gray
                // component is required, because usage of these LUTs makes
                // that assumption.
                AwtIccColorSpace ics = (AwtIccColorSpace)mColorSpace;
                mIsIccGrayStdScale = true;
                mNonStdScale = false;
                mFromsRgb8LUT16 = AwtColorModel.GetsRgb8ToLinearRgb16LUT();
                if (AwtColorModel.IsLinearGraySpace(ics))
                {
                    mIsLinearGrayStdScale = true;
                    if (mTransferType == AwtDataBufferType.Byte)
                        mToSRgb8LUT = AwtColorModel.GetGray8TosRgb8LUT(ics);
                    else
                        mToSRgb8LUT = AwtColorModel.GetGray16TosRgb8LUT(ics);
                }
                else
                {
                    if (mTransferType == AwtDataBufferType.Byte)
                    {
                        mToSRgb8LUT = AwtColorModel.GetGray8TosRgb8LUT(ics);
                        mFromLinearGray16ToOtherGray8LUT = AwtColorModel.GetLinearGray16ToOtherGray8LUT(ics);
                    }
                    else
                    {
                        mToSRgb8LUT = AwtColorModel.GetGray16TosRgb8LUT(ics);
                        mFromLinearGray16ToOtherGray16LUT = AwtColorModel.GetLinearGray16ToOtherGray16LUT(ics);
                    }
                }
            }
            else if (mNeedScaleInit)
            {
                // if transferType is byte, ushort, int, or short and we
                // don't already know the ColorSpace has minVlaue == 0.0f and
                // maxValue == 1.0f for all components, we need to check that
                // now and setup the min[] and diffMinMax[] arrays if necessary.
                mNonStdScale = false;
                for (int i = 0; i < mNumColorComponents; i++)
                {
                    if ((mColorSpace.GetMinValue(i) != 0.0f) ||
                            (mColorSpace.GetMaxValue(i) != 1.0f))
                    {
                        mNonStdScale = true;
                        break;
                    }
                }
                if (mNonStdScale)
                {
                    mMin = new float[mNumColorComponents];
                    mDiffMinMax = new float[mNumColorComponents];
                    for (int i = 0; i < mNumColorComponents; i++)
                    {
                        mMin[i] = mColorSpace.GetMinValue(i);
                        mDiffMinMax[i] = mColorSpace.GetMaxValue(i) - mMin[i];
                    }
                }
            }
        }

        private static int BitsHelper(AwtDataBufferType transferType, int numComponents, bool hasAlpha)
        {
            int numBits = AwtDataBuffer.GetDataTypeSize(transferType);
            if (hasAlpha)
                ++numComponents;

            return numBits * numComponents;
        }

        private static int[] BitsArrayHelper(int[] origBits, AwtDataBufferType transferType, int numComponents, bool hasAlpha)
        {
            switch (transferType)
            {
                case AwtDataBufferType.Byte:
                case AwtDataBufferType.Ushort:
                case AwtDataBufferType.Int:
                    if (origBits != null)
                        return origBits;

                    break;
                default:
                    break;
            }
            int numBits = AwtDataBuffer.GetDataTypeSize(transferType);
            if (hasAlpha)
                ++numComponents;

            int[] bits = new int[numComponents];
            for (int i = 0; i < numComponents; i++)
                bits[i] = numBits;

            return bits;
        }

        public override AwtSampleModel CreateCompatibleSampleModel(int w, int h)
        {
            int[] bandOffsets = new int[mNumComponents];
            for (int i = 0; i < mNumComponents; i++)
                bandOffsets[i] = i;

            switch (mTransferType)
            {
                case AwtDataBufferType.Byte:
                case AwtDataBufferType.Ushort:
                    return new AwtPixelInterleavedSampleModel(mTransferType, w, h, mNumComponents, w * mNumComponents, bandOffsets);
                default:
                    return new AwtComponentSampleModel(mTransferType, w, h, mNumComponents, w * mNumComponents, bandOffsets);
            }
        }

        private bool NoUnnorm
        {
            get { return mNoUnnorm; }
        }

        private bool IsLinearGrayStdScale
        {
            get { return mIsLinearGrayStdScale; }
        }

        private readonly bool mSigned;
        private bool mNeedScaleInit;
        private bool mNonStdScale;
        private bool mNoUnnorm;
        private bool mIsSRgbStdScale;
        private bool mIsLinearRgbStdScale;
        private bool mIsLinearGrayStdScale;
        private bool mIsIccGrayStdScale;
        private float[] mMin;
        private float[] mDiffMinMax;
        private float[] mCompOffset;
        private float[] mCompScale;

        private byte[] mToSRgb8LUT;
        private byte[] mFromsRgb8LUT8;
        private short[] mFromsRgb8LUT16;
        private byte[] mFromLinearGray16ToOtherGray8LUT;
        private short[] mFromLinearGray16ToOtherGray16LUT;
    }
}
#endif
