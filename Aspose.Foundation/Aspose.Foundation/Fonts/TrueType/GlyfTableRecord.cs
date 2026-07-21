// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Konstantin Kornilov

using Aspose.IO;
using Aspose.JavaAttributes;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents single record in glyf table in OpenType file.
    /// </summary>
    internal abstract class GlyfTableRecord
    {
        /// <summary>
        /// Writes record to binary writer.
        /// </summary>
        [JavaThrows(true)]
        public abstract void Write(BigEndianBinaryWriter writer);

        /// <summary>
        /// Writes number of contours and bounding box;
        /// </summary>
        public void WriteNumContoursAndBoundingBox(BigEndianBinaryWriter writer)
        {
            writer.WriteInt16(NumContours);
            writer.WriteInt16(XMin);
            writer.WriteInt16(YMin);
            writer.WriteInt16(XMax);
            writer.WriteInt16(YMax);
        }

        /// <summary>
        /// Read record from binary reader.
        /// </summary>
        public static GlyfTableRecord Read(BigEndianBinaryReader reader)
        {
            short numContours = reader.ReadInt16();
            reader.BaseStream.Position -= 2;

            if(numContours < 0)
                return CompositeGlyfTableRecord.ReadCompositeRecord(reader);

            return SimpleGlyfTableRecord.ReadSimpleRecord(reader);
        }

        protected void ReadHeader(BigEndianBinaryReader reader)
        {
            NumContours = reader.ReadInt16();
            XMin = reader.ReadInt16();
            YMin = reader.ReadInt16();
            XMax = reader.ReadInt16();
            YMax = reader.ReadInt16();
        }

        public void InitPhantomPoints(short lsb, short advance)
        {
            PhantomPoint1X = XMin - lsb;
            PhantomPoint2X = PhantomPoint1X + advance;
        }

        /// <summary>
        /// True when glyph is empty.
        /// </summary>
        public bool IsEmpty
        {
            get { return NumContours == 0; }
        }

        public bool IsComposite
        {
            get { return NumContours < 0; }
        }

        public short NumContours { get; set; }
        public short XMin { get; set; }
        public short XMax { get; set; }
        public short YMin { get; set; }
        public short YMax { get; set; }
        public int PhantomPoint1X { get; set; }
        public int PhantomPoint2X { get; set; }
    }
}
