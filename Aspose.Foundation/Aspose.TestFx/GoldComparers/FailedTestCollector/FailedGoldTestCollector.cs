// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/12/2012 by Alexey Butalov

using System.IO;
using System.Reflection;
using Aspose.JavaAttributes;

namespace Aspose.TestFx.GoldComparers.FailedTestCollector
{
    /// <summary>
    /// Utility methods for postprocessing of failed gold tests.
    /// </summary>
    /// <remarks>
    /// We need the gold tests postprocessing feature for CI (Jenkins). Please see WORDSNET-7572
    /// It's a part of the mechanism which allows CI users to see CI gold test problems.
    /// </remarks>
    internal static class FailedGoldTestCollector
    {
        /// <summary>
        /// Saves information about failed gold tests with zip test files to a specified folder.
        /// </summary>
        [JavaThrows(true)]
        internal static void SaveZipGoldTestInfoIfNeeded(
            string title,
            string fileNameSrc,
            string fileNameOut,
            string fileNameGold,
            string fileNameMS)
        {
            FailedGoldTestInfo testInfo = new FailedGoldTestInfo();
            testInfo.GoldFileType = GoldTestFileType.Zip;
            testInfo.Title = title;
            testInfo.FileNameSrc = fileNameSrc;
            testInfo.FileNameOut = fileNameOut;
            testInfo.FileNameGold = fileNameGold;
            testInfo.FileNameMS = fileNameMS;

            SaveGoldTestInfoIfNeeded(testInfo);
        }

        /// <summary>
        /// Saves information about failed gold tests with MOBI test files to a specified folder.
        /// </summary>
        [JavaThrows(true)]
        internal static void SaveMobiGoldTestInfoIfNeeded(
            string title,
            string fileNameSrc,
            string fileNameOut,
            string fileNameGold,
            string fileNameMS)
        {
            FailedGoldTestInfo testInfo = new FailedGoldTestInfo();
            testInfo.GoldFileType = GoldTestFileType.Mobi;
            testInfo.Title = title;
            testInfo.FileNameSrc = fileNameSrc;
            testInfo.FileNameOut = fileNameOut;
            testInfo.FileNameGold = fileNameGold;
            testInfo.FileNameMS = fileNameMS;

            SaveGoldTestInfoIfNeeded(testInfo);
        }

        /// <summary>
        /// Saves information about failed gold tests with AZW3 test files to a specified folder.
        /// </summary>
        [JavaThrows(true)]
        internal static void SaveAzw3GoldTestInfoIfNeeded(
            string title,
            string fileNameSrc,
            string fileNameOut,
            string fileNameGold,
            string fileNameMS)
        {
            FailedGoldTestInfo testInfo = new FailedGoldTestInfo();
            testInfo.GoldFileType = GoldTestFileType.Azw3;
            testInfo.Title = title;
            testInfo.FileNameSrc = fileNameSrc;
            testInfo.FileNameOut = fileNameOut;
            testInfo.FileNameGold = fileNameGold;
            testInfo.FileNameMS = fileNameMS;

            SaveGoldTestInfoIfNeeded(testInfo);
        }

        /// <summary>
        /// Saves information about failed gold tests with text test files to a specified folder.
        /// </summary>
        [JavaThrows(true)]
        internal static void SaveTxtGoldTestInfoIfNeeded(
            string title,
            string fileNameOut,
            string fileNameGold,
            string fileNameMS,
            string fileNameSource)
        {
            FailedGoldTestInfo testInfo = new FailedGoldTestInfo();
            testInfo.GoldFileType = GoldTestFileType.Text;
            testInfo.Title = title;
            testInfo.FileNameOut = fileNameOut;
            testInfo.FileNameGold = fileNameGold;
            testInfo.FileNameMS = fileNameMS;
            testInfo.FileNameSrc = fileNameSource;

            SaveGoldTestInfoIfNeeded(testInfo);
        }

        /// <summary>
        /// Saves information about failed gold tests with image test files to a specified folder.
        /// </summary>
        [JavaThrows(true)]
        internal static void SaveImgGoldTestInfoIfNeeded(
            string fileNameSrc,
            string fileNameOut,
            string fileNameGold)
        {
            FailedGoldTestInfo testInfo = new FailedGoldTestInfo();
            testInfo.GoldFileType = GoldTestFileType.Image;
            testInfo.FileNameSrc = fileNameSrc;
            testInfo.FileNameOut = fileNameOut;
            testInfo.FileNameGold = fileNameGold;

            SaveGoldTestInfoIfNeeded(testInfo);
        }

        /// <summary>
        /// Saves information about failed gold tests to a specified folder.
        /// </summary>
        [JavaThrows(true)]
        private static void SaveGoldTestInfoIfNeeded(FailedGoldTestInfo testInfo)
        {
#if JAVA
            FailedGoldTestStorageEngine engine = new FailedGoldTestStorageEngine();
            engine.saveFailedTest(testInfo);
#else
            string problemTestsDir = FailedGoldTestUtil.GetProblemTestBaseDir();
            if (!StringUtil.HasChars(problemTestsDir))
                return;

            MethodBase testMethod = FailedGoldTestUtil.GetTestMethod();
            if (testMethod == null)
            {
                Debug.Assert(false);
                return;
            }

#if CPLUSPLUS
            problemTestsDir = Path.Combine(problemTestsDir, FailedGoldTestUtil.GetTestModuleName());
#else
            problemTestsDir = Path.Combine(problemTestsDir, testMethod.Module.Assembly.GetName().Name);
#endif
            FailedGoldTestStorageEngine engine = new FailedGoldTestStorageEngine(problemTestsDir);
            engine.SaveFailedTest(testInfo);
#endif
        }
    }
}
