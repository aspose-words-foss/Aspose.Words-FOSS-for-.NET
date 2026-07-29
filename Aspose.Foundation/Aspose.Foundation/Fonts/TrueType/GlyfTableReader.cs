// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/04/2019 by Konstantin Kornilov

using System.Drawing;
using Aspose.Fonts.Sfnt;
using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Helps to read 'glyf' table.
    /// </summary>
    internal class GlyfTableReader
    {
        public GlyfTableReader(BigEndianBinaryReader reader, SfntTableRecordEntry locaEntry, SfntTableRecordEntry glyfEntry,
            bool isLocaShort)
        {
            mReader = reader;
            mGlyfEntry = glyfEntry;
            mIsLocaShort = isLocaShort;

            reader.BaseStream.Position = locaEntry.Offset;
            mLocaTable = TTLocaTable.Read(mReader, locaEntry.Length, isLocaShort);
        }

        public void LocateGlyph(int glyphIndex)
        {
            Debug.Assert(glyphIndex < GlyphCount);
            mReader.BaseStream.Position = mGlyfEntry.Offset + mLocaTable.GlyphLocations[glyphIndex];
        }

        public Rectangle ReadGlyphBox(int glyphIndex)
        {
            Debug.Assert(glyphIndex < GlyphCount);
            int glyphLength = GetGlyphLength(glyphIndex);
            if (glyphLength < 10)
            {
                Debug.Assert(glyphLength == 0);
                return Rectangle.Empty;
            }
            LocateGlyph(glyphIndex);
            mReader.ReadInt16(); // numberOfContours
            int xMin = mReader.ReadInt16();
            int yMin = mReader.ReadInt16();
            int xMax = mReader.ReadInt16();
            int yMax = mReader.ReadInt16();
            return Rectangle.FromLTRB(xMin, yMin, xMax, yMax);
        }

        public int GetGlyphLength(int glyphIndex)
        {
            Debug.Assert(glyphIndex < GlyphCount);
            return mLocaTable.GlyphLocations[glyphIndex + 1] - mLocaTable.GlyphLocations[glyphIndex];
        }

        public byte[] GetGlyphBytes(int glyphIndex)
        {
            int length = GetGlyphLength(glyphIndex);

            if(length == 0)
                return new byte[0];

            LocateGlyph(glyphIndex);
            return mReader.ReadBytes(length);
        }

        public int GlyphCount
        {
            get { return mLocaTable.GlyphLocations.Count - 1; }
        }

        public bool IsLocaShort
        {
            get { return mIsLocaShort; }
        }

        private readonly BigEndianBinaryReader mReader;
        private readonly SfntTableRecordEntry mGlyfEntry;
        private readonly bool mIsLocaShort;
        private readonly TTLocaTable mLocaTable;
    }
}
