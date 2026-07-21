// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/09/2017 by Konstantin Kornilov

using Aspose.IO;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents a record in 'name' table.
    /// </summary>
    internal class TTFontNameRecord
    {
        public static TTFontNameRecord Read(BigEndianBinaryReader reader, long tableStart, int stringStorageOffset)
        {
            int platformId = reader.ReadUInt16();
            int encodingId = reader.ReadUInt16();
            int languageId = reader.ReadUInt16();
            NameString nameId = (NameString)reader.ReadUInt16();
            // String length (in bytes).
            int length = reader.ReadUInt16();
            // String offset from start of storage area (in bytes).
            int stringOffset = reader.ReadUInt16();

            TTEncoding encoding = TTEncoding.GetEncoding(platformId, encodingId);
            string name = null;
            if (encoding != null)
            {
                int savePos = (int)reader.BaseStream.Position;
                reader.BaseStream.Position = tableStart + stringStorageOffset + stringOffset;
                byte[] bytes = reader.ReadBytes(length);
                name = encoding.GetString(bytes);
                reader.BaseStream.Position = savePos;
            }

            if (!StringUtil.HasChars(name))
                return null;

            TTFontNameRecord result = new TTFontNameRecord();
            result.mPlatformId = platformId;
            result.mEncodingId = encodingId;
            result.mLanguageId = languageId;
            result.mNameId = nameId;
            result.mName = name;
            return result;
        }

        public int PlatformId
        {
            get { return mPlatformId; }
        }

        public int EncodingId
        {
            get { return mEncodingId; }
        }

        public int LanguageId
        {
            get { return mLanguageId; }
        }

        public NameString NameId
        {
            get { return mNameId; }
        }

        public string Name
        {
            get { return mName; }
        }

        public bool IsFontSpecificName
        {
            get { return IsFontSpecificNameId((int)NameId); }
        }

        public static bool IsFontSpecificNameId(int nameId)
        {
            return nameId >= 256 && nameId <= 32767;
        }

        private int mPlatformId;
        private int mEncodingId;
        private int mLanguageId;
        private NameString mNameId;
        private string mName;
    }
}
