// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/09/2017 by Konstantin Kornilov

using System;
using Aspose.Bidi;
using Aspose.Collections.Generic;
using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents 'cmap' format 2 subtable.
    /// </summary>
    internal class CmapSubtableFormat2 : CmapSubtable
    {
        public CmapSubtableFormat2(int platformId, int encodingId, int language, SortedIntegerListGeneric<int> charMap)
            : base(platformId, encodingId, language, charMap)
        {
        }

        public override void Write(BigEndianBinaryWriter writer)
        {
            // Currently this format is replaced with format 4 on writing. 
            throw new NotImplementedException();
        }

        public static CmapSubtableFormat2 Read(BigEndianBinaryReader reader, CmapEncodingRecord encodingRecord)
        {
            SortedIntegerListGeneric<int> charMap = new SortedIntegerListGeneric<int>();
            TTEncoding encoding = TTEncoding.GetEncoding(encodingRecord.PlatformId, encodingRecord.EncodingId);

            reader.BaseStream.Position = encodingRecord.Position;
            int format = reader.ReadUInt16();
            Debug.Assert(format == 2);

            reader.ReadUInt16(); // length
            int language = reader.ReadUInt16();

            int[] subheaderKeys = new int[256];
            for (int i = 0; i < 256; i++)
                subheaderKeys[i] = reader.ReadUInt16();

            long subheaderStartPosition = reader.BaseStream.Position;

            reader.ReadUInt16(); // subheader0FirstCode
            int subheader0EntryCount = reader.ReadUInt16();
            int subheader0IdDelta = reader.ReadInt16();
            int subheader0IdRangeOffset = reader.ReadUInt16();

            reader.BaseStream.Position += subheader0IdRangeOffset - 2;
            int[] subheader0Array = new int[subheader0EntryCount];
            for (int i = 0; i < subheader0EntryCount; i++)
                subheader0Array[i] = reader.ReadUInt16();

            for (int firstByte = 0; firstByte < 256; firstByte++)
            {
                int subheaderKey = subheaderKeys[firstByte];
                if (subheaderKey == 0)
                {
                    int glyphId = subheader0Array[firstByte] + subheader0IdDelta;
                    AddGlyph(new byte[] { (byte)firstByte }, glyphId, charMap, encoding);
                    continue;
                }

                reader.BaseStream.Position = subheaderStartPosition + subheaderKey;
                int firstCode = reader.ReadUInt16();
                int entryCount = reader.ReadUInt16();
                int idDelta = reader.ReadInt16();
                int idRangeOffset = reader.ReadUInt16();

                if(entryCount == 0)
                    continue;

                reader.BaseStream.Position += idRangeOffset - 2;
                for (int secondByte = firstCode; secondByte < firstCode + entryCount; secondByte++)
                {
                    int id = reader.ReadUInt16();
                    if(id == 0)
                        continue;

                    int glyphId = id + idDelta;
                    AddGlyph(new byte[] {(byte)firstByte, (byte)secondByte}, glyphId, charMap, encoding);
                }
            }

            return new CmapSubtableFormat2(encodingRecord.PlatformId, encodingRecord.EncodingId, language, charMap);
        }

        private static void AddGlyph(byte[] bytes, int glyphId, SortedIntegerListGeneric<int> charMap, TTEncoding encoding)
        {
            string uniStr = encoding.GetString(bytes);
            int charCode = UnicodeUtil.ConvertToUtf32(uniStr, 0);

            if (!charMap.ContainsKey(charCode))
                charMap.Add(charCode, glyphId);
        }
    }
}
