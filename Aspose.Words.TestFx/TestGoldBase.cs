// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/11/2006 by Roman Korchagin

using System.IO;
using Aspose.Common;
using Aspose.TestFx;
using Aspose.Words.Loading;
using Aspose.Words.Saving;
using Aspose.Words.Tests.Fonts;
using NUnit.Framework;

namespace Aspose.Words.Tests
{
    /// <summary>
    /// Base class for exports into human-readable formats that we verify against gold files.
    /// </summary>
    public class TestGoldBase
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
        /// This is a standard test for export and import with compare against a text gold file.
        /// First it test export: saves a document in the specified format and verifies against a gold file.
        /// Then it tests import: opens the file that was just created, saves again and verifies against gold.
        ///
        /// This method allows you to test various save settings. You need to open
        /// the document yourself, set the settings and then call this method to verify against gold.
        /// </summary>
        /// <param name="doc">The document to save.</param>
        /// <param name="relSrcFileName">Name of the original file, relative to the TestData\Model folder.
        /// The file name will be used to derive the name for the Out and Gold.</param>
        /// <param name="suffix">Additional suffix is added to file name, both out and gold
        /// to form separate files for any options we need to test (i. e. embedded styles).</param>
        /// <param name="goldLevel">Specifies whether to verify export only or both export and import.</param>
        /// <param name="saveOptions"></param>
        /// <returns>The name of the Out file.</returns>
        protected static string VerifyTextGold(
            Document doc,
            string relSrcFileName,
            string suffix,
            GoldLevel goldLevel,
            SaveOptions saveOptions)
        {
            Debug.Assert(saveOptions != null);

            string srcFileName = GetSrcFileName(relSrcFileName);
            string outFileName = GetOutFileName(relSrcFileName, suffix, saveOptions.SaveFormat);
            string goldFileName = GetGoldFileName(relSrcFileName, suffix, saveOptions.SaveFormat);

            TestUtil.EnsureDirectoryForFileExists(outFileName);
            doc.Save(outFileName, saveOptions);

            TestUtil.VerifyGoldText(srcFileName, outFileName, goldFileName, null, goldLevel, saveOptions);
            return outFileName;
        }

        protected static Document Open(string relSrcFileName)
        {
            return Open(relSrcFileName, null);
        }

        protected static Document Open(string relSrcFileName, LoadOptions lo)
        {
            return new Document(GetSrcFileName(relSrcFileName), lo);
        }

        protected static string GetSrcFileName(string relSrcFileName)
        {
            return TestUtil.GetInModelPath(relSrcFileName);
        }

        protected static string GetOutFileName(string relSrcFileName, string userSuffix, SaveFormat sf)
        {
            return TestUtil.BuildOutFileName(TestUtil.GetInModelPath(relSrcFileName), userSuffix, sf);
        }

        protected static string GetGoldFileName(string relSrcFileName, string userSuffix, SaveFormat sf)
        {
            return TestUtil.BuildGoldFileName(TestUtil.GetInModelPath(relSrcFileName), userSuffix, sf);
        }
    }
}
