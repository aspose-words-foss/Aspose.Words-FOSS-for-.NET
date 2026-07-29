// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD

using System;

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal class AwtSimpleCmykColorSpace: AwtColorSpace
    {
        private AwtSimpleCmykColorSpace() : base(AwtColorSpaceType.Cmyk, 4)
        {
            mRgbColorSpace = GetInstance(AwtColorSpaces.LinearRgb);
        }

        public override bool Equals(object o)
        {
            return o != null && o is AwtSimpleCmykColorSpace;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override float[] ToRgb(float[] colorvalue)
        {
            float C = colorvalue[0];
            float M = colorvalue[1];
            float Y = colorvalue[2];
            float K = colorvalue[3];

            float K1 = 1.0F - K;

            // Convert from CMYK to linear RGB.
            float[] rgbvalue = new float[]{K1 * (1.0F - C),
                K1 * (1.0F - M),
                K1 * (1.0F - Y)};

            // Convert from linear RGB to sRGB.
            for (int i = 0; i < 3; i++)
            {
                float v = rgbvalue[i];

                if (v < 0.0F) v = 0.0F;

                if (v < 0.0031308F)
                {
                    rgbvalue[i] = 12.92F * v;
                }
                else
                {
                    if (v > 1.0F) v = 1.0F;

                    rgbvalue[i] = (float)(1.055 * Math.Pow(v, gPower1) - 0.055);
                }
            }

            return rgbvalue;
        }

        public float[] FromRgb(float[] rgbvalue)
        {
            // Convert from sRGB to linear RGB.
            for (int i = 0; i < 3; i++)
            {
                if (rgbvalue[i] < 0.040449936F)
                {
                    rgbvalue[i] /= 12.92F;
                }
                else
                {
                    rgbvalue[i] = (float)(Math.Pow((rgbvalue[i] + 0.055) / 1.055, 2.4));
                }
            }

            // Convert from linear RGB to CMYK.
            float c = 1.0F - rgbvalue[0];
            float m = 1.0F - rgbvalue[1];
            float y = 1.0F - rgbvalue[2];
            float k = Math.Min(c, Math.Min(m, y));

            // If K == 1.0F, then C = M = Y = 1.0F.
            if (k != 1.0F)
            {
                float K1 = 1.0F - k;

                c = (c - k) / K1;
                m = (m - k) / K1;
                y = (y - k) / K1;
            }
            else
            {
                c = m = y = 0.0F;
            }

            return new float[] { c, m, y, k };
        }

        public static AwtColorSpace Instance
        {
            get
            {
                lock (gLocker)
                {
                    if (gInstance == null)
                        gInstance = new AwtSimpleCmykColorSpace();
                }
                return gInstance;
            }
        }

        private readonly AwtColorSpace mRgbColorSpace;

        /// <summary>
        /// The exponent for gamma correction.
        /// </summary>
        private static readonly double gPower1 = 1.0 / 2.4;
        private static AwtColorSpace gInstance = null;
        private static readonly object gLocker = new object();
    }
}
#endif
