// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/12/2013 by Konstantin Kornilov

using System.IO;
using Aspose.JavaAttributes;

namespace Aspose.Images
{
    /// <summary>
    /// Represents DIB header (bitmap information header).
    /// See http://en.wikipedia.org/wiki/BMP_file_format.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    internal abstract class DibHeader
    {
        public static BitmapInfoHeader ReadAsBitmapInfoHeader(BinaryReader reader)
        {
            long headerStartPosition = reader.BaseStream.Position;
            int size = reader.ReadInt32();

            // Read all header types into BITMAPINFOHEADER for now.
            BitmapInfoHeader result = new BitmapInfoHeader();
            switch (size)
            {
                case Os21XBitmapHeaderSize:
                    // http://www.fastgraph.com/help/bmp_os2_header_format.html
                    result.Width = reader.ReadUInt16();
                    // NOTE: If a bitmask is present, this value includes the height of the mask (so often header.height = entry.height * 2)
                    result.Height = reader.ReadUInt16();
                    result.Planes = reader.ReadUInt16();
                    result.BitCount = reader.ReadUInt16();
                    break;
                case Os22XBitmapHeader16Size:
                case Os22XBitmapHeaderSize:
                case BitmapInfoHeaderSize:
                case BitmapV2InfoHeaderSize:
                case BitmapV3InfoHeaderSize:
                case BitmapV4HeaderSize:
                case BitmapV5HeaderSize:
                default:
                    // All these headers have the same first 40 bytes.
                    // Also this is the default case for all other values to read silently. Not very safe, but okay.
                    // Read only fields from BITMAPINFOHEADER.
                    result.Width = reader.ReadInt32();
                    // NOTE: If a bitmask is present, this value includes the height of the mask (so often header.height = entry.height * 2)
                    result.Height = reader.ReadInt32();
                    result.Planes = reader.ReadUInt16();
                    result.BitCount = reader.ReadUInt16();
                    if (size != Os22XBitmapHeader16Size)
                    {
                        result.Compression = (BitmapCompression) reader.ReadUInt32();
                        result.SizeImage = reader.ReadUInt32();
                        result.XPelsPerMeter = reader.ReadInt32();
                        result.YPelsPerMeter = reader.ReadInt32();
                        result.ClrUsed = reader.ReadUInt32();
                        result.ClrImportant = reader.ReadUInt32();
                    }
                    break;
            }

            reader.BaseStream.Position = headerStartPosition + size;

            return result;
        }

        [JavaThrows(true)]
        public abstract void Write(BinaryWriter writer);

        public abstract int Size { get; }
        public abstract int ColorTableSize { get; }

        public const int Os21XBitmapHeaderSize = 12;
        public const int Os22XBitmapHeader16Size = 16;
        public const int Os22XBitmapHeaderSize = 64;
        public const int BitmapInfoHeaderSize = 40;
        public const int BitmapV2InfoHeaderSize = 52;
        public const int BitmapV3InfoHeaderSize = 56;
        public const int BitmapV4HeaderSize = 108;
        public const int BitmapV5HeaderSize = 124;
    }
}
