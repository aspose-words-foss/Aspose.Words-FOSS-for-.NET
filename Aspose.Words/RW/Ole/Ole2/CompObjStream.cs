// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2012 by Alexey Morozov

using System;
using System.IO;
using Aspose.IO;
using Aspose.Ss;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Ole.Ole2
{
    /// <summary>
    /// Implements reading/writing of [MS-OLEDS] 2.3.8 CompObjStream.
    /// </summary>
    internal class CompObjStream : Ole2StreamBase
    {
        internal static CompObjStream Read(MemoryStorage storage)
        {
            try
            {
                return storage.ContainsKey(CompObjStreamName) 
                    ? new CompObjStream(new BinaryReader(storage.GetStreamZeroPositioned(CompObjStreamName))) 
                    : null;
            }
            catch
            {
                // Skip any errors.
                // TestDefect11473.
            }

            return null;
        }

        internal CompObjStream()
        {
        }

        /// <summary>
        /// Reads CompObjStream stream. 
        /// </summary>
        internal CompObjStream(BinaryReader reader)
        {
            // Skip header reserved.
            reader.ReadInt32();
            // Skip header version
            reader.ReadInt32();
            // Skip header reserved.
            reader.ReadBytes(4);
            mClsid = new Guid(reader.ReadBytes(16));

            // AM. I've not seen UNICODE part filled so it seems that only ANSI part of this stream is used.
            mAnsiUserType = OleUtil.ReadLengthPrefixedAnsiString(reader);
            mAnsiClipboardFormat = ClipboardFormat.Read(reader, false);

            mReserved1 = OleUtil.ReadLengthPrefixedAnsiString(reader);

            // According to spec: If this field is present and is NOT set to 0x71B239F4, 
            // the remaining fields of the structure MUST be ignored on processing. 
            if (StreamUtil.HasEnoughBytesToRead(reader, 4))
            {
                if (reader.ReadInt32() == UnicodeMarker)
                {
                    // Skip unicode user type.
                    DocBinaryReader.ReadWCharWithLengthBytes(reader);
                    // Skip unicode clipboard format.
                    ClipboardFormat.Read(reader, true);

                    mReserved2 = DocBinaryReader.ReadWCharWithLengthBytes(reader);
                }
            }
        }

        protected override void Write(BinaryWriter writer)
        {
            // Write CompObjHeader. According to spec this can be set to any arbitrary value and 
            // MUST be ignored on processing. I write values extracted from test file.
            writer.Write(0xfffe0001);
            writer.Write(0x00000a03);
            writer.Write(0xffffffff);
            writer.Write(mClsid.ToByteArray());

            OleUtil.WriteLengthPrefixedAnsiString(writer, mAnsiUserType);
            ((mAnsiClipboardFormat == null) ? new ClipboardFormat("") : mAnsiClipboardFormat).Write(writer, false);
            OleUtil.WriteLengthPrefixedAnsiString(writer, mReserved1);

            writer.Write(UnicodeMarker);
            writer.Write(0x00);
            writer.Write(0x00);
            writer.Write(0x00);
        }

        protected override string Name
        {
            get { return CompObjStreamName; }
        }

        internal string UserType
        {
            get { return mAnsiUserType; }
            set { mAnsiUserType = value; }
        }

        internal Guid Clsid
        {
            get { return mClsid; }
            set { mClsid = value; }
        }

        /// <summary>
        /// Programmatic identifier.
        /// </summary>
        /// <remarks>
        /// Identifies a class but with less precision because it is not guaranteed to be globally unique.
        /// </remarks>
        internal string ProgId
        {
            get { return StringUtil.HasChars(mReserved1) ? mReserved1 : mReserved2; }
            set { mReserved1 = value; }
        }

        /// <summary>
        /// Gets or sets a ClipboardFormat of the CompObj stream.
        /// </summary>
        internal ClipboardFormat AnsiClipboardFormat
        {
            get { return mAnsiClipboardFormat; }
            set { mAnsiClipboardFormat = value; }
        }

        // ReSharper disable UnaccessedField.Local
        // Although some values are unused I left it for debug purpose it seems they might contain something useful.

        // CompObjHeader
        private readonly string mReserved2;
        // ReSharper restore UnaccessedField.Local

        // According to spec this is reserved field and must be ignored. 
        // However sometimes it seems to be the only way to get embedded clsid without Registry interaction.
        private Guid mClsid = Guid.Empty; // Struct initialization is added for Java.

        // CompObjStream.
        private string mReserved1;
        private string mAnsiUserType;
        private ClipboardFormat mAnsiClipboardFormat;

        private const int UnicodeMarker = 0x71B239F4;
    }
}
