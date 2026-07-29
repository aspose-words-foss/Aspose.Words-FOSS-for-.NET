// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal class AwtDataBufferShort : AwtDataBuffer
    {
        public AwtDataBufferShort(int size, int numBanks) : base(AwtDataBufferType.Short, size, numBanks)
        {
            mBankdata = new short[numBanks][];
            for (int i = 0; i < numBanks; i++)
                mBankdata[i] = new short[size];

            mData = mBankdata[0];
        }

        public override int GetElem(int i)
        {
            return (int)(mData[i + mOffset]);
        }

        public override int GetElem(int bank, int i)
        {
            return (int)(mBankdata[bank][i + mOffsets[bank]]);
        }

        public override void SetElem(int bank, int i, int val)
        {
            mBankdata[bank][i + mOffsets[bank]] = (short)val;
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
