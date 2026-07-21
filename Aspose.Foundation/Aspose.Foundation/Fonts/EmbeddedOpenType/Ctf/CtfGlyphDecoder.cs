// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov

using System.IO;
using Aspose.Collections;
using Aspose.Fonts.Sfnt;
using Aspose.Fonts.TrueType;
using Aspose.IO;

namespace Aspose.Fonts.EmbeddedOpenType.Ctf
{
    /// <summary>
    /// Allows to decode CTF Glyph table.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/Submission/2008/SUBM-MTX-20080305/#glyfTable for more info.
    /// </remarks>
    internal static class CtfGlyphDecoder
    {
        /// <summary>
        /// Decodes glyph data and writes glyf and loca tables to sfnt builder.
        /// </summary>
        /// <remarks>
        /// Head table could be updated during decoding.
        /// </remarks>
        internal static void DecodeGlyphData(
            SfntBuilder sfntBuilder,
            byte[] glyphTableData,
            byte[] glyphPushData,
            byte[] glyphInstructions,
            FontHeader head)
        {
            using (MemoryStream
                glyphTableStream = new MemoryStream(glyphTableData),
                pushDataStream = new MemoryStream(glyphPushData),
                instructionStream = new MemoryStream(glyphInstructions))
            {
                CtfGlyphDecoderContext context = new CtfGlyphDecoderContext(glyphTableStream, pushDataStream, instructionStream);

                using (MemoryStream glyfTable = new MemoryStream())
                {
                    BigEndianBinaryWriter writer = new BigEndianBinaryWriter(glyfTable);

                    TTLocaTable loca = new TTLocaTable();

                    while (glyphTableStream.Position < glyphTableStream.Length)
                    {
                        // Loca table contains start positions of each glyph relative to glyf table start.
                        loca.GlyphLocations.Add((int)glyfTable.Position);

                        GlyfTableRecord glyph = DecodeGlyph(context);

                        if (!glyph.IsEmpty)
                            glyph.Write(writer);

                        // Specification says:
                        // Local offsets should be long-aligned, i.e., multiples of 4.
                        // Offsets which are not long-aligned may seriously degrade performance of some processors.
                        StreamUtil.SeekToNextPage(glyfTable, 4);
                    }

                    // Specification says:
                    // In the particular case of the last glyph(s), loca[n] will be equal the length of the
                    // glyph data ('glyf') table.
                    loca.GlyphLocations.Add((int)glyfTable.Position);

                    // If original font uses short loca we could exceed short values limit by producing more
                    // glyph data. So it is better to define which loca version to use after writing glyph data.
                    // Or we could optimize glyph writing.
                    loca.ChooseFormat();
                    head.IsLocaShort = loca.IsLocaShort;

                    sfntBuilder.AddTable(OpenTypeTableTag.Glyf, glyfTable.ToArray());
                    sfntBuilder.AddTable(OpenTypeTableTag.Loca, loca.ToByteArray());
                }
            }
        }

        /// <summary>
        /// Decodes single glyph.
        /// </summary>
        private static GlyfTableRecord DecodeGlyph( CtfGlyphDecoderContext context)
        {
            short numContours = context.GlyphTableReader.ReadInt16();
            context.GlyphTableReader.BaseStream.Position -= 2;

            GlyfTableRecord glyph = numContours >= 0
                ? DecodeSimpleGlyph(context)
                : DecodeCompositeGlyph(context);

            return glyph;
        }

        /// <summary>
        /// Decodes composite glyph from CTF format.
        /// </summary>
        private static GlyfTableRecord DecodeCompositeGlyph( CtfGlyphDecoderContext context)
        {
            short numContours = context.GlyphTableReader.ReadInt16();
            CompositeGlyfTableRecord glyf = new CompositeGlyfTableRecord();
            glyf.NumContours = numContours;
            glyf.XMin = context.GlyphTableReader.ReadInt16();
            glyf.YMin = context.GlyphTableReader.ReadInt16();
            glyf.XMax = context.GlyphTableReader.ReadInt16();
            glyf.YMax = context.GlyphTableReader.ReadInt16();

            // Internal data of composite glyph is not encoded so we can read it as TrueType data.
            glyf.ReadComponents(context.GlyphTableReader);

            if (glyf.WeHaveInstructions)
                glyf.Instructions = DecodeInstructions(context);

            return glyf;
        }

        /// <summary>
        /// Decodes simple glyph from CTF format.
        /// </summary>
        internal static GlyfTableRecord DecodeSimpleGlyph(CtfGlyphDecoderContext context)
        {
            short numContours = context.GlyphTableReader.ReadInt16();

            SimpleGlyfTableRecord glyph = new SimpleGlyfTableRecord();

            // When numContours = 0x7FFF actual number of contours and bounding box are written further.
            // Otherwise bounding box should be calculated from points coordinates.
            bool isBoundingBoxPresent = (numContours == 0x7FFF);
            if (isBoundingBoxPresent)
            {
                glyph.NumContours = context.GlyphTableReader.ReadInt16();
                glyph.XMin = context.GlyphTableReader.ReadInt16();
                glyph.YMin = context.GlyphTableReader.ReadInt16();
                glyph.XMax = context.GlyphTableReader.ReadInt16();
                glyph.YMax = context.GlyphTableReader.ReadInt16();
            }
            else
            {
                glyph.NumContours = numContours;
            }

            // For empty glyph nothing is written further.
            if (!glyph.IsEmpty)
            {
                // Spec says:
                // End point of first contour, number of points in each subsequent contour, cumulative sum should
                // be calculated to define total number of points.
                IntList contourEndPoints = new IntList(glyph.NumContours);
                int currentPointIndex = 0;
                for (int i = 0; i < glyph.NumContours; i++)
                {
                    currentPointIndex += UShort255.Read(context.GlyphTableReader);
                    contourEndPoints.Add(currentPointIndex);
                }
                int numPoints = currentPointIndex + 1;

                // Read points.
                byte[] flags = context.GlyphTableReader.ReadBytes(numPoints);
                glyph.Points = new SimpleGlyfPoint[numPoints];
                for (int i = 0; i < numPoints; i++)
                {
                    CtfGlyphPoint point = CtfGlyphPoint.Read(context.GlyphTableReader, flags[i]);
                    SimpleGlyfPoint ttPoint = new SimpleGlyfPoint(point.X, point.Y, point.IsOnCurve,
                                                            contourEndPoints.Contains(i));
                    glyph.Points[i] = ttPoint;
                }

                // Now we can calculate bounding box.
                if (!isBoundingBoxPresent)
                {
                    glyph.CalculateBoundingBox();
                }

                glyph.Instructions = DecodeInstructions(context);
            }

            return glyph;
        }

        /// <summary>
        /// Decodes TrueType instructions.
        /// </summary>
        internal static byte[] DecodeInstructions(CtfGlyphDecoderContext context)
        {
            int pushCount = UShort255.Read(context.GlyphTableReader);
            int codeSize = UShort255.Read(context.GlyphTableReader);

            using (MemoryStream stream = new MemoryStream())
            {
                BigEndianBinaryWriter writer = new BigEndianBinaryWriter(stream);

                short[] pushValues = DecodePushData(context.PushDataReader, pushCount);
                TTInstructionsUtil.WritePushData(writer, pushValues);

                byte[] code = context.InstructionsReader.ReadBytes(codeSize);

                writer.WriteBytes(code, 0, code.Length);

                return stream.ToArray();
            }
        }

        /// <summary>
        /// Decodes push data.
        /// </summary>
        /// <remarks>
        /// See http://www.w3.org/Submission/2008/SUBM-MTX-20080305/#PushData for more info.
        /// </remarks>
        internal static short[] DecodePushData(BigEndianBinaryReader reader, int pushCount)
        {
            short[] values = new short[pushCount];

            int i = 0;
            while (i < pushCount)
            {
                // kvk: Read255Short should not be used here because it can return
                // hop3Code or hop4Code in two situations:
                // 1. First byte value is a hop code.
                // 2. Value is encoded by 2 bytes and result equals a hop code.
                // In 2nd situation we should not treat the value as hop code.
                short code = reader.ReadByte();

                if (code == Hop3Code)
                {
                    short a = values[i - 2];
                    values[i++] = a;
                    values[i++] = Short255.Read(reader);
                    values[i++] = a;
                }
                else if (code == Hop4Code)
                {
                    short a = values[i - 2];
                    values[i++] = a;
                    values[i++] = Short255.Read(reader);
                    values[i++] = a;
                    values[i++] = Short255.Read(reader);
                    values[i++] = a;
                }
                else
                {
                    reader.BaseStream.Position--;
                    values[i++] = Short255.Read(reader);
                }
            }

            return values;
        }

        private const byte Hop3Code = 251;
        private const byte Hop4Code = 252;
    }
}
