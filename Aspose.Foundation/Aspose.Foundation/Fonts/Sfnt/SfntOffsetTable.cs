// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov

using System;
using Aspose.IO;

namespace Aspose.Fonts.Sfnt
{
    /// <summary>
    /// Represents an 'Offset Table' inside the 'sfnt' file.
    /// </summary>
    internal class SfntOffsetTable
    {
        /// <summary>
        /// Reads 'Offset Table' from binary reader.
        /// </summary>
        public static SfntOffsetTable Read(BigEndianBinaryReader reader)
        {
            SfntOffsetTable result = new SfntOffsetTable();
            result.Version = reader.ReadInt32();
            result.NumTables = reader.ReadUInt16();
            result.SearchRange = reader.ReadUInt16();
            result.EntrySelector = reader.ReadUInt16();
            result.RangeShift = reader.ReadUInt16();
            return result;
        }

        /// <summary>
        /// Writes table to binary writer.
        /// </summary>
        public void Write(BigEndianBinaryWriter writer)
        {
            writer.WriteInt32(Version);
            writer.WriteUInt16(NumTables);

            int entrySelector = (int)Math.Floor(Math.Log(NumTables, 2));
            int searchRange = (int)Math.Pow(2, entrySelector) * 16;
            int rangeShift = NumTables * 16 - searchRange;

            writer.WriteUInt16(searchRange);
            writer.WriteUInt16(entrySelector);
            writer.WriteUInt16(rangeShift);
        }

        /// <summary>
        /// Version.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Number of tables in file.
        /// </summary>
        public ushort NumTables { get; set; }

        /// <summary>
        /// True when 'Offset Table' is valid.
        /// </summary>
        public bool IsValid
        {
            get { return ((Version == SfntVersion1 || Version == SfntVersionOtto) && NumTables > 0); }
        }

        internal ushort EntrySelector { get; private set; }
        internal ushort SearchRange { get; private set; }
        internal ushort RangeShift { get; private set; }


        public const int SfntVersion1 = 0x00010000;
        public const int SfntVersionOtto = 0x4f54544f;
    }
}
