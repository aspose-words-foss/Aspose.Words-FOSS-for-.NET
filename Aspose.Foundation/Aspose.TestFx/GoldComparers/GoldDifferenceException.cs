// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/06/2009 by Roman Korchagin
using System;
using System.Collections.Generic;
using System.IO;

namespace Aspose.TestFx.GoldComparers
{
    /// <summary>
    /// Thrown when a comparer detects a difference between the Out file and the Gold file.
    /// </summary>
    [Serializable]
    public class GoldDifferenceException : Exception
    {
        /// <summary>
        /// Exception constructor with additional parameter which helps to identify filenames being compared.
        /// </summary>
        public GoldDifferenceException(string filename)
            : base(string.Format("Files are not the same. {0}", JoinFileNames(filename)))
        {
        }

        public GoldDifferenceException(string f1, string f2)
            : base(string.Format("Files are not the same. {0}", JoinFileNames(f1, f2)))
        {
        }

        private static string JoinFileNames(params string[] filenames)
        {
            Debug.Assert(filenames.Length != 0);
            
            List<string> fullFilePaths = new List<string>();
            foreach (string filename in filenames)
            {
                if (string.IsNullOrEmpty(filename))
                    continue;
                
                fullFilePaths.Add(Path.GetFullPath(filename));
            }

            if (fullFilePaths.Count == 1)
                return fullFilePaths[0];

            fullFilePaths.Insert(0, string.Empty);
            return string.Join(StringUtil.NewLine + "\t", fullFilePaths.ToArray());
        }
    }
}
