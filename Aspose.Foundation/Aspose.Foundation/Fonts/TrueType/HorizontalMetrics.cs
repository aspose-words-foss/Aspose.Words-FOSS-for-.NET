// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/05/2006 by Roman Korchagin

using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// hmtx - Horizontal Metrics
    ///
    /// Contains widths of the glyphs and their left side bearings.
    /// </summary>
    internal class HorizontalMetrics : TTTable
    {
        internal HorizontalMetrics(HorizontalMetric[] widthPairs, short[] leftSideBearings)
        {
            WidthPairs = widthPairs;
            LeftSideBearings = leftSideBearings;
        }

        /// <summary>
        /// Paired advance width and left side bearing values for each glyph in 1000th of the font size.
        /// Index is the glyph index.
        /// Last entry applies to all subsequent glyphs.
        /// If the font is monospaced, only one entry will be in this array.
        /// </summary>
        internal HorizontalMetric[] WidthPairs { get; private set; }
        /// <summary>
        /// Here the advanceWidth is assumed to be the same as the advanceWidth for the last entry above.
        /// The number of entries in this array is derived from numGlyphs (from 'maxp' table)
        /// minus numberOfHMetrics. This generally is used with a run of monospaced glyphs
        /// (e.g., Kanji fonts or Courier fonts). Only one run is allowed and it must be at the end.
        /// This allows a monospaced font to vary the left side bearing values for each glyph.
        /// </summary>
        internal short[] LeftSideBearings { get; private set; }

        /// <summary>
        /// Ctor. Reads the table.
        /// </summary>
        internal static HorizontalMetrics Read(BigEndianBinaryReader reader, int hMetricCount, int glyphCount)
        {
            HorizontalMetric[] widthPairs = new HorizontalMetric[hMetricCount];
            for (int i = 0; i < widthPairs.Length; i++)
                widthPairs[i] = new HorizontalMetric(reader);

            // Read array of left side bearings at the end of the table.
            int bearingsCount = glyphCount - hMetricCount;
            short[] leftSideBearings = new short[bearingsCount];
            for (int i = 0; i < leftSideBearings.Length; i++)
                leftSideBearings[i] = reader.ReadInt16();
            return new HorizontalMetrics(widthPairs, leftSideBearings);
        }

        /// <summary>
        /// Gets width for the specified glyph index.
        /// </summary>
        internal HorizontalMetric GetHMetric(int glyphIndex)
        {
            //There could be fewer glyph widths than glyphs,
            //in this case use the last glyph width according to the spec.
            if (glyphIndex < WidthPairs.Length)
            {
                return WidthPairs[glyphIndex];
            }

            HorizontalMetric lastMetric = WidthPairs[WidthPairs.Length - 1];
            short leftSideBearing = LeftSideBearings[glyphIndex - WidthPairs.Length];
            return new HorizontalMetric(lastMetric.AdvanceWidth, leftSideBearing);
        }

        internal void UpdateMetrics(HorizontalMetric[] metrics)
        {
            Debug.Assert(metrics.Length == WidthPairs.Length + LeftSideBearings.Length);
            WidthPairs = metrics;
            LeftSideBearings = new short[0];
        }

        internal void UpdateLeftSideBearing(int glyphIndex, short leftSideBearing)
        {
            if (glyphIndex < WidthPairs.Length)
                WidthPairs[glyphIndex] = new HorizontalMetric(WidthPairs[glyphIndex].AdvanceWidth, leftSideBearing);
            else if (glyphIndex < WidthPairs.Length + LeftSideBearings.Length)
                LeftSideBearings[glyphIndex - WidthPairs.Length] = leftSideBearing;
            else
                Debug.Fail("Invalid glyph index");
        }

        internal override void Write(BigEndianBinaryWriter writer)
        {
            foreach (HorizontalMetric widthPair in WidthPairs)
                widthPair.Write(writer);

            foreach (short leftSideBearing in LeftSideBearings)
                writer.WriteUInt16(leftSideBearing);
        }

        public HorizontalMetrics Clone()
        {
            return new HorizontalMetrics(WidthPairs, LeftSideBearings);
        }
    }
}
