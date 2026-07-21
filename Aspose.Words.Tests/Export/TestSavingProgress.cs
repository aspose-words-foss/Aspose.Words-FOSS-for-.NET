// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 09/12/2021 by Dmitry Sokolov

using System;
using System.Collections.Generic;
using System.IO;
using Aspose.Common;
using Aspose.Words.Saving;
using Aspose.Words.Tests.Import.Docx;
using NUnit.Framework;

namespace Aspose.Words.Tests.Export
{
    /// <summary>
    /// Tests which checks how client may be notified about document writing progress.
    /// </summary>
    [TestFixture]
    public class TestSavingProgress
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




        /// <summary>
        /// Related with WORDSNET-11575
        /// Checks the case when source document has zero paragraphs.
        /// </summary>
        [Test]
        [NonParallelizable]
        public void Test11575NoPara()
        {
            const string path = @"ExportRtf\Test19533.docx";
            TestSavingProgressCallback callback = OpenSaveAndCheckProgress(path, SaveFormat.FlatOpc);

            List<DocumentSavingArgs> args = callback.AcceptedArguments;

            Assert.That(args.Count, Is.GreaterThan(0));
            Assert.That(args[args.Count - 1].EstimatedProgress, Is.EqualTo(100.0d).Within(DoubleDelta));
        }

        private static TestSavingProgressCallback OpenSaveAndCheckProgress(string srcPath, SaveFormat saveFormat)
        {
            return OpenSaveAndCheckProgress(srcPath, saveFormat, null, null, true);
        }

        private static void OpenSaveAndCheckProgress(string srcPath,
            SaveFormat saveFormat, TestSavingProgressCallback callback)
        {
            OpenSaveAndCheckProgress(srcPath, saveFormat, callback, null, true);
        }

        private static TestSavingProgressCallback OpenSaveAndCheckProgress(
            string srcPath,
            SaveFormat saveFormat,
            TestSavingProgressCallback callback,
            TestWarningCallback warningCallback,
            bool reopenAfterSave)
        {
            Document doc = TestUtil.Open(srcPath);
            doc.WarningCallback = warningCallback;

            callback = (callback == null) ? new TestSavingProgressCallback() : callback;
            SaveOptions so = SaveOptions.CreateSaveOptions(saveFormat);
            so.ProgressCallback = callback;

            string savePath = @"Model\Progress\" + Path.GetFileNameWithoutExtension(srcPath) + FileFormatUtil.SaveFormatToExtension(saveFormat);

            try
            {
                if (reopenAfterSave)
                    TestUtil.SaveOpen(doc, savePath, so, false);
                else
                    TestUtil.Save(doc, savePath, so, false);
            }
            finally
            {
                CheckProgress(callback);
            }

            return callback;
        }

        /// <summary>
        /// Checks progress notifications content.
        /// </summary>
        private static void CheckProgress(TestSavingProgressCallback callback)
        {
            // 1. Not more than "100 * 10^ProgressUtils.Precision" items must be in the arguments collection.
            int maxArgsCount = TestLoadingProgress.GetMaxArgsCount(SavingProgressCalculator.Precision);
            Assert.That(maxArgsCount, Is.GreaterThanOrEqualTo(callback.AcceptedArguments.Count));

            DocumentSavingArgs prevArgs = null;
            foreach (DocumentSavingArgs args in callback.AcceptedArguments)
            {
                double progress = args.EstimatedProgress;

                // 2. Check progress precision.
                Assert.That(System.Math.Round(progress, SavingProgressCalculator.Precision), Is.EqualTo(progress));

                // 3. Next progress value must be greater than previous value.
                if (prevArgs != null)
                    Assert.That(progress, Is.GreaterThan(prevArgs.EstimatedProgress));

                prevArgs = args;
            }
        }

        private const double DoubleDelta = 0.000001d;
    }
}
