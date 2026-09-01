// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD || NET

using System;

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal abstract class AwtColorModel
    {
        protected AwtColorModel(int pixel_bits, int[] bits, AwtColorSpace cspace, bool hasAlpha, bool isAlphaPremultiplied, AwtTransparency transparency, AwtDataBufferType transferType)
        {
            mColorSpace = cspace;
            mColorSpaceType = cspace.Type;
            mNumColorComponents = cspace.NumComponents;
            mNumComponents = mNumColorComponents + (hasAlpha ? 1 : 0);
            mSupportsAlpha = hasAlpha;

            if (bits.Length < mNumComponents)
                throw new ArgumentException("Number of color/alpha components should be " + mNumComponents + " but Length of bits array is " + bits.Length);

            // 4186669
            if (transparency < AwtTransparency.Opaque || transparency > AwtTransparency.Translucent)
                throw new ArgumentException("Unknown transparency: " + transparency);

            if (!mSupportsAlpha)
            {
                this.mIsAlphaPremultiplied = false;
                this.mTransparency = AwtTransparency.Opaque;
            }
            else
            {
                this.mIsAlphaPremultiplied = isAlphaPremultiplied;
                this.mTransparency = transparency;
            }

            mNBits = (int[])bits.Clone();
            this.mPixelBits = pixel_bits;
            if (pixel_bits <= 0)
                throw new ArgumentException("Number of pixel bits must be > 0");

            // Check for bits < 0
            mMaxBits = 0;
            for (int i = 0; i < bits.Length; i++)
            {
                // bug 4304697
                if (bits[i] < 0)
                    throw new ArgumentException("Number of bits must be >= 0");

                if (mMaxBits < bits[i])
                    mMaxBits = bits[i];
            }

            // Make sure that we don't have all 0-bit components
            if (mMaxBits == 0)
                throw new ArgumentException("There must be at least one component with > 0 pixel bits.");

            // Save this since we always need to check if it is the default CS
            if (cspace.Type != AwtColorSpaceType.Rgb)
                mIsSrgb = false;

            this.mTransferType = transferType;

        }

        public abstract int GetRed(int pixel);

        public abstract int GetGreen(int pixel);

        public abstract int GetBlue(int pixel);

        public abstract int GetAlpha(int pixel);

        public virtual int GetAlpha(object inData)
        {
            int pixel = 0, Length = 0;
            switch (mTransferType)
            {
                case AwtDataBufferType.Byte:
                    byte[] bdata = (byte[])inData;
                    pixel = bdata[0] & 0xff;
                    Length = bdata.Length;
                    break;
                case AwtDataBufferType.Ushort:
                    short[] sdata = (short[])inData;
                    pixel = sdata[0] & 0xffff;
                    Length = sdata.Length;
                    break;
                case AwtDataBufferType.Int:
                    int[] idata = (int[])inData;
                    pixel = idata[0];
                    Length = idata.Length;
                    break;
                default:
                    throw new NotImplementedException("This method has not been implemented for transferType " + mTransferType);
            }
            if (Length == 1)
                return GetAlpha(pixel);

            else
                throw new NotImplementedException("This method is not supported by this color model");
        }

        public virtual int GetRed(object inData)
        {
            int pixel = 0, Length = 0;
            switch (mTransferType)
            {
                case AwtDataBufferType.Byte:
                    byte[] bdata = (byte[])inData;
                    pixel = bdata[0] & 0xff;
                    Length = bdata.Length;
                    break;
                case AwtDataBufferType.Ushort:
                    short[] sdata = (short[])inData;
                    pixel = sdata[0] & 0xffff;
                    Length = sdata.Length;
                    break;
                case AwtDataBufferType.Int:
                    int[] idata = (int[])inData;
                    pixel = idata[0];
                    Length = idata.Length;
                    break;
                default:
                    throw new NotImplementedException("This method has not been implemented for transferType " + mTransferType);
            }
            if (Length == 1)
                return GetRed(pixel);

            else
                throw new NotImplementedException("This method is not supported by this color model");
        }

        public virtual int GetGreen(object inData)
        {
            int pixel = 0, Length = 0;
            switch (mTransferType)
            {
                case AwtDataBufferType.Byte:
                    byte[] bdata = (byte[])inData;
                    pixel = bdata[0] & 0xff;
                    Length = bdata.Length;
                    break;
                case AwtDataBufferType.Ushort:
                    short[] sdata = (short[])inData;
                    pixel = sdata[0] & 0xffff;
                    Length = sdata.Length;
                    break;
                case AwtDataBufferType.Int:
                    int[] idata = (int[])inData;
                    pixel = idata[0];
                    Length = idata.Length;
                    break;
                default:
                    throw new NotImplementedException("This method has not been implemented for transferType " + mTransferType);
            }
            if (Length == 1)
                return GetGreen(pixel);

            else
                throw new NotImplementedException("This method is not supported by this color model");
        }

        public virtual int GetBlue(object inData)
        {
            int pixel = 0, Length = 0;
            switch (mTransferType)
            {
                case AwtDataBufferType.Byte:
                    byte[] bdata = (byte[])inData;
                    pixel = bdata[0] & 0xff;
                    Length = bdata.Length;
                    break;
                case AwtDataBufferType.Ushort:
                    short[] sdata = (short[])inData;
                    pixel = sdata[0] & 0xffff;
                    Length = sdata.Length;
                    break;
                case AwtDataBufferType.Int:
                    int[] idata = (int[])inData;
                    pixel = idata[0];
                    Length = idata.Length;
                    break;
                default:
                    throw new NotImplementedException("This method has not been implemented for transferType " + mTransferType);
            }
            if (Length == 1)
                return GetBlue(pixel);

            else
                throw new NotImplementedException("This method is not supported by this color model");
        }

        public virtual AwtSampleModel CreateCompatibleSampleModel(int w, int h)
        {
            throw new NotImplementedException("This method is not supported by this color model");
        }

        public virtual int GetRgb(int pixel)
        {
            return (GetAlpha(pixel) << 24) | (GetRed(pixel) << 16) | (GetGreen(pixel) << 8) | (GetBlue(pixel) << 0);
        }

        public virtual int GetRgb(object inData)
        {
            return (GetAlpha(inData) << 24) | (GetRed(inData) << 16) | (GetGreen(inData) << 8) | (GetBlue(inData) << 0);
        }

        public float[] GetNormalizedComponents(int[] components, int offset, float[] normComponents, int normOffset)
        {
            // Make sure that someone isn't using a custom color model
            // that called the super(bits) constructor.
            if (mColorSpace == null)
            {
                throw new NotImplementedException("This method is not supported by this color model.");
            }

            if (mNBits == null)
            {
                throw new NotImplementedException("This method is not supported. Unable to determine #bits per component.");
            }

            if ((components.Length - offset) < mNumComponents)
            {
                throw new ArgumentException("Incorrect number of components.  Expecting " + mNumComponents);
            }

            if (normComponents == null)
            {
                normComponents = new float[mNumComponents + normOffset];
            }

            if (mSupportsAlpha && mIsAlphaPremultiplied)
            {
                // Normalized coordinates are non premultiplied
                float normAlpha = (float)components[offset + mNumColorComponents];
                normAlpha /= (float)((1 << mNBits[mNumColorComponents]) - 1);
                if (normAlpha != 0.0f)
                {
                    for (int i = 0; i < mNumColorComponents; i++)
                    {
                        normComponents[normOffset + i] = ((float)components[offset + i]) / (normAlpha * ((float)((1 << mNBits[i]) - 1)));
                    }
                }
                else
                {
                    for (int i = 0; i < mNumColorComponents; i++)
                    {
                        normComponents[normOffset + i] = 0.0f;
                    }
                }
                normComponents[normOffset + mNumColorComponents] = normAlpha;
            }
            else
            {
                for (int i = 0; i < mNumComponents; i++)
                {
                    normComponents[normOffset + i] = ((float)components[offset + i]) / ((float)((1 << mNBits[i]) - 1));
                }
            }

            return normComponents;
        }

        protected static AwtDataBufferType GetDefaultTransferType(int pixelBits)
        {
            if (pixelBits <= 8)
                return AwtDataBufferType.Byte;
            else if (pixelBits <= 16)
                return AwtDataBufferType.Ushort;

            else if (pixelBits <= 32)
                return AwtDataBufferType.Int;

            else
                return AwtDataBufferType.Undefined;
        }

        protected static bool IsLinearRgbSpace(AwtColorSpace cs)
        {
            return (cs == AwtColorSpace.GetInstance(AwtColorSpaces.LinearRgb));
        }

        protected static bool IsLinearGraySpace(AwtIccColorSpace cs)
        {
            return (cs == AwtColorSpace.GetInstance(AwtColorSpaces.Gray));
        }

        protected static short[] GetLinearGray16ToOtherGray16LUT(AwtIccColorSpace colorSpace)
        {
            throw new NotImplementedException();
        }

        protected static byte[] GetLinearGray16ToOtherGray8LUT(AwtIccColorSpace colorSpace)
        {
            throw new NotImplementedException();
        }

        protected static byte[] GetGray16TosRgb8LUT(AwtIccColorSpace grayCS)
        {
            if (IsLinearGraySpace(grayCS))
                return GetLinearRgb16TosRgb8LUT();

            throw new NotImplementedException();
        }

        protected static byte[] GetGray8TosRgb8LUT(AwtIccColorSpace grayCS)
        {
            if (IsLinearGraySpace(grayCS))
                return GetLinearRgb8TosRgb8LUT();

            throw new NotImplementedException();
        }

        protected static short[] GetsRgb8ToLinearRgb16LUT()
        {
            if (s8Tol16 == null)
            {
                s8Tol16 = new short[256];
                float input, output;
                // algorithm from IEC 61966-2-1 International Standard
                for (int i = 0; i <= 255; i++)
                {
                    input = ((float)i) / 255.0f;
                    if (input <= 0.04045f)
                    {
                        output = input / 12.92f;
                    }
                    else
                    {
                        output = (float)Math.Pow((input + 0.055f) / 1.055f, 2.4);
                    }
                    s8Tol16[i] = (short)Math.Round(output * 65535.0f);
                }
            }
            return s8Tol16;
        }

        protected static byte[] GetLinearRgb16TosRgb8LUT()
        {
            if (l16Tos8 == null)
            {
                l16Tos8 = new byte[65536];
                float input, output;
                // algorithm from IEC 61966-2-1 International Standard
                for (int i = 0; i <= 65535; i++)
                {
                    input = ((float)i) / 65535.0f;
                    if (input <= 0.0031308f)
                    {
                        output = input * 12.92f;
                    }
                    else
                    {
                        output = 1.055f * ((float)Math.Pow(input, (1.0 / 2.4)))
                                - 0.055f;
                    }
                    l16Tos8[i] = (byte)Math.Round(output * 255.0f);
                }
            }
            return l16Tos8;
        }

        protected static byte[] GetsRgb8ToLinearRgb8LUT()
        {
            if (s8Tol8 == null)
            {
                s8Tol8 = new byte[256];
                float input, output;
                // algorithm from IEC 61966-2-1 International Standard
                for (int i = 0; i <= 255; i++)
                {
                    input = ((float)i) / 255.0f;
                    if (input <= 0.04045f)
                    {
                        output = input / 12.92f;
                    }
                    else
                    {
                        output = (float)Math.Pow((input + 0.055f) / 1.055f, 2.4);
                    }
                    s8Tol8[i] = (byte)Math.Round(output * 255.0f);
                }
            }
            return s8Tol8;
        }

        protected static byte[] GetLinearRgb8TosRgb8LUT()
        {
            if (l8Tos8 == null)
            {
                l8Tos8 = new byte[256];
                float input, output;
                // algorithm for linear RGB to nonlinear sRGB conversion
                // is from the IEC 61966-2-1 International Standard,
                // Colour Management - Default RGB colour space - sRGB,
                // First Edition, 1999-10,
                // avaiable for order at http://www.iec.ch
                for (int i = 0; i <= 255; i++)
                {
                    input = ((float)i) / 255.0f;
                    if (input <= 0.0031308f)
                    {
                        output = input * 12.92f;
                    }
                    else
                    {
                        output = 1.055f * ((float)Math.Pow(input, (1.0 / 2.4))) - 0.055f;
                    }
                    l8Tos8[i] = (byte)Math.Round(output * 255.0f);
                }
            }
            return l8Tos8;
        }

        public AwtDataBufferType TransferType
        {
            get { return mTransferType; }
        }

        internal AwtTransparency Transparency
        {
            get { return mTransparency; }
        }

        public AwtColorSpace ColorSpace
        {
            get { return mColorSpace; }
        }

        public bool HasAlpha
        {
            get { return mSupportsAlpha; }
        }

        public bool IsAlphaPremultiplied
        {
            get { return mIsAlphaPremultiplied; }
        }

        public int PixelSize
        {
            get { return mPixelBits; }
        }

        public int NumComponents
        {
            get { return mNumComponents; }
        }

        public int NumColorComponents
        {
            get { return mNumColorComponents; }
        }

        protected int mPixelBits;
        protected AwtTransparency mTransparency = AwtTransparency.Translucent;
        protected AwtColorSpace mColorSpace = AwtColorSpace.GetInstance(AwtColorSpaces.SRgb);
        protected AwtColorSpaceType mColorSpaceType = AwtColorSpaceType.Rgb;
        protected int[] mNBits;
        protected bool mSupportsAlpha = true;
        protected bool mIsAlphaPremultiplied = false;
        protected int mNumComponents = -1;
        protected int mNumColorComponents = -1;
        protected int mMaxBits;
        protected bool mIsSrgb = true;
        protected AwtDataBufferType mTransferType;

        private static byte[] l8Tos8 = null;   // 8-bit linear to 8-bit non-linear sRGB LUT
        private static byte[] s8Tol8 = null;   // 8-bit non-linear sRGB to 8-bit linear LUT
        private static byte[] l16Tos8 = null;  // 16-bit linear to 8-bit non-linear sRGB LUT
        private static short[] s8Tol16 = null; // 8-bit non-linear sRGB to 16-bit linear LUT
    }
}
#endif
