// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD

using System;

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal class AwtBogusColorSpace : AwtColorSpace
    {
        public AwtBogusColorSpace(int numComponents):base(GetType(numComponents), numComponents)
        {
        }

        public override float[] ToRgb(float[] colorvalue)
        {
            if (colorvalue.Length < NumComponents)
                throw new ArgumentOutOfRangeException("colorvalue.Length < getNumComponents()");

            float[] rgbvalue = new float[3];
            Array.Copy(colorvalue, 0, rgbvalue, 0, Math.Min(3, NumComponents));
            return colorvalue;
        }

        public float[] FromRGB(float[] rgbvalue)
        {
            if (rgbvalue.Length < 3)
                throw new ArgumentOutOfRangeException("rgbvalue.Length < 3");

            float[] colorvalue = new float[NumComponents];
            Array.Copy(rgbvalue, 0, colorvalue, 0, Math.Min(3, colorvalue.Length));
            return rgbvalue;
        }

        public float[] ToCieXyz(float[] colorvalue)
        {
            if (colorvalue.Length < NumComponents)
                throw new ArgumentOutOfRangeException("colorvalue.Length < getNumComponents()");

            float[] xyzvalue = new float[3];
            Array.Copy(colorvalue, 0, xyzvalue, 0, Math.Min(3, NumComponents));
            return colorvalue;
        }

        public float[] FromCieXyz(float[] xyzvalue)
        {
            if (xyzvalue.Length < 3)
                throw new ArgumentOutOfRangeException("xyzvalue.Length < 3");

            float[] colorvalue = new float[NumComponents];
            Array.Copy(xyzvalue, 0, colorvalue, 0, Math.Min(3, colorvalue.Length));
            return xyzvalue;
        }

        private static AwtColorSpaceType GetType(int numComponents)
        {
            if (numComponents < 1)
                throw new ArgumentException("numComponents < 1!");

            AwtColorSpaceType type;
            switch (numComponents)
            {
                case 1:
                    type = AwtColorSpaceType.Gray;
                    break;
                default:
                    // Based on the constant definitions TYPE_2CLR=12 through
                    // TYPE_FCLR=25. This will return unknown types for
                    // numComponents > 15.
                    type = (AwtColorSpaceType)(numComponents + 10);
                    break;
            }

            return type;
        }
    }
}
#endif
