// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/12/2016 by Vyacheslav Durin

using System;
using System.IO;
using Aspose.IO;
using Aspose.JavaAttributes;
using Aspose.TestFx.GoldComparers.FailedTestCollector;

namespace Aspose.TestFx.GoldComparers
{
    /// <summary>
    /// Compares two picture files and invokes a user interface to view 
    /// the difference and to override the gold file with the new output.
    /// </summary>
    public class ImageFileComparer : AbstractGoldComparer
    {
        public override void Execute(ComparerParams comparerParams)
        {
            Execute(comparerParams.FileNameOut, comparerParams.FileNameGold, comparerParams.FileNameSrc);
        }

        public override bool ExecuteForm(ComparerParams comparerParams)
        {
            return ExecuteForm(
                comparerParams.FileNameOut,
                comparerParams.FileNameGold,
                comparerParams.FileNameOriginalGold,
                comparerParams.FileNameSrc,
                comparerParams.ErrorMessage
                );
        }

        /// <summary>
        /// Compares two image files and brings up a GUI if they do not match.
        /// </summary>
        /// <param name="fileNameOut">The out file.</param>
        /// <param name="fileNameGold">The gold file.</param>
        /// <param name="fileNameSrc">The original file. Can be null or empty string.</param>
        public static void Execute(string fileNameOut, string fileNameGold, string fileNameSrc)
        {
            // The Java version needs to compare with a file in the TestGoldJava folder if it exists.
            // Let's show original .Net gold too in that case.
            string fileNameOriginalGold = fileNameGold;
            fileNameGold = TestFxUtil.GetReplacementGoldIfExists(fileNameGold);

            try
            {
                CompareImageFiles(fileNameOut, fileNameGold);
                if (TestSettings.ShowCompareAlways)
                    throw new Exception("Showing results even if there is no difference");
            }
            catch (Exception e)
            {
                bool passed = e is GoldDifferenceException && TryPassAutomatically(fileNameOut, fileNameGold);

                // Throw immediately if images are different and silent mode testing is selected.
                if (!passed && !ExecuteForm(fileNameOut, fileNameGold, fileNameOriginalGold, fileNameSrc, e.Message))
                {
#if !CPLUSPLUS
                    FailedGoldTestCollector.SaveImgGoldTestInfoIfNeeded(fileNameSrc, fileNameOut, fileNameGold);
#endif
                    throw;
                }
            }
        }

        private static bool ExecuteForm(string fileNameOut, string fileNameGold, string fileNameOriginalGold, string fileNameSrc, string errorMessage)
        {
            if (TestSettings.DontShowCompareForm)
                return false;

            ComparerFormResult userChoice = AsposeComparer.ExecuteImageTestUI(fileNameOut, fileNameGold, fileNameOriginalGold, fileNameSrc, errorMessage);

            // If the user clicks Accept we override the gold file, otherwise let the difference exception propagate to the caller.
            switch (userChoice)
            {
                case ComparerFormResult.Accept:
                    TestFxUtil.AcceptNewGold(fileNameOut, fileNameGold);
                    return true;
                case ComparerFormResult.SkipAndThrow:
                    return false;
                case ComparerFormResult.SkipAndLog:
                    // MF "Skip This" was used. Don't throw, just ignore that images are not equal.
                    // RK I think MF did this to allow skipping pages in a multipage test.
                    Console.WriteLine(string.Format("Image files are different (throw skipped): \nOut: {0}\nGold: {1}\n", fileNameOut, fileNameGold));
                    return true;
                default:
                    throw new InvalidOperationException("Unexpected result from the comparer form.");
            }
        }

        /// <summary>
        /// A utility method to compares two image files.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition(false)]
        public static void CompareImageFiles(string fileName1, string fileName2)
        {
            if (!File.Exists(fileName1) || !File.Exists(fileName2))
                throw new GoldDifferenceException(fileName1, fileName2);

            using (Stream fs1 = File.OpenRead(fileName1))
            {
                using (Stream fs2 = File.OpenRead(fileName2))
                {
                    CompareImageFiles(fs1, fs2, fileName1, fileName2);
                }
            }
        }

        /// <summary>
        /// A utility method to compares two image files. 
        /// Stream-oriented override with additional argument which helps to identify image name and 
        /// controlled pixel difference count and color tolerance.
        /// </summary>
        /// <remarks>Throw if images are different more than allowed.</remarks>
        /// <param name="fileStream1">First stream being compared.</param>
        /// <param name="fileStream2">Second stream being compared.</param>
        /// <param name="fileName1">String (typically user file name) which helps to identify stream being compared.</param>
        /// <param name="fileName2">String (typically gold file name) which helps to identify stream being compared.</param>
        public static void CompareImageFiles(Stream fileStream1, Stream fileStream2, string fileName1, string fileName2)
        {
            if (CompareFileBytes(fileStream1, fileStream2, fileName1, fileName2))
                return;
            else
                throw new GoldDifferenceException(fileName1, fileName2);
        }

        /// <summary>
        /// Compares two files by lengths and bytes. Returns true if they are the same.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition(false)] // Manually implemented using BitmapComparer util.
        private static bool CompareFileBytes(Stream fileStream1, Stream fileStream2, string fileName1, string fileName2)
        {
            if (fileStream1.Length != fileStream2.Length)
                return false;

            byte[] buf1 = StreamUtil.CopyStreamToByteArray(fileStream1);
            byte[] buf2 = StreamUtil.CopyStreamToByteArray(fileStream2);

            return ArrayUtil.IsArrayEqual(buf1, buf2);
        }

        public class ImageDifference
        {
            [Flags]
            public enum Kind
            {
                // Image files are binary equal
                None,

                // Image files are not binary equal
                BinaryDifferent = 1 << 0,

                // Image pixels are different
                PixelsDifferent = BinaryDifferent | (1 << 1),

                // Image difference has a label
                Labels = BinaryDifferent | (1 << 2),

                // This is the value to return if difference computation failed completely
                Invalid = 0xff
            }

            public Kind Difference { get ;}

            public string Label { get; }

            private ImageDifference(Kind difference)
            {
                Difference = difference;
            }

            public ImageDifference(string label) : this(Kind.Labels)
            {
                Label = label;
            }

            public static readonly ImageDifference None = new ImageDifference(Kind.None);

            public static readonly ImageDifference BinaryDifferent = new ImageDifference(Kind.BinaryDifferent);

            public static readonly ImageDifference PixelsDifferent = new ImageDifference(Kind.PixelsDifferent);
        }

        /// <summary>
        /// Converts documents to images and compares images. Returns True if images are equal based on the current test settings.
        /// </summary>
        [AndroidDelete]
        public static bool CompareDocumentsAsImages(string fileOut, string fileGold)
        {
            string x1 = Path.GetExtension(fileOut).ToLowerInvariant();
            Debug.Assert(x1 == Path.GetExtension(fileGold).ToLowerInvariant());

            // These are already images. We came here because image files are different.
            // What we can do is compare pixels and labels.
            if (x1 == ".png" || x1 == ".gif" || x1 == ".jpg" || x1 == ".jpeg" || x1 == ".tiff")
            {
                // We know that files are binary different, let's compare pixels.
                if (TestSettings.PassWhenImagesPixelsMatch)
                {
                    using (BasePathUtil.DeletePathOnDispose tmp = BasePathUtil.UseTempFolderPath())
                    {
                        string tempFolder = tmp.GetTmpPath();
                        // here we have compared images as binary streams already, no sense asking diff to do that
                        DiffClient.Result[] result = DiffClient.Generate(tempFolder, fileOut, fileGold, null, TestSettings.PassWhenImagesDifferenceLabelsMatch, false);

                        // we compared two files and expect one difference label
                        if (result.Length != 1)
                            return false;
                        var label = result[0].Label;

                        // since we know that files are binary different the empty value means significant difference
                        if (!string.IsNullOrEmpty(label))
                        {
                            // matching pixels
                            if (string.Equals(label, DiffClient.PIXEL, StringComparison.OrdinalIgnoreCase))
                                return true;

                            if (TestSettings.PassWhenImagesDifferenceLabelsMatch)
                            {
                                string[] labels = TestSettings.Labels;

                                // matching labels
                                int index = 0;
                                foreach (string l in labels)
                                    if (string.Equals(l, label, StringComparison.OrdinalIgnoreCase))
                                        break;
                                    else
                                        index++;

                                if (labels.Length > 0 && index >= 0 && index < labels.Length)
                                        return true;
                            }
                        }

                        // When learning automatically we can have a label which will be assigned to any significant difference
                        // the test will pass and the difference image will be copied to the dedicated folder
                        if (TestSettings.PassExplicitLabel)
                        {
                            DiffClient.RelocateDiffFile(DiffClient.GetDiffImagePath(tempFolder, fileOut, fileGold), TestSettings.Labels[0]);
                            return true;
                        }
                    }
                }

                return false;
            }

            if (x1 == ".xps")
            {
                PackageComparisonInfo info = new ZipComparisonInfo(null, fileOut, fileGold, null);
                info.UpdateStatus();
                // The false is returned where either comparisson failed completely or files were not equal
                // The true is returned where all image files were equal based on test settings we have
                // If not true then assume images were different, this is sufficient for now (do not bother with comparisson errors)
                return PackageFileComparer.CheckXpsMarkupAndPageImagesEqual(info);
            }

            if (x1 == ".svg" || x1 == ".html" || x1 == ".htm" || x1 == ".mht")
            {
#if JAVA
                // WebToPngClient.Convert didn't work in java, and we desided to manualy check html golds
                return false;
#else
                using (BasePathUtil.DeletePathOnDispose tmp = BasePathUtil.UseTempFolderPath())
                {
                    string tempFolder = tmp.GetTmpPath();
                    WebToPngClient.Convert(tempFolder, new String[] { fileOut, fileGold });

                    string fileOutPng = Path.Combine(tempFolder, WebToPngClient.GetOutputFileName(fileOut));
                    string fileGoldPng = Path.Combine(tempFolder, WebToPngClient.GetOutputFileName(fileGold));

                    bool areFilesEqual;
                    using (Stream streamOut = new FileStream(fileOutPng, FileMode.Open))
                    using (Stream streamGold = new FileStream(fileGoldPng, FileMode.Open))
                        areFilesEqual = CompareFileBytes(streamOut, streamGold, fileOutPng, fileGoldPng);

                    // TODO Use ai matcher to compute image difference
                    return areFilesEqual;
                }
#endif
            }

            return false;
        }
    }
}
