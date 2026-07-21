// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/04/2012 by Alexey Morozov

using System;
using System.IO;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Ole.Ole2
{
    /// <summary>
    /// Implements read/write for [MS-OLEDS] 2.3.1 ClipboardFormatOrAnsiString structure.
    /// </summary>
    internal class ClipboardFormat
    {
        internal ClipboardFormat(StandardClipboardFormat cf)
        {
            mStandardFormat = cf;
        }

        internal ClipboardFormat(string cf)
        {
            mRegisteredFormat = cf;
        }

        internal static ClipboardFormat Read(BinaryReader reader, bool isUnicode)
        {
            ClipboardFormat clipboardFormat;
            // Read clipboard format
            int markerOrlength = reader.ReadInt32();
            if (markerOrlength > 0)
            {
                clipboardFormat = new ClipboardFormat((isUnicode)
                    ? DocBinaryReader.ReadWChar(reader, markerOrlength)
                    : OleUtil.ReadLengthPrefixedAnsiString(reader, markerOrlength));
            }
            else if (markerOrlength < 0)
            {
                clipboardFormat = new ClipboardFormat((StandardClipboardFormat)reader.ReadInt32());
            }
            else
            {
                // In case markerOrLength equal 0 there is no data in this structure.
                clipboardFormat = new ClipboardFormat(StandardClipboardFormat.None);
            }

            return clipboardFormat;
        }

        internal void Write(BinaryWriter writer, bool isUnicode)
        {
            if(mStandardFormat != StandardClipboardFormat.None)
            {
                writer.Write(-1);              // markerOrlength
                writer.Write((Int32)mStandardFormat);
            }
            else if (StringUtil.HasChars(mRegisteredFormat))
            {
                if (isUnicode)
                    DocBinaryWriter.WriteWCharWithLengthChars(mRegisteredFormat, writer);
                else
                    OleUtil.WriteLengthPrefixedAnsiString(writer, mRegisteredFormat);
            }
            else
                writer.Write(0x00);
        }

        internal StandardClipboardFormat StandardFormat
        {
            get { return mStandardFormat; }
        }

        internal string RegisteredFormat
        {
            get { return mRegisteredFormat; }
        }

        private readonly StandardClipboardFormat mStandardFormat = StandardClipboardFormat.None;
        private readonly string mRegisteredFormat;
    }
}
