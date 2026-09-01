// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/09/2017 by Vyacheslav Durin
// Auto-ported from Java

#if NETSTANDARD || NET

namespace Aspose.Images.Pal.Graphics.Awt.Image
{
    internal class AwtDataBufferFloat : AwtDataBuffer
    {
        public AwtDataBufferFloat(int size, int numBanks) : base(AwtDataBufferType.Float, size, numBanks)
        {
            mBankdata = new float[numBanks][];
            for (int i = 0; i < numBanks; i++)
                mBankdata[i] = new float[size];

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
            mBankdata[bank][i + mOffsets[bank]] = (float)val;
        }

        public float[] Data
        {
            get { return mData; }
        }

        private readonly float[][] mBankdata;
        private readonly float[] mData;
    }
}
#endif
