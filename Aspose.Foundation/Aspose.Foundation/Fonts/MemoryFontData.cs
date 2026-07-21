// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2011 by Konstantin Kornilov

using System.IO;

namespace Aspose.Fonts
{
    /// <summary>
    /// Represents binary font data in memory.
    /// </summary>
    public class MemoryFontData : IFontData
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public MemoryFontData(byte[] data)
            : this(data, null, false)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        public MemoryFontData(byte[] data, string cacheKey, bool isEmbedded)
        {
            Debug.Assert(data != null);

            mData = data;
            mCacheKey = cacheKey;
            IsEmbedded = isEmbedded;
        }

        /// <summary>
        /// Byte array with font data.
        /// </summary>
        public byte[] Data
        {
            get { return mData; }
        }

        /// <summary>
        /// Opens the stream with font data.
        /// </summary>
        public Stream OpenStream()
        {
            return new MemoryStream(Data);
        }

        /// <summary>
        /// Returns the size of data in bytes.
        /// </summary>
        public int GetSize()
        {
            return Data.Length;
        }

        public string GetFilePath()
        {
            return null;
        }

        public string GetCacheKeyInternal()
        {
            return mCacheKey;
        }

        public byte[] GetFontBytes()
        {
            return mData;
        }

        public bool IsEmbedded { get; }

        private readonly byte[] mData;
        private readonly string mCacheKey;
    }
}
