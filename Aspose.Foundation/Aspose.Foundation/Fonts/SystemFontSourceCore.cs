// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/01/2012 by Konstantin Kornilov

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.Collections;
using Aspose.Common;
using Aspose.IO;
using Aspose.JavaAttributes;
using Aspose.Warnings;

namespace Aspose.Fonts
{    
    /// <summary>
    /// Represents all fonts installed to the system.
    /// </summary>
    public class SystemFontSourceCore : FontSourceBaseCore
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public SystemFontSourceCore()
        {
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        public SystemFontSourceCore(int priority)
            : base(priority)
        {
        }

        /// <summary>
        /// Scans this font source and returns all available font data.
        /// </summary>
        [JavaThrows(false)]
        public override IEnumerable<IFontData> GetFontDataInternal()
        {
            return GetSystemFontData(WarningCallbackCore);
        }
        
        [JavaThrows(false)]
        public static IEnumerable<IFontData> GetSystemFontData(IWarningCallbackCore warningCallbackCore)
        {
            try
            {
                if (PlatformUtilPal.IsWindows())
                    return GetSystemFontDataWin();

                return GetSystemFontDataNonWin();
            }
            catch (Exception ex)
            {
                string msg = string.Format("Error loading font from system folder: {0}", ex.Message);
                Warn(warningCallbackCore, msg);
                // Silence the exceptions. We don't want to crash when accessing font directories.
                return new List<IFontData>();
            }
        }

        private static IEnumerable<IFontData> GetSystemFontDataWin()
        {
            // Use hashtable because most files from registry on Windows platform duplicates files from fonts directory.
            StringToObjDictionary<string> fileNames = new StringToObjDictionary<string>(false);

            string systemFolder = SystemPal.GetWindowsFontsFolder();
            if (!StringUtil.HasChars(systemFolder) || !Directory.Exists(systemFolder))
                return new List<IFontData>();

            // WORDSNET-20152 Experiment shows that Windows do not use fonts in subfolders. Particularly "Deleted"
            // subfolder is used to temporary store locked font files. 
            foreach (string file in FsPathUtil.GetFilesFromFolder(systemFolder, false))
                fileNames[file] = file;

            // On Windows platform we also check the registry for font files.
            SystemPal.GetFontFileNamesFromRegistry(fileNames);

            List<IFontData> list = new List<IFontData>();
            StringToObjDictionary<string>.Enumerator enumerator = fileNames.GetEnumerator();
            while (enumerator.MoveNext())
            {
                // WORDSNET-9222 The problem occurred because there can be registry keys, which are linked to the font files, 
                // which do not exist. Fixed by filtering out non-existing files.
                string fileName = enumerator.CurrentKey;
                if (StringUtil.HasChars(fileName) && File.Exists(fileName))
                    list.Add(new FileFontData(fileName));
            }

            return list;
        }
        
        private static IEnumerable<IFontData> GetSystemFontDataNonWin()
        {
            List<IFontData> result = new List<IFontData>();
            foreach (string systemFolder in GetSystemFontsFolders())
            {
                if (!StringUtil.HasChars(systemFolder) || !Directory.Exists(systemFolder))
                    continue;

                // Font resolving may depend on files order in rare cases. Directory.GetFiles do not guarantee
                // the output order. So sort files array to prevent issues in tests.
                List<string> files = FsPathUtil.GetFilesFromFolder(systemFolder, true);
                files.Sort(StringOrdinalIgnoreCaseComparer.Default);
                foreach (string file in files)
                    result.Add(new FileFontData(file));
            }

            return result;
        }

        /// <summary>
        /// Returns system font folders or empty array if folders are inaccessible.
        /// </summary>
        public static string[] GetSystemFontsFolders()
        {
            switch (PlatformUtilPal.GetPlatform())
            {
                case Platform.Windows:
                    return new string[] { SystemPal.GetWindowsFontsFolder() };
                case Platform.MacOS:
                    return SystemPal.GetMacOSFontsFolder();
                case Platform.iOS:
                    return SystemPal.GetIosFontsFolder();
                case Platform.Unix:
                    return SystemPal.GetLinuxFontFolders();
                case Platform.Android:
                    return SystemPal.GetAndroidFontFolders();
                default:
                    return new string[0];
            }
        }
    }
}
