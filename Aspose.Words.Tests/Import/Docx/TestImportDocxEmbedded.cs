// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System.IO;
using Aspose.Ss;
using Aspose.TestFx;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Nrx;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Docx
{
    /// <summary>
    /// Tests for OLE and OOXML embedded objects in DOCX documents.
    /// </summary>
    [TestFixture]
    public class TestImportDocxEmbedded
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// This document contains embedded Word OOXML packages.
        /// </summary>
        [Test]
        public void TestEmbeddedWordOoxml()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\Embedded\TestEmbeddedWordOoxml.docx");
            CheckOoxmlObjectBySize(doc, 0, DocxContentType.Docx, 12791);
            CheckOoxmlObjectBySize(doc, 1, DocxContentType.Docm, 12913);
            CheckOoxmlObjectBySize(doc, 2, DocxContentType.Dotx, 12918);
            CheckOoxmlObjectBySize(doc, 3, DocxContentType.Dotm, 12934);
        }

        /// <summary>
        /// This document contains embedded Excel OOXML packages.
        /// </summary>
        [Test]
        public void TestEmbeddedExcelOoxml()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\Embedded\TestEmbeddedExcelOoxml.docx");
            // We use golds here because we unhide the workbook and the resulting ZIP on Java is different.
            // WORDSNET-14899 MSW hides workbook when it inserted as icon (DrawAspect="Icon"). Current DOCX
            // contains shapes with DrawAspect="Content", so unhide of workbooks for Excel OOXML packages
            // will be skipped.
            CheckOoxmlObjectByGold(doc, 0, DocxContentType.Xlsx);
            CheckOoxmlObjectByGold(doc, 1, DocxContentType.Xlsm);
            CheckOoxmlObjectBySize(doc, 2, DocxContentType.Xlsb, 8754);
            // Although I tried to embed Excel Templates XLTX and XLTM here, MS Word inserts them as Spreadsheets, so be it.
            CheckOoxmlObjectByGold(doc, 3, DocxContentType.Xlsx);
            CheckOoxmlObjectByGold(doc, 4, DocxContentType.Xlsm);
        }

        /// <summary>
        /// This test is a pert of the fix concerned with WORDSNET-14899
        /// Checks that workbook with DrawAspect="Icon" will be unhidden after saving to file.
        /// </summary>
        [Test]
        public void TestEmbeddedExcelOoxmlIcon()
        {
            const string path = @"ImportDocx\Embedded\TestEmbeddedExcelOoxmlIcon";
            Document doc = TestUtil.OpenSaveOpen(path, UnifiedScenario.Docx2DocxNoGold);

            CheckOoxmlObjectByGold(doc, 0, DocxContentType.Xlsx);
            CheckOoxmlObjectByGold(doc, 1, DocxContentType.Xlsm);
            CheckOoxmlObjectByGold(doc, 3, DocxContentType.Xlsx);
            CheckOoxmlObjectByGold(doc, 4, DocxContentType.Xlsm);
        }

        /// <summary>
        /// WORDSNET-14899 (fixes) - Saving Embedded Spreadsheet produces a different file every time.
        /// The reason of the problem is that OOXML package is re-created while saving to external file.
        /// Package re-creation is caused of attempt to unhide a workbook. Workbook has to be unhidden when
        /// attribute "DrawAspect" has value "Icon" to fix the problem.
        /// </summary>
        [Test]
        [NonParallelizable]
        public void TestJira14899()
        {
            Document doc = TestUtil.Open(@"Model\Shape\Ole\TestJira14899", LoadFormat.Docx);
            Shape excelWb = (Shape)doc.FirstSection.Body.GetChild(NodeType.Shape, 0, true);

            // Expected that "DrawAspect" is a "Content".
            Assert.That((bool)excelWb.FetchShapeAttrInternal(ShapeAttr.OleIcon), Is.False);
            // Compare output streams when "DrawAspect" is a "Content".
            CompareWorkbookData(excelWb, true, false);

            // Set "DrawAspect" value to "Icon".
            excelWb.SetShapeAttrInternal(ShapeAttr.OleIcon, true);
            // Compare output streams when "DrawAspect" is a "Icon".
            // In Java Streams are different in debg mode but identical in flat run.
#if !JAVA
            CompareWorkbookData(excelWb, true, false);
#endif
            // Output streams must have the same content when "DrawAspect" is a "Icon"
            // and output files creates at the same time.
            CompareWorkbookData(excelWb, true, true);
        }

        [Test]
        public void TestEmbeddedPowerPointOoxml()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\Embedded\TestEmbeddedPowerPointOoxml.docx");

            CheckOoxmlObjectBySize(doc, 0, DocxContentType.Pptx, 31138);
            CheckOoxmlObjectBySize(doc, 1, DocxContentType.Pptm, 31161);

            // Although I tried to embed POTX and POTM, MS Word embeds them as OLE objects, not as OOXML packages.
            CheckOleObjectBySize(doc, 2, "Package", ".potx", 31096);
            CheckOleObjectBySize(doc, 3, "Package", ".potm", 32139);

            // Although I tried to embed PPSX and PPSM (PowerPoint Slides), MS Word embeds them as PPTX and PPTM, so be it.
            CheckOoxmlObjectBySize(doc, 4, DocxContentType.Pptx, 31146);
            CheckOoxmlObjectBySize(doc, 5, DocxContentType.Pptm, 31169);
        }

        // FOSS TestSaveEmbeddedOoxmlToDoc removed: it tests how embedded OOXML packages are converted to
        // OLE objects when saving to the removed DOC format.

        private static void CheckOoxmlToOle(Document doc, int shapeIdx)
        {
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, shapeIdx, true);
            Assert.That(shape.OleFormat.OleObject, IsNot.Null());
        }

        /// <summary>
        /// This document contains Word documents in various formats embedded as OLE objects.
        /// </summary>
        [Test]
        public void TestEmbeddedWordOle()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\Embedded\TestEmbeddedWordOle.docx");

            CheckOleObjectBySize(doc, 0, "Word.Document.8", ".doc", 22016);
            // Although I tried to embed DOT, MS Word embeds it as DOC, so be it.
            CheckOleObjectBySize(doc, 1, "Word.Document.8", ".doc", 22016);
            // Although I tried to embed RTF, MS Word embeds it as DOC, so be it.
            CheckOleObjectBySize(doc, 2, "Word.Document.8", ".doc", 22016);
            // Although I tried to embed WML, MS Word embeds it wrapped into a structured storage.
            CheckOleObjectBySize(doc, 3, "Package", ".xml", 10860);
            // Although I tried to embed ODT, MS Word embeds it wrapped into a structured storage.
            CheckOleObjectBySize(doc, 4, "Word.OpenDocumentText.12", ".odt", 4419);
        }

        /// <summary>
        /// This document contains Excel documents in various formats embedded as OLE objects.
        /// </summary>
        [Test]
        public void TestEmbeddedExcelOle()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\Embedded\TestEmbeddedExcelOle.docx");

            CheckOleObjectBySize(doc, 0, "Excel.Sheet.8", ".xls", 18432);
            // Although I tried to embed XLT, MS Word embeds it as XLS, so be it.
            CheckOleObjectBySize(doc, 1, "Excel.Sheet.8", ".xls", 18432);
            // Although I tried to embed SpreadsheetML, MS Word embeds it wrapped into a structured storage.
            CheckOleObjectBySize(doc, 2, "Package", ".xml", 3123);
            // Although I tried to embed ODS, MS Word embeds it wrapped into a structured storage.
            CheckOleObjectBySize(doc, 3, "Excel.OpenDocumentSpreadsheet.12", ".ods", 2834);
            // Although I tried to embed TXT, MS Word embeds it wrapped into a structured storage.
            CheckOleObjectBySize(doc, 4, "Package", ".txt", 5);


            // WORDSNET-9995 XLS workbook is hidden in Excel when extracted from an OLE object.
            string xlsFile = TestUtil.GetInTestOutPath(@"ImportDocx\Embedded\TestEmbeddedExcelOle Out.docx 0 Out.xls");
            FileSystem fs = new FileSystem(xlsFile);
            MemoryStream workbook = fs.Root.GetStreamSafe("Workbook");
            workbook.Position = 0xee;
            int flags = workbook.ReadByte();
            // This flag is 0x39 in the original document, but we reset the hidden bit when exporting, check it.
            Assert.That(flags, Is.EqualTo(0x38));
        }

        /// <summary>
        /// This document contains PowerPoint documents in various formats embedded as OLE objects.
        /// </summary>
        [Test]
        public void TestEmbeddedPowerPointOle()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\Embedded\TestEmbeddedPowerPointOle.docx");

            CheckOleObjectBySize(doc, 0, "PowerPoint.Show.8", ".ppt", 47616);
            // Although I tried to embed POT, MS Word embeds it as PPT, so be it.
            CheckOleObjectBySize(doc, 1, "PowerPoint.Show.8", ".ppt", 47616);
            // Although I tried to embed PPS, MS Word embeds it as PPT, so be it.
            CheckOleObjectBySize(doc, 2, "PowerPoint.Show.8", ".ppt", 47616);

            // Although I tried to embed PowerPoint XML, MS Word embeds it wrapped into a structured storage.
            CheckOleObjectBySize(doc, 3, "Package", ".xml", 89540);
            // Although I tried to embed ODP, MS Word embeds it wrapped into a structured storage.
            CheckOleObjectBySize(doc, 4, "PowerPoint.OpenDocumentPresentation.12", ".odp", 24278);
        }

        /// <summary>
        /// This document contains various other non-MS Office embedded objects.
        /// </summary>
        [Test]
        public void TestEmbeddedVarious()
        {
            Document doc = TestUtil.OpenSaveOpen(@"ImportDocx\Embedded\TestEmbeddedVarious.docx");

            CheckOleObjectBySize(doc, 0, "AcroExch.Document.7", ".pdf", 57066);
            CheckOleObjectBySize(doc, 1, "Package", ".xps", 48261);
            CheckOleObjectBySize(doc, 2, "Package", ".xml", 34484); // FlatOPC
            CheckOleObjectBySize(doc, 3, "Package", ".wps", 5120); // Works
            CheckOleObjectBySize(doc, 4, "Package", ".htm", 21088); // HTML
            CheckOleObjectBySize(doc, 5, "Package", ".mht", 28683); // MHTML
        }

        /// <summary>
        /// WORDSNET-12589 Extracted .xlsx file is not valid.
        /// The XLSX file is wrapped into a structured storage inside a DOC file. If saved as a structured storage,
        /// it cannot be opened in Excel. Added code to unwrap it before saving.
        /// </summary>
        // FOSS TestEmbeddedXlsx removed: the input is a binary DOC (removed load format) and the test is about
        // unwrapping an XLSX from the DOC's structured storage - binary-DOC-specific behavior.



        private static void CheckOoxmlObjectByGold(Document doc, int shapeIdx, string contentType)
        {
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, shapeIdx, true);
            Assert.That(contentType, Is.EqualTo(shape.OleFormat.OoxmlObject.ContentType));

            string outFileName = SaveOleObject(shape, shapeIdx);
            string goldFileName = TestUtil.BuildGoldFileName(outFileName, "", SaveFormat.Unknown);

            TestZipUtil.VerifyFile("Embedded OOXML Object", null, outFileName, goldFileName, null);
        }

        /// <summary>
        /// Saves embedded data of the shape twice - with current time and fixed test time, compares output streams.
        /// </summary>
        /// <param name="excelWb">Shape with embedded Excel workbook.</param>
        /// <param name="isEqual">True - expected that output streams are equal.</param>
        /// <param name="preserveTestMode">True - do not reset test mode while saving embedded object.</param>
        private static void CompareWorkbookData(Shape excelWb, bool isEqual, bool preserveTestMode)
        {
            try
            {
                using (MemoryStream ms1 = new MemoryStream())
                using (MemoryStream ms2 = new MemoryStream())
                {
                    excelWb.OleFormat.Save(ms1);
                    // Reset test mode to get different current time while writing a package.
                    DateTimeUtil.SetTestMode(preserveTestMode);
                    excelWb.OleFormat.Save(ms2);

                    ms1.Position = 0;
                    ms2.Position = 0;

                    // Check streams data.
                    Assert.That(TestUtil.CompareStreams(ms1, ms2), Is.EqualTo(isEqual));
                }
            }
            finally
            {
                // Reset to original value.
                DateTimeUtil.SetTestMode(true);
            }
        }

        private static void CheckOoxmlObjectBySize(Document doc, int shapeIdx, string contentType, int expectedFileSize)
        {
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, shapeIdx, true);
            Assert.That(contentType, Is.EqualTo(shape.OleFormat.OoxmlObject.ContentType));

            string outFileName = SaveOleObject(shape, shapeIdx);
            CompareFileSize(outFileName, expectedFileSize);
        }

        private static void CheckOleObjectBySize(Document doc, int shapeIdx, string progId, string extension, int expectedFileSize)
        {
            Shape shape = (Shape)doc.GetChild(NodeType.Shape, shapeIdx, true);
            Assert.That(shape.OleFormat.ProgId, Is.EqualTo(progId));
            Assert.That(shape.OleFormat.SuggestedExtension, Is.EqualTo(extension));

            string outFileName = SaveOleObject(shape, shapeIdx);
            CompareFileSize(outFileName, expectedFileSize);
        }

        /// <summary>
        /// Compares file sizes with 5% tolerance for platform neutrality.
        ///
        /// Sizes can differ since platform-specific zip libraries are used to compress underlying streams.
        /// </summary>
        private static void CompareFileSize(string outFileName, int expectedFileSize)
        {
            int savedFileSize = TestUtil.GetFileSize(outFileName);
            double difference = System.Math.Abs((savedFileSize - expectedFileSize) / expectedFileSize);

            Assert.That(difference <= 0.05, Is.True, string.Format("Difference is more than 5%! Expected FileSize: {0}, Saved FileSize: {1}, Difference: {2}",                           expectedFileSize, savedFileSize, difference));
        }

        private static string SaveOleObject(Shape shape, int shapeIdx)
        {
            string outFileName = string.Format("{0} {1} Out{2}",
                shape.FetchDocument().OriginalFileName, shapeIdx, shape.OleFormat.SuggestedExtension);
            shape.OleFormat.Save(outFileName);

            return outFileName;
        }



        /// <summary>
        /// WORDSNET-24632 Implement OoxmlObject.GetFileNameForUser().
        /// Previously implementation returned empty string. Added logic to return file name from the package part name.
        /// </summary>
        [Test]
        public void Test24632()
        {
            Document doc = TestUtil.Open(@"ImportDocx\Embedded\Test24632.docx");
            ShapeCollection shapes = doc.FirstSection.Body.Shapes;

            Assert.That(shapes[0].OleFormat.SuggestedFileName, Is.EqualTo("Microsoft_Word_Document"));
            Assert.That(shapes[1].OleFormat.SuggestedFileName, Is.EqualTo("Microsoft_PowerPoint_Presentation"));
            Assert.That(shapes[2].OleFormat.SuggestedFileName, Is.EqualTo("Microsoft_Excel_Worksheet"));

            // Actually in the package file name is "oleObject1". However, AW takes the name from the OLE data.
            Assert.That(shapes[3].OleFormat.SuggestedFileName, Is.EqualTo("enquiry"));
        }

    }
}
