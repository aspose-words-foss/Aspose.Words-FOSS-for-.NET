// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD || NET

using System;

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal class AwtPixelInterleavedSampleModel : AwtComponentSampleModel
    {
        public AwtPixelInterleavedSampleModel(AwtDataBufferType dataType, int w, int h, int pixelStride, int scanlineStride, int[] bandOffsets) :
            base(dataType, w, h, pixelStride, scanlineStride, bandOffsets)
        {
            int minBandOff = this.mBandOffsets[0];
            int maxBandOff = this.mBandOffsets[0];
            for (int i = 1; i < this.mBandOffsets.Length; i++)
            {
                minBandOff = Math.Min(minBandOff, this.mBandOffsets[i]);
                maxBandOff = Math.Max(maxBandOff, this.mBandOffsets[i]);
            }

            maxBandOff -= minBandOff;
            if (maxBandOff > scanlineStride)
                throw new ArgumentException("Offsets between bands must be less than the scanline stride");

            if (pixelStride * w > scanlineStride)
                throw new ArgumentException("Pixel stride times width must be less than or equal to the scanline stride");

            if (pixelStride < maxBandOff)
                throw new ArgumentException("Pixel stride must be greater than or equal to the offsets between bands");
        }

        public override AwtSampleModel CreateCompatibleSampleModel(int w, int h)
        {
            int minBandoff = mBandOffsets[0];
            int numBands = mBandOffsets.Length;
            for (int i = 1; i < numBands; i++)
            {
                if (mBandOffsets[i] < minBandoff)
                    minBandoff = mBandOffsets[i];
            }

            int[] bandOff;
            if (minBandoff > 0)
            {
                bandOff = new int[numBands];
                for (int i = 0; i < numBands; i++)
                    bandOff[i] = mBandOffsets[i] - minBandoff;
            }
            else
                bandOff = mBandOffsets;

            return new AwtPixelInterleavedSampleModel(mDataType, w, h, mPixelStride, mPixelStride * w, bandOff);
        }

        public override AwtSampleModel CreateSubsetSampleModel(int[] bands)
        {
            int[] newBandOffsets = new int[bands.Length];
            for (int i = 0; i < bands.Length; i++)
                newBandOffsets[i] = mBandOffsets[bands[i]];

            return new AwtPixelInterleavedSampleModel(this.mDataType, mWidth, mHeight, this.mPixelStride, mScanlineStride, newBandOffsets);
        }
    }
}
#endif
