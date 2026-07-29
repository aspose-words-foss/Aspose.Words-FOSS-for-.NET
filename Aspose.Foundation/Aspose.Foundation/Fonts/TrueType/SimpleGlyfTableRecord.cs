// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov

using System;
using Aspose.Collections;
using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents simple record in glyf table in OpenType file.
    /// </summary>
    internal class SimpleGlyfTableRecord : GlyfTableRecord
    {
        public static int ReadPointsCount(BigEndianBinaryReader reader, short numContours)
        {
            IntList contourEndPoints = ReadContourEndPoints(reader, numContours);
            return contourEndPoints[contourEndPoints.Count - 1] + 1;
        }

        /// <summary>
        /// Reads simple record from binary reader.
        /// </summary>
        public static SimpleGlyfTableRecord ReadSimpleRecord(BigEndianBinaryReader reader)
        {
            SimpleGlyfTableRecord record = new SimpleGlyfTableRecord();
            record.ReadHeader(reader);

            if (record.IsEmpty)
                return record;

            IntList contourEndPoints = ReadContourEndPoints(reader, record.NumContours);

            // Points count equals last contour end point + 1.
            int pointsCount = 1 + contourEndPoints[record.NumContours - 1];

            int instructionsLength = reader.ReadUInt16();
            record.Instructions = reader.ReadBytes(instructionsLength);

            byte[] flags = new byte[pointsCount];
            int flagIndex = 0;
            while (flagIndex < pointsCount)
            {
                byte flag = reader.ReadByte();
                flags[flagIndex++] = flag;

                if (BitUtil.IsSetByte(flag, FlagRepeat))
                {
                    // If FlagRepeat is set, the next byte specifies the number of additional
                    // times this set of flags is to be repeated.
                    int count = reader.ReadByte();
                    for (int i = 0; i < count; i++)
                    {
                        flags[flagIndex++] = flag;
                    }
                }
            }

            short[] xCoordinates = new short[pointsCount];
            for (int i = 0; i < pointsCount; i++)
            {
                xCoordinates[i] =
                    ReadXyValue(
                        reader,
                        BitUtil.IsSetByte(flags[i], FlagShortX),
                        BitUtil.IsSetByte(flags[i], FlagPositiveShortOrSameX));
            }

            short[] yCoordinates = new short[pointsCount];
            for (int i = 0; i < pointsCount; i++)
            {
                yCoordinates[i] =
                   ReadXyValue(
                       reader,
                       BitUtil.IsSetByte(flags[i], FlagShortY),
                       BitUtil.IsSetByte(flags[i], FlagPositiveShortOrSameY));
            }

            // Build points array.
            record.Points = new SimpleGlyfPoint[pointsCount];
            for (int i = 0; i < pointsCount; i++)
            {
                SimpleGlyfPoint point = new SimpleGlyfPoint(
                    xCoordinates[i],
                    yCoordinates[i],
                    BitUtil.IsSetByte(flags[i], FlagOnCurve),
                    contourEndPoints.Contains(i),
                    BitUtil.IsSetByte(flags[i], FlagOverlapSimple));
                record.Points[i] = point;
            }

            return record;
        }

        private static IntList ReadContourEndPoints(BigEndianBinaryReader reader, int numContours)
        {
            IntList result = new IntList(numContours);
            for (int i = 0; i < numContours; i++)
                result.Add(reader.ReadUInt16());

            result.Sort();
            return result;
        }

        /// <summary>
        /// Read x or y value from binary reader according to flags logic.
        /// </summary>
        private static short ReadXyValue(BigEndianBinaryReader reader, bool isShort, bool isPositiveOrSame)
        {
            // isShort means value is written as 8-bit unsigned, 16-bit signed otherwise.
            // isPositiveOrSame for short values means actual value is positive, negative otherwise.
            // isPositiveOrSame for long values means that this value is 0.
            if (isShort)
                return (short)(reader.ReadByte() * (isPositiveOrSame ? 1 : -1));

            return isPositiveOrSame ? (short)0 : reader.ReadInt16();
        }

        /// <summary>
        /// Calculate bounding box from points coordinates.
        /// </summary>
        public void CalculateBoundingBox()
        {
            if (Points.Length == 0)
                return;

            short curX = Points[0].DX;
            short curY = Points[0].DY;
            XMin = curX;
            XMax = curX;
            YMin = curY;
            YMax = curY;

            for (int i = 1; i < Points.Length; i++)
            {
                curX += Points[i].DX;
                curY += Points[i].DY;

                XMin = Math.Min(XMin, curX);
                XMax = Math.Max(XMax, curX);
                YMin = Math.Min(YMin, curY);
                YMax = Math.Max(YMax, curY);
            }
        }

        /// <summary>
        /// Writes simple record to binary writer.
        /// </summary>
        public override void Write(BigEndianBinaryWriter writer)
        {
            // Empty glyphs could be written as 0 bytes. So do it always.
            if (IsEmpty)
                return;

            WriteNumContoursAndBoundingBox(writer);
            WriteContourEndPoints(writer);
            WriteInstructions(writer);
            WritePoints(writer);
        }

        private void WriteContourEndPoints(BigEndianBinaryWriter writer)
        {
            for (int i = 0; i < Points.Length; i++)
            {
                if (Points[i].IsEndOfContour)
                    writer.WriteUInt16(i);
            }
        }

        private void WriteInstructions(BigEndianBinaryWriter writer)
        {
            writer.WriteUInt16(Instructions.Length);
            writer.WriteBytes(Instructions, 0, Instructions.Length);
        }

        private void WritePoints(BigEndianBinaryWriter writer)
        {
            byte[] flags = BuildFlags();
            WriteFlags(writer, flags);
            WriteXCoordinates(writer, flags);
            WriteYCoordinates(writer, flags);
        }

        private byte[] BuildFlags()
        {
            byte[] flags = new byte[Points.Length];
            for (int i = 0; i < Points.Length; i++)
            {
                SimpleGlyfPoint point = Points[i];
                byte flag = 0;
                if (point.IsOnCurve)
                    flag += FlagOnCurve;
                if (point.IsOverlap)
                    flag += FlagOverlapSimple;
                flag += SelectCoordinateFlag(point.DX, FlagShortX, FlagPositiveShortOrSameX);
                flag += SelectCoordinateFlag(point.DY, FlagShortY, FlagPositiveShortOrSameY);
                flags[i] = flag;
            }
            return flags;
        }

        private static byte SelectCoordinateFlag(short coord, byte shortFlag, byte sameOrPositiveFlag)
        {
            byte result = 0;
            if (coord == 0)
            {
                result += sameOrPositiveFlag;
            }
            else if (Math.Abs(coord) <= byte.MaxValue)
            {
                result += shortFlag;
                if(coord >= 0)
                    result += sameOrPositiveFlag;
            }

            return result;
        }

        private static void WriteFlags(BigEndianBinaryWriter writer, byte[] flags)
        {
            int i = 0;
            while (i < flags.Length)
            {
                byte flag = flags[i++];

                byte repeatCount = 0;
                while (i + repeatCount < flags.Length && flags[i + repeatCount] == flag && repeatCount != byte.MaxValue)
                    repeatCount++;

                if (repeatCount > 1)
                {
                    writer.WriteByte((byte)(flag + FlagRepeat));
                    writer.WriteByte(repeatCount);
                    i += repeatCount;
                }
                else
                {
                    writer.WriteByte(flag);
                }
            }
        }

        private void WriteXCoordinates(BigEndianBinaryWriter writer, byte[] flags)
        {
            for (int i = 0; i < Points.Length; i++)
            {
                byte flag = flags[i];
                WriteCoordinate(
                    writer,
                    Points[i].DX,
                    BitUtil.IsSetByte(flag, FlagShortX),
                    BitUtil.IsSetByte(flag, FlagPositiveShortOrSameX));
            }
        }

        private void WriteYCoordinates(BigEndianBinaryWriter writer, byte[] flags)
        {
            for (int i = 0; i < Points.Length; i++)
            {
                byte flag = flags[i];
                WriteCoordinate(
                    writer,
                    Points[i].DY,
                    BitUtil.IsSetByte(flag, FlagShortY),
                    BitUtil.IsSetByte(flag, FlagPositiveShortOrSameY));
            }
        }

        private static void WriteCoordinate(
            BigEndianBinaryWriter writer,
            short value,
            bool isShort,
            bool isPositiveOrSame)
        {
            if (isShort)
            {
                writer.WriteByte((byte)Math.Abs(value));
            }
            else if (!isPositiveOrSame)
            {
                writer.WriteInt16(value);
            }
        }

        /// <summary>
        /// Array of glyph points.
        /// </summary>
        public SimpleGlyfPoint[] Points { get; set; }

        /// <summary>
        /// TrueType instructions.
        /// </summary>
        public byte[] Instructions { get; set; }

        private const byte FlagOnCurve = 0x01;
        private const byte FlagShortX = 0x02;
        private const byte FlagShortY = 0x04;
        private const byte FlagRepeat = 0x08;
        private const byte FlagPositiveShortOrSameX = 0x10;
        private const byte FlagPositiveShortOrSameY = 0x20;
        private const byte FlagOverlapSimple = 0x40;
    }
}
