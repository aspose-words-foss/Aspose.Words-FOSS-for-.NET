// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov

using System;
using System.IO;
using Aspose.Collections.Generic;
using Aspose.IO;

namespace Aspose.Fonts.Sfnt
{
    /// <summary>
    /// Responsible for reading 'sfnt' files.
    /// </summary>
    /// <remarks>
    /// See http://www.microsoft.com/typography/otspec/otff.htm for more info about 'sfnt' format.
    /// 'sfnt' format is used in TrueType and OpenType fonts and also in MTX compression.
    /// </remarks>
    internal class SfntReader
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public SfntReader(BigEndianBinaryReader reader)
        {
            mReader = reader;
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        public SfntReader(Stream stream)
        {
            mReader = new BigEndianBinaryReader(stream);
        }

        /// <summary>
        /// Reads 'sfnt' header from the current position in stream.
        /// </summary>
        public void ReadHeader()
        {
            if(!TryReadHeader())
                throw new InvalidOperationException("The sfnt file is not valid.");
        }

        /// <summary>
        /// Tries to read 'sfnt' header from the current position in stream.
        /// Returns true if header is valid and false otherwise.
        /// </summary>
        public bool TryReadHeader()
        {
            mOffsetTable = SfntOffsetTable.Read(mReader);
            if (!OffsetTable.IsValid)
                return false;

            mTableRecordEntries = new SortedStringListGeneric<SfntTableRecordEntry>();
            for (int i = 0; i < OffsetTable.NumTables; i++)
            {
                SfntTableRecordEntry tableRecordEntry = SfntTableRecordEntry.Read(mReader);
                TableRecordEntries.Add(tableRecordEntry.Tag, tableRecordEntry);
            }

            return true;
        }

        /// <summary>
        /// Sets the position of the reader stream to the beginning of the specified table.
        /// </summary>
        public void SeekToTable(string tableTag)
        {
            SfntTableRecordEntry tableRecordEntry = mTableRecordEntries.GetValueOrNull(tableTag);
            if(tableRecordEntry == null)
                throw new InvalidOperationException(string.Format("Cannot find table '{0}' in the font file.", tableTag));

            mReader.BaseStream.Position = tableRecordEntry.Offset;
        }

        /// <summary>
        /// Reads the specified table into an array of bytes.
        /// </summary>
        public byte[] ReadTable(string tableTag)
        {
            SeekToTable(tableTag);

            SfntTableRecordEntry tableRecordEntry = mTableRecordEntries.GetValueOrNull(tableTag);
            Debug.Assert(tableRecordEntry != null);
            return mReader.ReadBytes((int)tableRecordEntry.Length);
        }

        /// <summary>
        /// Binary reader.
        /// </summary>
        public BigEndianBinaryReader BinaryReader
        {
            get { return mReader; }
        }

        /// <summary>
        /// Base stream.
        /// </summary>
        public Stream BaseStream
        {
            get { return mReader.BaseStream; }
        }

        /// <summary>
        /// Offset Table.
        /// </summary>
        public SfntOffsetTable OffsetTable
        {
            get { return mOffsetTable; }
        }

        /// <summary>
        /// List of entries in Table Record.
        /// Key - table tag.
        /// Value - SfntTableRecordEntry instance.
        /// </summary>
        public SortedStringListGeneric<SfntTableRecordEntry> TableRecordEntries
        {
            get { return mTableRecordEntries; }
        }

        private readonly BigEndianBinaryReader mReader;
        private SfntOffsetTable mOffsetTable;
        private SortedStringListGeneric<SfntTableRecordEntry> mTableRecordEntries;
    }
}
