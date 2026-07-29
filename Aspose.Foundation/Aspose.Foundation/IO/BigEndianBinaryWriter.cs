// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/09/2005 by Roman Korchagin

using System.IO;

namespace Aspose.IO
{
    /// <summary>
    /// Big-Endian byte order binary writer.
    /// </summary>
    public class BigEndianBinaryWriter
    {
        public BigEndianBinaryWriter(Stream stream)
        {
            mWriter = new BinaryWriter(stream);
        }

        public Stream BaseStream
        {
            get { return mWriter.BaseStream; }
        }

        public void WriteInt32(int value)
        {
            mWriter.Write((int)BitUtil.SwapInt32(value));
        }

        public void WriteUInt32(uint value)
        {
            mWriter.Write((int)BitUtil.SwapUInt32(value));
        }

        public void WriteInt16(int value)
        {
            mWriter.Write((short)BitUtil.SwapInt16((short)value));
        }

        public void WriteUInt16(int value)
        {
            mWriter.Write(BitUtil.SwapUInt16((ushort)value));
        }

        public void WriteSByte(int value)
        {
            mWriter.Write((sbyte)value);
        }

        public void WriteUInt24(int value)
        {
            byte b0 = (byte)((value & 0xFF0000) >> 16);
            byte b1 = (byte)((value & 0x00FF00) >> 8);
            byte b2 = (byte)(value & 0x0000FF);
            mWriter.Write(b0);
            mWriter.Write(b1);
            mWriter.Write(b2);
        }

        public void WriteInt64(long value)
        {
            mWriter.Write(BitUtil.SwapInt64(value));
        }

        public void WriteByte(byte value)
        {
            mWriter.Write((byte)value);
        }

        public void WriteBytes(byte[] buffer, int index, int count)
        {
            mWriter.Write(buffer, index, count);
        }

        public void WriteBytes(byte[] buffer)
        {
            mWriter.Write(buffer, 0, buffer.Length);
        }

        public void Flush()
        {
            mWriter.Flush();
        }

        private readonly BinaryWriter mWriter;
    }
}
