// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/09/2016 by Ivan Lyagin
// 
using System.IO;

namespace Aspose.TestFx.GoldComparers
{
    /// <summary>
    /// Provides document file streams to perform a GOLD comparison test.
    /// </summary>
    internal class FileComparisonStreamProvider : IComparisonStreamProvider
    {
        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        internal FileComparisonStreamProvider(string fileNameOut, string fileNameGold, string fileNameOriginalGold, string fileNameMS)
        {
            mFileNameOut = fileNameOut;
            mFileNameGold = fileNameGold;
            mFileNameOriginalGold = fileNameOriginalGold;
            mFileNameMS = fileNameMS;
        }

        Stream IComparisonStreamProvider.GetStreamOut()
        {
            return GetFileStream(mFileNameOut);
        }

        Stream IComparisonStreamProvider.GetStreamGold()
        {
            return GetFileStream(mFileNameGold);
        }

        Stream IComparisonStreamProvider.GetStreamOriginalGold()
        {
            return GetFileStream(mFileNameOriginalGold);
        }

        Stream IComparisonStreamProvider.GetStreamMS()
        {
            return GetFileStream(mFileNameMS);
        }

        private static Stream GetFileStream(string fileName)
        {
            return File.Exists(fileName) ? File.OpenRead(fileName) : null;
        }

        private readonly string mFileNameOut;
        private readonly string mFileNameGold;
        private readonly string mFileNameOriginalGold;
        private readonly string mFileNameMS;
    }
}
