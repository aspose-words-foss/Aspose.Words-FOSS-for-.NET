// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/02/2015 by Denis Shvydkiy

using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// vhea - Vertical Header
    ///
    /// This table contains information for vertical layout.
    /// </summary>
    internal class VerticalHeader : TTTable
    {
        /// <summary>
        /// version 1.0: 0x00010000.
        /// version 1.1: 0x00011000.
        /// </summary>
        internal uint Version;
        /// <summary>
        /// version 1.0: Distance in FUnits from the centerline to the previous line’s descent.
        /// version 1.1: The vertical typographic ascender for this font.
        ///              It is the distance in FUnits from the ideographic em-box center baseline
        ///              for the vertical axis to the right of the ideographic em-box and is usually set to (head.unitsPerEm)/2.
        /// </summary>
        internal short Ascender;
        /// <summary>
        /// version 1.0: Distance in FUnits from the centerline to the next line’s ascent.
        /// version 1.1: The vertical typographic descender for this font.
        ///              It is the distance in FUnits from the ideographic em-box center baseline
        ///              for the horizontal axis to the left of the ideographic em-box and is usually set to (head.unitsPerEm)/2.
        /// </summary>
        internal short Descender;
        /// <summary>
        /// version 1.0: Reserved; set to 0
        /// version 1.1: The vertical typographic gap for this font.
        ///              An application can determine the recommended line spacing for single spaced vertical text
        ///              for an OpenType font by the following expression: ideo embox width + vhea.vertTypoLineGap.
        /// </summary>
        internal short LineGap;
        /// <summary>
        /// version 1.0 and 1.1: The maximum advance height measurement found in the font.
        /// </summary>
        internal ushort AdvanceHeightMax;
        /// <summary>
        /// version 1.0 and 1.1: The minimum top sidebearing measurement found in the font.
        /// </summary>
        internal short MinTopSideBearing;
        /// <summary>
        /// version 1.0 and 1.1: The minimum bottom sidebearing measurement found in the font.
        /// </summary>
        internal short MinBottomSideBearing;
        /// <summary>
        /// version 1.0 and 1.1: YMaxExtent = MinTopSideBearing + (yMax - yMin).
        /// </summary>
        internal short YMaxExtent;
        /// <summary>
        /// version 1.0 and 1.1: The value of the caretSlopeRise field divided by the value of the caretSlopeRun Field determines the slope of the caret.
        /// A value of 0 for the rise and a value of 1 for the run specifies a horizontal caret.
        /// A value of 1 for the rise and a value of 0 for the run specifies a vertical caret.
        /// Intermediate values are desirable for fonts whose glyphs are oblique or italic.
        /// For a vertical font, a horizontal caret is best.
        /// </summary>
        internal short CaretSlopeRise;
        /// <summary>
        /// version 1.0 and 1.1: See the CaretSlopeRise field. Value = 1 for nonslanted vertical fonts.
        /// </summary>
        internal short CaretSlopeRun;
        /// <summary>
        /// version 1.0 and 1.1: The amount by which the highlight on a slanted glyph needs to be shifted away from the glyph in order to produce the best appearance.
        /// Set value equal to 0 for nonslanted fonts.
        /// </summary>
        internal short CaretOffset;
        /// <summary>
        /// version 1.0 and 1.1: reserved.
        /// </summary>
        internal short r1;
        /// <summary>
        /// version 1.0 and 1.1: reserved.
        /// </summary>
        internal short r2;
        /// <summary>
        /// version 1.0 and 1.1: reserved.
        /// </summary>
        internal short r3;
        /// <summary>
        /// version 1.0 and 1.1: reserved.
        /// </summary>
        internal short r4;
        /// <summary>
        /// version 1.0 and 1.1: 0 for current format.
        /// </summary>
        internal short MetricDataFormat;
        /// <summary>
        /// version 1.0 and 1.1: Number of advance heights in the vertical metrics table.
        /// </summary>
        internal ushort NumberOfVMetrics;

        internal VerticalHeader(BigEndianBinaryReader reader)
        {
            // WORDSNET-24139, WORDSNET-25217 Customer fonts have invalid 'vmtx' version value. MW seems to be OK with that.
            // Do not validate the version at all for now. But potentially the 'vmtx' table with incorrect version
            // could be ignored.
            Version = reader.ReadUInt32();
            Ascender = reader.ReadInt16();
            Descender = reader.ReadInt16();
            LineGap = reader.ReadInt16();
            AdvanceHeightMax = reader.ReadUInt16();

            MinTopSideBearing = reader.ReadInt16();
            MinBottomSideBearing = reader.ReadInt16();
            YMaxExtent = reader.ReadInt16();

            CaretSlopeRise = reader.ReadInt16();
            CaretSlopeRun = reader.ReadInt16();
            CaretOffset = reader.ReadInt16();

            // reserved
            r1 = reader.ReadInt16();
            r2 = reader.ReadInt16();
            r3 = reader.ReadInt16();
            r4 = reader.ReadInt16();

            MetricDataFormat = reader.ReadInt16();
            NumberOfVMetrics = reader.ReadUInt16();
        }

        /// <summary>
        /// Writes the table.
        /// </summary>
        internal override void Write(BigEndianBinaryWriter writer)
        {
            writer.WriteUInt32(Version);

            writer.WriteInt16(Ascender);
            writer.WriteInt16(Descender);
            writer.WriteInt16(LineGap);
            writer.WriteInt16(AdvanceHeightMax);

            writer.WriteInt16(MinTopSideBearing);
            writer.WriteInt16(MinBottomSideBearing);
            writer.WriteInt16(YMaxExtent);

            writer.WriteInt16(CaretSlopeRise);
            writer.WriteInt16(CaretSlopeRun);
            writer.WriteInt16(CaretOffset);

            writer.WriteInt16(r1);
            writer.WriteInt16(r2);
            writer.WriteInt16(r3);
            writer.WriteInt16(r4);

            writer.WriteInt16(MetricDataFormat);
            writer.WriteInt16(NumberOfVMetrics);
        }
    }
}
