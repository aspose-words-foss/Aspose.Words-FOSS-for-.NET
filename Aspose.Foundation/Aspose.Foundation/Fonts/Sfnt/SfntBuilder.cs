// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov

using System.Collections.Generic;
using System.IO;
using Aspose.Collections.Generic;
using Aspose.IO;

namespace Aspose.Fonts.Sfnt
{
    /// <summary>
    /// Responsible for building 'sfnt' files.
    /// </summary>
    /// <remarks>
    /// See http://www.microsoft.com/typography/otspec/otff.htm for more info about 'sfnt' format.
    /// 'sfnt' format is used in TrueType and OpenType fonts and also in MTX compression.
    /// </remarks>
    internal class SfntBuilder : IFontBuilder
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public SfntBuilder(int sfntVersion)
            : this()
        {
            mSfntVersion = sfntVersion;
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        public SfntBuilder()
        {
            mTables = new SortedStringListGeneric<byte[]>(true);
        }

        public int SfntVersion
        {
            get { return mSfntVersion; }
            set { mSfntVersion = value; }
        }

        /// <summary>
        /// Add table to file.
        /// </summary>
        public void AddTable(string tag, byte[] tableData)
        {
            mTables.Add(tag, tableData);
        }

        /// <summary>
        /// Writes result 'sfnt' file to byte array.
        /// </summary>
        public byte[] WriteFileToByteArray()
        {
            using(MemoryStream stream = new MemoryStream())
            {
                WriteFileToStream(stream);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Writes result 'sfnt' file to stream.
        /// </summary>
        public void WriteFileToStream(Stream stream)
        {
            BigEndianBinaryWriter writer = new BigEndianBinaryWriter(stream);

            SfntOffsetTable offsetTable = new SfntOffsetTable();
            offsetTable.Version = mSfntVersion;
            offsetTable.NumTables = (ushort)mTables.Count;
            offsetTable.Write(writer);

            uint currentTableOffset = (uint)(stream.Position + SfntTableRecordEntry.StructureSize * mTables.Count);
            long currentTableRecordOffset = stream.Position;

            // Specification says: Entries in the Table Record must be sorted in ascending order by tag.
            // To achieve that we will enumerate through SortedStringList.
            // Note: There is a recommendation that 'head' table data (not table record entry) should be placed first
            // in the file or at least before the 'OS/2' table. Ignore for now.
            foreach (KeyValuePair<string, byte[]> pair in mTables)
            {
                stream.Position = currentTableRecordOffset;
                byte[] tableData = pair.Value;
                SfntTableRecordEntry tableRecordEntry = new SfntTableRecordEntry();
                tableRecordEntry.Tag = pair.Key;
                tableRecordEntry.Length = (uint)tableData.Length;
                tableRecordEntry.Offset = currentTableOffset;
                tableRecordEntry.Checksum = SfntTableRecordEntry.CalculateChecksum(tableData);
                tableRecordEntry.Write(writer);
                currentTableRecordOffset = stream.Position;

                stream.Position = currentTableOffset;
                writer.WriteBytes(tableData, 0, tableData.Length);
                PadTable(writer);
                currentTableOffset = (uint)stream.Position;
            }
        }

        /// <summary>
        /// Pads the previously written table.
        /// </summary>
        /// <remarks>
        /// Specification says:
        /// A font is not considered structurally proper without the correct padding. All tables must begin on
        /// four byte boundaries, and any remaining space between tables is padded with zeros. The length of all tables
        /// should be recorded in the table record with their actual length (not their padded length).
        /// </remarks>
        private static void PadTable(BigEndianBinaryWriter writer)
        {
            StreamUtil.SeekToNextPage(writer.BaseStream, 4);
        }

        private int mSfntVersion = SfntOffsetTable.SfntVersion1;
        private readonly SortedStringListGeneric<byte[]> mTables;
    }
}
