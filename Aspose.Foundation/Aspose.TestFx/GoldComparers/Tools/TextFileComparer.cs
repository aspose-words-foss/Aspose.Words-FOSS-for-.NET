// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2016 by Vyacheslav Durin

using System;
using System.IO;
using Aspose.TestFx.GoldComparers.FailedTestCollector;

namespace Aspose.TestFx.GoldComparers
{
    /// <summary>
    /// Provides a method to compare two text files, launch difference viewers and accept new gold or skip changes.
    /// </summary>
    public class TextFileComparer : AbstractGoldComparer
    {
        public override void Execute(ComparerParams comparerParams)
        {
            TextFileComparer comparer = new TextFileComparer();
            comparer.ComparerParams = comparerParams;
            comparer.ExecuteCore(comparerParams.Title, 
                                comparerParams.FileNameOut, 
                                comparerParams.FileNameGold, 
                                comparerParams.FileNameMs,
                                comparerParams.FileNameSrc);
        }

        public override bool ExecuteForm(ComparerParams comparerParams)
        {
            return ExecuteForm(
                  comparerParams.Title,
                  comparerParams.FileNameOut,
                  comparerParams.FileNameGold,
                  comparerParams.FileNameOriginalGold,
                  comparerParams.FileNameMs,
                  comparerParams.FileNameSrc
                  );
        }

        /// <summary>
        /// Compares user text file with the gold file. Brings up the user interface when there is a difference.
        /// </summary>
        public static void Execute(
            string title,
            string fileNameOut,
            string fileNameGold,
            string fileNameMs,
            string fileNameSource)
        {
            TextFileComparer comparer = new TextFileComparer();
            comparer.ExecuteCore(title, fileNameOut, fileNameGold, fileNameMs, fileNameSource);
        }

        protected void ExecuteCore(
            string title,
            string fileNameOut,
            string fileNameGold,
            string fileNameMS,
            string fileNameSource)
        {
            // Sometime Java has it's own version of gold file. Comparer form needs in former .Net gold too.
            string fileNameOriginalGold = fileNameGold;
            fileNameGold = TestFxUtil.GetReplacementGoldIfExists(fileNameGold);
            try
            {
                DoCompareFiles(fileNameOut, fileNameGold);
                if (TestSettings.ShowCompareAlways)
                    throw new Exception("Showing results even if there is no difference");
            }
            catch (Exception e)
            {
                bool passed = e is GoldDifferenceException && TryPassAutomatically(fileNameOut, fileNameGold);

                // Throw immediately if images are different and silent mode testing is selected.
                if (!passed && !ExecuteForm(title, fileNameOut, fileNameGold, fileNameOriginalGold, fileNameMS, fileNameSource))
                {
                    FailedGoldTestCollector.SaveTxtGoldTestInfoIfNeeded(title, fileNameOut, fileNameGold, fileNameMS, fileNameSource);
                    throw;
                }
            }
        }

        private bool ExecuteForm(
            string title,
            string fileNameOut,
            string fileNameGold,
            string fileNameOriginalGold,
            string fileNameMS,
            string fileNameSource)
        {
            if (TestSettings.DontShowCompareForm)
                return false;

            bool topmost = (ComparerParams != null) ? ComparerParams.DialogTopMost : false;
            bool restorePosition = (ComparerParams != null) ? ComparerParams.DialogRestorePosition : false;
            // The compare did not go well, bring up the user interface.
            ComparerFormResult userChoice = AsposeComparer.ExecuteTextTestUI(title, fileNameOut, fileNameGold,
                  fileNameOriginalGold, fileNameSource, fileNameMS, 
                  topmost, restorePosition);

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
        /// This is a template method that derived text comparers need to override.
        /// </summary>
        protected virtual void DoCompareFiles(string fileName1, string fileName2)
        {
            CompareTextFiles(fileName1, fileName2);
        }

        /// <summary>
        /// Utility method to compare two text files. 
        /// Throws an exception with detailed error message if they don't match.
        /// </summary>
        internal static void CompareTextFiles(string fileName1, string fileName2)
        {
            if (!File.Exists(fileName1) || !File.Exists(fileName2))
                throw new GoldDifferenceException(fileName1, fileName2);

            using (Stream stream1 = File.OpenRead(fileName1))
            {
                using (Stream stream2 = File.OpenRead(fileName2))
                {
                    CompareTextFiles(stream1, stream2, fileName1, fileName2);
                }
            }
        }

        /// <summary>
        /// Utility method to compare two text files in streams. 
        /// Has additional parameters which helps to identify stream being compared.
        /// Throws an exception with detailed error message if they don't match.
        /// </summary>
        /// <param name='userStream'>User stream compared</param>
        /// <param name='goldStream'>Gold stream compared</param>
        /// <param name='userFileName'>String (typically user filename) which helps to identify stream compared</param>
        /// <param name='goldFileName'>String (typically gold filename) which helps to identify stream compared</param>
        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition(false)] // Manualy implemented using xdiff util.
        protected static void CompareTextFiles(Stream userStream, Stream goldStream, string userFileName, string goldFileName)
        {
#if JAVA
            // On Java we compare XML using XmlDiff so indentation etc will pass on Java.
            // If the XML size is greater than 30 MB, testing will take about 2 hours.
            if (ComparisonInfo.isXml(userFileName) && userStream.getLength() < 30000000L)
            {
                if (XmlComparer.compareXmlStreams(userStream, goldStream))
                    return;
                else
                    throw new GoldDifferenceException(userFileName, goldFileName);
            }
#endif
            // This is the fast compare that we should normally use.
            if (!TestFxUtil.CompareStreams(userStream, goldStream))
                throw new GoldDifferenceException(userFileName, goldFileName);
        }

        internal ComparerParams ComparerParams { get; set; }
    }
}
