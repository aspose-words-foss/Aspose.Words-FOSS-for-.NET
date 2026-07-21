// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov

using System;
using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents Device Record in hdmx table in OpenType font.
    /// </summary>
    internal class HdmxDeviceRecord
    {
        /// <summary>
        /// Reads record from binary reader.
        /// </summary>
        internal static HdmxDeviceRecord Read(BigEndianBinaryReader reader, int numGlyphs, int deviceRecordSize)
        {
            if(deviceRecordSize < GetActualRecordSize(numGlyphs))
                throw new InvalidOperationException("Wrong hdmx device record size.");

            HdmxDeviceRecord record = new HdmxDeviceRecord();

            record.PixelSize = reader.ReadByte();
            record.MaxWidth = reader.ReadByte();
            record.Widths = reader.ReadBytes(numGlyphs);

            reader.BaseStream.Position += deviceRecordSize - GetActualRecordSize(numGlyphs);

            return record;
        }

        /// <summary>
        /// Writes record with its widths to writer.
        /// </summary>
        internal void Write(BigEndianBinaryWriter writer, int deviceRecordSize)
        {
            if(deviceRecordSize < GetActualRecordSize(Widths.Length))
                throw new InvalidOperationException("Wrong hdmx device record size.");

            Write(writer);
            writer.WriteBytes(Widths);

            // Pad record to deviceRecordSize.
            writer.BaseStream.Position += (deviceRecordSize - GetActualRecordSize());
        }

        /// <summary>
        /// Writes 'pixelSize' and 'maxWidth' of this record to writer.
        /// </summary>
        internal void Write(BigEndianBinaryWriter writer)
        {
            writer.WriteByte(PixelSize);
            writer.WriteByte(MaxWidth);
        }

        /// <summary>
        /// Returns actual size of device record data.
        /// </summary>
        internal static int GetActualRecordSize(int numGlyphs)
        {
            return numGlyphs + 2;
        }

        /// <summary>
        /// Returns actual size of device record data.
        /// </summary>
        private int GetActualRecordSize()
        {
            return GetActualRecordSize(Widths.Length);
        }

        internal byte PixelSize;
        internal byte MaxWidth;
        internal byte[] Widths;
    }
}
