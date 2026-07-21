// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2012 by Konstantin Kornilov

using System;
using Aspose.Bidi;
using Aspose.Collections.Generic;
using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents 'cmap' format 4 subtable.
    /// </summary>
    internal class CmapSubtableFormat4 : CmapSubtable
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public CmapSubtableFormat4(int platformId, int encodingId, SortedIntegerListGeneric<int> charMap, int language)
            : base(platformId, encodingId, language, FilterSupplementaryPlanesChars(charMap))
        {
        }

        private static SortedIntegerListGeneric<int> FilterSupplementaryPlanesChars(SortedIntegerListGeneric<int> charMap)
        {
            SortedIntegerListGeneric<int> result = new SortedIntegerListGeneric<int>();
            for (int i = 0; i < charMap.Count; i++)
            {
                int charCode = charMap.GetKey(i);

                // charMap is sorted so we could stop when find first supplementary plane char.
                if (UnicodeUtil.IsSupplementaryPlanesCharacter(charCode))
                    break;

                result.Add(charCode, charMap.GetByIndex(i));
            }
            return result;
        }

        /// <summary>
        /// Reads subtable from binary reader.
        /// </summary>
        public static CmapSubtableFormat4 Read(BigEndianBinaryReader reader, CmapEncodingRecord encodingRecord)
        {
            SortedIntegerListGeneric<int> charMap = new SortedIntegerListGeneric<int>();

            reader.BaseStream.Position = encodingRecord.Position;
            int format = reader.ReadUInt16();
            Debug.Assert(format == Cmap.CmapFormat4);

            int length = reader.ReadUInt16();
            int language = reader.ReadUInt16();

            int segCountX2 = reader.ReadUInt16();
            reader.ReadUInt16(); // searchRange
            reader.ReadUInt16(); // entrySelector
            reader.ReadUInt16(); // rangeShift

            int segCount = segCountX2 / 2;

            int[] endCodes = ReadUInt16Array(reader, segCount);       //End characterCode for each segment, last=0xFFFF.
            reader.ReadUInt16();                                //Pad
            int[] startCodes = ReadUInt16Array(reader, segCount);     //Start character code for each segment.
            int[] idDeltas = ReadInt16Array(reader, segCount);        //Delta for all character codes in segment.
            int[] idRangeOffsets = ReadUInt16Array(reader, segCount); //Offsets into glyphIdArray or 0

            int tableEnd = (int)encodingRecord.Position + length;
            int remainingBytes = tableEnd - (int)reader.BaseStream.Position;
            // Glyph IDs array is located in the end of the subtable. Up to this moment the
            // position of pointer in the stream points to the beginning of this array.
            // Thus glyphIdCount can be calculated as (number of unread bytes in subtable)/(2 bytes per ID)
            // I'm not 100% sure this count is correct, but looks okay.
            int glyphIdCount = remainingBytes / 2;

            // WORDSNET-13043 Resilience. MingLiu font contains glyph ID indexes 22103 and 22104 while the glyphIdCount = 22103.
            // Meanwhile the font contains both glyph IDs and glyphs itself, so the most probably subtable's length value 
            // is incorrect.
            const int extraBytesNumber = 4;
            if (tableEnd + extraBytesNumber <= reader.BaseStream.Length)
                glyphIdCount += extraBytesNumber / 2;

            int[] glyphIds = ReadUInt16Array(reader, glyphIdCount);

            for (int segIdx = 0; segIdx < segCount; segIdx++)
            {
                // Loop through all character codes in the segment.
                for (int charCode = startCodes[segIdx]; charCode <= endCodes[segIdx]; charCode++)
                {
                    int glyphIndex;

                    // WORDSNET-9205, 12852 - The problem occurred because fonts cmap has invalid mapping for the latest charCode.
                    // Due to TTF specification - For the search to terminate, the final endCode value must be 0xFFFF.
                    // This segment need not contain any valid mappings. 
                    // (It can just map the single character code 0xFFFF to missingGlyph).
                    // To fix the problem just map 65535 char to the missing glyph.
                    if (charCode == 65535)
                    {
                        glyphIndex = 0;
                    }
                    else
                    {
                        int idRangeOffset = idRangeOffsets[segIdx];
                        switch (idRangeOffset)
                        {
                            case 0:
                                {
                                    // If the idRangeOffset is 0, the idDelta value is added directly to the
                                    // character code offset (i.e. idDelta[i] + c) to get the corresponding glyph index.
                                    // The idDelta arithmetic is modulo 65536.
                                    glyphIndex = (charCode + idDeltas[segIdx]) & 0xFFFF;

                                    // This is to fix the problem similar to 7531 (see comment below)
                                    if (glyphIndex == 65535)
                                    {
                                        glyphIndex = 0;
                                    }
                                    break;
                                }
                            case 65535:
                                {
                                    // WORDSNET-7531 RESILIENCY There seems to be this value in the file.
                                    // It is for the last char and I think it is for the "missing glyph"
                                    glyphIndex = 0;
                                    break;
                                }
                            default:
                                {
                                    // If the idRangeOffset value for the segment is not 0, the mapping of character codes
                                    // relies on glyphIdArray.
                                    // The character code offset from startCode is added to the idRangeOffset value.
                                    // This sum is used as an offset from the current location within
                                    // idRangeOffset itself to index out the correct glyphIdArray value.

                                    // original C expression: 
                                    // *(idRangeOffset[segIdx] / 2 +            this is idRangeOffset value, why divided?
                                    //  (charCode - startCount[segIdx]) +       this is character code offset
                                    //  &idRangeOffset[segIdx])                 current location within isRangeOffset itself

                                    int charCodeOffset = charCode - startCodes[segIdx];

                                    // Calculate index into the glyphId array.
                                    int glyphIdIdx =
                                        idRangeOffsets[segIdx] / 2          // the idRangeOffsetValue
                                        + charCodeOffset                    // character code offset
                                        - segCount                          // probably helps to come down from pointer arithmetic
                                        + segIdx;                           // just use index instead of pointer arithmetic

                                    // If the value obtained from the indexing operation is not 0 (which indicates missingGlyph),
                                    // idDelta[i] is added to it to get the glyph index. The idDelta arithmetic is modulo 65536.
                                    glyphIndex = glyphIds[glyphIdIdx];
                                    if (glyphIndex != 0)
                                        glyphIndex = (glyphIndex + idDeltas[segIdx]) & 0xFFFF;

                                    break;
                                }
                        }
                    }

                    charMap[charCode] = glyphIndex;
                }
            }

            return new CmapSubtableFormat4(encodingRecord.PlatformId, encodingRecord.EncodingId, charMap, language);
        }

        private static int[] ReadUInt16Array(BigEndianBinaryReader reader, int count)
        {
            int[] result = new int[count];
            for (int i = 0; i < count; i++)
                result[i] = reader.ReadUInt16();
            return result;
        }

        private static int[] ReadInt16Array(BigEndianBinaryReader reader, int count)
        {
            int[] result = new int[count];
            for (int i = 0; i < count; i++)
                result[i] = reader.ReadInt16();
            return result;
        }

        /// <summary>
        /// Writes subtable to binary writer.
        /// </summary>
        public override void Write(BigEndianBinaryWriter writer)
        {
            long tableStart = writer.BaseStream.Position;

            writer.WriteInt16(Cmap.CmapFormat4);

            // Last segment should always map 0xFFFF char to 0 index.
            const int lastCharCode = 0xFFFF;
            const int lastGlyphIndex = 0;

            // From the SPEC: The final endCode value must be 0xFFFF. This segment need not contain
            // any valid mappings. (It can just map the single character code 0xFFFF to missingGlyph).
            // However, the segment must be present.
            // So add this segment even if character 0xFFFF is not in usedChars map.
            bool additionalSegmentNeeded = !CharMap.ContainsKey(lastCharCode);

            // These calculations are copied from the TTF spec. I have no idea what they are really for.
            int segCount = CharMap.Count + (additionalSegmentNeeded ? 1 : 0);
            int segCountX2 = segCount * 2;
            int searchRange = 2 << (int)Math.Floor(Math.Log(segCount) / Math.Log(2));  // 2 ^ largest power of 2 <= segCount
            int entrySelector = (int)(Math.Log((double)searchRange / 2) / Math.Log(2));
            int rangeShift = 2 * segCount - searchRange;

            // This writes the table in a very simplistic way: one segment for each character.
            // We are using the following formula: If the idRangeOffset is 0, the idDelta value
            // is added directly to the character code offset (i.e. idDelta[i] + c) to get the 3
            // corresponding glyph index.
            int[] endCodes = new int[segCount];
            int[] startCodes = new int[segCount];
            int[] idDeltas = new int[segCount];
            int[] idRangeOffsets = new int[segCount];
            for (int i = 0; i < CharMap.Count; i++)
            {
                int charCode = CharMap.GetKey(i);
                int newGlyphIndex = CharMap.GetByIndex(i);

                endCodes[i] = charCode;
                startCodes[i] = charCode;
                idDeltas[i] = newGlyphIndex - charCode;
            }

            // Last segment should always map 0xFFFF char to 0 index.
            if (additionalSegmentNeeded)
            {
                endCodes[segCount - 1] = lastCharCode;
                startCodes[segCount - 1] = lastCharCode;
                idDeltas[segCount - 1] = lastGlyphIndex - lastCharCode;
            }

            int length =
                8 * 2 +
                segCountX2 * 4;

            writer.WriteInt16(length);
            writer.WriteInt16(Language);

            writer.WriteInt16(segCountX2);
            writer.WriteInt16(searchRange);
            writer.WriteInt16(entrySelector);
            writer.WriteInt16(rangeShift);

            WriteInt16Array(endCodes, writer);
            writer.WriteInt16(0);
            WriteInt16Array(startCodes, writer);
            WriteInt16Array(idDeltas, writer);
            WriteInt16Array(idRangeOffsets, writer);

            Debug.Assert((writer.BaseStream.Position - tableStart) == length);
        }

        private static void WriteInt16Array(int[] data, BigEndianBinaryWriter writer)
        {
            foreach (int v in data)
                writer.WriteInt16(v);
        }
    }
}
