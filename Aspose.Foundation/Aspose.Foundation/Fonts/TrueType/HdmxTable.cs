// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov

using System;
using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents 'hdmx' table of OpenType font.
    /// </summary>
    internal class HdmxTable : TTTable
    {
        /// <summary>
        /// Read hdmx table from binary reader.
        /// </summary>
        internal static HdmxTable Read(BigEndianBinaryReader reader, int numGlyphs)
        {
            HdmxTable hdmx = new HdmxTable();

            hdmx.NumGlyphs = numGlyphs;
            hdmx.Version = reader.ReadUInt16();
            int numRecords = reader.ReadInt16();
            hdmx.DeviceRecordSize = reader.ReadInt32();

            if (numRecords < 0 || hdmx.DeviceRecordSize < HdmxDeviceRecord.GetActualRecordSize(numGlyphs))
                throw new InvalidOperationException("Unsupported hdmx table format.");

            hdmx.DeviceRecords = new HdmxDeviceRecord[numRecords];
            for (int i = 0; i < numRecords; i++)
                hdmx.DeviceRecords[i] = HdmxDeviceRecord.Read(reader, numGlyphs, hdmx.DeviceRecordSize);

            return hdmx;
        }

        /// <summary>
        /// Writes table to binary writer.
        /// </summary>
        internal override void Write(BigEndianBinaryWriter writer)
        {
            WriteHeader(writer);

            for (int i = 0; i < DeviceRecords.Length; i++)
                DeviceRecords[i].Write(writer, DeviceRecordSize);
        }

        /// <summary>
        /// Writes table header to binary writer.
        /// </summary>
        internal void WriteHeader(BigEndianBinaryWriter writer)
        {
            writer.WriteUInt16(Version);
            writer.WriteInt16(DeviceRecords.Length);
            writer.WriteInt32(DeviceRecordSize);
        }

        /// <summary>
        /// Sets <see cref="DeviceRecordSize"/> to optimal value.
        /// </summary>
        internal void SetOptimalRecordSize()
        {
            // Spec says that records should be long aligned so increase actualRecordSize to multiply 4.
            DeviceRecordSize = MathUtil.RoundUp(HdmxDeviceRecord.GetActualRecordSize(NumGlyphs), 4);
        }

        internal int NumGlyphs;
        internal ushort Version;
        internal int DeviceRecordSize;
        internal HdmxDeviceRecord[] DeviceRecords;
    }
}
