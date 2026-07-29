// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/09/2020 by Tengiz Sharafiev

using System.Collections.Generic;
using System.IO;

namespace Aspose.IO
{
    /// <summary>
    /// Contains utility methods for working with folder and file names.
    /// </summary>
    public static class FsPathUtil
    {
        /// <summary>
        /// Returns the names of files in the specified folder.
        /// </summary>
        /// <remarks>
        /// The method is a replacement of
        /// Directory.GetFiles(string path, string searchPattern, SearchOption searchOption)
        /// existing in .NET 2.0 and higher.
        /// </remarks>
        public static List<string> GetFilesFromFolder(string folderPath, bool scanSubfolders)
        {
            List<string> files = new List<string>();

            if (scanSubfolders)
                GetFilesFromFolderCore(files, folderPath);
            else
                files.AddRange(Directory.GetFiles(folderPath));

            return files;
        }

        private static void GetFilesFromFolderCore(List<string> files, string folderPath)
        {
            files.AddRange(Directory.GetFiles(folderPath));
            foreach (string subfolderPath in Directory.GetDirectories(folderPath))
                GetFilesFromFolderCore(files, subfolderPath);
        }
    }
}
