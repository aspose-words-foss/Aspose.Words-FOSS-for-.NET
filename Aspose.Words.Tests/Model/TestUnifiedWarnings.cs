// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/03/2012 by Denis Darkin

using System.Collections.Generic;
using System.IO;
using Aspose.Fonts;
using Aspose.JavaAttributes;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Charts;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Fonts;
using Aspose.Words.Loading;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using Aspose.Words.Validation;
using NUnit.Framework;

namespace Aspose.Words.Tests.Model
{
    /// <summary>
    /// Test suite to verify unified Warnings.
    /// Presently this test suite includes only tests for Load and Save operations across all Conversions modules.
    /// </summary>
    /// <remarks>
    /// Tests related to rendering, layout and other modules except Conversions are scattered across other test suites and are not tested here.
    /// </remarks>
    [TestFixture]
    public class TestUnifiedWarnings
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }




        /// <summary>
        /// Tests that the compatibility mode hint is generated on saving a document with calling
        /// the <see cref="CompatibilityOptions.OptimizeFor"/> method for MS Word 2010 and higher compatibility
        /// if OOXML format is ECMA-376.
        /// </summary>
        [Test]
        public void TestCompatibilityModeHint()
        {
            Document doc = new Document();
            doc.CompatibilityOptions.OptimizeFor(MsWordVersion.Word2010);

            TestWarningCallback warningCallback = new TestWarningCallback();
            doc.WarningCallback = warningCallback;

            TestUtil.ExecuteValidator(doc, OoxmlSaveOptions.DocxWithCompliance(OoxmlComplianceCore.Ecma376));

            int warningCount = warningCallback.Count;
            Assert.That(warningCount > 0, Is.True);
            Assert.That(warningCallback[warningCount - 1].Description, Is.EqualTo(WarningStrings.HintToRemoveCompatibilityMode));

            warningCallback.Clear();
            TestUtil.ExecuteValidator(doc, OoxmlSaveOptions.DocxWithCompliance(OoxmlComplianceCore.IsoTransitional));

            Assert.That(warningCallback.Count, Is.EqualTo(warningCount - 1));
            if (warningCallback.Count > 0)
                Assert.That(warningCallback[warningCallback.Count - 1].Description, IsNot.EqualTo(WarningStrings.HintToRemoveCompatibilityMode));
        }

        /// <summary>
        /// WORDSNET-13779 Provide IWarningCallback mechanism for embedding font functionality.
        /// Tests that there is a warning about fonts are not embedded when <see cref="DocPr.EmbedSystemFonts"/>
        /// or <see cref="DocPr.SaveSubsetFonts"/> is set to 'true', because
        /// <see cref="DocPr.EmbedTrueTypeFonts"/> is set to false.
        /// </summary>
        // FOSS: only Docx survives — Doc/Rtf save were removed.
        [TestCase(SaveFormat.Docx)]
        public void TestWarningWhenEmbedFontsOptionDisabled(SaveFormat sf)
        {
            // Checks when 'EmbedSystemFonts' enabled.
            TestWarningWhenEmbedFontsOptionDisabledCore(true, false, sf);

            // Checks when 'SaveSubsetFonts' enabled.
            TestWarningWhenEmbedFontsOptionDisabledCore(false, true, sf);

            // Checks when both enabled.
            TestWarningWhenEmbedFontsOptionDisabledCore(true, true, sf);
        }

        /// <summary>
        /// WORDSNET-13779 Provide IWarningCallback mechanism for embedding font functionality.
        /// Tests that there is no a warning <see cref="WarningStrings.FontsNotEmbeddedDueToEmbeddingDisabled"/>
        /// when all font embedding options are set to 'false'.
        /// </summary>
        // FOSS: only Docx survives — Doc/Rtf save were removed.
        [TestCase(SaveFormat.Docx)]
        public void TestNoWarningWhenAllEmbedFontOptionsDisabled(SaveFormat sf)
        {
            Document doc = CreateTestDocumentWithEmbedFontsOptions(false, false, false, "Tahoma");
            TestWarningCallback warnings = SaveDocumentWithWarningCallback(doc, SaveOptions.CreateSaveOptions(sf));
            Assert.That(ContainsWarning(warnings, WarningStrings.FontsNotEmbeddedDueToEmbeddingDisabled), Is.False);
        }

        // FOSS: TestWarningFontEmbeddingNotSupported (Odt/WordML only), TestWarningEmbeddingLargeFontNotUsedInText
        // and TestWarningEmbeddingLargeFontUsedInText removed — they assert font-embedding size/subset warnings,
        // but font-embedding OUTPUT (and subsetting) was removed, so those warnings are never produced.

        // FOSS: TestCellRevisionWarning removed — it verified the CellRevisionLost warning on saving to WordML,
        // a removed format.

        /// <summary>
        /// Tests that a warning is generated on saving a document that contains a table with defined
        /// <see cref="Table.Title"/> or <see cref="Table.Description"/> property.
        /// </summary>
        // FOSS: only Docx survives — Doc/WordML/Rtf/Odt save were removed.
        [TestCase(SaveFormat.Docx)]
        public void TestTableTitleAndDescription(SaveFormat format)
        {
            SaveOptions options = SaveOptions.CreateSaveOptions(format);
            string formatAsString = format.ToString();
            if (format == SaveFormat.Docx)
            {
                ((OoxmlSaveOptions)options).Compliance = OoxmlCompliance.Ecma376_2006;
                formatAsString = DocumentValidator.Ecma376FormatTitle;
            }

            IDictionary<int, WarningInfo> toCheck = new Dictionary<int, WarningInfo>();
            toCheck.Add(0, new WarningInfo(WarningType.DataLoss, WarningSource.Validator,
                string.Format(WarningStrings.NotSupportedTableTitle, formatAsString)));
            toCheck.Add(1, new WarningInfo(WarningType.DataLoss, WarningSource.Validator,
                string.Format(WarningStrings.NotSupportedTableDescription, formatAsString)));

            TestSaveWarnings(@"ImportIso29500T\TestTableTitleDescription.docx", -1, toCheck, options);
        }

        /// <summary>
        /// WORDSNET-16830 Add warning if improper compatibility is set for LayoutInCell property.
        /// </summary>
        [Test]
        public void TestJira16830()
        {
            const string fileName = @"Model\Shape\Unsorted\TestJira16546";
            Document doc = TestUtil.Open(fileName, LoadFormat.Docx);

            Shape shape = doc.GetChildNodes(NodeType.Shape, true)[0] as Shape;
            shape.IsLayoutInCell = false;

            TestWarningCallback wc = new TestWarningCallback();
            doc.WarningCallback = wc;

            doc.CompatibilityOptions.OptimizeFor(MsWordVersion.Word2013);
            OoxmlSaveOptions saveOptions = new OoxmlSaveOptions();
            saveOptions.SaveFormat = SaveFormat.Docx;
            TestUtil.ExecuteValidator(doc, saveOptions);
            WarningInfo wi = wc[0];
            Assert.That(wc.Count, Is.EqualTo(1));
            Assert.That(wi.Source, Is.EqualTo(WarningSource.Validator));
            Assert.That(wi.WarningType, Is.EqualTo(WarningType.Hint));
            Assert.That(wi.Description.Contains("optimized for the version 2013 or higher"), Is.True);
        }

        // FOSS: Test20932 removed — its input is a binary .doc (removed load format), and the unsupported-VML-color
        // warning it checked targets WordML/Doc output.

        /// <summary>
        /// WORDSNET-25162 Font style underline does not work.
        /// Tests generating a warning when TextPath.Underline for VML is being edited.
        /// </summary>
        // FOSS: Test25162 removed — it only saved to Doc/Rtf/WordML (all removed) and did not even assert the
        // resulting warning (the ContainsWarning result was unused).

        /// <summary>
        /// Opens document and checks that there is a warning with the
        /// specified <paramref name="source"/> and <paramref name="description"/>.
        /// </summary>
        private static void VerifyOpenWarning(string fileName, WarningSource source, string description)
        {
            WarningInfoCollection warnings = new WarningInfoCollection();
            LoadOptions lo = new LoadOptions();
            lo.WarningCallback = warnings;

            TestUtil.Open(fileName, lo);
            Assert.That(TestUtil.ContainsWarningBySource(warnings, source, description), Is.True, string.Format("Warning '{0}' missed.", description));
        }

        /// <summary>
        /// Saves document and checks that there is a warning with the specified
        /// <paramref name="type"/>, <paramref name="source"/> and <paramref name="description"/>.
        /// </summary>
        private static void VerifySaveWarning(string fileName, WarningType type, WarningSource source, string description)
        {
            VerifySaveWarning(fileName, SaveFormat.Doc, type, source, description);
        }

        /// <summary>
        /// Saves document and checks that there is a warning with the specified
        /// <paramref name="type"/>, <paramref name="source"/> and <paramref name="description"/>.
        /// </summary>
        private static void VerifySaveWarning(string fileName, SaveFormat sf, WarningType type, WarningSource source, string description)
        {
            Document doc = TestUtil.Open(fileName);

            WarningInfoCollection warnings = new WarningInfoCollection();
            SaveOptions so = SaveOptions.CreateSaveOptions(sf);
            doc.WarningCallback = warnings;

            doc.Save(new MemoryStream(), so);
            Assert.That(TestUtil.ContainsWarning(warnings, type, source, description), Is.True, string.Format("Warning '{0}' missed.", description));
        }

        /// <summary>
        /// Test that a model node of specified document that is not supported by Aspose.Words raises warning during export.
        /// </summary>
        /// <param name="documentPath">Test document path.</param>
        /// <param name="warningsCount">Expected warning count.</param>
        /// <param name="toCheck">List of warnings to check.</param>
        /// <param name="saveOptions">Save options for specified format.</param>
        private static void TestSaveWarnings(string documentPath, int warningsCount,
            IDictionary<int, WarningInfo> toCheck, SaveOptions saveOptions)
        {
            Document doc = TestUtil.Open(documentPath);
            TestWarningCallback warningCallback = SaveDocumentWithWarningCallback(doc, saveOptions);

            TestWarningsCore(warningCallback, warningsCount, toCheck);
        }

        /// <summary>
        /// Check warnings count, source, type and description.
        /// </summary>
        private static void TestWarningsCore(TestWarningCallback warningCallback, int warningsCount,
            IDictionary<int, WarningInfo> toCheck)
        {
            // Check warnings count for test document.
            if (warningsCount >= 0)
                Assert.That(warningCallback.Count, Is.EqualTo(warningsCount));

            foreach (KeyValuePair<int, WarningInfo> entry in toCheck)
            {
                WarningInfo warning = warningCallback[entry.Key];
                WarningInfo expected = entry.Value;

                Assert.That(warning.Source, Is.EqualTo(expected.Source));
                Assert.That(warning.WarningType, Is.EqualTo(expected.WarningType));
                Assert.That(warning.Description, Is.EqualTo(expected.Description));
            }
            warningCallback.Clear();
        }

        /// <summary>
        /// Saves document with <see cref="TestWarningCallback"/> into temporary memory stream.
        /// </summary>
        private static TestWarningCallback SaveDocumentWithWarningCallback(Document doc, SaveOptions so)
        {
            TestWarningCallback warningCallback = new TestWarningCallback();
            doc.WarningCallback = warningCallback;
            using (MemoryStream tempStream = new MemoryStream())
            {
                doc.Save(tempStream, so);
            }

            return warningCallback;
        }

        /// <summary>
        /// Returns true if <see cref="WarningInfo"/> with specified description exists in <paramref name="warnings"/>.
        /// </summary>
        private static bool ContainsWarning(TestWarningCallback warnings, string warningDescription)
        {
            for (int i = 0; i < warnings.Count; i++)
                if (warnings[i].Description.Equals(warningDescription))
                    return true;

            return false;
        }

        /// <summary>
        /// Creates test document that contains text with the specified
        /// font and having specified font embedding options.
        /// </summary>
        private static Document CreateTestDocumentWithEmbedFontsOptions(bool isEmbedFonts, bool isEmbedSystem, bool isSubset, string fontName)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Font.Name = fontName;
            builder.Write("RegularStyle");

            FontInfoCollection fontInfos = doc.FontInfos;
            fontInfos.EmbedTrueTypeFonts = isEmbedFonts;
            fontInfos.EmbedSystemFonts = isEmbedSystem;
            fontInfos.SaveSubsetFonts = isSubset;

            return doc;
        }

        /// <summary>
        /// Core method for <see cref="TestWarningWhenEmbedFontsOptionDisabled"/>.
        /// Creates and saves document for testing and checks that
        /// warning <see cref="WarningStrings.FontsNotEmbeddedDueToEmbeddingDisabled"/> exists.
        /// </summary>
        private static void TestWarningWhenEmbedFontsOptionDisabledCore(bool embedSystemFonts, bool saveSubsetFonts, SaveFormat sf)
        {
            Document doc = CreateTestDocumentWithEmbedFontsOptions(false, embedSystemFonts, saveSubsetFonts, "Tahoma");
            TestWarningCallback warnings = SaveDocumentWithWarningCallback(doc, SaveOptions.CreateSaveOptions(sf));
            Assert.That(ContainsWarning(warnings, WarningStrings.FontsNotEmbeddedDueToEmbeddingDisabled), Is.True);
        }
    }
}
