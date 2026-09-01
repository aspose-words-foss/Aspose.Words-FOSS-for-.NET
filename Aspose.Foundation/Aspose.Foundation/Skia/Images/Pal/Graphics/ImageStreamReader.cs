// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/12/2017 by Vyacheslav Durin
// Ported from Java

#if NETSTANDARD || NET
using System;
using System.IO;
using Aspose.IO;

namespace Aspose.Images.Pal.Graphics
{
    internal class ImageStreamReader
    {
        public ImageStreamReader(bool isBigEndianByteOrder, Stream stream)
        {
            mStream = stream;

            if (isBigEndianByteOrder)
                mBigEndianBinaryReader = new BigEndianBinaryReader(stream);
            else
                mBinaryReader = new BinaryReader(stream);
        }

        public long ReadUnsignedInt()
        {
            long result;
            if (mBigEndianBinaryReader != null)
                result = mBigEndianBinaryReader.ReadInt32();
            else
                result = mBinaryReader.ReadInt32();
            return result & 0xffffffffL;
        }

        public int ReadUnsignedShort()
        {
            int result;
            if (mBigEndianBinaryReader != null)
                result = mBigEndianBinaryReader.ReadInt16();
            else
                result = mBinaryReader.ReadInt16();
            return result & 0xffff;
        }

        public void ReadFully(byte[] b)
        {
            ReadFully(b, 0, b.Length);
        }

        public void ReadFully(byte[] b, int off, int len)
        {
            if (off < 0 || len < 0 || off + len > b.Length || off + len < 0)
                throw new ArgumentOutOfRangeException("off < 0 || len < 0 || off + len > b.Length!");

            while (len > 0)
            {
                int nbytes = Read(b, off, len);
                // FIX WORDSNET-20321 - BinaryReader never return negative value, so changed the code to check for zero and end reading insted of throwing exception.
                if (nbytes <= 0)
                    break;

                off += nbytes;
                len -= nbytes;
            }
        }

        public short ReadShort()
        {
            short result;
            if (mBigEndianBinaryReader != null)
                result = mBigEndianBinaryReader.ReadInt16();
            else
                result = mBinaryReader.ReadInt16();
            return result;
        }

        public int ReadInt()
        {
            int result;
            if (mBigEndianBinaryReader != null)
                result = mBigEndianBinaryReader.ReadInt32();
            else
                result = mBinaryReader.ReadInt32();
            return result;
        }

        public float ReadFloat()
        {
            float result;
            if (mBigEndianBinaryReader != null)
                result = MathUtil.IntBitsToFloat(BitUtil.SwapInt32(mBinaryReader.ReadInt32()));

            else
                result = mBinaryReader.ReadSingle();
            return result;
        }

        public double ReadDouble()
        {
            double result;
            if (mBigEndianBinaryReader != null)
                result = BitConverter.Int64BitsToDouble(BitUtil.SwapInt64(mBinaryReader.ReadInt64()));
            else
                result = mBinaryReader.ReadDouble();
            return result;
        }

        public int Read(byte[] b, int off, int len)
        {
            int result;
            if (mBigEndianBinaryReader != null)
                result = mBigEndianBinaryReader.Read(b, off, len);
            else
                result = mBinaryReader.Read(b, off, len);
            return result;
        }

        public void Seek(long startPos)
        {
            mStream.Position = startPos;
        }

        public void SkipBytes(int i)
        {
            mStream.Position = mStream.Position + i;
        }

        public long StreamPosition
        {
            get { return mStream.Position; }
        }

        private readonly BinaryReader mBinaryReader;
        private readonly BigEndianBinaryReader mBigEndianBinaryReader;
        private readonly Stream mStream;
    }
}
#endif
