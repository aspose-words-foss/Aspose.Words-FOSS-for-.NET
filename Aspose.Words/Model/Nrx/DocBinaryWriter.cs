// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/02/2017 by Alexey Butalov

using System;
using System.IO;
using System.Text;

namespace Aspose.Words.Nrx
{
    /// <summary>
    /// Utility methods for writing binary format DOC files.
    /// </summary>
    internal static class DocBinaryWriter
    {
        /// <summary>
        /// Writes 16 bit length and string characters in Unicode or 1252 encoding, optional 2 byte terminator.
        /// </summary>
        /// <returns>
        /// Returns total number of bytes written.
        /// </returns>
        internal static int WriteWordString(string value, int maxLengthChars, BinaryWriter writer, bool isUnicode, bool isTerminator)
        {
            int startPos = (int)writer.BaseStream.Position;

            value = (value != null) ? value : "";

            if (value.Length > maxLengthChars)
                value = value.Substring(0, maxLengthChars);

            if (isUnicode)
            {
                writer.Write((UInt16)value.Length);
                byte[] data = DocStringUtil.GetEncoding(true).GetBytes(value);
                writer.Write(data);
                if (isTerminator)
                    writer.Write((UInt16)0);
            }
            else
            {
                // WORDSNET-4502 A string is longer than maximum that can be encoded by a byte. Let's truncate.
                const int Max1252StringLength = 0xff;
                int length = (byte)System.Math.Min(value.Length, Max1252StringLength);
                writer.Write((byte)length);
                byte[] data = DocStringUtil.GetEncoding(false).GetBytes(value);
                writer.Write(data, 0, length);
                if (isTerminator)
                    writer.Write((byte)0);
            }

            return (int)writer.BaseStream.Position - startPos;
        }

        /// <summary>
        /// Writes a UInt16 length in chars, followed by Unicode characters, followed
        /// by padding up to the total length.
        /// </summary>
        internal static void WriteWordStringWithPad(string value, BinaryWriter writer, int totalLength)
        {
            // RESILIENCY Prevent writing more than specified by the caller. 
            int maxLengthChars = (totalLength / 2) - 1;
            int lengthChars = System.Math.Min(value.Length, maxLengthChars);

            writer.Write((UInt16)lengthChars);

            Encoding unicodeEncoding = DocStringUtil.GetEncoding(true);
            byte[] data = unicodeEncoding.GetBytes(value);
            int lengthBytes = lengthChars * 2;
            writer.Write(data, 0, lengthBytes);

            // Minus 2 because wrote two bytes already.
            int padLength = totalLength - lengthBytes - 2;
            writer.Write(new byte[padLength]);
        }

        /// <summary>
        /// Writes a WCHAR string. It is a unicode string with a zero termnator.
        /// Returns number of bytes written.
        /// </summary>
        internal static int WriteWChar(string value, BinaryWriter writer)
        {
            byte[] data = new byte[DocStringUtil.GetWCharLengthBytes(value)];
            Encoding unicodeEncoding = DocStringUtil.GetEncoding(true);
            unicodeEncoding.GetBytes(value, 0, value.Length, data, 0);
            writer.Write(data);
            return data.Length;
        }

        internal static void WriteWCharWithLengthChars(string value, BinaryWriter writer)
        {
            writer.Write(DocStringUtil.GetWCharLengthChars(value));
            WriteWChar(value, writer);
        }

        internal static void WriteWCharWithLengthBytes(string value, BinaryWriter writer)
        {
            writer.Write(DocStringUtil.GetWCharLengthBytes(value));
            WriteWChar(value, writer);
        }

        /// <summary>
        /// Writes ANSI string with given encoding. Optionally writes 16-bit length prefix.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="text"></param>
        /// <param name="codePage"></param>
        /// <param name="writeLength"></param>
        internal static void WriteAnsiString(BinaryWriter writer, string text, int codePage, bool writeLength)
        {
            if (writeLength)
                writer.Write((Int16)(text.Length + 1));

            writer.Write(Encoding.GetEncoding(codePage).GetBytes(text));
            writer.Write((byte)0x00);
        }
    }
}
