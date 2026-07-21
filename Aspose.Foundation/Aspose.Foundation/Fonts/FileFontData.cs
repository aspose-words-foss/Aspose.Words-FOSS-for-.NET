// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2011 by Konstantin Kornilov

using System.IO;
using Aspose.IO;
using Aspose.JavaAttributes;

namespace Aspose.Fonts
{
    /// <summary>
    /// Represents binary font data in file.
    /// </summary>
    public class FileFontData : IFontData, IFileFontData
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public FileFontData(string fileName)
            : this(fileName, fileName)
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        public FileFontData(string fileName, string cacheKey)
        {
            mFileName = fileName;
            mCacheKey = cacheKey;
        }

        /// <summary>
        /// Font file name.
        /// </summary>
        public string FileName
        {
            get { return mFileName; }
        }

        /// <summary>
        /// Opens the stream with font data.
        /// </summary>
        public Stream OpenStream()
        {
            return File.OpenRead(FileName);
        }

        /// <summary>
        /// Returns the size of data in bytes.
        /// </summary>
        [JavaThrows(false)]
        public int GetSize()
        {
            FileInfo fileInfo = new FileInfo(FileName);
            return (int)fileInfo.Length;
        }

        public string GetFilePath()
        {
            return mFileName;
        }

        public string GetCacheKeyInternal()
        {
            return mCacheKey;
        }

        public byte[] GetFontBytes()
        {
            using (Stream stream = OpenStream())
                return StreamUtil.CopyStreamToByteArray(stream);
        }

        public bool IsEmbedded
        {
            get { return false; }
        }

        private readonly string mFileName;
        private readonly string mCacheKey;
    }
}
