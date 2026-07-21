// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2011 by Konstantin Kornilov

using System;
using System.Drawing;
using System.IO;
using System.Text;
using Aspose.Charset;
using Aspose.Fonts.TrueType;
using Aspose.JavaAttributes;

namespace Aspose.Fonts.EmbeddedOpenType
{
    /// <summary>
    /// Responsible for reading of Embedded Open Type (EOT) font files.
    /// See http://www.w3.org/Submission/EOT/ for more info.
    /// </summary>
    [CodePorting.Translator.Cs2Cpp.CppConstexpr]
    public static class EotFontFiler
    {
        /// <summary>
        /// Compresses OpenType font to EOT format.
        /// </summary>
        public static byte[] CompressOpenTypeToEot(byte[] openTypeFile, string fontName, bool isSubsetted)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                // MS Word uses MTX encoding for EOT files when font size is not greater than 4 Mb.
                // Otherwise it performs XOR encoding only.
                int fontDataLength = openTypeFile.Length;
                byte[] fontData;
                uint processingFlags;
                if (fontDataLength < FontUtil.MtxEncodeSizeLimit)
                {
                    fontData = MtxCoder.Encode(openTypeFile);
                    processingFlags = CompressedFlag;
                }
                else
                {
                    fontData = new byte[fontDataLength];
                    Array.Copy(openTypeFile, fontData, fontDataLength);
                    XorFontData(fontData);
                    processingFlags = XorEncryptedFlag;
                }

                if (fontData == null)
                    return null;

                if (isSubsetted)
                    processingFlags |= SubsetFlag;

                BinaryWriter writer = new BinaryWriter(outStream);

                uint headerSize;
                using (MemoryStream fontStream = new MemoryStream(openTypeFile))
                {
                    OpenTypeReader openTypeReader = OpenTypeReader.Create(fontStream, new MemoryFontData(openTypeFile), fontName);
                    openTypeReader.ReadHeader();
                    // MS Word seem uses EOT format version 2.2 for a moment.
                    // The RootString contains URLs from which the embedded font
                    // object may be referenced and for a moment it is empty.
                    headerSize = WriteEotHeaderV22(writer, openTypeReader, new byte[] { });
                }

                // Write version specific values to a header.
                long savedPosition = writer.BaseStream.Position;
                writer.BaseStream.Position = 0;

                // EOT Size
                uint eotSize = (uint)(headerSize + fontData.Length);
                writer.Write(eotSize);

                // Font data Size.
                writer.Write((uint)fontData.Length);

                // EOT format version.
                writer.Write((uint)EotVersion.Version22);

                // Processing flags.
                writer.Write(processingFlags);
                writer.BaseStream.Position = savedPosition;

                // Write font data.
                writer.Write(fontData);

                return outStream.ToArray();
            }
        }

        private static uint WriteEotHeaderV22(BinaryWriter writer, OpenTypeReader reader, byte[] rootString)
        {
            WriteEotHeaderV21(writer, reader, rootString);

            writer.Write(GetRootStringChecksum(rootString));

            // Codepage value needed for EUDC font support. Default Windows ANSI for a moment.
            writer.Write((uint)CodePage.WindowsLatin1CodePage);
            writer.Write(Padding);

            // Number of bytes used by the Signature array. Currently reserved and should be set to 0x0000.
            writer.Write((ushort)0x0000);

            // Processing flags for the EUDC font. Zero for a moment.
            writer.Write((uint)0x0);

            // Number of bytes used for the EUDC font data. Zero for a moment.
            writer.Write((uint)0x0);

            return (uint)writer.BaseStream.Position;
        }

        private static uint GetRootStringChecksum(byte[] rootString)
        {
            uint checksum = 0;
            foreach (byte t in rootString)
                checksum += t;

            // Value taken from the EOT spec - 4.3.2 RootString Checksum Calculation.
            const uint rootStringXorKey = 0x50475342;
            return checksum ^ rootStringXorKey;
        }

        /// <summary>
        /// Calculates root string checksum according to EOT spec.
        /// </summary>
        private static void WriteEotHeaderV21(BinaryWriter writer, OpenTypeReader reader, byte[] rootString)
        {
            WriteEotHeaderV10(writer, reader);

            writer.Write(Padding);
            writer.Write((ushort)rootString.Length);
            writer.Write(rootString);
        }

        private static void WriteEotHeaderV10(BinaryWriter writer, OpenTypeReader reader)
        {
            // Family name should be ended with '\0' character.
            string familyNameStr = string.Format("{0}{1}", reader.Name.FamilyName, '\0');
            byte[] familyName = Encoding.Unicode.GetBytes(familyNameStr);
            byte[] styleName = GetStyleUnicodeName(reader.Os2.Style);
            byte[] versionName = Encoding.Unicode.GetBytes(reader.Name.VersionString);
            byte[] fullName = Encoding.Unicode.GetBytes(reader.Name.FullFontName);

            // We are not writing EOT size, font data size and processing flags upon writing of a header.
            // We will write it later along with EOT font data.
            // EOT size.
            writer.Write((uint)0x0);

            // Font data size.
            writer.Write((uint)0x0);

            // EOT format version.
            writer.Write((uint)EotVersion.Version10);

            // Processing flags.
            writer.Write((uint)0x0);
            writer.Write(reader.Os2.panose.Values);
            writer.Write(reader.Os2.ulCodePageRanges.GetCharset());

            // Italic.
            writer.Write((byte)(((reader.Os2.fsSelection & 0x01) != 0) ? 1 : 0));
            writer.Write((uint)reader.Os2.usWeightClass);

            // Type flags that provide information about embedding permissions.
            writer.Write(reader.Os2.fsType.Value);

            // Magic number for EOT file - 0x504C. Used to check for data corruption.
            writer.Write(MagicNumber);

            // os/2.UnicodeRange1 (bits 0-31)
            writer.Write(reader.Os2.ulUnicodeRanges.Range1);
            // os/2.UnicodeRange2 (bits 32-63)
            writer.Write(reader.Os2.ulUnicodeRanges.Range2);
            // os/2.UnicodeRange3 (bits 64-95)
            writer.Write(reader.Os2.ulUnicodeRanges.Range3);
            // os/2.UnicodeRange4 (bits 96-127)
            writer.Write(reader.Os2.ulUnicodeRanges.Range4);

            // CodePageRange1 (bits 0-31)
            writer.Write(reader.Os2.ulCodePageRanges.Range1);
            // CodePageRange2 (bits 32-63)
            writer.Write(reader.Os2.ulCodePageRanges.Range2);

            // head.CheckSumAdjustment - See http://www.microsoft.com/typography/otspec/head.htm
            writer.Write(reader.Head.CheckSumAdjustment);

            // Reserved1
            writer.Write((uint)0x0);

            // Reserved2
            writer.Write((uint)0x0);

            // Reserved3
            writer.Write((uint)0x0);

            // Reserved4
            writer.Write((uint)0x0);

            writer.Write(Padding);
            writer.Write((ushort)familyName.Length);
            writer.Write(familyName);

            writer.Write(Padding);
            writer.Write((ushort)styleName.Length);
            writer.Write(styleName);

            writer.Write(Padding);
            writer.Write((ushort)versionName.Length);
            writer.Write(versionName);

            writer.Write(Padding);
            writer.Write((ushort)fullName.Length);
            writer.Write(fullName);
        }

        /// <summary>
        /// Returns byte array with font's style Unicode name. Name string will be ended with '\0' character.
        /// </summary>
        private static byte[] GetStyleUnicodeName(FontStyle style)
        {
            if (((style & FontStyle.Italic) != 0) && ((style & FontStyle.Bold) != 0))
                return Encoding.Unicode.GetBytes("BoldItalic\0");
            if ((style & FontStyle.Italic) != 0)
                return Encoding.Unicode.GetBytes("Italic\0");
            if ((style & FontStyle.Bold) != 0)
                return Encoding.Unicode.GetBytes("Bold\0");

            return Encoding.Unicode.GetBytes("Regular\0");
        }

        /// <summary>
        /// Tries to extract Open Type font from EOT file.
        /// Returns Open Type font data if possible and null otherwise.
        /// </summary>
        [JavaThrows(false)]
        public static byte[] TryExtractOpenTypeFromEot(byte[] eotFile)
        {
            try
            {
                return ExtractOpenTypeFromEot(eotFile);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Extracts Open Type font from EOT file.
        /// </summary>
        public static byte[] ExtractOpenTypeFromEot(byte[] eotFile)
        {
            if (eotFile == null || eotFile.Length == 0)
                return null;

            using (BinaryReader reader = new BinaryReader(new MemoryStream(eotFile)))
            {
                // Verify EOTSize field.
                uint eotSize = reader.ReadUInt32();
                if (eotSize != reader.BaseStream.Length)
                    throw new InvalidOperationException("The EOT file is not valid.");

                uint fontDataSize = reader.ReadUInt32();

                // Verify Version field.
                EotVersion version = ParseVersion(reader.ReadInt32());
                if (version == EotVersion.Unknown)
                    throw new InvalidOperationException("Unsupported EOT version.");

                int flags = reader.ReadInt32();

                reader.BaseStream.Position += 18;

                // Verify MagicNumber field.
                short magicNumber = reader.ReadInt16();
                if (magicNumber != MagicNumber)
                    throw new InvalidOperationException("The EOT file is not valid.");

                reader.BaseStream.Position += 46;

                ushort familyNameSize = reader.ReadUInt16();
                reader.BaseStream.Position += familyNameSize + 2;

                ushort styleNameSize = reader.ReadUInt16();
                reader.BaseStream.Position += styleNameSize + 2;

                ushort versionNameSize = reader.ReadUInt16();
                reader.BaseStream.Position += versionNameSize + 2;

                ushort fullNameSize = reader.ReadUInt16();
                reader.BaseStream.Position += fullNameSize;

                if (version == EotVersion.Version21 || version == EotVersion.Version22)
                {
                    reader.BaseStream.Position += 2;

                    ushort rootStringSize = reader.ReadUInt16();
                    reader.BaseStream.Position += rootStringSize;

                    if (version == EotVersion.Version22)
                    {
                        reader.BaseStream.Position += 10;

                        ushort signatureSize = reader.ReadUInt16();
                        reader.BaseStream.Position += signatureSize;

                        reader.BaseStream.Position += 4;

                        uint eudcFontSize = reader.ReadUInt32();
                        reader.BaseStream.Position += eudcFontSize;
                    }
                }

                // Verify FontDataSize filed.
                if (reader.BaseStream.Position + fontDataSize != reader.BaseStream.Length)
                    throw new InvalidOperationException("The EOT file is not valid.");

                byte[] fontData = reader.ReadBytes((int)fontDataSize);

                if (BitUtil.IsSetInt32(flags, XorEncryptedFlag))
                    XorFontData(fontData);

                if (BitUtil.IsSetInt32(flags, CompressedFlag))
                    fontData = MtxCoder.Decode(fontData);

                return fontData;
            }
        }

        /// <summary>
        /// Extracts style name of embedded font from EOT file.
        /// </summary>
        public static string GetStyleName(byte[] eotFile)
        {
            // Font family name goes before font style name and has variable length,
            // so we need to determine it to skip a number of it's bytes.
            int familyNameSize = GetFamilyNameSize(eotFile);
            // Offset of the style name size is equal to offset of font family size
            // plus 2 bytes of it's value plus font family name size plus 2 bytes of font style name size.
            int offsetStyleNameSize = OffsetFamilyNameSize + 2 + familyNameSize + 2;

            ushort styleNameSize = BitConverter.ToUInt16(eotFile, offsetStyleNameSize);

            // The string with style name starts from the offset of style name size plus 2 bytes of it's value.
            // This string is always stored in UTF16 encoding.
            return Encoding.Unicode.GetString(eotFile, offsetStyleNameSize + 2, styleNameSize);
        }

        /// <summary>
        /// Returns size of EOT file header.
        /// </summary>
        public static int GetHeaderSize(byte[] eotFile)
        {
            return  GetEotSize(eotFile) - GetFontDataSize(eotFile);
        }

        /// <summary>
        /// Returns value of 'Subset' flag from EOT file header processing flags.
        /// </summary>
        public static bool IsSubset(byte[] eotFile)
        {
            uint flags = GetFlags(eotFile);
            return ((flags & SubsetFlag) != 0);
        }

        /// <summary>
        /// Performs XOR operation with font data according to EOT processing spec.
        /// </summary>
        internal static void XorFontData(byte[] fontData)
        {
            // See http://www.w3.org/Submission/EOT/#Processing.
            for (int i = 0; i < fontData.Length; i++)
            {
                fontData[i] = (byte)(fontData[i] ^ XorKey);
            }
        }

        /// <summary>
        /// Returns size of EOT file.
        /// </summary>
        private static int GetEotSize(byte[] eotFile)
        {
            return BitConverter.ToInt32(eotFile, 0);
        }

        /// <summary>
        /// Returns size of EOT file font data.
        /// </summary>
        private static int GetFontDataSize(byte[] eotFile)
        {
            return BitConverter.ToInt32(eotFile, 4);
        }

        /// <summary>
        /// Parses specified integer to <see cref="EotVersion"/>.
        /// </summary>
        private static EotVersion ParseVersion(int version)
        {
            switch(version)
            {
                case (int)EotVersion.Version10:
                    return EotVersion.Version10;
                case (int)EotVersion.Version21:
                    return EotVersion.Version21;
                case (int)EotVersion.Version22:
                    return EotVersion.Version22;
                default:
                    return EotVersion.Unknown;
            }
        }

        /// <summary>
        /// Returns length of font family name string inside EOT file.
        /// </summary>
        private static int GetFamilyNameSize(byte[] eotFile)
        {
            return BitConverter.ToUInt16(eotFile, OffsetFamilyNameSize);
        }

        /// <summary>
        /// Extracts processing Flags from EOT file.
        /// </summary>
        private static uint GetFlags(byte[] eotFile)
        {
            const int offsetFlags = 12;
            return BitConverter.ToUInt32(eotFile, offsetFlags);
        }

        // EOT font processing flags.
        internal const int SubsetFlag = 0x00000001;
        internal const int CompressedFlag = 0x00000004;
        internal const int XorEncryptedFlag = 0x10000000;
        // Magic number used in EOT header.
        internal const short MagicNumber = 0x504C;
        private const byte XorKey = 0x50;

        // Offset of font family name size inside EOT file.
        // All values before this element have fixed lengths.
        private const int OffsetFamilyNameSize = 82;

        /// <summary>
        /// Padding value used in EOT header. Must always be set to 0x0000.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        private const ushort Padding = 0x0000;
    }
}
