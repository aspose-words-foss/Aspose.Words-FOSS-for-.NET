// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov

using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents vTable in VDMX table in OpenType file.
    /// </summary>
    internal class VdmxVTable
    {
        /// <summary>
        /// Writes vTable to binary writer.
        /// </summary>
        public void Write(BigEndianBinaryWriter writer)
        {
            writer.WriteUInt16(YPelHeight);
            writer.WriteInt16(YMax);
            writer.WriteInt16(YMin);
        }

        /// <summary>
        /// Reads vTable from binary reader.
        /// </summary>
        public static VdmxVTable Read(BigEndianBinaryReader reader)
        {
            VdmxVTable vdmxVTable = new VdmxVTable();
            vdmxVTable.YPelHeight = reader.ReadUInt16();
            vdmxVTable.YMax = reader.ReadInt16();
            vdmxVTable.YMin = reader.ReadInt16();

            return vdmxVTable;
        }
        
        public ushort YPelHeight;
        public short YMax;
        public short YMin;
    }
}
