// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/05/2024 by Anton Savko

using System.IO;

namespace Aspose.TestFx.GoldComparers
{
    /// <summary>
    /// Stores information about the results of 'gold' test for OPC import/export.
    /// </summary>
    public class ZipComparisonInfo : PackageComparisonInfo
    {
        public ZipComparisonInfo(string filenameSrc, string filenameOut, string filenameGold, string filenameMS)
            : base(filenameSrc, filenameOut, filenameGold, filenameMS)
        {
            // Do nothing.
        }

        public ZipComparisonInfo(string filenameSrc, string filenameOut, string filenameGold, string filenameOriginalGold, string filenameMS)
            : base(filenameSrc, filenameOut, filenameGold, filenameOriginalGold, filenameMS)
        {
            // Do nothing.
        }

        public ZipComparisonInfo(IComparisonStreamProvider streamProvider)
            : base(null, null, null, null, null)
        {
            // Do nothing.
        }

        protected override ComparisonPackage CreatePackage(Stream stream)
        {
            return new ComparisonPackage(ZipPackageReader.ReadEntries(stream));
        }
    }
}
