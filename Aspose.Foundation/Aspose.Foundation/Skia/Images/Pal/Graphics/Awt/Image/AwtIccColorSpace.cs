// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD

using System;

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal class AwtIccColorSpace : AwtColorSpace
    {
        public AwtIccColorSpace(AwtColorSpaceType type, int numcomponents) : this(type, numcomponents, false)
        {
        }

        public AwtIccColorSpace(AwtColorSpaceType type, int numcomponents, bool isLinearRGB) : base(type, numcomponents)
        {
            mMinVal = new float[numcomponents];
            mMaxVal = new float[numcomponents];

            for (int i = 0; i < numcomponents; i++)
            {
                mMinVal[i] = 0.0f;
                mMaxVal[i] = 1.0f;
            }

            mNeedScaleInit = (type == AwtColorSpaceType.Rgb) || (type == AwtColorSpaceType.Gray);
            mLinearRgb = isLinearRGB;
        }

        public override float[] ToRgb(float[] colorvalue)
        {
            throw new NotImplementedException();
        }

        private readonly bool mLinearRgb;
        private readonly float[] mMinVal;
        private readonly float[] mMaxVal;
        private readonly bool mNeedScaleInit;
    }
}
#endif
