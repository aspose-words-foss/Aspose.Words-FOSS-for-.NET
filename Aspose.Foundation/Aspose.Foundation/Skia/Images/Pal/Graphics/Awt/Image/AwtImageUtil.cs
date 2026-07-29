// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD

using Aspose.Images.Pal.Graphics.Decoder.Tiff;
using System.IO;

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal class AwtImageUtil
    {
        /// <summary>
        /// Ceil(num/den)
        /// </summary>
        public static int Ceil(int num, int den)
        {
            if (num > 0)
                num += den - 1;

            return num / den;
        }

        /// <summary>
        /// Floor(num/den)
        /// </summary>
        public static int Floor(int num, int den)
        {
            if (num < 0)
                num -= den - 1;

            return num / den;
        }

        public static int XToTileX(int x, int tileGridXOffset, int tileWidth)
        {
            x -= tileGridXOffset;
            if (x < 0)
                x += 1 - tileWidth;        // force round to -infinity (ceiling)
            return x / tileWidth;
        }

        public static int YToTileY(int y, int tileGridYOffset, int tileHeight)
        {
            y -= tileGridYOffset;
            if (y < 0)
                y += 1 - tileHeight;     // force round to -infinity (ceiling)
            return y / tileHeight;
        }

        public static bool IsBinary(AwtSampleModel sm)
        {
            return (sm is AwtMultiPixelPackedSampleModel) &&
                (((AwtMultiPixelPackedSampleModel)sm).PixelBitStride == 1) &&
                (sm.NumBands == 1) &&
                (sm.DataType == AwtDataBufferType.Byte);
        }

        /// <summary>
        /// Returns the number of bits per pixel.
        /// </summary>
        public static int GetBitsPerPixel(AwtSampleModel sm)
        {
            int bitsPerPixel = 0;
            int[] sampleSize = sm.SampleSize;
            int numBands = sampleSize.Length;
            for (int i = 0; i < numBands; i++)
                bitsPerPixel += sampleSize[i];

            return bitsPerPixel;
        }

        /// <summary>
        /// Returns whether all samples have the same number of bits.
        /// </summary>
        public static bool AreSampleSizesEqual(AwtSampleModel sm)
        {
            bool allSameSize = true;
            int[] sampleSize = sm.SampleSize;
            int sampleSize0 = sampleSize[0];
            int numBands = sampleSize.Length;

            for (int i = 1; i < numBands; i++)
            {
                if (sampleSize[i] != sampleSize0)
                {
                    allSameSize = false;
                    break;
                }
            }

            return allSameSize;
        }

        /// <summary>
        /// Determines whether the <code>DataBuffer</code> is filled without
        /// any interspersed padding bits.
        /// </summary>
        public static bool IsDataBufferBitContiguous(AwtSampleModel sm)
        {
            int dataTypeSize = GetDataTypeSize(sm.DataType);

            if (sm is AwtComponentSampleModel)
            {
                int numBands = sm.NumBands;
                for (int i = 0; i < numBands; i++)
                {
                    if (sm.GetSampleSize(i) != dataTypeSize)
                    {
                        // Sample does not fill data element.
                        return false;
                    }
                }
            }
            else if (sm is AwtMultiPixelPackedSampleModel)
            {
                AwtMultiPixelPackedSampleModel mppsm = (AwtMultiPixelPackedSampleModel)sm;
                if (dataTypeSize % mppsm.PixelBitStride != 0)
                {
                    // Pixels do not fill the data element.
                    return false;
                }
            }
            else if (sm is AwtSinglePixelPackedSampleModel)
            {
                int numBands = sm.NumBands;
                int numBits = 0;
                for (int i = 0; i < numBands; i++)
                {
                    numBits += sm.GetSampleSize(i);
                }
                if (numBits != dataTypeSize)
                {
                    // Pixel does not fill the data element.
                    return false;
                }
            }
            else
            {
                // Unknown SampleModel class.
                return false;
            }

            return true;
        }

        /// <summary>
        /// Return the number of bits occupied by <code>dataType</code>
        /// which must be one of the<code>DataBuffer</code> <code>TYPE</code> s.
        /// </summary>
        private static int GetDataTypeSize(AwtDataBufferType dataType)
        {
            int dataTypeSize = 0;
            switch (dataType)
            {
                case AwtDataBufferType.Byte:
                    dataTypeSize = 8;
                    break;
                case AwtDataBufferType.Short:
                case AwtDataBufferType.Ushort:
                    dataTypeSize = 16;
                    break;
                case AwtDataBufferType.Int:
                case AwtDataBufferType.Float:
                    dataTypeSize = 32;
                    break;
                case AwtDataBufferType.Double:
                    dataTypeSize = 64;
                    break;
                default:
                    throw new IOException("Unknown data type " + dataType);
            }

            return dataTypeSize;
        }

        public static AwtDataBufferType GetDataTypeFromNumBits(int numBits, bool isSigned)
        {
            AwtDataBufferType dataType;

            if (numBits <= 8)
                dataType = AwtDataBufferType.Byte;

            else if (numBits <= 16)
                dataType = isSigned ? AwtDataBufferType.Short : AwtDataBufferType.Ushort;

            else
                dataType = AwtDataBufferType.Int;

            return dataType;
        }

        public static int CreateMask(int[] bitsPerSample, int band)
        {
            int mask = (1 << bitsPerSample[band]) - 1;
            for (int i = band + 1; i < bitsPerSample.Length; i++)
                mask <<= bitsPerSample[i];

            return mask;
        }
    }
}
#endif
