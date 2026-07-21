// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/09/2005 by Roman Korchagin

using System;
using System.Drawing;
using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// head - Font Header
    ///
    /// This table gives global information about the font.
    /// </summary>
    internal class FontHeader : TTTable
    {
        /// <summary>
        /// Read head table from the binary reader.
        /// </summary>
        internal static FontHeader Read(BigEndianBinaryReader reader)
        {
            FontHeader header = new FontHeader();

            header.Version = reader.ReadUInt32();
            // WORDSNET-Test27393 Version 2 is not defined in OTF spec but in this case there is legacy Taiwan font
            // with version 2 which is handled well by MW and Windows.
            if (header.Version != 0x00010000 && header.Version != 0x00020000)
                throw new NotSupportedException("Unsupported font head version.");
            header.Revision = reader.ReadUInt32();
            header.CheckSumAdjustment = reader.ReadUInt32();
            header.MagicNumber = reader.ReadUInt32();
            if (header.MagicNumber != 0x5F0F3CF5)
                throw new NotSupportedException("Unsupported font head magic number.");
            header.Flags = reader.ReadUInt16();
            header.UnitsPerEm = reader.ReadUInt16();
            header.CreatedTime = reader.ReadInt64();
            header.ModifiedTime = reader.ReadInt64();
            header.XMin = reader.ReadInt16();
            header.YMin = reader.ReadInt16();
            header.XMax = reader.ReadInt16();
            header.YMax = reader.ReadInt16();
            header.MacStyle = reader.ReadUInt16();
            header.LowestRecPpem = reader.ReadUInt16();
            header.FontDirectionHint = reader.ReadInt16();
            header.IndexToLocFormat = reader.ReadInt16();
            header.GlyphDataFormat = reader.ReadInt16();

            return header;
        }

        /// <summary>
        /// Writes head table to binary writer.
        /// </summary>
        internal override void Write(BigEndianBinaryWriter writer)
        {
            writer.WriteUInt32(Version);
            writer.WriteUInt32(Revision);
            writer.WriteUInt32(CheckSumAdjustment);
            writer.WriteUInt32(MagicNumber);
            writer.WriteUInt16(Flags);
            writer.WriteUInt16(UnitsPerEm);
            writer.WriteInt64(CreatedTime);
            writer.WriteInt64(ModifiedTime);
            writer.WriteInt16(XMin);
            writer.WriteInt16(YMin);
            writer.WriteInt16(XMax);
            writer.WriteInt16(YMax);
            writer.WriteUInt16(MacStyle);
            writer.WriteUInt16(LowestRecPpem);
            writer.WriteInt16(FontDirectionHint);
            writer.WriteInt16(IndexToLocFormat);
            writer.WriteInt16(GlyphDataFormat);
        }

        /// <summary>
        /// 0x00010000 for version 1.0.
        /// </summary>
        internal uint Version;

        /// <summary>
        /// Set by font manufacturer.
        /// </summary>
        internal uint Revision;

        /// <summary>
        /// To compute: set it to 0, sum the entire font as ULONG, then store 0xB1B0AFBA - sum.
        /// </summary>
        internal uint CheckSumAdjustment;

        /// <summary>
        /// Set to 0x5F0F3CF5.
        /// </summary>
        internal uint MagicNumber;

        /// <summary>
        /// Bit 0: Baseline for font at y=0;
        /// Bit 1: Left sidebearing point at x=0;
        /// Bit 2: Instructions may depend on point size;
        /// Bit 3: Force ppem to integer values for all internal scaler math; may use fractional ppem sizes if this bit is clear;
        /// Bit 4: Instructions may alter advance width (the advance widths might not scale linearly);
        /// Bits 5-10: These should be set according to Apple's specification . However, they are not implemented in OpenType.
        /// Bit 11: Font data is 'lossless,' as a result of having been compressed and decompressed with the Agfa MicroType Express engine.
        /// Bit 12: Font converted (produce compatible metrics)
        /// Bit 13: Font optimized for ClearType
        /// Bit 14: Reserved, set to 0
        /// Bit 15: Reserved, set to 0
        /// </summary>
        internal ushort Flags;

        /// <summary>
        /// Valid range is from 16 to 16384. This value should be a power of 2 for fonts that have TrueType outlines.
        /// </summary>
        internal ushort UnitsPerEm;

        /// <summary>
        /// Number of seconds since 12:00 midnight, January 1, 1904. 64-bit integer.
        /// </summary>
        internal long CreatedTime;

        /// <summary>
        /// Number of seconds since 12:00 midnight, January 1, 1904. 64-bit integer.
        /// </summary>
        internal long ModifiedTime;

        /// <summary>
        /// For all glyph bounding boxes.
        /// </summary>
        internal short XMin;

        /// <summary>
        /// For all glyph bounding boxes.
        /// </summary>
        internal short YMin;

        /// <summary>
        /// For all glyph bounding boxes.
        /// </summary>
        internal short XMax;

        /// <summary>
        /// For all glyph bounding boxes.
        /// </summary>
        internal short YMax;

        /// <summary>
        /// Bit 0: Bold (if set to 1);
        /// Bit 1: Italic (if set to 1)
        /// Bit 2: Underline (if set to 1)
        /// Bit 3: Outline (if set to 1)
        /// Bit 4: Shadow (if set to 1)
        /// Bit 5: Condensed (if set to 1)
        /// Bit 6: Extended (if set to 1)
        /// Bits 7-15: Reserved (set to 0).
        /// </summary>
        internal ushort MacStyle;

        /// <summary>
        /// Smallest readable size in pixels.
        /// </summary>
        internal ushort LowestRecPpem;

        /// <summary>
        /// 0: Fully mixed directional glyphs;
        /// 1: Only strongly left to right;
        /// 2: Like 1 but also contains neutrals;
        /// -1: Only strongly right to left;
        /// -2: Like -1 but also contains neutrals.
        /// </summary>
        internal short FontDirectionHint;

        /// <summary>
        /// 0 for short offsets, 1 for long.
        /// </summary>
        internal short IndexToLocFormat;

        /// <summary>
        /// True if short glyph offsets are used.
        /// </summary>
        internal bool IsLocaShort
        {
            get { return IndexToLocFormat == 0; }
            set { IndexToLocFormat = (short)(value ? 0 : 1); }
        }

        /// <summary>
        /// 0 for current format
        /// </summary>
        internal short GlyphDataFormat;

        internal FontStyle Style
        {
            get
            {
                FontStyle style = 0;
                style |= ((MacStyle & 0x01) != 0) ? FontStyle.Bold : 0;
                style |= ((MacStyle & 0x02) != 0) ? FontStyle.Italic : 0;
                style |= ((MacStyle & 0x04) != 0) ? FontStyle.Underline : 0;
                return style;
            }
        }
    }
}
