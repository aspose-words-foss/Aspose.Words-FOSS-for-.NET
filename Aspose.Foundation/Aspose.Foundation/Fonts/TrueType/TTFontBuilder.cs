// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/10/2024 by Konstantin Kornilov

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Aspose.Collections;
using Aspose.Drawing;
using Aspose.Drawing.Fonts;
using Aspose.Fonts.Sfnt;
using Aspose.Fonts.Ttc;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Helper class for building <see cref="TTFont"/> instances.
    /// </summary>
    public class TTFontBuilder
    {
        public static TTFont Read(PhysicalFontData fontData)
        {
            // Register encodings support if required.
            EncodingUtil.RegisterEncodings();

            using (Stream stream = fontData.FileData.OpenStream())
            {
                TTFontBuilder builder = new TTFontBuilder(OpenTypeReader.Create(stream, fontData), fontData);
                return builder.ReadFontCore();
            }
        }

        /// <summary>
        /// Reads a TrueType/OpenType font from a file. TTC/Variation fonts are not handled.
        /// Throws if the font is not recognized as a valid true type font or if the font file does not exist.
        /// </summary>
        internal static TTFont ReadOpenType(string fileName)
        {
            return Read(new PhysicalFontData(new FileFontData(fileName)));
        }

        /// <summary>
        /// Reads a TrueType/OpenType font from a file. TTC/Variation fonts are not handled.
        /// Throws if the font is not recognized as a valid true type font or if the font file does not exist.
        /// </summary>
        internal static TTFont ReadTtc(string fileName, string fontName)
        {
            return Read(new TtcPhysicalFontData(new FileFontData(fileName), -1, fontName));
        }

        /// <summary>
        /// Reads a TrueType/OpenType font from a byte array. TTC/Variation fonts are not handled.
        /// Throws if the font is not recognized as a valid true type font.
        /// </summary>
        public static TTFont ReadOpenType(byte[] fontData)
        {
            return Read(new PhysicalFontData(new MemoryFontData(fontData)));
        }


        private TTFontBuilder(OpenTypeReader reader, PhysicalFontData fontData)
        {
            mReader = reader;
            mFontData = fontData;
        }

        private TTFont ReadFontCore()
        {
            mReader.ReadHeader();
            TTFont font = new TTFont();
            // Potentially, there are cases when TTC font data comes without a font index (like when loading a previous
            // version of font search info cache).
            // Update font data to actual one after reading a header.
            PhysicalFontData updatedFontData = mReader.GetUpdatedFontData();
            font.PhysicalData = updatedFontData != null ? updatedFontData : mFontData;
            BuildFont(font);
            return font;
        }

        private void BuildFont(TTFont font)
        {
            font.IsCff = mReader.ContainsTable(OpenTypeTableTag.Cff);

            font.EmHeight = mReader.Head.UnitsPerEm;
            font.XMin = mReader.Head.XMin;
            font.YMin = mReader.Head.YMin;
            font.XMax = mReader.Head.XMax;
            font.YMax = mReader.Head.YMax;

            // Store required names in the object that we are building.
            SetFontNames(font);

            // See description in the TTFont remarks.
            font.TypoLineMeasurements = GetTypoLineMeasurements();
            font.WinLineMeasurements = GetWinLineMeasurements(font.IsCff);
            font.UseTypoMetrics = mReader.Os2.UseTypoMetrics;
            font.OfficeLineMeasurements = (mReader.Os2.UseTypoMetrics && !font.IsCff)
                                              ? font.TypoLineMeasurements
                                              : font.WinLineMeasurements;

            // Taken this from iTextSharp.
            font.ItalicAngle =
                (float)(-Math.Atan2(mReader.Hhea.CaretSlopeRun, mReader.Hhea.CaretSlopeRise) * 180 / Math.PI);

            font.StrikeoutSize = mReader.Os2.yStrikeoutSize;
            font.StrikeoutPosition = mReader.Os2.yStrikeoutPosition;
            font.SubscriptSize = mReader.Os2.ySubscriptYSize;
            font.SubscriptOffset = mReader.Os2.ySubscriptYOffset;
            font.SuperscriptSize = mReader.Os2.ySuperscriptYSize;
            font.SuperscriptOffset = mReader.Os2.ySuperscriptYOffset;
            font.Style = SelectFontStyle(mReader.Os2, mReader.Head, font.FamilyName);
            font.WeightClass = mReader.Os2.usWeightClass;
            font.CapHeight = mReader.Os2.sCapHeight;
            font.XHeight = mReader.Os2.sxHeight;
            font.AvgCharWidth = mReader.Os2.xAvgCharWidth;
            font.UnderlinePosition = mReader.Post.UnderlinePosition;
            font.UnderlineThickness = mReader.Post.UnderlineThickness;
            font.FsType = mReader.Os2.fsType;
            font.UnicodeRanges = mReader.Os2.ulUnicodeRanges;
            font.CodepageRanges = mReader.Os2.ulCodePageRanges;
            font.IsMonospaced = FontUtil.IsMonospacedFont(font.FamilyName);

            // It seems that MS Word uses both these flags when decide to use or not symbol charset.
            font.IsSymbolic = mReader.Cmap.IsSymbolEncoding || mReader.Os2.ulCodePageRanges.IsSymbolCharsetUsed;

            font.Glyphs = mReader.Cmap.BuildGlyphs(mReader.Hmtx, mReader.Vmtx, mReader.Maxp.NumGlyphs);
            font.Glyphs.Replacers = GetFontCharacterReplacers(font);
            ProcessColoredGlyphs(font);

            font.IsLegacyEncoding = IsLegacyEncodingFont();
            font.IsLegacyArabicSimplified = mReader.Os2.IsLegacyArabicSimplified;
            font.IsLegacyArabicTraditional = mReader.Os2.IsLegacyArabicTraditional;
            if (font.IsLegacyEncoding)
                ProcessLegacyEncodings(font.Glyphs);

            font.IsCjkMetrics = mReader.Os2.ulCodePageRanges.IsCjkMetrics;
            if (font.IsCjkMetrics)
            {
                const int cjkRadical1CharCode = 0x4E00;
                const int leftCornerBracketCharCode = 0x300C;
                const int leftDoubleQuotationMarkCharCode = 0x201C;
                font.IsCjkPunctuationCompressible = GetAreGlyphWidthsEqual(
                    font,
                    cjkRadical1CharCode,
                    leftCornerBracketCharCode);
                font.IsCjkQuotationMarkCompressible = GetAreGlyphWidthsEqual(
                    font,
                    cjkRadical1CharCode,
                    leftDoubleQuotationMarkCharCode);
            }

            ProcessMwEmbeddedSubset(font);
        }

        private void ProcessMwEmbeddedSubset(TTFont font)
        {
            // WORDSNET-28236 In customer case, MW adds 'cmap' mapping for glyph which is not embedded.
            // Seems to be a bug in MW. For proper rendering, we could filter out such mappings.
            if(!font.Data.IsEmbedded)
                return;

            // Note: Experiments shows that MW is not able to subset CFF fonts.
            if(font.IsCff)
                return;

            SfntTableRecordEntry locaEntry = mReader.SfntReader.TableRecordEntries[OpenTypeTableTag.Loca];
            mReader.SfntReader.SeekToTable(OpenTypeTableTag.Loca);
            TTLocaTable loca = TTLocaTable.Read(mReader.BinaryReader, locaEntry.Length, mReader.Head.IsLocaShort);

            IntToIntDictionary unusedGlyph = new IntToIntDictionary(font.Glyphs.Glyphs.Count);
            for (int i = 1; i < font.Glyphs.Glyphs.Count; i++)
            {
                // Experiments shows that MW empties glyph outlines and zeroes advance for unused glyphs.
                TTGlyph glyph = font.Glyphs.GetGlyphByIndex(i);
                bool isEmptyGlyph = (loca.GlyphLocations[i + 1] - loca.GlyphLocations[i]) == 0;
                if (isEmptyGlyph && glyph.AdvanceWidth == 0)
                    unusedGlyph.Add(i, i);
            }

            IntList charcodesToRemove = new IntList(font.Glyphs.CharMap.Count);
            IntToObjDictionary<TTGlyph>.Enumerator enumerator = font.Glyphs.CharMap.GetEnumerator();
            while (enumerator.MoveNext())
                if(unusedGlyph.ContainsKey(enumerator.CurrentValue.GlyphIndex))
                    charcodesToRemove.Add(enumerator.CurrentKey);

            for (int i = 0; i < charcodesToRemove.Count; i++)
                font.Glyphs.CharMap.Remove(charcodesToRemove[i]);
        }

        private void SetFontNames(TTFont font)
        {
            font.FamilyName = mReader.Name.FamilyName;
            font.SubFamilyName = mReader.Name.SubFamilyName;
            font.FullFontName = mReader.Name.FullFontName;
            font.VersionString = mReader.Name.VersionString;

            // WORDSNET-25211 PostScriptName may not be presented in font file but is required for some renderers.
            // In this case use full font name instead of PostScript name.
            font.PostscriptName = StringUtil.HasChars(mReader.Name.PostScriptName)
                ? mReader.Name.PostScriptName
                : mReader.Name.FullFontName;
        }

        private FontLineMeasurements GetTypoLineMeasurements()
        {
            return new FontLineMeasurements(
                mReader.Os2.sTypoAscender,
                -mReader.Os2.sTypoDescender,
                mReader.Os2.sTypoAscender - mReader.Os2.sTypoDescender + mReader.Os2.sTypoLineGap);
        }

        private FontLineMeasurements GetWinLineMeasurements(bool isCff)
        {
            // These calculations are taken from http://www.microsoft.com/typography/otspec/recom.htm
            int externalLeading = Math.Max(0,
                mReader.Hhea.LineGap - ((mReader.Os2.usWinAscent + mReader.Os2.usWinDescent) -
                                             (mReader.Hhea.Ascender - mReader.Hhea.Descender)));

            // Seems that MW don't use external leading for CFF fonts.
            if (isCff)
                externalLeading = 0;

            return new FontLineMeasurements(
                mReader.Os2.usWinAscent,
                mReader.Os2.usWinDescent,
                mReader.Os2.usWinAscent + mReader.Os2.usWinDescent + externalLeading);
        }

        internal static FontStyle SelectFontStyle(FontMetrics metrics, FontHeader header, string familyName)
        {
            FontStyle style = metrics.fsSelection == 0 ? header.Style : metrics.Style;

            // WORDSNET-9215 Experiments shows that MW do not perform bold simulation for fonts with weight class >= 600.
            // However font style for these fonts is determined from fsSelection flags and may be Regular for example.
            // WORDSNET-12365 In this case MW actually consider the font as Bold when style flag is Regular and weight
            // class is 700.
            if (metrics.usWeightClass > FontMetrics.SemiBoldWeightValue)
                style |= FontStyle.Bold;

            if (metrics.usWeightClass == FontMetrics.SemiBoldWeightValue && !FontUtil.IsBoldSimulationAllowedOnSemibold(familyName))
                style |= FontStyle.Bold;

            return style;
        }

        private List<CharacterReplacerBase> GetFontCharacterReplacers(TTFont font)
        {
            List<CharacterReplacerBase> result = new List<CharacterReplacerBase>();

            // Several replacers for obsolete fonts.
            // Legacy encoding fonts uses Windows Symbol cmap encoding but should have special encoding processing.
            if (font.IsSymbolic && !IsLegacyEncodingFont())
            {
                result.Add(new SymbolCharacterReplacer());
                result.Add(new Win1252PuaCharacterReplacer());
            }

            // Control chars
            result.Add(new ControlCharacterReplacer());

            // Linux (for Java only). Mac also uses this replacer WORDSJAVA-1123 for bullets issues
            // WORDSJAND-333 LinuxCharacterReplacer is not used in Android because we ask users to use Windows' fonts
            // For Android's fonts there is going to be another replacer.
            // WORDSJAVA-1634 Don't use LinuxCharacterReplacer if Windows' fonts are used
            if (!FontPal.IsWindowsFont(font) && PlatformUtilPal.IsUnixLike() && !PlatformUtilPal.IsAndroid())
                result.Add(new LinuxCharacterReplacer());

            return result;
        }

        private bool IsLegacyEncodingFont()
        {
            return mReader.Os2.IsLegacyHebrew || mReader.Os2.IsLegacyArabicSimplified || mReader.Os2.IsLegacyArabicTraditional;
        }

        private void ProcessLegacyEncodings(TTGlyphHashtable glyphs)
        {
            if (mReader.Os2.IsLegacyArabicSimplified)
            {
                LegacyEncodingsUtil.ProcessArabicSimplified(glyphs);
            }
            else if (mReader.Os2.IsLegacyArabicTraditional)
            {
                LegacyEncodingsUtil.ProcessArabicTraditional(glyphs);
            }
            else if (mReader.Os2.IsLegacyHebrew)
            {
                LegacyEncodingsUtil.ProcessHebrew(glyphs);
            }
        }

        private void ProcessColoredGlyphs(TTFont font)
        {
            if (!mReader.ContainsTable(OpenTypeTableTag.Colr) || !mReader.ContainsTable(OpenTypeTableTag.Cpal))
                return;

            font.IsColored = true;

            foreach (ColrBaseGlyphRecord glyphRecord in mReader.Colr.GlyphRecords)
            {
                List<TTGlyphColoredLayer> layers = new List<TTGlyphColoredLayer>(glyphRecord.NumLayers);
                for (int i = 0; i < glyphRecord.NumLayers; i++)
                {
                    ColrLayerRecord layerRecord = mReader.Colr.LayerRecords[glyphRecord.FirstLayerIndex + i];
                    DrColor color = mReader.Cpal.PaletteColors[layerRecord.PaletteIndex];
                    layers.Add(new TTGlyphColoredLayer(layerRecord.GlyphId, color));
                }

                TTGlyph glyph = font.Glyphs.GetGlyphByIndex(glyphRecord.GlyphId);
                glyph.ColoredInfo = new TTGlyphColoredInfo(layers);
            }
        }

        private static bool GetAreGlyphWidthsEqual(TTFont font, int charCode1, int charCode2)
        {
            TTGlyph glyph1 = font.Glyphs.GetGlyphByCharCode(charCode1);
            TTGlyph glyph2 = font.Glyphs.GetGlyphByCharCode(charCode2);

            return glyph1 != null && glyph2 != null &&
                   glyph1.AdvanceWidth == glyph2.AdvanceWidth;
        }

        private readonly OpenTypeReader mReader;
        private readonly PhysicalFontData mFontData;
    }
}
