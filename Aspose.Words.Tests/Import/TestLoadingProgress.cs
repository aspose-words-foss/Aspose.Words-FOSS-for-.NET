// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/09/2021 by Dmitry Sokolov

using System;
using System.Collections.Generic;
using Aspose.Common;
using Aspose.Words.Loading;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Docx
{
    /// <summary>
    /// Tests which checks how client may be notified about document loading progress.
    /// </summary>
    [TestFixture]
    public class TestLoadingProgress
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        [SetUp]
        public void Init()
        {
            RandomUtil.Reset();
        }


        // FOSS Test14472NoProgress removed: it loads an MHTML file to check the "progress unsupported"
        // warning, but MHTML loading is removed (throws) in the FOSS build.



        /// <summary>
        /// Related with WORDSNET-14472
        /// Checks that content was read and may be saved when progress callback is set.
        /// </summary>
        [Test]
        public void Test14472Simple()
        {
            TestWarningCallback wc = new TestWarningCallback();

            TestLoadingProgressCallback testCallback = LoadAndCheckProgress(
                @"Model\Progress\Test14472Simple.fopc",
                wc,
                null,
                true,
                LoadFormat.Auto);

            Assert.That(wc.Contains(
                WarningUtil.ToLoadWarningSource(LoadFormat.FlatOpc),
                WarningType.Hint,
                string.Format(WarningStrings.LoadingProgressUnsupported, LoadFormat.FlatOpc)), Is.False);

            List<DocumentLoadingArgs> args = testCallback.AcceptedArguments;
            Assert.That(args.Count, Is.GreaterThan(0));
            Assert.That(args[args.Count - 1].EstimatedProgress, Is.EqualTo(100.0d).Within(DoubleDelta));
        }


        /// <summary>
        /// Related with WORDSNET-14472
        /// Checks how loading errors process.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FileCorruptedException))]
        public void Test14472LoadError()
        {
            string path = @"ImportDocx\Package\TestJira13791.docx";
            LoadAndCheckProgress(path);
        }

        /// <summary>
        /// Related with WORDSNET-14472
        /// Checks the case when source document has zero paragraphs.
        /// </summary>
        [Test]
        [NonParallelizable]
        public void Test14472NoPara()
        {
            string path = @"ExportRtf\Test19533.docx";
            TestLoadingProgressCallback testCallback = LoadAndCheckProgress(path);

            List<DocumentLoadingArgs> args = testCallback.AcceptedArguments;

            Assert.That(args.Count, Is.GreaterThan(0));
            Assert.That(args[args.Count - 1].EstimatedProgress, Is.EqualTo(100.0d).Within(DoubleDelta));
        }

        /// <summary>
        /// Related with WORDSNET-14472
        /// Checks the case when post loading stage progress may complete with value less than 100%.
        /// I.e. we should notify about 100% progress at the end of post loading stage.
        /// </summary>
        [Test]
        [NonParallelizable]
        public void Test14472PostLoadingProgressComplete()
        {
            string path = @"Model\Markup\Test18729.docx";
            TestLoadingProgressCallback testCallback = LoadAndCheckProgress(path);

            List<DocumentLoadingArgs> args = testCallback.AcceptedArguments;

            Assert.That(args.Count, Is.GreaterThan(0));
            Assert.That(args[args.Count - 1].EstimatedProgress, Is.EqualTo(100.0d).Within(DoubleDelta));
        }

        // FOSS TestStageBorders removed: its only cases load WML and ODT, both removed load formats.

        /// <summary>
        /// WORDSNET-23836 Progress does not work while loading blank file.
        /// AW loads the blank document from resources when input stream is empty.
        /// Use client callback for loading the blank to fix the issue.
        /// </summary>
        [Test]
        public void Test23836()
        {
            TestLoadingProgressCallback testCallback = LoadAndCheckProgress(@"Model\Progress\Test23836.docx");

            List<DocumentLoadingArgs> args = testCallback.AcceptedArguments;
            Assert.That(args.Count, Is.EqualTo(2));

            Assert.That(args[0].EstimatedProgress, Is.EqualTo(70.0d).Within(DoubleDelta));
            Assert.That(args[1].EstimatedProgress, Is.EqualTo(100.0d).Within(DoubleDelta));
        }

        public static int GetMaxArgsCount(int precision)
        {
            // Not more than "100 * 10^ProgressUtils.Precision" items must be in the arguments collection.
            // I.e. for two digits it will be 100*100 = 10000.
            const int decimalBase = 10;
            int maxArgsCount = (int)System.Math.Round(System.Math.Pow(decimalBase, precision));

            // Take into account digits before point.
            maxArgsCount *= 100;

            return maxArgsCount;
        }

        private TestLoadingProgressCallback LoadAndCheckProgress(string path)
        {
            return LoadAndCheckProgress(path, null, new TestLoadingProgressCallback(), false, LoadFormat.Auto);
        }

        private TestLoadingProgressCallback LoadAndCheckProgress(string path, TestWarningCallback warningCallback)
        {
            return LoadAndCheckProgress(path, warningCallback, new TestLoadingProgressCallback(), false, LoadFormat.Auto);
        }

        private TestLoadingProgressCallback LoadAndCheckProgress(
            string path,
            TestWarningCallback warningCallback,
            TestLoadingProgressCallback testCallback,
            bool verifyExport,
            LoadFormat loadFormat)
        {
            testCallback = (testCallback == null) ? new TestLoadingProgressCallback() : testCallback;

            LoadOptions lo = new LoadOptions();
            lo.ProgressCallback = testCallback;
            lo.LoadFormat = loadFormat;

            if (warningCallback != null)
                lo.WarningCallback = warningCallback;

            Document doc = null;
            try
            {
                doc = TestUtil.Open(path, lo);
            }
            finally
            {
                CheckProgress(testCallback);
            }

            if (verifyExport)
                TestUtil.SaveOpen(doc, path, UnifiedScenario.Docx2Docx);

            return testCallback;
        }

        private void CheckProgress(TestLoadingProgressCallback testCallback)
        {
            // 1. Not more than "100 * 10^ProgressUtils.Precision" items must be in the arguments collection.
            int maxArgsCount = GetMaxArgsCount(LoadingProgressCalculator.Precision);
            Assert.That(maxArgsCount, Is.GreaterThanOrEqualTo(testCallback.AcceptedArguments.Count));

            DocumentLoadingArgs prevArgs = null;
            foreach (DocumentLoadingArgs args in testCallback.AcceptedArguments)
            {
                double progress = args.EstimatedProgress;

                // 2. Check progress precision.
                Assert.That(System.Math.Round(progress, LoadingProgressCalculator.Precision), Is.EqualTo(progress));

                // 3. Next progress value must be greater than previous value.
                // DS: AW does two attempts of loading when input format specified explicitly.
                // So, generally it is possible to obtain situation when a next progress value will be less than previous.
                // However, i do not see a way to reproduce it in the scope of DOCX format.
                if (prevArgs != null)
                    Assert.That(progress, Is.GreaterThan(prevArgs.EstimatedProgress));

                prevArgs = args;
            }
        }

        private static string LoadFormatToExtension(LoadFormat lf)
        {
            string extension;
            switch (lf)
            {
                case LoadFormat.WordML:
                    extension = "wml";
                    break;
                case LoadFormat.Mhtml:
                    extension = "mht";
                    break;
                case LoadFormat.Text:
                    extension = "txt";
                    break;
                default:
                    extension = lf.ToString();
                    break;
            }

            return extension;
        }

        private const double DoubleDelta = 0.000001d;
    }
}
