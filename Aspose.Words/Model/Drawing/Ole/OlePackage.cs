// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/03/2014 by Alexey Morozov

using System;
using System.IO;
using System.Text;
using Aspose.IO;
using Aspose.Words.Drawing.Core;
using Aspose.Words.RW.Ole.Ole2;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Allows to access OLE Package properties.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-ole-objects/">Working with Ole Objects</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// OLE package is a legacy and "undocumented" way to store embedded object if OLE handler is unknown.
    /// Early Windows versions such as Windows 3.1, 95 and 98 had Packager.exe application which could be used to embed any type of data into document.
    /// Now this application is excluded from Windows but MS Word and other applications still use it to embed data if OLE handler is missing or unknown.
    /// </remarks>
    public class OlePackage
    {
        /// <summary>
        /// Gets or sets OLE Package file name.
        /// </summary>
        public string FileName
        {
            get
            {
                return mFileName;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                if (!value.Equals(mFileName, StringComparison.Ordinal))
                {
                    mFileName = value;
                    TempFileName = mFileName;
                    UpdateOleObjectData();
                }
            }
        }

        /// <summary>
        /// Gets or sets OLE Package display name.
        /// </summary>
        public string DisplayName
        {
            get
            {
                return mDisplayName;
            }
            set
            {
                if (value == null)
                {
                    value = "";
                }
                if (!value.Equals(mDisplayName, StringComparison.Ordinal))
                {
                    mDisplayName = value;
                    UpdateOleObjectData();
                }
            }
        }

        internal OlePackage()
        {
        }

        /// <summary>
        /// Attaches OLE Package to parent OleObject.
        /// </summary>
        internal OlePackage(OleObject oleObject)
        {
            mOleObject = oleObject;
        }

        internal void Read(BinaryReader reader)
        {
            int header1 = reader.ReadUInt16();
            if (header1 != 0x0002)
                return;

            mDisplayName = ReadNullTerminatedString(reader);
            mFileName = ReadNullTerminatedString(reader);

            // There should be 2bytes of 0.
            reader.ReadUInt16(); // int zero =

            // Read second header, should be 03 00 or 01 00 (have no idea what does this header mean).
            int header2 = reader.ReadUInt16();

            switch (header2)
            {
                // WORDSNET-10087 Ole10NativeStream ends with filename2 if header2 is 0x0001, so there is no content.
                case 0x0001:
                {
                    // read 2 more bytes, which should be 01 00
                    reader.ReadUInt16(); // int bytes0001 =

                    // Read file name 2.
                    // I think it is ok to read it as a null-terminated string, instead of using the preceding length.
                    TempFileName = ReadNullTerminatedString(reader);

                    Data = null;

                    break;
                }
                case 0x0003:
                {
                    // Read length of the filename that goes next.
                    reader.ReadInt32(); // int filenameLength =

                    // Read file name 2.
                    // I think it is ok to read it as a null-terminated string, instead of using the preceding length.
                    TempFileName = ReadNullTerminatedString(reader);

                    // Read file length.
                    int dataLength = reader.ReadInt32();
                    Data = reader.ReadBytes(dataLength);
                    break;
                }
                default:
                {
                    // If we read unknown header, stop reading and write nothing into the output stream.
                    return;
                }
            }

            // WORDSNET-20288 Try to read Unicode properties.
            ReadUnicodeProperties(reader);
        }

        /// <summary>
        /// Read Unicode properties that may be present.
        /// </summary>
        private void ReadUnicodeProperties(BinaryReader reader)
        {
            string unicodeString = ReadUnicodeProperty(reader);
            TempFileName = StringUtil.HasChars(unicodeString) ? unicodeString : TempFileName;

            unicodeString = ReadUnicodeProperty(reader);
            mDisplayName = StringUtil.HasChars(unicodeString) ? unicodeString : mDisplayName;

            unicodeString = ReadUnicodeProperty(reader);
            mFileName = StringUtil.HasChars(unicodeString) ? unicodeString : mFileName;
        }

        private static string ReadUnicodeProperty(BinaryReader reader)
        {
            if (StreamUtil.HasEnoughBytesToRead(reader, 4))
            {
                int len = reader.ReadInt32();

                if (len > 0)
                    return ReadLengthPrefixedUnicodeString(reader, len);
            }

            return string.Empty;
        }

        internal void Write(BinaryWriter writer)
        {
            // Header1
            writer.Write((ushort)0x0002);

            WriteNullTerminatedString(writer, mDisplayName);
            WriteNullTerminatedString(writer, mFileName);

            writer.Write((ushort)0x00);
            if ((Data != null) && (Data.Length > 0))
            {
                // Header 2
                writer.Write((ushort)0x0003);

                // Here goes temporary file name that was used by Packager for unknown reason.
                writer.Write(TempFileName.Length + 1 /* include terminating zero */);
                WriteNullTerminatedString(writer, TempFileName);

                writer.Write(Data.Length);
                writer.Write(Data);

                // WORDSNET-20288 After 'reverse engineering' of the word files it was found was Word writes these values
                // in that order and by length prefixed unicode strings without terminating zero.
                // It's not documented, and may change in future.
                WriteLengthPrefixedUnicodeString(writer, TempFileName);
                WriteLengthPrefixedUnicodeString(writer, mDisplayName);
                WriteLengthPrefixedUnicodeString(writer, mFileName);
            }
            else
            {
                // Header 2
                writer.Write((ushort)0x0001);

                // Here goes temporary file name that was used by Packager for unknown reason.
                writer.Write((ushort)1);
                WriteNullTerminatedString(writer, TempFileName);
            }
        }

        internal bool IsLink
        {
            get { return Data == null; }
        }

        /// <summary>
        /// WORDSNET-15616 Provide ability to set file name and extension when inserting ole object using MemoryStream.
        /// Have to change properties on fly because Document.Сlone() clones only attributes
        /// and OleObject stores as attribute.
        /// </summary>
        private void UpdateOleObjectData()
        {
            if (null != mOleObject)
            {
                // Make OLE Package object.
                MemoryStream packageStream = new MemoryStream();
                Write(new BinaryWriter(packageStream));

                Ole10NativeStream ole10NativeStream = new Ole10NativeStream();
                ole10NativeStream.NativeData = packageStream.ToArray();

                // Update parent MemoryStorage with new OLE1 Native stream.
                ole10NativeStream.Write(mOleObject.Data);
            }
        }

        private static string ReadNullTerminatedString(BinaryReader reader)
        {
            StringBuilder sb = new StringBuilder();

            byte[] bytes = new byte[1];

            while (true)
            {
                reader.Read(bytes, 0, 1);

                char[] ch = Encoding.GetEncoding(DefaultCodePage).GetChars(bytes);

                if (ch[0] == '\0')
                    break;
                else
                    sb.Append(ch[0]);
            }

            return sb.ToString();
        }

        private static void WriteNullTerminatedString(BinaryWriter writer, string text)
        {
            writer.Write(Encoding.GetEncoding(DefaultCodePage).GetBytes(text));
            writer.Write((byte)0);
        }

        private static void WriteLengthPrefixedUnicodeString(BinaryWriter writer, string text)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            writer.Write(text.Length);
            writer.Write(bytes);
        }

        private static string ReadLengthPrefixedUnicodeString(BinaryReader reader, int len)
        {
            byte[] bytes = reader.ReadBytes(len * 2); //assume there is no 4bytes characters
            string result = Encoding.Unicode.GetString(bytes);
            return result;
        }

        internal string TempFileName;

        internal byte[] Data;

        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int DefaultCodePage = 1251;

        private string mFileName;

        private string mDisplayName;

        private readonly OleObject mOleObject;
    }
}
