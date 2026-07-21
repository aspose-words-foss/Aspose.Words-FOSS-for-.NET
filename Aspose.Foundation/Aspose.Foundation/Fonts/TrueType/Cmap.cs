// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/06/2009 by Roman Korchagin

using System;
using System.Collections.Generic;
using Aspose.Collections.Generic;
using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents 'cmap' table in a OpenType font.
    /// http://www.microsoft.com/typography/otspec/cmap.htm
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    internal abstract class Cmap : TTTable
    {
        protected Cmap(SortedIntegerListGeneric<int> charMap, int language)
        {
            CharMap = charMap;
            Language = language;
        }

        /// <summary>
        /// Reads 'cmap' table from binary reader.
        /// </summary>
        public static Cmap Read(BigEndianBinaryReader reader)
        {
            int version = reader.ReadUInt16();
            if (version != 0)
                throw new InvalidOperationException("Unexpected cmap table version.");
            int numTables = reader.ReadUInt16();

            CmapEncodingRecord[] records = ReadEncodingRecords(reader, numTables);
            return SelectCmapEncoding(reader, records);
        }

        private static CmapEncodingRecord[] ReadEncodingRecords(BigEndianBinaryReader reader, int numTables)
        {
            long tableStart = reader.BaseStream.Position - 4;
            CmapEncodingRecord[] result = new CmapEncodingRecord[numTables];

            for (int i = 0; i < numTables; i++)
            {
                int platformId = reader.ReadUInt16();
                int encodingId = reader.ReadUInt16();
                int offset = reader.ReadInt32();

                long currentPosition = reader.BaseStream.Position;
                long subtablePosition = tableStart + offset;
                reader.BaseStream.Position = subtablePosition;
                int format = reader.ReadUInt16();
                reader.BaseStream.Position = currentPosition;

                result[i] = new CmapEncodingRecord(platformId, encodingId, format, subtablePosition);
            }

            return result;
        }

        private static Cmap SelectCmapEncoding(BigEndianBinaryReader reader, CmapEncodingRecord[] encodingRecords)
        {
            // First try Windows UCS-4 encoding.
            CmapEncodingRecord record =
                TryGetEncodingRecord(encodingRecords, PlatformIdMicrosoft, MicrosoftEncodingIdUnicodeUcs4, CmapFormat12);
            if (record != null)
            {
                CmapSubtableFormat12 subtable = CmapSubtableFormat12.Read(reader, record);
                return new CmapWinUcs4(subtable.CharMap, subtable.Language);
            }

            // Then try Windows Symbol encoding.
            record = TryGetEncodingRecord(encodingRecords, PlatformIdMicrosoft, MicrosoftEncodingIdSymbol, CmapFormat4);
            if (record != null)
            {
                CmapSubtableFormat4 subtable = CmapSubtableFormat4.Read(reader, record);
                return new CmapUcs2(subtable.CharMap, subtable.Language, record.PlatformId, record.EncodingId);
            }

            // Then try Windows UCS-2 encoding.
            record = TryGetEncodingRecord(encodingRecords, PlatformIdMicrosoft, MicrosoftEncodingIdUnicodeUcs2, CmapFormat4);
            if (record != null)
            {
                CmapSubtableFormat4 subtable = CmapSubtableFormat4.Read(reader, record);
                return new CmapUcs2(subtable.CharMap, subtable.Language, record.PlatformId, record.EncodingId);
            }

            // WORDSNET-6283 Mac fonts in most cases contains encoding records for Mac and Unicode platforms.
            // Mac platform uses specific encodings which should be translated to Unicode. For example MacRomanEncoding.
            // Don't use it for now. Try to use Unicode platform.
            // kvk: Unicode encoding id = 1 should have special treatment for some Unicode ranges. Ignore it for now.
            // See https://developer.apple.com/fonts/ttrefman/rm06/Chap6name.html#ID.
            record = TryGetEncodingRecord(encodingRecords, PlatformIdUnicode, CmapFormat4);
            if (record != null)
            {
                // PDF validator do not like Unicode platform. Replace it with regular MS Unicode.
                CmapSubtableFormat4 subtable = CmapSubtableFormat4.Read(reader, record);
                return new CmapUcs2(subtable.CharMap, subtable.Language, PlatformIdMicrosoft, MicrosoftEncodingIdUnicodeUcs2);
            }

            // WORDSNET-12916 Customer is using fonts (probably obsolete) with only format 2 cmap and PRC encoding.
            record = TryGetEncodingRecord(encodingRecords, PlatformIdMicrosoft, 2);
            if (record != null)
            {
                CmapSubtableFormat2 subtable = CmapSubtableFormat2.Read(reader, record);
                // Do not write format 2 into subset. Replace it with regular format 4 subtable with Unicode encoding.
                return new CmapUcs2(subtable.CharMap, subtable.Language, PlatformIdMicrosoft, MicrosoftEncodingIdUnicodeUcs2);
            }

            throw new InvalidOperationException("Cannot find a required cmap encoding record.");
        }

        private static CmapEncodingRecord TryGetEncodingRecord(
            CmapEncodingRecord[] records, int platformId, int encodingId, int format)
        {
            foreach (CmapEncodingRecord record in records)
                if (record.PlatformId == platformId && record.EncodingId == encodingId && record.Format == format)
                    return record;

            return null;
        }

        private static CmapEncodingRecord TryGetEncodingRecord(CmapEncodingRecord[] records, int platformId, int format)
        {
            foreach (CmapEncodingRecord record in records)
                if (record.PlatformId == platformId && record.Format == format)
                    return record;

            return null;
        }

        /// <summary>
        /// Builds a <see cref="TTGlyphHashtable"/> from character map.
        /// </summary>
        public TTGlyphHashtable BuildGlyphs(HorizontalMetrics hMetrics, VerticalMetrics vMetrics, int numGlyphs)
        {
            TTGlyphHashtable result = new TTGlyphHashtable(numGlyphs);

            for (int i = 0; i < numGlyphs; i++)
            {
                HorizontalMetric hMetric = hMetrics.GetHMetric(i);

                TTGlyph glyph;
                if (vMetrics != null)
                {
                    VerticalMetric vMetric = vMetrics.GetVMetric(i);
                    glyph = new TTGlyph(i, hMetric.AdvanceWidth, hMetric.LeftSideBearing, vMetric.AdvanceHeight, vMetric.TopSideBearing);
                }
                else
                {
                    // Use advance width and left side bearing as advance height and top side bearing when the font contains no vertical metrics.
                    glyph = new TTGlyph(i, hMetric.AdvanceWidth, hMetric.LeftSideBearing, hMetric.AdvanceWidth, hMetric.LeftSideBearing);
                }
                result.AddGlyph(glyph);
            }

            result.MissingGlyph = result.GetGlyphByIndex(0);
            Debug.Assert(result.MissingGlyph != null);

            // Most of the font has a mapping from 0xFFFF to missing glyph. Add it explicitly to uniformly process the
            // missing glyph for all fonts.
            result.AddCharCodeMapping(0xFFFF, 0);

            for (int i = 0; i < CharMap.Count; i++)
            {
                int charCode = CharMap.GetKey(i);
                int glyphIndex = CharMap.GetByIndex(i);
                if(glyphIndex >= 0  && glyphIndex < numGlyphs)
                    result.AddCharCodeMapping(charCode, glyphIndex);
            }

            return result;
        }

        /// <summary>
        /// Creates a cmap that contains info for the specified characters and glyphs only.
        /// </summary>
        /// <param name="usedChars">
        /// Map of used characters. Key - character code, value - new glyph index.
        /// </param>
        public void Subset(SortedIntegerListGeneric<int> usedChars)
        {
            CharMap = usedChars;
        }

        internal override void Write(BigEndianBinaryWriter writer)
        {
            CmapSubtable[] subtables = BuildSubtables();

            // Spec says:
            // The header entries must be sorted first by platform ID, then by platform-specific encoding ID, and then by
            // the language field in the corresponding subtable.
            //
            // Currently we write only one or two subtables which are sorted in BuildSubtables method.
            List<byte[]> subtablesData = new List<byte[]>();

            for (int i = 0; i < subtables.Length; i++)
                subtablesData.Add(subtables[i].WriteToBytes());

            writer.WriteInt16(0);  // version
            writer.WriteInt16(subtables.Length);  // tableCount

            int currentSubtabelOffset = 4 + 8*subtables.Length;
            for (int i = 0; i < subtables.Length; i++)
            {
                // Write encoding table record.
                writer.WriteInt16(subtables[i].PlatformId);
                writer.WriteInt16(subtables[i].EncodingId);
                writer.WriteUInt32((uint)currentSubtabelOffset);
                currentSubtabelOffset += subtablesData[i].Length;
            }

            for (int i = 0; i < subtables.Length; i++)
                writer.WriteBytes(subtablesData[i]);
        }

        protected abstract CmapSubtable[] BuildSubtables();

        /// <summary>
        /// True if MicrosoftSymbolEncoding is specified.
        /// </summary>
        public abstract bool IsSymbolEncoding { get; }


        /// <summary>
        /// Reads and returns all encoding records used in the 'cmap'.
        /// Used for unit testing.
        /// </summary>
        internal static CmapEncodingRecord[] GetEncodingRecords(BigEndianBinaryReader reader)
        {
            int version = reader.ReadUInt16();
            if (version != 0)
                throw new InvalidOperationException("Unexpected cmap table version.");
            int numTables = reader.ReadUInt16();

            return ReadEncodingRecords(reader, numTables);
        }

        /// <summary>
        /// Sorted list of characters - glyph ids pairs.
        /// Key - int char code. Value - int glyph id.
        /// </summary>
        public SortedIntegerListGeneric<int> CharMap;
        protected readonly int Language;

        public const int CmapFormat4 = 4;
        public const int CmapFormat12 = 12;

        internal const int PlatformIdUnicode = 0;
        internal const int PlatformIdMacintosh = 1;
        internal const int PlatformIdMicrosoft = 3;

        internal const int MicrosoftEncodingIdSymbol = 0;
        internal const int MicrosoftEncodingIdUnicodeUcs2 = 1;
        internal const int MicrosoftEncodingIdPrc = 3;
        internal const int MicrosoftEncodingIdBig5 = 4;
        internal const int MicrosoftEncodingIdUnicodeUcs4 = 10;

        internal const int MacintoshEncodingIdRoman = 0;
        internal const int MacintoshEncodingIdJapanese = 1;
        internal const int MacintoshEncodingIdTraditionalChinese = 2;
        internal const int MacintoshEncodingIdKorean = 3;
        internal const int MacintoshEncodingIdSimplifiedChinese = 25;

        internal const int MicrosoftLanguageIdEnglishUs = 0x0409;

        internal const int MacintoshLanguageIdEnglish = 0;
    }
}
