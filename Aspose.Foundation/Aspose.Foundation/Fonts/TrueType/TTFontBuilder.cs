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
            font.WinLineMeasurements = GetWinLineMeasurements();
            font.UseTypoMetrics = mReader.Os2.UseTypoMetrics;
            font.OfficeLineMeasurements = mReader.Os2.UseTypoMetrics
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

            font.IsCjkMetrics = mReader.Os2.ulCodePageRanges.IsCjkMetrics;
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

        private FontLineMeasurements GetWinLineMeasurements()
        {
            // These calculations are taken from http://www.microsoft.com/typography/otspec/recom.htm
            int externalLeading = Math.Max(0,
                mReader.Hhea.LineGap - ((mReader.Os2.usWinAscent + mReader.Os2.usWinDescent) -
                                             (mReader.Hhea.Ascender - mReader.Hhea.Descender)));

            return new FontLineMeasurements(
                mReader.Os2.usWinAscent,
                mReader.Os2.usWinDescent,
                mReader.Os2.usWinAscent + mReader.Os2.usWinDescent + externalLeading);
        }

        internal static FontStyle SelectFontStyle(FontMetrics metrics, FontHeader header, string familyName)
        {
            return metrics.fsSelection == 0 ? header.Style : metrics.Style;
        }

        private List<CharacterReplacerBase> GetFontCharacterReplacers(TTFont font)
        {
            List<CharacterReplacerBase> result = new List<CharacterReplacerBase>();

            if (font.IsSymbolic)
            {
                result.Add(new SymbolCharacterReplacer());
                result.Add(new Win1252PuaCharacterReplacer());
            }

            // Control chars
            result.Add(new ControlCharacterReplacer());

            return result;
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
