// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/12/2016 by Alexey Butalov

using System.IO;
using System.Text;

namespace Aspose.Words.RW.OfficeCrypto
{
    /// <summary>
    /// Utility methods to work with encrypted/signed etc Office documents.
    /// </summary>
    internal static class OfficeCryptoUtil
    {
        /// <summary>
        /// Reads a WCHAR string. It is a unicode string with a zero terminator.
        /// </summary>
        internal static string ReadWChar(BinaryReader reader, int lengthBytes)
        {
            if (lengthBytes == 0)
                return string.Empty;

            byte[] data = reader.ReadBytes(lengthBytes);
            return gUnicodeEncoding.GetString(data, 0, lengthBytes - 2);
        }

        /// <summary>
        /// Writes a WCHAR string. It is a unicode string with a zero termnator.
        /// Returns number of bytes written.
        /// </summary>
        internal static int WriteWChar(string value, BinaryWriter writer)
        {
            byte[] data = new byte[GetWCharLengthBytes(value)];
            gUnicodeEncoding.GetBytes(value, 0, value.Length, data, 0);
            writer.Write(data);
            return data.Length;
        }

        /// <summary>
        /// Returns the number of bytes needed to write a unicode string with a zero terminator.
        /// </summary>
        private static int GetWCharLengthBytes(string value)
        {
            return GetWCharLengthChars(value) * 2;
        }

        /// <summary>
        /// Returns the number of characters in a unicide string plus one for zero terminator.
        /// </summary>
        private static int GetWCharLengthChars(string value)
        {
            return value.Length + 1;
        }

        private static readonly Encoding gUnicodeEncoding = Encoding.Unicode;
    }
}
