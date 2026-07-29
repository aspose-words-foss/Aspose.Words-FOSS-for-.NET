// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/03/2014 by Alexey Morozov

using System;
using System.IO;
using System.Text;

namespace Aspose.Words.RW.Ole.Moniker
{
    /// <summary>
    /// Implements [MS-OSHARED] 2.3.7.6 URLMoniker. http://msdn.microsoft.com/en-us/library/dd906718(v=office.12).aspx
    /// </summary>
    internal class UrlMoniker : MonikerBase
    {
        internal UrlMoniker()
        {
        }

        internal UrlMoniker(string url)
        {
            mUrl = url;
        }

        internal override void Read(BinaryReader reader)
        {
            int length = reader.ReadInt32();

            int startPos = (int) reader.BaseStream.Position;

            StringBuilder sb = new StringBuilder();
            while ((reader.BaseStream.Position - startPos) < length)
            {
                char ch = (char) reader.ReadUInt16();
                if (ch == 0x00)
                    break;

                sb.Append(ch);
            }
            mUrl = sb.ToString();

            if ((reader.BaseStream.Position - startPos) < length)
            {
                // This field MUST equal {0xF4815879, 0x1D3B, 0x487F, 0xAF, 0x2C, 0x82, 0x5D, 0xC4, 0x85, 0x27, 0x63} if present.
                reader.ReadBytes(16); // Guid serialGuid

                int serialVersion = reader.ReadInt32();
                Debug.Assert(serialVersion == 0);

                reader.ReadInt32(); // int uriFlags
            }
        }

        internal override void Write(BinaryWriter writer)
        {
            byte[] unicodeBytes = Encoding.Unicode.GetBytes((mUrl != null) ? mUrl : "");

            writer.Write(unicodeBytes.Length + 2 /* zero terminator */);
            writer.Write(unicodeBytes);
            writer.Write((byte)0x00);
            writer.Write((byte)0x00);

            // Do not write other data.
        }

        internal override Guid ClsId
        {
            get { return UrlMonikerClsId; }
        }

        internal string Url
        {
            get { return mUrl; }
        }

        private string mUrl;
    }
}
