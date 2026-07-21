// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/02/2015 by Denis Shvydkiy

using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Entry in the vertical metrics array.
    /// http://www.microsoft.com/typography/otspec/vmtx.htm
    /// </summary>
    internal struct VerticalMetric
    {
        // kvk: Specification says that AdvanceHeight is USHORT but MS Word and FontLab Studio reads this value as SHORT.
        internal short AdvanceHeight;
        internal short TopSideBearing;

        internal VerticalMetric(short advanceHeight, short topSideBearing)
        {
            AdvanceHeight = advanceHeight;
            TopSideBearing = topSideBearing;
        }

        internal VerticalMetric(BigEndianBinaryReader reader)
        {
            AdvanceHeight = reader.ReadInt16();
            TopSideBearing = reader.ReadInt16();
        }

        internal void Write(BigEndianBinaryWriter writer)
        {
            writer.WriteInt16(AdvanceHeight);
            writer.WriteInt16(TopSideBearing);
        }
    }
}
