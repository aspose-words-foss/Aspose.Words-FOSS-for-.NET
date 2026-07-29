// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2016 by Ilya Navrotskiy

using System;
using System.IO;
using Aspose.Fonts.Sfnt;
using Aspose.Fonts.TrueType;
using Aspose.IO;

namespace Aspose.Fonts.EmbeddedOpenType.Ctf
{
    /// <summary>
    /// Allows to encode to CTF format.
    /// </summary>
    /// <remarks>
    /// See http://www.w3.org/Submission/2008/SUBM-MTX-20080305/#CTF for more info.
    /// </remarks>
    internal static class CtfEncoder
    {
        /// <summary>
        /// Encodes font data to CTF format.
        /// </summary>
        internal static CtfEncodedData Encode(byte[] fontTables)
        {
            using (MemoryStream fontTablesStream = new MemoryStream(fontTables))
            {
                SfntReader fontTablesReader = new SfntReader(fontTablesStream);
                if (!fontTablesReader.TryReadHeader())
                    return null;

                // These tables are required to encode data.
                fontTablesReader.SeekToTable(OpenTypeTableTag.Head);
                FontHeader head = FontHeader.Read(fontTablesReader.BinaryReader);

                fontTablesReader.SeekToTable(OpenTypeTableTag.Maxp);
                MaximumProfile maxp = new MaximumProfile(fontTablesReader.BinaryReader);

                fontTablesReader.SeekToTable(OpenTypeTableTag.Hhea);
                HorizontalHeader hhea = new HorizontalHeader(fontTablesReader.BinaryReader);

                fontTablesReader.SeekToTable(OpenTypeTableTag.Hmtx);
                HorizontalMetrics hmtx = HorizontalMetrics.Read(fontTablesReader.BinaryReader, hhea.NumberOfHMetrics, maxp.NumGlyphs);

                SfntBuilder builder = new SfntBuilder(fontTablesReader.OffsetTable.Version);

                CtfEncodedData ctfEncodedData = new CtfEncodedData();
                foreach (SfntTableRecordEntry tableRecord in fontTablesReader.TableRecordEntries.Values)
                {
                    string tag = tableRecord.Tag;
                    switch (tag)
                    {
                        case OpenTypeTableTag.Hdmx:
                            {
                                byte[] hdmx = fontTablesReader.ReadTable(tag);
                                byte[] hdmxEncoded = EncodeHdmxTable(hdmx, maxp.NumGlyphs, head.UnitsPerEm, hmtx);
                                builder.AddTable(tag, hdmxEncoded);
                                break;
                            }
                        case OpenTypeTableTag.Vdmx:
                            builder.AddTable(tag, EncodeVdmxTable(fontTablesReader.ReadTable(tag)));
                            break;
                        case OpenTypeTableTag.Cvt:
                            builder.AddTable(tag, EncodeCvtTable(fontTablesReader.ReadTable(tag)));
                            break;
                        case OpenTypeTableTag.Loca:
                            // 'loca' table will be constructed from the 'glyf' table in decoder,
                            // but it must be written with zero length.
                            builder.AddTable(tag, new byte[]{});
                            break;
                        case OpenTypeTableTag.Glyf:
                            {
                                ctfEncodedData = CtfGlyphEncoder.EncodeGlyphData(fontTablesReader, head.IsLocaShort);
                                builder.AddTable(tag, ctfEncodedData.FontTable);
                                break;
                            }
                        default:
                            // All other tables should be added not encoded.
                            builder.AddTable(tag, fontTablesReader.ReadTable(tag));
                            break;
                    }
                }

                ctfEncodedData.FontTable = builder.WriteFileToByteArray();
                return ctfEncodedData;
            }
        }

      /// <summary>
      /// Encodes cvt table.
      /// </summary>
      /// <remarks>
      /// See http://www.w3.org/Submission/2008/SUBM-MTX-20080305/#CVTTable for more info.
      /// </remarks>
      internal static byte[] EncodeCvtTable(byte[] tableData)
      {
          using (MemoryStream inStream = new MemoryStream(tableData))
          {
              BigEndianBinaryReader reader = new BigEndianBinaryReader(inStream);

              using (MemoryStream outStream = new MemoryStream())
              {
                  BigEndianBinaryWriter writer = new BigEndianBinaryWriter(outStream);

                  int cvtLength = tableData.Length / 2;
                  writer.WriteInt16(cvtLength);

                  int previousValue = 0;
                  for (int i = 0; i < cvtLength; i++)
                  {
                      int value = reader.ReadInt16();
                      int difference = value - previousValue;
                      previousValue = value;

                      bool isNegative = (difference < 0);
                      int positiveDifference = (isNegative) ? 0x00 - difference : difference;

                      int index = positiveDifference / CtfCoderUtil.CvtLowestCode;

                      if ((index <= 8) && (difference != -32768))
                      {
                          if (isNegative)
                          {
                              int code = CtfCoderUtil.CvtNegative0 + index;
                              writer.WriteByte((byte)code);
                              positiveDifference = (positiveDifference - index * CtfCoderUtil.CvtLowestCode);
                              writer.WriteByte((byte)positiveDifference);
                          }
                          else
                          {
                              if (index > 0)
                              {
                                  int code = CtfCoderUtil.CvtPositive1 + index - 1;
                                  writer.WriteByte((byte)code);
                                  positiveDifference = positiveDifference - index * CtfCoderUtil.CvtLowestCode;
                              }

                              writer.WriteByte((byte)positiveDifference);
                          }
                      }
                      else
                      {
                          writer.WriteByte((byte)CtfCoderUtil.CvtLowestCode);
                          writer.WriteInt16(difference);
                      }
                  }

                  return outStream.ToArray();
              }
          }
      }

        /// <summary>
        /// Encodes hdmx table.
        /// </summary>
        /// <remarks>
        /// See http://www.w3.org/Submission/2008/SUBM-MTX-20080305/#hdmxTable for more info.
        /// </remarks>
        internal static byte[] EncodeHdmxTable(
            byte[] tableData,
            ushort numGlyphs,
            ushort unitsPerEm,
            HorizontalMetrics hmtx)
        {
            using (MemoryStream inStream = new MemoryStream(tableData))
            {
                BigEndianBinaryReader reader = new BigEndianBinaryReader(inStream);

                HdmxTable hdmx = HdmxTable.Read(reader, numGlyphs);

                using (MemoryStream outStream = new MemoryStream())
                {
                    BigEndianBinaryWriter writer = new BigEndianBinaryWriter(outStream);

                    hdmx.WriteHeader(writer);

                    for (int i = 0; i < hdmx.DeviceRecords.Length; i++)
                        hdmx.DeviceRecords[i].Write(writer);

                    MagnitudeWriter magnitudeWriter = new MagnitudeWriter(outStream);
                    int bitsOffset = 0;
                    for (int i = 0; i < hdmx.DeviceRecords.Length; i++)
                    {
                        HdmxDeviceRecord record = hdmx.DeviceRecords[i]; // extract stuff below in the loop's body into EncodeRecord method to simplify
                        for (int glyphId = 0; glyphId < record.Widths.Length; glyphId++)
                        {
                            int width = record.Widths[glyphId];

                            int prediction = CtfCoderUtil.CalculatePrediction(hdmx.DeviceRecords[i], hmtx.GetHMetric(glyphId), unitsPerEm);
                            int predictionError = width - prediction;

                            // CtfHeader size = 8 and each width is byte size.
                            int newLength = 8 + (bitsOffset + Math.Abs(predictionError) + 2 + 7) / 8;

                            if (newLength > tableData.Length)
                            {
                                // Encoded table size is greater than original table size.
                                // Set version to 0xFFFF-real version and just copy data.
                                return CtfCoderUtil.CloneWithVersion(tableData, (ushort)(0xFFFF - hdmx.Version));
                            }

                            bitsOffset += magnitudeWriter.WriteValue((short) predictionError);
                        }
                    }
                    magnitudeWriter.Flush();
                    return outStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Encodes VDMX table.
        /// </summary>
        /// <remarks>
        /// See http://www.w3.org/Submission/2008/SUBM-MTX-20080305/#VDMXTable for more info.
        /// </remarks>
        internal static byte[] EncodeVdmxTable(byte[] tableData)
        {
            using (MemoryStream inStream = new MemoryStream(tableData))
            {
                BigEndianBinaryReader reader = new BigEndianBinaryReader(inStream);

                using (MemoryStream outStream = new MemoryStream())
                {
                    BigEndianBinaryWriter writer = new BigEndianBinaryWriter(outStream);

                    ushort version = reader.ReadUInt16();
                    ushort numRecs = reader.ReadUInt16();
                    ushort numRatios = reader.ReadUInt16();

                    writer.WriteUInt16(version);
                    writer.WriteUInt16(numRecs);
                    writer.WriteUInt16(numRatios);

                    // Ratios (4 bytes) and offsets (2 bytes) are not encoded.
                    writer.WriteBytes(reader.ReadBytes(4 * numRatios));
                    ushort[] offsets = new ushort[numRatios];
                    for (int i = 0; i < numRatios; i++)
                    {
                        offsets[i] = reader.ReadUInt16();
                        writer.WriteUInt16(offsets[i]);
                    }

                    VdmxGroup[] vdmxGroups = new VdmxGroup[numRatios];
                    for (int i = 0; i < numRatios; i++)
                    {
                        reader.BaseStream.Position = offsets[i];
                        vdmxGroups[i] = VdmxGroup.Read(reader);
                    }

                    for (int i = 0; i < numRatios; i++)
                    {
                        WriteVdmxGroup(writer, vdmxGroups[i]);

                        if (outStream.Position > tableData.Length)
                        {
                            // Encoded table size is greater than original table size.
                            // Set version to 0xFFFF-real version and just copy data.
                            return CtfCoderUtil.CloneWithVersion(tableData, (ushort)(0xFFFF - version));
                        }
                    }

                    return outStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Writes VDMX group to binary writer.
        /// </summary>
        private static void WriteVdmxGroup(BigEndianBinaryWriter writer, VdmxGroup vdmxGroup)
        {
            writer.WriteUInt16(vdmxGroup.VTables.Length);
            writer.WriteInt16(vdmxGroup.YMaxMultiplier);
            writer.WriteInt16(vdmxGroup.YMinMultiplier);

            ushort predictedYPelHeight = 8;
            MemoryStream outStream = (MemoryStream)writer.BaseStream;
            MagnitudeWriter magnitudeWriter = new MagnitudeWriter(outStream);
            foreach (VdmxVTable vTable in vdmxGroup.VTables)
            {
                WriteVdmxVTable(magnitudeWriter, vTable, vdmxGroup, predictedYPelHeight);
                predictedYPelHeight++;
            }
            magnitudeWriter.Flush();
        }

        /// <summary>
        /// Writes VDMX VTable to the binary writer.
        /// </summary>
        private static void WriteVdmxVTable(MagnitudeWriter magnitudeWriter, VdmxVTable vTable, VdmxGroup vdmxGroup, int predictedYPelHeight)
        {
            int predictedYMax = (vTable.YPelHeight * vdmxGroup.YMaxMultiplier + 1024) / 2048;
            int predictedYMin = -(vTable.YPelHeight * vdmxGroup.YMinMultiplier + 1024) / 2048;

            int yPelHeightError = vTable.YPelHeight - predictedYPelHeight;
            int yMaxError = vTable.YMax - predictedYMax;
            int yMinError = vTable.YMin - predictedYMin;

            magnitudeWriter.WriteValue((short)yPelHeightError);
            magnitudeWriter.WriteValue((short)yMaxError);
            magnitudeWriter.WriteValue((short)yMinError);
        }
    }
}
