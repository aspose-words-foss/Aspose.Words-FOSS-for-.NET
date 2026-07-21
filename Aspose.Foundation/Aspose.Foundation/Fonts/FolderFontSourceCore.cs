// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/01/2012 by Konstantin Kornilov

using System;
using System.Collections.Generic;
using Aspose.IO;
using Aspose.JavaAttributes;
using Aspose.Warnings;

namespace Aspose.Fonts
{
    /// <summary>
    /// Represents the folder that contains font files.
    /// </summary>
    public class FolderFontSourceCore : FontSourceBaseCore
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public FolderFontSourceCore(string folderPath, bool scanSubfolders)
        {
            mFolderPath = folderPath;
            mScanSubfolders = scanSubfolders;
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        public FolderFontSourceCore(string folderPath, bool scanSubfolders, int priority)
            : base(priority)
        {
            mFolderPath = folderPath;
            mScanSubfolders = scanSubfolders;
        }

        /// <summary>
        /// Path to folder.
        /// </summary>
        public string FolderPath
        {
            get { return mFolderPath; }
        }

        /// <summary>
        /// Determines whether or not to scan subfolders.
        /// </summary>
        public bool ScanSubfolders
        {
            get { return mScanSubfolders; }
        }

        /// <summary>
        /// Scans this font source and returns all available font data.
        /// </summary>
        [JavaThrows(false)]
        public override IEnumerable<IFontData> GetFontDataInternal()
        {
            return GetFolderFontData(mFolderPath, mScanSubfolders, WarningCallbackCore);
        }

        [JavaThrows(false)]
        public static IEnumerable<IFontData> GetFolderFontData(
            string folderPath, bool scanSubfolders, IWarningCallbackCore warningCallbackCore)
        {
            try
            {
                if (!StringUtil.HasChars(folderPath))
                    return new List<IFontData>();

                // Font resolving may depend on files order in rare cases. Directory.GetFiles do not guarantee
                // the output order. So sort files array to prevent issues in tests.
                List<string> files = FsPathUtil.GetFilesFromFolder(folderPath, scanSubfolders);
                files.Sort(StringOrdinalIgnoreCaseComparer.Default);
                
                List<IFontData> list = new List<IFontData>();
                foreach (string file in files)
                    list.Add(new FileFontData(file));

                return list;
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error loading font from the folder \"{0}\": {1}", folderPath, ex.Message);
                Warn(warningCallbackCore, msg);

                // Silence the exceptions. We don't want to crash while accessing the font directories.
                return new List<IFontData>();
            }
        }

        private readonly string mFolderPath;
        private readonly bool mScanSubfolders;
    }
}
