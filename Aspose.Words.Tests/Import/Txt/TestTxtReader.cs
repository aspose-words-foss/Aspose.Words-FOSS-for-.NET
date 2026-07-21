// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2012 by Alexey Butalov

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Aspose.Common;
using Aspose.Words.Fields;
using Aspose.Words.Lists;
using Aspose.Words.Loading;
using Aspose.Words.RW.Txt.Reader;
using Aspose.Words.Saving;
using Aspose.Words.Tests.Import.Docx;
using NUnit.Framework;

namespace Aspose.Words.Tests.Import.Txt
{
    /// <summary>
    /// Tests import of various text files.
    /// </summary>
    [TestFixture]
    public class TestTxtReader
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// WORDSNET-9958 Text document with form feeds/page breaks is not rendering to PDF correctly
        /// </summary>
        [Test]
        public void TestJira9958()
        {
            LoadOptions loadOptions = new LoadOptions();
            loadOptions.LoadFormat = LoadFormat.Text;
            loadOptions.Encoding = Encoding.ASCII;
            Document doc = TestUtil.Open(@"ImportTxt\TestJira9958.txt", loadOptions);
            Assert.That(doc.Sections.Count, Is.EqualTo(4));
        }

        [Test]
        public void TestReadSimpleParagraph()
        {
            Document doc = TestUtil.Open(@"ImportTxt\SimplePara1", LoadFormat.Text);

            CheckParagraph(doc.Sections[0].Body.FirstChild, "Line1 line1 line1\r", 0, 0);
            CheckParagraph(doc.Sections[0].Body.FirstChild.NextSibling, "line2\f", 0, 0);
        }

        [Test]
        public void TestReadSimpleParagraph2()
        {
            Document doc = TestUtil.Open(@"ImportTxt\SimplePara2", LoadFormat.Text);

            Node n1 = doc.Sections[0].Body.FirstChild;
            CheckParagraph(n1, "Line1 line1 line1\r", 0, 2);

            Node n2 = n1.NextSibling;
            CheckParagraph(n2, "line2\r", 0, 0);

            CheckParagraph(n2.NextSibling, "Line3\f", 0, 3);
        }

        [Test]
        public void TestReadSimpleParagraph3()
        {
            Document doc = TestUtil.Open(@"ImportTxt\SimplePara3.txt");

            Node n1 = doc.Sections[0].Body.FirstChild;
            CheckParagraph(n1, "Line1 line1 line1\r", 0, 3);

            Node n2 = n1.NextSibling;
            CheckParagraph(n2, "line2\r", 0, 3);

            CheckParagraph(n2.NextSibling, "Line3 line3 line3\f", 0, 5);
        }

        [Test]
        public void TestReadSimpleParagraph5()
        {
            Document doc = TestUtil.Open(@"ImportTxt\SimplePara4.txt");

            Node n1 = doc.Sections[0].Body.FirstChild;
            CheckParagraph(n1, "Line1 line1 line1\r", 0, 0);

            Node n2 = n1.NextSibling;
            CheckParagraph(n2, "line2\r", 0, 0);

            Node n3 = n2.NextSibling;
            CheckParagraph(n3, "\r", 0, 0);

            Node n4 = n3.NextSibling;
            CheckParagraph(n4, "Line3 line3 line3\r", 0, 0);

            CheckParagraph(n4.NextSibling, "line3\f", 0, 0);
        }

        [Test]
        public void TestReadDefect1()
        {
            Document doc = TestUtil.Open(@"ImportTxt\Defect1", LoadFormat.Text);

            CheckParagraph(doc.Sections[0].Body.FirstChild, null, 0, 12);
        }

        /// <summary>
        /// Some of text documents contain numbering like:
        /// 1. some text1
        /// 2. some text2
        ///   2.1 some text 3  -  dot character is missed after 1 in this numbering
        /// </summary>
        [Test]
        public void TestReadArabicNumberingWithoutTrailingDot()
        {
            Document doc = TestUtil.Open(@"ImportTxt\NumberingWOTrailingDot", LoadFormat.Text);

            Node n1 = doc.Sections[0].Body.FirstChild;
            CheckNumbering(n1, NumberStyle.Arabic, 0, null);
            Assert.That("Line1\r", Is.EqualTo(n1.GetText()));

            Node n2 = n1.NextSibling;
            CheckNumbering(n2, NumberStyle.Arabic, 1, null);
            Assert.That("Line2\r", Is.EqualTo(n2.GetText()));
        }

        /// <summary>
        /// Defect: line of several '*' is detected as bullet
        /// </summary>
        [Test]
        public void TestReadAsteriskLineDelimiter()
        {
            Document doc = TestUtil.Open(@"ImportTxt\AsteriskLineDelimiter", LoadFormat.Text);

            Assert.That(doc.Sections[0].Body.FirstParagraph.IsListItem, Is.False);
        }

        [Test]
        public void TestReadDelimiterLine()
        {
            Document doc = TestUtil.Open(@"ImportTxt\DelimiterLine", LoadFormat.Text);

            Node n1 = doc.Sections[0].Body.FirstChild;
            CheckParagraph(n1, "License\r", 0, 0);

            Node n2 = n1.NextSibling;
            CheckParagraph(n2, "=======\r", 0, 0);

            Node n3 = n2.NextSibling;
            CheckParagraph(n3, "Some text\f", 0, 0);
        }

        [Test]
        public void TestReadBulletWithDifferentIndents()
        {
            Document doc = TestUtil.Open(@"ImportTxt\BulletWithDifferentIndents", LoadFormat.Text);

            Node n1 = doc.Sections[0].Body.FirstParagraph;
            ListFormat f1 = CheckNumbering(n1, NumberStyle.Bullet, 0, null);
            Assert.That("line1\r", Is.EqualTo(n1.GetText()));

            Node p2 = n1.NextSibling;
            ListFormat f2 = CheckNumbering(p2, NumberStyle.Bullet, 0, f1);
            Assert.That("line2\r", Is.EqualTo(p2.GetText()));

            Node p3 = p2.NextSibling;
            ListFormat f3 = CheckNumbering(p3, NumberStyle.Bullet, 0, null);
            Assert.That(f3.ListLevel == f2.ListLevel, Is.False);
            Assert.That("line 3\r", Is.EqualTo(p3.GetText()));

            Node p4 = p3.NextSibling;
            CheckNumbering(p4, NumberStyle.Bullet, 0, f3);
            Assert.That("line 4\f", Is.EqualTo(p4.GetText()));
        }


        [Test]
        public void TestReadNumberingLevels()
        {
            Document doc = TestUtil.Open(@"ImportTxt\NumberingLevels", LoadFormat.Text);

            Node p1 = doc.Sections[0].Body.FirstChild;
            ListFormat f1 = CheckNumbering(p1, NumberStyle.Arabic, 0, null);

            Node p2 = p1.NextSibling;
            ListFormat f2 = CheckNumbering(p2, NumberStyle.Arabic, 1, null);

            Node p3 = p2.NextSibling;
            CheckNumbering(p3, NumberStyle.Arabic, 1, f2);

            Node p4 = p3.NextSibling;
            CheckNumbering(p4, NumberStyle.Arabic, 0, f1);

            Node p5 = p4.NextSibling;
            ListFormat f5 = CheckNumbering(p5, NumberStyle.Arabic, 0, null);
            Assert.That(f5.ListLevel == f1.ListLevel, Is.False);

            Node p6 = p5.NextSibling;
            ListFormat f6 = CheckNumbering(p6, NumberStyle.Arabic, 1, null);
            Assert.That(f6.ListLevel == f2.ListLevel, Is.False);

            Node p7 = p6.NextSibling;
            CheckNumbering(p7, NumberStyle.Arabic, 1, f6);

            Node p8 = p7.NextSibling;
            CheckNumbering(p8, NumberStyle.Arabic, 1, f6);

            Node p9 = p8.NextSibling;
            CheckNumbering(p9, NumberStyle.Arabic, 0, f5);
        }

        [Test]
        public void TestReadNumberingLatin1()
        {
            Document doc = TestUtil.Open(@"ImportTxt\NumberingLatin1", LoadFormat.Text);

            Node p1 = doc.Sections[0].Body.FirstChild;
            ListFormat f1 = CheckNumbering(p1, NumberStyle.LowercaseLetter, 0, null);
            Assert.That("line 1\r", Is.EqualTo(p1.GetText()));

            Node p2 = p1.NextSibling;
            ListFormat f2 = CheckNumbering(p2, NumberStyle.LowercaseLetter, 0, f1);
            Assert.That("line 2\r", Is.EqualTo(p2.GetText()));

            Node p3 = p2.NextSibling;
            CheckNumbering(p3, NumberStyle.LowercaseLetter, 0, f2);
            Assert.That("line 3\f", Is.EqualTo(p3.GetText()));
        }


        /// <summary>
        /// WORDSNET-8072 An ArgumentNullException exception occurs if encoding is null.
        /// Encoding can be null now, it will trigger autodetection.
        /// </summary>
        [Test]
        public void TestJira8072()
        {
            Document doc = new Document();
            using (Stream stream = new FileStream(TestUtil.BuildTestFileName(@"ImportTxt\TestJira8072.txt"), FileMode.Open, FileAccess.Read))
            {
                TxtReader txtReader = new TxtReader(stream, doc, null);
                txtReader.Read();
            }
            Paragraph para = doc.GetChild(NodeType.Paragraph, 0, true) as Paragraph;
            Assert.That(para, IsNot.Null());
            Assert.That(para.GetText(), Is.EqualTo("sometext:<<foo>>\r"));
        }

        /// <summary>
        /// WORDSNET-7677 Line breaks issue in text files.
        /// When a text file is loaded, single line breaks disappear from the output file.
        /// </summary>
        [Test]
        public void TestJira7677()
        {
            Document doc = TestUtil.Open(@"ImportTxt\TestJira7677.txt");

            Node n1 = doc.Sections[0].Body.FirstChild;
            CheckParagraph(n1, "Line 1  dfdf\r", 0, 0);

            Node n2 = n1.NextSibling;
            CheckParagraph(n2, "Line 2 dfdf\r", 0, 0);

            Node n3 = n2.NextSibling;
            CheckParagraph(n3, "\r", 0, 0);

            Node n4 = n3.NextSibling;
            CheckParagraph(n4, "Line 4 fdgffgds\r", 0, 0);

            Node n5 = n4.NextSibling;
            CheckParagraph(n5, "\r", 0,0);

            Node n6 = n5.NextSibling;
            CheckParagraph(n6, "\r",0 ,0);

            Node n7 = n6.NextSibling;
            CheckParagraph(n7, "Line 7 fdgfdgfdg\r", 0, 0);

            Node n8 = n7.NextSibling;
            CheckParagraph(n8, "Line 8\r", 0, 0);

            Node n9 = n8.NextSibling;
            CheckParagraph(n9, "Line 9 sdfhsdkfsd\f", 0, 0);
        }

        /// <summary>
        /// WORDSNET-10456 Txt to Pdf/Doc/Docx/Txt convesion issue with number text.
        /// Search for previous numbered paragraph was performed beyond the last numbered list.
        /// </summary>
        [Test]
        public void TestJira10456()
        {
            Document doc = TestUtil.Open(@"ImportTxt\TestJira10456.txt");

            Node p1 = doc.Sections[0].Body.FirstChild;
            Assert.That("A\r", Is.EqualTo(p1.GetText()));
            ListFormat f1 = CheckNumbering(p1, NumberStyle.Arabic, 0, null);

            Node p2 = p1.NextSibling;
            Assert.That("B\r", Is.EqualTo(p2.GetText()));
            CheckNumbering(p2, NumberStyle.Arabic, 0, f1);

            Node p3 = p2.NextSibling;
            Assert.That("C\r", Is.EqualTo(p3.GetText()));
            CheckNumbering(p3, NumberStyle.Arabic, 0, f1);

            Node p4 = p3.NextSibling;
            Assert.That("D\r", Is.EqualTo(p4.GetText()));
            CheckNumbering(p4, NumberStyle.Arabic, 0, f1);

            Node p5 = p4.NextSibling;
            Assert.That("E\r", Is.EqualTo(p5.GetText()));
            ListFormat f5 = CheckNumbering(p5, NumberStyle.Arabic, 0, null);

            Node p6 = p5.NextSibling;
            Assert.That("F\r", Is.EqualTo(p6.GetText()));
            CheckNumbering(p6, NumberStyle.Arabic, 0, f5);

            Node p7 = p6.NextSibling;
            Assert.That("G\r", Is.EqualTo(p7.GetText()));
            ListFormat f7 = CheckNumbering(p7, NumberStyle.Arabic, 0, null);

            Node p8 = p7.NextSibling;
            Assert.That("H 3. I\r", Is.EqualTo(p8.GetText()));
            CheckNumbering(p8, NumberStyle.Arabic, 0, f7);

            Node p9 = p8.NextSibling;
            Assert.That(p9.GetText(), Is.EqualTo("4. J\r"));
            Paragraph paragraph9 = (Paragraph)p9;
            Assert.That(paragraph9.IsListItem, Is.False);

            Node p10 = p9.NextSibling;
            Assert.That(p10.GetText(), Is.EqualTo("4. K\f"));
            Paragraph paragraph10 = (Paragraph)p10;
            Assert.That(paragraph10.IsListItem, Is.False);
        }

        /// <summary>
        /// WORDSNET-10612 Aspose.Word incorrectly loads floating point values into DOM.
        /// AW incorrectly detects list numbers for ArabicDotTxtNumberingStyle,
        /// if input string after list item number has no any symbols.
        /// </summary>
        [Test]
        public void TestJira10612()
        {
            Document doc = TestUtil.Open(@"ImportTxt\TestJira10612.txt");

            // Check each paragraph in the input file: is it a list number and has it a right text?
            // Input file is the customer one, with some related special cases added.

            Node p0 = doc.Sections[0].Body.FirstChild;
            CheckParagraph(p0, "Test list\r", 0, 0);

            Node p1 = p0.NextSibling;
            Assert.That(p1.GetText(), Is.EqualTo("test 1\r"));
            ListFormat f1 = CheckNumbering(p1, NumberStyle.Arabic, 0, null);

            Node p2 = p1.NextSibling;
            Assert.That(p2.GetText(), Is.EqualTo("test 2\r"));
            CheckNumbering(p2, NumberStyle.Arabic, 0, f1);

            Node p3 = p2.NextSibling;
            Assert.That(p3.GetText(), Is.EqualTo("\r"));
            ListFormat f3 = CheckNumbering(p3, NumberStyle.Arabic, 1, null);

            Node p4 = p3.NextSibling;
            Assert.That(p4.GetText(), Is.EqualTo("\r"));
            CheckNumbering(p4, NumberStyle.Arabic, 1, f3);

            Node p5 = p4.NextSibling;
            Assert.That(p5.GetText(), Is.EqualTo("test 3\r"));
            CheckNumbering(p5, NumberStyle.Arabic, 0, null);

            Node p6 = p5.NextSibling;
            CheckParagraph(p6, "4\r", 0, 0);

            Node p7 = p6.NextSibling;
            CheckParagraph(p7, "2.91\r", 0, 0);

            Node p8 = p7.NextSibling;
            CheckParagraph(p8, "2.91 text\r", 0, 0);

            Node p9 = p8.NextSibling;
            CheckParagraph(p9, "FLOATING POINT NUMBER:\r", 0, 0);

            Node p10 = p9.NextSibling;
            CheckParagraph(p10, "2.91 2.91\r", 0, 0);

            Node p11 = p10.NextSibling;
            CheckParagraph(p11, "2.91\f", 0, 0);
        }



        /// <summary>
        /// WORDSNET-13550 Wrong runs were generated on importing Hebrew text with numbers and
        /// number separators.
        /// </summary>
        [Test]
        public void TestHebrewText()
        {
            LoadOptions options = new LoadOptions();
            options.LoadFormat = LoadFormat.Text;
            options.Encoding = Encoding.GetEncoding("DOS-862");

            Document doc = TestUtil.Open(@"ImportTxt\TestJira13550HebrewDOS862.txt", options);

            CheckBidiRuns(doc, false,
                "01020100104000007.00000000000.00",
                @"רטוקב םיצוק תרדחהו מ""מ 8 רטוקב חודיק תוברל ,תוספרמב תונירטיול תחתמ ןכו םיבוטר םירוזאב תוציחמ תיתחתב מ""ס 20 דע הבוגבו מ""ס 10-20 בחורב 20-ב ןוטב תורוגח");
        }


        /// <summary>
        /// WORDSNET-14858 Provide an option to preserve leading and trailing spaces.
        /// Added new TxtLeadingSpacesOptions and TxtTrailingSpacesOptions enums to control behavior.
        /// </summary>
        [Test]
        public void TestJira14858()
        {
            // Verify leading space options.
            TxtParagraph paragraph = ReadTxtParagraphFromString("   Text   ", TxtLeadingSpacesOptions.Preserve, TxtTrailingSpacesOptions.Preserve);
            VerifyTxtParagraph(paragraph, "   Text   ", 0, null);

            paragraph = ReadTxtParagraphFromString("   Text   ", TxtLeadingSpacesOptions.Trim, TxtTrailingSpacesOptions.Preserve);
            VerifyTxtParagraph(paragraph, "Text   ", 0, null);

            paragraph = ReadTxtParagraphFromString("   Text   ", TxtLeadingSpacesOptions.ConvertToIndent, TxtTrailingSpacesOptions.Preserve);
            VerifyTxtParagraph(paragraph, "Text   ", 3, null);

            // Verify leading space handling with DetectNumbering.
            paragraph = ReadTxtParagraphFromString("   1) Text   ", TxtLeadingSpacesOptions.ConvertToIndent, TxtTrailingSpacesOptions.Preserve);
            VerifyTxtParagraph(paragraph, "Text   ", 6, "1)");

            paragraph = ReadTxtParagraphFromString("   1) Text   ", TxtLeadingSpacesOptions.Preserve, TxtTrailingSpacesOptions.Preserve);
            VerifyTxtParagraph(paragraph, "   1) Text   ", 0, null);

            paragraph = ReadTxtParagraphFromString("1) Text   ", TxtLeadingSpacesOptions.Preserve, TxtTrailingSpacesOptions.Preserve);
            VerifyTxtParagraph(paragraph, "Text   ", 3, "1)");

            // Verify trailing space options.
            paragraph = ReadTxtParagraphFromString("   Text   ", TxtLeadingSpacesOptions.Preserve, TxtTrailingSpacesOptions.Trim);
            VerifyTxtParagraph(paragraph, "   Text", 0, null);

            paragraph = ReadTxtParagraphFromString("   Text   ", TxtLeadingSpacesOptions.Trim, TxtTrailingSpacesOptions.Trim);
            VerifyTxtParagraph(paragraph, "Text", 0, null);

            paragraph = ReadTxtParagraphFromString("   Text   ", TxtLeadingSpacesOptions.ConvertToIndent, TxtTrailingSpacesOptions.Trim);
            VerifyTxtParagraph(paragraph, "Text", 3, null);
        }

        /// <summary>
        /// WORDSNET-15894 UTF8 encoding Text to PDF rendering issue
        /// WORDSNET-15720 DOCX to PDF conversion issue with Khmer text
        /// Check mongolian, ethiopic, runic, russian, thai, khmer fonts in docx from txt
        /// </summary>
        [Test]
        public void TestJira15894()
        {
            const string testString = "aᠠᠡᠢᠣᠤᠥᠦᠧᠨᠩb\nካለኝ\nᚳᚹᚫ\nо́и́у́\nฉันกินกระจกได้\nត្រូវបានដាក់នៅលើសន្លឹក";
            Document doc;

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(testString)))
                doc = new Document(stream, new LoadOptions(LoadFormat.Text, null, null));

            SaveOptions so = SaveOptions.CreateSaveOptions(SaveFormat.Docx);
            so.UpdateAmbiguousTextFont = true;
            // Start validator to set fonts
            TestUtil.ExecuteValidator(doc, so);

            // Check fonts in document
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(6));

            // Check combined fonts
            RunCollection runs = paras[0].Runs;
            Assert.That(runs.Count, Is.EqualTo(3));
            Assert.That(runs[0].RunPr.NameAscii, Is.EqualTo("Courier New"));
            Assert.That(runs[1].RunPr.NameAscii, Is.EqualTo("Mongolian Baiti"));
            Assert.That(runs[2].RunPr.NameAscii, Is.EqualTo("Courier New"));

            // Check different fonts on each line
            Assert.That(paras[1].Runs[0].RunPr.NameAscii, Is.EqualTo("Nyala"));
            Assert.That(paras[2].Runs[0].RunPr.NameAscii, Is.EqualTo("Segoe UI Historic"));
            Assert.That(paras[3].Runs[0].RunPr.NameAscii, Is.EqualTo("Courier New"));
            Assert.That(paras[4].Runs[0].RunPr.NameAscii, Is.EqualTo("Angsana New"));
            Assert.That(paras[5].Runs[0].RunPr.NameAscii, Is.EqualTo("DaunPenh"));
        }

        /// <summary>
        /// WORDSNET-16047 Japanese text is imported incorrectly into DOM.
        /// Check correct japanese text determination from ASCII file.
        /// </summary>
        [Test]
        public void TestJira16047()
        {
            Document doc = TestUtil.Open(@"ImportTxt\TestJira16047.txt");
            Assert.That(doc.FirstSection.Body.FirstParagraph.FirstRun.Text, Is.EqualTo("頎凞"));
        }

        /// <summary>
        /// WORDSNET-16496 Incorrectly detects sinhalese and ethiopic (Amharic) languages and fonts for it.
        /// Checks ethiopic and sinhalese fonts and languages in PDF document loaded from txt.
        /// </summary>
        [Test]
        public void TestJira16496()
        {
            const string testString = "ሰማይ හෙලෝ";
            Document doc;

            using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(testString)))
                doc = new Document(stream, new LoadOptions(LoadFormat.Text, null, null));

            SaveOptions so = SaveOptions.CreateSaveOptions(SaveFormat.Markdown);
            so.UpdateAmbiguousTextFont = true;
            // Start validator to set fonts
            TestUtil.ExecuteValidator(doc, so);

            // Check fonts in document
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras.Count, Is.EqualTo(1));

            // Check if run properties are correct
            RunCollection runs = paras[0].Runs;

            Assert.That(runs.Count, Is.EqualTo(3));

            Assert.That(runs[0].RunPr.NameAscii, Is.EqualTo("Nyala"));
            Assert.That(runs[0].RunPr.ComplexScript.ToBool(), Is.False);
            Assert.That((Language)runs[0].RunPr.LocaleIdBi, Is.EqualTo(Language.NoProof));

            Assert.That(runs[2].RunPr.NameAscii, Is.EqualTo("Iskoola Pota"));
            Assert.That(runs[2].RunPr.ComplexScript.ToBool(), Is.True);
            Assert.That((Language)runs[2].RunPr.LocaleIdBi, Is.EqualTo(Language.Sinhalese));
        }

        /// <summary>
        /// WORDSNET-17877 PageSetup.PaperSize returns Letter for TXT document.
        /// PageSetup.PaperSize depends on locale. Added definition by locale.
        /// </summary>
        [TestCase("ru-RU", PaperSize.A4)]
        [TestCase("en-NZ", PaperSize.A4)]
        [TestCase("ja-JP", PaperSize.A4)]
        [TestCase("de-DE", PaperSize.A4)]
        [TestCase("en-US", PaperSize.Letter)]
        [NonParallelizable]
        public void Test17877(string culture, PaperSize expectedPaperSize)
        {
            string currentCulture = SystemPal.GetCurrentCultureName();
            bool testMode = SectPr.TestMode;

            try
            {
                SectPr.TestMode = false;
                SystemPal.SetCulture(culture);

                Document doc = TestUtil.Open(@"ImportTxt\Test17877.txt");
                PaperSize actualPaperSize = doc.FirstSection.PageSetup.PaperSize;

                Assert.That(actualPaperSize, Is.EqualTo(expectedPaperSize));
            }
            finally
            {
                SystemPal.SetCulture(currentCulture);
                SectPr.TestMode = testMode;
            }
        }

        /// <summary>
        /// WORDSNET-19067 Add feature to detect text direction (RTL / LTR) while import TXT document.
        /// A new load option DocumentDirection was introduced with one of the possible values Auto.
        /// </summary>
        [TestCase("Test19067Hebrew.txt", true)]
        [TestCase("Test19067Arabic.txt", true)]
        [TestCase("Test19067English.txt", false)]
        [TestCase("Test19067Russian.txt", false)]
        [TestCase("Test19067Mixed.txt", true)]
        public void Test19067(string testName, bool expectedAutoBidi)
        {
            string testFile = string.Format(@"ImportTxt\{0}", testName);

            DocumentDirection[] documentDirections =
                new DocumentDirection[] {DocumentDirection.LeftToRight, DocumentDirection.RightToLeft, DocumentDirection.Auto};

            foreach (DocumentDirection direction in documentDirections)
            {
                TxtLoadOptions lo = new TxtLoadOptions();
                lo.DocumentDirection = direction;

                Document doc = TestUtil.Open(testFile, lo);
                bool isBidi = doc.FirstSection.Body.FirstParagraph.ParaPr.Bidi;

                string assertMessage = string.Format("DocumentDirection = {0}", direction);
                switch (direction)
                {
                    case DocumentDirection.LeftToRight:
                        Assert.That(isBidi, Is.False, assertMessage);
                        break;
                    case DocumentDirection.RightToLeft:
                        Assert.That(isBidi, Is.True, assertMessage);
                        break;
                    case DocumentDirection.Auto:
                    {
                        Assert.That(isBidi, Is.EqualTo(expectedAutoBidi), assertMessage);
                        break;
                    }
                    default:
                        throw new InvalidOperationException("Unexpected document direction.");
                }
            }
        }

        /// <summary>
        /// WORDSNET-20378 Extra paragraph is created when single line TXT document is imported into Document.
        /// </summary>
        [Test]
        public void Test20378()
        {
            Document doc = TestUtil.Open(@"ImportTxt\Test20378.txt");
            Assert.That(doc.FirstSection.Body.Count, Is.EqualTo(1));
        }



        /// <summary>
        /// WORDSJAVA-2579 GetText() method returns text plus a list bullet when working with .txt documents consisting of a third level marked list
        /// </summary>
        [Test]
        public void TestJava2579()
        {
            const string testString = "1. Item 1\r\n   a. Item 2\r\n      i. Item 3\r\n";
            Document doc;

            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(testString)))
                doc = new Document(stream, new LoadOptions(LoadFormat.Text, null, null));

            string docText = doc.GetText().Trim();

            Assert.That(docText, Is.EqualTo("Item 1\rItem 2\rItem 3"));
        }



        /// <summary>
        /// Relates to WORDSNET-23562
        /// Tests another customer`s input document.
        /// </summary>
        [TestCase(true)]
        [TestCase(false)]
        public void Test23562A(bool autoNumberingDetection)
        {
            TxtLoadOptions options = new TxtLoadOptions { AutoNumberingDetection = autoNumberingDetection};
            Document doc = TestUtil.Open(@"ImportTxt\Test23562A.txt", options);

            Paragraph para = TestUtil.GetParagraphWithText(doc.FirstSection.Body, "...cm....");
            Assert.That(para.IsListItem, Is.EqualTo(autoNumberingDetection));
        }


        /// <summary>
        /// Relates to WORDSNET-24408
        /// Checks that .docx, .odt and .sxw formats don't fallback to .txt format when they are detected as unknown.
        /// </summary>
        [TestCase("Test24408.docx")]
        [TestCase("Test24408.odt")]
        [TestCase("Test24408.sxw")]
        [Test, ExpectedException(typeof(UnsupportedFileFormatException))]
        public void Test24408A(string testName)
        {
            TestUtil.Open(string.Format(@"ImportTxt\{0}", testName));
        }


        /// <summary>
        /// Relates to WORDSNET-25529
        /// Tests <see cref="TxtLoadOptions.HyperlinksColor"/> and
        /// <see cref="TxtLoadOptions.HyperlinksUnderline"/> options.
        /// </summary>
        [Test]
        public void Test25529A()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                const string text = "http://link.com";
                stream.Write(Encoding.ASCII.GetBytes(text), 0, text.Length);

                // Check default values.
                TxtLoadOptions loadOptions = new TxtLoadOptions() { DetectHyperlinks = true };
                Document doc = new Document(stream, loadOptions);

                Run run = TestUtil.GetRunWithText(doc, text);
                Assert.That(run.Font.Color.ToArgb(), Is.EqualTo(Color.Blue.ToArgb()));
                Assert.That(run.Font.Underline, Is.EqualTo(Underline.Single));

                // Check set custom values.
                loadOptions.HyperlinksColor = Color.Red;
                Assert.That(Color.Red, Is.EqualTo(loadOptions.HyperlinksColor));

                loadOptions.HyperlinksUnderline = Underline.Double;
                Assert.That(Underline.Double, Is.EqualTo(loadOptions.HyperlinksUnderline));

                doc = new Document(stream, loadOptions);

                run = TestUtil.GetRunWithText(doc, text);
                Assert.That(run.Font.Color.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
                Assert.That(run.Font.Underline, Is.EqualTo(Underline.Double));
            }
        }

        /// <summary>
        /// Copies specified file into a MemoryStream.
        /// </summary>
        /// <remarks>
        /// Not used StreamUtil.CopyFileToStream() here, to mimic customer's scenario exactly.
        /// </remarks>
        private static MemoryStream CopyFileToMemoryStream(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                MemoryStream ms = new MemoryStream();
                fs.CopyTo(ms);
                ms.Position = 0;

                return ms;
            }
        }

        /// <summary>
        /// Returns array of all list items text in specified document.
        /// </summary>
        private static IList<string> GetArrayOfListItemsText(Document doc)
        {
            List<string> listItemsText = new List<string>();
            NodeCollection paras = doc.GetChildNodes(NodeType.Paragraph, true);
            foreach (Paragraph para in paras)
            {
                if (para.IsListItem)
                    listItemsText.Add(para.GetText());
            }

            return listItemsText;
        }

        /// <summary>
        /// Combines text of continuous runs with same Rtl (Bidi) flag and checks them by comparing with the expected
        /// run text array.
        /// </summary>
        private static void CheckBidiRuns(Document doc, bool expectedIsFirstRtl, params string[] expectedRunText)
        {
            NodeCollection runs = doc.FirstSection.Body.GetChildNodes(NodeType.Run, true);
            Assert.That(runs.Count, Is.GreaterThan(0), "Run count must be more than 0.");
            bool isRtl = ((Run)runs[0]).Font.Bidi;
            Assert.That(isRtl, Is.EqualTo(expectedIsFirstRtl), "Wrong RTL value.");

            List<string> actualRunText = new List<string>();
            StringBuilder currentText = new StringBuilder();
            foreach (Run run in runs)
            {
                if (run.Font.Bidi != isRtl)
                {
                    actualRunText.Add(currentText.ToString());
                    currentText.Length = 0;
                    isRtl = !isRtl;
                }
                currentText.Append(run.Text);
            }
            actualRunText.Add(currentText.ToString());

            Assert.That(actualRunText.Count, Is.EqualTo(expectedRunText.Length), "Wrong run count.");
            for (int i = 0; i < expectedRunText.Length; i++)
                Assert.That(actualRunText[i].Trim(), Is.EqualTo(expectedRunText[i].Trim()), "Wrong run text.");
        }

        private static ListFormat CheckNumbering(Node node,
            NumberStyle numberStyle,
            int listLevelNumber,
            ListFormat previousSibling)
        {
            Assert.That(node, Is.InstanceOf(typeof(Paragraph)));
            Paragraph p = (Paragraph)node;
            Assert.That(p.IsListItem, Is.True);
            Assert.That(p.ListFormat.ListLevelNumber, Is.EqualTo(listLevelNumber));
            Assert.That(p.ListFormat.ListLevel.NumberStyle, Is.EqualTo(numberStyle));
            if (previousSibling != null)
            {
                Assert.That(p.ListFormat.ListId, Is.EqualTo(previousSibling.ListId));
                Assert.That(p.ListFormat.ListLevelNumber, Is.EqualTo(previousSibling.ListLevelNumber));
            }

            return p.ListFormat;
        }

        private static void CheckParagraph(Node node,
            string paragraphText,
            int expectedLeftIndent,
            int expectedFirstLineIndent)
        {
            Assert.That(node, Is.InstanceOf(typeof(Paragraph)));
            Paragraph p = (Paragraph)node;
            if (paragraphText != null)
                Assert.That(p.GetText(), Is.EqualTo(paragraphText));
            TxtReader txtReader = new TxtReader(new MemoryStream(), new Document(), null);
            Assert.That(System.Math.Round(p.ParagraphFormat.LeftIndent, 0), Is.EqualTo(System.Math.Round(txtReader.GetIndentInPoints(expectedLeftIndent), 0)));
            Assert.That(System.Math.Round(p.ParagraphFormat.FirstLineIndent), Is.EqualTo(System.Math.Round(txtReader.GetIndentInPoints(expectedFirstLineIndent), 0)).Within(0));
        }

        /// <summary>
        /// Reads a text from a string and returns a TxtParagraph.
        /// </summary>
        private static TxtParagraph ReadTxtParagraphFromString(string text, TxtLeadingSpacesOptions leading, TxtTrailingSpacesOptions trailing)
        {
            StreamReader reader = new StreamReader(new MemoryStream(Encoding.ASCII.GetBytes(text)));

            TxtLoadOptions loadOptions = new TxtLoadOptions();
            loadOptions.LeadingSpacesOptions = leading;
            loadOptions.TrailingSpacesOptions = trailing;

            TxtParagraphReader paragraphReader = new TxtParagraphReader(reader, loadOptions);
            TxtParagraph[] paragraphs = paragraphReader.Read();

            return paragraphs[0];
        }

        /// <summary>
        /// Verifies TxtParagraph properties.
        /// </summary>
        private static void VerifyTxtParagraph(TxtParagraph para, string expectedText, int expectedFirstLineIndent, string numberingText)
        {
            Assert.That(para.GetText(), Is.EqualTo(expectedText));
            Assert.That(para.FirstLineIndent, Is.EqualTo(expectedFirstLineIndent));
            if (numberingText == null)
            {
                Assert.That(para.Numbering, Is.Null);
            }
            else
            {
                Assert.That(para.Numbering.Text, Is.EqualTo(numberingText));
            }
        }
    }
}
