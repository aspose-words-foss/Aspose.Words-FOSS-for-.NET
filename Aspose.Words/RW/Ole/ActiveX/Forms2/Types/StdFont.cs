// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/01/2019 by Ilya Navrotskiy

using System.IO;
using System.Text;

namespace Aspose.Words.Forms2
{
    /// <summary>
    /// Specifies the format of a standard font structure as persisted to a stream.
    /// </summary>
    /// <dev>
    /// [MS-OFORMS] 2.4.12 StdFont.
    /// </dev>
    internal class StdFont
    {
        /// <summary>
        /// Reads StdFont structure into <see cref="Forms2Attr.Font"/> key.
        /// </summary>
        internal static void Read(BinaryReader reader, Forms2Pr pr)
        {
            StdFont font = new StdFont();
            font.Read(reader);

            pr.SetAttr(Forms2Attr.Font, font);
        }

        /// <summary>
        /// Writes to a binary writer.
        /// </summary>
        internal void Write(BinaryWriter writer)
        {
            writer.Write(mVersion); 
            writer.Write(mCharset); 
            writer.Write(mFlags); 
            writer.Write(mWeight); 
            writer.Write(mHeight);

            byte[] fontFaceBytes = Encoding.ASCII.GetBytes(mFontFace);
            writer.Write((byte)fontFaceBytes.Length);
            writer.Write(fontFaceBytes);
        }

        /// <summary>
        /// Returns true, if this instance has default value in a specified collection.
        /// </summary>
        internal static bool HasDefaultValue(Forms2Pr pr)
        {
            return pr.IsDefaultValue(Forms2Attr.Font);
        }

        /// <summary>
        /// Reads from a binary reader.
        /// </summary>
        private void Read(BinaryReader reader)
        {
            mVersion = reader.ReadByte();
            mCharset = reader.ReadInt16();
            mFlags = reader.ReadByte();
            mWeight = reader.ReadInt16();
            mHeight = reader.ReadUInt32();
            
            int fontFaceLen = reader.ReadByte();
            byte[] fontFaceBytes = reader.ReadBytes(fontFaceLen);
            mFontFace = Encoding.ASCII.GetString(fontFaceBytes);
        }

        private byte mVersion;
        private short mCharset;
        private byte mFlags;
        private short mWeight;
        private uint mHeight;
        private string mFontFace;

        internal const string Guid = "0be35203-8f91-11ce-9de3-00aa004bb851";
    }
}
