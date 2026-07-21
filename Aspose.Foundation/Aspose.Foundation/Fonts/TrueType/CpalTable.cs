// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/11/2021 by Konstantin Kornilov

using Aspose.Drawing;
using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Handles 'CPAL' OpenType table.
    /// </summary>
    internal class CpalTable
    {
        public CpalTable(BigEndianBinaryReader reader)
        {
            long tableStartPosition = reader.BaseStream.Position;
            reader.ReadUInt16(); // version
            int numPaletteEntries = reader.ReadUInt16();
            reader.ReadUInt16(); // numPalettes
            reader.ReadUInt16(); // numColorRecords
            int colorRecordsArrayOffset = reader.ReadInt32();

            // For now read only first palette.
            int palette0ColorRecordIndex = reader.ReadUInt16();
            reader.BaseStream.Position = tableStartPosition + colorRecordsArrayOffset + palette0ColorRecordIndex * 4;
            PaletteColors = new DrColor[numPaletteEntries];
            for (int i = 0; i < numPaletteEntries; i++)
            {
                byte b = reader.ReadByte();
                byte g = reader.ReadByte();
                byte r = reader.ReadByte();
                byte a = reader.ReadByte();
                PaletteColors[i] = new DrColor(a, r, g, b);
            }
        }

        public DrColor[] PaletteColors { get; }
    }
}
