// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/02/2015 by Denis Shvydkiy

using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// vmtx - Vertical Metrics.
    /// http://www.microsoft.com/typography/otspec/vmtx.htm
    ///
    /// Contains heights of the glyphs and their top side bearings.
    /// </summary>
    internal class VerticalMetrics : TTTable
    {
        /// <summary>
        /// Paired advance height and top side bearing values for each glyph in 1000th of the font size.
        /// Index is the glyph index.
        /// Last entry applies to all subsequent glyphs.
        /// If the font is monospaced, only one entry will be in this array.
        /// </summary>
        internal VerticalMetric[] HeightPairs;
        /// <summary>
        /// Here the advance height is assumed to be the same as the advance height for the last entry above.
        /// The number of entries in this array is derived from numGlyphs (from 'maxp' table)
        /// minus numberOfVMetrics. This generally is used with a run of monospaced glyphs
        /// (e.g., Kanji fonts or Courier fonts). Only one run is allowed and it must be at the end.
        /// This allows a monospaced font to vary the top side bearing values for each glyph.
        /// </summary>
        internal short[] TopSideBearings;

        /// <summary>
        /// Ctor. Reads the table.
        /// </summary>
        internal VerticalMetrics(BigEndianBinaryReader reader, int vMetricCount, int glyphCount)
        {
            // WORDSNET-25754 Customer font has malformed vmtx table. Number of metrics is larger than glyph count.
            if (vMetricCount > glyphCount)
                vMetricCount = glyphCount;

            HeightPairs = new VerticalMetric[vMetricCount];
            for (int i = 0; i < HeightPairs.Length; i++)
                HeightPairs[i] = new VerticalMetric(reader);

            // Read array of top side bearings at the end of the table.
            int bearingsCount = glyphCount - vMetricCount;
            TopSideBearings = new short[bearingsCount];
            for (int i = 0; i < TopSideBearings.Length; i++)
                TopSideBearings[i] = reader.ReadInt16();
        }

        /// <summary>
        /// Gets height for the specified glyph index.
        /// </summary>
        internal VerticalMetric GetVMetric(int glyphIndex)
        {
            //There could be fewer glyph heights than glyphs,
            //in this case use the last glyph height according to the spec.
            if (glyphIndex < HeightPairs.Length)
            {
                return HeightPairs[glyphIndex];
            }
            else
            {
                VerticalMetric lastMetric = HeightPairs[HeightPairs.Length - 1];
                short topSideBearing = TopSideBearings[glyphIndex - HeightPairs.Length];
                return new VerticalMetric(lastMetric.AdvanceHeight, topSideBearing);
            }
        }

        internal override void Write(BigEndianBinaryWriter writer)
        {
            foreach (VerticalMetric pair in HeightPairs)
                pair.Write(writer);

            foreach (short sideBearing in TopSideBearings)
                writer.WriteUInt16(sideBearing);
        }
    }
}
