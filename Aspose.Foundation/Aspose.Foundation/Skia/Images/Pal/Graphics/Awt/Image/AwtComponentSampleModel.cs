// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD

using System;

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal class AwtComponentSampleModel : AwtSampleModel
    {
        public AwtComponentSampleModel(AwtDataBufferType dataType, int w, int h, int pixelStride, int scanlineStride, int[] bankIndices, int[] bandOffsets) :
            base(dataType, w, h, bandOffsets.Length)
        {
            mDataType = dataType;
            mPixelStride = pixelStride;
            mScanlineStride = scanlineStride;
            mBandOffsets = (int[])bandOffsets.Clone();
            mBankIndices = (int[])bankIndices.Clone();
            mNumBands = 1;

            if (pixelStride < 0)
                throw new ArgumentException("Pixel stride must be >= 0");

            // TODO - bug 4296691 - remove this check
            if (scanlineStride < 0)
                throw new ArgumentException("Scanline stride must be >= 0");

            if ((dataType < AwtDataBufferType.Byte) || (dataType > AwtDataBufferType.Double))
                throw new ArgumentException("Unsupported dataType.");

            int maxBank = this.mBankIndices[0];
            if (maxBank < 0)
                throw new ArgumentException("Index of bank 0 is less than " + "0 (" + maxBank + ")");

            for (int i = 1; i < mBankIndices.Length; i++)
            {
                if (mBankIndices[i] > maxBank)
                    maxBank = this.mBankIndices[i];

                else if (this.mBankIndices[i] < 0)
                    throw new ArgumentException("Index of bank " + i + " is less than 0 (" + maxBank + ")");
            }
            mNumBanks = maxBank + 1;
            mNumBands = mBandOffsets.Length;
            if (mBandOffsets.Length != mBankIndices.Length)
                throw new ArgumentException("Length of bandOffsets must equal Length of bankIndices.");
        }

        public AwtComponentSampleModel(AwtDataBufferType dataType, int w, int h, int pixelStride, int scanlineStride, int[] bandOffsets) :
            base(dataType, w, h, bandOffsets.Length)
        {
            mDataType = dataType;
            mPixelStride = pixelStride;
            mScanlineStride = scanlineStride;
            mBandOffsets = (int[])bandOffsets.Clone();
            mNumBands = this.mBandOffsets.Length;

            if (pixelStride < 0)
                throw new ArgumentException("Pixel stride must be >= 0");

            // TODO - bug 4296691 - remove this check
            if (scanlineStride < 0)
                throw new ArgumentException("Scanline stride must be >= 0");

            if (mNumBands < 1)
                throw new ArgumentException("Must have at least one band.");

            if ((dataType < AwtDataBufferType.Byte) || (dataType > AwtDataBufferType.Double))
                throw new ArgumentException("Unsupported dataType.");

            mBankIndices = new int[mNumBands];
            for (int i = 0; i < mNumBands; i++)
                mBankIndices[i] = 0;
        }

        public override AwtSampleModel CreateSubsetSampleModel(int[] bands)
        {
            if (bands.Length > mBankIndices.Length)
                throw new ArgumentException("There are only " + mBankIndices.Length + " bands");

            int[] newBankIndices = new int[bands.Length];
            int[] newBandOffsets = new int[bands.Length];

            for (int i = 0; i < bands.Length; i++)
            {
                newBankIndices[i] = mBankIndices[bands[i]];
                newBandOffsets[i] = mBandOffsets[bands[i]];
            }

            return new AwtComponentSampleModel(this.mDataType, mWidth, mHeight, this.mPixelStride, this.mScanlineStride, newBankIndices, newBandOffsets);
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

            int pixelOffset = y * mScanlineStride + x * mPixelStride;
            for (int i = 0; i < mNumBands; i++)
                pixels[i] = data.GetElem(mBankIndices[i], pixelOffset + mBandOffsets[i]);

            return pixels;
        }

        public override int GetSample(int x, int y, int b, AwtDataBuffer data)
        {
            // Bounds check for 'b' will be performed automatically
            if ((x < 0) || (y < 0) || (x >= mWidth) || (y >= mHeight))
                throw new ArgumentOutOfRangeException("Coordinate out of bounds!");

            int sample = data.GetElem(mBankIndices[b], y * mScanlineStride + x * mPixelStride + mBandOffsets[b]);
            return sample;
        }

        public override void SetSample(int x, int y, int b, int s, AwtDataBuffer data)
        {
            // Bounds check for 'b' will be performed automatically
            if ((x < 0) || (y < 0) || (x >= mWidth) || (y >= mHeight))
                throw new ArgumentOutOfRangeException("Coordinate out of bounds!");

            data.SetElem(mBankIndices[b], y * mScanlineStride + x * mPixelStride + mBandOffsets[b], s);
        }

        public override AwtDataBuffer CreateDataBuffer()
        {
            AwtDataBuffer dataBuffer = null;

            int size = GetBufferSize();
            switch (mDataType)
            {
                case AwtDataBufferType.Byte:
                    dataBuffer = new AwtDataBufferByte(size, mNumBanks);
                    break;
                case AwtDataBufferType.Ushort:
                    dataBuffer = new AwtDataBufferUShort(size, mNumBanks);
                    break;
                case AwtDataBufferType.Short:
                    dataBuffer = new AwtDataBufferShort(size, mNumBanks);
                    break;
                case AwtDataBufferType.Int:
                    dataBuffer = new AwtDataBufferInt(size, mNumBanks);
                    break;
                case AwtDataBufferType.Float:
                    dataBuffer = new AwtDataBufferFloat(size, mNumBanks);
                    break;
                case AwtDataBufferType.Double:
                    dataBuffer = new AwtDataBufferDouble(size, mNumBanks);
                    break;
                default:
                    throw new ArgumentException("Unexpected data type.");
            }

            return dataBuffer;
        }

        public override object GetDataElements(int x, int y, object obj, AwtDataBuffer data)
        {
            if ((x < 0) || (y < 0) || (x >= mWidth) || (y >= mHeight))
                throw new ArgumentOutOfRangeException("Coordinate out of bounds!");

            AwtDataBufferType type = TransferType;
            int numDataElems = NumDataElements;
            int pixelOffset = y * mScanlineStride + x * mPixelStride;

            switch (type)
            {
                case AwtDataBufferType.Byte:
                {
                    byte[] bdata;

                    if (obj == null)
                        bdata = new byte[numDataElems];
                    else
                        bdata = (byte[])obj;

                    for (int i = 0; i < numDataElems; i++)
                    {
                        bdata[i] = (byte)data.GetElem(mBankIndices[i], pixelOffset + mBandOffsets[i]);
                    }

                    obj = (object)bdata;
                    break;
                }
                case AwtDataBufferType.Ushort:
                case AwtDataBufferType.Short:
                {
                    short[] sdata;

                    if (obj == null)
                        sdata = new short[numDataElems];
                    else
                        sdata = (short[])obj;

                    for (int i = 0; i < numDataElems; i++)
                    {
                        sdata[i] = (short)data.GetElem(mBankIndices[i],
                                pixelOffset + mBandOffsets[i]);
                    }

                    obj = (object)sdata;
                    break;
                }
                case AwtDataBufferType.Int:
                {
                    int[] idata;

                    if (obj == null)
                        idata = new int[numDataElems];
                    else
                        idata = (int[])obj;

                    for (int i = 0; i < numDataElems; i++)
                    {
                        idata[i] = data.GetElem(mBankIndices[i],
                                pixelOffset + mBandOffsets[i]);
                    }

                    obj = (object)idata;
                    break;
                }
                case AwtDataBufferType.Float:
                {
                    float[] fdata;

                    if (obj == null)
                        fdata = new float[numDataElems];
                    else
                        fdata = (float[])obj;

                    for (int i = 0; i < numDataElems; i++)
                    {
                        fdata[i] = data.GetElemFloat(mBankIndices[i], pixelOffset + mBandOffsets[i]);
                    }

                    obj = (object)fdata;
                    break;
                }
                case AwtDataBufferType.Double:
                {
                    double[] ddata;

                    if (obj == null)
                        ddata = new double[numDataElems];
                    else
                        ddata = (double[])obj;

                    for (int i = 0; i < numDataElems; i++)
                    {
                        ddata[i] = data.GetElemDouble(mBankIndices[i],
                                pixelOffset + mBandOffsets[i]);
                    }

                    obj = (object)ddata;
                    break;
                }
                default:
                    throw new ArgumentException("Unexpected data type.");
            }

            return obj;
        }

        public int GetOffset(int x, int y)
        {
            int offset = y * mScanlineStride + x * mPixelStride + mBandOffsets[0];
            return offset;
        }

        public override int GetSampleSize(int band)
        {
            return AwtDataBuffer.GetDataTypeSize(mDataType);
        }

        private int GetBufferSize()
        {
            int maxBandOff = mBandOffsets[0];
            for (int i = 1; i < mBandOffsets.Length; i++)
                maxBandOff = Math.Max(maxBandOff, mBandOffsets[i]);

            if (maxBandOff < 0 || maxBandOff > (int.MaxValue - 1))
                throw new ArgumentException("Invalid band offset");

            if (mPixelStride < 0 || mPixelStride > (int.MaxValue / mWidth))
                throw new ArgumentException("Invalid pixel stride");

            if (mScanlineStride < 0 || mScanlineStride > (int.MaxValue / mHeight))
                throw new ArgumentException("Invalid scanline stride");

            int size = maxBandOff + 1;
            int val = mPixelStride * (mWidth - 1);

            if (val > (int.MaxValue - size))
                throw new ArgumentException("Invalid pixel stride");

            size += val;
            val = mScanlineStride * (mHeight - 1);

            if (val > (int.MaxValue - size))
                throw new ArgumentException("Invalid scan stride");

            size += val;
            return size;
        }

        public int[] BandOffsets
        {
            get { return mBandOffsets; }
        }

        public int[] BankIndices
        {
            get { return mBankIndices; }
        }
                
        public override int NumDataElements
        {
            get { return NumBands; }
        }

        public override int[] SampleSize
        {
            get
            {
                int[] sampleSize = new int[mNumBands];
                int sizeInBits = GetSampleSize(0);

                for (int i = 0; i < mNumBands; i++)
                    sampleSize[i] = sizeInBits;

                return sampleSize;
            }
        }

        public int ScanlineStride
        {
           get { return mScanlineStride; }
        }

        public int PixelStride
        {
            get { return mPixelStride; }
        }

        protected int[] mBandOffsets;
        protected int[] mBankIndices;
        protected int mNumBanks = 1;
        protected int mScanlineStride;
        protected int mPixelStride;
    }
}
#endif
