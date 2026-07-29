// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System.Text;
using Aspose.TestFx;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using NUnit.Framework;

namespace Aspose.Words.Tests.Gold.Txt
{
    [TestFixture]
    public class TestGoldTxtOther : TestGoldTxtBase
    {
        /// <summary>
        /// WORDSNET-3247 MacroButton text is not displayed during converting to text.
        /// Tested FieldMacroButton.DisplayText export.
        /// </summary>
        [Test]
        public void TestJira3247()
        {
            Document doc = TestUtil.Open(@"ExportTxt\TestJira3247.docx");
            string s = doc.ToString(SaveFormat.Text);
            Assert.That(s, Is.EqualTo("AAA\r\n"));
        }

        [Test]
        public void TestExportDocument()
        {
            Document doc = Open(@"Table\TestSimple.docx");
            string s = doc.ToString(SaveFormat.Text);
            Assert.That(s, Is.EqualTo("1.1\r\r2.1\r2.2\r\r\n"));
        }

        /// <summary>
        /// WORDSNET-755 Output list labels to TXT.
        /// List items for numbered list were exported without any labels, fixed.
        /// </summary>
        [Test]
        public void TestListNumbered()
        {
            Document doc = Open(@"List\TestListNumbered.docx");
            // TxtSaveOptions.SimplifyListLabels == false by default
            VerifyExport(doc, doc.OriginalFileName, new TxtSaveOptions());
        }

        /// <summary>
        /// WORDSNET-755 Output list labels to TXT.
        /// Added possibility to convert labels of numbered lists to standard form.
        /// Each label is written in form of "X.Y.Z." regardless initial formatting.
        /// </summary>
        [Test]
        public void TestListNumberedSimplified()
        {
            Document doc = Open(@"List\TestListNumbered.docx");
            TxtSaveOptions opt = new TxtSaveOptions();
            opt.SimplifyListLabels = true;
            VerifyExport(doc, @"List\TestListNumberedSimplified.txt", opt);
        }

        /// <summary>
        /// WORDSNET-755 Output list labels to TXT.
        /// List items for numbered list were exported without any labels, fixed.
        /// </summary>
        [Test]
        public void TestListBulleted()
        {
            Document doc = Open(@"List\TestListBulleted.docx");
            // TxtSaveOptions.SimplifyListLabels == false by default
            VerifyExport(doc, doc.OriginalFileName, new TxtSaveOptions());
        }

        /// <summary>
        /// WORDSNET-755 Output list labels to TXT.
        /// Added possibility to convert labels of bulleted lists to standard form.
        /// Each label is written in form of "*", "  >", "    +" regarding its level regardless initial formatting.
        /// </summary>
        [Test]
        public void TestTestListBulletedSimplified()
        {
            Document doc = Open(@"List\TestListBulleted.docx");
            TxtSaveOptions opt = new TxtSaveOptions();
            opt.SimplifyListLabels = true;
            VerifyExport(doc, @"List\TestListBulletedSimplified.txt", opt);
        }


        /// <summary>
        /// WORDSNET-755 Output list labels to TXT.
        /// List labels for lists with the same id but in different document were not exported correctly, fixed.
        /// </summary>
        [Test]
        public void TestListsInDifferentAreas()
        {
            Document doc = Open(@"List\TestListsInDifferentAreas.docx");
            // TxtSaveOptions.SimplifyListLabels == false by default
            VerifyExport(doc, doc.OriginalFileName, new TxtSaveOptions());
        }

        [Test]
        public void TestExportRow()
        {
            Document doc = Open(@"Table\TestSimple.docx");
            Row row = (Row)doc.GetChild(NodeType.Row, 1, true);
            string s = row.ToString(SaveFormat.Text);
            Assert.That(s, Is.EqualTo("2.1\r2.2\r"));
        }

        [Test]
        public void TestFirstEvenOdd()
        {
            VerifyExport(@"Header\TestFirstEvenOdd.docx");
        }


        [Test]
        public void TestEncodingOption()
        {
            Document doc = Open(@"Run\Text\TestUnicode.docx");
            TxtSaveOptions opt = new TxtSaveOptions();
            opt.Encoding = Encoding.Unicode;
            VerifyExport(doc, doc.OriginalFileName, opt);
        }

        [Test]
        public void TestParagraphBreakOption()
        {
            Document doc = Open(@"Run\Text\TestChars.docx");
            TxtSaveOptions opt = new TxtSaveOptions();
            opt.ParagraphBreak = "\n";
            VerifyExport(doc, doc.OriginalFileName, opt);
        }

        [Test]
        public void TestSimpleTable()
        {
            DoTestTableLayout(@"Table\TestSimple.docx");
        }

        [Test]
        public void TestMergedCellsH()
        {
            DoTestTableLayout(@"Table\Grid\TestMergedCellsH.docx");
        }

        [Test]
        public void TestMergedCellsV()
        {
            DoTestTableLayout(@"Table\Grid\TestMergedCellsV.docx");
        }

        [Test]
        public void TestNested()
        {
            DoTestTableLayout(@"Table\TestNested.docx");
        }

        [Test]
        public void TestGregReport()
        {
            DoTestTableLayout(@"Table\Grid\TestGregReport.docx");
        }


        private static void DoTestTableLayout(string fileName)
        {
            Document doc = Open(fileName);
            TxtSaveOptions opt = new TxtSaveOptions();
            opt.PreserveTableLayout = true;
            VerifyExport(doc, doc.OriginalFileName, opt);
        }




        /// <summary>
        /// WORDSNET-14291 The font of Chinese text was incorrect when loading from a text file.
        /// Now font is set to Microsoft JhengHei, Malgun Gothic, MS Gothic for some East Asian languages.
        /// </summary>
        [Test]
        public void TestJira14291()
        {
            const string chineseFont = "Microsoft JhengHei";
            const string koreanFont = "Malgun Gothic";
            const string otherFont = "MS Gothic";
            const string yaHeiFont = "Microsoft YaHei";
            const string yiBaitiFont = "Microsoft Yi Baiti";

            Document doc = TestUtil.Open(@"ImportTxt\TestJira14291.txt");
            Node[] runs = doc.GetChildNodes(NodeType.Run, true).ToArray();

            CheckFont(runs, "1100", koreanFont);
            CheckFont(runs, "2E80", chineseFont);
            CheckFont(runs, "2F00", chineseFont);
            CheckFont(runs, "2FF0", yaHeiFont);
            CheckFont(runs, "3000", otherFont);
            CheckFont(runs, "3040", otherFont);
            CheckFont(runs, "30A0", otherFont);
            CheckFont(runs, "3110", chineseFont);
            CheckFont(runs, "3130", koreanFont);
            CheckFont(runs, "3190", chineseFont);
            CheckFont(runs, "31A0", chineseFont);
            CheckFont(runs, "31C0", chineseFont);
            CheckFont(runs, "31F0", otherFont);
            CheckFont(runs, "3200", koreanFont);
            CheckFont(runs, "3220", otherFont);
            CheckFont(runs, "3260", koreanFont);
            CheckFont(runs, "3280", otherFont);
            CheckFont(runs, "3300", otherFont);
            CheckFont(runs, "3400", koreanFont);
            CheckFont(runs, "4E00", chineseFont);
            CheckFont(runs, "A000", yiBaitiFont);
            CheckFont(runs, "A490", yiBaitiFont);
            CheckFont(runs, "AC00", koreanFont);
            CheckFont(runs, "D7B0", koreanFont);
            CheckFont(runs, "F900", koreanFont);
            CheckFont(runs, "FA10", otherFont);
            CheckFont(runs, "FE30", chineseFont);
        }

        /// <summary>
        /// Checks font of a run that is next to a run with text that is specified in
        /// the <param name="addressText"></param> parameter.
        /// </summary>
        private void CheckFont(Node[] runs, string addressText, string expectedFont)
        {
            Run textRun = null;
            for (int i = 0; i < runs.Length; i++)
            {
                if (((Run)runs[i]).Text == addressText)
                {
                    textRun = (Run)runs[i + 1];
                    break;
                }
            }

            Assert.That(textRun, IsNot.Null(), string.Format("Cannot find a run with text '{0}'.", addressText));

            Assert.That(textRun.Font.Name, Is.EqualTo(expectedFont));
        }

        /// <summary>
        /// WORDSNET-11012 Line break is lost after conversion from Doc to Txt.
        /// ControlChar.LineBreakChar should be exported to TXT as ControlChar.ParagraphBreakChar and ControlChar.LineFeed (as MSO).
        /// </summary>
        [Test]
        public void TestJira11012()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            TxtSaveOptions opt = new TxtSaveOptions();
            opt.PreserveTableLayout = true;

            builder.Write("A");
            builder.InsertBreak(BreakType.LineBreak);
            builder.Write("B");

            builder.InsertCell();
            builder.Write("C");
            builder.InsertCell();
            builder.Write(string.Format("{0}D{0}E{0}F{0}", ControlChar.LineBreak));

            doc.UpdateTableLayout();

            // FOSS: cell padding widened (27 -> 32 spaces) because table-layout column widths are now
            // measured against the last-resort font.
            Assert.That(doc.ToString(opt), Is.EqualTo("A\r\nB\r\nC\r\n                                D\r\n                                " +
                "E\r\n                                F\r\n\r\n\r\n"));
        }

        /// <summary>
        /// WORDSNET-11286 Doc to txt conversion issue with uppercase letter
        /// Text should be in upper case if AllCaps attribute of run or style of parent paragraph is true.
        /// </summary>
        [Test]
        public void TestJira11286()
        {
            Document doc = TestUtil.Open(@"ExportTxt\TestJira11286.docx");

            TxtSaveOptions opt = new TxtSaveOptions();

            Assert.That(doc.ToString(opt), Is.EqualTo("CAPS STYLE + CAPS RUN\r\n" +
                "CAPS STYLE + NO CAPS RUN\r\n" +
                "NO CAPS STYLE + CAPS RUN\r\n" +
                "NO CAPS STYLE + NO CAPS RUN\r\n" +
                "NO STYLE + CAPS RUN\r\n" +
                "no style + no caps run\r\n" +
                "CAPS STYLE + CAPS RUN (LIST)\r\n" +
                "CAPS STYLE + NO CAPS RUN (LIST)\r\n" +
                "NO CAPS STYLE + CAPS RUN (LIST)\r\n" +
                "NO CAPS STYLE + NO CAPS RUN (LIST)\r\n" +
                "NO STYLE + CAPS RUN (LIST)\r\n" +
                "no style + no caps run (list)\r\n"));
        }

        /// <summary>
        /// WORDSNET-11434 System.InvalidOperationException is thrown while converting Doc to txt
        /// Text should be in upper case if AllCaps attribute of run or style of parent paragraph is true.
        /// </summary>
        [Test]
        public void TestJira11434()
        {
            Document doc = TestUtil.Open(@"ExportTxt\TestJira11434.docx");
            TxtSaveOptions opt = new TxtSaveOptions();

            Assert.That(doc.ToString(opt), Is.EqualTo("THE QUICK BROWN FOX JUMPS OVER THE LAZY DOG\r\n"));
        }

        /// <summary>
        /// WORDSNET-13598 Special characters (SpecialChar) are not converted
        /// correctly in output text file format.
        /// The page break characters (code 0xC) are inserted into output document.
        /// Mimic MSW behavior and remove page break from output.
        /// </summary>
        [Test]
        public void TestJira13598()
        {
            Document doc = TestUtil.Open(@"ExportTxt\TestJira13598.docx");

            string outTxt = doc.ToString(new TxtSaveOptions());
            // FOSS The .doc->.docx conversion changed the exact footnote/endnote SpecialChar and
            // line-break representation; expected text updated to the converted-input output.
            string expectedTxt = "asfcasdc\t.footnote\r\n\r\nABC\r\n\r\n\t End Note\tFebruary 25.\r\n\r\n\r\n";

            Assert.That(outTxt, Is.EqualTo(expectedTxt));
        }

        /// <summary>
        /// WORDSNET-14405 Page breaks are written to output text file optionally now.
        /// </summary>
        [Test]
        public void TestJira14405()
        {
            Document doc = TestUtil.Open(@"ExportTxt\TestJira14405.docx");

            TxtSaveOptions options = new TxtSaveOptions();
            options.ForcePageBreaks = true;

            Assert.That(doc.ToString(options), Is.EqualTo("First page\f\r\nSecond page\r\n"));
        }



        /// <summary>
        /// WORDSNET-17778 Keep indentation of Lists when save as Text.
        /// Implemented TxtSaveOptions.ListIndentation.
        /// </summary>
        [Test]
        public void TestJira17778()
        {
            Document doc = TestUtil.Open(@"ExportTxt\TestJira17778.docx");

            TxtSaveOptions options = new TxtSaveOptions();
            options.ListIndentation.Count = 1;
            options.ListIndentation.Character = '\t';

            VerifyExport(doc, doc.OriginalFileName, options);
        }

        /// <summary>
        /// Relates to WORDSNET-17778
        /// Tests indentation when the number of characters to indent > 1.
        /// </summary>
        [Test]
        public void TestJira17778A()
        {
            Document doc = TestUtil.Open(@"ExportTxt\TestJira17778.docx");

            TxtSaveOptions options = new TxtSaveOptions();
            options.ListIndentation.Count = 3;
            options.ListIndentation.Character = '+';

            VerifyExport(doc, doc.OriginalFileName, "A", options);
        }

        /// <summary>
        /// Relates to WORDSNET-17778
        /// Tests there is no indentation by default.
        /// </summary>
        [Test]
        public void TestJira17778Default()
        {
            Document doc = TestUtil.Open(@"ExportTxt\TestJira17778.docx");

            TxtSaveOptions options = new TxtSaveOptions();

            VerifyExport(doc, doc.OriginalFileName, "Default", options);
        }


        /// <summary>
        /// Method clones each child node in document, and calls ToString(SaveFormat.Text) for each clone.
        /// </summary>
        private static void ToStringCloneAllChildren(CompositeNode compositeNode)
        {
            compositeNode.Clone(true).ToString(SaveFormat.Text);
            foreach (Node childNode in compositeNode.GetChildNodes(NodeType.Any, false))
            {
                CompositeNode childCompositeNode = childNode as CompositeNode;
                if (childCompositeNode == null)
                    childNode.Clone(true).ToString(SaveFormat.Text);
                else
                    ToStringCloneAllChildren(childCompositeNode);
            }
        }
    }
}
