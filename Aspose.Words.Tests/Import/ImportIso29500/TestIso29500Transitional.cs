// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/02/2009 by Denis Darkin

using System.Drawing;
using Aspose.Words.Drawing.Core.Dml.Geometries;
using Aspose.Words.Drawing.Core.Dml.Path;
using Aspose.Words.Nrx;
using Aspose.Words.RW.Docx.Reader;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using Aspose.Words.Tables;
using NUnit.Framework;

using Shape = Aspose.Words.Drawing.Shape;

namespace Aspose.Words.Tests.Import.ImportIso29500
{
    /// <summary>
    /// Test that OOXML ISO/IEC 29500 Transitional compliance level files are opened correctly.
    /// Also test that when saving to pre-ISO29500 formats all the newer stuff is not written.
    /// </summary>
    [TestFixture]
    public class TestIso29500Transitional
    {

        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }


        /// <summary>
        /// Test that <see cref="DocxXmlReader.ConvertUniversalMeasure"/> works.
        /// </summary>
        [TestCase("1", (int)NrxUnit.HalfPoints, 1d, (int)OoxmlComplianceCore.Ecma376)]
        [TestCase("1", (int)NrxUnit.Twips, 1d, (int)OoxmlComplianceCore.Ecma376)]
        [TestCase("1", (int)NrxUnit.HundredthsOfPoint, 1d, (int)OoxmlComplianceCore.Ecma376)]
        [TestCase("1", (int)NrxUnit.Emus, 1d, (int)OoxmlComplianceCore.Ecma376)]
        [TestCase("1pt", (int)NrxUnit.HalfPoints, 2d, (int)OoxmlComplianceCore.IsoTransitional)]
        [TestCase("1pt", (int)NrxUnit.Twips, 20d, (int)OoxmlComplianceCore.IsoTransitional)]
        [TestCase("1pt", (int)NrxUnit.HundredthsOfPoint, 100d, (int)OoxmlComplianceCore.IsoTransitional)]
        [TestCase("1pt", (int)NrxUnit.Emus, 12700d, (int)OoxmlComplianceCore.IsoTransitional)]
        [TestCase("1in", (int)NrxUnit.HalfPoints, 144d, (int)OoxmlComplianceCore.IsoTransitional)]
        [TestCase("0.5in", (int)NrxUnit.Twips, 720d, (int)OoxmlComplianceCore.IsoTransitional)]
        [TestCase("1in", (int)NrxUnit.HundredthsOfPoint, 7200d, (int)OoxmlComplianceCore.IsoTransitional)]
        [TestCase("1in", (int)NrxUnit.Emus, 914400d, (int)OoxmlComplianceCore.IsoTransitional)]
        [TestCase("2.54cm", (int)NrxUnit.HalfPoints, 144d, (int)OoxmlComplianceCore.IsoTransitional)]
        [TestCase("127mm", (int)NrxUnit.Twips, 7200d, (int)OoxmlComplianceCore.IsoTransitional)]
        [TestCase("2.54cm", (int)NrxUnit.HundredthsOfPoint, 7200d, (int)OoxmlComplianceCore.IsoTransitional)]
        [TestCase("25.4mm", (int)NrxUnit.Emus, 914400d, (int)OoxmlComplianceCore.IsoTransitional)]
        [TestCase("24pc", (int)NrxUnit.HalfPoints, 576d, (int)OoxmlComplianceCore.IsoTransitional)]
        [TestCase("24pi", (int)NrxUnit.Twips, 5760d, (int)OoxmlComplianceCore.IsoTransitional)]
        [TestCase("10pc", (int)NrxUnit.HundredthsOfPoint, 12000d, (int)OoxmlComplianceCore.IsoTransitional)]
        [TestCase("10pi", (int)NrxUnit.Emus, 1524000d, (int)OoxmlComplianceCore.IsoTransitional)]
        public void TestUniversalMeasureReader(string inputValue, int targetUnit, double expectedResult,
            int expectedCompliance)
        {
            OoxmlComplianceInfo complianceInfo = new OoxmlComplianceInfo();
            double actual = NrxXmlReader.ConvertUniversalMeasure(inputValue, (NrxUnit)targetUnit, complianceInfo);
            Assert.That(System.Math.Round(actual, 3), Is.EqualTo(System.Math.Round(expectedResult, 3)));
            Assert.That(complianceInfo.Compliance, Is.EqualTo((OoxmlComplianceCore)expectedCompliance));
        }

        /// <summary>
        /// Tests that <see cref="DmlAdjustableCoordinate"/> supports values in format defined
        /// by the ST_UniversalMeasure simple type.
        /// </summary>
        [TestCase("1pt", 12700d)]
        [TestCase("-0.01in", -9144d)]
        [TestCase("10", 10d)]
        public void TestUniversalMeasureInAdjustableCoordinate(string value, double expectedResult)
        {
            DmlAdjustableCoordinate coordinate = new DmlAdjustableCoordinate(value);
            Assert.That(System.Math.Round(coordinate.GetValue(null)), Is.EqualTo(System.Math.Round(expectedResult)), string.Format(                "Wrong conversion of {0}.", value));
        }

        /// <summary>
        /// Tests that <see cref="DmlTextBoxRect"/> supports values in format defined
        /// by the ST_UniversalMeasure simple type.
        /// </summary>
        [TestCase("1pt", "-0.1in", "30000", "5.08mm", 12700f, -91440f, 30000f, 182880f)]
        public void TestUniversalMeasureInRect(string left, string top, string right, string bottom,
            float expectedOutputL, float expectedOutputT, float expectedOutputR, float expectedOutputB)
        {
            DmlTextBoxRect textBoxRect = new DmlTextBoxRect(left, top, right, bottom);
            RectangleF rect = textBoxRect.GetRectangle(null, false);

            Assert.That(System.Math.Round(rect.Left), Is.EqualTo(System.Math.Round(expectedOutputL)), string.Format(                "Wrong left coordinate (input value: {0}).", left));
            Assert.That(System.Math.Round(rect.Top), Is.EqualTo(System.Math.Round(expectedOutputT)), string.Format(                "Wrong top coordinate (input value: {0}).", top));
            Assert.That(System.Math.Round(rect.Right), Is.EqualTo(System.Math.Round(expectedOutputR)), string.Format(                "Wrong right coordinate (input value: {0}).", right));
            Assert.That(System.Math.Round(rect.Bottom), Is.EqualTo(System.Math.Round(expectedOutputB)), string.Format(                "Wrong bottom coordinate (input value: {0}).", bottom));
        }


        private static void VerifyDocProtectionProperties(Document doc)
        {
            Assert.That(doc.DocumentProtection.PasswordHash.CryptAlgorithmSid, Is.EqualTo(4));
            Assert.That(doc.DocumentProtection.PasswordHash.CryptSpinCount, Is.EqualTo(100000));
        }

        private static void VerifyCustomCompatSettings(Document doc, int nSettings)
        {
            Assert.That(doc.CompatibilityOptions.CustomCompatibilitySettings.Count, Is.EqualTo(nSettings));
            Assert.That(doc.CompatibilityOptions.CustomCompatibilitySettings[0].Name, Is.EqualTo("compatibilityMode"));
            Assert.That(doc.CompatibilityOptions.CustomCompatibilitySettings[0].Uri, Is.EqualTo("http://schemas.microsoft.com/office/word"));

            Assert.That(doc.CompatibilityOptions.CustomCompatibilitySettings[3].Name, Is.EqualTo("doNotFlipMirrorIndents"));
            Assert.That(doc.CompatibilityOptions.CustomCompatibilitySettings[3].Uri, Is.EqualTo("http://schemas.microsoft.com/office/word"));
            Assert.That(doc.CompatibilityOptions.CustomCompatibilitySettings[3].Value, Is.EqualTo("1"));
        }


        private static void VerifyStylePaneFormatFilterSettings(Document readDoc)
        {
            Assert.That(readDoc.DocPr.StylePaneFormatFilterSettings.Data, Is.EqualTo(38696));
            Assert.That(readDoc.DocPr.StylePaneFormatFilterSettings.AllStyles, Is.False);
            Assert.That(readDoc.DocPr.StylePaneFormatFilterSettings.CustomStyles, Is.False);
            Assert.That(readDoc.DocPr.StylePaneFormatFilterSettings.LatentStyles, Is.False);
            Assert.That(readDoc.DocPr.StylePaneFormatFilterSettings.StylesInUse, Is.True);
            Assert.That(readDoc.DocPr.StylePaneFormatFilterSettings.HeadingStyles, Is.True);
            Assert.That(readDoc.DocPr.StylePaneFormatFilterSettings.NumberingStyles, Is.False);
            Assert.That(readDoc.DocPr.StylePaneFormatFilterSettings.TableStyles, Is.False);

            Assert.That(readDoc.DocPr.StylePaneFormatFilterSettings.DirectFormattingOnNumbering, Is.True);
            Assert.That(readDoc.DocPr.StylePaneFormatFilterSettings.DirectFormattingOnParagraphs, Is.True);
            Assert.That(readDoc.DocPr.StylePaneFormatFilterSettings.DirectFormattingOnRuns, Is.True);
            Assert.That(readDoc.DocPr.StylePaneFormatFilterSettings.DirectFormattingOnTables, Is.False);

            Assert.That(readDoc.DocPr.StylePaneFormatFilterSettings.ClearFormatting, Is.True);
            Assert.That(readDoc.DocPr.StylePaneFormatFilterSettings.Top3HeadingStyles, Is.False);
            Assert.That(readDoc.DocPr.StylePaneFormatFilterSettings.VisibleStyles, Is.False);
            Assert.That(readDoc.DocPr.StylePaneFormatFilterSettings.AlternateStyleNames, Is.True);
        }



        private static void VerifyNewBorderArtEnumMembers(Document readDoc)
        {
            Assert.That(readDoc.Sections[0].SectPr.BorderLeft.IsPageBorderArt, Is.True);
            Assert.That(readDoc.Sections[0].SectPr.BorderTop.LineStyle, Is.EqualTo((LineStyle)PageBorderArt.TriangleCircle2));
            Assert.That(readDoc.Sections[0].SectPr.BorderRight.LineStyle, Is.EqualTo((LineStyle)PageBorderArt.TriangleCircle2));
            Assert.That(readDoc.Sections[0].SectPr.BorderBottom.LineStyle, Is.EqualTo((LineStyle)PageBorderArt.TriangleCircle2));
        }

        /// <summary>
        /// Verify that new table elements: title and description are read and saved correctly.
        /// </summary>
        [Test]
        public void TestNewTableElements()
        {
            const string testName = @"ImportIso29500T\TestTableTitleDescription.docx";

            Document readDoc = TestUtil.Open(testName);
            VerifyNewTableElements(readDoc);

            Document writtenDoc = SaveIso29500Open(readDoc, testName);
            VerifyNewTableElements(writtenDoc);
        }

        private static void VerifyNewTableElements(Document doc)
        {
            Assert.That(doc.Sections[0].Body.Tables[0].Title, Is.EqualTo("Table title. "));
            Assert.That(doc.Sections[0].Body.Tables[0].Description, Is.EqualTo("Table descriptions."));
        }

        /// <summary>
        /// Some of the rPr elements can now be present in newer format: points, inches etc.
        /// Test that this can be read properly. Write tests are not required since we will write as usual.
        /// </summary>
        [Test]
        public void TestNewPrNumericFormat()
        {
            const string testName = @"ImportIso29500T\TestrPrNewNumericFormats.docx";
            Document readDoc = TestUtil.Open(testName);
            Assert.That(readDoc.Sections[0].Body.FirstParagraph.Runs[0].RunPr.Size, Is.EqualTo(27 * 12 * 2));
            Assert.That(readDoc.Sections[0].Body.FirstParagraph.Runs[0].RunPr.SizeBi, Is.EqualTo(12 * 2));
            Assert.That(readDoc.Sections[0].Body.FirstParagraph.Runs[0].RunPr.Position, Is.EqualTo(-24));
        }


        /// <summary>
        /// Combined test for testing reading ISO29500 new stuff related to formatting properties:
        /// - tblLook new attributes,
        /// - tcBorders start/end
        /// </summary>
        [Test]
        public void TestTableStartEndTblLook()
        {
            const string testName = @"ImportIso29500T\TestTableStartEndTblLook.docx";
            Document readDoc = TestUtil.Open(testName);

            Assert.That((readDoc.Sections[0].Body.Tables[0].Rows[0].TablePr.StyleOptions), Is.EqualTo(TableStyleOptions.FirstRow | TableStyleOptions.FirstColumn | TableStyleOptions.RowBands));

            CellPr cellProperties = readDoc.Sections[0].Body.Tables[0].Rows[0].Cells[0].CellPr;
            Assert.That(cellProperties.BorderLeft.LineStyle, Is.EqualTo(LineStyle.DoubleWave));
            Assert.That(cellProperties.BorderRight.LineStyle, Is.EqualTo(LineStyle.Single));

            cellProperties = readDoc.Sections[0].Body.Tables[0].Rows[1].Cells[0].CellPr;
            Assert.That(cellProperties.BorderRight.LineStyle, Is.EqualTo(LineStyle.DoubleWave));
        }

        /// <summary>
        /// Verify that optimize for browser setting is read and written.
        /// </summary>
        [Test]
        public void TestOptimizeForBrowser()
        {
            const string testName = @"ImportIso29500T\TestOptimizeForBrowserAndPercentage.docx";
            Document readDoc = TestUtil.Open(testName);
            Assert.That(readDoc.DocPr.WebTarget, Is.EqualTo(WebTarget.XhtmlPlusCss2));

            Document iso29500loaded = SaveIso29500Open(readDoc, testName);
            Assert.That(iso29500loaded.DocPr.WebTarget, Is.EqualTo(WebTarget.XhtmlPlusCss2));

            Document isoECMA376loaded = SavePreIso29500Open(readDoc, testName);
            Assert.That(isoECMA376loaded.DocPr.WebTarget, Is.EqualTo(WebTarget.None));
        }

        /// <summary>
        /// Test transitional feature 9.10.8 ST_UnqualifiedPercentage, and verify
        /// that qualified e.g. with "%" sign, and unqualified percentage values are read
        /// correctly.
        /// </summary>
        [Test]
        public void TestPercentage()
        {
            const string testName = @"ImportIso29500T\TestOptimizeForBrowserAndPercentage.docx";
            Document readDoc = TestUtil.Open(testName);
            Assert.That(readDoc.Sections[0].Body.Tables[0].Rows[0].TablePr.PreferredWidth.Value, Is.EqualTo(PreferredWidth.FromPercent(100).Value).Within(0.0001));
            Assert.That(readDoc.Sections[0].Body.Tables[0].Rows[0].Cells[0].CellPr.PreferredWidth.Value, Is.EqualTo(PreferredWidth.FromPercent(30).Value).Within(0.0001));
        }

        /// <summary>
        /// Verify that keywords are read ok in doc core properties package.
        /// </summary>
        [Test]
        public void TestKeywords()
        {
            const string keywordString = "test, document, aspose, docx, iso29500, transitional, strict";

            Document readDoc = TestUtil.Open(@"ImportIso29500T\TestKeywords.docx");
            Assert.That(readDoc.BuiltInDocumentProperties.Keywords, Is.EqualTo(keywordString));

            Document readOlderDoc = TestUtil.Open(@"ImportIso29500T\TestOlderKeywords.docx");
            Assert.That(readOlderDoc.BuiltInDocumentProperties.Keywords, Is.EqualTo(keywordString));


            Document readCombinedKeywords = TestUtil.Open(@"ImportIso29500T\TestCombinedKeywords.docx");
            Assert.That(readCombinedKeywords.BuiltInDocumentProperties.Keywords, Is.EqualTo(keywordString));
        }


        /// <summary>
        /// Test cloning document with AlternateContent (clone AlternateContent complex attribute).
        /// </summary>
        [Test]
        public void AlternateContentClone()
        {
            Document doc = TestUtil.Open(@"ImportIso29500T\AlternateContentClone.docx");
            Shape originalDrawingMl = (Shape)doc.GetChild(NodeType.Shape, 0, true);

            Document clonedDocument = (Document)doc.Clone(true);
            Shape clonedDrawingMl = (Shape)clonedDocument.GetChild(NodeType.Shape, 0, true);

            Assert.That(clonedDrawingMl.RunPr.AlternateContent, IsNot.SameAs(originalDrawingMl.RunPr.AlternateContent));

            Shape originalFallBackShape = (Shape) originalDrawingMl.RunPr.AlternateContent.FallBack.FirstChild;
            Shape clonedFallBackShape = (Shape) originalDrawingMl.RunPr.AlternateContent.FallBack.FirstChild.Clone(true);
            Assert.That(clonedFallBackShape, IsNot.SameAs(originalFallBackShape));
        }

#if (!JAVA) && WORD2010   // This uses automation to open documents in MS Word. We don't want this in Java.
        /// <summary>
        /// Verify that all test input and output files generated in this suite are opened ok in MSWord 2010.
        /// </summary>
        [Test, Ignore("This uses automation to open documents in MS Word. We don't want this in Java.")]
        public void TestWordIn2010()
        {
            OpenAllWordFiles(TestUtil.GetInTestDataPath("ImportIso29500T"));
            OpenAllWordFiles(TestUtil.GetInTestOutPath("ImportIso29500T"));
        }

        /// <summary>
        /// Verify that docx files are opened alright in MSWord 2010.
        /// </summary>
        /// <param name="sourceDir"></param>
        private static void OpenAllWordFiles(string sourceDir)
        {
            DirectoryInfo di = new DirectoryInfo(sourceDir);
            FileInfo[] testFiles = di.GetFiles();
            Microsoft.Office.Interop.Word.ApplicationClass wordApp = new Microsoft.Office.Interop.Word.ApplicationClass();
            object missing = System.Reflection.Missing.Value;
            foreach (FileInfo curFile in testFiles)
            {
                if (Path.GetExtension(curFile.Name) != ".docx") // skip anything but docx
                    continue;

                object testName = sourceDir + "\\" + curFile.Name;
                Microsoft.Office.Interop.Word.Document doc =
                    wordApp.Documents.Open(ref testName, ref missing, ref missing, ref missing,
                                           ref missing, ref missing, ref missing, ref missing,
                                           ref missing, ref missing, ref missing, ref missing,
                                           ref missing, ref missing, ref missing, ref missing);
                object saveFile = false;
                Assert.IsNotNull(doc._CodeName);
                doc.Close(ref saveFile, ref missing, ref missing);
            }
            wordApp.Quit(ref missing, ref missing, ref missing);
        }
#endif

        /// <summary>
        /// WORDSNET-6139 Open XML SDK 2.0 Doesn't validate the DOCX generated by Aspose.Words.
        /// 1. Exclude scriptAnchor from picture for Docx format, absolute for tblpXSpec and tblpYSpec.
        /// 2. Write prop lang for noLineBreaksAfter and noLineBreaksBefore (Custom Set of Characters
        /// Which Cannot End a Line, Custom Set Of Characters Which Cannot Begin A Line) part 1 reference c059575_ISO_IEC_29500-1_2011.
        /// 3. MaxLength in txextInput is Int16
        /// 4. Write lnNumType as in spec (17.6.8 lnNumType (Line Numbering Settings) part 1 reference c059575_ISO_IEC_29500-1_2011.
        /// 5. Header and footer in pgMar - UInt32.
        /// 6. Write anchorx , anchory as in spec.
        /// </summary>
        /// <remarks>
        /// If this test broke, check Out file using x:\awnet\Aspose.Foundation\Tools\DocumentValidator.
        /// </remarks>
        [Test]
        public void TestJira6139()
        {
            TestUtil.OpenSaveOpen(@"ImportIso29500T\TestJira6139.docx");
        }

        /// <summary>
        /// andrnosk: WORDSNET-6710 Unable to access SDT nodes from inside DrawingML.
        /// Fixed by adding mechanism to check whether Choice contains unsupported elements,
        /// if contains Fallback will be read instead.
        /// </summary>
        [Test]
        public void TestJira6710()
        {
            Document doc = TestUtil.Open(@"ImportIso29500T\TestJira6710.docx");
            Assert.That(doc.ComplianceInfo.IsDrawingExtensions, Is.True);
            Assert.That(doc.GetChildNodes(NodeType.StructuredDocumentTag, true).Count, Is.EqualTo(4));
        }

        /// <summary>
        /// Saves ISO 29500 transitional docx and then opens it without gold comparison.
        /// Returns opened document.
        /// </summary>
        private static Document SaveIso29500Open(Document readDoc, string testName)
        {
            OoxmlSaveOptions opt = new OoxmlSaveOptions();
            opt.Compliance = OoxmlCompliance.Iso29500_2008_Transitional;
            string outFileName = TestUtil.Save(readDoc, testName, opt, false);
            return TestUtil.Open(outFileName);
        }

        private static Document SaveRtfOpen(Document readDoc, string testName)
        {
            string srcFileName = TestUtil.BuildTestFileName(testName);
            string outFileName = TestUtil.BuildOutFileName(srcFileName, "", SaveFormat.Rtf);
            TestUtil.Save(readDoc, outFileName);
            return TestUtil.Open(outFileName);
        }

        /// <summary>
        /// Saves pre-ISO29500 format docx and then opes it without gold comparison.
        /// Allows to verify that all new stuff is lost during saves to older formats.
        /// </summary>
        private static Document SavePreIso29500Open(Document readDoc, string testName)
        {
            OoxmlSaveOptions opt = new OoxmlSaveOptions();
            opt.Compliance = OoxmlCompliance.Ecma376_2006;
            string outFileName = TestUtil.Save(readDoc, testName, opt, false);
            return TestUtil.Open(outFileName);
        }
    }
}
