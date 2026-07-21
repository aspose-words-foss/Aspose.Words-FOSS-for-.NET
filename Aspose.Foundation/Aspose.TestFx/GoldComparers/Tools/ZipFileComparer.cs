// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/05/2024 by Anton Savko

using Aspose.TestFx.GoldComparers.FailedTestCollector;

namespace Aspose.TestFx.GoldComparers
{
    public class ZipFileComparer : PackageFileComparer
    {
        protected override PackageComparisonInfo CreatePackageComparisonInfo(
            string fileNameSrc,
            string fileNameOut,
            string fileNameGold)
        {
            return new ZipComparisonInfo(fileNameSrc, fileNameOut, fileNameGold, null);
        }

        protected override ComparerFormResult ExecutePackageTestUI(
            string title,
            string fileNameOut,
            string fileNameGold,
            string fileNameOriginalGold,
            string fileNameSrc,
            string fileNameMS)
        {
            return AsposeComparer.ExecuteZipTestUI(title, fileNameOut, fileNameGold, fileNameOriginalGold, fileNameSrc, fileNameMS);
        }

        protected override void SaveGoldTestInfoIfNeeded(
            string title,
            string fileNameSrc,
            string fileNameOut,
            string fileNameGold,
            string fileNameMS)
        {
            FailedGoldTestCollector.SaveZipGoldTestInfoIfNeeded(title, fileNameSrc, fileNameOut, fileNameGold, fileNameMS);
        }
    }
}
