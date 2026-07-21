// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/09/2005 by Roman Korchagin

using System;
using System.Drawing;
using System.Text;
using Aspose.Bidi;
using Aspose.Collections.Generic;
using Aspose.Common;
using Aspose.Fonts.Ttc;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Contains information about a true type font.
    /// </summary>
    /// <remarks>
    /// Each OpenType font contains two sets of line measurements (metrics) relevant to MW: typographic and windows. Spec says that
    /// typographic metrics should be preferred over the windows metrics. But MW relies on the UseTypeMetrics flag for TrueType fonts.
    /// For OpenType(CFF) font MW13 and older used only win metrics. MW16 starts to use UseTypeMetrics flag when laying-out text in GUI.
    /// But when printing or saving to PDF windows metrics are still used.
    ///
    /// <see cref="WinLineMeasurements"/> represents windows metrics. <see cref="TypoLineMeasurements"/> represents typographic metrics.
    /// <see cref="OfficeLineMeasurements"/> represents metrics used by Office. For OpenType(CFF) font it is always windows metrics for now.
    /// </remarks>
    public class TTFont
    {
        /// <summary>
        /// Up to four fonts can share the Font Family name, forming a font style linking group
        /// (regular, italic, bold, bold italic - as defined by OS/2.fsSelection bit settings).
        /// </summary>
        public string FamilyName { get; set; }

        /// <summary>
        /// The Font Subfamily name distinguishes the font in a group with the same Font Family name (name ID 1).
        /// This is assumed to address style (italic, oblique) and weight (light, bold, black, etc.).
        /// A font with no particular differences in weight or style (e.g. medium weight, not italic and fsSelection bit 6 set) should have the string "Regular" stored in this position.
        /// </summary>
        public string SubFamilyName { get; set; }

        /// <summary>
        /// This should be a combination of strings 1 and 2. Exception: if the font is "Regular" as indicated
        /// in string 2, then use only the family name contained in string 1.
        /// An exception to the above definition of Full font name is for Microsoft platform strings
        /// for CFF OpenType fonts: in this case, the Full font name string must be identical to the PostScript
        /// FontName in the CFF Name INDEX.
        /// </summary>
        public string FullFontName { get; set; }

        /// <summary>
        /// Specifies a string which is used to invoke a PostScript language font that corresponds to this
        /// OpenType font.
        /// </summary>
        public string PostscriptName { get; set; }

        public string VersionString { get; set; }

        /// <summary>
        /// Gets the full font file name if the font was read from a file.
        /// Gets a generated font file name if the font was read from memory.
        /// </summary>
        public string FileName
        {
            get
            {
                if (PhysicalData == null)
                    return null;

                IFileFontData fileFontData = PhysicalData.FileData as IFileFontData;
                return (fileFontData != null)
                    ? fileFontData.FileName
                    : CreateFileNameForEmbeddedFont();
            }
        }

        /// <summary>
        /// Map of char codes to TTGlyph objects.
        /// </summary>
        public TTGlyphHashtable Glyphs { get; set; }

        /// <summary>
        /// Gets the font style.
        /// </summary>
        public FontStyle Style { get; set; }

        public bool IsBold
        {
            get { return ((Style & FontStyle.Bold) != 0); }
        }

        public bool IsItalic
        {
            get { return ((Style & FontStyle.Italic) != 0); }
        }

        /// <summary>
        /// Font weight class.
        /// </summary>
        /// <remarks>
        /// https://learn.microsoft.com/en-us/typography/opentype/spec/os2#wtc
        /// </remarks>
        public int WeightClass { get; set; }

        /// <summary>
        /// Gets the height, in font design units, of the em square for the specified style.
        /// Corresponds to FontFamily.GetEmHeight.
        /// </summary>
        public int EmHeight { get; set; }

        /// <summary>
        /// Returns the ascent, in design units. Corresponds to FontFamily.GetCellAscent.
        /// </summary>
        public int Ascent
        {
            get { return OfficeLineMeasurements.Ascent; }
        }

        /// <summary>
        /// Returns the descent, in design units. Corresponds to FontFamily.GetCellDescent.
        /// </summary>
        public int Descent
        {
            get { return OfficeLineMeasurements.Descent; }
        }

        /// <summary>
        /// Gets Ascent + Descent in design units.
        /// </summary>
        public int CellHeight
        {
            get { return Ascent + Descent; }
        }

        /// <summary>
        /// Returns the line spacing in design units. Corresponds to FontFamily.GetLineSpacing.
        /// The line spacing is the vertical distance between the base lines of two consecutive lines of text.
        /// </summary>
        public int LineSpacing
        {
            get { return OfficeLineMeasurements.LineSpacing; }
        }

        /// <summary>
        /// Width of the strikeout stroke in font design units.
        /// </summary>
        public int StrikeoutSize { get; set; }

        /// <summary>
        /// The recommended horizontal size in font design units for superscripts for this font.
        /// </summary>
        public int SuperscriptSize { get; set; }

        /// <summary>
        /// The recommended vertical offset in font design units from the baseline for superscripts for this font.
        /// </summary>
        public int SuperscriptOffset { get; set; }

        /// <summary>
        /// The recommended horizontal size in font design units for subscripts for this font.
        /// </summary>
        public int SubscriptSize { get; set; }

        /// <summary>
        /// The recommended vertical offset in font design units from the baseline for subscripts for this font.
        /// </summary>
        public int SubscriptOffset { get; set; }

        /// <summary>
        /// The position of the top of the strikeout stroke relative to the baseline in font design units.
        /// </summary>
        public int StrikeoutPosition { get; set; }

        /// <summary>
        /// The distance between the baseline and the approximate height of
        /// non-ascending lowercase letters measured in font design units.
        /// </summary>
        public int XHeight { get; set; }

        /// <summary>
        /// Bounding box in design units.
        /// </summary>
        public int XMin { get; set; }

        /// <summary>
        /// Bounding box in design units.
        /// </summary>
        public int YMin { get; set; }

        /// <summary>
        /// Bounding box in design units.
        /// </summary>
        public int XMax { get; set; }

        /// <summary>
        /// Bounding box in design units.
        /// </summary>
        public int YMax { get; set; }

        public int CapHeight { get; set; }

        public int AvgCharWidth { get; set; }

        /// <summary>
        /// In degrees I think. Needed to pass to PDF document.
        /// </summary>
        public float ItalicAngle { get; set; }

        /// <summary>
        /// Returns char advance width from the font.
        /// </summary>
        /// <remarks>
        /// This method should not be used to get the real char advance. DrFont may apply
        /// some modifications to these values. DrFont.GetCharWidthPoints() method should be used instead.
        /// </remarks>
        internal int GetCharAdvanceWidthDesignUnits(int c)
        {
            return Glyphs.FetchGlyphByCharCode(c).AdvanceWidth;
        }

        /// <summary>
        /// Returns char advance height from the font.
        /// </summary>
        /// <remarks>
        /// This method should not be used to get the real char advance. DrFont may apply
        /// some modifications to these values. DrFont.GetCharWidthPoints() method should be used instead.
        /// </remarks>
        internal int GetCharAdvanceHeightDesignUnits(int c)
        {
            return Glyphs.FetchGlyphByCharCode(c).AdvanceHeight;
        }

        /// <summary>
        /// Converts the value in design units into points, given the font size in points.
        /// </summary>
        public float DesignUnitsToPoints(float designUnits, float fontSize)
        {
            float factor = EmHeight / fontSize;
            return designUnits / factor;
        }

        /// <summary>
        /// Gets or set  Preview and Print embedding restriction for a font usage.
        /// TODO AN. There are few levels of restriction in OTF fonts usage described e.g. in XPS 1.0, 2.1.7.2.
        /// Right now the only Preview and Print embedding is supported. The support for all other levels should be
        /// implemented later.
        /// </summary>
        public bool IsPrintPreview
        {
            get { return FsType.Permissions == FontFsTypePermissions.PrintAndPreview; }
        }

        /// <summary>
        /// Gets fsType value of "OS/2" table (font embedding licensing rights).
        /// </summary>
        public FontFsType FsType { get; set; }

        /// <summary>
        /// True if the font file contains CFF table.
        /// When the font has a CFF table, it means it is an OpenType font based on PostScript glyph outlines.
        /// When this is false, then it is either a TrueType font or an OpenType font based on TrueType glyph outlines.
        /// </summary>
        public bool IsCff { get; set; }

        /// <summary>
        /// Font data.
        /// </summary>
        public IFontData Data
        {
            get { return PhysicalData.FileData; }
        }

        /// <summary>
        /// Font data.
        /// </summary>
        public PhysicalFontData PhysicalData { get; set; }

        /// <summary>
        /// True if font is symbolic.
        /// </summary>
        public bool IsSymbolic { get; set; }

        /// <summary>
        /// True if font is legacy.
        /// </summary>
        public bool IsLegacyEncoding { get; set; }

        /// <summary>
        /// True if font is a legacy traditional Arabic font.
        /// </summary>
        public bool IsLegacyArabicTraditional { get; set; }

        /// <summary>
        /// True if font is a legacy simplified Arabic font.
        /// </summary>
        public bool IsLegacyArabicSimplified { get; set; }

        /// <summary>
        /// True if font is monospaced.
        /// </summary>
        public bool IsMonospaced { get; set; }

        /// <summary>
        /// True if font data is in TTC format.
        /// </summary>
        internal bool IsTtc
        {
            get { return PhysicalData.IsTtc; }
        }

        /// <summary>
        /// Returns a key for the font cache table.
        /// Uses <see cref="FullFontName"/>, <see cref="FamilyName"/> and <see cref="Style"/>
        /// as parameters for key generation. If it is needed to use more parameters,
        /// use <see cref="BuildFontKey(string[])"/> method.
        /// </summary>
        public string TtFontKey
        {
            get
            {
                if (!StringUtil.HasChars(mTtFontKey))
                    mTtFontKey = BuildFontKey(new string[] { FullFontName, FamilyName, FormatterPal.IntToStr((int)Style) });

                return mTtFontKey;
            }
        }

        /// <summary>
        /// True if this font should adjust metrics according to MW CJK handling.
        /// </summary>
        public bool IsCjkMetrics { get; set; }

        /// <summary>
        /// True if this font is CJK and the punctuation characters can be compressed.
        /// </summary>
        public bool IsCjkPunctuationCompressible { get; set; }

        /// <summary>
        /// True if this font is CJK and the quotation mark characters can be compressed.
        /// </summary>
        public bool IsCjkQuotationMarkCompressible { get; set; }

        /// <summary>
        /// True if the font is embedded.
        /// </summary>
        public bool IsEmbedded
        {
            get { return Data.IsEmbedded; }
        }

        /// <summary>
        /// Windows line measurements.
        /// </summary>
        public FontLineMeasurements WinLineMeasurements { get; set; }

        /// <summary>
        /// Typographic line measurements.
        /// </summary>
        public FontLineMeasurements TypoLineMeasurements { get; set; }

        /// <summary>
        /// Line measurements used by Office.
        /// </summary>
        public FontLineMeasurements OfficeLineMeasurements { get; set; }

        /// <summary>
        /// Flag from the font data.
        /// </summary>
        /// <remarks>
        /// See class remarks for more info.
        /// </remarks>
        public bool UseTypoMetrics { get; set; }

        /// <summary>
        /// Supported unicode ranges.
        /// </summary>
        public FontUnicodeRanges UnicodeRanges { get; set; }

        /// <summary>
        /// Supported codepage ranges.
        /// </summary>
        public FontCodepageRanges CodepageRanges { get; set; }

        /// <summary>
        /// Gets or sets the underline position.
        /// </summary>
        public short UnderlinePosition { get; set; }

        /// <summary>
        /// Gets or sets the underline thickness.
        /// </summary>
        public short UnderlineThickness { get; set; }

        /// <summary>
        /// True if font contains colored glyphs.
        /// </summary>
        public bool IsColored { get; set; }

        /// <summary>
        /// Generates file name for embedded fonts.
        /// </summary>
        private string CreateFileNameForEmbeddedFont()
        {
            // This code copied from XpsContextBase.GetFontResourceName method.
            // FullFontName or combination of FamilyName and Style could be used,
            // but there is no verification that these fields may act as valid file name.
            byte[] guidBytes = new byte[16];
            ArrayUtil.WriteUInt32ToByteArray(FamilyName.GetHashCode(), guidBytes, 0);
            ArrayUtil.WriteUInt32ToByteArray((int)Style, guidBytes, 4);
            ArrayUtil.WriteUInt32ToByteArray(FullFontName.GetHashCode(), guidBytes, 8);
            Guid guid = new Guid(guidBytes);
            string guidName = guid.ToString("D");
            string ext = IsCff ? "otf" : "ttf";
            return string.Format("{0}.{1}", guidName, ext);
        }

        /// <summary>
        /// Builds font key from specified string parameters.
        /// Use this method if you need to build custom font key, if not use <see cref="TtFontKey"/> property instead.
        /// </summary>
        public static string BuildFontKey(string[] keyParams)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < keyParams.Length; i++)
            {
                builder.Append(keyParams[i]);
                if (i < keyParams.Length - 1)
                    builder.Append(Separator);
            }
            return builder.ToString();
        }

        /// <summary>
        /// Builds font key from specified string parameters.
        /// Use this method if you need to build custom font key, if not use <see cref="TtFontKey"/> property instead.
        /// This method is optimized version of <see cref="BuildFontKey(string[])"/> to reach better performance.
        /// </summary>
        public static string BuildFontKey(string keyParam1, string keyParam2)
        {
            return keyParam1 + Separator + keyParam2;
        }

        private string mTtFontKey;

        private const char Separator = '-';
    }
}
