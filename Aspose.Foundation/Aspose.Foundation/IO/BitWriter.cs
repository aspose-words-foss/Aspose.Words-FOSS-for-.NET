// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/05/2011 by Alexey Titov

using System;

namespace Aspose.IO
{
    /// <summary>
    /// Represents a class that can sequentially write bits in array of bytes.
    /// See unit tests in Aspose.Tests.Base.TestBitWriter for usage examples.
    /// </summary>
    internal class BitWriter
    {
        public BitWriter(byte[] bytes)
        {
            mBytes = bytes;
            ResetValue();
        }

        public void WriteOneInCurrentBit()
        {
            mByteValue += (byte)mBitValue;
        }

        public void MoveToNextBit()
        {
            if (IsByteEnded)
            {
                WriteCurrentByte();
                ByteIndex++;
            }
            else
                mBitValue >>= 1;
        }

        public void Flush()
        {
            if (mBitValue != 128 && mByteIndex < mBytes.Length)
                WriteCurrentByte();
        }

        private void ResetValue()
        {
            mBitValue = 128;
            mByteValue = 0;
        }

        private void WriteCurrentByte()
        {
            mBytes[ByteIndex] = (byte)mByteValue;
        }

        public int ByteIndex
        {
            get { return mByteIndex; }
            set
            {
                Flush(); // Write current value
                ResetValue();
                mByteIndex = value;
            }
        }

        private bool IsByteEnded
        {
            get { return mBitValue == 1; }
        }

        private int mBitValue;
        private int mByteIndex;
        private int mByteValue;
        private readonly byte[] mBytes;
    }
}
