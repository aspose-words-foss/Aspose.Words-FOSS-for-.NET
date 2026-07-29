// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2012 by Konstantin Kornilov

using System;
using Aspose.Collections.Generic;
using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents 'cmap' format 12 subtable.
    /// </summary>
    internal class CmapSubtableFormat12 : CmapSubtable
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public CmapSubtableFormat12(int platformId, int encodingId, SortedIntegerListGeneric<int> charMap, int language)
            : base(platformId, encodingId, language, charMap)
        {
        }

        /// <summary>
        /// Reads subtable from binary reader.
        /// </summary>
        public static CmapSubtableFormat12 Read(BigEndianBinaryReader reader, CmapEncodingRecord encodingRecord)
        {
            reader.BaseStream.Position = encodingRecord.Position;
            int format = reader.ReadUInt16();
            Debug.Assert(format == Cmap.CmapFormat12);
            reader.ReadUInt16(); // reserved
            reader.ReadInt32();  // length
            int language = reader.ReadInt32();
            int nGroups = reader.ReadInt32();

            SortedIntegerListGeneric<int> charMap = new SortedIntegerListGeneric<int>();
            for (int i = 0; i < nGroups; i++)
            {
                int startCharCode = reader.ReadInt32();
                int endCharCode = reader.ReadInt32();
                int startGlyphId = reader.ReadInt32();

                if (startCharCode > endCharCode || startCharCode < 0 || startGlyphId < 0)
                    throw new InvalidOperationException("Invalid Format12 'cmap' table entry.");

                for (int charCode = startCharCode; charCode <= endCharCode; charCode++)
                    charMap[charCode] = startGlyphId++;
            }

            // Explicitly add mapping for missing glyph because it is not required to be in subtable.
            charMap[0xFFFF] = 0;

            return new CmapSubtableFormat12(encodingRecord.PlatformId, encodingRecord.EncodingId, charMap, language);
        }

        /// <summary>
        /// Writes subtable to binary writer.
        /// </summary>
        public override void Write(BigEndianBinaryWriter writer)
        {
            writer.WriteUInt16(Cmap.CmapFormat12); // format
            writer.WriteUInt16(0);  // reserved
            writer.WriteInt32(16 + 12 * CharMap.Count);
            writer.WriteInt32(Language);
            writer.WriteInt32(CharMap.Count);

            for (int i = 0; i < CharMap.Count; i++)
            {
                int charCode = CharMap.GetKey(i);
                int glyphId = CharMap.GetByIndex(i);

                writer.WriteInt32(charCode); // startCharCode
                writer.WriteInt32(charCode); // endCharCode
                writer.WriteInt32(glyphId); // startGlyphId
            }
        }
    }
}
