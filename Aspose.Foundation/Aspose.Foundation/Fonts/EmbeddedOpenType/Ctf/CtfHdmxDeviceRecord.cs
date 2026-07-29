// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov

using Aspose.IO;

namespace Aspose.Fonts.EmbeddedOpenType.Ctf
{
    /// <summary>
    /// Represents Device Record in hdmx table in CTF file.
    /// </summary>
    internal class CtfHdmxDeviceRecord
    {
        /// <summary>
        /// Reads record from binary reader.
        /// </summary>
        public static CtfHdmxDeviceRecord Read(BigEndianBinaryReader reader)
        {
            CtfHdmxDeviceRecord record = new CtfHdmxDeviceRecord();
            record.PixelSize = reader.ReadByte();
            record.MaxWidth = reader.ReadByte();
            return record;
        }

        /// <summary>
        /// Writes record to binary reader.
        /// </summary>
        public void Write(BigEndianBinaryWriter writer)
        {
            writer.WriteByte(PixelSize);
            writer.WriteByte(MaxWidth);
        }

        public byte PixelSize;

        public byte MaxWidth;
    }
}
