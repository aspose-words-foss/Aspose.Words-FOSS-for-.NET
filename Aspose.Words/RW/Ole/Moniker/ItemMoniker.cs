// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/03/2014 by Alexey Morozov

using System;
using System.IO;
using System.Text;
using Aspose.Words.Nrx;

namespace Aspose.Words.RW.Ole.Moniker
{
    /// <summary>
    /// Implements [MS-OSHARED] 2.3.7.5 ItemMoniker. http://msdn.microsoft.com/en-us/library/dd926939(v=office.12).aspx
    /// </summary>
    internal class ItemMoniker : MonikerBase
    {
        internal ItemMoniker()
        {
        }

        internal ItemMoniker(string delimeter, string item)
        {
            mDelimeter = delimeter;
            mItem = item;
        }

        internal override void Read(BinaryReader reader)
        {
            int delimeterLength = reader.ReadInt32();
            mDelimeter = ReadAnsiNullTerminated(reader);

            int delimeterUnicodeSize = delimeterLength - (mDelimeter.Length + 1 /* include terminator */);
            if(delimeterUnicodeSize > 0)
            {
                // There is delimeterUnicode field.
                mDelimeter = DocBinaryReader.ReadWChar(reader, delimeterUnicodeSize/2);
            }

            int itemLength = reader.ReadInt32();
            mItem = ReadAnsiNullTerminated(reader);

            int itemUnicodeSize = itemLength - (mItem.Length + 1 /* include terminator */);
            if (itemUnicodeSize > 0)
            {
                // There is itemUnicode field.
                mItem = DocBinaryReader.ReadWChar(reader, itemUnicodeSize / 2);
            }
        }

        internal override void Write(BinaryWriter writer)
        {
            // AM. We never write this moniker so implementaion is postponed.
            throw new NotImplementedException();
        }

        internal override Guid ClsId
        {
            get { return ItemMonikerClsId; }
        }

// Seems I have this in OleUtil.
        private static string ReadAnsiNullTerminated(BinaryReader reader)
        {
            int startPos = (int)reader.BaseStream.Position;

            while (reader.ReadByte() != 0)
            {
                // Do nothing, just read the bytes.
            }

            int endPos = (int)reader.BaseStream.Position;

            reader.BaseStream.Position = startPos;
            byte[] bytes = reader.ReadBytes(endPos - startPos);

            return Encoding.Default.GetString(bytes, 0, bytes.Length - 1);
        }

        internal string Delimeter
        {
            get { return mDelimeter; }
        }

        internal string Item
        {
            get { return mItem; }
        }

        private string mDelimeter;
        private string mItem;
    }

}
