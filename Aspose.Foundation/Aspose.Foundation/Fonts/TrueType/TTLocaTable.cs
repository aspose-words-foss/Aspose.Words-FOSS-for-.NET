// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov

using System;
using Aspose.Collections;
using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents 'loca' table in OpenType file.
    /// </summary>
    internal class TTLocaTable : TTTable
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public TTLocaTable()
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        public TTLocaTable(bool isLocaShort)
        {
            mIsLocaShort = isLocaShort;
        }

        /// <summary>
        /// Reads loca table from binary reader.
        /// </summary>
        public static TTLocaTable Read(BigEndianBinaryReader reader, uint tableLenght, bool isLocaShort)
        {
            TTLocaTable loca = new TTLocaTable(isLocaShort);

            if (isLocaShort)
            {
                int count = (int)(tableLenght/2);
                for (int i = 0; i < count; i++)
                    loca.GlyphLocations.Add(reader.ReadUInt16()*2);
            }
            else
            {
                int count = (int)(tableLenght/4);
                for (int i = 0; i < count; i++)
                    loca.GlyphLocations.Add(reader.ReadInt32());
            }

            return loca;
        }

        /// <summary>
        /// Writes loca table to binary writer.
        /// </summary>
        internal override void Write(BigEndianBinaryWriter writer)
        {
            for (int i = 0; i < GlyphLocations.Count; i++)
            {
                if (IsLocaShort)
                    writer.WriteInt16(GlyphLocations[i]/2);
                else
                    writer.WriteInt32(GlyphLocations[i]);
            }
        }

        /// <summary>
        /// Chooses loca table format according to glyphs offset values.
        /// </summary>
        public void ChooseFormat()
        {
            int maxOffset = 0;
            for (int i = 0; i < GlyphLocations.Count; i++)
            {
                if (GlyphLocations[i]%2 != 0)
                {
                    // In short loca only even offsets could be defined.
                    IsLocaShort = false;
                    return;
                }
                maxOffset = Math.Max(maxOffset, GlyphLocations[i]);
            }

            IsLocaShort = (maxOffset <= MaxShortLocaOffset);
        }

        /// <summary>
        /// True if short glyph offsets are used.
        /// </summary>
        public bool IsLocaShort
        {
            get { return mIsLocaShort; }
            set { mIsLocaShort = value; }
        }

        /// <summary>
        /// Relative offsets of glyph records in glyph table.
        /// </summary>
        public IntList GlyphLocations
        {
            get { return mGlyphLocations; }
        }

        private readonly IntList mGlyphLocations = IntList.CreateAllocated();
        private bool mIsLocaShort;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int MaxShortLocaOffset = ushort.MaxValue*2;
    }
}
