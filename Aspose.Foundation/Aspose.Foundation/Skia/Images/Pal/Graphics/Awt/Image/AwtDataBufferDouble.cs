// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD || NET

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal class AwtDataBufferDouble : AwtDataBuffer
    {
        public AwtDataBufferDouble(int size, int numBanks) : base(AwtDataBufferType.Double, size, numBanks)
        {
            mBankdata = new double[numBanks][];
            for (int i = 0; i < numBanks; i++)
                mBankdata[i] = new double[size];

            mData = mBankdata[0];
        }

        public override int GetElem(int bank, int i)
        {
            return (int)(mBankdata[bank][i + mOffsets[bank]]);
        }

        public override int GetElem(int i)
        {
            return (int)(mData[i + mOffset]);
        }

        public override void SetElem(int bank, int i, int val)
        {
            mBankdata[bank][i + mOffsets[bank]] = (double)val;
        }

        public double[] Data
        {
            get { return mData; }
        }

        private readonly double[][] mBankdata;
        private readonly double[] mData;
    }
}
#endif
