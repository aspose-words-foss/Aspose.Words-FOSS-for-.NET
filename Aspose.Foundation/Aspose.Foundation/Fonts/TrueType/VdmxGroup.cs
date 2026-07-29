// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov

using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents VDMX Group in VDMX table in OpenType file.
    /// </summary>
    internal class VdmxGroup
    {
        /// <summary>
        /// Writes VDMX Group to binary writer.
        /// </summary>
        public void Write(BigEndianBinaryWriter writer)
        {
            writer.WriteUInt16(VTables.Length);
            writer.WriteByte((byte)VTables[0].YPelHeight);
            writer.WriteByte((byte)VTables[VTables.Length - 1].YPelHeight);
            foreach (VdmxVTable vdmxVTable in VTables)
            {
                vdmxVTable.Write(writer);
            }
        }

        /// <summary>
        /// Reads VDMX Group from binary reader.
        /// </summary>
        public static VdmxGroup Read(BigEndianBinaryReader reader)
        {
            VdmxGroup vdmxGroup = new VdmxGroup();
            ushort vTablesCount = reader.ReadUInt16();
            vdmxGroup.VTables = new VdmxVTable[vTablesCount];

            // These values will be recalculated according to the specification below.
            vdmxGroup.mYMaxMultiplier = reader.ReadByte();
            vdmxGroup.mYMinMultiplier = reader.ReadByte();
            int yMaxRatio = 0;
            int yMinRatio = 0;
            for (int i = 0; i < vTablesCount; i++)
            {
                vdmxGroup.VTables[i] = VdmxVTable.Read(reader);
                yMaxRatio += ((vdmxGroup.VTables[i].YMax << 16) + vdmxGroup.VTables[i].YPelHeight / 2) / vdmxGroup.VTables[i].YPelHeight;
                yMinRatio += ((vdmxGroup.VTables[i].YMin << 16) - vdmxGroup.VTables[i].YPelHeight / 2) / vdmxGroup.VTables[i].YPelHeight;
            }

            yMaxRatio = (yMaxRatio + vTablesCount / 2) / vTablesCount;
            yMinRatio = (yMinRatio - vTablesCount / 2) / vTablesCount;

            // Convert to 21.11 format.
            vdmxGroup.mYMaxMultiplier = (yMaxRatio + 16) >> 5;
            vdmxGroup.mYMinMultiplier = (-yMinRatio + 16) >> 5;
            
            return vdmxGroup;
        }
        
        /// <summary>
        /// Used to predict actual yMax values.
        /// </summary>
        internal int YMaxMultiplier
        {
            get { return mYMaxMultiplier; }
        }

        /// <summary>
        /// used to predict actual yMin values.
        /// </summary>
        internal int YMinMultiplier
        {
            get { return mYMinMultiplier; }
        }

        public VdmxVTable[] VTables;
        private int mYMaxMultiplier;
        private int mYMinMultiplier;
    }
}
