// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD || NET

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal class AwtDataBufferInt : AwtDataBuffer
    {
        public AwtDataBufferInt(int size, int numBanks) : base(AwtDataBufferType.Int, size, numBanks)
        {
            mBankdata = new int[numBanks][];
            for (int i = 0; i < numBanks; i++)
                mBankdata[i] = new int[size];

            mData = mBankdata[0];
        }

        public AwtDataBufferInt(int size) : base(AwtDataBufferType.Int, size)
        {
            mData = new int[size];
            mBankdata = new int[1][];
            mBankdata[0] = mData;
        }

        public override int GetElem(int i)
        {
            return mData[i + mOffset];
        }

        public override int GetElem(int bank, int i)
        {
            return mBankdata[bank][i + mOffsets[bank]];
        }

        public override void SetElem(int bank, int i, int val)
        {
            mBankdata[bank][i + mOffsets[bank]] = (int)val;
        }

        public int[] getData()
        {
            return mData;
        }

        private readonly int[] mData;
        private readonly int[][] mBankdata;
    }
}
#endif
