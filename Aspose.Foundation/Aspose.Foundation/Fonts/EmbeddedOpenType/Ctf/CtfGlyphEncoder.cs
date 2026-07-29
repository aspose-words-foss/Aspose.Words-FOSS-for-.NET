// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2016 by Ilya Navrotskiy

using System.IO;
using Aspose.Collections;
using Aspose.Fonts.Sfnt;
using Aspose.Fonts.TrueType;
using Aspose.IO;

namespace Aspose.Fonts.EmbeddedOpenType.Ctf
{
    /// <summary>
    /// Allows to encode glyph table to CTF.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/Submission/2008/SUBM-MTX-20080305/#glyfTable for more info.
    /// </remarks>
    internal static class CtfGlyphEncoder
    {
        /// <summary>
        /// Encodes glyph table data and returns it in <see cref="CtfEncodedData"/>.
        /// </summary>
        internal static CtfEncodedData EncodeGlyphData(SfntReader fontReader, bool isLocaShort)
        {
            fontReader.SeekToTable(OpenTypeTableTag.Loca);
            TTLocaTable loca = TTLocaTable.Read(
                fontReader.BinaryReader,
                fontReader.TableRecordEntries[OpenTypeTableTag.Loca].Length,
                isLocaShort);

            CtfEncodedData encodedData = new CtfEncodedData();
            using (MemoryStream
                encodedGlyphTableStream = new MemoryStream(),
                encodedInstructionsStream = new MemoryStream(),
                encodedPushDataStream = new MemoryStream())
            {
                CtfGlyphEncoderContext context = new CtfGlyphEncoderContext(
                    encodedGlyphTableStream,
                    encodedPushDataStream,
                    encodedInstructionsStream);

                fontReader.SeekToTable(OpenTypeTableTag.Glyf);
                int glyphTableStartPosition = (int)fontReader.BaseStream.Position;
                BigEndianBinaryReader reader = fontReader.BinaryReader;
                IntList glyphLocations = loca.GlyphLocations;
                for (int i = 0; i < glyphLocations.Count - 1; i++)
                {
                    // Write zero for duplicated glyphs.
                    if (((i + 1) < glyphLocations.Count) && (glyphLocations[i] == glyphLocations[i + 1]))
                    {
                        context.GlyphTableWriter.WriteInt16(0x0);
                        continue;
                    }

                    reader.BaseStream.Position = glyphTableStartPosition + loca.GlyphLocations[i];
                    GlyfTableRecord glyph = GlyfTableRecord.Read(reader);
                    EncodeGlyph(glyph, context);
                }

                encodedData.FontTable = encodedGlyphTableStream.ToArray();
                encodedData.PushData = encodedPushDataStream.ToArray();
                encodedData.Instructions = encodedInstructionsStream.ToArray();
                return encodedData;
            }
        }

        /// <summary>
        /// Encodes glyph to CTF format.
        /// </summary>
        internal static void EncodeGlyph(GlyfTableRecord glyph, CtfGlyphEncoderContext context)
        {
            if (glyph.NumContours >= 0)
                EncodeSimpleGlyph((SimpleGlyfTableRecord)glyph, context);
            else
                EncodeCompositeGlyph((CompositeGlyfTableRecord)glyph, context);
        }

        /// <summary>
        /// Encodes composite glyph to CTF format.
        /// </summary>
        internal static void EncodeCompositeGlyph(CompositeGlyfTableRecord glyph, CtfGlyphEncoderContext context)
        {
            glyph.WriteNumContoursAndBoundingBox(context.GlyphTableWriter);
            foreach (ComponentGlyphRecord component in glyph.Components)
                component.Write(context.GlyphTableWriter);

            if (glyph.WeHaveInstructions)
                EncodeInstructions(glyph.Instructions, context);
        }

        /// <summary>
        /// Encodes simple glyph to CTF format.
        /// </summary>
        internal static void EncodeSimpleGlyph(SimpleGlyfTableRecord glyph, CtfGlyphEncoderContext context)
        {
            // IN. By the MTX spec 5.7.1. Bounding Box,
            // the bounding box information can be computed from the data point coordinates and,
            // therefore, bounding box information may be omitted in glyph record.
            // At the same time it is said that in some event it is necessary to store the bounding box.
            // But I cannot see such events, so for a moment bounding box is not writing and therefore, it
            // will be always reconstructed in decoder.
            context.GlyphTableWriter.WriteInt16(glyph.NumContours);

            // If glyph is empty, then it is nothing to write else.
            if (glyph.IsEmpty)
                return;

            // Write number of points in each contour.
            // First value is offset of second end point.
            int numberOfContourPoints = -1;
            foreach (SimpleGlyfPoint point in glyph.Points)
            {
                numberOfContourPoints++;
                if (point.IsEndOfContour)
                {
                    UShort255.Write((ushort)numberOfContourPoints, context.GlyphTableWriter);
                    numberOfContourPoints = 0;
                }
            }
            // Write triple encoding flags for each point.
            byte[] flags = new byte[glyph.Points.Length];
            for (int i = 0; i < flags.Length; i++)
            {
                SimpleGlyfPoint point = glyph.Points[i];
                flags[i] = TripletEncoding.GetTripletEncodingIndex(point.DX, point.DY);
                if (!point.IsOnCurve)
                    flags[i] |= 0x80;

                context.GlyphTableWriter.WriteByte(flags[i]);
            }

            // Write coordinates for each point.
            BitBinaryWriter bitWriter = new BitBinaryWriter(context.GlyphTableWriter.BaseStream, true);
            for (int i = 0; i < glyph.Points.Length; i++)
                TripletEncoding.WriteTriplet(bitWriter, flags[i], glyph.Points[i]);
            bitWriter.Flush();

            // Write glyph instructions.
            EncodeInstructions(glyph.Instructions, context);
        }

        /// <summary>
        /// Encodes TrueType instructions to CTF format and writes it to binary writer.
        /// </summary>
        internal static void EncodeInstructions(byte[] instructions, CtfGlyphEncoderContext context)
        {
            using (MemoryStream instructionsStream = new MemoryStream(instructions))
            {
                BigEndianBinaryReader instructionsReader = new BigEndianBinaryReader(instructionsStream);
                IntList pushData = TTInstructionsUtil.ReadPushData(instructionsReader);

                // Write push data length.
                UShort255.Write((ushort)pushData.Count, context.GlyphTableWriter);
                // Write code length.
                ushort codeSize = (ushort)(instructionsStream.Length - instructionsStream.Position);
                UShort255.Write(codeSize, context.GlyphTableWriter);

                EncodePushData(pushData, context.PushDataWriter);

                byte[] code = instructionsReader.ReadBytes(codeSize);
                context.InstructionsWriter.WriteBytes(code, 0, code.Length);
            }
        }

        /// <summary>
        /// Encodes push data to CTF format and writes it to binary writer.
        /// </summary>
        /// <remarks>
        /// See http://www.w3.org/Submission/2008/SUBM-MTX-20080305/#PushData for more info.
        /// </remarks>
        internal static void EncodePushData(IntList pushData, BigEndianBinaryWriter writer)
        {
            int i = 0;
            while (i < pushData.Count)
            {
                int a = pushData[i];
                // Check for Hop3 sequence.
                // If the previous time we already wrote down a hop-code with the same value 'a',
                // then we have to lookup back to check it.
                // In this case we should add just Hop3 code and 'X2' value.
                if ((((i - 2) >= 0) && ((i + 2) < pushData.Count)) && ((pushData[i - 2] == a) && (pushData[i + 2] == a)))
                {
                    // Check for Hop4 sequence.
                    byte hopCode = (((i + 4) < pushData.Count) && (pushData[i + 4] == a)) ? Hop4Code : Hop3Code;

                    // Write values for Hop3 code.
                    writer.WriteByte(hopCode);
                    Short255.Write((short)pushData[i + 1], writer);
                    i += 3;

                    if (hopCode == Hop4Code)
                    {
                        // Write values for Hop4 code.
                        Short255.Write((short)pushData[i], writer);
                        i += 2;
                    }
                }// Check for Hop3 sequence again, but now we check forward.
                else if (((i + 4) < pushData.Count) && ((pushData[i + 2] == a) && (pushData[i + 4] == a)))
                {
                    // Sequence for Hop3Code found.
                    Short255.Write((short)a, writer);
                    Short255.Write((short)pushData[i + 1], writer);

                    // Check for Hop4 sequence.
                    byte hopCode = (((i + 6) < pushData.Count) && (pushData[i + 6] == a)) ? Hop4Code : Hop3Code;

                    // Write rest values for Hop3 code.
                    writer.WriteByte(hopCode);
                    Short255.Write((short)pushData[i + 3], writer);
                    i += 5;

                    if (hopCode == Hop4Code)
                    {
                        // Write values for Hop4 code.
                        Short255.Write((short)pushData[i], writer);
                        i += 2;
                    }
                }
                else
                {
                    Short255.Write((short)pushData[i++], writer);
                }
            }
        }

        private const byte Hop3Code = 251;
        private const byte Hop4Code = 252;
    }
}
