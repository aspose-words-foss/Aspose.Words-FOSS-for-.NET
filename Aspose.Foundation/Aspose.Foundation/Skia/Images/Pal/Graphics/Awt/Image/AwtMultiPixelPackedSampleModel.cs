// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD || NET

using System;

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal class AwtMultiPixelPackedSampleModel : AwtSampleModel
    {
        public AwtMultiPixelPackedSampleModel(AwtDataBufferType dataType, int w, int h, int numberOfBits) :
            this(dataType, w, h, numberOfBits, (w * numberOfBits + AwtDataBuffer.GetDataTypeSize(dataType) - 1) / AwtDataBuffer.GetDataTypeSize(dataType), 0)
        {
            if (dataType != AwtDataBufferType.Byte && dataType != AwtDataBufferType.Ushort && dataType != AwtDataBufferType.Int)
                throw new ArgumentException("Unsupported data type " + dataType);
        }

        public AwtMultiPixelPackedSampleModel(AwtDataBufferType dataType, int w, int h, int numberOfBits, int scanlineStride, int dataBitOffset) :
            base(dataType, w, h, 1)
        {
            if (dataType != AwtDataBufferType.Byte && dataType != AwtDataBufferType.Ushort && dataType != AwtDataBufferType.Int)
                throw new ArgumentException("Unsupported data type " + dataType);

            this.mDataType = dataType;
            this.mPixelBitStride = numberOfBits;
            this.mScanlineStride = scanlineStride;
            this.mDataBitOffset = dataBitOffset;
            this.mDataElementSize = AwtDataBuffer.GetDataTypeSize(dataType);
            this.mPixelsPerDataElement = mDataElementSize / numberOfBits;
            if (mPixelsPerDataElement * numberOfBits != mDataElementSize)
                throw new InvalidOperationException("MultiPixelPackedSampleModel does not allow pixels to span data element boundaries");

            this.mBitMask = (1 << numberOfBits) - 1;
        }

        public override int GetSampleSize(int band)
        {
            return mPixelBitStride;
        }

        public override AwtSampleModel CreateSubsetSampleModel(int[] bands)
        {
            if ((bands != null) && (bands.Length != 1))
                throw new ArgumentException("MultiPixelPackedSampleModel has only one band.");

            return CreateCompatibleSampleModel(mWidth, mHeight);
        }

        public override AwtSampleModel CreateCompatibleSampleModel(int w, int h)
        {
            return new AwtMultiPixelPackedSampleModel(mDataType, w, h, mPixelBitStride);
        }

        public override int[] GetPixel(int x, int y, int[] iArray, AwtDataBuffer data)
        {
            if ((x < 0) || (y < 0) || (x >= mWidth) || (y >= mHeight))
                throw new ArgumentOutOfRangeException("Coordinate out of bounds!");

            int[] pixels;
            if (iArray != null)
                pixels = iArray;
            else
                pixels = new int[mNumBands];

            int bitnum = mDataBitOffset + x * mPixelBitStride;
            int element = data.GetElem(y * mScanlineStride + bitnum / mDataElementSize);
            int shift = mDataElementSize - (bitnum & (mDataElementSize - 1)) - mPixelBitStride;
            pixels[0] = (element >> shift) & mBitMask;
            return pixels;
        }

        public override int GetSample(int x, int y, int b, AwtDataBuffer data)
        {
            // 'b' must be 0
            if ((x < 0) || (y < 0) || (x >= mWidth) || (y >= mHeight) || (b != 0))
                throw new ArgumentOutOfRangeException("Coordinate out of bounds!");

            int bitnum = mDataBitOffset + x * mPixelBitStride;
            int element = data.GetElem(y * mScanlineStride + bitnum / mDataElementSize);
            int shift = mDataElementSize - (bitnum & (mDataElementSize - 1)) - mPixelBitStride;
            return (element >> shift) & mBitMask;
        }

        public override void SetSample(int x, int y, int b, int s, AwtDataBuffer data)
        {
            // 'b' must be 0
            if ((x < 0) || (y < 0) || (x >= mWidth) || (y >= mHeight) || (b != 0))
                throw new ArgumentOutOfRangeException("Coordinate out of bounds!");

            int bitnum = mDataBitOffset + x * mPixelBitStride;
            int index = y * mScanlineStride + (bitnum / mDataElementSize);
            int shift = mDataElementSize - (bitnum & (mDataElementSize - 1))
                    - mPixelBitStride;
            int element = data.GetElem(index);
            element &= ~(mBitMask << shift);
            element |= (s & mBitMask) << shift;
            data.SetElem(index, element);
        }

        public override AwtDataBuffer CreateDataBuffer()
        {
            AwtDataBuffer dataBuffer = null;

            int size = (int)mScanlineStride * mHeight;
            switch (mDataType)
            {
                case AwtDataBufferType.Byte:
                    dataBuffer = new AwtDataBufferByte(size + (mDataBitOffset + 7) / 8);
                    break;
                case AwtDataBufferType.Ushort:
                    dataBuffer = new AwtDataBufferUShort(size + (mDataBitOffset + 15) / 16);
                    break;
                case AwtDataBufferType.Int:
                    dataBuffer = new AwtDataBufferInt(size + (mDataBitOffset + 31) / 32);
                    break;
                default: // Added default case for SQ.
                    break;
            }
            return dataBuffer;
        }

        public override object GetDataElements(int x, int y, object obj, AwtDataBuffer data)
        {
            if ((x < 0) || (y < 0) || (x >= mWidth) || (y >= mHeight))
                throw new ArgumentOutOfRangeException("Coordinate out of bounds!");

            AwtDataBufferType type = TransferType;
            int bitnum = mDataBitOffset + x * mPixelBitStride;
            int shift = mDataElementSize - (bitnum & (mDataElementSize - 1)) - mPixelBitStride;
            int element = 0;

            switch (type)
            {

                case AwtDataBufferType.Byte:

                    byte[] bdata;

                    if (obj == null)
                        bdata = new byte[1];
                    else
                        bdata = (byte[])obj;

                    element = data.GetElem(y * mScanlineStride + bitnum / mDataElementSize);
                    bdata[0] = (byte)((element >> shift) & mBitMask);

                    obj = (object)bdata;
                    break;

                case AwtDataBufferType.Ushort:

                    short[] sdata;

                    if (obj == null)
                        sdata = new short[1];
                    else
                        sdata = (short[])obj;

                    element = data.GetElem(y * mScanlineStride + bitnum / mDataElementSize);
                    sdata[0] = (short)((element >> shift) & mBitMask);

                    obj = (object)sdata;
                    break;

                case AwtDataBufferType.Int:

                    int[] idata;

                    if (obj == null)
                        idata = new int[1];
                    else
                        idata = (int[])obj;

                    element = data.GetElem(y * mScanlineStride +
                            bitnum / mDataElementSize);
                    idata[0] = (element >> shift) & mBitMask;

                    obj = (object)idata;
                    break;
                default: // Added default case for SQ.
                    break;
            }

            return obj;
        }

        public int GetOffset(int x, int y)
        {
            int offset = y * mScanlineStride;
            offset += (x * mPixelBitStride + mDataBitOffset) / mDataElementSize;
            return offset;
        }

        public override AwtDataBufferType TransferType
        {
            get
            {
                if (mPixelBitStride > 16)
                    return AwtDataBufferType.Int;
                else if (mPixelBitStride > 8)
                    return AwtDataBufferType.Ushort;
                else
                    return AwtDataBufferType.Byte;
            }
        }

        public override int[] SampleSize
        {
            get
            {
                int[] sampleSize = { mPixelBitStride };
                return sampleSize;
            }
        }

        public override int NumDataElements
        {
            get { return 1; }
        }

        public int PixelBitStride
        {
            get { return mPixelBitStride; }
        }

        public int ScanlineStride
        {
            get { return mScanlineStride; }
        }

        public int DataBitOffset
        {
            get { return mDataBitOffset; }
        }

        private readonly int mPixelBitStride;
        private readonly int mBitMask;
        private readonly int mPixelsPerDataElement;
        private readonly int mDataElementSize;
        private readonly int mDataBitOffset;
        private readonly int mScanlineStride;
    }
}
#endif
