// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/09/2005 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Fonts.Sfnt;
using Aspose.Fonts.Ttc;
using Aspose.JavaAttributes;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Responsible for reading and subsetting of true type font files.
    /// See http://www.microsoft.com/typography/otspec/otff.htm for more info.
    /// </summary>
    public class TTFontFiler
    {

        /// <summary>
        /// Tries to read a single TTF table directory entries from the font file into a hashtable.
        /// </summary>
        private bool TryReadTtfTableDirectoryEntries(long offset)
        {
            // If file is not in TTF or OTF format then it should be handled fast.
            // Throwing and catching exceptions will decrease performance.
            mSfntReader.BaseStream.Position = offset;
            if (!mSfntReader.TryReadHeader())
                return false;

            // Check TrueType tables.
            if (mSfntReader.TableRecordEntries.ContainsKey(OpenTypeTableTag.Glyf) &&
                mSfntReader.TableRecordEntries.ContainsKey(OpenTypeTableTag.Loca))
                return true;

            // Check CFF tables.
            if (mSfntReader.TableRecordEntries.ContainsKey(OpenTypeTableTag.Cff))
                return true;

            // There also may be bitmap fonts which we do not support for now.
            return false;
        }

        /// <summary>
        /// Partially parses the font data and adds an <see cref="FontSearchInfo"/> instance for each font in the data
        /// to the specified list. Does not add anything if the font data is not recognized as a valid font file.
        /// </summary>
        [JavaThrows(false)]
        public void ExtractFontSearchInfo(IList<FontSearchInfo> list, IFontData fontData, int sourcePriority)
        {
            try
            {
                using (Stream stream = fontData.OpenStream())
                {
                    mSfntReader = new SfntReader(stream);
                    TtcReader ttcReader = new TtcReader(mSfntReader.BinaryReader);

                    if (ttcReader.TryReadHeader())
                    {
                        ExtractFontSearchInfoFromTtc(ttcReader, list, fontData, sourcePriority);
                    }
                    else if (TryReadTtfTableDirectoryEntries(0))
                    {
                        list.Add(ReadFontSearchInfo(new PhysicalFontData(fontData), sourcePriority));
                    }
                }
            }
            catch
            {
                // Exception could be thrown if font data is in invalid format. Do nothing so this font data
                // will be ignored later.
            }
        }

        private FontSearchInfo ReadFontSearchInfo(PhysicalFontData fontData, int priority)
        {
            mFontHeader = ReadFontHeader();
            ReadNames();
            mMetrics = ReadFontMetrics();

            bool isCff = mSfntReader.TableRecordEntries.ContainsKey(OpenTypeTableTag.Cff);
            FontStyle style = TTFontBuilder.SelectFontStyle(mMetrics, mFontHeader, mNames.FamilyName);
            string fullFontName = mNames.FullFontName;
            string fontFamilyName = mNames.FamilyName;

            SortedStringListGeneric<string> fontFullNames = new SortedStringListGeneric<string>(false);
            foreach (string name in mNames.FullFontNamesAllLanguages)
                fontFullNames[name] = name;

            SortedStringListGeneric<string> fontFamilyNames = new SortedStringListGeneric<string>(false);
            foreach (string name in mNames.FamilyNamesAllLanguages)
                fontFamilyNames[name] = name;

            return new FontSearchInfo(fontData, fullFontName, fontFamilyName,
                fontFullNames.Keys, fontFamilyNames.Keys, style, isCff, priority, mNames.VersionString,
                mMetrics.panose, mMetrics.ulUnicodeRanges, mMetrics.ulCodePageRanges, mMetrics.fsType);
        }

        private void ExtractFontSearchInfoFromTtc(TtcReader ttcReader, IList<FontSearchInfo> list, IFontData fontData, int priority)
        {
            for (int i = 0; i < ttcReader.NumberOfFonts; i++)
            {
                ttcReader.SeekToFont(i);
                mSfntReader.ReadHeader();
                ReadNames();
                list.Add(ReadFontSearchInfo(new TtcPhysicalFontData(fontData, i, mNames.FullFontName), priority));
            }
        }

        /// <summary>
        /// Extracts specified font from a TTC collection and writes it to <paramref name="dstStream"/>.
        /// </summary>
        internal static void ExtractFontFromTtc(TTFont font, Stream dstStream)
        {
            using (Stream srcStream = font.Data.OpenStream())
            {
                OpenTypeReader reader = OpenTypeReader.Create(srcStream, font.PhysicalData);
                reader.ReadHeader();

                // Copy all font tables from TTC file to builder.
                SfntBuilder builder = new SfntBuilder(reader.SfntReader.OffsetTable.Version);
                foreach (string tag in reader.SfntReader.TableRecordEntries.Keys)
                    builder.AddTable(tag, reader.SfntReader.ReadTable(tag));

                builder.WriteFileToStream(dstStream);
            }
        }

        /// <summary>
        /// See http://www.microsoft.com/typography/otspec/head.htm for more info.
        /// </summary>
        private FontHeader ReadFontHeader()
        {
            mSfntReader.SeekToTable(OpenTypeTableTag.Head);
            return FontHeader.Read(mSfntReader.BinaryReader);
        }

        /// <summary>
        /// See http://www.microsoft.com/typography/otspec/name.htm for more info.
        /// </summary>
        [JavaThrows(true)]
        private void ReadNames()
        {
            mSfntReader.SeekToTable(OpenTypeTableTag.Name);
            mNames = TTFontNames.Read(mSfntReader.BinaryReader);
        }

        private FontMetrics ReadFontMetrics()
        {
            mSfntReader.SeekToTable(OpenTypeTableTag.Os2);
            return new FontMetrics(mSfntReader.BinaryReader);
        }

        /// <summary>
        /// Joins subsets embedded to MW document.
        /// </summary>
        /// <remarks>
        /// This method relies on some features of MW font embedding. So it may be required to review this method
        /// if use in other cases.
        /// </remarks>
        [JavaConvertCheckedExceptions]
        public static TTFont JoinEmbeddedSubsets(IList<TTFont> toJoin)
        {
            if (toJoin == null || toJoin.Count == 0)
                return null;

            if (toJoin.Count == 1)
                return toJoin[0];

            // Note: In most cases MW only clears the unneeded glyphs data. But in rare cases MW also
            // clears cmap, hmtx so we should join them too.
            // Note: At the moment cmap writing code is not optimal and large cmap's could cause overflow. Try to
            // use already encoded cmap as a base if nothing is joined into it.
            // Note: AW at the moment only clears the glyph data when subsetting and embedding to DOCX. In some older
            // versions AW used compact subsets but joining such subsets is not supported here.
            IFontBuilder builder = new SfntBuilder();
            FontHeader joinedHead;
            FontMetrics joinedOs2;
            HorizontalHeader joinedHhea;
            MaximumProfile joinedMaxp;
            Cmap joinedCmap;
            List<HorizontalMetric> joinedMetrics;
            short[] joinedLsb;
            IntToObjDictionary<byte[]> glyphData;
            string[] tagsToJoin = new[]
            {
                OpenTypeTableTag.Head, OpenTypeTableTag.Os2, OpenTypeTableTag.Hhea, OpenTypeTableTag.Maxp,
                OpenTypeTableTag.Cmap, OpenTypeTableTag.Hmtx, OpenTypeTableTag.Glyf, OpenTypeTableTag.Loca
            };
            bool useUnparsedCmap = true;
            byte[] unparsedCmapData;
            using (Stream baseSubsetData = toJoin[0].Data.OpenStream())
            {
                SfntReader reader = new SfntReader(baseSubsetData);
                reader.ReadHeader();
                builder.SfntVersion = reader.OffsetTable.Version;
                reader.SeekToTable(OpenTypeTableTag.Maxp);
                joinedMaxp = new MaximumProfile(reader.BinaryReader);
                reader.SeekToTable(OpenTypeTableTag.Head);
                joinedHead = FontHeader.Read(reader.BinaryReader);
                reader.SeekToTable(OpenTypeTableTag.Os2);
                joinedOs2 = new FontMetrics(reader.BinaryReader);
                reader.SeekToTable(OpenTypeTableTag.Hhea);
                joinedHhea = new HorizontalHeader(reader.BinaryReader);

                unparsedCmapData = reader.ReadTable(OpenTypeTableTag.Cmap);
                reader.SeekToTable(OpenTypeTableTag.Cmap);
                joinedCmap = Cmap.Read(reader.BinaryReader);

                reader.SeekToTable(OpenTypeTableTag.Hmtx);
                HorizontalMetrics hmtx = HorizontalMetrics.Read(
                    reader.BinaryReader,
                    joinedHhea.NumberOfHMetrics,
                    joinedMaxp.NumGlyphs);
                joinedMetrics = new List<HorizontalMetric>(hmtx.WidthPairs);
                joinedLsb = hmtx.LeftSideBearings;
                glyphData = new IntToObjDictionary<byte[]>(joinedMaxp.NumGlyphs);
                JoinGlyf(reader, glyphData);

                foreach (SfntTableRecordEntry entry in reader.TableRecordEntries.Values)
                    if (!ContainsTag(tagsToJoin, entry.Tag))
                        builder.AddTable(entry.Tag, reader.ReadTable(entry.Tag));
            }

            for (int subsetNumber = 1; subsetNumber < toJoin.Count; subsetNumber++)
            {
                TTFont subset = toJoin[subsetNumber];
                using (Stream stream = subset.Data.OpenStream())
                {
                    SfntReader reader = new SfntReader(stream);
                    reader.ReadHeader();
                    JoinOs2(joinedOs2, reader);
                    JoinMaxp(joinedMaxp, reader);
                    JoinGlyf(reader, glyphData);

                    // Note: Sometimes MW writes full cmap into one subset (mostly regular style) and truncated cmaps
                    // into other subsets. Take the largest cmap as a base cmap and join others into it.
                    reader.SeekToTable(OpenTypeTableTag.Cmap);
                    Cmap newCmap = Cmap.Read(reader.BinaryReader);
                    if (newCmap.CharMap.Count > joinedCmap.CharMap.Count)
                    {
                        unparsedCmapData = reader.ReadTable(OpenTypeTableTag.Cmap);
                        Cmap temp = joinedCmap;
                        joinedCmap = newCmap;
                        newCmap = temp;
                        useUnparsedCmap = true;
                    }

                    if (JoinCmap(newCmap, joinedCmap))
                        useUnparsedCmap = false;

                    reader.SeekToTable(OpenTypeTableTag.Hhea);
                    HorizontalHeader hhea = new HorizontalHeader(reader.BinaryReader);
                    reader.SeekToTable(OpenTypeTableTag.Hmtx);
                    HorizontalMetrics hmtx = HorizontalMetrics.Read(
                        reader.BinaryReader,
                        hhea.NumberOfHMetrics,
                        joinedMaxp.NumGlyphs);
                    if (hhea.NumberOfHMetrics > joinedMetrics.Count)
                    {
                        for (int i = joinedMetrics.Count; i < hhea.NumberOfHMetrics; i++)
                            joinedMetrics.Add(hmtx.WidthPairs[i]);
                        joinedLsb = hmtx.LeftSideBearings;
                    }

                    for (int i = 0; i < hmtx.WidthPairs.Length; i++)
                        joinedMetrics[i] = ChooseHMetric(joinedMetrics[i], hmtx.WidthPairs[i]);
                }
            }

            GlyfTableBuilder glyfBuilder = new GlyfTableBuilder(joinedHead.IsLocaShort);
            for (int i = 0; i < joinedMaxp.NumGlyphs; i++)
            {
                if (!glyphData.ContainsKey(i))
                {
                    glyfBuilder.WriteEmptyGlyph();
                    continue;
                }

                glyfBuilder.WriteGlyph(glyphData[i]);
            }

            glyfBuilder.EndTable();
            glyfBuilder.LocaTable.ChooseFormat();
            glyfBuilder.BuildLocaStream();

            joinedHead.IsLocaShort = glyfBuilder.LocaTable.IsLocaShort;
            joinedHhea.NumberOfHMetrics = (ushort)joinedMetrics.Count;
            HorizontalMetrics joinedHmtx = new HorizontalMetrics(joinedMetrics.ToArray(), joinedLsb);
            builder.AddTable(OpenTypeTableTag.Head, joinedHead.ToByteArray());
            builder.AddTable(OpenTypeTableTag.Os2, joinedOs2.ToByteArray());
            builder.AddTable(OpenTypeTableTag.Maxp, joinedMaxp.ToByteArray());
            builder.AddTable(OpenTypeTableTag.Glyf, glyfBuilder.GlyfStream.ToArray());
            builder.AddTable(OpenTypeTableTag.Loca, glyfBuilder.LocaStream.ToArray());
            builder.AddTable(OpenTypeTableTag.Hhea, joinedHhea.ToByteArray());
            builder.AddTable(
                OpenTypeTableTag.Cmap,
                useUnparsedCmap
                    ? unparsedCmapData
                    : joinedCmap.ToByteArray());
            builder.AddTable(OpenTypeTableTag.Hmtx, joinedHmtx.ToByteArray());

            byte[] joinedData = builder.WriteFileToByteArray();
            bool isEmbedded = toJoin[0].IsEmbedded;
            return TTFontBuilder.Read(new PhysicalFontData(new MemoryFontData(joinedData, null, isEmbedded)));
        }

        private static bool JoinCmap(Cmap newCmap, Cmap joinedCmap)
        {
            bool newEntriesAdded = false;
            for (int i = 0; i < newCmap.CharMap.Count; i++)
            {
                int charCode = newCmap.CharMap.GetKey(i);
                int glyphIndex = newCmap.CharMap.GetByIndex(i);
                if (!joinedCmap.CharMap.ContainsKey(charCode))
                {
                    joinedCmap.CharMap[charCode] = glyphIndex;
                    newEntriesAdded = true;
                }
            }

            return newEntriesAdded;
        }

        private static void JoinGlyf(SfntReader reader, IntToObjDictionary<byte[]> glyphData)
        {
            GlyfTableReader glyfReader = InitGlyfTableReader(reader);
            for (int i = 0; i < glyfReader.GlyphCount; i++)
                if (glyfReader.GetGlyphLength(i) > 0 && !glyphData.ContainsKey(i))
                    glyphData[i] = glyfReader.GetGlyphBytes(i);
        }

        private static bool ContainsTag(string[] tagsToWrite, string tag)
        {
            foreach (string tagToWrite in tagsToWrite)
                if (tag.Equals(tagToWrite, StringComparison.Ordinal))
                    return true;
            return false;
        }

        private static void JoinOs2(FontMetrics joined, SfntReader reader)
        {
            reader.SeekToTable(OpenTypeTableTag.Os2);
            FontMetrics os2 = new FontMetrics(reader.BinaryReader);
            joined.usLastCharIndex = UshortMax(joined.usLastCharIndex, os2.usLastCharIndex);
        }

        private static void JoinMaxp(MaximumProfile joined, SfntReader reader)
        {
            reader.SeekToTable(OpenTypeTableTag.Maxp);
            MaximumProfile maxp = new MaximumProfile(reader.BinaryReader);
            joined.MaxContours = UshortMax(joined.MaxContours, maxp.MaxContours);
            joined.MaxPoints = UshortMax(joined.MaxPoints, maxp.MaxPoints);
            joined.MaxStorage = UshortMax(joined.MaxStorage, maxp.MaxStorage);
            joined.MaxZones = UshortMax(joined.MaxZones, maxp.MaxZones);
            joined.MaxComponentDepth = UshortMax(joined.MaxComponentDepth, maxp.MaxComponentDepth);
            joined.MaxComponentElements = UshortMax(joined.MaxComponentElements, maxp.MaxComponentElements);
            joined.MaxCompositeContours = UshortMax(joined.MaxCompositeContours, maxp.MaxCompositeContours);
            joined.MaxCompositePoints = UshortMax(joined.MaxCompositePoints, maxp.MaxCompositePoints);
            joined.MaxFunctionDefs = UshortMax(joined.MaxFunctionDefs, maxp.MaxFunctionDefs);
            joined.MaxInstructionDefs = UshortMax(joined.MaxInstructionDefs, maxp.MaxInstructionDefs);
            joined.MaxStackElements = UshortMax(joined.MaxStackElements, maxp.MaxStackElements);
            joined.MaxTwilightPoints = UshortMax(joined.MaxTwilightPoints, maxp.MaxTwilightPoints);
            joined.MaxSizeOfInstructions = UshortMax(joined.MaxSizeOfInstructions, maxp.MaxSizeOfInstructions);
        }

        private static ushort UshortMax(ushort val1, ushort val2)
        {
            // Java doesn't have unsigned short value and need some conversion to compare and save max value
            return (ushort)Math.Max((int)val1, (int)val2);
        }

        private static HorizontalMetric ChooseHMetric(HorizontalMetric m1, HorizontalMetric m2)
        {
            // Note: Ideally, we should check the presence of the glyph in cmap or glyph table. But probably
            // it is OK just to check if advance is not zero.
            if (m2.AdvanceWidth != 0 || m2.LeftSideBearing != 0)
                return m2;
            return m1;
        }

        private static GlyfTableReader InitGlyfTableReader(SfntReader reader)
        {
            reader.SeekToTable(OpenTypeTableTag.Head);
            FontHeader fontHeader = FontHeader.Read(reader.BinaryReader);

            SfntTableRecordEntry loca = reader.TableRecordEntries[OpenTypeTableTag.Loca];
            SfntTableRecordEntry glyf = reader.TableRecordEntries[OpenTypeTableTag.Glyf];
            return new GlyfTableReader(reader.BinaryReader, loca, glyf, fontHeader.IsLocaShort);
        }

        /// <summary>
        /// Helps to read sfnt files.
        /// </summary>
        private SfntReader mSfntReader;

        /// <summary>
        /// The "head" table.
        /// </summary>
        private FontHeader mFontHeader;

        /// <summary>
        /// Collection of true type font names.
        /// </summary>
        private TTFontNames mNames;
        /// <summary>
        /// The "os/2" table.
        /// </summary>
        private FontMetrics mMetrics;
    }
}
