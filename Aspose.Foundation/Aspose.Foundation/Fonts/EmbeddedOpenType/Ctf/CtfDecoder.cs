// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov

using System.IO;
using Aspose.Fonts.Sfnt;
using Aspose.Fonts.TrueType;
using Aspose.IO;

namespace Aspose.Fonts.EmbeddedOpenType.Ctf
{
    /// <summary>
    /// Allows to decode CTF files.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/Submission/2008/SUBM-MTX-20080305/#CTF for more info.
    /// </remarks>
    internal static class CtfDecoder
    {
        /// <summary>
        /// Decodes CTF data.
        /// </summary>
        /// <param name="fontTables">CTF font tables block.</param>
        /// <param name="glyphPushData">CTF glyph push data block.</param>
        /// <param name="glyphInstructions">CTF glyph instructions block.</param>
        /// <returns>Decoded OpenType font.</returns>
        internal static byte[] Decode(byte[] fontTables, byte[] glyphPushData, byte[] glyphInstructions)
        {
            using (MemoryStream fontTablesStream = new MemoryStream(fontTables))
            {
                SfntReader fontTablesReader = new SfntReader(fontTablesStream);
                fontTablesReader.ReadHeader();

                // These tables required to decode data.
                fontTablesReader.SeekToTable(OpenTypeTableTag.Head);
                FontHeader head = FontHeader.Read(fontTablesReader.BinaryReader);

                fontTablesReader.SeekToTable(OpenTypeTableTag.Maxp);
                MaximumProfile maxp = new MaximumProfile(fontTablesReader.BinaryReader);

                fontTablesReader.SeekToTable(OpenTypeTableTag.Hhea);
                HorizontalHeader hhea = new HorizontalHeader(fontTablesReader.BinaryReader);

                fontTablesReader.SeekToTable(OpenTypeTableTag.Hmtx);
                HorizontalMetrics hmtx = HorizontalMetrics.Read(fontTablesReader.BinaryReader, hhea.NumberOfHMetrics, maxp.NumGlyphs);

                SfntBuilder builder = new SfntBuilder(fontTablesReader.OffsetTable.Version);

                foreach (SfntTableRecordEntry tableRecord in fontTablesReader.TableRecordEntries.Values)
                {
                    switch (tableRecord.Tag)
                    {
                        case OpenTypeTableTag.Hdmx:
                            builder.AddTable(
                                tableRecord.Tag,
                                DecodeHdmxTable(
                                    fontTablesReader.ReadTable(tableRecord.Tag),
                                    maxp.NumGlyphs,
                                    head.UnitsPerEm,
                                    hmtx));
                            break;
                        case OpenTypeTableTag.Vdmx:
                            builder.AddTable(tableRecord.Tag, DecodeVdmxTable(fontTablesReader.ReadTable(tableRecord.Tag)));
                            break;
                        case OpenTypeTableTag.Cvt:
                            builder.AddTable(tableRecord.Tag, DecodeCvtTable(fontTablesReader.ReadTable(tableRecord.Tag)));
                            break;
                        case OpenTypeTableTag.Loca:
                            // loca table will be created during glyf table processing.
                            break;
                        case OpenTypeTableTag.Glyf:
                            CtfGlyphDecoder.DecodeGlyphData(
                                builder,
                                fontTablesReader.ReadTable(tableRecord.Tag),
                                glyphPushData,
                                glyphInstructions,
                                head);
                            break;
                        case OpenTypeTableTag.Head:
                            // head table could be updated and will be added to builder after decoding.
                            break;
                        default:
                            // All other table are not encoded.
                            builder.AddTable(tableRecord.Tag, fontTablesReader.ReadTable(tableRecord.Tag));
                            break;
                    }
                }

                builder.AddTable(OpenTypeTableTag.Head, head.ToByteArray());

                return builder.WriteFileToByteArray();
            }
        }

        /// <summary>
        /// Decodes cvt table.
        /// </summary>
        /// <remarks>
        /// See http://www.w3.org/Submission/2008/SUBM-MTX-20080305/#CVTTable for more info.
        /// </remarks>
        internal static byte[] DecodeCvtTable(byte[] tableData)
        {
            using (MemoryStream inStream = new MemoryStream(tableData))
            {
                BigEndianBinaryReader reader = new BigEndianBinaryReader(inStream);
                ushort numEntries = reader.ReadUInt16();

                using (MemoryStream outStream = new MemoryStream(numEntries * 2))
                {
                    BigEndianBinaryWriter writer = new BigEndianBinaryWriter(outStream);

                    short previousValue = 0;
                    for (int i = 0; i < numEntries; i++)
                    {
                        int newVlaue;
                        short byte1 = reader.ReadByte();
                        if (byte1 < CtfCoderUtil.CvtLowestCode)
                        {
                            newVlaue = previousValue + byte1;
                        }
                        else if (byte1 >= CtfCoderUtil.CvtPositive1 && byte1 <= CtfCoderUtil.CvtPositive8)
                        {
                            short byte2 = reader.ReadByte();
                            int difference = (CtfCoderUtil.CvtLowestCode * (byte1 - CtfCoderUtil.CvtPositive1 + 1) + byte2);
                            newVlaue = previousValue + difference;
                        }
                        else if (byte1 >= CtfCoderUtil.CvtNegative0 && byte1 <= CtfCoderUtil.CvtNegative8)
                        {
                            short byte2 = reader.ReadByte();
                            int difference = (-(CtfCoderUtil.CvtLowestCode * (byte1 - CtfCoderUtil.CvtNegative0) + byte2));
                            newVlaue = previousValue + difference;
                        }
                        else
                        {
                            newVlaue = previousValue + reader.ReadInt16();
                        }

                        if (newVlaue < short.MinValue)
                            newVlaue += 1 << 16;
                        else if (newVlaue > short.MaxValue)
                            newVlaue -= 1 << 16;

                        writer.WriteInt16(newVlaue);
                        previousValue = (short)newVlaue;
                    }

                    return outStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Decodes hdmx table.
        /// </summary>
        /// <remarks>
        /// See http://www.w3.org/Submission/2008/SUBM-MTX-20080305/#hdmxTable for more info.
        /// </remarks>
        private static byte[] DecodeHdmxTable(
            byte[] tableData,
            ushort numGlyphs,
            ushort unitsPerEm,
            HorizontalMetrics hmtx)
        {
            using (MemoryStream inStream = new MemoryStream(tableData))
            {
                BigEndianBinaryReader reader = new BigEndianBinaryReader(inStream);

                ushort version = reader.ReadUInt16();
                if (version == 0xFFFF)
                {
                    // Table is not encoded.
                    return CtfCoderUtil.CloneWithVersion(tableData, (ushort)(0xFFFF - version));
                }

                HdmxTable hdmx = new HdmxTable();
                hdmx.Version = version;
                hdmx.NumGlyphs = numGlyphs;
                ushort numRecords = reader.ReadUInt16();
                hdmx.DeviceRecordSize = reader.ReadInt32();

                hdmx.DeviceRecords = new HdmxDeviceRecord[numRecords];
                for (int i = 0; i < numRecords; i++)
                {
                    CtfHdmxDeviceRecord ctfRecord = CtfHdmxDeviceRecord.Read(reader);
                    HdmxDeviceRecord record = new HdmxDeviceRecord();
                    record.MaxWidth = ctfRecord.MaxWidth;
                    record.PixelSize = ctfRecord.PixelSize;
                    record.Widths = new byte[numGlyphs];
                    hdmx.DeviceRecords[i] = record;
                }

                MagnitudeReader magnitudeReader = new MagnitudeReader(inStream);
                for (int i = 0; i < numRecords; i++)
                {
                    for (int glyphId = 0; glyphId < numGlyphs; glyphId++)
                    {
                        // Each width is encoded by its prediction error.
                        // Prediction is calculated by following formula for correct rounding.
                        int prediction = CtfCoderUtil.CalculatePrediction(hdmx.DeviceRecords[i], hmtx.GetHMetric(glyphId), unitsPerEm);
                        short predictionError = magnitudeReader.ReadValue();
                        hdmx.DeviceRecords[i].Widths[glyphId] = (byte)(prediction + predictionError);
                    }
                }

                // Ignore original record size and use optimal value.
                hdmx.SetOptimalRecordSize();

                return hdmx.ToByteArray();
            }
        }

        /// <summary>
        /// Decodes VDMX table.
        /// </summary>
        /// <remarks>
        /// See http://www.w3.org/Submission/2008/SUBM-MTX-20080305/#VDMXTable for more info.
        /// </remarks>
        private static byte[] DecodeVdmxTable(byte[] tableData)
        {
            using (MemoryStream inStream = new MemoryStream(tableData))
            {
                BigEndianBinaryReader reader = new BigEndianBinaryReader(inStream);

                using (MemoryStream outStream = new MemoryStream())
                {
                    ushort version = reader.ReadUInt16();
                    if (version == 0xFFFF || version == (0xFFFF - 0x0001))
                    {
                        // Table is not encoded.
                        return CtfCoderUtil.CloneWithVersion(tableData, (ushort)(0xFFFF - version));
                    }

                    BigEndianBinaryWriter writer = new BigEndianBinaryWriter(outStream);

                    ushort numRecs = reader.ReadUInt16();
                    ushort numRatios = reader.ReadUInt16();

                    writer.WriteUInt16(version);
                    writer.WriteUInt16(numRecs);
                    writer.WriteUInt16(numRatios);

                    // Ratios and offsets are not encoded.
                    writer.WriteBytes(reader.ReadBytes(numRatios*6));

                    for (int i = 0; i < numRecs; i++)
                    {
                        ushort predictedPpem = 8;
                        ushort groupNumRecs = reader.ReadUInt16();
                        short yMaxMult = reader.ReadInt16();
                        short yMinMult = reader.ReadInt16();

                        VdmxGroup group = new VdmxGroup();
                        group.VTables = new VdmxVTable[groupNumRecs];

                        MagnitudeReader magnitudeReader = new MagnitudeReader(inStream);
                        for (int j = 0; j < groupNumRecs; j++)
                        {
                            group.VTables[j] = new VdmxVTable();

                            short ppemError = magnitudeReader.ReadValue();
                            short yMaxError = magnitudeReader.ReadValue();
                            short yMinError = magnitudeReader.ReadValue();

                            group.VTables[j].YPelHeight = (ushort)(predictedPpem + ppemError);
                            int predictedYMax = (group.VTables[j].YPelHeight*yMaxMult + 1024)/2048;
                            int predictedYMin = -(group.VTables[j].YPelHeight*yMinMult + 1024)/2048;
                            group.VTables[j].YMax = (short)(predictedYMax + yMaxError);
                            group.VTables[j].YMin = (short)(predictedYMin + yMinError);

                            predictedPpem++;
                        }

                        group.Write(writer);
                    }

                    return outStream.ToArray();
                }
            }
        }
    }
}
