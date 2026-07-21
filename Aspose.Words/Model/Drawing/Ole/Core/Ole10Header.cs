// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/04/2010 by Roman Korchagin

using System.IO;
using System.Text;

namespace Aspose.Words.Drawing.Core
{
    /// <summary>
    /// We do not know exactly what this is, but it seems to occur at the beginning of 
    /// NativeData of OLE10 objects.
    /// </summary>
    internal class Ole10Header
    {
        internal Ole10Header(BinaryReader reader)
        {
            TotalLength = reader.ReadInt32();
            Header1 = reader.ReadUInt16();
            if (Header1 == 0x0002)
            {
                DisplayName = ReadAsciizString(reader);

                FileName1 = ReadAsciizString(reader);
                
                // There should be 2bytes of 0. Skip it.
                reader.ReadUInt16();

                // Read second header, should be 03 00 or 01 00 (have no idea what does this header mean).
                Header2 = reader.ReadUInt16();

                switch (Header2)
                {
                        // WORDSNET-10087 Ole10NativeStream ends with filename2 if header2 is 0x0001, so there is no content.
                    case 0x0001:
                    {
                        // read 2 more bytes, which should be 01 00
                        reader.ReadUInt16();

                        // Read file name 2.
                        // I think it is ok to read it as a null-terminated string, instead of using the preceding length.
                        FileName2 = ReadAsciizString(reader);
                        reader.ReadUInt16();

                        break;
                    }
                    case 0x0003:
                    {
                        // Read length of the filename that goes next.
                        reader.ReadInt32();

                        // Read file name 2.
                        // I think it is ok to read it as a null-terminated string, instead of using the preceding length.
                        FileName2 = ReadAsciizString(reader);

                        // Read file length.
                        ContentLength = reader.ReadInt32();
                        break;
                    }
                    default:
                    {
                        // If we read unknown header, stop reading and write nothing into the output stream.
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Read a null-terminated ASCII string.
        /// The reader must be using the ASCII encoding.
        /// </summary>
        private static string ReadAsciizString(BinaryReader reader)
        {
            StringBuilder sb = new StringBuilder();

            while (true)
            {
                char ch = reader.ReadChar();

                if (ch == '\0')
                    break;
                else
                    sb.Append(ch);
            }

            return sb.ToString();
        }

        internal int TotalLength;
        internal int Header1;
        internal string DisplayName;
        internal string FileName1;
        internal int Header2;
        internal string FileName2;
        internal int ContentLength;
    }
}
