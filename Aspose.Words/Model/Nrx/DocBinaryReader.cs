// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/02/2017 by Alexey Butalov

using System;
using System.IO;
using System.Text;
using Aspose.IO;

namespace Aspose.Words.Nrx
{
    /// <summary>
    /// Utility methods for reading binary format DOC files.
    /// </summary>
    internal static class DocBinaryReader
    {
        /// <summary>
        /// Reads Word style string in Unicode or 1252 encoding. 
        /// String is prefixed with length, then body and optional terminator.
        /// The length stored in the string does not include the terminator.
        /// The prefix of the string length is int16 for Unicode strings and byte for 1252 encoding strings.
        /// </summary>
        /// <param name="reader">Reader can be in any encoding.</param>
        /// <param name="isUnicode"></param>
        /// <param name="isTerminator">Specifies whether to read the zero terminator.</param>
        internal static string ReadWordString(BinaryReader reader, bool isUnicode, bool isTerminator)
        {
            if (isUnicode)
            {
                int length = reader.ReadUInt16() * 2;

                // WORDSNET-5512 Resilience from reading invalid strings.
                if (!StreamUtil.HasEnoughBytesToRead(reader, length))
                    length = 0;

                byte[] data = reader.ReadBytes(length);
                string s = DocStringUtil.GetEncoding(isUnicode).GetString(data);
                if (isTerminator)
                    reader.ReadUInt16();
                return s;
            }
            else
            {
                int length = reader.ReadByte();
                byte[] data = reader.ReadBytes(length);
                string s = DocStringUtil.GetEncoding(isUnicode).GetString(data);
                if (isTerminator)
                    reader.ReadByte();
                return s;
            }
        }

        /// <summary>
        /// Reads a string that has UInt16 length in chars, consists of Unicode characters, padded
        /// with zeroes to total length. Strips padding from the end of the string and positions
        /// the reader to be at the end of the padding.
        /// </summary>
        internal static string ReadWordStringWithPad(BinaryReader reader, int totalLength)
        {
            int startPos = (int)reader.BaseStream.Position;

            int lengthBytes = reader.ReadUInt16() * 2;

            // RESILIENCY Prevent reading more than specified by the caller.
            // Minus 2 because read two bytes already.
            lengthBytes = System.Math.Min(lengthBytes, totalLength - 2);

            byte[] data = reader.ReadBytes(lengthBytes);
            Encoding unicodeEncoding = DocStringUtil.GetEncoding(true);
            string result = unicodeEncoding.GetString(data);

            // We must set the position to the end of the padding.
            reader.BaseStream.Position = startPos + totalLength;

            return result;
        }

        /// <summary>
        /// Reads a WCHAR string. It is a unicode string with a zero terminator.
        /// </summary>
        internal static string ReadWChar(BinaryReader reader, int lengthBytes)
        {
            if (lengthBytes == 0)
                return String.Empty;

            byte[] data = reader.ReadBytes(lengthBytes);
            Encoding unicodeEncoding = DocStringUtil.GetEncoding(true);
            return unicodeEncoding.GetString(data, 0, lengthBytes - 2);
        }

        /// <summary>
        /// Reads int32 length of a string in bytes and then reads a WCHAR string.
        /// </summary>
        internal static string ReadWCharWithLengthBytes(BinaryReader reader)
        {
            int lengthBytes = reader.ReadInt32();
            return ReadWChar(reader, lengthBytes);
        }

        /// <summary>
        /// Reads int32 length of a string in chars and then reads a WCHAR string.
        /// </summary>
        internal static string ReadWCharWithLengthChars(BinaryReader reader)
        {
            int lengthBytes = reader.ReadInt32() * 2;
            return ReadWChar(reader, lengthBytes);
        }

        /// <summary>
        /// Reads an ACHAR 1252 encoding string null terminated.
        /// </summary>
        internal static string ReadAChar(BinaryReader reader, int lengthBytes)
        {
            if (lengthBytes == 0)
                return string.Empty;

            byte[] data = reader.ReadBytes(lengthBytes);
            Encoding win1252Encoding = DocStringUtil.GetEncoding(false);
            return win1252Encoding.GetString(data, 0, lengthBytes - 1);
        }

        /// <summary>
        ///  Reads an ACHAR null terminated string using given codepage.
        /// </summary>
        /// <remarks>
        /// AM. Above method uses cached encoding, think it's better do not refactor these methods.
        /// </remarks>
        internal static string ReadAChar(BinaryReader reader, int lengthBytes, int codepage)
        {
            if (lengthBytes == 0)
                return string.Empty;

            byte[] data = reader.ReadBytes(lengthBytes);
            Encoding encoding = Encoding.GetEncoding(codepage);
            return encoding.GetString(data, 0, lengthBytes - 1);
        }

        /// <summary>
        /// Reads int32 length of a string in bytes and then reads an ACHAR (1252 encoding) string.
        /// </summary>
        internal static string ReadACharLengthBytes(BinaryReader reader)
        {
            int lengthBytes = reader.ReadInt32();
            return ReadAChar(reader, lengthBytes);
        }

        /// <summary>
        /// Reds the specified number of characters in unicode or in 1252 encoding.
        /// </summary>
        /// <param name="reader">Read can be in any encoding.</param>
        /// <param name="isUnicode"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        internal static char[] ReadChars(BinaryReader reader, bool isUnicode, int count)
        {
            byte[] bytes = reader.ReadBytes((isUnicode) ? count * 2 : count);
            return DocStringUtil.GetEncoding(isUnicode).GetChars(bytes);
        }

        /// <summary>
        /// Reads ANSI string encoded with given code page.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="length">Lenght of string including trailing terminator.</param>
        /// <param name="codePage"></param>
        /// <returns></returns>
        internal static string ReadAnsiString(BinaryReader reader, int length, int codePage)
        {
            byte[] data = reader.ReadBytes(length - 1);
            reader.ReadByte();
            return Encoding.GetEncoding(codePage).GetString(data);
        }

        /// <summary>
        /// Reads ANSI string encoded with given codepage and prefixed with 16-bit length.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="codePage"></param>
        /// <returns></returns>
        internal static string ReadAnsiString(BinaryReader reader, int codePage)
        {
            return ReadAnsiString(reader, reader.ReadInt16(), codePage);
        }
    }
}
