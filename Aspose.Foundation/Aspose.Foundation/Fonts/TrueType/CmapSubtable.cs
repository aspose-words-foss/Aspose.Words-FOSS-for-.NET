// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2012 by Konstantin Kornilov

using System.IO;
using Aspose.Collections.Generic;
using Aspose.IO;
using Aspose.JavaAttributes;

namespace Aspose.Fonts.TrueType
{
    /// <summary>
    /// Represents subtable in 'cmap' table.
    /// </summary>
    internal abstract class CmapSubtable
    {
        protected CmapSubtable(int platformId, int encodingId, int language, SortedIntegerListGeneric<int> charMap)
        {
            mPlatformId = platformId;
            mEncodingId = encodingId;
            mCharMap = charMap;
            mLanguage = language;
        }

        [JavaThrows(true)]
        public abstract void Write(BigEndianBinaryWriter writer);

        public byte[] WriteToBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BigEndianBinaryWriter writer = new BigEndianBinaryWriter(stream);
                Write(writer);
                return stream.ToArray();
            }
        }

        public int EncodingId
        {
            get { return mEncodingId; }
        }

        public int PlatformId
        {
            get { return mPlatformId; }
        }

        public int Language
        {
            get { return mLanguage; }
        }

        /// <summary>
        /// Sorted list of characters - glyph ids pairs.
        /// Key - int char code. Value - int glyph id.
        /// </summary>
        public SortedIntegerListGeneric<int> CharMap
        {
            get { return mCharMap; }
        }

        private readonly int mPlatformId;
        private readonly int mEncodingId;
        private readonly SortedIntegerListGeneric<int> mCharMap;
        private readonly int mLanguage;
    }
}
