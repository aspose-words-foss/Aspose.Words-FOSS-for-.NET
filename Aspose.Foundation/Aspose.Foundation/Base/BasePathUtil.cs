// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/08/2016 by Konstantin Sidorenko

using System;
using System.IO;

namespace Aspose
{
    public static class BasePathUtil
    {
        /// <summary>
        /// Returns path to a temporary folder on disk
        /// </summary>
        /// <returns></returns>
        public static string GetTempFolderPath()
        {
            char[] folder = new char[8];
            string temp = Path.GetTempPath();
            for (int a = 0; a <= 10; a++)    // 10 retries
            {
                Random r = new Random(Environment.TickCount);
                for (int i = 0; i < folder.Length; i++)
                    folder[i] = (char)r.Next('a', 'z'+1);
                string target = Path.Combine(temp, new string(folder));
                if (File.Exists(target) || Directory.Exists(target))
                    continue;
                // Here some other process can create target, and we will happilly return it anyway
                Directory.CreateDirectory(target);
                return target;
            }
            throw new Exception("failed to create temporary folder");
        }

        public static DeletePathOnDispose UseTempFolderPath()
        {
            return new DeletePathOnDispose(GetTempFolderPath());
        }

        public class DeletePathOnDispose : IDisposable
        {
            public DeletePathOnDispose(string path)
            {
                _Path = path;
            }

            public string GetTmpPath() { return _Path; }

            public void Dispose()
            {
                if (_Path != null)
                {
                    Directory.Delete(_Path, true);
                    _Path = null;
                }
            }

            private string _Path;
        }
    }
}
