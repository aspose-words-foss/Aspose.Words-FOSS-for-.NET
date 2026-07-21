// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/03/2014 by Alexey Morozov

using System;
using System.IO;
using System.Text;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Ole.Moniker
{
    /// <summary>
    /// Implements [MS-OSHARED] 2.3.7.8 FileMoniker. http://msdn.microsoft.com/en-us/library/dd951987(v=office.12).aspx
    /// </summary>
    internal class FileMoniker : MonikerBase
    {
        internal override void Read(BinaryReader reader)
        {
            // cAnti
            reader.ReadInt16();

            int ansiLength = reader.ReadInt32();
            mAnsiPath = DocBinaryReader.ReadAChar(reader, ansiLength);

            // endServer
            reader.ReadUInt16();

            int versionNumber = reader.ReadUInt16();
            Debug.Assert(versionNumber == 0xdead);

            // reserved1
            reader.ReadBytes(16);

            // reserved2
            reader.ReadBytes(4);

            int cbUnicodePathSize = reader.ReadInt32();

            if(cbUnicodePathSize > 0)
            {
                int cbUnicodePathBytes = reader.ReadInt32();
                int usKeyValue = reader.ReadInt16();
                Debug.Assert(usKeyValue == 3);

                byte[] unicodePathBytes = reader.ReadBytes(cbUnicodePathBytes);

                mUnicodePath = Encoding.Unicode.GetString(unicodePathBytes);
            }
        }

        internal override void Write(BinaryWriter writer)
        {
            // cAnti
            writer.Write((Int16)0);

            // AnsiPath
            if (StringUtil.HasChars(mAnsiPath))
            {
                writer.Write(mAnsiPath.Length + 1);
                DocBinaryWriter.WriteAnsiString(writer, mAnsiPath, 1251, false);
            }
            else
            {
                writer.Write(0);
            }

            // EndServer
            writer.Write((UInt16)0xffff);

            // VersionNumber
            writer.Write((UInt16)0xdead);

            // Reserved1
            writer.Write(new byte[16]);

            // Reserved2
            writer.Write(new byte[4]);

            byte[] unicodePathBytes = Encoding.Unicode.GetBytes(mUnicodePath);

            // cbUnicodePathSize
            writer.Write(unicodePathBytes.Length + 6 /* cbUnicodePathBytes + usKeyValue */);

            // cbUnicodePathBytes
            writer.Write(unicodePathBytes.Length);

            // usKeyValue
            writer.Write((UInt16)3);

            writer.Write(unicodePathBytes);
        }

        internal override Guid ClsId
        {
            get { return FileMonikerClsId; }
        }

        internal string AnsiPath
        {
            get { return mAnsiPath; }
        }

        internal string UnicodePath
        {
            get { return mUnicodePath; }
            set { mUnicodePath = value; }
        }

        private string mAnsiPath;
        private string mUnicodePath;
    }
}
