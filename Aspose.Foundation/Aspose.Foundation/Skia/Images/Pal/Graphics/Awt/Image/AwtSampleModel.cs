// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD

using System;

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal abstract class AwtSampleModel
    {
        public AwtSampleModel(AwtDataBufferType dataType, int w, int h, int numBands)
        {
            long size = (long)w * h;
            if (w <= 0 || h <= 0)
                throw new ArgumentException("Width (" + w + ") and height (" + h + ") must be > 0");

            if (size >= int.MaxValue)
                throw new ArgumentException("Dimensions (width=" + w + " height=" + h + ") are too large");

            if (dataType < AwtDataBufferType.Byte || (dataType > AwtDataBufferType.Double && dataType != AwtDataBufferType.Undefined))
                throw new ArgumentException("Unsupported dataType: " + dataType);

            if (numBands <= 0)
                throw new ArgumentException("Number of bands must be > 0");

            mDataType = dataType;
            mWidth = w;
            mHeight = h;
            mNumBands = numBands;
        }

        public virtual int[] GetPixel(int x, int y, int[] iArray, AwtDataBuffer data)
        {
            int[] pixels;

            if (iArray != null)
                pixels = iArray;
            else
                pixels = new int[mNumBands];

            for (int i = 0; i < mNumBands; i++)
                pixels[i] = GetSample(x, y, i, data);

            return pixels;
        }

        public int[] GetPixels(int x, int y, int w, int h, int[] iArray, AwtDataBuffer data)
        {
            int[] pixels;
            int Offset = 0;
            int x1 = x + w;
            int y1 = y + h;

            if (x < 0 || x >= mWidth || w > mWidth || x1 < 0 || x1 > mWidth || y < 0 || y >= mHeight || h > mHeight || y1 < 0 || y1 > mHeight)
                throw new ArgumentOutOfRangeException("Invalid coordinates.");

            if (iArray != null)
                pixels = iArray;
            else
                pixels = new int[mNumBands * w * h];

            for (int i = y; i < y1; i++)
            {
                for (int j = x; j < x1; j++)
                {
                    for (int k = 0; k < mNumBands; k++)
                        pixels[Offset++] = GetSample(j, i, k, data);
                }
            }

            return pixels;
        }

        public virtual double[] GetPixels(int x, int y, int w, int h, double[] dArray, AwtDataBuffer data)
        {
            double[] pixels;
            int Offset = 0;
            int x1 = x + w;
            int y1 = y + h;

            if (x < 0 || x >= mWidth || w > mWidth || x1 < 0 || x1 > mWidth || y < 0 || y >= mHeight || h > mHeight || y1 < 0 || y1 > mHeight)
                throw new ArgumentOutOfRangeException("Invalid coordinates.");

            if (dArray != null)
                pixels = dArray;
            else
                pixels = new double[mNumBands * w * h];

            // Fix 4217412
            for (int i = y; i < y1; i++)
            {
                for (int j = x; j < x1; j++)
                {
                    for (int k = 0; k < mNumBands; k++)
                        pixels[Offset++] = GetSampleDouble(j, i, k, data);
                }
            }

            return pixels;
        }

        public virtual float[] GetPixels(int x, int y, int w, int h, float[] fArray, AwtDataBuffer data)
        {
            float[] pixels;
            int Offset = 0;
            int x1 = x + w;
            int y1 = y + h;

            if (x < 0 || x >= mWidth || w > mWidth || x1 < 0 || x1 > mWidth || y < 0 || y >= mHeight || h > mHeight || y1 < 0 || y1 > mHeight)
                throw new ArgumentOutOfRangeException("Invalid coordinates.");

            if (fArray != null)
                pixels = fArray;
            else
                pixels = new float[mNumBands * w * h];

            for (int i = y; i < y1; i++)
            {
                for (int j = x; j < x1; j++)
                {
                    for (int k = 0; k < mNumBands; k++)
                        pixels[Offset++] = GetSampleFloat(j, i, k, data);
                }
            }

            return pixels;
        }

        public void SetPixels(int x, int y, int w, int h, int[] iArray, AwtDataBuffer data)
        {
            int Offset = 0;
            int x1 = x + w;
            int y1 = y + h;

            if (x < 0 || x >= mWidth || w > mWidth || x1 < 0 || x1 > mWidth || y < 0 || y >= mHeight || h > mHeight || y1 < 0 || y1 > mHeight)
                throw new ArgumentOutOfRangeException("Invalid coordinates.");

            for (int i = y; i < y1; i++)
            {
                for (int j = x; j < x1; j++)
                {
                    for (int k = 0; k < mNumBands; k++)
                        SetSample(j, i, k, iArray[Offset++], data);
                }
            }
        }

        public void SetPixels(int x, int y, int w, int h, double[] dArray, AwtDataBuffer data)
        {
            int Offset = 0;
            int x1 = x + w;
            int y1 = y + h;

            if (x < 0 || x >= mWidth || w > mWidth || x1 < 0 || x1 > mWidth || y < 0 || y >= mHeight || h > mHeight || y1 < 0 || y1 > mHeight)
                throw new ArgumentOutOfRangeException("Invalid coordinates.");

            for (int i = y; i < y1; i++)
            {
                for (int j = x; j < x1; j++)
                {
                    for (int k = 0; k < mNumBands; k++)
                        SetSample(j, i, k, dArray[Offset++], data);
                }
            }
        }

        public virtual void SetPixels(int x, int y, int w, int h, float[] fArray, AwtDataBuffer data)
        {
            int Offset = 0;
            int x1 = x + w;
            int y1 = y + h;

            if (x < 0 || x >= mWidth || w > mWidth || x1 < 0 || x1 > mWidth || y < 0 || y >= mHeight || h > mHeight || y1 < 0 || y1 > mHeight)
                throw new ArgumentOutOfRangeException("Invalid coordinates.");

            for (int i = y; i < y1; i++)
            {
                for (int j = x; j < x1; j++)
                {
                    for (int k = 0; k < mNumBands; k++)
                        SetSample(j, i, k, fArray[Offset++], data);
                }
            }
        }

        public float GetSampleFloat(int x, int y, int b, AwtDataBuffer data)
        {
            float sample;
            sample = (float)GetSample(x, y, b, data);
            return sample;
        }

        public double GetSampleDouble(int x, int y, int b, AwtDataBuffer data)
        {
            double sample;
            sample = (double)GetSample(x, y, b, data);
            return sample;
        }

        public void SetSample(int x, int y, int b, double s, AwtDataBuffer data)
        {
            int sample = (int)s;
            SetSample(x, y, b, sample, data);
        }

        public void SetSample(int x, int y, int b, float s, AwtDataBuffer data)
        {
            int sample = (int)s;
            SetSample(x, y, b, sample, data);
        }

        public virtual AwtSampleModel CreateCompatibleSampleModel(int w, int h) { throw new NotImplementedException(); }

        public abstract AwtSampleModel CreateSubsetSampleModel(int[] bands);

        public abstract int GetSample(int x, int y, int b, AwtDataBuffer data);

        public abstract void SetSample(int x, int y, int b, int s, AwtDataBuffer data);

        public abstract AwtDataBuffer CreateDataBuffer();

        public abstract int GetSampleSize(int band);

        public abstract object GetDataElements(int x, int y, object obj, AwtDataBuffer data);

        public abstract int NumDataElements { get; }

        public abstract int[] SampleSize { get; }

        public virtual AwtDataBufferType TransferType
        {
            get { return mDataType; }
        }

        public int Width
        {
            get { return mWidth; }
        }

        public int Height
        {
            get { return mHeight; }
        }

        public int NumBands
        {
            get { return mNumBands; }
        }

        public AwtDataBufferType DataType
        {
            get { return mDataType; }
        }

        protected int mWidth;
        protected int mHeight;
        protected int mNumBands;
        protected AwtDataBufferType mDataType;
    }
}
#endif
