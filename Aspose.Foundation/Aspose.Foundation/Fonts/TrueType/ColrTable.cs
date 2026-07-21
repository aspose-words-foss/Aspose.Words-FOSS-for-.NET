// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/11/2021 by Konstantin Kornilov

using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Handles 'COLR' OpenType table.
    /// </summary>
    internal class ColrTable
    {
        public ColrTable(BigEndianBinaryReader reader)
        {
            long tableStartPosition = reader.BaseStream.Position;
            reader.ReadUInt16(); // version
            int numGlyphRecords = reader.ReadInt16();
            int glyphRecordsOffset = reader.ReadInt32();
            int layerRecordsOffset = reader.ReadInt32();
            int numLayerRecords = reader.ReadUInt16();

            reader.BaseStream.Position = tableStartPosition + glyphRecordsOffset;
            GlyphRecords = new ColrBaseGlyphRecord[numGlyphRecords];
            for (int i = 0; i < numGlyphRecords; i++)
            {
                ColrBaseGlyphRecord record = new ColrBaseGlyphRecord();
                record.GlyphId = reader.ReadUInt16();
                record.FirstLayerIndex = reader.ReadUInt16();
                record.NumLayers = reader.ReadUInt16();
                GlyphRecords[i] = record;
            }

            reader.BaseStream.Position = tableStartPosition + layerRecordsOffset;
            LayerRecords = new ColrLayerRecord[numLayerRecords];
            for (int i = 0; i < numLayerRecords; i++)
            {
                ColrLayerRecord record = new ColrLayerRecord();
                record.GlyphId = reader.ReadUInt16();
                record.PaletteIndex = reader.ReadUInt16();
                LayerRecords[i] = record;
            }
        }

        public ColrBaseGlyphRecord[] GlyphRecords { get; }
        public ColrLayerRecord[] LayerRecords { get; }
    }

    /// <summary>
    /// Represents "Base Glyph Record" in "COLR" table.
    /// </summary>
    internal class ColrBaseGlyphRecord
    {
        public int GlyphId;
        public int FirstLayerIndex;
        public int NumLayers;
    }

    /// <summary>
    /// Represents "Layer Record" in "COLR" table.
    /// </summary>
    internal class ColrLayerRecord
    {
        public int GlyphId;
        public int PaletteIndex;
    }
}
