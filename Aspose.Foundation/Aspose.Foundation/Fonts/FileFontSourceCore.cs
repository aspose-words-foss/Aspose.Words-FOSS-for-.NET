// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/01/2012 by Konstantin Kornilov

using System.Collections.Generic;

namespace Aspose.Fonts
{
    /// <summary>
    /// Represents single font file stored in file system.
    /// </summary>
    public class FileFontSourceCore : FontSourceBaseCore
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public FileFontSourceCore(string filePath)
        {
            mFilePath = filePath;
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        public FileFontSourceCore(string filePath, int priority)
            : base(priority)
        {
            mFilePath = filePath;
        }

        /// <summary>
        /// Path to font file.
        /// </summary>
        public string FilePath
        {
            get { return mFilePath; }
        }

        /// <summary>
        /// Scans this font source and returns all available font data.
        /// </summary>
        public override IEnumerable<IFontData> GetFontDataInternal()
        {
            return new IFontData[] {new FileFontData(mFilePath)};
        }

        private readonly string mFilePath;
    }
}
