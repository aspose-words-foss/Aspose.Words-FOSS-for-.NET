// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD || NET

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal class AwtDataBufferUShort : AwtDataBuffer
    {
        public AwtDataBufferUShort(int size, int numBanks) : base(AwtDataBufferType.Ushort, size, numBanks)
        {
            mBankdata = new short[numBanks][];
            for (int i = 0; i < numBanks; i++)
                mBankdata[i] = new short[size];

            mData = mBankdata[0];
        }

        public AwtDataBufferUShort(int size) : base(AwtDataBufferType.Ushort, size)
        {
            mData = new short[size];
            mBankdata = new short[1][];
            mBankdata[0] = mData;
        }

        public override int GetElem(int i)
        {
            return (mData[i + mOffset] & 0xffff);
        }

        public override int GetElem(int bank, int i)
        {
            return (mBankdata[bank][i + mOffsets[bank]] & 0xffff);
        }

        public override void SetElem(int bank, int i, int val)
        {
            mBankdata[bank][i + mOffsets[bank]] = (short)(val & 0xffff);
        }

        public short[] Data
        {
            get { return mData; }
        }

        private readonly short[] mData;
        private readonly short[][] mBankdata;
    }
}
#endif
