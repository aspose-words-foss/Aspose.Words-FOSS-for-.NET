// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/11/2005 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Xml;
using Aspose.Common;
using Aspose.Crypto;
using Aspose.Drawing;
using Aspose.Images.Jpeg;
using Aspose.Images.Pal;
using Aspose.IO;
using Aspose.JavaAttributes;
using Aspose.Xml;

namespace Aspose.Images
{
    /// <summary>
    /// Utility functions to deal with images.
    /// </summary>
    public static class ImageUtil
    {
        public static FileFormat GetImageType(byte[] data)
        {
            if ((data == null) || (data.Length == 0))
                return FileFormat.Unknown;

            using (Stream stream = new MemoryStream(data))
            {
                return GetImageType(CompressedData.GetStream(stream));
            }
        }

        /// <summary>
        /// Gets the image type without invoking GDI+.
        /// Retains the stream position.
        /// </summary>
        public static FileFormat GetImageType(Stream stream)
        {
            if ((stream == null) || (stream.Length == 0))
                return FileFormat.Unknown;

            long savePos = stream.Position;
            try
            {
                bool check = IsStandardMetafile(stream);
                stream.Position = savePos;
                if (check)
                    return FileFormat.Wmf;

                check = IsPlaceableMetafile(stream);
                stream.Position = savePos;
                if (check)
                    return FileFormat.Wmf;

                check = IsEmf(stream);
                stream.Position = savePos;
                if (check)
                    return FileFormat.Emf;

                check = IsJpeg(stream);
                stream.Position = savePos;
                if (check)
                    return FileFormat.Jpeg;

                check = IsPng(stream);
                stream.Position = savePos;
                if (check)
                    return FileFormat.Png;

                check = IsBmp(stream);
                stream.Position = savePos;
                if (check)
                    return FileFormat.Bmp;

                check = IsPict(stream);
                stream.Position = savePos;
                if (check)
                    return FileFormat.Pict;

                // alexnosk: PICT can be represented also as a PICT file. check for it too.
                check = IsPictFile(stream);
                stream.Position = savePos;
                if (check)
                    return FileFormat.Pict;

                check = IsGif(stream);
                stream.Position = savePos;
                if (check)
                    return FileFormat.Gif;

                check = IsTiff(stream);
                stream.Position = savePos;
                if (check)
                    return FileFormat.Tiff;

                check = IsPdf(stream);
                stream.Position = savePos;
                if (check)
                    return FileFormat.Pdf;

                check = IsSvg(stream);
                stream.Position = savePos;
                if (check)
                    return FileFormat.Svg;

                check = IsMov(stream);
                stream.Position = savePos;
                if (check)
                    return FileFormat.Mov;

                check = IsIco(stream);
                stream.Position = savePos;
                if (check)
                    return FileFormat.Ico;

                check = IsWebP(stream);
                stream.Position = savePos;
                if (check)
                    return FileFormat.WebP;
            }
            catch
            {
                // WORDSNET-4996 Just ignore all unexpected troubles recognizing the image type.
            }

            stream.Position = savePos;
            return FileFormat.Unknown;
        }

        /// <summary>
        /// Gets the image size and resolution of given image file without invoking GDI+.
        /// </summary>
        public static ImageSizeCore GetImageSize(string imageFile)
        {
            using (FileStream imageStream = new FileStream(imageFile, FileMode.Open))
                return GetImageSize(imageStream);
        }

        /// <summary>
        /// Gets the image size and resolution without invoking GDI+.
        /// </summary>
        public static ImageSizeCore GetImageSize(Stream imageStream)
        {
            return GetImageSize(imageStream, GetImageType(imageStream));
        }

        /// <summary>
        /// Gets the image size and resolution without invoking GDI+.
        /// </summary>
        public static ImageSizeCore GetImageSize(byte[] imageBytes)
        {
            return GetImageSize(imageBytes, GetImageType(imageBytes));
        }

        /// <summary>
        /// Gets the image size and resolution without invoking GDI+.
        /// </summary>
        public static ImageSizeCore GetImageSize(byte[] imageBytes, FileFormat imageType)
        {
            return GetImageSize(new MemoryStream(imageBytes), imageType);
        }

        /// <summary>
        /// Gets the image size and resolution without invoking GDI+.
        /// Retains the stream position.
        /// </summary>
        [JavaConvertCheckedExceptions]
        public static ImageSizeCore GetImageSize(Stream imageStream, FileFormat imageType)
        {
            long savePos = imageStream.Position;
            ImageSizeCore imageSize;

            switch (imageType)
            {
                case FileFormat.Wmf:
                case FileFormat.Emf:
                    imageSize = GetMetafileSize(imageStream);
                    break;
                case FileFormat.Pict:
                    imageSize = GetPictSize(imageStream);
                    break;
                case FileFormat.Bmp:
                    imageSize = GetBmpSize(imageStream);
                    break;
                case FileFormat.Png:
                    imageSize = GetPngSize(imageStream);
                    break;
                case FileFormat.Jpeg:
                    imageSize = GetJpegSize(imageStream);
                    break;
                case FileFormat.Gif:
                    imageSize = GetGifSize(imageStream);
                    break;
                case FileFormat.Tiff: // AN 20/05/2009
                    imageSize = GetTiffSize(imageStream);
                    break;
                case FileFormat.Ico:
                    imageSize = GetIcoSize(imageStream, 0);
                    break;
                case FileFormat.WebP:
                    imageSize = GetWebPSize(imageStream);
                    break;
                default:
                    // RK Return at least some usable value for unknown formats.
                    imageSize = ImageSizeCore.CreateWithResolution(100, 100, ImageConstants.StandardResolution);
                    break;
            }

            imageStream.Position = savePos;
            return imageSize;
        }

        #region Is<ImageType>

        /// <summary>
        /// Documentation for the format is in Aspose.Words\Doc.
        /// </summary>
        public static bool IsTiff(byte[] data)
        {
            return TiffDataReader.IsTiff(data);
        }

        /// <summary>
        /// Documentation for the format is in Aspose.Words\Doc.
        /// </summary>
        public static bool IsTiff(Stream stream)
        {
            return TiffDataReader.IsTiff(stream);
        }

        public static bool IsJpeg(byte[] data)
        {
            Debug.Assert(data != null);

            using (MemoryStream stream = new MemoryStream(data))
                return IsJpeg(stream);
        }

        /// <summary>
        /// Documentation for the format is in Aspose.Words\Doc.
        /// Will detect Jfif and Exif files as jpeg.
        /// </summary>
        public static bool IsJpeg(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            try
            {
                uint soi = reader.ReadUInt16();
                if (soi != 0xd8ff)
                    return false;

                // For Jfif further bytes will be:
                // 0xe0ff
                // 0x1000
                // 0x4649464a

                // For Exif further bytes will be
                // 0xe1ff
                // 0xef21
                // 0x66697845
            }
            catch (EndOfStreamException)
            {
                // The input stream is too short.
                return false;
            }
            return true;
        }

        public static bool IsPng(byte[] data)
        {
            Debug.Assert(data != null);

            using (MemoryStream stream = new MemoryStream(data))
                return IsPng(stream);
        }

        /// <summary>
        /// http://www.libpng.org/pub/png/spec/iso/index-object.html
        /// </summary>
        public static bool IsPng(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            try
            {
                uint b1 = reader.ReadUInt32();
                if (b1 != 0x474e5089)
                    return false;

                uint b2 = reader.ReadUInt32();
                if (b2 != 0x0a1a0a0d)
                    return false;
            }
            catch (EndOfStreamException)
            {
                // The input stream is too short.
                return false;
            }
            return true;
        }

        public static bool IsBmp(byte[] data)
        {
            Debug.Assert(data != null);

            using (MemoryStream stream = new MemoryStream(data))
                return IsBmp(stream);
        }

        /// <summary>
        /// Documentation for the format is in Aspose.Words\Doc.
        /// Also here is a good description of OS/2 BMP headers:
        ///   http://www.fileformat.info/format/os2bmp/egff.htm
        /// </summary>
        /// <dev>
        /// Here are several supported headers. (Are they going to invent any more?)
        /// BITMAPINFOHEADER - 0x28
        /// BITMAPV4HEADER - 0x6C
        /// BITMAPV5HEADER - 0x7C
        /// OS/2 1.x BMP header - 0x0C
        /// OS/2 2.x BMP header - 0xF0 (? - saw in our docs)
        /// </dev>
        public static bool IsBmp(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            try
            {
                // "BM" as the first two bytes.
                // We don't recognize other signatures as well as Windows 1.x device dependent bitmaps (DDB).
                uint sig = reader.ReadUInt16();
                if (sig != 0x4d42)
                    return false;

                // Full file size if non-zero. This can be zero in files, means not specified.
                uint fileSize = reader.ReadUInt32();

                // Reserved and should be zero in Windows. In OS/2 there are two hot-spot coordinates.
                // Let's don't check this otherwise we would have to guess what format version it is.
                reader.ReadUInt32();

                // I saw a file with image failing this condition. WORDSNET-4899.
                // Do we need a more precise test? First guess is no, since we hadn't trouble before WORDSNET-4899.
                uint dataOffset = reader.ReadUInt32();
                if ((fileSize != 0) && (dataOffset > fileSize))
                    return false;

                // At this point we have read first 14 bytes which are the BITMAPFILEHEADER.
                // Bitmap info header size.
                // The very minimum it can be is 12 bytes (0x0C) for OS/2 v1.x.
                // If it is greater, then next minimum is 16 (0x10) since the last mandatory field is BPP.
                // Before reading any further field this length should be checked to properly handle OS/2 BMPs.
                const uint bmihOS21xSize = 0x0C;
                const uint bmihMinOtherThenOS21xSize = 0x10;

                uint bmihSize = reader.ReadUInt32();
                bool isOS21x = (bmihSize == bmihOS21xSize);

                if (!isOS21x && (bmihSize < bmihMinOtherThenOS21xSize))
                    return false;

                // Width and height.
                // In OS/2 1.x they are unsigned 16 bits long each. In OS/2 2.x - unsigned 32 bits.
                // In Windows always signed 32 bits.
                if (isOS21x)
                {
                    reader.ReadUInt32();
                }
                else
                {
                    reader.ReadInt32();
                    reader.ReadInt32();
                }

                // Planes. Seems to be always 1 in any BMP dialect.
                ushort planes = reader.ReadUInt16();
                if (planes != 1)
                    return false;

                // Bits per pixel value is restricted to this subset.
                ushort bitsPerPixel = reader.ReadUInt16();
                List<ushort> allowedbitsPerPixel = new List<ushort>(new ushort[] { 1, 4, 8, 16, 24, 32 });
                if (allowedbitsPerPixel.Contains(bitsPerPixel))
                    return true;

                // If you suggest reading and checking more be aware of possible header size variations.
            }
            catch (EndOfStreamException)
            {
                // The input stream is too short.
                return false;
            }
            return false;
        }

        public static bool IsGif(byte[] data)
        {
            Debug.Assert(data != null);

            using (MemoryStream stream = new MemoryStream(data))
                return IsGif(stream);
        }

        /// <summary>
        /// Documentation for the format is in Aspose.Words\Doc.
        /// </summary>
        public static bool IsGif(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            if (!StreamUtil.HasEnoughBytesToRead(reader, 4 /* Int32 */))
                return false;

            int sig = reader.ReadInt32() & 0x00FFFFFF;
            return (sig == 0x00464947); // 'GIF'
        }

        public static bool IsPict(byte[] data)
        {
            Debug.Assert(data != null);

            using (MemoryStream stream = new MemoryStream(data))
                return IsPict(stream);
        }

        /// <summary>
        /// If PICT is read from file, it contains 512 bytes header (if it is read from document it does not contain it).
        /// Method checks if data contains PICT image read from file.
        /// </summary>
        public static bool IsPictFile(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
                return IsPictFile(stream);
        }

        /// <summary>
        /// If PICT is read from file, it contains 512 bytes header (if it is read from document it does not contain it).
        /// Method checks if data contains PICT image read from file.
        /// </summary>
        public static bool IsPictFile(Stream data)
        {
            // If data contains less than 512 bytes + 12 bytes header it cannot be pict.
            if (data.Length <= (ImageConstants.PictHeaderLength + 12))
                return false;

            data.Position = ImageConstants.PictHeaderLength;
            return IsPict(data);
        }

        /// <summary>
        /// http://www.fileformat.info/format/macpict/
        /// </summary>
        public static bool IsPict(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            try
            {
                // This is 16 byte size of the image. It is lame.
                // We've seen files longer than 64k. This value is simply a truncation of 32 bit.
                reader.ReadUInt16();  // SHORT

                // Top, left, bottom, right (big-endian).
                // Documentation says left, top, right, bottom, but stupid Mac stores y first, then x. Perverts.
                reader.ReadUInt16();  // SHORT
                reader.ReadUInt16();  // SHORT
                reader.ReadUInt16();  // SHORT
                reader.ReadUInt16();  // SHORT

                int versionOperator = BitUtil.SwapUInt16(reader.ReadUInt16());
                // This is PICT V1.0
                switch (versionOperator)
                {
                    case 0x0111:
                    {
                        // WORDSJAVA-1142 Missing ODT chart while saving to Png.
                        // It's not enough to check only version operator and version number for PICT V1.0, because it can
                        // return true for PNG format.
                        // So check picFrame (PICT v1.0) additionally.

                        // Skip picSize records
                        reader.ReadUInt16(); // WORD     Picture size in bytes
                        reader.ReadUInt16(); // WORD     Image top
                        reader.ReadUInt16(); // WORD     Image left
                        reader.ReadUInt16(); // WORD     Image bottom
                        reader.ReadUInt16(); // WORD     Image right
                                             // Check picFrame
                        return (reader.ReadByte() == 0x11 && reader.ReadByte() == 0x01);
                    }
                    case 0x0011:
                    {
                        int versionNumber = BitUtil.SwapUInt16(reader.ReadUInt16());
                        if (versionNumber == 0x02ff)
                            return true;
                        break;
                    }
                    default:
                        break;
                }
            }
            catch (EndOfStreamException)
            {
                // The input stream is too short.
                return false;
            }
            return false;
        }

        /// <summary>
        /// Returns true if this image is a Dib file.
        /// </summary>
        public static bool IsDib(byte[] data)
        {
            Debug.Assert(data != null);

            using (MemoryStream stream = new MemoryStream(data))
                return IsDib(stream);
        }

        /// <summary>
        /// Returns true if this image is a Dib file.
        /// </summary>
        public static bool IsDib(Stream imageStream)
        {
            long pos = imageStream.Position;

            imageStream.Position = 0;
            if (imageStream.Length < 2/*sizeof(ushort)*/)
            {
                // The input buffer is too short.
                return false;
            }

            // Fairly primitive attempt to detect a DIB header.
            byte[] data = new byte[2];
            StreamUtil.Read(imageStream, data, 0, 2);

            imageStream.Position = pos;

            uint size = BitConverter.ToUInt16(data, 0);
            return size == 40;
        }

        /// <summary>
        /// Returns true if this image is an ICO file.
        /// </summary>
        public static bool IsIco(byte[] data)
        {
            Debug.Assert(data != null);

            using (MemoryStream stream = new MemoryStream(data))
                return IsIco(stream);
        }

        /// <summary>
        /// Returns true if this image is an ICO file.
        /// See https://en.wikipedia.org/wiki/ICO_(file_format)
        /// </summary>
        public static bool IsIco(Stream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            try
            {
                int reserved = reader.ReadUInt16();
                if (reserved != 0)
                    return false;
                int type = reader.ReadUInt16();
                if (type != 1)
                    return false;
                int nrEntries = reader.ReadUInt16();
                if (nrEntries < 1)
                    return false;

                for (int i = 0; i < nrEntries; ++i)
                {
                    reader.ReadByte(); // width
                    reader.ReadByte(); // height
                    reader.ReadByte(); // number of color palettes
                    int reservedByte = reader.ReadByte(); // reserved byte
                    if (reservedByte != 0)
                        return false;
                    int nrColorPanes = reader.ReadUInt16(); // number of color planes
                    if (nrColorPanes != 0 && nrColorPanes != 1)
                        return false;
                    reader.ReadUInt16(); // bits per pixel
                    reader.ReadUInt32(); // size in bytes
                    reader.ReadUInt32(); // offset
                }
            }
            catch (EndOfStreamException)
            {
                // The input stream is too short.
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns true if this image is a non-placeable WML.
        ///
        /// See X:\Aspose\Aspose.Words\Doc\Specs\Wmf\wmf.htm
        /// </summary>
        public static bool IsStandardMetafile(byte[] imageBytes)
        {
            using (MemoryStream imageStream = new MemoryStream(imageBytes))
                return IsStandardMetafile(imageStream);
        }

        public static bool IsStandardMetafile(Stream imageStream)
        {
            Debug.Assert(imageStream != null);

            // WORDSNET-7011 Resilience against too short image bytes.
            const int metafileDetectionChunkSize = 18;
            if (imageStream.Length < metafileDetectionChunkSize)
                return false;

            BinaryReader reader = new BinaryReader(imageStream);

            // Type of metafile (0=memory, 1=disk)
            int type = reader.ReadInt16();
            if ((type != 0) && (type != 1))
                return false;

            // Size of header in WORDS (always 9)
            int headerSize = reader.ReadInt16();
            if (headerSize != 9)
                return false;

            // Version of Microsoft Windows used
            reader.ReadInt16();

            // Total size of the metafile in WORDs
            reader.ReadInt32();

            // Number of objects in the file
            reader.ReadInt16();

            // The size of largest record in WORDs
            reader.ReadInt32();

            // Not Used (always 0)
            int unused = reader.ReadInt16();
            if (unused != 0)
                return false;

            return true;
        }

        /// <summary>
        /// Returns true if this image is a placeable WMF.
        /// </summary>
        public static bool IsPlaceableMetafile(byte[] imageBytes)
        {
            Debug.Assert(imageBytes != null);

            using (MemoryStream imageStream = new MemoryStream(imageBytes))
                return IsPlaceableMetafile(imageStream);
        }

        public static bool IsPlaceableMetafile(Stream imageStream)
        {
            BinaryReader reader = new BinaryReader(imageStream);

            // Detection needs at least 6 bytes.
            if (!StreamUtil.HasEnoughBytesToRead(reader, 6))
                return false;

            // Magic number (always 9AC6CDD7h)
            uint key = reader.ReadUInt32();
            if (key != 0x9AC6CDD7)
                return false;

            // Metafile HANDLE number (always 0)
            int handle = reader.ReadInt16();
            if (handle != 0)
                return false;

            return true;
        }

        /// <summary>
        /// Returns true if the image is a WMF or EMF metafile.
        /// </summary>
        public static bool IsMetafile(FileFormat imageType)
        {
            return (imageType == FileFormat.Wmf) || (imageType == FileFormat.Emf);
        }

        public static bool IsEmf(Stream imageStream)
        {
            BinaryReader reader = new BinaryReader(imageStream);

            // Detection needs at least 44 bytes.
            if (!StreamUtil.HasEnoughBytesToRead(reader, 44))
                return false;

            // For the EMF header record this value is always 00000001h
            uint recordType = reader.ReadUInt32();
            if (recordType != 1)
                return false;

            // Signature ID (always 0x464D4520)
            reader.BaseStream.Position = 40;
            uint signature = reader.ReadUInt32();
            if (signature != 0x464D4520)
                return false;

            return true;
        }

        public static bool IsPdf(Stream stream)
        {
            return IsPdf(StreamUtil.ReadAndReturn(stream, 4));
        }

        /// <summary>
        /// Returns true if PDF.
        /// </summary>
        public static bool IsPdf(byte[] data)
        {
            Debug.Assert(data != null);

            // First four bytes should be: 0x25 0x50 0x44 0x46 in hex format, in ASCII it's %PDF
            if (data != null)
                return ((data[0] == '%') && (data[1] == 'P') && (data[2] == 'D') && (data[3] == 'F'));

            return false;
        }

        /// <summary>
        /// Returns true if the specified byte array contains an SVG image.
        /// </summary>
        [JavaThrows(false)]
        public static bool IsSvg(byte[] data)
        {
            if (!ArrayUtil.HasData(data))
                return false;

            // Check and convert Java Expected Exception
            try
            {
                using (MemoryStream dataStream = new MemoryStream(data))
                {
                    return IsSvg(dataStream);
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if the specified stream contains an SVG image.
        /// </summary>
        public static bool IsSvg(Stream stream)
        {
            // WORDSNET-21302 Fast check if the stream looks like an SVG XML. This reduces the number of exceptions we get
            // later when we try to parse the input as XML.
            if (!CanBeXml(stream))
                return false;

            // Check if the root node is SVG.
            try
            {
                AnyXmlReader xmlReader = new AnyXmlReader(stream);
                // WORDSNET-21024 We no longer check if the root node of the SVG image has an "xmlns" attribute with
                // the SVG namespace. Standalone SVG images without the namespace declaration are malformed but Aspose.Words
                // supports them.
                return xmlReader.LocalName == "svg";
            }
            catch (XmlException)
            {
                return false;
            }
        }

        public static bool IsMov(byte[] data)
        {
            using (MemoryStream dataStream = new MemoryStream(data))
                return IsMov(dataStream);
        }

        /// <summary>
        /// Returns true if QuickTime MOV file.
        /// </summary>
        public static bool IsMov(Stream stream)
        {
            const int headerSize = 16;

            // WORDSNET-13374.
            // Check if stream is long enough to have the MOV header.
            if (stream.Position + headerSize > stream.Length)
                return false;

            BinaryReader reader = new BinaryReader(stream);
            string t = Encoding.ASCII.GetString(reader.ReadBytes(headerSize), 4, 6);
            // Rewind stream position.
            stream.Position -= headerSize;

            return t == "ftypqt";
        }

        /// <summary>
        /// Returns true if WebP file.
        /// </summary>
        public static bool IsWebP(Stream stream)
        {
            const int headerSize = 4;

            if (stream.Position + headerSize > stream.Length)
                return false;

            BinaryReader reader = new BinaryReader(stream);
            string t = Encoding.ASCII.GetString(reader.ReadBytes(headerSize), 0, 4);
            // Rewind stream position.
            stream.Position -= headerSize;

            return t == "RIFF";
        }
        #endregion

        /// <summary>
        /// Reads a DIB-bitmap from a reader into a byte array and prepends it with a BMP header.
        /// </summary>
        public static byte[] PrependBmpHeader(BinaryReader reader, int dibLength)
        {
            long startPos = reader.BaseStream.Position;
            BitmapInfoHeader dibHeader = DibHeader.ReadAsBitmapInfoHeader(reader);
            byte[] colorTable = GetColorTable(reader, dibHeader, dibLength, startPos);
            int headerLength = (int)(reader.BaseStream.Position - startPos);
            return BuildBmp(dibHeader, colorTable, reader, dibLength - headerLength);
        }

        private static byte[] GetColorTable(BinaryReader reader, BitmapInfoHeader dibHeader, int dibLength, long startPos)
        {
            byte[] colorTable = new byte[0];

            // Read Color Table only if there are some bytes will be remained for an pixels array.
            if (reader.BaseStream.Position - startPos + dibHeader.ColorTableSize < dibLength)
                colorTable = reader.ReadBytes(dibHeader.ColorTableSize);

            return colorTable;
        }

        /// <summary>
        /// Gets palette size.
        /// </summary>
        internal static int GetPaletteSize(BitmapCompression compression, int bitCount, int clrUsed)
        {
            const int colorEntrySize = 4;

            // See http://msdn.microsoft.com/en-us/library/cc250406%28v=prot.10%29 for more info.

            // If compression is BI_BITFIELDS then color table contains three DWORD color masks.
            // BI_BITFIELDS could be used for 32 and 16 bit colors.
            if (((bitCount == 32) || (bitCount == 16)) && (compression == BitmapCompression.BitFields))
                return 12;

            // If BI_BITFIELDS compression is not used then color table is not used for 32, 24 and 16 bit colors.
            if (bitCount > 8)
                return 0;

            // For 1, 2, 4 and 8 bit colors ClrUsed can be explicit number of colors or the full table is present.
            int numColors = (clrUsed != 0) ? clrUsed : (1 << bitCount);

            return numColors * colorEntrySize;
        }

        /// <summary>
        /// Reads a DIB-bitmap from a reader into a byte array and prepends it with a BMP header.
        ///
        /// WORDSJAVA-1333 Inline image not displayed.
        /// For some reason image height, specified in ico header is different from image height, specified in dib header.
        /// It seems that ico header contains a correct size 16x16, rather than 16x32 as specified in dib header.
        /// So, for dib rasters extracted from ico files, we should specify image size explicitly.
        /// </summary>
        public static byte[] PrependBmpHeader(BinaryReader reader, int dibLength, int width, int height)
        {
            long startPos = reader.BaseStream.Position;
            BitmapInfoHeader dibHeader = DibHeader.ReadAsBitmapInfoHeader(reader);
            dibHeader.Width = width;
            dibHeader.Height = height;
            byte[] colorTable = GetColorTable(reader, dibHeader, dibLength, startPos);
            int headerLength = (int)(reader.BaseStream.Position - startPos);
            return BuildBmp(dibHeader, colorTable, reader, dibLength - headerLength);
        }

        private static byte[] BuildColorTableData(DrColor[] colorTable)
        {
            byte[] colorTableData = new byte[colorTable.Length * 4];

            for (int i = 0; i < colorTable.Length; i++)
            {
                colorTableData[4 * i] = (byte)colorTable[i].B;
                colorTableData[4 * i + 1] = (byte)colorTable[i].G;
                colorTableData[4 * i + 2] = (byte)colorTable[i].R;
                colorTableData[4 * i + 3] = 0x00;
            }

            return colorTableData;
        }

        /// <summary>
        /// Builds valid BMP.
        /// </summary>
        internal static byte[] BuildBmp(
            DibHeader dibHeader, byte[] colorTable, BinaryReader reader, int bitmapDataLength)
        {
            // BMP layout:
            // - BitmapFileHeader
            // - BitmapInfoHeader
            // - Color table
            // - Bitmap data
            int bmpHeaderSize = BitmapFileHeader.StructureSize + dibHeader.Size + colorTable.Length;
            byte[] bmp = new byte[bmpHeaderSize + bitmapDataLength];
            using (MemoryStream stream = new MemoryStream(bmp))
            {
                BinaryWriter writer = new BinaryWriter(stream);
                BitmapFileHeader fileHeader = new BitmapFileHeader();
                fileHeader.Size = (uint)(bmpHeaderSize + bitmapDataLength);
                fileHeader.OffBits = (uint)bmpHeaderSize;
                fileHeader.Write(writer);
                dibHeader.Write(writer);
                writer.Write(colorTable);
                writer.Flush();

                reader.Read(bmp, bmpHeaderSize, bitmapDataLength);

                return bmp;
            }
        }

        /// <summary>
        /// DDB - device dependent bitmaps is an old Windows bitmap format.
        /// They sometimes occur in RTF etc. They consist of raw scan data and some fields.
        /// http://msdn2.microsoft.com/en-us/library/ms532298.aspx
        ///
        /// This method converts a DDB into a BMP.
        /// Assumes 1 plane and 1 bit per pixel.
        /// </summary>
        public static byte[] DdbToBmp(
            int width,
            int height,
            int pixelsPerMeterX,
            int pixelsPerMeterY,
            int planes,
            int bitsPerPixel,
            int ddbWidthData,
            byte[] ddbData,
            DrColor[] colorTable)
        {
            if (planes != 1)
                throw new InvalidOperationException("Expected 1 plane only here.");

            // WORDSNET-22980 Do not throw but ignore non-monochrome images like MS Word does.
            if (bitsPerPixel != 1)
                return null;

            // We can calculate the resulting byte array length upfront.
            const int format1BppIndexedColorTableSize = 8;
            int bmpLength =
                BitmapFileHeader.StructureSize + // BMP header
                DibHeader.BitmapInfoHeaderSize + // DIB header
                format1BppIndexedColorTableSize + // 1 bit color colortable
                ddbData.Length * 2; // DDB scan data is 16 bit aligned, becomes 32 bit aligned.

            // Allocate the resulting byte array and the stream and writer for writing into the array.
            byte[] bmp = new byte[bmpLength];
            using (MemoryStream stream = new MemoryStream(bmp))
            {
                BinaryWriter writer = new BinaryWriter(stream);

                // Write the BMP header.
                BitmapFileHeader bmpHeader = new BitmapFileHeader();
                bmpHeader.Size = (uint)bmpLength;
                bmpHeader.OffBits =
                    BitmapFileHeader.StructureSize +
                    DibHeader.BitmapInfoHeaderSize +
                    format1BppIndexedColorTableSize;
                bmpHeader.Write(writer);

                // Write the DIB header.
                BitmapInfoHeader dibHeader = new BitmapInfoHeader();
                dibHeader.Width = width;
                dibHeader.Height = height;
                dibHeader.Planes = (ushort)planes;
                dibHeader.BitCount = (ushort)bitsPerPixel;
                dibHeader.XPelsPerMeter = pixelsPerMeterX;
                dibHeader.YPelsPerMeter = pixelsPerMeterY;
                dibHeader.Write(writer);

                // Write the color table.
                writer.Write(BuildColorTableData(colorTable));

                writer.Flush();

                // Convert scanData into DIB bytes and write.

                // DDB scan width is 16 bit aligned, but DIB scan width is 32 bit aligned.
                // This finds the width of the DIB scan line in bytes.
                int dibWidthBytes = MathUtil.RoundUp(ddbWidthData, 4);

                // This is how many zero bytes we need to add at the end of each DIB scan line.
                // I guess it could be 2 or 0.
                int padCount = dibWidthBytes - ddbWidthData;

                int dibIdx = (int)bmpHeader.OffBits;
                // Go through every scan line in DDB, but in reverse order because DDB is top-down, but DIB is bottom-up.
                for (int ddbY = height - 1; ddbY >= 0; ddbY--)
                {
                    // Go through every byte in the DDB scan line and copy to DIB.
                    for (int x = 0; x < ddbWidthData; x++)
                    {
                        int ddbIdx = ddbY * ddbWidthData + x;
                        bmp[dibIdx] = ddbData[ddbIdx];
                        dibIdx++;
                    }

                    // Add pad bytes to DIB scan lines.
                    for (int pad = 0; pad < padCount; pad++)
                    {
                        bmp[dibIdx] = 0;
                        dibIdx++;
                    }
                }
            }

            return bmp;
        }

        public static ImageSizeCore GetBmpSize(Stream imageStream)
        {
            imageStream.Position = BitmapFileHeader.StructureSize;
            BinaryReader reader = new BinaryReader(imageStream);
            BitmapInfoHeader infoHeader = DibHeader.ReadAsBitmapInfoHeader(reader);

            ImageSizeCore size = ImageSizeCore.CreateWithResolution(
                infoHeader.Width,
                Math.Abs(infoHeader.Height),
                PpmToDpi(infoHeader.XPelsPerMeter),
                PpmToDpi(infoHeader.YPelsPerMeter));

            return size;
        }

        /// <summary>
        /// Returns the size of a GIF image in twips. SPEED This method does not use the Image object to get the size.
        /// </summary>
        public static ImageSizeCore GetGifSize(byte[] imageBytes)
        {
            Debug.Assert(IsGif(imageBytes));

            using (MemoryStream stream = new MemoryStream(imageBytes))
                return GetGifSize(stream);
        }

        public static ImageSizeCore GetGifSize(Stream imageStream)
        {
            imageStream.Position = 6;
            BinaryReader reader = new BinaryReader(imageStream);
            ushort width = reader.ReadUInt16();
            ushort height = reader.ReadUInt16();

            // RK We need to check the GIF specification, if resolution is specified, the we should read it.
            return ImageSizeCore.CreateWithResolution(width, height, 0);
        }

        /// <summary>
        /// Returns the size of a WebP image in twips. SPEED This method does not use the Image object to get the size.
        /// </summary>
        public static ImageSizeCore GetWebPSize(Stream imageStream)
        {
            const int headerSize = 30;

            int width = 0;
            int height = 0;

            BinaryReader reader = new BinaryReader(imageStream);
            byte[] data = reader.ReadBytes(headerSize);
            string imageFormat = Encoding.ASCII.GetString(data, 12, 4);

            switch (imageFormat)
            {
                case "VP8 ":
                {
                    width = Get16BitData(data, 26) & 0x3FFF;
                    height = Get16BitData(data, 28) & 0x3FFF;
                    break;
                }
                case "VP8X":
                {
                    width = 1 + Get24BitData(data, 24);
                    height = 1 + Get24BitData(data, 27);
                    break;
                }
                case "VP8L":
                {
                    int firstBytes = Get16BitData(data, 21);
                    width = 1 + (firstBytes & 0x3FFF);
                    int lastTwoDigits = (firstBytes & 0xC000) >> 14;
                    height = 1 + ((Get16BitData(data, 23) & 0xFFF) << 2 | lastTwoDigits);
                    break;
                }
                default:
                {
                    // The WebP file is broken and contains no size information or has invalid encoding format.
                    return ImageSizeCore.CreateEmpty();
                }
            }

            imageStream.Position -= headerSize;

            return ImageSizeCore.CreateWithResolution(width, height, ImageConstants.StandardResolution);
        }

        private static int Get16BitData(byte[] data, int index)
        {
            return (data[index] & 0xFF) | ((data[index + 1] & 0xFF) << 8);
        }

        private static int Get24BitData(byte[] data, int index)
        {
            return Get16BitData(data, index) | ((data[index + 2] & 0xFF) << 16);
        }

        public static ImageSizeCore GetPngSize(Stream stream)
        {
            BigEndianBinaryReader reader = new BigEndianBinaryReader(stream);

            const int SignatureSize = 8;
            stream.Position = SignatureSize;

            int width = 0;
            int height = 0;
            double hRes = 0;
            double vRes = 0;

            // RK Need this flag to break the loop from within the switch.
            bool isExit = false;

            // Indicates that the size of image was already obtained.
            bool isSizeSet = false;

            while (!isExit && (stream.Position < stream.Length))
            {
                uint chunkLength = reader.ReadUInt32();
                string charType = new string(reader.ReadByteChars(4));

                switch (charType)
                {
                    case "IHDR":
                    {
                        // Width:              4 bytes
                        // Height:             4 bytes
                        // Bit depth:          1 byte
                        // Color type:         1 byte
                        // Compression method: 1 byte
                        // Filter method:      1 byte
                        // Interlace method:   1 byte

                        width = reader.ReadInt32();
                        height = reader.ReadInt32();

                        isSizeSet = true;

                        // Seek back to the beginning of the chunk data.
                        stream.Seek(-(4 + 4), SeekOrigin.Current);

                        break;
                    }
                    case "pHYs":
                    {
                        // Pixels per unit, X axis  4 bytes (PNG unsigned integer)
                        // Pixels per unit, Y axis  4 bytes (PNG unsigned integer)
                        // Unit specifier            1 byte

                        // WORDSNET-7590 The problem occurred because the images in the document have
                        // a very large value of pixelsPerUnitX and pixelsPerUnitX in pHYs.
                        // Their values equal maximum value of unsigned int, i.e. FF FF FF FF,
                        // this value is greater than maximum value of signed integer,
                        // that is why upon casting value becomes incorrect and resolution is calculated improperly.
                        long pixelsPerUnitX = (long)reader.ReadUInt32();
                        long pixelsPerUnitY = (long)reader.ReadUInt32();
                        short unitSpecifier = reader.ReadByte();

                        // Unit is the metre.
                        if (unitSpecifier == 1)
                        {
                            hRes = PpmToDpi(pixelsPerUnitX);
                            vRes = PpmToDpi(pixelsPerUnitY);
                        }

                        isExit = true;
                        break;
                    }
                    case "IEND":
                        // RK In one document PNG had extra 4 bytes at the end garbage,
                        // so we have to exit by the end tag, not by reaching the end of the file.
                        isExit = true;
                        break;
                    default:
                        // Do nothing.
                        break;
                }

                // Advance by the length of the chunk + CRC.
                uint shift = chunkLength + 4;

                // WORDSNET-17482
                // If the minimum data for calculating the size were successfully obtained,
                // the corrupted chunk length may be ignored, since this is not of interest for this method.
                // WORDSNET-18931
                // 8 bytes should be considered as the data containing the chunk length and chunk type
                // for the next step of the loop.
                if (isSizeSet && (stream.Position + shift + 8 >= stream.Length))
                    break;

                stream.Seek(shift, SeekOrigin.Current);
            }

            return ImageSizeCore.CreateWithResolution(width, height, hRes, vRes);
        }

        /// <summary>
        /// Gets orientation data from the image bytes.
        /// </summary>
        /// <param name="imageBytes">Image data bytes.</param>
        /// <returns>Obtained tiff orientation data.</returns>
        public static ExifOrientation GetJpegOrientation(byte[] imageBytes)
        {
            Debug.Assert(IsJpeg(imageBytes));

            using (MemoryStream stream = new MemoryStream(imageBytes))
                return GetJpegOrientation(stream);
        }

        /// <summary>
        /// Gets orientation data from the image bytes.
        /// </summary>
        /// <param name="imageStream">Image data stream.</param>
        /// <returns>Obtained tiff orientation data.</returns>
        public static ExifOrientation GetJpegOrientation(Stream imageStream)
        {
            Debug.Assert(imageStream.CanSeek); // Method logic changes current stream position.
            ExifOrientation orientation = ExifOrientation.Horizontal;

            try
            {
                BigEndianBinaryReader reader = new BigEndianBinaryReader(imageStream);
                JpegReader jpegReader = new JpegReader(reader);
                jpegReader.GoToNextMarker(); // Skip SOI (Start of Image Marker).

                // Process segments of the JPEG file.
                // DS: About 65% of checked files have no orientation data.
                // So, safe marker reading was introduced to avoid "EndOfStreamException" exception in normal processing.
                while (jpegReader.GoToNextMarker())
                {
                    if(jpegReader.IsEoiMarker)
                        break;

                    ushort segmentLength = reader.ReadUInt16();
                    long nextMarkerPosition = imageStream.Position + segmentLength - 2;

                    // EXIF segment.
                    if ((jpegReader.CurrentMarker == JpegReader.MarkerExif) && IsExifData(reader))
                    {
                        TiffDataReader dataReader = new TiffDataReader(reader);
                        orientation = dataReader.Orientation;
                        break;
                    }

                    // Proceed to the next segment.
                    imageStream.Position = nextMarkerPosition;
                }
            }
            catch (EndOfStreamException)
            {
                // Ignore end of stream i.e. may be invalid file. And return default value.
            }

            return orientation;
        }

        public static bool IsCmykOrYCCK(byte[] imageBytes)
        {
            using (MemoryStream ms = new MemoryStream(imageBytes))
                return IsCmykOrYCCK(ms);
        }

        /// <summary>
        /// Checks whether the image is CMYK or YCCK image. Works only with JPEG.
        /// </summary>
        public static bool IsCmykOrYCCK(Stream imageStream)
        {
            if (GetImageType(imageStream) != FileFormat.Jpeg)
                return false;

            long savePos = imageStream.Position;

            bool hasApp14Marker = false;
            // ap12 value of APP14 marker must be 0, 1 or 2. 8 means the value was not read.
            byte ap12Value = 8;
            // Components value of SOF must be 1, 3 or 4. 8 means the value was not read.
            byte components = 8;

            try
            {
                BigEndianBinaryReader reader = new BigEndianBinaryReader(imageStream);
                JpegReader jpegReader = new JpegReader(reader);

                // Skip SOI.
                jpegReader.GoToNextMarker();

                jpegReader.GoToNextMarker();
                // Search for SOF.
                // RK Values 0xFFC0 to 0xFFCF are all SOF (Start of Frame Markers) except 0xFFC4 and 0xFFCC.
                while ((jpegReader.CurrentMarker & 0xFFF0) != 0xFFC0 ||
                       jpegReader.CurrentMarker == 0xFFC4 ||
                       jpegReader.CurrentMarker == 0xFFCC)
                {
                    switch (jpegReader.CurrentMarker)
                    {
                        case 0xFFEE: // APP14 marker.
                            ushort app14FieldLength = reader.ReadUInt16();
                            // The first five bytes must be of the segment coded as X'41', X'64', X'6F', X'62', X'65', X'00'
                            // (the zero - terminated string "Adobe", according to Rec.ITU - T T.50 or ISO / IEC 646 coding)
                            string adobe = Encoding.UTF8.GetString(reader.ReadBytes(5));

                            // Shift to ap12.
                            reader.ReadBytes(5);

                            // Read the ap12 value.
                            ap12Value = reader.ReadByte();
                            hasApp14Marker = adobe == "Adobe";

                            imageStream.Seek(app14FieldLength - 13, SeekOrigin.Current);
                            jpegReader.GoToNextMarker();
                            continue;
                        default:
                            break;
                    }

                    ushort fieldLength = reader.ReadUInt16();
                    imageStream.Seek(fieldLength - 2, SeekOrigin.Current);

                    jpegReader.GoToNextMarker();
                }

                // WORDSNET-24845 Some images detected as PixelFormat.PixelFormat32bppCMYK in GDI.
                // But this value not exist in System.Drawing Enum and PixelFormat property is defined as 8207.
                // There are no exceptions with such Jpeg images processing in .Net Framework,
                // but such pictures are damaged in SkiaSharp. This method should recognize such pictures as CMYK(marker == 0xFFC2).
                // This is not critical when working with the Net Framework, but must be handled when working with SkiaSharp.
                if (jpegReader.CurrentMarker == 0xFFC0 || jpegReader.CurrentMarker == 0xFFC1 || jpegReader.CurrentMarker == 0xFFC2)
                {
                    // Skip field length.
                    reader.ReadUInt16();
                    // Skip the sample precision field.
                    reader.ReadByte();
                    // Skip image height from a frame segment.
                    reader.ReadUInt16();
                    // Skip image width from a frame segment.
                    reader.ReadUInt16();
                    // Read number of components.
                    components = reader.ReadByte();
                }
            }
            catch
            {
                // In case of reading problems silent exception and assume the image is not CMYK or YCCK.
                return false;
            }
            finally
            {
                imageStream.Position = savePos;
            }

            // If there are 4 components it is likely to be CMYK or YCCK image.
            // Another check. If there is APP14 marker and AP12 value of this marker is 0(CMYK) or 2(YCCK)
            if (((components == 4) && !hasApp14Marker) ||
            ((components == 4) && hasApp14Marker && (ap12Value == 0 || ap12Value == 2)))
                return true;

            // Otherwise assume the image is not CMYK or YCCK and can be used as is in PDF.
            return false;
        }

        public static ImageSizeCore GetJpegSize(Stream imageStream)
        {
            try
            {
                bool sizeFound = false;
                int height = 0;
                int width = 0;
                PointF resolution = PointF.Empty;

                BigEndianBinaryReader reader = new BigEndianBinaryReader(imageStream);
                JpegReader jpegReader = new JpegReader(reader);

                // Skip SOI (Start of Image Marker).
                jpegReader.GoToNextMarker();

                // Process segments of the JPEG file.
                jpegReader.GoToNextMarker();
                while (!sizeFound)
                {
                    ushort segmentLength = reader.ReadUInt16();
                    long nextMarkerPosition = imageStream.Position + segmentLength - 2;

                    // RK Values 0xFFC0 to 0xFFCF are all SOF (Start of Frame Markers) except 0xFFC4 and 0xFFCC.
                    if ((jpegReader.CurrentMarker & 0xFFF0) == 0xFFC0 &&
                        jpegReader.CurrentMarker != 0xFFC4 &&
                        jpegReader.CurrentMarker != 0xFFCC)
                    {
                        // Skip the sample precision field.
                        reader.ReadByte();
                        // Read image size from a frame segment.
                        height = reader.ReadUInt16();
                        width = reader.ReadUInt16();
                        sizeFound = true;
                    }
                    // APP0 Marker - possible JFIF segment.
                    else if (jpegReader.CurrentMarker == 0xFFE0)
                    {
                        resolution = ReadResolutionJfif(reader, resolution);
                    }
                    // APP1 Marker - possible EXIF segment.
                    else if (jpegReader.CurrentMarker == 0xFFE1)
                    {
                        resolution = ReadResolutionExif(reader, resolution);
                    }
                    // APP13 Photoshop image resource marker segment. (Saved as: IRB, 8BIM, IPTC)
                    else if ((jpegReader.CurrentMarker == 0xFFED) && (nextMarkerPosition >= 0x30))
                    {
                        resolution = PhotoshopMetadataReader.ReadResolution(reader, resolution);
                    }

                    // Proceed to the next segment.
                    imageStream.Position = nextMarkerPosition;
                    jpegReader.GoToNextMarker();
                }

                return ImageSizeCore.CreateWithResolution(width, height, resolution.X, resolution.Y);
            }
            catch (EndOfStreamException)
            {
                // WORDSNET-10253 If we reach the end of file while looking for its size and resolution, it means that
                // the JPEG file is broken and contains no size information or has invalid segment structure.
                return ImageSizeCore.CreateEmpty();
            }
        }

        /// <summary>
        /// Converts image data to PNG format.
        /// </summary>
        public static byte[] ConvertToPng(MemoryStream imageStream)
        {
            // alexnosk: in .NET Standard the underlying object of BitmapPal is SKBitmap,
            // when create it from stream the stream is disposed. To avoid ObjectDisposedException,
            // check Is16BppRgbTiff before creating a BitmapPal.
            bool is16BppRgbTiff = Is16BppRgbTiff(imageStream);

            using (BitmapPal bitmap = new BitmapPal(imageStream))
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    // WORDSNET-3898 The system library method Image.Save() incorrectly exports images from the
                    // 'Tiff' format with 16-bit per sample RGB color model to the 'Png' format.
                    // However, it exports correctly such images to 'Bmp' format.
                    if (is16BppRgbTiff)
                    {
                        using (BitmapPal bmpBitmap = ConvertToBmpFormat(bitmap))
                            bmpBitmap.Save(stream, FileFormat.Png);
                    }
                    else
                    {
                        bitmap.Save(stream, FileFormat.Png);
                    }

                    return StreamUtil.CopyStreamToByteArray(stream);
                }
            }
        }

        /// <summary>
        /// Returns image bytes in 'Png' format.
        /// </summary>
        public static byte[] ConvertToPng(byte[] imageBytes)
        {
            using (MemoryStream stream = new MemoryStream(imageBytes))
                return ConvertToPng(stream);
        }

        /// <summary>
        /// Converts specified bitmap to the 'Bmp' format.
        /// </summary>
        private static BitmapPal ConvertToBmpFormat(BitmapPal bitmap)
        {
            // This memory stream will be disposed by the BitmapPal.
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, FileFormat.Bmp);
            stream.Position = 0;
            return new BitmapPal(stream);
        }

        /// <summary>
        /// Returns true if the specified image stream is 'Tiff' with 16 bit per sample RGB format.
        /// </summary>
        private static bool Is16BppRgbTiff(MemoryStream imageStream)
        {
            bool is16BppRgb = false;
            long pos = imageStream.Position;
            if (IsTiff(imageStream))
            {
                imageStream.Position = pos;
                TiffDataReader reader = new TiffDataReader(imageStream);
                is16BppRgb = reader.Is16BppRgb();
            }

            imageStream.Position = pos;

            return is16BppRgb;
        }

        /// <summary>
        /// http://www.martinreddy.net/gfx/2d/JPEG.txt
        /// </summary>
        private static PointF ReadResolutionJfif(BigEndianBinaryReader reader, PointF resolution)
        {
            const int idLength = 5;
            const int versionLength = 2;

            reader.BaseStream.Seek(idLength + versionLength, SeekOrigin.Current);

            short units = reader.ReadByte();

            ushort destinyX = reader.ReadUInt16();
            ushort destinyY = reader.ReadUInt16();

            switch (units)
            {
                case 0:
                    // No uints, aspect ratio, skip?
                    break;
                case 1:
                    // Dots per inch.
                    resolution = new PointF(destinyX, destinyY);
                    break;
                case 2:
                    // Dots per cm.
                    const double cmPerInch = 2.54;
                    resolution = new PointF((float)(destinyX * cmPerInch), (float)(destinyY * cmPerInch));
                    break;
                default:
                    // Bad value, skip.
                    break;
            }

            return resolution;
        }

        private static bool IsExifData(BigEndianBinaryReader reader)
        {
            // Read EXIF signature.
            string sig = new string(reader.ReadByteChars(6));
            return sig.StartsWith("Exif", StringComparison.Ordinal);
        }

        /// <summary>
        /// http://park2.wakwak.com/~tsuruzoh/Computer/Digicams/exif-e.html#ExifData
        /// </summary>
        private static PointF ReadResolutionExif(BigEndianBinaryReader reader, PointF resolution)
        {
            if (IsExifData(reader))
            {
                TiffDataReader dataReader = new TiffDataReader(reader);

                // RK This code is probably by VS. In this case I think tag 0x0128 ResolutionUnit should
                // also be parsed it could specify units per inch or per centimeter.
                // alexnosk: WORDSNET-5142 The problem occurred because resolution read from Exif is 0 dpi, there is no
                // actual value of resolution.
                // So default resolution was used. To resolve the problem check whether values read from Exif are not zeros.
                if (dataReader.ImageXResolution > 0 && dataReader.ImageYResolution > 0)
                {
                    // Resolution are always stored in inches in our model.
                    double imageXResolution = dataReader.ImageXResolution;
                    double imageYResolution = dataReader.ImageYResolution;

                    // WORDSNET-10356 If resolution units read from 'EXIF' are centimeters,
                    // then we should convert it to inches for storing in the model.
                    // In addition, we should take into account that relationship between
                    // image size and resolution is inversed.
                    if (dataReader.ImageResolutionUnit == TiffResolutionUnitCore.Centimeter)
                    {
                        imageXResolution = ConvertUtilCore.InchToCm(imageXResolution);
                        imageYResolution = ConvertUtilCore.InchToCm(imageYResolution);
                    }

                    resolution = new PointF((float)imageXResolution, (float)imageYResolution);
                }
            }

            return resolution;
        }

        public static ImageSizeCore GetTiffSize(Stream imageStream)
        {
            TiffDataReader dataReader = new TiffDataReader(imageStream);

            return ImageSizeCore.CreateWithResolution(
                dataReader.ImageWidth,
                dataReader.ImageHeight,
                dataReader.ImageXResolution,
                dataReader.ImageYResolution);
        }

        public static ImageSizeCore GetIcoSize(Stream imageStream, int imageIndex)
        {
            BinaryReader reader = new BinaryReader(imageStream);

            int reserved = reader.ReadUInt16();
            Debug.Assert(reserved == 0);
            int type = reader.ReadUInt16();
            Debug.Assert(type == 1);
            int nrEntries = reader.ReadUInt16();
            Debug.Assert(nrEntries > 0);

            if (imageIndex < 0 || imageIndex >= nrEntries)
            {
                throw new ArgumentException("Couldn't extract image with index " + imageIndex +
                                            " from .ico file, because it contains only " + nrEntries + " image(s).");
            }

            for (int i = 0; i < nrEntries; ++i)
            {
                int width = reader.ReadByte();
                int height = reader.ReadByte();
                // Ignore nrColorPalette.
                reader.ReadByte();
                int reservedByte = reader.ReadByte();
                Debug.Assert(reservedByte == 0);
                int nrColorPanes = reader.ReadUInt16();
                Debug.Assert(nrColorPanes == 0 || nrColorPanes == 1);
                // Ignore bitsPerPixel.
                reader.ReadUInt16();
                // Ignore sizeInBytes.
                reader.ReadUInt32();
                // Ignore offset.
                reader.ReadUInt32();
                if (i == imageIndex)
                {
                    return ImageSizeCore.CreateWithResolution(width, height, ImageConstants.StandardResolution);
                }
            }

            throw new InvalidOperationException("Couldn't extract BMP/PNG image from .ico file.");
        }

        public static ImageSizeCore GetMetafileSize(byte[] imageBytes)
        {
            return GetMetafileSize(new MemoryStream(imageBytes));
        }

        public static ImageSizeCore GetMetafileSize(Stream imageStream)
        {
            // FOSS simplified
            Size defaultScreenSize = new Size(1280, 1024);
            const float defaultResolution = 96f;

            return ImageSizeCore.CreateWithResolution(
                defaultScreenSize.Width,
                defaultScreenSize.Height,
                defaultResolution);
        }

        public static ImageSizeCore GetPictSize(byte[] imageBytes)
        {
            Debug.Assert(IsPict(imageBytes));

            using (MemoryStream imageStream = new MemoryStream(imageBytes))
                return GetPictSize(imageStream);
        }

        public static ImageSizeCore GetPictSize(Stream imageStream)
        {
            BinaryReader reader = new BinaryReader(imageStream);

            // First goes file length to ushort. Skip.
            reader.ReadUInt16();

            int top = BitUtil.SwapInt16(reader.ReadInt16());
            int left = BitUtil.SwapInt16(reader.ReadInt16());
            int bottom = BitUtil.SwapInt16(reader.ReadInt16());
            int right = BitUtil.SwapInt16(reader.ReadInt16());
            int widthEmus = ConvertUtilCore.PointToEmu((double)(right - left));//JAVA-added casting
            int heightEmus = ConvertUtilCore.PointToEmu((double)(bottom - top));//JAVA-added casting

            return ImageSizeCore.CreateWithDimensions(left, top, right, bottom, widthEmus, heightEmus);
        }

        /// <summary>
        /// Puts a placeable WMF header before the image bytes.
        /// Use this method if you know the values that must go into the header.
        /// </summary>
        /// <param name="imageBytes">Must be bytes of a WMF file without a placeable header.</param>
        /// <param name="imageSize">Contains image size info. It is used to create the placeable header.</param>
        public static byte[] PrependWmfPlaceableHeader(byte[] imageBytes, ImageSizeCore imageSize)
        {
            Debug.Assert(IsStandardMetafile(imageBytes));

            byte[] header = GetWmfPlaceableHeader(imageSize);
            byte[] prepended = new byte[header.Length + imageBytes.Length];

            Array.Copy(header, prepended, header.Length);
            Array.Copy(imageBytes, 0, prepended, header.Length, imageBytes.Length);

            return prepended;
        }

        /// <summary>
        /// Puts a placeable WMF header before the image bytes.
        /// Use this method if you know the values that must go into the header.
        /// </summary>
        /// <param name="stream">Must be stream of a WMF file without a placeable header.</param>
        /// <param name="imageSize">Contains image size info. It is used to create the placeable header.</param>
        public static byte[] PrependWmfPlaceableHeader(Stream stream, ImageSizeCore imageSize)
        {
            byte[] header = GetWmfPlaceableHeader(imageSize);
            byte[] prepended = new byte[(int)(header.Length + stream.Length)];

            Array.Copy(header, prepended, header.Length);
            int read = stream.Read(prepended, header.Length, (int)stream.Length);

            Debug.Assert(read == stream.Length);

            return prepended;
        }

        private static byte[] GetWmfPlaceableHeader(ImageSizeCore imageSize)
        {
            using (MemoryStream result = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(result, Encoding.Unicode);

                int checksum = 0;

                const int MagicLow = 0xCDD7;
                const int MagicHigh = 0x9AC6;
                writer.Write((ushort)MagicLow); //magic number
                checksum ^= MagicLow;
                writer.Write((ushort)MagicHigh);
                checksum ^= MagicHigh;

                writer.Write((short)0); //handle
                checksum ^= 0;

                writer.Write((short)imageSize.Left);
                checksum ^= imageSize.Left;

                writer.Write((short)imageSize.Top);
                checksum ^= imageSize.Top;

                writer.Write((short)imageSize.Right);
                checksum ^= imageSize.Right;

                writer.Write((short)imageSize.Bottom);
                checksum ^= imageSize.Bottom;

                // Lets write horizontal resolution.
                writer.Write((short)imageSize.HorizontalResolution); //units per inch
                checksum ^= (int)imageSize.HorizontalResolution;

                writer.Write((short)0); //reserved
                checksum ^= 0;
                writer.Write((short)0); //reserved
                checksum ^= 0;

                writer.Write((ushort)checksum);

                return result.ToArray();
            }
        }

        /// <summary>
        /// Returns true if creating a bitmap of this size is likely to be too big for the system.
        /// If a bitmap is too big, .NET can fail with argument exception or out of memory exception.
        /// I've seen a problem where .NET fails to create a bitmap of 29Mpixels. I managed .NET to work with 18Mpixels without
        /// failing on my machine. So let's just set a hardcoded limit for approx 20Mpixels. You can use this method to check
        /// before creating or processing bitmaps.
        /// </summary>
        public static bool IsBitmapTooBigForSystem(int width, int height)
        {
            const int MaxPixels = 20 * 1024 * 1024;
            // WORDSNET-25310 Used long to avoid the loss of senior discharges
            long size = (long)width * height;
            return (size > MaxPixels);
        }

        internal static double PpmToDpi(long pixelsPerMeter)
        {
            return pixelsPerMeter / ImageConstants.InchesPerMeter;
        }

        /// <summary>
        /// Gets the stream that contains the red-cross image to represent missing pictures.
        /// </summary>
        /// <returns>The stream. Don't forget to close it when finished.</returns>
        [JavaThrows(true)]  // IO exceptions.
        public static Stream GetNoImageStream()
        {
            return new MemoryStream(GetNoImageBytes());
        }

        /// <summary>
        /// Gets the bytes of the red-cross image to represent missing pictures.
        /// </summary>
        [JavaThrows(true)]  // IO exceptions.
        public static byte[] GetNoImageBytes()
        {
            // alexnosk: WORDSNET-19721 Cache NoImage bytes to decrease number of resource reading calls and redundant
            // streams creation.
            // Size of cached data is about 1kb, I think it is ok to keep it in memory.
            // double-checked locking pattern.
            if (gNoImageCache == null)
            {
                lock (gNoImageCacheSyncRoot)
                {
                    if (gNoImageCache == null)
                    {
                        using (Stream stream = SystemPal.FetchResourceStream("Aspose.Resources.NoImage.png"))
                            gNoImageCache = StreamUtil.CopyStreamToByteArray(stream);
                    }
                }
            }

            return gNoImageCache;
        }

        /// <summary>
        /// Returns a value that indicates whether the pixel format for this image contains alpha information.
        /// </summary>
        public static bool HasAlphaChannel(byte[] imageBytes)
        {
            // Platform-specific code is moved to BitmapPal.
            // Ideally we should write optimized low-level code here, something like GetImageSize().
            return BitmapPal.ImageHasAlphaChannel(imageBytes);
        }

        /// <summary>
        /// Returns True if the FileFormat represents an image.
        /// </summary>
        public static bool IsImage(FileFormat fileFormat)
        {
            return fileFormat == FileFormat.Bmp
                   || fileFormat == FileFormat.Emf
                   || fileFormat == FileFormat.Gif
                   || fileFormat == FileFormat.Jpeg
                   || fileFormat == FileFormat.Png
                   || fileFormat == FileFormat.Tiff
                   || fileFormat == FileFormat.Wmf
                   || fileFormat == FileFormat.Pict
                   || fileFormat == FileFormat.Ico
                   || fileFormat == FileFormat.WebP
                   || fileFormat == FileFormat.Svg;
        }

        /// <summary>
        /// Converts resolution from pixels per inch (ppi) to pixels per meter (ppm)
        /// </summary>
        /// <param name="resolution">Resolution in pixels per inch</param>
        /// <returns>Resolution in pixels per meter</returns>
        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition(false)]
        private static uint ConvertPPItoPPM(double resolution)
        {
            // Defines meter to unit conversion coefficient.
            const double metersInInch = 0.0254;

            return (uint)Convert.ToUInt32(Math.Round(resolution / metersInInch));
        }

        /// <summary>
        /// WORDSNET-12001 MONO. Fix for PNG codec skips writing image resolution to PNG data.
        /// Add (if not exist) PNG resolution information to PNG data in a stream.
        /// </summary>
        /// <param name="inputStream">Input stream object with the original PNG data</param>
        /// <param name="outputStream">Output stream object where PNG data and pixel resolution will be written to.</param>
        /// <param name="horizontalResolution">Desired horizontal resolution.</param>
        /// <param name="verticalResolution">Desired horizontal resolution.</param>
        internal static void SavePngResolution(
            Stream inputStream, Stream outputStream, double horizontalResolution, double verticalResolution)
        {
            inputStream.Position = 0;

            BigEndianBinaryReader br = new BigEndianBinaryReader(inputStream);
            BigEndianBinaryWriter bw = new BigEndianBinaryWriter(outputStream);

            // Read/write png signature
            byte[] b = br.ReadBytes(8);
            bw.WriteBytes(b, 0, 8);

            // Flags that pHYs chunk already present or written in data
            bool pHYsWritten = false;

            // Start chunk read/write cycle
            while (true)
            {
                uint len = br.ReadUInt32();    // Read chunk length
                byte[] cname = br.ReadBytes(4);         // Read chunk name
                byte[] data = br.ReadBytes((int)len);   // Read chunk data
                uint crc = br.ReadUInt32();    // Read CRC

                string sname = Encoding.ASCII.GetString(cname);

                // Check if pixel dimensions chunk already stored in file
                if (sname == "pHYs")
                    pHYsWritten = true;

                // According to PNG 1.2 specification pHYs chunk should be before IDAT chunk
                // Png file may contain several IDAT chunks.
                // So if IDAT chunk in process and pHYs is not already present/written write them
                if (sname == "IDAT" && !pHYsWritten)
                {
                    // always 9 bytes length
                    bw.WriteUInt32(9);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        BigEndianBinaryWriter phys_bw = new BigEndianBinaryWriter(memoryStream);

                        // pHYs chunk signature
                        phys_bw.WriteBytes(new byte[] { (byte)'p', (byte)'H', (byte)'Y', (byte)'s' }, 0, 4);
                        phys_bw.WriteUInt32(ConvertPPItoPPM(horizontalResolution));
                        phys_bw.WriteUInt32(ConvertPPItoPPM(verticalResolution));
                        // Meter unit
                        phys_bw.WriteByte(1);
                        phys_bw.Flush();

                        byte[] buffer = memoryStream.GetBuffer();
                        bw.WriteBytes(buffer, 0, (int)memoryStream.Length);

                        // Write CRC
                        bw.WriteUInt32(Crc32.ComputeCRC(buffer, 0, (int)memoryStream.Length));
                    }

                    pHYsWritten = true;
                }

                // Copy chunk that has been recently read into output stream
                bw.WriteUInt32(len);
                bw.WriteBytes(cname, 0, 4);
                bw.WriteBytes(data, 0, (int)len);
                bw.WriteUInt32(crc);

                // IEND is strictly last chunk in file
                if (sname == "IEND")
                    break;
            }

            bw.Flush();
        }

#if !NETSTANDARD
        /// <summary>
        /// The method reads stream as Bitmap image, if the format is not supported in GDI(WebP or Dib)
        /// then it is converted to PNG format. If the attempt to read fails, an exception is thrown.
        /// </summary>
        [JavaDelete]
        internal static Bitmap TryReadAsBitmap(Stream stream)
        {
            try
            {
                return new Bitmap(stream);
            }
            // WORDSNET-7384 If failed to recognize image data as a common image type, then try to load imagedata from rare image formats.
            catch (ArgumentException)
            {
                if (ImageUtil.IsDib(stream))
                {
                    stream.Position = 0;
                    BinaryReader reader = new BinaryReader(stream);
                    byte[] imageBytes = ImageUtil.PrependBmpHeader(reader, (int)stream.Length);
                    return new Bitmap(new MemoryStream(imageBytes));
                }

                if (ImageUtil.IsWebP(stream))
                {
                    stream.Position = 0;
                    return WebPConverterPal.ReadBySkia(stream);
                }

                throw;
            }
        }
#endif

        /// <summary>
        /// Checks if given byte array can be xml.
        /// </summary>
        private static bool CanBeXml(Stream stream)
        {
            // Xml can starts with space/tab/linefeed.
            // 200 first bytes should be enough for common xml.
            // Valid xml must have the first significant char '<', no matter '<?' or '<svg_or_anyxmltag'.
            // Another point is that false positive is acceptable, and false negative is not.
            // If check individual chars, not the whole string, we don't care about byte order and UTF-8/16/32.

            const int maxLength = 200;
            int dataLength = (stream.Length > maxLength) ? maxLength : (int)stream.Length;
            byte[] data = new byte[200];
            StreamUtil.Read(stream, data, 0, dataLength);
            stream.Position = 0;

            for (int i = 0; i < dataLength; i++)
            {
                byte tmp = data[i];

                if (IsIgnoredValue(tmp))
                    continue;

                return (tmp == '<');
            }

            return false;
        }

        /// <summary>
        /// Returns true if given value should be ignored in xml checking;
        /// Tab, line feed, space, nil, byte order marks are ignored.
        /// </summary>
        private static bool IsIgnoredValue(byte value)
        {
            byte[] ignored = new byte[10];

            // Different byte order marks.
            ignored[0] = 0xFE;
            ignored[1] = 0xFF;
            ignored[2] = 0xEF;
            ignored[3] = 0xBB;
            ignored[4] = 0xBF;

            // Insignificant symbols and nil.
            ignored[5] = 0x00;
            ignored[6] = (byte)'\r';
            ignored[7] = (byte)'\n';
            ignored[8] = (byte)'\t';
            ignored[9] = (byte)' ';

            for (int i = 0; i < ignored.Length; i++)
            {
                if (ignored[i] == value)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks whether the specified image is colored.
        /// </summary>
        /// <returns>True if image considered to be colored.</returns>
        internal static bool IsImageColored(byte[] imageBytes)
        {
            using (BitmapPal bitmap = new BitmapPal(imageBytes))
            {
#if !NETSTANDARD
                if (bitmap.PixelFormat == PixelFormat.Format16bppGrayScale)
                    return false;
#endif
                using (BitmapPal bitmapArgb = bitmap.ConvertTo32BppArgb())
                {
                    BitmapDataPal bitmapData = bitmapArgb.LockBits();
#if JAVA
                    //Java ArgbData returns Buffer insted of byte[]
                    int pixelCount = bitmapData.ArgbDataLength / 4;
#else
                    int pixelCount = bitmapData.ArgbData.Length / 4;
#endif

                    for (int i = 0; i < pixelCount; i++)
                    {
                        byte r = bitmapData.GetR(i);
                        byte g = bitmapData.GetG(i);
                        byte b = bitmapData.GetB(i);

                        int rg = Math.Abs(r - g);
                        int gb = Math.Abs(g - b);
                        int br = Math.Abs(b - r);

                        int colorDiff = Math.Max(rg, Math.Max(gb, br));
                        if (colorDiff > DrColorUtil.ColorDifferenceThreshold)
                        {
                            bitmapData.UnlockBits();
                            return true;
                        }
                    }

                    bitmapData.UnlockBits();
                }
            }

            return false;
        }

        private static readonly object gNoImageCacheSyncRoot = new object();
        private static volatile byte[] gNoImageCache;
    }
}
