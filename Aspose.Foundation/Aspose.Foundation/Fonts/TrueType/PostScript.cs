// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/06/2009 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Text;
using Aspose.Collections.Generic;
using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// http://www.microsoft.com/typography/otspec/post.htm
    ///
    /// Corresponds to the "post" table in a TTF file. Contains information about PostScript glyph names.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    internal class PostScript : TTTable
    {
        // --- common fields
        internal int Version;
        internal int ItalicAngle;
        internal short UnderlinePosition;
        internal short UnderlineThickness;
        internal int IsFixedPitch;
        internal int MinMemType42;
        internal int MaxMemType42;
        internal int MinMemType1;
        internal int MaxMemType1;

        /// <summary>
        /// No glyph names stored here.
        /// </summary>
        internal const int Version10 = 0x00010000;
        /// <summary>
        /// This is the version used for fonts on Windows. Names are stored as per the fields below.
        /// </summary>
        internal const int Version20 = 0x00020000;
        /// <summary>
        /// The spec says this format was deprecated. We don't support it.
        /// </summary>
        internal const int Version25 = 0x00025000;
        /// <summary>
        /// No glyph names stored here.
        /// </summary>
        internal const int Version30 = 0x00030000;

        // --- fields for Version20
        /// <summary>
        /// Number of glyphs (this should be the same as numGlyphs in 'maxp' table).
        /// ushort in file.
        /// </summary>
        internal int NumberOfGlyphs;
        /// <summary>
        /// Each item in this array corresponds to a glyph. E.g. item 0 is for glyph index 0.
        /// The value in this array is an index into the names table.
        /// Null for other versions.
        ///
        /// The glyph name array maps the glyphs in this font to name index. If the name index is
        /// between 0 and 257, treat the name index as a glyph index in the Macintosh standard order.
        /// If the name index is between 258 and 32767, then subtract 258 and use that to index into
        /// the list of Pascal strings at the end of the table. Thus a given font may map some of its
        /// glyphs to the standard glyph names, and some to its own names.
        /// Index numbers 32768 through 65535 are reserved for future use. If you do not want to associate
        /// a PostScript name with a particular glyph, use index number 0 which points the name .notdef.
        ///
        /// ushort[] in file.
        /// </summary>
        internal int[] GlyphNameIndexes;
        /// <summary>
        /// Glyph names.
        /// Null for other versions.
        /// </summary>
        internal IList<string> Names;


        internal PostScript(BigEndianBinaryReader reader, long tableLength)
        {
            long tableStartPosition = reader.BaseStream.Position;
            Version = reader.ReadInt32();   // uint in file
            ItalicAngle = reader.ReadInt32();   // uint in file
            UnderlinePosition = reader.ReadInt16();
            UnderlineThickness = reader.ReadInt16();
            IsFixedPitch = reader.ReadInt32();  // uint in file
            MinMemType42 = reader.ReadInt32();  // uint in file
            MaxMemType42 = reader.ReadInt32();  // uint in file
            MinMemType1 = reader.ReadInt32();   // uint in file
            MaxMemType1 = reader.ReadInt32();   // uint in file

            switch (Version)
            {
                case Version20:
                {
                    NumberOfGlyphs = reader.ReadUInt16();

                    GlyphNameIndexes = new int[NumberOfGlyphs];
                    int maxGlyphNameIndex = 0;
                    for (int i = 0; i < GlyphNameIndexes.Length; i++)
                    {
                        ushort index = reader.ReadUInt16();
                        GlyphNameIndexes[i] = index;

                        // According to the spec, indexes over 32767 are reserved.
                        if (index <= 32767)
                            maxGlyphNameIndex = Math.Max(index, maxGlyphNameIndex);
                    }

                    // RK First 258 indexes are standard system glyph names that are not stored on file.
                    // Therefore need to subtract 258 to use as an index into the name array.
                    // Add one to turn max index into a count of items in the array.
                    int nameCount = maxGlyphNameIndex - StandardMacintoshGlyphCount + 1;
                    // RESILIENCY 15704 - nameCount cannot be negative. So set it to zero if it is negative.
                    nameCount = nameCount < 0 ? 0 : nameCount;
                    Names = new List<string>(nameCount);

                    int nameIndex = StandardMacintoshGlyphCount;
                    while (true)
                    {
                        // There is no need to read names that are not referenced even if they are present in the table.
                        if (nameIndex > maxGlyphNameIndex)
                            break;

                        // WORDSNET-10428 Stop reading when we reach the end of the table.
                        if (reader.BaseStream.Position >= tableStartPosition + tableLength)
                            break;

                        Names.Add(ReadPascalString(reader));
                        nameIndex++;
                    }

                    break;
                }
                case Version10:
                case Version30:
                    // No PostScript names are provided in this file.
                    break;
                default:
                    // WORDSNET-26737 Custom font with version 2.8. For now just change the version to 3.0 (which is the
                    // simplest version). But maybe it will be better to ignore the parsed values and set some
                    // default values. Other option is to ignore the table at all and not include it into subsets,
                    // but it seems that this table is required.
                    Version = Version30;
                    break;
            }
        }

        internal override void Write(BigEndianBinaryWriter writer)
        {
            writer.WriteInt32(Version); //uint in file
            writer.WriteInt32(ItalicAngle); // uint in file
            writer.WriteInt16(UnderlinePosition);
            writer.WriteInt16(UnderlineThickness);
            writer.WriteInt32(IsFixedPitch);    // uint in file
            writer.WriteInt32(MinMemType42);    // uint in file
            writer.WriteInt32(MaxMemType42);    // uint in file
            writer.WriteInt32(MinMemType1);     // uint in file
            writer.WriteInt32(MaxMemType1);     // uint in file

            switch (Version)
            {
                case Version20:
                {
                    writer.WriteInt16(NumberOfGlyphs);

                    foreach (int index in GlyphNameIndexes)
                        writer.WriteInt16(index);

                    for (int i = 0; i < Names.Count; i++)
                        WritePascalString(Names[i], writer);

                    break;
                }
                case Version10:
                case Version30:
                    // No PostScript names are provided in this file.
                    break;
                default:
                    throw new InvalidOperationException("Unexpected PostScript table version.");
            }
        }

        /// <summary>
        /// Updates the table so it is valid only for the specified subset of glyphs.
        /// </summary>
        /// <param name="usedGlyphs">The glyphs to keep in this font. The key is the old glyph index.
        /// The value is the new glyph index.</param>
        internal void Subset(SortedIntegerListGeneric<int> usedGlyphs)
        {
            // It is easier for us to set these all to zeroes so the print driver figures it out for itself.
            MinMemType42 = 0;
            MaxMemType42 = 0;
            MinMemType1 = 0;
            MaxMemType1 = 0;

            switch (Version)
            {
                case Version20:
                    SubsetVersion20Fields(usedGlyphs);
                    break;
                case Version10:
                case Version30:
                    // No PostScript names are provided in this file.
                    break;
                default:
                    throw new InvalidOperationException("Unexpected PostScript table version.");
            }
        }

        private void SubsetVersion20Fields(SortedIntegerListGeneric<int> usedGlyphs)
        {
            int[] newGlyphNameIndexes = new int[usedGlyphs.Count];
            List<string> newNames = new List<string>();

            // This loop copies only used glyph names into new arrays.
            for (int i = 0; i < usedGlyphs.Count; i++)
            {
                int oldGlyphIndex = usedGlyphs.GetKey(i);
                int newGlyphIndex = usedGlyphs.GetByIndex(i);

                // Get the glyph name for the old glyph index.
                // WORDSNET-21910 The NumberOfGlyphs in post table is less than actual number of glyphs in the font
                // for subsets embedded into the customer document.
                int oldNameIndex = oldGlyphIndex < NumberOfGlyphs ? GlyphNameIndexes[oldGlyphIndex] : 0;

                if ((oldNameIndex < StandardMacintoshGlyphCount) || (oldNameIndex > 32767))
                {
                    // System or reserved value. Names for these are not stored in the file.
                    newGlyphNameIndexes[newGlyphIndex] = (ushort) oldNameIndex;
                    continue;
                }

                // For values 258..32767 need to subtract 258 and then use as an index into the name array.
                int oldNameIndexNormalized = oldNameIndex - StandardMacintoshGlyphCount;

                // WORDSNET-10428 If there is no name for the glyph in the file then set it to 0.
                if (oldNameIndexNormalized >= Names.Count)
                {
                    newGlyphNameIndexes[newGlyphIndex] = 0;
                    continue;
                }

                string name = Names[oldNameIndexNormalized];

                // Store the name and its index in the new array.
                newNames.Add(name);
                int newNameIndex = newNames.Count - 1;
                newGlyphNameIndexes[newGlyphIndex] = (ushort) (newNameIndex + StandardMacintoshGlyphCount);
            }

            NumberOfGlyphs = (ushort)usedGlyphs.Count;
            GlyphNameIndexes = newGlyphNameIndexes;
            Names = newNames;
        }

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const int StandardMacintoshGlyphCount = 258;

        private static string ReadPascalString(BigEndianBinaryReader reader)
        {
            int charCount = reader.ReadByte();

            if (reader.BaseStream.Position + charCount > reader.BaseStream.Length)
                charCount = (int)(reader.BaseStream.Length - reader.BaseStream.Position);

            byte[] data = reader.ReadBytes(charCount);

            Encoding encoding = Encoding.ASCII;
            return encoding.GetString(data);
        }

        private static void WritePascalString(string s, BigEndianBinaryWriter writer)
        {
            if (s.Length > 255)
                throw new InvalidOperationException("The PostScript glyph name is too long.");

            Encoding encoding = Encoding.ASCII;
            byte[] data = encoding.GetBytes(s);

            writer.WriteByte((byte)data.Length);
            writer.WriteBytes(data, 0, data.Length);
        }
    }
}
