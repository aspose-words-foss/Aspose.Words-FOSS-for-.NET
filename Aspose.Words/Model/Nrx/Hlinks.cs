// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/01/2014 by Alexey Morozov

using System.IO;
using System.Text;
using Aspose.Collections;

namespace Aspose.Words.Nrx
{
    /// <summary>
    /// Implements VecVtHyperlink structure. See http://msdn.microsoft.com/en-us/library/dd908491(v=office.12).aspx
    /// Used only during DOC import.
    /// </summary>
    internal class Hlinks
    {
        /// <summary>
        /// Reads VecVtHyperlink.
        /// </summary>
        internal Hlinks(byte[] data)
        {
            BinaryReader reader = new BinaryReader(new MemoryStream(data));

            // Item count.
            int cElements = reader.ReadInt32() / 6;

            // This array can have elements for few SubDocument. Elements must be sorted by dwAppValue descending. 
            // Break in this order means that next SubDocument should be selected.
            int lastDwApp = int.MaxValue;
            int subDocTypeIndex = 0;

            for (int i = 0; i < cElements; i++)
            {
                // Skip unused dwHash.
                ReadVT_I4(reader);
                int dwApp = ReadVT_I4(reader);
                int dwOfficeArt = ReadVT_I4(reader);
                // Skip unused dwInfo.
                ReadVT_I4(reader);

                string hlink1 = ReadVtString(reader);
                // Skip unused hlink2.
                ReadVtString(reader);

                // There are can be several type of links: links to field, links to OfficeArt shape, links to picture etc.
                // Current implementation deals with links to field only.
                if (dwOfficeArt == 0/*dwInfo == 5*/)
                {
                    // Order is broken, select next SubDocument.
                    if (dwApp >= lastDwApp)
                        subDocTypeIndex++;

                    IntToObjDictionary<string> curDictionary = (IntToObjDictionary<string>)mMapBySubDocType[subDocTypeIndex];
                    if (curDictionary == null)
                    {
                        curDictionary = new IntToObjDictionary<string>();
                        mMapBySubDocType[subDocTypeIndex] = curDictionary;
                    }

                    curDictionary.Add(dwApp, hlink1);

                    lastDwApp = dwApp;
                }
                else
                {
                    // Linked with something else. Ignore now.
                }
            }
        }

        /// <summary>
        /// Returns hyperlink for given index.
        /// </summary>
        internal string GetLink(SubDocType subDocType, int index)
        {
            IntToObjDictionary<string> curDictionary = (IntToObjDictionary<string>)mMapBySubDocType[(int)subDocType];

            if (curDictionary != null)
                return curDictionary[index];

            return null;
        }

        /// <summary>
        /// Reads VT_I4 value. See [MS-OLEPS] 2.15 TypedPropertyValue.
        /// </summary>
        private static int ReadVT_I4(BinaryReader reader)
        {
            int type = reader.ReadInt16();
            Debug.Assert(type == 0x0003 /* VT_I4 */);

            int padding = reader.ReadInt16();
            Debug.Assert(padding == 0);

            int value = reader.ReadInt32();
            return value;
        }

        /// <summary>
        /// Reads VT_LPWSTR value. See [MS-OLEPS] 2.15 TypedPropertyValue.
        /// </summary>
        private static string ReadVtString(BinaryReader reader)
        {
            int type = reader.ReadInt16();
            Debug.Assert(type == 0x001F /* VT_LPWSTR */);

            int padding = reader.ReadInt16();
            Debug.Assert(padding == 0);

            int cch = reader.ReadInt32();

            byte[] bytes = reader.ReadBytes(cch * 2);

            // padding.
            if (cch % 2 != 0)
                reader.BaseStream.Position += 2;

            string value = Encoding.Unicode.GetString(bytes, 0, bytes.Length - 2);
            return value;
        }

        /// <summary>
        /// Elements are IntToObjDictionary which elements maps indexes to hyperlink string.
        /// </summary>
        private readonly object[] mMapBySubDocType = new object[gSubDocTypeCount];

        private static readonly int gSubDocTypeCount = EnumUtilPal.GetEffectiveArrayLength(SubDocType.Comment.GetType(), 7);
    }
}
