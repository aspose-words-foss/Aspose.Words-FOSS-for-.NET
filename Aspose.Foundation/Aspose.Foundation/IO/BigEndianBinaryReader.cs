// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/09/2005 by Roman Korchagin

using System.IO;
using Aspose.JavaAttributes;

namespace Aspose.IO
{
    /// <summary>
    /// Big-Endian byte order binary reader.
    /// </summary>
    public class BigEndianBinaryReader
    {
        [JavaThrows(false)]
        public BigEndianBinaryReader(Stream stream)
        {
            mReader = new BinaryReader(stream);
        }

        [JavaThrows(false)]
        public Stream BaseStream
        {
            get { return mReader.BaseStream; }
        }

        /// <summary>
        /// Determines that current position is end of the stream.
        /// </summary>
        /// <remark>
        /// Note, not all streams allow to determine stream position.
        /// </remark>
        public bool IsEndOfStream
        {
            get { return BaseStream.Position == BaseStream.Length; }
        }

        public int ReadInt32()
        {
            return BitUtil.SwapInt32(mReader.ReadInt32());
        }

        public uint ReadUInt32()
        {
            return BitUtil.SwapUInt32(mReader.ReadUInt32());
        }

        public short ReadInt16()
        {
            return BitUtil.SwapInt16(mReader.ReadInt16());
        }

        public ushort ReadUInt16()
        {
            return BitUtil.SwapUInt16(mReader.ReadUInt16());
        }

        public long ReadInt64()
        {
            return BitUtil.SwapInt64(mReader.ReadInt64());
        }

        public int ReadUInt24()
        {
            byte[] data = mReader.ReadBytes(3);
            int value = 0;
            int multiplier = 1;
            for (int i = 2; i >= 0; i--)
            {
                value += data[i] * multiplier;
                multiplier <<= 8;
            }
            return value;
        }

        public byte ReadByte()
        {
            return mReader.ReadByte();
        }

        public sbyte ReadSByte()
        {
            return mReader.ReadSByte();
        }

        public byte[] ReadBytes(int count)
        {
            return mReader.ReadBytes(count);
        }

        public char[] ReadByteChars(int count)
        {
            char[] chars = new char[count];

            for (int i = 0; i < chars.Length; i++)
                chars[i] = (char)mReader.ReadByte();

            return chars;
        }

        private readonly BinaryReader mReader;
    }
}
