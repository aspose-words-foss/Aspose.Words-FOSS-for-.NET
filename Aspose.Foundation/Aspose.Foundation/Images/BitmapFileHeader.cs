// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/01/2006 by Roman Korchagin

using System.IO;

namespace Aspose.Images
{
    /// <summary>
    /// BITMAPFILEHEADER contains information about the type, size, and layout of a file that contains a DIB. 
    /// 
    /// A BITMAPINFO immediately follows the BITMAPFILEHEADER structure in the DIB file.
    /// </summary>
    public class BitmapFileHeader
    {
        internal BitmapFileHeader()
        {
        }
        
        internal void Read(BinaryReader reader)
        {
            FileType = reader.ReadUInt16();
            Size = reader.ReadUInt32();
            Reserved1 = reader.ReadUInt16();
            Reserved2 = reader.ReadUInt16();
            OffBits = reader.ReadUInt32();
        }

        internal void Write(BinaryWriter writer)
        {
            writer.Write((short)FileType);
            writer.Write((int)Size);
            writer.Write((short)Reserved1);
            writer.Write((short)Reserved2);
            writer.Write((int)OffBits);
        }

        /// <summary>
        /// Specifies the file type, must be BM. 
        /// </summary>
        internal ushort FileType = 0x4d42;    //BM

        /// <summary>
        /// Specifies the size, in bytes, of the bitmap file. 
        /// </summary>
        internal uint Size = 0;

        /// <summary>
        /// Reserved; must be zero. 
        /// </summary>
        internal ushort Reserved1 = 0;

        /// <summary>
        /// Reserved; must be zero. 
        /// </summary>
        internal ushort Reserved2 = 0;

        /// <summary>
        /// Specifies the offset, in bytes, from the beginning of the BITMAPFILEHEADER structure to the bitmap bits.
        /// </summary>
        internal uint OffBits = 0;

        /// <summary>
        /// Size of the BITMAPFILEHEADER structure.
        /// </summary>
        public const int StructureSize = 14;
    }
}
