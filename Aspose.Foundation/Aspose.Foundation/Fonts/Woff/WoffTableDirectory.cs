// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/11/2022 by Nikolay Sezganov

using Aspose.IO;

namespace Aspose.Fonts.Woff
{
    /// <summary>
    /// Directory of font tables, indicating the original size, compressed size and location of each table within the WOFF file.
    /// </summary>
    internal class WoffTableDirectory
    {
        internal WoffTableDirectory(
            string name,
            uint offset,
            uint compLength,
            uint origLength,
            uint origChecksum)
        {
            Name = name;
            Offset = offset;
            CompLength = compLength;
            OrigLength = origLength;
            OrigChecksum = origChecksum;
        }

        internal static WoffTableDirectory Read(BigEndianBinaryReader reader)
        {
            return new WoffTableDirectory(
                TagToString(reader.ReadUInt32()), // tag - 4-byte sfnt table identifier
                reader.ReadUInt32(), // offset - Offset to the data, from beginning of WOFF file.
                reader.ReadUInt32(), // compLength - Length of the compressed data, excluding padding.
                reader.ReadUInt32(), // origLength - Length of the uncompressed table, excluding padding.
                reader.ReadUInt32()); // origChecksum - Checksum of the uncompressed table.
        }

        internal string Name { get; }

        /// <summary>
        /// Offset to the data, from beginning of WOFF file.
        /// </summary>
        internal uint Offset { get; }

        /// <summary>
        /// Length of the compressed data, excluding padding.
        /// </summary>
        internal uint CompLength { get; }

        /// <summary>
        /// Length of the uncompressed table, excluding padding.
        /// </summary>
        internal uint OrigLength { get; }

        /// <summary>
        /// Gets a value indicating whether the data are compressed.
        /// </summary>
        internal bool IsCompressed
        {
            get { return CompLength != OrigLength; }
        }

        /// <summary>
        /// Checksum of the uncompressed table.
        /// </summary>
        internal uint OrigChecksum { get; }

        private static string TagToString(uint tag)
        {
            // Convert bytes to chars in big-endian order (high-order byte becomes the first char).
            char[] chars = new char[sizeof(uint)];
            for (int i = chars.Length - 1; i >= 0; i--)
            {
                chars[i] = (char)(tag & 0xFF);
                tag >>= 8;
            }
            return new string(chars);
        }
    }
}
