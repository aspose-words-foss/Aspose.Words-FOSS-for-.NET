// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/04/2019 by Konstantin Kornilov

using System.IO;
using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Helps to build 'glyf' and 'loca' tables.
    /// </summary>
    /// <remarks>
    /// Individual glyphs can be written either by <see cref="WriteGlyph"/> method or by calling <see cref="StartGlyph"/>
    /// method and then writing the glyph data directly to the <see cref="GlyfStream"/>/<see cref="GlyfWriter"/>.
    /// At the end to the table you should call <see cref="EndTable"/> method.
    /// To get the 'loca' output you should set the <see cref="TTLocaTable.IsLocaShort"/> field on the <see cref="LocaTable"/>
    /// and call <see cref="BuildLocaStream"/> method.
    /// Output is in the <see cref="GlyfStream"/> and <see cref="LocaStream"/>.
    /// </remarks>
    internal class GlyfTableBuilder
    {
        public GlyfTableBuilder(bool padToLocaShort)
        {
            mPadToLocaShort = padToLocaShort;
            GlyfStream = new MemoryStream();
            GlyfWriter = new BigEndianBinaryWriter(GlyfStream);
            LocaTable = new TTLocaTable();
            LocaStream = new MemoryStream();
        }

        public void WriteEmptyGlyph()
        {
            StartGlyph();
        }

        public void WriteGlyph(byte[] glyphData)
        {
            StartGlyph();
            GlyfStream.Write(glyphData, 0, glyphData.Length);
            if(mPadToLocaShort && GlyfStream.Length % 2 == 1)
                GlyfStream.WriteByte(0);
        }

        public void StartGlyph()
        {
            LocaTable.GlyphLocations.Add((int)GlyfStream.Position);
        }

        public void EndTable()
        {
            LocaTable.GlyphLocations.Add((int)GlyfStream.Position);
        }

        public void BuildLocaStream()
        {
            BigEndianBinaryWriter writer = new BigEndianBinaryWriter(LocaStream);
            LocaTable.Write(writer);
        }

        public MemoryStream GlyfStream { get; }

        public BigEndianBinaryWriter GlyfWriter { get; }

        public TTLocaTable LocaTable { get; }

        public MemoryStream LocaStream { get; }

        private readonly bool mPadToLocaShort;
    }
}
