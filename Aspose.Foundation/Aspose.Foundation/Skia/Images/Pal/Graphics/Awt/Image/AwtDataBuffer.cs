// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD || NET

using System;

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal abstract class AwtDataBuffer
    {
        public AwtDataBuffer(AwtDataBufferType dataType, int size) : this(dataType, size, 1)
        {
        }

        public AwtDataBuffer(AwtDataBufferType dataType, int size, int numBanks)
        {
            mDataType = dataType;
            mBanks = numBanks;
            mSize = size;
            mOffset = 0;
            mOffsets = new int[mBanks];
        }

        public abstract int GetElem(int bank, int i);

        public abstract void SetElem(int bank, int i, int val);

        public virtual int GetElem(int i)
        {
            return GetElem(0, i);
        }

        public static int GetDataTypeSize(AwtDataBufferType type)
        {
            if (type < AwtDataBufferType.Byte || type > AwtDataBufferType.Double)
                throw new ArgumentException("Unknown data type " + type);

            return gDataTypeSize[(int)type];
        }

        public void SetElem(int i, int val)
        {
            SetElem(0, i, val);
        }

        public float GetElemFloat(int bank, int i)
        {
            return (float)GetElem(bank, i);
        }

        public double GetElemDouble(int bank, int i)
        {
            return (double)GetElem(bank, i);
        }

        public int Size
        {
            get { return mSize; }
        }

        protected AwtDataBufferType mDataType;
        protected int mBanks;
        protected int mOffset;
        protected int mSize;
        protected int[] mOffsets;

        private static readonly int[] gDataTypeSize = { 8, 16, 16, 32, 32, 64 };
    }

    internal enum AwtDataBufferType
    {
        Byte = 0,
        Ushort = 1,
        Short = 2,
        Int = 3,
        Float = 4,
        Double = 5,
        Undefined = 32
    }
}
#endif
