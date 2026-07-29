// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/11/2022 by Nikolay Sezganov

using Aspose.IO;

namespace Aspose.Fonts.Woff
{
    /// <summary>
    /// File header with basic font type and version, along with offsets to metadata and private data blocks.
    /// https://www.w3.org/TR/WOFF/#OverallStructure
    /// </summary>
    internal class WoffHeader
    {
        internal WoffHeader(int numTables)
        {
            NumTables = numTables;
        }

        internal static WoffHeader Read(BigEndianBinaryReader reader)
        {
            uint signature = reader.ReadUInt32(); // signature - 0x774F4646 'wOFF'
            if (signature != 0x774F4646)
            {
                return null;
            }

            reader.ReadUInt32(); // flavor - The "sfnt version" of the input font.
            reader.ReadUInt32(); // length - Total size of the WOFF file.
            int numTables = reader.ReadUInt16(); // numTables - Number of entries in directory of font tables.
            reader.ReadUInt16(); // reserved - Reserved; set to zero.
            reader.ReadUInt32(); // totalSfntSize - Total size needed for the uncompressed font data, including the sfnt header,
                                 // directory, and font tables(including padding).
            reader.ReadUInt16(); // majorVersion - Major version of the WOFF file.
            reader.ReadUInt16(); // minorVersion - Minor version of the WOFF file.
            reader.ReadUInt32(); // metaOffset - Offset to metadata block, from beginning of WOFF file.
            reader.ReadUInt32(); // metaLength - Length of compressed metadata block.
            reader.ReadUInt32(); // metaOrigLength - Uncompressed size of metadata block.
            reader.ReadUInt32(); // privOffset - Offset to private data block, from beginning of WOFF file.
            reader.ReadUInt32(); // privLength - Length of private data block.

            return new WoffHeader(numTables);
        }

        /// <summary>
        /// Number of entries in directory of font tables.
        /// </summary>
        internal int NumTables { get; }
    }
}
