// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov

using Aspose.IO;

namespace Aspose.Fonts.Sfnt
{
    /// <summary>
    /// Represents a Table Record entry inside the 'sfnt' file.
    /// </summary>
    internal class SfntTableRecordEntry
    {
        /// <summary>
        /// Reads Table Record entry from binary reader.
        /// </summary>
        public static SfntTableRecordEntry Read(BigEndianBinaryReader reader)
        {
            SfntTableRecordEntry result = new SfntTableRecordEntry();
            result.Tag = new string(reader.ReadByteChars(4));
            result.Checksum = reader.ReadInt32();
            result.Offset = reader.ReadUInt32();
            result.Length = reader.ReadUInt32();
            return result;
        }

        /// <summary>
        /// Writes Table Record entry to binary writer.
        /// </summary>
        /// <param name="writer"></param>
        public void Write(BigEndianBinaryWriter writer)
        {
            for (int i = 0; i < Tag.Length; i++)
                writer.WriteByte((byte)Tag[i]);

            writer.WriteInt32(Checksum);
            writer.WriteUInt32(Offset);
            writer.WriteUInt32(Length);
        }

        /// <summary>
        /// Calculates checksum of a 'sfnt' table.
        /// </summary>
        public static int CalculateChecksum(byte[] data)
        {
            return CalculateChecksum(data, 0, data.Length);
        }

        /// <summary>
        /// Calculates checksum of a 'sfnt' table.
        /// </summary>
        public static int CalculateChecksum(byte[] data, int index, int count)
        {
            int sum = 0;

            int v0, v1, v2, v3;

            //Calculate the sum of the whole unsigned integers.
            int ptr = index;
            int wholeCount = count / 4;
            for (int i = 0; i < wholeCount; i++)
            {
                v3 = data[ptr++];
                v2 = data[ptr++];
                v1 = data[ptr++];
                v0 = data[ptr++];
                int value = v0 | (v1 << 8) | (v2 << 16) | (v3 << 24);
                sum += value;
            }

            //The stream length could be not a multiple of 4 bytes,
            //therefore up to 3 bytes might still be in the stream.
            v3 = (ptr < count) ? (int)data[ptr++] : 0;
            v2 = (ptr < count) ? (int)data[ptr++] : 0;
            v1 = (ptr < count) ? (int)data[ptr] : 0;
            v0 = 0;
            int lastValue = v0 | (v1 << 8) | (v2 << 16) | (v3 << 24);
            sum += lastValue;

            return sum;
        }

        /// <summary>
        /// 4-char table identifier.
        /// </summary>
        public string Tag;
        /// <summary>
        /// CheckSum for this table.
        /// </summary>
        public int Checksum;
        /// <summary>
        /// Offset from beginning of file.
        /// </summary>
        public uint Offset;
        /// <summary>
        /// Length of this table.
        /// </summary>
        public uint Length;

        /// <summary>
        /// Length of this structure on file.
        /// </summary>
        public const int StructureSize = 16;
    }
}
