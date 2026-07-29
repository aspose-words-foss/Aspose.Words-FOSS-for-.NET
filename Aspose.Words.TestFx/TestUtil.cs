// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using Aspose.Common;
using Aspose.Crypto;
using Aspose.IO;
using Aspose.JavaAttributes;
using Aspose.OpcPackaging;
using Aspose.Images.Pal;
using Aspose.Ss;
using Aspose.TestFx;
using Aspose.TestFx.Pal;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Fields;
using Aspose.Words.RW.Docx.Writer;
using Aspose.Words.RW.Ole;
using Aspose.Words.Saving;
using Aspose.Words.Tests.Fonts;
using Aspose.Words.Validation;
using CodePorting.Translator.Cs2Cpp;
using NUnit.Framework;
using Aspose.Words.Fonts;
using Aspose.Words.Loading;
using System.Threading;
#if !NETSTANDARD
using System.Windows.Forms;
#endif

namespace Aspose.Words.Tests
{
    /// <summary>
    /// Utility functions used by unit tests.
    /// </summary>
    public static class TestUtil
    {
        /// <summary>
        /// Performs common setup of test environment.
        /// </summary>
        public static void SetUpTests()
        {
            Debug.SetTestDefaults();
            RandomUtil.SetTestMode();
            DateTimeUtil.SetTestMode();
            OleUtil.SetTestMode();
            // FOSS
            SystemPal.SetTestMode(true);
            SectPr.TestMode = true;
            // FOSS
            OpcDocumentFragmentWriter.SetTestMode();

            LanguagePreferences.TestMode = true;

#if !JAVA && !CPLUSPLUS
            // This redirects all web requests to local resources
            // It helps when resources on the net disappear or if there is no connectivity.
            TestSettings.EnableRequestRedirect();
#endif
        }

        /// <summary>
        /// Opens a test file relative to the TestData folder or from an explicit path.
        /// </summary>
        /// <param name="fileName">Can be relative to the TestData folder or an explicit path.</param>
        public static Document Open(string fileName)
        {
            fileName = BuildTestFileName(fileName);

#if !JAVA && !NETSTANDARD && !CPLUSPLUS   // Exclude some MF-specific MS Word automation that we don't need on Java.
            return OpenDocRetry(fileName);
#else
            return new Document(fileName);
#endif
        }

#if !JAVA  && !NETSTANDARD  && !CPLUSPLUS // Some MF-specific MS Word automation that we don't need on Java.
        /// <summary>
        /// This method opens document. If document cannot be opened and we are debugging
        /// on MFDELL it will attempt to close the MS Word which locks the document and retry.
        /// </summary>
        private static Document OpenDocRetry(string fileName)
        {
            // Close Word window.
            for (int i = 0; ; i++)
            {
                try
                {
                    return new Document(fileName);
                }
                catch
                {
                    if (!Debugger.IsAttached || !TestSettings.Is("closedoc"))
                        throw;

                    Process proc = FindWordProcess(fileName);
                    if (proc == null)
                        throw;

                    if (i <= 2 && proc.CloseMainWindow())
                    {
                        // Give some time to exit.
                        proc.WaitForExit(1000);
                        continue;
                    }

                    if (MessageBox.Show("The document is opened in Word, please close it first",
                        "Cannot load test document",
                            MessageBoxButtons.RetryCancel, MessageBoxIcon.Error,
                                MessageBoxDefaultButton.Button1) == DialogResult.Cancel)
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Returns Word process which has specified document open.
        /// </summary>
        private static Process FindWordProcess(string fileName)
        {
            string title = Path.GetFileName(fileName);

            // Find and close Word process with the document opened.
            Process[] procs = Process.GetProcessesByName("WINWORD");
            foreach (Process proc in procs)
            {
                if (string.Compare(proc.MainWindowTitle, 0, title, 0, title.Length, true) == 0 &&
                    proc.MainWindowTitle.EndsWith(" - Microsoft Word"))
                {
                    return proc;
                }
            }
            return null;
        }
#endif

        /// <summary>
        /// Saves the document relative to the TestData folder or in explicit path.
        /// NO gold check is performed.
        /// </summary>
        /// <param name="doc">The document to save.</param>
        /// <param name="fileName">Can be relative to the TestData folder or an explicit path.
        /// Should not have the "Out" suffix as it is added automatically.</param>
        /// <returns>The full name of the saved file.</returns>
        public static string Save(Document doc, string fileName)
        {
            return Save(doc, fileName, null, false);
        }

        /// <summary>
        /// Saves the document relative to the TestData folder or in explicit path.
        /// Performs a gold compare test with export and import gold comparison level.
        /// </summary>
        public static string SaveCheckGold(Document doc, string fileName)
        {
            return Save(doc, fileName, null, true, GoldLevel.ExportImport);
        }

        /// <summary>
        /// Saves the document relative to the TestData folder or in explicit path.
        /// Performs a gold compare test with export gold comparison level.
        /// </summary>
        public static string SaveCheckGoldExportOnly(Document doc, string fileName)
        {
            return Save(doc, fileName, null, true, GoldLevel.ExportOnly);
        }

        public static string SaveCheckGold(Document doc, string fileName, SaveOptions saveOptions)
        {
            return Save(doc, fileName, saveOptions, true);
        }

        /// <summary>
        /// Saves the document relative to the TestData folder or in explicit path.
        /// Optionally performs a gold compare test with specified gold comparison level.
        /// </summary>
        public static string Save(Document doc, string fileName, SaveOptions saveOptions, bool isCheckGold, GoldLevel goldLevel)
        {
            // This is a "fake" src file name, e.g. it does not have to exist. It is used to build the out and gold file names.
            string srcFileName = BuildTestFileName(fileName);
            string outFileName = BuildOutFileName(srcFileName, "", SaveFormat.Unknown);
            EnsureDirectoryForFileExists(outFileName);

            if (saveOptions == null)
            {
                saveOptions = SaveOptions.CreateSaveOptions(outFileName);
                saveOptions.SetTestMode();
            }
            else if (!saveOptions.IsTestMode)
            {
                saveOptions.SetTestMode();
            }

            doc.Save(outFileName, saveOptions);

            if (isCheckGold)
            {
                string goldFileName = BuildGoldFileName(srcFileName, "", SaveFormat.Unknown);
                // In this method the "ms" file is usually the original file.
                string msFileName = srcFileName;
                VerifyGold(srcFileName, outFileName, goldFileName, msFileName, saveOptions, goldLevel == GoldLevel.ExportOnly);
            }

            return outFileName;
        }

        /// <summary>
        /// Saves the document relative to the TestData folder or in explicit path.
        /// Optionally performs a gold compare test.
        /// </summary>
        public static string Save(Document doc, string fileName, SaveOptions saveOptions, bool isCheckGold)
        {
            return Save(doc, fileName, saveOptions, isCheckGold, GoldLevel.ExportImport);
        }

        /// <summary>
        /// 27/07/2012 DV THIS IS A TEMPORARY METHOD. While working on 6554, I noticed that many tests in TestIndexAndTables
        /// started failing permanently. Accepting did not work. This was because of empty w:wPr tag written to the TOC1 style
        /// in DOCX. I notified DD and he promised to resolve it, but since I have to integrate dvorob_6554 to the trunk,
        /// I'm forced to temporarily use the ExportOnly mode.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static Document SaveOpenDocxExportOnly(Document doc, string fileName)
        {
            // Clone. This makes sure cloning code is exercised.
            doc = doc.Clone();

            // This is a "fake" src file name, e.g. it does not have to exist. It is used to build the out and gold file names.
            string srcFileName = BuildTestFileName(fileName);
            string outFileName = BuildOutFileName(srcFileName, "", SaveFormat.Unknown);
            EnsureDirectoryForFileExists(outFileName);

            SaveOptions saveOptions = SaveOptions.CreateSaveOptions(outFileName);
            saveOptions.SetTestMode();

            doc.Save(outFileName, saveOptions);

            string goldFileName = BuildGoldFileName(srcFileName, "", SaveFormat.Unknown);
            // In this method the "ms" file is usually the original file.
            string msFileName = srcFileName;
            VerifyGoldDocx(srcFileName, outFileName, goldFileName, msFileName, GoldLevel.ExportOnly, saveOptions);

            // Open again.
            return new Document(outFileName);
        }

        /// <summary>
        /// Saves and then opens a document relative to the TestData folder or in explicit path.
        /// Performs a gold compare test.
        /// </summary>
        /// <param name="doc">The document to save.</param>
        /// <param name="fileName">Can be relative to the TestData folder or an explicit path.
        /// Should not have the "Out" suffix as it is added automatically.</param>
        public static Document SaveOpen(Document doc, string fileName)
        {
            return SaveOpen(doc, fileName, null);
        }

        /// <summary>
        /// Saves and then opens a document relative to the TestData folder or in explicit path.
        /// Performs a gold compare test.
        /// </summary>
        public static Document SaveOpen(Document doc, string fileName, SaveOptions saveOptions)
        {
            return SaveOpen(doc, fileName, saveOptions, true);
        }

        /// <summary>
        /// Saves and then opens a document relative to the TestData folder or in explicit path.
        /// Performs a gold compare test if isCheckGold set as true.
        /// </summary>
        public static Document SaveOpen(Document doc, string fileName, SaveOptions saveOptions, bool isCheckGold)
        {
            // Clone. This makes sure cloning code is exercised.
            doc = doc.Clone();

            // Save and verify gold.
            string outFileName = Save(doc, fileName, saveOptions, isCheckGold);

            return new Document(outFileName);
        }

        /// <summary>
        /// Saves and then opens a document in memory. DOES NOT perform a gold compare test.
        /// </summary>
        public static Document SaveOpen(Document document, SaveFormat sf)
        {
            using (Stream stream = new MemoryStream())
            {
                document.Save(stream, sf);
                return new Document(stream);
            }
        }

        /// <summary>
        /// Saves a document to a memory stream and returns it.
        /// </summary>
        public static MemoryStream Save(Document document, SaveFormat sf)
        {
            MemoryStream stream = new MemoryStream();
            document.Save(stream, sf);
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Opens, saves and then opens the file again. File is relative to the TestData folder or in explicit path.
        /// This allows to check import and "limited" export so that we can at least read the files we produce.
        /// </summary>
        /// <param name="fileName">Can be relative to the TestData folder or an explicit path.</param>
        public static Document OpenSaveOpen(string fileName)
        {
            Document doc = Open(fileName);
            return SaveOpen(doc, fileName);
        }

        /// <summary>
        /// Works exactly like Open but also clears then updates field results throughout the document.
        /// Used by the field evaluation engine tests.
        /// </summary>
        /// <param name="fileName">Can be relative to the TestData folder or an explicit path.</param>
        public static Document OpenUpdateFields(string fileName)
        {
            return OpenUpdateFields(fileName, null);
        }

        /// <summary>
        /// Works exactly like Open but also clears then updates field results throughout the document.
        /// Used by the field evaluation engine tests.
        /// </summary>
        /// <param name="fileName">Can be relative to the TestData folder or an explicit path.</param>
        /// <param name="lo"></param>
        public static Document OpenUpdateFields(string fileName, LoadOptions lo)
        {
            Document doc = Open(fileName, lo);
            IList<Field> fields = FieldExtractor.ExtractToCollection(doc);

            foreach (Field field in fields)
            {
                if (field.HasSeparator)
                    field.Result = "34twrsdgrw546565twetwet";   // Some garbage text that should never appear during update.
            }

            doc.UpdateFields();

            return doc;
        }

        /// <summary>
        /// Use for unified tests.
        ///
        /// Opens a test file relative to TestData folder or from an explicit path.
        /// </summary>
        /// <param name="fileNameWithoutExtension">Can be relative to TestData folder or
        /// an explicit path. Should not have "ms" or "Out" suffixes as they are added automatically.</param>
        /// <param name="lf">The format of the document. Used to figure extension of the file.</param>
        public static Document Open(string fileNameWithoutExtension, LoadFormat lf)
        {
            return Open(fileNameWithoutExtension, lf, null);
        }

        /// <summary>
        /// Opens a test file relative to TestData folder or from an explicit path.
        /// </summary>
        /// <remarks>
        /// Method overload allows to open specified file name with custom load options.
        /// "LoadFormat" is used to detect extension of the file, because in general case
        /// load options can have "auto" load format.
        /// </remarks>
        public static Document Open(string fileNameWithoutExtension, LoadFormat lf, LoadOptions lo)
        {
            string srcFileName = BuildSrcFileName(fileNameWithoutExtension, lf);
            if (lo == null)
                lo = new LoadOptions { ReadDocTheme = false };
            return new Document(srcFileName, lo);
        }

        /// <summary>
        /// Opens a test file relative to TestData folder or from an explicit path.
        /// </summary>
        /// <param name="fileName">Can be relative to TestData folder or an explicit path.
        /// Should not have "ms" or "Out" suffixes as they are added automatically.</param>
        /// <param name="lo">Options for document loading.</param>
        public static Document Open(string fileName, LoadOptions lo)
        {
            string srcFileName = BuildTestFileName(fileName);
            return new Document(srcFileName, lo);
        }

        /// <summary>
        /// RK This is the old "classical" method used by the unified tests.
        /// Consider replacing all usages with the other overload that accepts the UnifiedScenario parameter.
        /// </summary>
        public static Document SaveOpen(Document doc, string fileNameWithoutExtension, LoadFormat lf, SaveFormat sf)
        {
            return SaveOpen(doc, fileNameWithoutExtension, GetUnifiedScenario(lf, sf));
        }

        /// <summary>
        /// Use for unified tests.
        ///
        /// Performs clone/save/open followed by compare against gold when it is applicable.
        /// </summary>
        /// <param name="doc">Document to operate with</param>
        /// <param name="fileNameWithoutExtension">Can be relative to TestData folder or
        /// an explicit path. Should not have "ms" or "Out" suffixes as they are added automatically.</param>
        /// <param name="lf">This is used to figure out the original file name and also decide
        /// whether to execute gold compare tests or not.</param>
        /// <param name="sf">The format in which to save the document.</param>
        public static Document SaveOpen(Document doc, string fileNameWithoutExtension, UnifiedScenario scenario)
        {
            return SaveOpen(doc, fileNameWithoutExtension, scenario, (LoadOptions)null);
        }

        /// <summary>
        /// Performs clone/save/open followed by compare against gold when it is applicable.
        /// </summary>
        /// <remarks>
        /// This method overload allows to specified custom load options for document opening.
        /// </remarks>
        public static Document SaveOpen(Document doc, string fileNameWithoutExtension,
            UnifiedScenario scenario, LoadOptions loadOptions)
        {
            // Most of the time we want things like pretty format and no generator name.
            SaveOptions saveOptions = SaveOptions.CreateSaveOptions(GetSaveFormat(scenario));
            saveOptions.SetTestMode();

            if (GetLoadFormat(scenario) == (LoadFormat)LoadFormatTest.TestDocxDml)
            {
                // For unified tests that load files with DrawingML pictures we want to convert them to VML.
                // Firstly, this tests our conversion of DrawingML to VML, secondly it is more logical for unified
                // tests because they are a common base and the "common" shapes in Word documents are VML.
                OoxmlSaveOptions ooxmlSaveOptions = (OoxmlSaveOptions)saveOptions;
                ooxmlSaveOptions.ConvertDmlPictureToVml = true;
            }

            return SaveOpen(doc, fileNameWithoutExtension, scenario, saveOptions, loadOptions);
        }

        /// <summary>
        /// Used for unified tests.
        ///
        /// See comments above.
        /// This overload accepts custom save options object that allows to control how the document is saved.
        /// </summary>
        public static Document SaveOpen(Document doc, string fileNameWithoutExtension,
            UnifiedScenario scenario, SaveOptions saveOptions)
        {
            return SaveOpen(doc, fileNameWithoutExtension, scenario, saveOptions, null);
        }

        /// <summary>
        /// This overload accepts custom save options and load options objects that allows to control how the document is saved.
        /// </summary>
        public static Document SaveOpen(Document doc, string fileNameWithoutExtension,
            UnifiedScenario scenario, SaveOptions saveOptions, LoadOptions loadOptions)
        {
            if (saveOptions == null)
                throw new ArgumentNullException("saveOptions");

            // Clone. This makes sure cloning code is exercised.
            doc = doc.Clone();

            LoadFormat loadFormat = GetLoadFormat(scenario);

            // We need the source file name to build the out and gold file names.
            // Cannot get the source file name from the document because it might have been loaded from a stream.
            string srcFileName = BuildSrcFileName(fileNameWithoutExtension, loadFormat);

            // This is related to the growth of unified tests from "doc to docx, rtf, wml" to include other combinations
            // such as "docx to docx, docx to rtf, rtf to docx" and others. We need to distinguish out and gold files
            // for different source formats. In the original "doc to xxx" tests the gold files had simple names.
            // But for the new conversions we add the source format info into the gold file name.
            // For example "TestSizeRelative.docx" processed by a Docx2Rtf unified test will look like "TestSizeRelative.docx Out.rtf"
            bool needsLoadFormatSuffix = (loadFormat != LoadFormat.Doc) && (loadFormat != (LoadFormat)LoadFormatTest.TestDocxDml);
            string nonDocSuffix = (needsLoadFormatSuffix) ? FileFormatUtil.LoadFormatToExtension(loadFormat) : "";

            // Save.
            saveOptions.SaveFormat = AdjustSaveFormatForMacrosInDocx(doc.HasMacros, saveOptions.SaveFormat);
            string outFileName = BuildOutFileName(srcFileName, nonDocSuffix, saveOptions.SaveFormat);
            EnsureDirectoryForFileExists(outFileName);
            doc.Save(outFileName, saveOptions);

            // Open again.
            // As we open just saved format, then it becomes the load format on opening.
            if (loadOptions == null)
                loadOptions = new LoadOptions(FileFormatUtil.SaveFormatToLoadFormat(saveOptions.SaveFormat), null, null);
            doc = new Document(outFileName, loadOptions);

            // Gold compare.
            if ((scenario & UnifiedScenario.NoGold) == 0)
            {
                string goldFileName = BuildGoldFileName(srcFileName, nonDocSuffix, saveOptions.SaveFormat);

                // We want to provide an ability for the user to compare against the ms file too.
                // The ms file is located in the same dir as the src file and we are interested in the ms file that matches our save format.
                string msFileName = BuildSrcFileName(fileNameWithoutExtension, FileFormatUtil.SaveFormatToLoadFormat(saveOptions.SaveFormat));

                VerifyGold(srcFileName, outFileName, goldFileName, msFileName, saveOptions, ((scenario & UnifiedScenario.ExportOnly) != 0));
            }

            return doc;
        }

        /// <summary>
        /// RK This is the old "classical" method used by the unified tests.
        /// Consider replacing all usages with the other overload that accepts the UnifiedScenario parameter.
        /// </summary>
        public static Document OpenSaveOpen(string fileNameWithoutExtension, LoadFormat lf, SaveFormat sf)
        {
            return OpenSaveOpen(fileNameWithoutExtension, GetUnifiedScenario(lf, sf));
        }

        /// <summary>
        /// Use for unified tests.
        ///
        /// Performs open/clone/save/open followed by compare against gold when it is applicable.
        /// </summary>
        public static Document OpenSaveOpen(string fileNameWithoutExtension, UnifiedScenario scenario)
        {
            return OpenSaveOpen(fileNameWithoutExtension, scenario, null);
        }

        /// <summary>
        /// Performs open/clone/save/open followed by compare against gold when it is applicable.
        /// </summary>
        /// <remarks>
        /// This overload allows to specified custom load options.
        /// </remarks>
        public static Document OpenSaveOpen(string fileNameWithoutExtension, UnifiedScenario scenario, LoadOptions loadOptions)
        {
            Document doc = Open(fileNameWithoutExtension, GetLoadFormat(scenario), loadOptions);
            return SaveOpen(doc, fileNameWithoutExtension, scenario, loadOptions);
        }

        public static void VerifyGold(string srcFileName, string outFileName, string goldFileName, string msFileName, SaveOptions saveOptions, bool exportOnly)
        {
            SaveFormat sf = FileFormatUtil.ExtensionToSaveFormat(Path.GetExtension(outFileName));
            switch (sf)
            {
                case SaveFormat.Doc:
                case SaveFormat.Dot:
                    // We do not do a gold compare for DOC files because they are binary.
                    return;
                case SaveFormat.Rtf:
                case SaveFormat.Markdown:
                case SaveFormat.Text:
                case SaveFormat.Docling:
                    // RK RTF does not yet support ExportImport gold level, needs more work.
                    VerifyGoldText(srcFileName, outFileName, goldFileName, msFileName, GoldLevel.ExportOnly, saveOptions);
                    return;
                case SaveFormat.WordML:
                case SaveFormat.FlatOpc:
                    VerifyGoldText(srcFileName, outFileName, goldFileName, msFileName, exportOnly ? GoldLevel.ExportOnly : GoldLevel.ExportImport, saveOptions);
                    return;
                case SaveFormat.Docx:
                case SaveFormat.Docm:
                case SaveFormat.Dotx:
                case SaveFormat.Dotm:
                    VerifyGoldDocx(srcFileName, outFileName, goldFileName, msFileName, exportOnly ? GoldLevel.ExportOnly : GoldLevel.ExportImport, saveOptions);
                    return;
                case SaveFormat.Odt:
                case SaveFormat.Ott:
                    // RK This is not a "mainstream" method. It is here because I save to ODT from "my tests" occasionally.
                    // The "mainstream" code is in TestGoldOdtBase so I might refactor some day.
                    TestZipUtil.VerifyFile("EXPORT ODT", srcFileName, outFileName, goldFileName, null);
                    return;
                case SaveFormat.Xps:
                case SaveFormat.OpenXps:
                    TestZipUtil.VerifyFile("EXPORT XPS", srcFileName, outFileName, goldFileName, null);
                    return;
                case SaveFormat.Html:
                case SaveFormat.Mhtml:
                case SaveFormat.Pdf:
                case SaveFormat.XamlFlow:
                case SaveFormat.Svg:
                    // This is not the "main steam" method for gold verification of HTML or PDF, but it can be called from some tests to make them "simpler".
                    VerifyGoldText(srcFileName, outFileName, goldFileName, msFileName, GoldLevel.ExportOnly, saveOptions);
                    return;
                case SaveFormat.Mobi:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Azw3:
                    throw new NotSupportedException("FOSS");
                case SaveFormat.Epub:
                    TestZipUtil.VerifyFile("EXPORT ePub", srcFileName, outFileName, goldFileName, null);
                    break;
                case SaveFormat.XamlFlowPack:
                    TestZipUtil.VerifyFile("EXPORT Xaml Package", srcFileName, outFileName, goldFileName, null);
                    return;
                case SaveFormat.Png:
                    TestImageUtil.VerifyFile(outFileName, goldFileName, srcFileName);
                    return;

                default:
                    throw new InvalidOperationException("Unexpected file format.");
            }
        }

        public static void VerifyGoldText(string srcFileName, string outFileName, string goldFileName, string msFileName,
            GoldLevel goldLevel, SaveOptions saveOptions)
        {
            // This part is the "export" part of the test.
            TestTxtUtil.VerifyFile(
                "EXPORT " + Path.GetExtension(outFileName),
                outFileName,
                goldFileName,
                msFileName,
                srcFileName);

            if (goldLevel == GoldLevel.ExportImport)
            {
                // This part is the "import" part of the test.
                // It reads a file that we exported above, saves it again and checks gold.
                Document doc = new Document(outFileName);
                doc.Save(outFileName, saveOptions);

                TestTxtUtil.VerifyFile(
                    "EXPORT IMPORT EXPORT " + Path.GetExtension(outFileName),
                    outFileName,
                    goldFileName,
                    msFileName,
                    srcFileName
                    );
            }
        }

        private static void VerifyGoldDocx(string srcFileName, string outFileName, string goldFileName, string msFileName, GoldLevel goldLevel, SaveOptions saveOptions)
        {
            // This part is the "export" test.
            TestZipUtil.VerifyFile(
                "EXPORT " + Path.GetExtension(outFileName),
                srcFileName,
                outFileName,
                goldFileName,
                msFileName);

            if (goldLevel == GoldLevel.ExportImport)
            {
                // This part is the "import" test.
                // It reads a file that we exported above, saves it again and checks gold.
                Document doc = new Document(outFileName);
                doc.Save(outFileName, saveOptions);

                TestZipUtil.VerifyFile(
                    "IMPORT AFTER EXPORT " + Path.GetExtension(outFileName),
                    srcFileName,
                    outFileName,
                    goldFileName,
                    msFileName);
            }
        }

        /// <summary>
        /// If the specified file name is absolute, returns it as is.
        /// If the specified file name is relative, returns TestData\fileName.
        /// </summary>
        [JavaThrows(false)]
        public static string BuildTestFileName(string fileName)
        {
            if (Path.IsPathRooted(fileName))
                return fileName;
            else
                return GetInTestDataPath(fileName);
        }

        /// <summary>
        /// Appends an ms suffix (if needed) and file extension to the specified file name.
        /// </summary>
        [JavaThrows(false)]
        private static string BuildSrcFileName(string fileNameWithoutExtension, LoadFormat loadFormat)
        {
            string result = fileNameWithoutExtension;

            // Add suffix.
            switch (loadFormat)
            {
                case LoadFormat.Rtf:
                case LoadFormat.WordML:
                case LoadFormat.Docx:
                case LoadFormat.Docm:
                case LoadFormat.Dotx:
                case LoadFormat.Dotm:
                    result = AddFileNameSuffix(result, " ms");
                    break;
                case (LoadFormat)LoadFormatTest.TestDocxDml:
                    result = AddFileNameSuffix(result, " dml ms");
                    break;
                default:
                    // Do nothing.
                    break;
            }

            // Add extension.
            result += FileFormatUtil.LoadFormatToExtension(loadFormat);

            // Add path.
            result = BuildTestFileName(result);

            // RK This is somewhat controversial. Historically we had only .doc files without the "ms" suffix and generated all other
            // .docx, .rtf, .wml from the .doc files and all generated files had the "ms" suffix. But this is gradually changing
            // because we are having more .docx, .rtf files that were not generated from .doc (because a particular feature is only in Word 2007 .docx).
            // We need to return the correct file name and for such files it will be without the "ms" suffix.
            if (!File.Exists(result))
                result = RemoveFileNameSuffix(result, " ms");

            return result;
        }

        /// <summary>
        /// Creates a directory for the specified file if the directory does not exist.
        /// </summary>
        [JavaThrows(false)]
        public static void EnsureDirectoryForFileExists(string fileName)
        {
            Aspose.TestFx.TestFxUtil.EnsureDirectoryForFileExists(fileName);
        }

        /// <summary>
        /// Deletes directory an d all files and folders in the directory.
        /// </summary>
        public static void DeleteTestDirectory(string testDirectoryPath)
        {
            if (Directory.Exists(testDirectoryPath))
                Directory.Delete(testDirectoryPath, true);
        }

        /// <summary>
        /// If the file is in TestData, the directory is replaced with TestOut.
        /// Removes all file name suffixes and adds the user and Out suffixes.
        /// Optionally replaces the file extension.
        /// </summary>
        [JavaThrows(false)]
        public static string BuildOutFileName(string srcFileName, string userSuffix, SaveFormat sf)
        {
            string result = BuildDerivedFileName(srcFileName, gTestOutPath, StringUtil.NullToEmptyString(userSuffix) + " Out", sf);
            return TestUtilPal.CorrectOutFileNameIfNeeded(result);
        }

        /// <summary>
        /// If the file is in TestData, the directory is replaced with TestGold.
        /// Removes all file name suffixes and adds the user and Gold suffixes.
        /// Optionally replaces the file extension.
        /// </summary>
        [JavaThrows(false)]
        public static string BuildGoldFileName(string srcFileName, string userSuffix, SaveFormat sf)
        {
            string result = BuildDerivedFileName(srcFileName, gTestGoldPath, StringUtil.NullToEmptyString(userSuffix) + " Gold", sf);
            return TestUtilPal.CorrectGoldFileNameIfNeeded(result);
        }

        [JavaThrows(false)]
        private static string BuildDerivedFileName(string srcFileName, string testDataDirReplacement, string fileNameSuffix, SaveFormat sf)
        {
            string result = ReplaceDirectory(srcFileName, gTestDataPath, testDataDirReplacement);
            if (result == srcFileName)
                result = ReplaceDirectory(srcFileName, gTestOutPath, testDataDirReplacement);
            if (result == srcFileName)
                result = ReplaceDirectory(srcFileName, gTestGoldPath, testDataDirReplacement);

            result = RemoveAllFileNameSuffixes(result);

            result = AddFileNameSuffix(result, fileNameSuffix);

            if (sf != SaveFormat.Unknown)
                result = Path.ChangeExtension(result, FileFormatUtil.SaveFormatToExtension(sf));

            return result;
        }

        /// <summary>
        /// Replaces oldDir with newDir if the oldDir is at the beginning of the file name.
        /// </summary>
        private static string ReplaceDirectory(string fileName, string oldDir, string newDir)
        {
            oldDir = TestFxUtil.CorrectPath(oldDir);
            newDir = TestFxUtil.CorrectPath(newDir);

            if (fileName.IndexOf(oldDir, StringComparison.OrdinalIgnoreCase) == 0)
                fileName = newDir + fileName.Substring(oldDir.Length);
            return fileName;
        }

        /// <summary>
        /// If we are saving as DOCX and document has macros we have to choose DOCM instead.
        /// </summary>
        private static SaveFormat AdjustSaveFormatForMacrosInDocx(bool hasMacros, SaveFormat sf)
        {
            switch (sf)
            {
                case SaveFormat.Docx:
                    if (hasMacros)
                        sf = SaveFormat.Docm;
                    break;
                default:
                    // Do nothing.
                    break;
            }
            return sf;
        }

        [JavaThrows(false)]
        private static string RemoveAllFileNameSuffixes(string fileName)
        {
            fileName = RemoveFileNameSuffix(fileName, " ms");
            fileName = RemoveFileNameSuffix(fileName, " Out");
            fileName = RemoveFileNameSuffix(fileName, " Gold");
            return fileName;
        }

        [JavaThrows(false)]
        public static string RemoveFileNameSuffix(string fileName, string suffix)
        {
            string name = Path.GetFileNameWithoutExtension(fileName);
            if (name.EndsWith(suffix))
                name = name.Substring(0, name.Length - suffix.Length);

            return Path.Combine(
                Path.GetDirectoryName(fileName),
                name + Path.GetExtension(fileName));
        }

        [JavaThrows(false)]
        public static string AddFileNameSuffix(string fileName, string suffix)
        {
            return Path.Combine(
                Path.GetDirectoryName(fileName),
                Path.GetFileNameWithoutExtension(fileName) + suffix + Path.GetExtension(fileName));
        }

        /// <summary>
        /// Returns RootPath + RelativePath.
        /// </summary>
        [JavaThrows(false)]
        public static string GetInRootPath(string relativePath)
        {
            return TestFxUtil.CorrectPath(Path.Combine(gRootPath, relativePath));
        }

        /// <summary>
        /// Returns TestDataPath + RelativePath.
        /// </summary>
        [JavaThrows(false)]
        public static string GetInTestDataPath(string relativePath)
        {
            return TestFxUtil.CorrectPath(Path.Combine(gTestDataPath, relativePath));
        }

        /// <summary>
        /// Returns TestOutPath + RelativePath.
        /// </summary>
        [JavaThrows(false)]
        public static string GetInTestOutPath(string relativePath)
        {
            return TestFxUtil.CorrectPath(Path.Combine(gTestOutPath, relativePath));
        }

        /// <summary>
        /// Returns TestGoldPath + RelativePath.
        /// </summary>
        [JavaThrows(false)]
        public static string GetInTestGoldPath(string relativePath)
        {
            return TestFxUtil.CorrectPath(Path.Combine(gTestGoldPath, relativePath));
        }

        /// <summary>
        /// Returns ModelPath + RelativePath.
        /// </summary>
        [JavaThrows(false)]
        public static string GetInModelPath(string relativePath)
        {
            return TestFxUtil.CorrectPath(Path.Combine(gModelPath, relativePath));
        }

        public static UnifiedScenario BuildScenario(LoadFormat lf, SaveFormat sf, bool isNoGold)
        {
            return (UnifiedScenario)((int)lf | ((int)sf << 8) | (isNoGold ? (int)UnifiedScenario.NoGold : 0));
        }

        public static UnifiedScenario GetUnifiedScenario(LoadFormat lf, SaveFormat sf)
        {
            switch (lf)
            {
                case LoadFormat.Doc:
                {
                    return BuildScenario(lf, sf, false);
                }
                case (LoadFormat)LoadFormatTest.TestDocxDml:
                {
                    return BuildScenario(lf, sf, false);
                }
                case LoadFormat.Docx:
                case LoadFormat.Dotx:
                case LoadFormat.FlatOpc:
                case LoadFormat.FlatOpcTemplate:
                {
                    switch (sf)
                    {
                        case SaveFormat.Docx:
                            return UnifiedScenario.Docx2DocxNoGold;
                        default:
                            return BuildScenario(lf, sf, false);
                    }
                }
                case LoadFormat.Docm:
                case LoadFormat.Dotm:
                case LoadFormat.FlatOpcMacroEnabled:
                case LoadFormat.FlatOpcTemplateMacroEnabled:
                {
                    switch (sf)
                    {
                        case SaveFormat.Docm:
                            return UnifiedScenario.Docm2DocmNoGold;
                        default:
                            return BuildScenario(lf, sf, false);
                    }
                }
                case LoadFormat.Rtf:
                {
                    switch (sf)
                    {
                        case SaveFormat.Rtf:
                            return UnifiedScenario.Rtf2RtfNoGold;
                        default:
                            return BuildScenario(lf, sf, false);
                    }
                }
                case LoadFormat.WordML:
                {
                    switch (sf)
                    {
                        case SaveFormat.WordML:
                            return UnifiedScenario.Wml2WmlNoGold;
                        default:
                            return BuildScenario(lf, sf, false);
                    }
                }
                case LoadFormat.MsWorks:
                case LoadFormat.Markdown:
                {
                    return BuildScenario(lf, sf, false);
                }
                default:
                    throw new InvalidOperationException("Unknown file format.");
            }
        }
        public static LoadFormat GetLoadFormat(UnifiedScenario scenario)
        {
            return (LoadFormat)(scenario & UnifiedScenario.LoadFormatMask);
        }
        public static SaveFormat GetSaveFormat(UnifiedScenario scenario)
        {
            return (SaveFormat)((int)(scenario & UnifiedScenario.SaveFormatMask) >> 8);
        }
        public static int GetFileSize(string fileName)
        {
            return TestFxUtil.GetFileSize(fileName);
        }

        /// <summary>
        /// Compares two streams and returns the result of comparison.
        /// </summary>
        /// <param name="stream1">First stream to compare.</param>
        /// <param name="stream2">Second stream to compare.</param>
        /// <returns>True, if streams have the same content.</returns>
        public static bool CompareStreams(Stream stream1, Stream stream2)
        {
            return TestFxUtil.CompareStreams(stream1, stream2);
        }

        public static string ReadAllText(string fileName)
        {
            using (StreamReader sr = File.OpenText(BuildTestFileName(fileName)))
                return sr.ReadToEnd();
        }

        public static byte[] ReadAllBytes(string fileName)
        {
            return StreamUtil.CopyFileToByteArray(BuildTestFileName(fileName));
        }

        /// <summary>
        /// Extracts content of MemoryStorage and writes it into specified directory.
        /// </summary>
        public static void ExtractStorage(MemoryStorage storage, string path)
        {
            Directory.CreateDirectory(path);
            FileStream guid = File.Open(Path.Combine(path, '{' + storage.Clsid.ToString()) + "}.clsid", FileMode.Create);
            guid.Close();
            foreach (KeyValuePair<string, object> entry in storage)
            {
                string fileName = GetPrintableName(entry.Key);
                if (entry.Value is MemoryStorage)
                {
                    ExtractStorage((MemoryStorage)entry.Value, Path.Combine(path, fileName));
                }
                else
                {
                    MemoryStream memoryStream = (MemoryStream)entry.Value;
                    long savedPos = memoryStream.Position;
                    memoryStream.Position = 0;
                    FileStream fileStream = File.Open(Path.Combine(path, fileName) + ".bin", FileMode.Create);
                    StreamUtil.CopyStream(memoryStream, fileStream);
                    fileStream.Close();
                    memoryStream.Position = savedPos;
                }
            }
        }

        /// <summary>
        /// Looks up for unprintable chars in name and converts them into HEX representation.
        /// </summary>
        public static string GetPrintableName(string name)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                if (char.IsLetterOrDigit(name[i]) || (name[i] == '_'))
                    sb.Append(name[i]);
                else
                    sb.AppendFormat("0x{0:x4}", (int)name[i]);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Saves part of Stream to file. Preserves Stream position.
        /// </summary>
        public static void SaveStreamBytes(Stream stream, int pos, int len, string filename)
        {
            long savedPos = stream.Position;

            stream.Position = pos;
            byte[] buffer = new byte[len];
            stream.Read(buffer, 0, len);

            FileStream f = new FileStream(filename, FileMode.Create);
            f.Write(buffer, 0, len);
            f.Close();

            stream.Position = savedPos;
        }

        /// <summary>
        /// Inserts specified document after node in source document. Commonly used test routine.
        /// </summary>
        public static void InsertDocument(Node insertAfterNode, Document srcDoc)
        {
            // We need to make sure that the specified node is either paragraph or table.
            if (!((insertAfterNode.NodeType == NodeType.Paragraph) || (insertAfterNode.NodeType == NodeType.Table)))
                throw new ArgumentException("The destination node should be either paragraph or table.");

            // We will be inserting into the parent of the destination paragraph.
            CompositeNode dstStory = insertAfterNode.ParentNode;

            // This object will be translating styles and lists during the import.
            NodeImporter importer = new NodeImporter(srcDoc, insertAfterNode.Document,
                                                     ImportFormatMode.KeepSourceFormatting);

            // Loop through all sections in the source document.
            foreach (Section srcSection in srcDoc.Sections)
            {

                // Loop through all block level nodes (paragraphs and tables) in the body of the section.
                foreach (Node srcNode in srcSection.Body)
                {
                    Node newNode;
                    if (srcNode.GetType() == typeof(Paragraph))
                    {
                        // Do not insert node if it is a last empty paragraph in the section.
                        Paragraph para = (Paragraph)srcNode;
                        if (para.IsEndOfSection && !para.HasChildNodes)
                        {
                            break;
                        }

                        // This creates a clone of the node, suitable for insertion into the destination document.
                        newNode = importer.ImportNode(srcNode, true);
                    }
                    else
                    {
                        // This creates a clone of the node, suitable for insertion into the destination document.
                        newNode = importer.ImportNode(srcNode, true);
                    }

                    if (newNode != null)
                    {
                        // Insert new node after the reference node.
                        dstStory.InsertAfter(newNode, insertAfterNode);
                        insertAfterNode = newNode;
                    }
                }
            }
        }

        /// <summary>
        /// Checks that certain warning exists in collection.
        /// </summary>
        public static bool ContainsWarning(WarningInfoCollection warnings, WarningType type, WarningSource source, string description)
        {
            foreach (WarningInfo warning in warnings)
            {
                if ((warning.WarningType == type) && (warning.Source == source) && (warning.Description == description))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks that certain warning exists in collection.
        /// </summary>
        public static bool ContainsWarningBySource(WarningInfoCollection warnings, WarningSource source, string description)
        {
            foreach (WarningInfo warning in warnings)
            {
                if ((warning.Source == source) && (warning.Description == description))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks that certain warning exists in collection.
        /// </summary>
        public static bool ContainsWarningByType(WarningInfoCollection warnings, WarningType type, string description)
        {
            foreach (WarningInfo warning in warnings)
            {
                if ((warning.WarningType == type) && (warning.Description == description))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Loads a file, and checks if this process hangs.
        /// </summary>
        /// <returns>Document object if loading process does not hang, null otherwise.</returns>
        [JavaThrows(true)]
        [CppSkipEntity("HangingDetector isn't supported in C++")]
        public static Document LoadWithTimeout(string fileName, int timeout)
        {
            return HangingDetector.LoadWithTimeoutCore(fileName, timeout);
        }

        /// <summary>
        /// Checks that given string is written to OPC package. Used to test DOCX export.
        /// </summary>
        public static bool IsWrittenToOpcPackage(string file, string partName, string text)
        {
            return IsWrittenToOpcPackageCore(new OpcPackage(file), partName, text);
        }

        /// <summary>
        /// Checks that given string is written to OPC package. Used to test DOCX export.
        /// </summary>
        public static bool IsWrittenToOpcPackage(Stream stream, string partName, string text)
        {
            return IsWrittenToOpcPackageCore(new OpcPackage(stream), partName, text);
        }

        /// <summary>
        /// Opens specified document and checks warning <see cref="WarningStrings.ViolationI4I"/> is issued.
        /// </summary>
        /// <remarks>WORDSNET-10749 Violation of i4i's patent #5,787,449. Custom xml will no longer be supported.</remarks>
        public static Document OpenAndVerifyCustomXmlWarning(string fileName)
        {
            LoadOptions lo = new LoadOptions();
            WarningInfoCollection warnings = new WarningInfoCollection();
            lo.WarningCallback = warnings;

            Document doc = Open(fileName, lo);

            // Check warning is issued.
            Assert.That(ContainsWarningByType(warnings, WarningType.DataLoss, WarningStrings.ViolationI4I), Is.True);

            return doc;
        }

        /// <summary>
        /// Opens specified document and checks warning <see cref="WarningStrings.ViolationI4I"/> is issued.
        /// </summary>
        /// <remarks>WORDSNET-10749 Violation of i4i's patent #5,787,449. Custom xml will no longer be supported.</remarks>
        public static Document OpenAndVerifyCustomXmlWarning(string fileNameWithoutExtension, LoadFormat lf)
        {
            string fileName = BuildSrcFileName(fileNameWithoutExtension, lf);
            return OpenAndVerifyCustomXmlWarning(fileName);
        }

        /// <summary>
        /// Executes the document validator without saving a document. Save format should be specified.
        /// </summary>
        public static void ExecuteValidator(Document doc, SaveFormat sf)
        {
            ExecuteValidator(doc, SaveOptions.CreateSaveOptions(sf));
        }

        /// <summary>
        /// Executes the document validator without saving a document. Save options should be specified.
        /// </summary>
        internal static DocumentValidator ExecuteValidator(Document doc, SaveOptions options)
        {
            DocumentValidator validator = new DocumentValidator();
            SaveInfo saveInfo = new SaveInfo(doc, null, null, options);
            validator.Execute(saveInfo);
            return validator;
        }

        /// <summary>
        /// Verifies MD5 hash value of given data.
        /// </summary>
        public static void VerifyHash(string hash, byte[] data)
        {
            byte[] dataHash = HashUtil.ComputeHash(CryptoUtilPal.CreateHashAlgorithm(DigestAlgorithm.MD5), data);
            Assert.That(ArrayUtil.DumpArray(dataHash).Replace(" ", ""), Is.EqualTo(hash));
        }

        /// <summary>
        /// Opens Document from ZIP archive.
        /// </summary>
        public static Document OpenFromZip(string zipFileName)
        {
            using (FileStream fs = File.Open(BuildTestFileName(zipFileName), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ZipReaderPal zipReader = new ZipReaderPal(fs);
                zipReader.MoveToNextEntry();
                using (MemoryStream docStream = zipReader.LoadEntryToMemory())
                    return new Document(docStream);
            }
        }

        /// <summary>
        /// Opens Document from ZIP archive.
        /// </summary>
        public static Document OpenFromZip(string zipFileName, string password)
        {
            using (FileStream fs = File.Open(BuildTestFileName(zipFileName), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ZipReaderPal zipReader = new ZipReaderPal(fs, password);
                zipReader.MoveToNextEntry();
                using (MemoryStream docStream = zipReader.LoadEntryToMemory())
                    return new Document(docStream);
            }
        }

        /// <summary>
        /// Opens Document from ZIP archive.
        /// </summary>
        public static Document OpenFromZip(string zipFileName, LoadOptions lo)
        {
            using (FileStream fs = File.Open(BuildTestFileName(zipFileName), FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ZipReaderPal zipReader = new ZipReaderPal(fs, lo.Password);
                zipReader.MoveToNextEntry();
                using (MemoryStream docStream = zipReader.LoadEntryToMemory())
                    return new Document(docStream, lo);
            }
        }

        /// <summary>
        /// Checks that given string is written to OPC package. Used to test DOCX export.
        /// </summary>
        private static bool IsWrittenToOpcPackageCore(OpcPackage package, string partName, string text)
        {
            OpcPackagePart part = package.FetchPartByName(partName);
            StreamReader reader = new StreamReader(part.Stream);
            part.Stream.Position = 0;
            string content = reader.ReadToEnd();

            return StringUtil.Contains(content, text, true);
        }

        /// <summary>
        /// Gets all tests data files (which are corresponding to pattern) from the root folder (and all its subdirectories) of all test documents.
        /// </summary>
        public static string[] GetTestDataFiles(string searchPattern)
        {
            return GetTestDataFiles(searchPattern, ".");
        }

        /// <summary>
        /// Gets all tests data files (which are corresponding to pattern) from the subdirectory (and all its subdirectories) of the root folder of all test documents.
        /// </summary>
        public static string[] GetTestDataFiles(string searchPattern, string subdir)
        {
            return Directory.GetFiles(Path.Combine(gTestDataPath, subdir), searchPattern, SearchOption.AllDirectories);
        }

        /// <summary>
        /// Checks if the specified <see cref="ThreadPal"/> has not completed within the specified timeout.
        /// </summary>
        /// <param name="worker"> THe process that should be checked.</param>
        /// <param name="secondsLimit">Timeout in seconds.</param>
        /// <returns>"True" if the process has not finished, otherwise -"false".</returns>
        public static bool IsHanging(ThreadPal worker, int secondsLimit)
        {
            worker.Start();
            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (worker.IsAlive && (sw.Elapsed.Seconds < secondsLimit))
                Thread.Sleep(500);

            sw.Stop();

            // The process did not completed within the specified timeout.
            if (worker.IsAlive)
            {
                worker.Abort();
                Assert.Fail("Most probably test hangs, please check!");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets nodes of specified types with a specified text.
        /// </summary>
        private static List<Node> GetNodesWithText(CompositeNode parent, string text, params NodeType[] nodeTypes)
        {
            const string fuzzy = "...";
            bool isFuzzyAtStart = text.StartsWith(fuzzy);
            bool isFuzzyAtEnd = text.EndsWith(fuzzy);

            string searchText = isFuzzyAtStart ? text.Substring(fuzzy.Length) : text;
            if (isFuzzyAtEnd)
                searchText = searchText.Substring(0, searchText.Length - fuzzy.Length);

            List<Node> foundNodes = new List<Node>();
            foreach (Node node in parent.GetChildNodes(nodeTypes, true))
            {
                if (isFuzzyAtStart && isFuzzyAtEnd)
                {
                    if (node.GetText().Contains(searchText))
                        foundNodes.Add(node);
                }
                else if (isFuzzyAtStart)
                {
                    if (node.GetText().EndsWith(searchText))
                        foundNodes.Add(node);
                }
                else if (isFuzzyAtEnd)
                {
                    if (node.GetText().StartsWith(searchText))
                        foundNodes.Add(node);
                }
                else
                {
                    if (node.GetText().Equals(searchText))
                        foundNodes.Add(node);
                }
            }

            return foundNodes;
        }

        /// <summary>
        /// Gets first occurred paragraph with a specified text.
        /// </summary>
        internal static Paragraph GetParagraphWithText(CompositeNode parent, string text)
        {
            List<Node> paragraphs = GetNodesWithText(parent, text, NodeType.Paragraph);
            if (paragraphs.Count > 0)
                return (Paragraph)paragraphs[0];

            return null;
        }

        /// <summary>
        /// Gets first occurred run with a specified text.
        /// </summary>
        internal static Run GetRunWithText(CompositeNode parent, string text)
        {
            List<Node> runs = GetNodesWithText(parent, text, NodeType.Run);
            if (runs.Count > 0)
                return (Run)runs[0];

            return null;
        }

        /// <summary>
        /// Checks color with diagnostic message.
        /// </summary>
        internal static void CheckColor(Color expectedColor, Color actualColor)
        {
            int expectedValue = expectedColor.ToArgb();
            int actualValue = actualColor.ToArgb();
            Assert.That(
                actualValue,
                Is.EqualTo(expectedValue),
                string.Format("Expected: 0x{0:X8}, Actual: 0x{1:X8}", expectedValue, actualValue));
        }

        /// <summary>
        /// Returns HyphPatternsPath + RelativePath.
        /// </summary>
        public static string GetInHyphenationPatternsPath(string relativePath)
        {
            return GetInTestDataPath(Path.Combine(@"Hyphenation\Patterns", relativePath));
        }

        /// <summary>
        /// The root folder of the solution.
        /// </summary>
        private static readonly string gRootPath = TestEnvironment.GetDirAwnet();

        /// <summary>
        /// The root folder of all test documents.
        /// </summary>
        private static readonly string gTestDataPath = TestEnvironment.GetTestData();

        /// <summary>
        /// The root folder of the "main" test documents.
        /// </summary>
        private static readonly string gModelPath = TestEnvironment.GetTestData("Model");

        private static readonly string gTestGoldPath = TestEnvironment.GetTestGold();

        private static readonly string gTestOutPath = TestEnvironment.GetTestOut();
    }
}
