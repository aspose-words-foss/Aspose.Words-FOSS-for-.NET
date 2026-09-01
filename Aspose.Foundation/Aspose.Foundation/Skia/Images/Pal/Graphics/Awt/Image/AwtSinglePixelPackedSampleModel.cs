// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD || NET

using System;

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal class AwtSinglePixelPackedSampleModel : AwtSampleModel
    {
        public AwtSinglePixelPackedSampleModel(AwtDataBufferType dataType, int w, int h, int[] bitMasks) : this(dataType, w, h, w, bitMasks)
        {
            if (dataType != AwtDataBufferType.Byte && dataType != AwtDataBufferType.Ushort && dataType != AwtDataBufferType.Int)
                throw new ArgumentException("Unsupported data type " + dataType);
        }

        public AwtSinglePixelPackedSampleModel(AwtDataBufferType dataType, int w, int h, int scanlineStride, int[] bitMasks) :
            base(dataType, w, h, bitMasks.Length)
        {
            if (dataType != AwtDataBufferType.Byte && dataType != AwtDataBufferType.Ushort && dataType != AwtDataBufferType.Int)
                throw new ArgumentException("Unsupported data type " + dataType);

            this.mDataType = dataType;
            this.mBitMasks = (int[])bitMasks.Clone();
            this.mScanlineStride = scanlineStride;

            this.mBitOffsets = new int[mNumBands];
            this.mBitSizes = new int[mNumBands];

            int maxMask = (int)((1L << AwtDataBuffer.GetDataTypeSize(dataType)) - 1);

            this.mMaxBitSize = 0;
            for (int i = 0; i < mNumBands; i++)
            {
                int bitOffset = 0, bitSize = 0, mask;
                this.mBitMasks[i] &= maxMask;
                mask = this.mBitMasks[i];
                if (mask != 0)
                {
                    while ((mask & 1) == 0)
                    {
                        mask = mask >> 1;
                        bitOffset++;
                    }
                    while ((mask & 1) == 1)
                    {
                        mask = mask >> 1;
                        bitSize++;
                    }
                    if (mask != 0)
                        throw new ArgumentException("Mask " + bitMasks[i] + " must be contiguous");
                }
                mBitOffsets[i] = bitOffset;
                mBitSizes[i] = bitSize;
                if (bitSize > mMaxBitSize)
                    mMaxBitSize = bitSize;
            }
        }

        public override void SetSample(int x, int y, int b, int s, AwtDataBuffer data)
        {
            // Bounds check for 'b' will be performed automatically
            if ((x < 0) || (y < 0) || (x >= mWidth) || (y >= mHeight))
            {
                throw new ArgumentOutOfRangeException("Coordinate out of bounds!");
            }
            int value = data.GetElem(y * mScanlineStride + x);
            value &= ~mBitMasks[b];
            value |= (s << mBitOffsets[b]) & mBitMasks[b];
            data.SetElem(y * mScanlineStride + x, value);
        }

        public override AwtSampleModel CreateSubsetSampleModel(int[] bands)
        {
            if (bands.Length > mNumBands)
                throw new ArgumentException("There are only " + mNumBands + " bands");

            int[] newBitMasks = new int[bands.Length];
            for (int i = 0; i < bands.Length; i++)
                newBitMasks[i] = mBitMasks[bands[i]];

            return new AwtSinglePixelPackedSampleModel(this.mDataType, mWidth, mHeight, this.mScanlineStride, newBitMasks);
        }

        public override int GetSample(int x, int y, int b, AwtDataBuffer data)
        {
            // Bounds check for 'b' will be performed automatically
            if ((x < 0) || (y < 0) || (x >= mWidth) || (y >= mHeight))
                throw new ArgumentOutOfRangeException("Coordinate out of bounds!");

            int sample = data.GetElem(y * mScanlineStride + x);
            return ((sample & mBitMasks[b]) >> mBitOffsets[b]);
        }

        public override AwtDataBuffer CreateDataBuffer()
        {
            AwtDataBuffer dataBuffer = null;

            int size = (int)BufferSize;
            switch (mDataType)
            {
                case AwtDataBufferType.Byte:
                    dataBuffer = new AwtDataBufferByte(size);
                    break;
                case AwtDataBufferType.Ushort:
                    dataBuffer = new AwtDataBufferUShort(size);
                    break;
                case AwtDataBufferType.Int:
                    dataBuffer = new AwtDataBufferInt(size);
                    break;
                default:
                    throw new ArgumentException("Unexpected data type");
            }
            return dataBuffer;
        }

        public override int GetSampleSize(int band)
        {
            return mBitSizes[band];
        }

        public override object GetDataElements(int x, int y, object obj, AwtDataBuffer data)
        {
            // Bounds check for 'b' will be performed automatically
            if ((x < 0) || (y < 0) || (x >= mWidth) || (y >= mHeight))
            {
                throw new ArgumentOutOfRangeException("Coordinate out of bounds!");
            }

            AwtDataBufferType type = TransferType;

            switch (type)
            {

                case AwtDataBufferType.Byte:
                {
                    byte[] bdata;

                    if (obj == null)
                        bdata = new byte[1];
                    else
                        bdata = (byte[])obj;

                    bdata[0] = (byte)data.GetElem(y * mScanlineStride + x);

                    obj = (object)bdata;
                    break;
                }
                case AwtDataBufferType.Ushort:
                {
                    short[] sdata;

                    if (obj == null)
                        sdata = new short[1];
                    else
                        sdata = (short[])obj;

                    sdata[0] = (short)data.GetElem(y * mScanlineStride + x);

                    obj = (object)sdata;
                    break;
                }
                case AwtDataBufferType.Int:
                {
                    int[] idata;

                    if (obj == null)
                        idata = new int[1];
                    else
                        idata = (int[])obj;

                    idata[0] = data.GetElem(y * mScanlineStride + x);

                    obj = (object)idata;
                    break;
                }
                default:
                {
                    throw new ArgumentException("Unexpected data type");
                }
            }

            return obj;
        }

        public int GetOffset(int x, int y)
        {
            int offset = y * mScanlineStride + x;
            return offset;
        }

        public override int NumDataElements
        {
            get { return 1; }
        }

        public override int[] SampleSize
        {
            get { return (int[])mBitSizes.Clone(); }
        }

        private long BufferSize
        {
            get { return mScanlineStride * (mHeight - 1) + mWidth; }
        }

        public int ScanlineStride
        {
            get { return mScanlineStride; }
        }

        private readonly int[] mBitMasks;
        private readonly int[] mBitOffsets;
        private readonly int[] mBitSizes;
        private readonly int mMaxBitSize;
        private readonly int mScanlineStride;

    }
}
#endif
