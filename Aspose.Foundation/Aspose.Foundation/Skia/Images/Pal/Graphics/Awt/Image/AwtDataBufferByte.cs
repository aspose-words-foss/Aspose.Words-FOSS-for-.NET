// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD || NET

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal class AwtDataBufferByte : AwtDataBuffer
    {
        public AwtDataBufferByte(int size) : base(AwtDataBufferType.Byte, size)
        {
            mData = new byte[size];
            mBankdata = new byte[1][];
            mBankdata[0] = mData;
        }

        public AwtDataBufferByte(int size, int numBanks) : base(AwtDataBufferType.Byte, size, numBanks)
        {
            mBankdata = new byte[numBanks][];
            for (int i = 0; i < numBanks; i++)
                mBankdata[i] = new byte[size];

            mData = mBankdata[0];
        }

        public override int GetElem(int i)
        {
            return (int)(mData[i + mOffset]) & 0xff;
        }

        public override int GetElem(int bank, int i)
        {
            return (int)(mBankdata[bank][i + mOffsets[bank]]) & 0xff;
        }

        public override void SetElem(int bank, int i, int val)
        {
            mBankdata[bank][i + mOffsets[bank]] = (byte)val;
        }

        public byte[] Data
        {
            get { return mData; }
        }

        private readonly byte[] mData;
        private readonly byte[][] mBankdata;
    }
}
#endif
