// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD

using System;

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal class AwtPackedColorModel : AwtColorModel
    {
        public AwtPackedColorModel(AwtColorSpace space, int bits, int rmask, int gmask, int bmask, int amask, bool isAlphaPremultiplied, 
                AwtTransparency trans, AwtDataBufferType transferType) : 
            base(bits, AwtPackedColorModel.CreateBitsArray(rmask, gmask, bmask, amask), space, (amask != 0), 
                isAlphaPremultiplied, trans, transferType)
        {
            if (space.Type != AwtColorSpaceType.Rgb)
                throw new ArgumentException("ColorSpace must be TYPE_RGB.");

            mMaskArray = new int[mNumComponents];
            mMaskOffsets = new int[mNumComponents];
            mScaleFactors = new float[mNumComponents];

            DecomposeMask(rmask, 0, "red");

            DecomposeMask(gmask, 1, "green");

            DecomposeMask(bmask, 2, "blue");

            if (amask != 0)
            {
                DecomposeMask(amask, 3, "alpha");
                if (mNBits[3] == 1)
                    mTransparency = AwtTransparency.Bitmask;
            }
        }

        public override AwtSampleModel CreateCompatibleSampleModel(int w, int h)
        {
            return new AwtSinglePixelPackedSampleModel(mTransferType, w, h, mMaskArray);
        }

        public override int GetRed(int pixel)
        {
            throw new NotImplementedException();
        }

        public override int GetGreen(int pixel)
        {
            throw new NotImplementedException();
        }

        public override int GetBlue(int pixel)
        {
            throw new NotImplementedException();
        }

        public override int GetAlpha(int pixel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// A utility function to compute the mask offset and scalefactor,
        /// store these and the mask in instance arrays, and verify that
        /// the mask fits in the specified pixel size.
        /// </summary>
        private void DecomposeMask(int mask, int idx, String componentName)
        {
            int off = 0;
            int count = mNBits[idx];

            // Store the mask
            mMaskArray[idx] = mask;

            // Now find the shift
            if (mask != 0)
            {
                while ((mask & 1) == 0)
                {
                    mask >>= 1;
                    off++;
                }
            }

            if (off + count > mPixelBits)
            {
                throw new ArgumentException(string.Format("{0} mask {1:X} overflows pixel (expecting {2} bits", componentName, mMaskArray[idx], mPixelBits));
            }

            mMaskOffsets[idx] = off;
            if (count == 0)
            {
                // High enough to scale any 0-ff value down to 0.0, but not
                // high enough to get Infinity when scaling back to pixel bits
                mScaleFactors[idx] = 256.0f;
            }
            else
            {
                mScaleFactors[idx] = 255.0f / ((1 << count) - 1);
            }

        }

        private static int[] CreateBitsArray(int rmask, int gmask, int bmask, int amask)
        {
            int[] arr = new int[3 + (amask == 0 ? 0 : 1)];
            arr[0] = CountBits(rmask);
            arr[1] = CountBits(gmask);
            arr[2] = CountBits(bmask);
            if (arr[0] < 0)
            {
                throw new ArgumentException(string.Format("Noncontiguous red mask {0:X}", rmask));
            }
            else if (arr[1] < 0)
            {
                throw new ArgumentException(string.Format("Noncontiguous green mask {0:X}", gmask));
            }
            else if (arr[2] < 0)
            {
                throw new ArgumentException(string.Format("Noncontiguous blue mask {0:X}", bmask));
            }
            if (amask != 0)
            {
                arr[3] = CountBits(amask);
                if (arr[3] < 0)
                {
                    throw new ArgumentException(string.Format("Noncontiguous alpha mask {0:X}", amask));
                }
            }
            return arr;
        }

        private static int CountBits(int mask)
        {
            int count = 0;
            if (mask != 0)
            {
                while ((mask & 1) == 0)
                {
                    mask >>= 1;
                }
                while ((mask & 1) == 1)
                {
                    mask >>= 1;
                    count++;
                }
            }
            if (mask != 0)
            {
                return -1;
            }
            return count;
        }

        protected int[] mMaskArray;
        protected int[] mMaskOffsets;
        protected float[] mScaleFactors;
    }
}
#endif
