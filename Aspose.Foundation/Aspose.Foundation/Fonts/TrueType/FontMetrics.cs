// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/09/2005 by Roman Korchagin

using System;
using System.Drawing;
using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// The OS/2 table consists of a set of metrics that are required in OpenType fonts.
    /// See http://www.microsoft.com/typography/otspec/os2.htm for more info.
    /// Here is info about older versions:
    /// http://www.microsoft.com/typography/otspec/os2ver3.htm
    /// http://www.microsoft.com/typography/otspec/os2ver2.htm
    /// http://www.microsoft.com/typography/otspec/os2ver1.htm
    /// http://www.microsoft.com/typography/otspec/os2ver0.htm
    /// </summary>
    internal class FontMetrics : TTTable
    {
        /// <summary>
        /// The version number for this OS/2 table.
        /// </summary>
        internal ushort version;
        /// <summary>
        /// The Average Character Width parameter specifies the arithmetic average of the escapement (width) of
        /// all non-zero width glyphs in the font.
        /// Pels / em units.
        /// </summary>
        internal short xAvgCharWidth;
        /// <summary>
        /// Indicates the visual weight (degree of blackness or thickness of strokes) of the characters in the font.
        /// </summary>
        internal ushort usWeightClass;
        /// <summary>
        /// Indicates a relative change from the normal aspect ratio (width to height ratio) as specified by a font
        /// designer for the glyphs in a font.
        /// </summary>
        internal ushort usWidthClass;

        /// <summary>
        /// Indicates font embedding licensing rights for the font.
        /// </summary>
        internal FontFsType fsType;
        /// <summary>
        /// The recommended horizontal size in font design units for subscripts for this font.
        /// </summary>
        internal short ySubscriptXSize;
        /// <summary>
        /// The recommended vertical size in font design units for subscripts for this font.
        /// </summary>
        internal short ySubscriptYSize;
        /// <summary>
        /// The recommended horizontal offset in font design untis for subscripts for this font.
        /// </summary>
        internal short ySubscriptXOffset;
        /// <summary>
        /// The recommended vertical offset in font design units from the baseline for subscripts for this font.
        /// </summary>
        internal short ySubscriptYOffset;
        /// <summary>
        /// The recommended horizontal size in font design units for superscripts for this font.
        /// </summary>
        internal short ySuperscriptXSize;
        /// <summary>
        /// The recommended vertical size in font design units for superscripts for this font.
        /// </summary>
        internal short ySuperscriptYSize;
        /// <summary>
        /// The recommended horizontal offset in font design units for superscripts for this font.
        /// </summary>
        internal short ySuperscriptXOffset;
        /// <summary>
        /// The recommended vertical offset in font design units from the baseline for superscripts for this font.
        /// </summary>
        internal short ySuperscriptYOffset;
        /// <summary>
        /// Width of the strikeout stroke in font design units.
        /// </summary>
        internal short yStrikeoutSize;
        /// <summary>
        /// The position of the top of the strikeout stroke relative to the baseline in font design units.
        /// </summary>
        internal short yStrikeoutPosition;
        /// <summary>
        /// This parameter is a classification of font-family design.
        /// The high byte of this field contains the family class, while the low byte contains the family subclass.
        /// </summary>
        internal short sFamilyClass;
        /// <summary>
        /// This 10 byte series of numbers is used to describe the visual characteristics of a given typeface.
        /// </summary>
        internal FontPanose panose;
        /// <summary>
        /// Unicode Character Range.
        /// 32-bit unsigned long(4 copies) totaling 128 bits.
        /// This field is used to specify the Unicode blocks or ranges encompassed by the font file in the
        /// 'cmap' subtable for platform 3, encoding ID 1 (Microsoft platform).
        /// If the bit is set (1) then the Unicode range is considered functional. If the bit is clear (0)
        /// then the range is not considered functional.
        /// </summary>
        internal FontUnicodeRanges ulUnicodeRanges;

        /// <summary>
        /// The four character identifier for the vendor of the given type face.
        /// </summary>
        internal byte[] achVendID;
        /// <summary>
        /// Contains information concerning the nature of the font patterns.
        /// </summary>
        internal ushort fsSelection;
        /// <summary>
        /// If set, it is strongly recommended to use OS/2.sTypoAscender - OS/2.sTypoDescender+ OS/2.sTypoLineGap
        /// as a value for default line spacing for this font.
        /// </summary>
        internal bool UseTypoMetrics
        {
            get { return BitUtil.IsSetUInt16(fsSelection, FsSelectionUseTypoMetricsMask); }
        }

        /// <summary>
        /// MS spec says that legacy arabic fonts uses high byte of fsSelection in version 0 table.
        /// See https://docs.microsoft.com/en-us/typography/legacy/legacy_arabic_fonts.
        /// </summary>
        internal bool IsLegacyArabicSimplified
        {
            get { return version == 0 && fsSelection >> 8 == 178; }
        }

        /// <summary>
        /// MS spec says that legacy arabic fonts uses high byte of fsSelection in version 0 table.
        /// See https://docs.microsoft.com/en-us/typography/legacy/legacy_arabic_fonts.
        /// </summary>
        internal bool IsLegacyArabicTraditional
        {
            get { return version == 0 && fsSelection >> 8 == 179; }
        }

        /// <summary>
        /// Experiments shows that legacy hebrew font uses 177 value in fsSelection upper byte.
        /// </summary>
        internal bool IsLegacyHebrew
        {
            get { return version == 0 && fsSelection >> 8 == 177; }
        }

        /// <summary>
        /// The minimum Unicode index (character code) in this font, according to the cmap subtable for
        /// platform ID 3 and platform- specific encoding ID 0 or 1.
        /// </summary>
        internal ushort usFirstCharIndex;
        /// <summary>
        /// The maximum Unicode index (character code) in this font, according to the cmap subtable for
        /// platform ID 3 and encoding ID 0 or 1.
        /// </summary>
        internal ushort usLastCharIndex;
        /// <summary>
        /// The typographic ascender for this font.
        /// </summary>
        internal short sTypoAscender;
        /// <summary>
        /// The typographic descender for this font
        /// </summary>
        internal short sTypoDescender;
        /// <summary>
        /// The typographic line gap for this font.
        /// </summary>
        internal short sTypoLineGap;
        /// <summary>
        /// The ascender metric for Windows.
        /// </summary>
        internal short usWinAscent;
        /// <summary>
        /// The descender metric for Windows.
        /// </summary>
        internal short usWinDescent;
        /// <summary>
        /// 32-bit unsigned long(2 copies) totaling 64 bits.
        /// Code Page Character Range
        /// This field is used to specify the code pages encompassed by the font file in the 'cmap'
        /// subtable for platform 3, encoding ID 1 (Microsoft platform).
        /// </summary>
        internal FontCodepageRanges ulCodePageRanges = FontCodepageRanges.Empty;

        /// <summary>
        /// This metric specifies the distance between the baseline and the approximate height of
        /// non-ascending lowercase letters measured in FUnits.
        /// </summary>
        internal short sxHeight;
        /// <summary>
        /// This metric specifies the distance between the baseline and the approximate height of
        /// uppercase letters measured in FUnits.
        /// </summary>
        internal short sCapHeight;
        /// <summary>
        /// Whenever a request is made for a character that is not in the font, Windows provides
        /// this default character.
        /// </summary>
        internal ushort usDefaultChar;
        /// <summary>
        /// This is the Unicode encoding of the glyph that Windows uses as the break character.
        /// </summary>
        internal ushort usBreakChar;
        /// <summary>
        /// The maximum length of a target glyph context for any feature in this font.
        /// </summary>
        internal ushort usMaxContext;

        /// <summary>
        /// Returns the font style.
        /// </summary>
        internal FontStyle Style
        {
            get
            {
                FontStyle style = 0;
                style |= ((fsSelection & 0x01) != 0) ? FontStyle.Italic : 0;
                style |= ((fsSelection & 0x02) != 0) ? FontStyle.Underline : 0;
                // 0x04 - Negative
                // 0x08 - Outlined
                style |= ((fsSelection & 0x10) != 0) ? FontStyle.Strikeout : 0;
                style |= ((fsSelection & 0x20) != 0) ? FontStyle.Bold : 0;
                // 0x40 - Regular
                return style;
            }
        }

        /// <summary>
        /// Ctor. Reads font metrics from the stream.
        /// </summary>
        internal FontMetrics(BigEndianBinaryReader reader)
        {
            this.version = reader.ReadUInt16();
            this.xAvgCharWidth = reader.ReadInt16();
            this.usWeightClass = reader.ReadUInt16();
            this.usWidthClass = reader.ReadUInt16();
            this.fsType = new FontFsType(reader.ReadUInt16());

            this.ySubscriptXSize = reader.ReadInt16();
            this.ySubscriptYSize = reader.ReadInt16();
            this.ySubscriptXOffset = reader.ReadInt16();
            this.ySubscriptYOffset = reader.ReadInt16();

            this.ySuperscriptXSize = reader.ReadInt16();
            this.ySuperscriptYSize = reader.ReadInt16();
            this.ySuperscriptXOffset = reader.ReadInt16();
            this.ySuperscriptYOffset = reader.ReadInt16();

            this.yStrikeoutSize = reader.ReadInt16();
            this.yStrikeoutPosition = reader.ReadInt16();
            this.sFamilyClass = reader.ReadInt16();

            this.panose = new FontPanose(reader.ReadBytes(10));

            uint ulUnicodeRange1 = reader.ReadUInt32();
            uint ulUnicodeRange2 = reader.ReadUInt32();
            uint ulUnicodeRange3 = reader.ReadUInt32();
            uint ulUnicodeRange4 = reader.ReadUInt32();
            ulUnicodeRanges =
                new FontUnicodeRanges(ulUnicodeRange1, ulUnicodeRange2, ulUnicodeRange3, ulUnicodeRange4);

            this.achVendID = reader.ReadBytes(4);

            this.fsSelection = reader.ReadUInt16();
            this.usFirstCharIndex = reader.ReadUInt16();
            this.usLastCharIndex = reader.ReadUInt16();

            this.sTypoAscender = reader.ReadInt16();
            this.sTypoDescender = reader.ReadInt16();
            this.sTypoLineGap = reader.ReadInt16();

            // WORDSNET-9896 Spec says that these values are unsigned and GDI+ returns them as unsigned too.
            // Despite that MW seems to read them as signed. Do the same for now.
            this.usWinAscent = reader.ReadInt16();
            this.usWinDescent = reader.ReadInt16();

            // WORDSNET-15710 Seems that MW and Windows consider the font invalid when usWinAscent + usWinDescent = 0.
            if (usWinAscent + usWinDescent == 0)
                throw new InvalidOperationException("Invalid font metrics.");

            // The following attributes are available only in True Type Font Version greater than 0.
            if (this.version <= 0)
                return;

            uint ulCodePageRange1 = reader.ReadUInt32();
            uint ulCodePageRange2 = reader.ReadUInt32();
            ulCodePageRanges = new FontCodepageRanges(ulCodePageRange1, ulCodePageRange2);

            // The following attributes are available only in True Type Font Version greater than 1.
            if (this.version <= 1)
                return;

            this.sxHeight = reader.ReadInt16();
            this.sCapHeight = reader.ReadInt16();

            this.usDefaultChar = reader.ReadUInt16();
            this.usBreakChar = reader.ReadUInt16();
            this.usMaxContext = reader.ReadUInt16();
        }

        internal override void Write(BigEndianBinaryWriter writer)
        {
            writer.WriteUInt16(version);
            writer.WriteInt16(xAvgCharWidth);
            writer.WriteUInt16(usWeightClass);
            writer.WriteUInt16(usWidthClass);
            writer.WriteUInt16(fsType.Value);

            writer.WriteInt16(ySubscriptXSize);
            writer.WriteInt16(ySubscriptYSize);
            writer.WriteInt16(ySubscriptXOffset);
            writer.WriteInt16(ySubscriptYOffset);

            writer.WriteInt16(ySuperscriptXSize);
            writer.WriteInt16(ySuperscriptYSize);
            writer.WriteInt16(ySuperscriptXOffset);
            writer.WriteInt16(ySuperscriptYOffset);

            writer.WriteInt16(yStrikeoutSize);
            writer.WriteInt16(yStrikeoutPosition);
            writer.WriteInt16(sFamilyClass);

            writer.WriteBytes(panose.Values);

            writer.WriteUInt32(ulUnicodeRanges.Range1);
            writer.WriteUInt32(ulUnicodeRanges.Range2);
            writer.WriteUInt32(ulUnicodeRanges.Range3);
            writer.WriteUInt32(ulUnicodeRanges.Range4);

            writer.WriteBytes(achVendID);

            writer.WriteUInt16(fsSelection);
            writer.WriteUInt16(usFirstCharIndex);
            writer.WriteUInt16(usLastCharIndex);

            writer.WriteInt16(sTypoAscender);
            writer.WriteInt16(sTypoDescender);
            writer.WriteInt16(sTypoLineGap);

            writer.WriteInt16(usWinAscent);
            writer.WriteInt16(usWinDescent);

            // The following attributes are available only in True Type Font Version greater than 0.
            if (version <= 0)
                return;

            writer.WriteUInt32(ulCodePageRanges.Range1);
            writer.WriteUInt32(ulCodePageRanges.Range2);

            // The following attributes are available only in True Type Font Version greater than 1.
            if (version <= 1)
                return;

            writer.WriteInt16(sxHeight);
            writer.WriteInt16(sCapHeight);

            writer.WriteUInt16(usDefaultChar);
            writer.WriteUInt16(usBreakChar);
            writer.WriteUInt16(usMaxContext);
        }

        internal void FixVersionCompatibility()
        {
            // WORDSNET-10817 fsSelection bits 7-9 are allowed only in version 4.
            if ((fsSelection & ~FsSelectionVersion0Mask) != 0)
            {
                if (version <= 1)
                    // Version 0 and 1 has different layout than version 4. So remove prohibited bits to fix the problem.
                    fsSelection = (ushort)(fsSelection & FsSelectionVersion0Mask);
                else
                    // Version 2, 3 and 4 have the same layout. So we could simply update the version to 4 in this case.
                    // Moreover experiments shows that MW considers bit 7 (USE_TYPO_METRICS) in version 3.
                    version = 4;
            }
        }

        private const ushort FsSelectionUseTypoMetricsMask = 0x0080;
        private const ushort FsSelectionVersion0Mask = 0x003F;

        public const int SemiBoldWeightValue = 600;
        public const int BoldWeightValue = 700;
        public const int RegularWeightValue = 400;
    }
}
