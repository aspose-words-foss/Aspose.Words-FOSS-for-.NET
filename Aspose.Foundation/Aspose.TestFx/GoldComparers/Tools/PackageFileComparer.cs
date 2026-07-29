// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2016 by Vyacheslav Durin

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Aspose.JavaAttributes;

namespace Aspose.TestFx.GoldComparers
{
    public abstract class PackageFileComparer : AbstractGoldComparer
    {
        public override void Execute(ComparerParams comparerParams)
        {
            Execute(
                comparerParams.Title,
                comparerParams.FileNameSrc,
                comparerParams.FileNameOut,
                comparerParams.FileNameGold,
                comparerParams.FileNameMs);
        }

        public override bool ExecuteForm(ComparerParams comparerParams)
        {
            return ExecuteForm(
                comparerParams.Title,
                comparerParams.FileNameSrc,
                comparerParams.FileNameOut,
                comparerParams.FileNameGold,
                comparerParams.FileNameOriginalGold,
                comparerParams.FileNameMs);
        }

        /// <summary>
        /// Compares the generated package with the gold file and an "MS" file.
        /// Brings up the user interface when there is a difference.
        /// </summary>
        /// <param name="title">Title for the dialog box.</param>
        /// <param name="fileNameSrc">Original DOC file name.</param>
        /// <param name="fileNameOut">Name of the generated file.</param>
        /// <param name="fileNameGold">Name of the gold file.</param>
        /// <param name="fileNameMS">Name of the file created by MS Word.</param>
        public void Execute(
            string title,
            string fileNameSrc,
            string fileNameOut,
            string fileNameGold,
            string fileNameMS)
        {
            Execute(title, fileNameSrc, fileNameOut, fileNameGold, fileNameMS, null);
        }

        /// <summary>
        /// Compares the generated package with the gold file and an "MS" file.
        /// Brings up the user interface when there is a difference.
        /// Uses custom <see cref="IPackagePartComparer"/> implementation.
        /// </summary>
        /// <param name="title">Title for the dialog box.</param>
        /// <param name="fileNameSrc">Original DOC file name.</param>
        /// <param name="fileNameOut">Name of the generated file.</param>
        /// <param name="fileNameGold">Name of the gold file.</param>
        /// <param name="fileNameMS">Name of the file created by MS Word.</param>
        /// <param name="partComparer">Custom comparer of package parts.</param>
        public void Execute(
            string title,
            string fileNameSrc,
            string fileNameOut,
            string fileNameGold,
            string fileNameMS,
            IPackagePartComparer partComparer)
        {
            // Sometime Java has it's own version of gold file. Comparer form needs in former .Net gold too.
            string fileNameOriginalGold = fileNameGold;
            fileNameGold = TestFxUtil.GetReplacementGoldIfExists(fileNameGold);

            // RK We do not pass MS file name here because at the moment we only need to compare OUT vs GOLD.
            PackageComparisonInfo info = CreatePackageComparisonInfo(fileNameSrc, fileNameOut, fileNameGold);
            try
            {
                info.UpdateStatus(partComparer);
                if (info.GoldStatus != ComparisonStatus.Ok)
                    throw new GoldDifferenceException(fileNameOut, fileNameGold);

                if (TestSettings.ShowCompareAlways)
                    throw new Exception("Showing results even if there is no difference");
            }
            catch (Exception e)
            {
                bool passed = e is GoldDifferenceException && TryPassAutomatically(fileNameOut, fileNameGold);

                // Throw immediately if images are different and silent mode testing is selected.
                // RK We DO pass the MS file name here because in the UI form we want to compare OUT vs GOLD and OUT vs MS.
                if (!passed && !ExecuteForm(title, fileNameSrc, fileNameOut, fileNameGold, fileNameOriginalGold, fileNameMS))
                {
                    SaveGoldTestInfoIfNeeded(title, fileNameSrc, fileNameOut, fileNameGold, fileNameMS);
                    throw;
                }
            }
        }

        protected abstract PackageComparisonInfo CreatePackageComparisonInfo(
            string fileNameSrc,
            string fileNameOut,
            string fileNameGold);

        [JavaThrows(true)]
        protected abstract ComparerFormResult ExecutePackageTestUI(
            string title,
            string fileNameOut,
            string fileNameGold,
            string fileNameOriginalGold,
            string fileNameSrc,
            string fileNameMS);

        [JavaThrows(true)]
        protected abstract void SaveGoldTestInfoIfNeeded(
            string title,
            string fileNameSrc,
            string fileNameOut,
            string fileNameGold,
            string fileNameMS);

        private bool ExecuteForm(
            string title,
            string fileNameSrc,
            string fileNameOut,
            string fileNameGold,
            string fileNameOriginalGold,
            string fileNameMS)
        {
            if (TestSettings.DontShowCompareForm)
                return false;

            ComparerFormResult userChoice = ExecutePackageTestUI(title, fileNameOut, fileNameGold, fileNameOriginalGold, fileNameSrc, fileNameMS);

            // If the user clicks Accept we override the gold file, otherwise propagate exception to the caller.
            switch (userChoice)
            {
                case ComparerFormResult.Accept:
                    TestFxUtil.AcceptNewGold(fileNameOut, fileNameGold);
                    return true;
                case ComparerFormResult.SkipAndThrow:
                    return false;
                default:
                    throw new InvalidOperationException("Unexpected result from the comparer form.");
            }
        }

        /// <summary>
        /// Returns True if xps files have same page images and the rest of markup binary equal
        /// </summary>
        [AndroidDelete]
        public static bool CheckXpsMarkupAndPageImagesEqual(PackageComparisonInfo info)
        {
            Debug.Assert(info.FilenameOut.EndsWith(".xps", StringComparison.OrdinalIgnoreCase));

            // either out or gold are missing
            if (info.OutStatus != ComparisonStatus.Ok || info.GoldStatus == ComparisonStatus.Missing)
                return false;

            // this method should not be called when packages match
            if (info.GoldStatus == ComparisonStatus.Ok)
                return true;

            Debug.Assert(info.OutStatus == ComparisonStatus.Ok && info.GoldStatus == ComparisonStatus.Different);

            // we know that packages are different, thus some pages are different and/or some other parts are different
            // if other parts are different we return False, since its a significant difference
            // if some pages are diffrent we may return True where test settings tell us to pass

            bool passPixels = TestSettings.PassWhenImagesPixelsMatch;
            bool passLabel = TestSettings.PassWhenImagesDifferenceLabelsMatch;

            // if test settings do not tell us to pass pixel matching or label matching pages we can fail now
            // this is because we know that there is a difference in markup or pages
            // and if its in pages then they are either pixel or label matching, both of which we do not compare
            if (!passPixels && !passLabel)
                return false;

            List<String> nonMatchingPageNumbers = new List<String>();
            foreach (var pci in info.PartComparisonInfos)
            {
                // Part is missing in out compared to gold. A significant difference.
                if (pci.OutStatus != ComparisonStatus.Ok)
                    return false;

                Match m = Regex.Match(pci.Name, @"Pages/(\d+)\.fpage$", RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    // Missing gold counterpart
                    if (pci.GoldStatus == ComparisonStatus.Missing)
                        return false;

                    // NOTE here we assume that 1.fpage is 1st page
                    // in reality part name is not directly related to page number
                    // it is theoretically possible that we generated images of wrong pages
                    if (pci.GoldStatus == ComparisonStatus.Different)
                        nonMatchingPageNumbers.Add(m.Groups[1].Value);
                }
                else if (pci.GoldStatus != ComparisonStatus.Ok)
                {
                    // Not a page part which is not equal to gold. A significant difference.
                    return false;
                }
            }

            Debug.Assert(nonMatchingPageNumbers.Count > 0);

            // We can generate images from Xps, not from individual parts inside of it.
            // However we can specify for which pages we want images generated.
            string[] labels = passLabel ? TestSettings.Labels : new string[0];
            using (BasePathUtil.DeletePathOnDispose tmp = BasePathUtil.UseTempFolderPath())
            {
                string tempPath = tmp.GetTmpPath();
                string pages = String.Join(",", nonMatchingPageNumbers);
                XpsToPngClient.ConvertSourceParam outFile = new XpsToPngClient.ConvertSourceParam();
                outFile.InputFile = info.FilenameOut;
                outFile.Pages = pages;
                XpsToPngClient.ConvertSourceParam goldFile = new XpsToPngClient.ConvertSourceParam();
                goldFile.InputFile = info.FilenameGold;
                goldFile.Pages = pages;

                Aspose.TestFx.XpsToPngClient.Convert(tempPath, new XpsToPngClient.ConvertSourceParam[] { outFile, goldFile });

                // since we saved non-matching images only then no point in binary compare, we already know they are not equal
                // we pass folder name only, the diff will compute all corresponding differences and place them alongside
                DiffClient.Result[] results = Aspose.TestFx.DiffClient.Generate(tempPath, tempPath, null, null, passLabel, false);

                return CheckResult(passPixels, passLabel, labels, results);
            }
        }

        [AndroidDelete]
        private static bool CheckResult(bool passPixels, bool passLabel, string[] labels, DiffClient.Result[] results)
        {
            foreach (DiffClient.Result d in results)
                if (!string.IsNullOrEmpty(d.Label) &&   // empty label is 'other' difference
                                ((passPixels && string.Equals(d.Label, DiffClient.PIXEL, StringComparison.OrdinalIgnoreCase)) ||
                                 (passLabel && DiffClient.IsMatching(labels, d.Label))))
                    continue;
                else
                    return false;
            return true;
        }
    }
}
