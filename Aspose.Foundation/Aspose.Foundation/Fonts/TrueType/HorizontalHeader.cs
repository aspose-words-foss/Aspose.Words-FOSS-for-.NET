// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/09/2005 by Roman Korchagin

using System;
using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// hhea - Horizontal Header
    ///
    /// This table contains information for horizontal layout.
    /// </summary>
    internal class HorizontalHeader : TTTable
    {
        /// <summary>
        /// 0x00010000 for version 1.0.
        /// </summary>
        internal uint Version;
        /// <summary>
        /// Typographic ascent. Apple specific.
        /// </summary>
        internal short Ascender;
        /// <summary>
        /// Typographic descent. Apple specific.
        /// </summary>
        internal short Descender;
        /// <summary>
        /// Typographic line gap. Apple specific.
        /// </summary>
        internal short LineGap;
        /// <summary>
        /// Maximum advance width value in 'hmtx' table.
        /// </summary>
        internal ushort AdvanceWidthMax;
        /// <summary>
        /// Minimum left sidebearing value in 'hmtx' table.
        /// </summary>
        internal short MinLeftSideBearing;
        /// <summary>
        /// Minimum right sidebearing value; calculated as Min(aw - lsb - (xMax - xMin)).
        /// </summary>
        internal short MinRightSideBearing;
        /// <summary>
        /// Max(lsb + (xMax - xMin)).
        /// </summary>
        internal short XMaxExtent;
        /// <summary>
        /// Used to calculate the slope of the cursor (rise/run); 1 for vertical.
        /// </summary>
        internal short CaretSlopeRise;
        /// <summary>
        /// 0 for vertical.
        /// </summary>
        internal short CaretSlopeRun;
        /// <summary>
        /// The amount by which a slanted highlight on a glyph needs to be shifted to produce the best appearance. Set to 0 for non-slanted fonts.
        /// </summary>
        internal short CaretOffset;
        /// <summary>
        /// reserved
        /// </summary>
        internal short r1;
        /// <summary>
        /// reserved
        /// </summary>
        internal short r2;
        /// <summary>
        /// reserved
        /// </summary>
        internal short r3;
        /// <summary>
        /// reserved
        /// </summary>
        internal short r4;
        /// <summary>
        /// 0 for current format.
        /// </summary>
        internal short MetricDataFormat;
        /// <summary>
        /// Number of hMetric entries in 'hmtx' table.
        /// </summary>
        internal ushort NumberOfHMetrics;

        internal HorizontalHeader(BigEndianBinaryReader reader)
        {
            Version = reader.ReadUInt32();
            // WORDSNET-Test27393 Version 2 is not defined in OTF spec but in this case there is legacy Taiwan font
            // with version 2 which is handled well by MW and Windows.
            if (Version != 0x00010000 && Version != 0x00020000)
                throw new NotSupportedException ("Unsupported horizontal header version.");

            Ascender = reader.ReadInt16();
            Descender = reader.ReadInt16();
            LineGap = reader.ReadInt16();
            AdvanceWidthMax = reader.ReadUInt16();

            MinLeftSideBearing = reader.ReadInt16();
            MinRightSideBearing = reader.ReadInt16();
            XMaxExtent = reader.ReadInt16();

            CaretSlopeRise = reader.ReadInt16();
            CaretSlopeRun = reader.ReadInt16();
            CaretOffset = reader.ReadInt16();

            r1 = reader.ReadInt16();
            r2 = reader.ReadInt16();
            r3 = reader.ReadInt16();
            r4 = reader.ReadInt16();

            MetricDataFormat = reader.ReadInt16();
            NumberOfHMetrics = reader.ReadUInt16();
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
            writer.WriteInt16(AdvanceWidthMax);

            writer.WriteInt16(MinLeftSideBearing);
            writer.WriteInt16(MinRightSideBearing);
            writer.WriteInt16(XMaxExtent);

            writer.WriteInt16(CaretSlopeRise);
            writer.WriteInt16(CaretSlopeRun);
            writer.WriteInt16(CaretOffset);

            writer.WriteInt16(r1);
            writer.WriteInt16(r2);
            writer.WriteInt16(r3);
            writer.WriteInt16(r4);

            writer.WriteInt16(MetricDataFormat);
            writer.WriteInt16(NumberOfHMetrics);
        }
    }
}
