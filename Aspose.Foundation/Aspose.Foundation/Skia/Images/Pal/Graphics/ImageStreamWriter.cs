// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/12/2017 by Alexey Noskov

#if NETSTANDARD || NET

using System;
using System.IO;
using Aspose.IO;

namespace Aspose.Images.Pal.Graphics
{
    internal class ImageStreamWriter
    {
        public ImageStreamWriter(bool isBigEndian, Stream stream)
        {
            if (isBigEndian)
                mBigEndianWriter = new BigEndianBinaryWriter(stream);
            else
                mWriter = new BinaryWriter(stream);

            mIsBigEndian = isBigEndian;
            mStream = stream;
        }

        public void WriteBytes(byte[] val)
        {
            if (mIsBigEndian)
                mBigEndianWriter.WriteBytes(val);
            else
                mWriter.Write(val);
        }

        public void WriteByte(int val)
        {
            if (mIsBigEndian)
                mBigEndianWriter.WriteByte((byte)val);
            else
                mWriter.Write((byte)val);
        }

        public void WriteInt(int val)
        {
            if (mIsBigEndian)
                mBigEndianWriter.WriteInt32(val);
            else
                mWriter.Write((int)val);
        }

        public void WriteShort(int size)
        {
            if (mIsBigEndian)
                mBigEndianWriter.WriteInt16(size);
            else
                mWriter.Write((short)size);
        }

        public void Write(byte[] b, int off, int numBytes)
        {
            try
            {
                mStream.Write(b, off, numBytes);
            }
            catch
            {
                throw new IOException();
            }
        }

        public void WriteChars(char[] c, int off, int len)
        {
            // Fix 4430357 - if off + len < 0, overflow occurred
            if (off < 0 || len < 0 || off + len > c.Length || off + len < 0)
                throw new ArgumentOutOfRangeException("off < 0 || len < 0 || off + len > c.length!");

            byte[] b = new byte[len * 2];
            int boff = 0;
            if (mIsBigEndian)
            {
                for (int i = 0; i < len; i++)
                {
                    char v = c[off + i];
                    b[boff++] = (byte)(v >> 8);
                    b[boff++] = (byte)(v >> 0);
                }
            }
            else
            {
                for (int i = 0; i < len; i++)
                {
                    char v = c[off + i];
                    b[boff++] = (byte)(v >> 0);
                    b[boff++] = (byte)(v >> 8);
                }
            }

            mStream.Write(b, 0, len * 2);
        }

        protected void WriteShorts(short[] s, int off, int len)
        {
            // Fix 4430357 - if off + len < 0, overflow occurred
            if (off < 0 || len < 0 || off + len > s.Length || off + len < 0)
                throw new ArgumentOutOfRangeException("off < 0 || len < 0 || off + len > s.length!");

            byte[] b = new byte[len * 2];
            int boff = 0;
            if (mIsBigEndian)
            {
                for (int i = 0; i < len; i++)
                {
                    short v = s[off + i];
                    b[boff++] = (byte)(v >> 8);
                    b[boff++] = (byte)(v >> 0);
                }
            }
            else
            {
                for (int i = 0; i < len; i++)
                {
                    short v = s[off + i];
                    b[boff++] = (byte)(v >> 0);
                    b[boff++] = (byte)(v >> 8);
                }
            }

            mStream.Write(b, 0, len * 2);
        }

        public void WriteInts(int[] i, int off, int len)
        {
            // Fix 4430357 - if off + len < 0, overflow occurred
            if (off < 0 || len < 0 || off + len > i.Length || off + len < 0)
                throw new ArgumentOutOfRangeException("off < 0 || len < 0 || off + len > i.length!");

            byte[] b = new byte[len * 4];
            int boff = 0;
            if (mIsBigEndian)
            {
                for (int j = 0; j < len; j++)
                {
                    int v = i[off + j];
                    b[boff++] = (byte)(v >> 24);
                    b[boff++] = (byte)(v >> 16);
                    b[boff++] = (byte)(v >> 8);
                    b[boff++] = (byte)(v >> 0);
                }
            }
            else
            {
                for (int j = 0; j < len; j++)
                {
                    int v = i[off + j];
                    b[boff++] = (byte)(v >> 0);
                    b[boff++] = (byte)(v >> 8);
                    b[boff++] = (byte)(v >> 16);
                    b[boff++] = (byte)(v >> 24);
                }
            }

            mStream.Write(b, 0, len * 4);
        }

        public void WriteFloats(float[] f, int off, int len)
        {
            // Fix 4430357 - if off + len < 0, overflow occurred
            if (off < 0 || len < 0 || off + len > f.Length || off + len < 0)
                throw new ArgumentOutOfRangeException("off < 0 || len < 0 || off + len > f.length!");

            byte[] b = new byte[len * 4];
            int boff = 0;
            if (mIsBigEndian)
            {
                for (int i = 0; i < len; i++)
                {
                    int v = BitConverter.ToInt32(BitConverter.GetBytes(f[off + i]), 0); //Float.floatToIntBits(f[off + i]);
                    b[boff++] = (byte)(v >> 24);
                    b[boff++] = (byte)(v >> 16);
                    b[boff++] = (byte)(v >> 8);
                    b[boff++] = (byte)(v >> 0);
                }
            }
            else
            {
                for (int i = 0; i < len; i++)
                {
                    int v = BitConverter.ToInt32(BitConverter.GetBytes(f[off + i]), 0);// Float.floatToIntBits(f[off + i]);
                    b[boff++] = (byte)(v >> 0);
                    b[boff++] = (byte)(v >> 8);
                    b[boff++] = (byte)(v >> 16);
                    b[boff++] = (byte)(v >> 24);
                }
            }

            mStream.Write(b, 0, len * 4);
        }

        public void WriteDoubles(double[] d, int off, int len)
        {
            // Fix 4430357 - if off + len < 0, overflow occurred
            if (off < 0 || len < 0 || off + len > d.Length || off + len < 0)
                throw new ArgumentOutOfRangeException("off < 0 || len < 0 || off + len > d.length!");

            byte[] b = new byte[len * 8];
            int boff = 0;
            if (mIsBigEndian)
            {
                for (int i = 0; i < len; i++)
                {
                    long v = BitConverter.DoubleToInt64Bits(d[off + i]);
                    b[boff++] = (byte)(v >> 56);
                    b[boff++] = (byte)(v >> 48);
                    b[boff++] = (byte)(v >> 40);
                    b[boff++] = (byte)(v >> 32);
                    b[boff++] = (byte)(v >> 24);
                    b[boff++] = (byte)(v >> 16);
                    b[boff++] = (byte)(v >> 8);
                    b[boff++] = (byte)(v >> 0);
                }
            }
            else
            {
                for (int i = 0; i < len; i++)
                {
                    long v = BitConverter.DoubleToInt64Bits(d[off + i]);
                    b[boff++] = (byte)(v >> 0);
                    b[boff++] = (byte)(v >> 8);
                    b[boff++] = (byte)(v >> 16);
                    b[boff++] = (byte)(v >> 24);
                    b[boff++] = (byte)(v >> 32);
                    b[boff++] = (byte)(v >> 40);
                    b[boff++] = (byte)(v >> 48);
                    b[boff++] = (byte)(v >> 56);
                }
            }

            mStream.Write(b, 0, len * 8);
        }

        public void SkipBytes(int offset)
        {
            mStream.Position = mStream.Position + offset;
        }

        public void Seek(long position)
        {
            mStream.Position = position;
        }

        public void Flush()
        {
            mStream.Flush();
        }

        public Stream BaseStream
        {
            get { return mStream; }
        }

        public long Position
        {
            get { return mStream.Position; }
            set { mStream.Position = value; }
        }

        protected readonly Stream mStream;
        private readonly bool mIsBigEndian;
        protected BigEndianBinaryWriter mBigEndianWriter;
        protected BinaryWriter mWriter;
    }
}

#endif
