// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD

using System;

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal abstract class AwtColorSpace
    {
        protected AwtColorSpace(AwtColorSpaceType type, int numcomponents)
        {
            mType = type;
            mNumComponents = numcomponents;
        }

        public abstract float[] ToRgb(float[] colorvalue);

        public float GetMinValue(int component)
        {
            if ((component < 0) || (component > mNumComponents - 1))
                throw new ArgumentException("Component index out of range: " + component);

            return 0.0f;
        }

        public float GetMaxValue(int component)
        {
            if ((component < 0) || (component > mNumComponents - 1))
                throw new ArgumentException("Component index out of range: " + component);

            return 1.0f;
        }

        public static AwtColorSpace GetInstance(AwtColorSpaces colorspace)
        {
            AwtColorSpace theColorSpace;

            switch (colorspace)
            {
                case AwtColorSpaces.SRgb:

                    lock (Locker)
                    {
                        if (gSRgbSpace == null)
                            gSRgbSpace = new AwtIccColorSpace(AwtColorSpaceType.Rgb, 3);

                        theColorSpace = gSRgbSpace;
                    }

                    break;
                case AwtColorSpaces.Gray:

                    lock (Locker)
                    {
                        if (gGraySpace == null)
                            gGraySpace = new AwtIccColorSpace(AwtColorSpaceType.Gray, 1);

                        theColorSpace = gGraySpace;
                    }

                    break;
                case AwtColorSpaces.LinearRgb:

                    lock (Locker)
                    {
                        if (gLinearRgbSpace == null)
                            gLinearRgbSpace = new AwtIccColorSpace(AwtColorSpaceType.Rgb, 3, true);

                        theColorSpace = gLinearRgbSpace;
                    }

                    break;

                default:
                    throw new ArgumentException("Unknown color space");
            }
            return theColorSpace;
        }

        public AwtColorSpaceType Type
        {
            get { return mType; }
        }

        public int NumComponents
        {
            get { return mNumComponents; }
        }

        private readonly AwtColorSpaceType mType;
        private readonly int mNumComponents;

        private static AwtColorSpace gSRgbSpace;
        private static AwtColorSpace gGraySpace;
        private static AwtColorSpace gLinearRgbSpace;
        private static object Locker = new object();
    }

    internal enum AwtColorSpaces
    {
        SRgb = 1000,
        LinearRgb = 1004,
        Gray = 1003
    }

    internal enum AwtColorSpaceType
    {
        YCbCr = 3,
        Rgb = 5,
        Gray = 6,
        Cmyk = 9
    }
}
#endif
