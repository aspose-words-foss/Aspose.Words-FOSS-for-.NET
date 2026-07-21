// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/09/2019 by Alexander Sevidov

using System.IO;
using System.Text;

namespace Aspose.Words.Vba
{
    /// <summary>
    /// Implements a binary reading routines for a VBA project.
    /// </summary>
    internal class VbaRecordReader
    {
        internal VbaRecordReader(BinaryReader binaryReader)
        {
            mReader = binaryReader;
        }

        internal Encoding Encoding
        {
            get { return mEncoding; }
            set { mEncoding = value; }
        }

        internal int ReadInt32Record(int id)
        {
            int readId = mReader.ReadUInt16();
            Debug.Assert(id == readId);
            int size = mReader.ReadInt32();
            Debug.Assert(size == 4);
            return mReader.ReadInt32();
        }

        internal int ReadInt32()
        {
            return mReader.ReadInt32();
        }

        internal short ReadInt16Record(int id)
        {
            int readId = mReader.ReadUInt16();
            Debug.Assert(id == readId);
            int size = mReader.ReadInt32();
            Debug.Assert(size == 2);
            return mReader.ReadInt16();
        }

        internal short ReadInt16()
        {
            return mReader.ReadInt16();
        }

        internal ushort ReadUInt16()
        {
            return mReader.ReadUInt16();
        }

        internal uint ReadUInt32()
        {
            return mReader.ReadUInt32();
        }

        internal byte[] ReadBytes(int count)
        {
            return mReader.ReadBytes(count);
        }

        internal string ReadStringRecord(int id)
        {
            return ReadStringRecord(id, mEncoding);
        }

        internal string ReadString()
        {
            return ReadString(mEncoding);
        }

        /// <summary>
        /// Reads the string record and corresponding unicode record.
        /// </summary>
        internal string ReadStringRecords(int id, int idUnicode)
        {
            string record = ReadStringRecord(id, mEncoding);
            string recordUnicode = ReadStringRecordOptional(idUnicode, Encoding.Unicode);
            return string.IsNullOrEmpty(recordUnicode) ? record : recordUnicode;
        }

        internal int PeekId()
        {
            int id = mReader.ReadUInt16();
            mReader.BaseStream.Position -= 2;

            return id;
        }

        internal void ReadVersionRecord(int id)
        {
            int readId = mReader.ReadUInt16();
            Debug.Assert(id == readId);
            int reserved = mReader.ReadInt32();
            Debug.Assert(reserved == 4);
        }

        private string ReadStringRecord(int id, Encoding encoding)
        {
            int readId = mReader.ReadUInt16();
            Debug.Assert(id == readId);
            return ReadString(encoding);
        }

        private string ReadString(Encoding encoding)
        {
            int size = mReader.ReadInt32();
            byte[] bytes = mReader.ReadBytes(size);
            return encoding.GetString(bytes);
        }

        /// <summary>
        /// Reads the optional string record.
        /// </summary>
        private string ReadStringRecordOptional(int id, Encoding encoding)
        {
            return (PeekId() == id) ? ReadStringRecord(id, encoding) : string.Empty;
        }

        private readonly BinaryReader mReader;
        private Encoding mEncoding;
    }
}
