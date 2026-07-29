// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/11/2022 by Nikolay Sezganov

using System.IO;
using Aspose.Common;
using Aspose.Fonts.Sfnt;
using Aspose.IO;

namespace Aspose.Fonts.Woff
{
    /// <summary>
    /// Converts WOFF font to SFNT format.
    /// https://www.w3.org/TR/WOFF/#OverallStructure
    /// </summary>
    public static class WoffConverter
    {
        public static byte[] Convert(Stream woffStream)
        {
            BigEndianBinaryReader woffBinaryReader = new BigEndianBinaryReader(woffStream);
            WoffHeader header = WoffHeader.Read(woffBinaryReader);
            if (header == null)
            {
                return null;
            }

            WoffTableDirectory[] woffTables = ReadTableDirectories(woffBinaryReader, header.NumTables);
            SfntBuilder sfntBuilder = new SfntBuilder();

            foreach (WoffTableDirectory table in woffTables)
            {
                woffBinaryReader.BaseStream.Seek(table.Offset, SeekOrigin.Begin);
                byte[] tableData = woffBinaryReader.ReadBytes((int)table.CompLength);
                if (table.IsCompressed)
                {
                    tableData = ZipUtilPal.Inflate(tableData, (int)table.OrigLength, ZipMethod.Zlib);
                    Debug.Assert(tableData.Length == table.OrigLength);
                }
                sfntBuilder.AddTable(table.Name, tableData);
            }

            return sfntBuilder.WriteFileToByteArray();
        }

        private static WoffTableDirectory[] ReadTableDirectories(
            BigEndianBinaryReader reader,
            int tableCount)
        {
            WoffTableDirectory[] tableDirectories = new WoffTableDirectory[tableCount];
            for (int i = 0; i < tableCount; ++i)
            {
                tableDirectories[i] = WoffTableDirectory.Read(reader);
            }
            return tableDirectories;
        }
    }
}
