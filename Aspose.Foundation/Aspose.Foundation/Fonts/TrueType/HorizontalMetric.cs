// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/05/2006 by Roman Korchagin

using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Corresponds to longHorMetric in the TTF file.
    /// </summary>
    internal struct HorizontalMetric
    {
        internal HorizontalMetric(short advanceWidth, short leftSideBearing)
        {
            AdvanceWidth = advanceWidth;
            LeftSideBearing = leftSideBearing;
        }

        internal HorizontalMetric(BigEndianBinaryReader reader)
        {
            // kvk: Specification says that AdvanceWidth is USHORT but MS Word and FontLab Studio reads this value as SHORT.
            AdvanceWidth = reader.ReadInt16();
            LeftSideBearing = reader.ReadInt16();
        }

        internal void Write(BigEndianBinaryWriter writer)
        {
            writer.WriteInt16(AdvanceWidth);
            writer.WriteInt16(LeftSideBearing);
        }

        public short AdvanceWidth { get; }
        public short LeftSideBearing { get; }
    }
}
