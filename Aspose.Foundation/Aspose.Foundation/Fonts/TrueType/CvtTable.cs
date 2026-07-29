// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/04/2025 by Konstantin Kornilov

using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    public class CvtTable : TTTable
    {
        public static CvtTable Read(BigEndianBinaryReader reader, long tableLength)
        {
            CvtTable table = new CvtTable();
            table.Values = new short[(int)tableLength / 2];
            for (int i = 0; i <  table.Values.Length; i++)
                table.Values[i] = reader.ReadInt16();
            return table;
        }

        internal override void Write(BigEndianBinaryWriter writer)
        {
            for (int i = 0; i < Values.Length; i++)
                writer.WriteInt16(Values[i]);
        }

        public short[] Values { get; private set; }
    }
}
