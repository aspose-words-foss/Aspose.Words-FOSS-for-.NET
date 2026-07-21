// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/10/2019 by Alexander Sevidov

using System.IO;
using System.Text;

namespace Aspose.Words.Vba
{
    /// <summary>
    /// Implements a binary writing routines for a VBA project.
    /// </summary>
    internal class VbaRecordWriter
    {
        internal VbaRecordWriter(BinaryWriter binaryWriter, Encoding encoding)
        {
            mWriter = binaryWriter;
            mEncoding = encoding;
        }

        internal Encoding Encoding
        {
            get { return mEncoding; }
        }

        internal void WriteInt32Record(ushort id, int value)
        {
            mWriter.Write(id);
            int size = 4;
            mWriter.Write(size);
            mWriter.Write(value);
        }

        internal void WriteInt32(int value)
        {
            mWriter.Write(value);
        }

        internal void WriteInt16Record(ushort id, short value)
        {
            mWriter.Write(id);
            int size = 2;
            mWriter.Write(size);
            mWriter.Write(value);
        }

        internal void WriteInt16(short value)
        {
            mWriter.Write(value);
        }

        internal void WriteUInt16(ushort value)
        {
            mWriter.Write(value);
        }

        internal void WriteUInt32(uint value)
        {
            mWriter.Write(value);
        }

        internal void WriteBytes(byte[] bytes)
        {
            mWriter.Write(bytes);
        }

        internal void WriteStringRecord(ushort id, Encoding encoding, string value)
        {
            mWriter.Write(id);
            WriteString(encoding, value);
        }

        internal void WriteStringRecord(ushort id, string value)
        {
            WriteStringRecord(id, mEncoding, value);
        }

        internal void WriteString(Encoding encoding, string value)
        {
            byte[] bytes = value!= null ? encoding.GetBytes(value) : new byte[0];
            int size = bytes.Length;
            mWriter.Write(size);
            mWriter.Write(bytes);
        }

        internal void WriteString(string value)
        {
            WriteString(mEncoding, value);
        }

        private readonly BinaryWriter mWriter;
        private readonly Encoding mEncoding;
    }
}

