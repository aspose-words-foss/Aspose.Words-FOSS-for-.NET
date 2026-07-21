// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Xml;
using Aspose.Bidi;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.TestFx;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Fields;
using Aspose.Words.Fonts;
using Aspose.Words.Lists;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using Aspose.Words.Tests.Export.Docx;
using Aspose.Xml;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for how character formatting is read and parsed and saved.
    /// </summary>
    [TestFixture]
    public class TestRuns : UnifiedTestsBase
    {
        /// <summary>
        /// Test that non English characters and special characters a preserved.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestChars(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Run\Text\TestChars", lf, sf);

            Paragraph para = (Paragraph)doc.GetChild(NodeType.Paragraph, 1, true);
            Assert.That(para.GetText(), Is.EqualTo("А Җ ' \" <> {xxx} ‘ “ & < > £ ¥ § ¨ © ª « ® » ¼ \\ \\\\\r"));

            // Empty paragraph.
            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 2, true);
            Assert.That(para.GetText(), Is.EqualTo("\r"));
            Assert.That(para.ParagraphBreakFont.Size, Is.EqualTo(24));


            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 4, true);
            // In all other formats we simply get them as characters.
            Assert.That(para.GetText(), Is.EqualTo("\xf075 \xf0a8\r"));
            Assert.That(para.GetChildNodes(NodeType.Run, false).Count, Is.EqualTo(3));

            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 10, true);
            Assert.That(para.GetText(), Is.EqualTo("tab1\ttab2\ttab3\r"));
            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 11, true);
            Assert.That(para.GetText(), Is.EqualTo(" “double opening and closing quotes”\r"));
            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 12, true);
            Assert.That(para.GetText(), Is.EqualTo("‘single opening and closing quotes’\r"));

            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 14, true);
            Assert.That(para.GetText(), Is.EqualTo("em\x2014dash and em\x2003space\r"));
            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 15, true);
            Assert.That(para.GetText(), Is.EqualTo("en\x2013dash and en\x2002space\r"));

            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 17, true);
            Assert.That(para.GetText(), Is.EqualTo("nonbreaking\x00a0space\r"));
            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 18, true);
            Assert.That(para.GetText(), Is.EqualTo("one fourths em\x2005space\r"));

            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 20, true);
            Assert.That(para.GetText(), Is.EqualTo("non\x001ebreaking hyphen\r")); // Note this is not a Unicode char.
            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 21, true);
            Assert.That(para.GetText(), Is.EqualTo("op\x001ftional hyphen\r")); // Note this is not a Unicode char.

            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 23, true);
            Assert.That(para.GetText(), Is.EqualTo("left-to-right mark\x200etest\x200fright-to-left mark\r"));

            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 25, true);
            Assert.That(para.GetText(), Is.EqualTo("zero-width non-joiner\x200ca.k.a No-Width Optional Break\r"));
            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 26, true);
            Assert.That(para.GetText(), Is.EqualTo("zero-width joiner\x200da.k.a No-Width Non Break\r"));

            // Checks that \r (carriage return) is possible in text (not as a paragraph break).
            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 28, true);

            if (lf == LoadFormat.Rtf)
            {
                // Original file RTF saved from DOC.
                // RK MS Word cannot save carriage returns that are not paragraph breaks into RTF.
                Assert.That(para.GetText(), Is.EqualTo("Line 1 ends with a carriage return, not a paragraph break.Line 2.\r"));
            }
            else if (sf == SaveFormat.Rtf)
            {
                // Original file is DOC.
                // WORDSNET-3480 Paragraph is split at carriage return. Line2 goes to separate paragraph.
                Assert.That(para.GetText(), Is.EqualTo("Line 1 ends with a carriage return, not a paragraph break.\r"));
            }
            else
            {
                // RK This is our (more correct) way.
                Assert.That(para.GetText(), Is.EqualTo("Line 1 ends with a carriage return, not a paragraph break.\rLine 2.\r"));
            }

            // Because paragraph could be split we need to adjust below text retrieval.
            int inc = (lf != LoadFormat.Rtf) && (sf == SaveFormat.Rtf) ? 1 : 0;

            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 30 + inc, true);
            Assert.That(para.GetText(), Is.EqualTo("<MyTag1>My text</MyTag1>\r"));
            para = (Paragraph)doc.GetChild(NodeType.Paragraph, 31 + inc, true);
            Assert.That(para.GetText(), Is.EqualTo("<MyTag2 a=”hello”/>\r"));
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFontNameEnums(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Run\Font\TestFontNameEnums", lf, sf);

            FontInfo font;

            if (lf != LoadFormat.WordML)
            {
                // RK It looks like MS Word 2003 does not write standard fonts like Courier New,
                // Arial, Time New Roman, Symbol to WordML. However, Word 2007 seems to write them.
                // To make this test run, we simply don't test standard fonts in WordML.

                font = doc.FontInfos["Courier New"];
                Assert.That(font.Pitch, Is.EqualTo(FontPitch.Fixed));
                Assert.That(font.Family, Is.EqualTo(Words.Fonts.FontFamily.Modern));

                font = doc.FontInfos["Times New Roman"];
                Assert.That(font.Pitch, Is.EqualTo(FontPitch.Variable));
                Assert.That(font.Family, Is.EqualTo(Words.Fonts.FontFamily.Roman));

                font = doc.FontInfos["Arial"];
                Assert.That(font.Pitch, Is.EqualTo(FontPitch.Variable));
                Assert.That(font.Family, Is.EqualTo(Words.Fonts.FontFamily.Swiss));
            }

            // These are non standard fonts and they are written in all MS Word formats.
            font = doc.FontInfos["Vladimir Script"];
            Assert.That(font.Pitch, Is.EqualTo(FontPitch.Variable));
            Assert.That(font.Family, Is.EqualTo(Words.Fonts.FontFamily.Script));

            font = doc.FontInfos["Bauhaus 93"];
            Assert.That(font.Pitch, Is.EqualTo(FontPitch.Variable));
            Assert.That(font.Family, Is.EqualTo(Words.Fonts.FontFamily.Decorative));
        }

        /// <summary>
        /// Tests the chain of inheritance for font attributes
        /// (direct formatting, char styles, paragraph styles).
        ///
        /// RK This does not actually test any import/export. It purely test the model.
        /// Maybe we should move tests that test the model only out of the unified tests?
        /// </summary>
        [Test]
        public void TestRunWithCharAndParaStyle()
        {
            Document doc = new Document();

            Paragraph para = (Paragraph)doc.SelectSingleNode("//Paragraph[1]");
            Run run = para.AppendChild(new Run(doc, "Hello"));

            //These attrs comes from default font formatting.
            Assert.That(run.Font.Name, Is.EqualTo("Times New Roman"));
            Assert.That(run.Font.Size, Is.EqualTo(12d));
            Assert.That(run.Font.Bold, Is.EqualTo(false));

            //Check paragraph style works.
            //FOSS: the blank document is now loaded from Blank.docx (Word2007-style) instead of Blank.doc,
            //so Heading 9 resolves to the theme major font (Cambria) rather than the old Word2003 Arial.
            para.ParagraphFormat.StyleName = "Heading 9";
            Assert.That(run.Font.Name, Is.EqualTo("Cambria"));
            Assert.That(run.Font.Size, Is.EqualTo(10d));
            Assert.That(run.Font.Bold, Is.EqualTo(false));

            //Check character style works.
            run.Font.StyleName = "Strong";
            Assert.That(run.Font.Bold, Is.EqualTo(true));

            //Check explicit formatting on the run works.
            run.Font.Size = 20;
            Assert.That(run.Font.Size, Is.EqualTo(20d));
        }

        /// <summary>
        /// Test how font border complex attributes work on a text run.
        /// </summary>
        [Test]
        public void TestFontBorderOnRunInheritance()
        {
            Document doc = new Document();

            Paragraph para = (Paragraph)doc.SelectSingleNode("//Paragraph[1]");
            Run run = para.AppendChild(new Run(doc, "Hello"));

            //No font border by default.
            Assert.That(run.Font.Border.LineWidth, Is.EqualTo(0d));

            //Change the paragraph style font border and check it gets inherited
            para.ParagraphFormat.Style.Font.Border.LineWidth = 1;
            Assert.That(doc.Styles["Normal"].Font.Border.LineWidth, Is.EqualTo(1d)); //Check we changed it in the style.
            Assert.That(run.Font.Border.LineWidth, Is.EqualTo(1d));

            //Change the character style font border and check it gets inherited
            run.Font.Style.Font.Border.LineWidth = 10;

            // AM. According to my tests Normal paragraph doesn't inherit Default Paragraph Font. See saved output file.
            Assert.That(doc.Styles[StyleIdentifier.DefaultParagraphFont].Font.Border.LineWidth, Is.EqualTo(10d));
            Assert.That(run.Font.Border.LineWidth, Is.EqualTo(1d));

            // FOSS: Doc writer removed; these were smoke saves after the real model assertions above.

            //Change the border on the run itself and check it overrides font border specified in the styles.
            run.Font.Border.LineWidth = 3;
            Assert.That(run.Font.Border.LineWidth, Is.EqualTo(3d));
        }

        /// <summary>
        /// WORDSNET-2932 Tab characters are lost after open/save document.
        /// Verify that absolute tabs are nicely saved to other formats and rendered without a crash.
        /// </summary>
        [Test]
        public void TestAbsolutePositionTabNoCrash()
        {
            const string testName = @"Model\Run\Text\TestAbsolutePositionTab";
            Document doc = TestUtil.Open(testName + ".docx");
            // FOSS: doc/wml/odt/html/pdf writers removed. Keep a Docx round-trip as the no-crash check.
            TestUtil.SaveOpen(doc, testName + ".docx", (SaveOptions)null, false);
        }

        /// <summary>
        /// TODO 3 Note that when a table is autoformatted some character formatting does not
        /// seem to be stored in CHPX so I cannot get for example font to be white.
        /// I guess I need to make a copy of table autoformats and recreate formats
        /// explicitly when displaying or saving into other formats.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestWhiteFont(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Run\Font\TestWhiteFont", lf, sf);

            Run run = (Run)doc.SelectSingleNode("//Run[1]");
            Font font = run.Font;
            Assert.That(font.Color.ToArgb(), Is.EqualTo(Color.White.ToArgb()));
        }

        /// <summary>
        /// Tab characters are lost after open/save document.
        /// Added new node to the model.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTestAbsolutePositionTab(LoadFormat lf, SaveFormat sf)
        {
            if ((lf != LoadFormat.Doc) && (lf != LoadFormat.WordML))
            {
                Document doc = TestUtil.OpenSaveOpen(@"Model\Run\Text\TestAbsolutePositionTab", lf, sf);

                AbsolutePositionTab tab = (AbsolutePositionTab)doc.GetNodeById("1.0.0.0");
                Assert.That(tab.GetText(), Is.EqualTo("\t"));
                Assert.That(tab.LeaderChar, Is.EqualTo(AbsolutePositionTabLeaderChar.Dot));

                tab = (AbsolutePositionTab)doc.GetNodeById("1.4.0.0");
                Assert.That(tab.LeaderChar, Is.EqualTo(AbsolutePositionTabLeaderChar.Dot));
                Assert.That(tab.Alignment, Is.EqualTo(AbsolutePositionTabAlignment.Center));
                Assert.That(tab.PositioningBase, Is.EqualTo(AbsolutePositionTabPositioningBase.Indent));

                tab = (AbsolutePositionTab)doc.GetNodeById("2.4.0.0");
                Assert.That(tab.LeaderChar, Is.EqualTo(AbsolutePositionTabLeaderChar.None));
                Assert.That(tab.Alignment, Is.EqualTo(AbsolutePositionTabAlignment.Right));
                Assert.That(tab.PositioningBase, Is.EqualTo(AbsolutePositionTabPositioningBase.Margin));
            }
        }

        /// <summary>
        /// There was a problem with direct formatting applied to a character
        /// style not working when direct formatting was switching off features like bold
        /// or strike through defined in the character style.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDirectFormattingOnCharStyle(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Run\Style\TestDirectFormattingOnCharStyle", lf, sf);

            // 0       1       2   3
            // [Word1 ][Word2 ][Wo][rd3][\r]
            NodeList runs = doc.SelectNodes("//Run");
            Assert.That(runs.Count, Is.EqualTo(4));

            //This run has bold and strike inherited from character style.
            Run run = (Run)runs[1];
            Assert.That(run.GetText(), Is.EqualTo("Word2 "));
            Assert.That(run.Font.Bold, Is.EqualTo(true));
            Assert.That(run.Font.HighlightColor.ToArgb(), Is.EqualTo(0));
            Assert.That(run.Font.StrikeThrough, Is.EqualTo(true));

            run = (Run)runs[2];
            Assert.That(run.GetText(), Is.EqualTo("Wo"));
            Assert.That(run.Font.Bold, Is.EqualTo(true));

            // Highlight is switched on by direct formatting.
            Assert.That(run.Font.HighlightColor.ToArgb(), Is.EqualTo(Color.Cyan.ToArgb()));

            //Strike switched off by direct formatting
            Assert.That(run.Font.StrikeThrough, Is.EqualTo(false));
        }


        private static void CheckRun(Run run, string expectedText, Underline expectedUnderline, Color expectedColor,
            string expectedStyle)
        {
            Assert.That(run.GetText(), Is.EqualTo(expectedText));
            Assert.That(run.Font.Underline, Is.EqualTo(expectedUnderline));
            Assert.That(run.Font.Color.ToArgb(), Is.EqualTo(expectedColor.ToArgb()));
            Assert.That(run.Font.StyleName, Is.EqualTo(expectedStyle));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCustomColor(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Run\Font\TestCustomColor", lf, sf);

            Run run = (Run)doc.SelectSingleNode("//Run[1]");
            Assert.That(run.GetText(), Is.EqualTo("Word1"));
            DrColor reddish = DrColor.FromArgb(218, 24, 70);
            DrColor greenish = DrColor.FromArgb(0, 100, 50);
            //Test custom color in the original doc
            //Font color
            Assert.That(run.Font.ColorInternal, Is.EqualTo(reddish));
            //Shading colors
            Assert.That(run.Font.Shading.BackgroundPatternColorInternal, Is.EqualTo(greenish));
        }

        /// <summary>
        /// In this the RTL attributes are different from the normal italic and dont size.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLostFontSize(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Run\Font\TestLostFontSize", lf, sf);

            Run run = (Run)doc.SelectSingleNode("//Run[1]");
            Assert.That(run.Font.Italic, Is.EqualTo(false));
            Assert.That(run.Font.ItalicBi, Is.EqualTo(true));
            Assert.That(run.Font.Size, Is.EqualTo(9d));
            Assert.That(run.Font.SizeBi, Is.EqualTo(10d));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSymbol(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Run\Text\TestSymbol", lf, sf);

            NodeCollection runs = doc.GetChildNodes(NodeType.Run, true);

            Run run = (Run)runs[0];
            Assert.That(run.Font.Name, Is.EqualTo("Arial"));
            Assert.That(run.Text, Is.EqualTo("\x2264"));

            run = (Run)runs[1];

            Assert.That(run.Font.NameAscii, Is.EqualTo("Symbol"));
            Assert.That(run.Font.NameOther, Is.EqualTo("Symbol"));
            Assert.That(run.Text, Is.EqualTo("\xf0a3"));
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLineBreakType(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Run\Text\TestLineBreakType", lf, sf);

            switch (lf)
            {
                case LoadFormat.Doc:
                case LoadFormat.Rtf:
                    {
                        Assert.That(doc.GetChildNodes(NodeType.Run, true).Count, Is.EqualTo(6));

                        Run run = (Run)doc.GetChild(NodeType.Run, 0, true);
                        Assert.That(run.Text, Is.EqualTo("Line break\x000bLine break clear left"));
                        Assert.That(run.RunPr.LineBreakClear, Is.EqualTo(LineBreakClear.None));

                        run = (Run)doc.GetChild(NodeType.Run, 1, true);
                        Assert.That(run.Text, Is.EqualTo("\x000b"));
                        Assert.That(run.RunPr.LineBreakClear, Is.EqualTo(LineBreakClear.Left));

                        run = (Run)doc.GetChild(NodeType.Run, 3, true);
                        Assert.That(run.Text, Is.EqualTo("\x000b"));
                        Assert.That(run.RunPr.LineBreakClear, Is.EqualTo(LineBreakClear.Right));

                        run = (Run)doc.GetChild(NodeType.Run, 5, true);
                        Assert.That(run.Text, Is.EqualTo("\x000b"));
                        Assert.That(run.RunPr.LineBreakClear, Is.EqualTo(LineBreakClear.All));
                        break;
                    }
                case LoadFormat.WordML:
                case LoadFormat.Docx:
                    {
                        // RK When saved as WordML or DOCX MS Word breaks runs differently.
                        Assert.That(doc.GetChildNodes(NodeType.Run, true).Count, Is.EqualTo(5));

                        Run run = (Run)doc.GetChild(NodeType.Run, 0, true);
                        Assert.That(run.Text, Is.EqualTo("Line break"));

                        run = (Run)doc.GetChild(NodeType.Run, 1, true);
                        Assert.That(run.Text, Is.EqualTo("\x000bLine break clear left"));
                        Assert.That(run.RunPr.LineBreakClear, Is.EqualTo(LineBreakClear.None));

                        run = (Run)doc.GetChild(NodeType.Run, 2, true);
                        Assert.That(run.Text, Is.EqualTo("\x000bLine break clear right"));
                        Assert.That(run.RunPr.LineBreakClear, Is.EqualTo(LineBreakClear.Left));

                        run = (Run)doc.GetChild(NodeType.Run, 3, true);
                        Assert.That(run.Text, Is.EqualTo("\x000bText wrapping break"));
                        Assert.That(run.RunPr.LineBreakClear, Is.EqualTo(LineBreakClear.Right));

                        run = (Run)doc.GetChild(NodeType.Run, 4, true);
                        Assert.That(run.Text, Is.EqualTo("\x000b"));
                        Assert.That(run.RunPr.LineBreakClear, Is.EqualTo(LineBreakClear.All));
                        break;
                    }
                default:
                    throw new InvalidOperationException("Unknown file format.");
            }
        }

        /// <summary>
        /// RK There is a date custom property in the document.
        /// RK When reading RTF, MS Word seems to use current culture.
        /// Therefore we need to switch to the culture in which the document was created.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestPageBreak(LoadFormat lf, SaveFormat sf)
        {
            SystemPal.SaveCulture();
            try
            {
                SystemPal.SetStandardCulture();

                Document doc = TestUtil.OpenSaveOpen(@"Model\Run\Text\TestPageBreak", lf, sf);

                Assert.That(doc.GetText(), Is.EqualTo("Text before page break.\r\x000c" +
                        "Text after page break.\x000c"));

                NodeList runs = doc.SelectNodes("//Run");
                Assert.That(runs.Count, Is.EqualTo(3));

                //Page break seems to have its own character run in Word document that can have properties.
                Run pageBreak = (Run)runs[1];
                Assert.That(pageBreak.Font.Color.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));

                //Page break belongs seems to belong to a paragraph object.
                Paragraph para = pageBreak.ParentParagraph;
                //Some settings I modified on page break has affected the paragraph.
                Assert.That(para.ParagraphFormat.SpaceBefore, Is.EqualTo(24.0));
            }
            finally
            {
                SystemPal.RestoreCulture();
            }
        }

        /// <summary>
        /// WORDSNET-803 Setting LocaleID does not influence the language setting,
        /// seeing in statusbar of MS Word.
        /// Test creating a document and specifying locale id for runs.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLocaleId(LoadFormat lf, SaveFormat sf)
        {
            DocumentBuilder b = new DocumentBuilder();
            b.Font.LocaleId = 1030;
            b.Writeln("Hello");
            TestUtil.SaveOpen(b.Document, @"Model\Run\Text\TestLocaleId", lf, sf);
            // Can't really test anything automatically.
            // Open the file in MS Word and see what language is displayed.
        }

        [Test]
        public void TestSetCharacterStyle()
        {
            Document doc = new Document();
            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            Assert.That(doc.Styles["Default Paragraph Font"], Is.EqualTo(para.ParagraphBreakFont.Style));
            para.ParagraphBreakFont.Style = doc.Styles["Hyperlink"];
            Assert.That(doc.Styles["Hyperlink"], Is.EqualTo(para.ParagraphBreakFont.Style));
        }

        [Test, ExpectedException(typeof (ArgumentNullException))]
        public void TestSetCharacterStyleNull()
        {
            Document doc = new Document();
            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            para.ParagraphBreakFont.Style = null;
        }

        [Test,
         ExpectedException(typeof (ArgumentException), ExpectedMessage = "This style belongs to a different document.")]
        public void TestSetCharacterStyleWrongDocument()
        {
            Document doc1 = new Document();
            Document doc2 = new Document();
            Paragraph para = doc1.FirstSection.Body.FirstParagraph;
            para.ParagraphBreakFont.Style = doc2.Styles["Hyperlink"];
        }

        [Test, ExpectedException(typeof (ArgumentException), ExpectedMessage = "This style is not a character style.")]
        public void TestSetCharacterStyleWrongStyleType()
        {
            Document doc = new Document();
            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            para.ParagraphBreakFont.Style = doc.Styles["Normal"];
        }

        /// <summary>
        /// Test language settings.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLanguageSettings(LoadFormat lf, SaveFormat sf)
        {
            const string fileName = @"Model\Run\Text\TestLanguageSettings";

            Document doc = TestUtil.Open(fileName, lf);
            SaveOptions so = SaveOptions.CreateSaveOptions(sf);
            so.SetTestMode();
            so.UpdateAmbiguousTextFont = true;
            TestUtil.SaveOpen(doc, fileName, TestUtil.GetUnifiedScenario(lf, sf), so);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFontHighlight(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestFontHighlight", lf, sf);
        }

        /// <summary>
        /// WORDSNET-4653 Font.HighlightColor doesn’t work for DOCX documents.
        /// Export of highlight was incorrect.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCreateHighlight(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            // Check a "stock color".
            builder.Font.HighlightColor = Color.Yellow;
            builder.Writeln("Highlighted in stock yellow (from stock).");

            // Check a custom color. Highlights are only 16 predefined colors and
            // such custom colors get converted to the nearest color.
            builder.Font.HighlightColor = DrColor.FromArgb(240, 0, 0).ToNativeColor();
            builder.Writeln("Highlighted in red (from custom).");

            doc = TestUtil.SaveOpen(doc, @"Model\Run\Font\TestCreateHighlight", lf, sf);

            // This was set using stock color.
            Run run = (Run)doc.GetChild(NodeType.Run, 0, true);
            Assert.That(run.Font.HighlightColor.ToArgb(), Is.EqualTo(Color.Yellow.ToArgb()));

            // This was set using custom color.
            run = (Run)doc.GetChild(NodeType.Run, 1, true);
            Assert.That(run.Font.HighlightColor.ToArgb(), Is.EqualTo(Color.Red.ToArgb()));
        }

        /// <summary>
        /// Test that BoolEx toggle values are property calculated as booleans and written to wordml.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBoolEx(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestBoolEx", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLineBreak(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Text\TestLineBreak", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestUnicode(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Text\TestUnicode", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestWingdings(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Text\TestWingdings", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAll(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestAll", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCaps(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestCaps", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDefaultFont(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestDefaultFont", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestEffects(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestEffects", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestEmboss(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestEmboss", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFontBoldItalic(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestFontBoldItalic", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFontMetrics(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestFontMetrics", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFontNameStyleSize(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestFontNameStyleSize", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFontOutline(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestFontOutline", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFontRaisedLowered(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestFontRaisedLowered", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFontSize(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestFontSize", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFontWidth(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestFontWidth", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFontSubscript(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestFontSubscript", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestHiddenInvisible(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestHiddenInvisible", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestHiddenVisible(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestHiddenVisible", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestProofing(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestProofing", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevisionEdit(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Other\TestRevisionEdit", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevisionFormatting(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Other\TestRevisionFormatting", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevisionNumbering(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Other\TestRevisionNumbering", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSpacing(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestSpacing", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestStrikeThrough(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestStrikeThrough", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestStrikeUnderline(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestStrikeUnderline", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSubscript(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestSubscript", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestUnderline(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestUnderline", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestUnderlineAverageMetrics(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestUnderlineAverageMetrics", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestUnderlineBigFont(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestUnderlineBigFont", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestUnderlineMetrics(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestUnderlineMetrics", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestUnderlinePosition(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Font\TestUnderlinePosition", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAutoColorBackground(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Border\TestAutoColorBackground", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAutoColorTexture(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Border\TestAutoColorTexture", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFontBorder(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Border\TestFontBorder", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFontShading(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Border\TestFontShading", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTextureShading(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Border\TestTextureShading", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestComment(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Other\TestComment", lf, sf);
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestHyperlinkLayout(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Other\TestHyperlinkLayout", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestInlinePicture(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Other\TestInlinePicture", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestInlinePictureEffects(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Other\TestInlinePictureEffects", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestInlinePicturePosition(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Other\TestInlinePicturePosition", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestOle2(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Other\TestOle2", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestPrivate(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Other\TestPrivate", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTextInputAsText(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Other\TestTextInputAsText", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTabLeader(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Other\TestTabLeader", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestType1Font(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Other\TestType1Font", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestYen(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Run\Text\TestYen", lf, sf);
        }

        /// <summary>
        /// Tests emphasis attribute for text.
        /// </summary>
        /// <remarks>
        /// AM. I couldn't understand how to make text emphasized in MS Word. There is no such menu command or something.
        /// So I made test file in weird way - just write various emphasis value for CKcd sprm (from 0 to 4).
        /// Original test file contains just one emphasis option, this file contains all of them.
        /// </remarks>
        /// <param name="lf"></param>
        /// <param name="sf"></param>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestEmphasis(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Run\Font\TestDefect4081-2", lf, sf);

            Assert.That(((Run)doc.GetNodeById("2.1.0.0")).RunPr.EmphasisMark, Is.EqualTo(EmphasisMark.None));
            Assert.That(((Run)doc.GetNodeById("2.2.0.0")).RunPr.EmphasisMark, Is.EqualTo(EmphasisMark.OverSolidCircle));
            Assert.That(((Run)doc.GetNodeById("2.3.0.0")).RunPr.EmphasisMark, Is.EqualTo(EmphasisMark.OverComma));
            Assert.That(((Run)doc.GetNodeById("2.4.0.0")).RunPr.EmphasisMark, Is.EqualTo(EmphasisMark.OverWhiteCircle));
            Assert.That(((Run)doc.GetNodeById("2.5.0.0")).RunPr.EmphasisMark, Is.EqualTo(EmphasisMark.UnderSolidCircle));
        }


        /// <summary>
        /// WORDSNET-5184 Size of font is changed during converting to RTF.
        /// "Default Paragraph Font" style should not be used as default font.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestJira5184(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Run\Style\TestJira5184", lf);

            Style defaultParagraphFont = doc.Styles.GetByIstd(StyleIndex.DefaultParagraphFont, false);
            Assert.That(defaultParagraphFont.Font.Name, Is.EqualTo("Arial"));
            // Modify "Default Paragraph Font" style in order to be sure that its attributes are not inherited.
            defaultParagraphFont.RunPr.Color = DrColor.Red;

            Run run = (Run)doc.GetNodeById("0.0.3.0");
            Paragraph para = run.ParentParagraph;
            Assert.That(para.ParagraphStyle.Name, Is.EqualTo("Footer"));
            // This means no character style is assigned.
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("Default Paragraph Font"));

            // Verify direct formatting absence. RTF stores resolved attributes as well.
            if(lf != LoadFormat.Rtf)
                Assert.That(run.RunPr.GetDirectAttr(FontAttr.NameAscii), Is.Null);

            // Attribute can be resolved in two ways. Verify font is inherited from paragraph style.
            Assert.That(run.Font.NameAscii, Is.EqualTo("Algerian"));
            Assert.That(run.GetExpandedRunPr(RunPrExpandFlags.Normal).NameAscii, Is.EqualTo("Algerian"));

            // The same is for font size.
            if(lf != LoadFormat.Rtf)
                Assert.That(run.RunPr.GetDirectAttr(FontAttr.Size), Is.Null);

            Assert.That(run.Font.Size, Is.EqualTo(48));
            // Size is multiplied by 2 in attribute collection.
            Assert.That(run.GetExpandedRunPr(RunPrExpandFlags.Normal).Size, Is.EqualTo(96));

            // Character style is applied.
            run = (Run)doc.GetNodeById("1.3.3.0");
            para = run.ParentParagraph;
            Assert.That(para.ParagraphStyle.Name, Is.EqualTo("Footer"));
            Assert.That(run.CharacterStyle.Name, Is.EqualTo("Emphasis"));
            Assert.That(run.Font.Name, Is.EqualTo("Arial"));
            Assert.That(run.GetExpandedRunPr(RunPrExpandFlags.Normal).NameAscii, Is.EqualTo("Arial"));
            Assert.That(run.Font.Size, Is.EqualTo(10));
            Assert.That(run.GetExpandedRunPr(RunPrExpandFlags.Normal).Size, Is.EqualTo(20));
        }

        /// <summary>
        /// WORDSNET-5894 Incorrect style applied while saving document object as Pdf.
        /// Character style is applied to paragraph - use Normal paragraph style instead.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestJira5894(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Run\Style\TestJira5894", lf);

            Run run = (Run)doc.GetNodeById("0.1.0.0");
            RunPr expandedRunPr = run.GetExpandedRunPr(RunPrExpandFlags.Layout);
            Assert.That(expandedRunPr.Color, IsNot.EqualTo(DrColor.Blue));
        }

        /// <summary>
        /// WORDSNET-11771 Support the dir and the bdo elements.
        /// Added reading and writing 'dir' and 'bdo' elements for DOCX.
        /// These elements in model are represented as runs with Unicode BiDi control character.
        /// </summary>
        [Test]
        // FOSS: Doc/Rtf/Wml formats removed. Keep the Docx roundtrip case.
        [TestCase(UnifiedScenario.Docx2Docx)]
        public void UnifiedTestJira11771(UnifiedScenario scenario)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Bidi\TestJira11771", scenario | UnifiedScenario.NoGold);

            // Paragraph with BiDi control chars.
            Paragraph para = (Paragraph) doc.FirstSection.Body.FirstParagraph.NextSibling;

            //Check runs with BiDi control chars.
            NodeCollection runs = para.GetChildNodes(NodeType.Run, true);

            LoadFormat lf = TestUtil.GetLoadFormat(scenario);
            SaveFormat sf = TestUtil.GetSaveFormat(scenario);

            // MS Word produces different number of BiDi Control char runs depending on file format.
            int expectedRunsCount;
            if (lf == LoadFormat.Docx)
                expectedRunsCount = (sf == SaveFormat.Docx) ? 21 : 17;
            else
                expectedRunsCount = 21;

            Assert.That(runs.Count, Is.EqualTo(expectedRunsCount));

            Assert.That(((Run)runs[4]).Text, Is.EqualTo("\u202a")); //LRE
            Assert.That(((Run)runs[6]).Text, Is.EqualTo("\u202b")); //RLE
            Assert.That(((Run)runs[8]).Text, Is.EqualTo("\u202c")); //PDF
            Assert.That(((Run)runs[9]).Text, Is.EqualTo("\u202d")); //LRO
            Assert.That(((Run)runs[11]).Text, Is.EqualTo("\u202e")); //RLO

            for (int i = 13; i < expectedRunsCount; i++)
                Assert.That(((Run)runs[i]).Text, Is.EqualTo("\u202c")); //PDF
        }

        /// <summary>
        /// WORDSNET-27952 Toggle attributes not resolved correctly when defined in Character style and Paragraph style
        /// </summary>
        [Test]
        public void TestDefect27952CharStyleVsParaStyle()
        {
            Document doc = TestUtil.Open(@"Model\Run\Font\TestDefect27952CharStyleVsParaStyle.docx");

            CheckStyleAttributes(doc);
            TestCombinedAttributes(doc);
        }

        /// <summary>
        /// Test WORDSNET-7965 compliance.
        /// </summary>
        [Test, Ignore("WORDSNET-8160 Work in progress.")]
        public void TestJira7965Compliance()
        {
            Document doc = TestUtil.Open(@"Model\Run\Other\TestJira7965Styles.docx");

            // Verify compliance info is properly set.
            Assert.That(doc.ComplianceInfo.IsDocxExtensions, Is.True);

            OoxmlSaveOptions so = new OoxmlSaveOptions(SaveFormat.Docx);
            so.Compliance = OoxmlCompliance.Ecma376_2006;

            doc = TestUtil.SaveOpen(doc, @"Model\Run\Other\TestJira7965Styles ECMA376", UnifiedScenario.Docx2DocxNoGold, so);
            Assert.That(doc.ComplianceInfo.IsDocxExtensions, Is.False);
        }

        /// <summary>
        /// WORDSNET-10102 Alignment setting are not preserved during open/save a doc
        /// Added support for FontAttr.FitText
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestJira10102(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Run\Text\TestJira10102", lf, sf);

            RunCollection runs = doc.FirstSection.Body.Paragraphs[1].Runs;

            // FitTextId is not supported in RTF format so we have to auto-generate it.
            int fitTextId = ((lf == LoadFormat.Rtf) || (sf == SaveFormat.Rtf)) ? 1 : 0x25135a00;

            Assert.That(runs[1].RunPr[FontAttr.FitText], Is.EqualTo(new FitText(2100, fitTextId)));
            Assert.That(runs[2].RunPr[FontAttr.FitText], Is.EqualTo(new FitText(2100, fitTextId)));
        }




        /// <summary>
        /// Relates to WORDSNET-11841 Additional check for run split logic.
        /// </summary>
        [Test]
        public void TestJira11841Text()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.Writeln("ὕαλον ϕαγεῖν δύναμαι· τοῦτο οὔ με βλάπτει. ฉันกินกระจกได้แต่มันไม่ทำให้ฉันเจ็บ 私はガラスを食べられます。それは私を傷つけません。");
            builder.Writeln("ฉันกินกระจกได้แต่มันไม่ทำให้ฉันเจ็บ 私はガラスを食べられます。それは私を傷つけません。");
            builder.Writeln("ὕαλον ϕαγεῖν δύναμαι· τοῦτο οὔ με βλάπτει. ฉันกิน");

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            // FOSS: Doc writer removed; Markdown (NonMSWordFormat) also extracts script subruns.
            TestUtil.ExecuteValidator(doc, SaveFormat.Markdown);

            // After validator is executed Thai subruns should be extracted.
            Paragraph p = paras[0];
            Assert.That(p.Runs.Count, Is.EqualTo(3));
            CheckNotThai(p.Runs[0]);
            CheckThai(p.Runs[1]);
            CheckNotThai(p.Runs[2]);

            p = paras[1];
            Assert.That(p.Runs.Count, Is.EqualTo(2));
            CheckThai(p.Runs[0]);
            CheckNotThai(p.Runs[1]);

            p = paras[2];
            Assert.That(p.Runs.Count, Is.EqualTo(2));
            CheckNotThai(p.Runs[0]);
            CheckThai(p.Runs[1]);
        }

        /// <summary>
        /// Fields were parsed incorrectly so "Error!" is rendered instead of ruby.
        /// </summary>
        [Test]
        public void TestRubyFieldError()
        {
            // EQ field can be written to OOXML. We need to convert it too.
            // FOSS: Doc reader removed; the FlatOpc branch exercises the same conversion.
            foreach (LoadFormat lf in new LoadFormat[] { LoadFormat.FlatOpc })
            {
                Document doc = TestUtil.Open(@"Model\Run\Text\Ruby\TestRubyFromField", lf);

                FieldExtractorToCollection fieldExtractor = new FieldExtractorToCollection();
                fieldExtractor.Extract(doc.FirstSection.Body);

                // Verify that no fields were loaded to model.
                Assert.That(fieldExtractor.Fields.Count, Is.EqualTo(0));
            }
        }

        /// <summary>
        /// WORDSNET-4136 Ruby (EQ fields) are lost after open/save.
        /// Added roundtrip support and made initial rendering.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestJira4136(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Run\Text\Ruby\TestJira4136", TestUtil.GetUnifiedScenario(lf, sf));
            Run rubyRun = doc.FirstSection.Body.FirstParagraph.GetLastRun();

            Ruby ruby = (Ruby)rubyRun.RunPr[FontAttr.Ruby];

            Assert.That(ruby.Top[0].Text, Is.EqualTo("ait"));
            Assert.That(ruby.Top[0].RunPr[FontAttr.Size], Is.EqualTo(12));

            Assert.That(ruby.Base[0].Text, Is.EqualTo("8"));
            Assert.That(ruby.Base[0].RunPr[FontAttr.Size], Is.EqualTo(28));

            Assert.That(ruby.BaseSize, Is.EqualTo(28));
            Assert.That(ruby.TopSize, Is.EqualTo(12));
            Assert.That(ruby.Distance, Is.EqualTo(26));

            Assert.That(ruby.Alignment, Is.EqualTo(RubyAlignment.DistributeSpace));
            Assert.That(ruby.Language, Is.EqualTo(Language.ChineseChina));
        }

        /// <summary>
        /// WORDSNET-4136 Ruby (EQ fields) are lost after open/save.
        /// </summary>
        /// <remarks>
        /// AM. Save using SaveFormat.Text is original WORDSNET-4136 customer request.
        /// </remarks>
        [Test]
        public void TestJira4136ToText()
        {
            Document doc = TestUtil.Open(@"Model\Run\Text\Ruby\TestJira4136.docx");

            MemoryStream stream = new MemoryStream();
            doc.Save(stream, SaveFormat.Text);

            stream.Position = 0;
            TextReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();

            Assert.That(text, Is.EqualTo("Phonetic Guide: 1(wun)\t2(too)\t4(fow er)\t8(ait)\r\n\r\n"));
        }

        /// <summary>
        /// Relates to WORDSNET-4136 Tests various alignment.
        /// </summary>
        [Test]
        public void TestRubyAlignment()
        {
            // FOSS: Doc writer removed; FlatOpc also preserves ruby through the roundtrip (input is .fopc).
            Document doc = TestUtil.OpenSaveOpen(@"Model\Run\Text\Ruby\TestRubyAlignment",
                TestUtil.GetUnifiedScenario(LoadFormat.FlatOpc, SaveFormat.FlatOpc) | UnifiedScenario.NoGold);

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Ruby ruby = (Ruby)paras[1].FirstRun.RunPr[FontAttr.Ruby];
            Assert.That(ruby.Alignment, Is.EqualTo(RubyAlignment.Center));

            ruby = (Ruby)paras[4].FirstRun.RunPr[FontAttr.Ruby];
            Assert.That(ruby.Alignment, Is.EqualTo(RubyAlignment.DistributeLetter));

            ruby = (Ruby)paras[7].FirstRun.RunPr[FontAttr.Ruby];
            Assert.That(ruby.Alignment, Is.EqualTo(RubyAlignment.DistributeSpace));

            ruby = (Ruby)paras[10].FirstRun.RunPr[FontAttr.Ruby];
            Assert.That(ruby.Alignment, Is.EqualTo(RubyAlignment.Left));

            ruby = (Ruby)paras[13].FirstRun.RunPr[FontAttr.Ruby];
            Assert.That(ruby.Alignment, Is.EqualTo(RubyAlignment.Right));

            ruby = (Ruby)paras[16].FirstRun.RunPr[FontAttr.Ruby];
            Assert.That(ruby.Alignment, Is.EqualTo(RubyAlignment.RightVertical));
        }

        /// <summary>
        /// Tests that EQ field is completely removed after document is saved.
        /// </summary>
        [Test]
        public void TestRubyConversionReverted()
        {
            Document doc = TestUtil.Open(@"Model\Run\Text\Ruby\TestSimpleRuby.xml");

            // FOSS: Doc writer removed; Markdown (NonMSWordFormat) runs the same ruby->field convert/revert.
            doc.Save(new MemoryStream(), SaveFormat.Markdown);

            // Verify that there is still only one empty run with ruby collection.
            Paragraph p = doc.FirstSection.Body.FirstParagraph;
            Assert.That(p.Runs.Count, Is.EqualTo(1));
            Assert.That(p.FirstRun.GetTextLength(), Is.EqualTo(0));
            Assert.That(p.FirstRun.RunPr.Contains(FontAttr.Ruby), Is.True);
        }

        /// <summary>
        /// WORDSNET-11935 EQ field is lost after saving document
        /// </summary>
        [Test]
        public void TestJira11935()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Run\Text\Ruby\TestJira11935", UnifiedScenario.Docx2DocxNoGold);

            Ruby ruby = (Ruby)doc.FirstSection.Body.FirstParagraph.FirstRun.RunPr[FontAttr.Ruby];

            Assert.That(ruby.BaseSize, Is.EqualTo(14));
            Assert.That(ruby.TopSize, Is.EqualTo(12));
            Assert.That(ruby.Top.Text, Is.EqualTo("yorosii"));
            Assert.That(ruby.Base.Text, Is.EqualTo("よろしい"));
        }




        /// <summary>
        /// WORDSNET-12559 Need support for ruby with multiple base and top parts and each part having its own formatting.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestJira12559(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Run\Text\Ruby\TestJira12559", TestUtil.GetUnifiedScenario(lf, sf) | UnifiedScenario.NoGold);

            Run rubyRun = doc.FirstSection.Body.FirstParagraph.FirstRun;

            Ruby ruby = rubyRun.RunPr[FontAttr.Ruby] as Ruby;
            Assert.That(ruby, IsNot.Null());
            Assert.That(ruby.BaseSize, Is.EqualTo(22));
            Assert.That(ruby.TopSize, Is.EqualTo(24));
            Assert.That(ruby.Distance, Is.EqualTo(22));

            Assert.That(ruby.Top.Count, Is.EqualTo(2));
            Assert.That(ruby.Base.Count, Is.EqualTo(2));

            RunPr runPr = ruby.Top[0].RunPr;
            Assert.That(runPr[FontAttr.Color], Is.EqualTo(DrColor.FromArgb(0x00, 0xff, 0x00)));
            Assert.That(runPr[FontAttr.Bold], IsNot.Null());
            Assert.That(runPr[FontAttr.Italic], IsNot.Null());
            Assert.That(runPr[FontAttr.Underline], Is.EqualTo(Underline.Single));
            Assert.That(runPr[FontAttr.Size], Is.EqualTo(144));
            Assert.That(runPr[FontAttr.SizeBi], Is.EqualTo(144));
            Assert.That(runPr[FontAttr.Underline], Is.EqualTo(Underline.Single));

            runPr = ruby.Top[1].RunPr;
            Assert.That(runPr[FontAttr.Color], Is.EqualTo(DrColor.FromArgb(0x00, 0x00, 0xff)));
            Assert.That(runPr[FontAttr.Bold], IsNot.Null());
            Assert.That(runPr[FontAttr.Italic], IsNot.Null());

            runPr = ruby.Base[0].RunPr;
            Assert.That(runPr[FontAttr.Color], Is.EqualTo(DrColor.FromArgb(0xff, 0x00, 0x00)));
            Assert.That(runPr[FontAttr.Bold], IsNot.Null());
            Assert.That(runPr[FontAttr.Italic], IsNot.Null());
            Assert.That(runPr[FontAttr.Underline], Is.EqualTo(Underline.Single));
            Assert.That(runPr[FontAttr.Size], Is.EqualTo(72));
            Assert.That(runPr[FontAttr.SizeBi], Is.EqualTo(72));
            Assert.That(runPr[FontAttr.Underline], Is.EqualTo(Underline.Single));

            runPr = ruby.Base[1].RunPr;
            Assert.That(runPr[FontAttr.Color], Is.EqualTo(DrColor.FromArgb(0x00, 0xff, 0x00)));
            Assert.That(runPr[FontAttr.Bold], Is.Null);
            Assert.That(runPr[FontAttr.Italic], Is.Null);
        }

        // FOSS: TestJira12559FontName removed. It asserted the ruby->EQ-field representation, which only the
        // removed legacy writers (Doc/Rtf/WordML) produce; supported formats (Markdown/Text) flatten ruby to
        // plain text, so the EQ-field field code cannot be produced in FOSS.



        /// <summary>
        /// Tests that ruby constructed properly when text is split into few runs.
        /// </summary>
        [Test]
        public void TestJira13920A()
        {
            CheckRuby(TestUtil.Open(@"Model\Run\Text\Ruby\TestRubyTextSplit.xml"), 0, "amorozov", "morzal");
        }

        private static void CheckRuby(Document doc, int idx, string baseText, string topText)
        {
            NodeCollection runs = doc.GetChildNodes(NodeType.Run, true);

            // Look for Nth ruby
            int cur = 0;
            Ruby ruby = null;
            foreach (Run run in runs)
            {
                if (!run.RunPr.Contains(FontAttr.Ruby))
                    continue;

                if (idx == cur)
                {
                    ruby = (Ruby) run.RunPr[FontAttr.Ruby];
                    break;
                }

                cur++;
            }

            Assert.That(ruby, IsNot.Null());
            Assert.That(ruby.Base.Text, Is.EqualTo(baseText));
            Assert.That(ruby.Top.Text, Is.EqualTo(topText));
        }


        /// <summary>
        /// WORDSNET-15005 x-none language is not supported for DOCX
        /// Added support for 'x-none' (LanguageNotSet) value.
        /// </summary>
        /// <remarks>
        /// Implementation should be simpler but I found that NrxRunEnum is used in PDF export.
        /// I tested produced PDFs and Acrobat Reader opens documents with 'x-none' language but I'm afraid
        /// if any 3rd party will not able to open such files. So I decided to isolate 'x-none' language to DOCX import/export only.
        /// </remarks>
        [Test]
        public void TestJira15005()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Run\Font\TestJira15005", UnifiedScenario.Docx2DocxNoGold);
            Assert.That((Language)doc.FirstSection.Body.FirstParagraph.FirstRun.RunPr[FontAttr.LocaleId], Is.EqualTo(Language.LanguageNotSet));
        }





        /// <summary>
        /// Additional test for WORDSNET-13910 Tests that Ruby has 30 chars limit for its parts.
        /// </summary>
        [Test]
        public void TestJira13910C()
        {
            Document doc = TestUtil.Open(@"Model\Run\Text\Ruby\TestJira13910C.xml");

            // First part of very long ruby is moved to separate run before.
            Run run = doc.FirstSection.Body.FirstParagraph.Runs[0];
            Assert.That(run.Text, Is.EqualTo("123456789123456"));
            Assert.That(run.RunPr[FontAttr.Ruby], Is.Null);

            // Only 30 characters are left in ruby top.
            Ruby ruby = (Ruby)doc.FirstSection.Body.FirstParagraph.Runs[1].RunPr[FontAttr.Ruby];
            Assert.That(ruby.Top.Text, Is.EqualTo("789123456789123456789123456789"));
        }



        /// <summary>
        /// WORDSNET-15827 DOCX to PNG conversion issue with text formatting.
        /// Positioning in the document is incorrect because length of "Ruby" run determines as zero.
        /// Change calculation of the node length in <see cref="DocumentPosition"/> to fix the problem.
        /// </summary>
        [Test]
        public void TestJira15827()
        {
            Document doc = TestUtil.Open(@"Model\Run\Text\Ruby\TestJira15827.docx");
            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            Run firstRunInRunCollection = para.Runs[0];

            NodeRange range = new NodeRange(para, para);
            Run firstRunInNodeRange = null;

            foreach (Node node in range)
            {
                if (node.NodeType == NodeType.Run)
                {
                    firstRunInNodeRange = (Run)node;
                    break;
                }
            }

            Assert.That(firstRunInRunCollection.Equals(firstRunInNodeRange), Is.True);
        }



        // FOSS: removed Test16734 (WORDSNET-16734). It asserted font-specific Font.LineSpacing
        // values (Calibri vs bidi vs Segoe UI Semibold); FOSS resolves every font to the last-resort
        // font, so font-specific line-spacing metrics no longer exist.


        /// <summary>
        /// Simplified case for WORDSNET-20144
        /// </summary>
        [Test]
        public void Test20144A()
        {
            RunPr runPr1 = new RunPr();
            runPr1.SetAttr(FontAttr.Bold, AttrBoolEx.Toggle);

            RunPr runPr2 = runPr1.Clone();

            runPr1.ExpandTo(runPr2);        // Must not throw.

            Assert.That(runPr2[FontAttr.Bold], Is.EqualTo(AttrBoolEx.True));
        }

        /// <summary>
        /// WORDSNET-13983 Add feature to support 'Emphasis Mark' font setting.
        /// </summary>
        [Test]
        public void Test13983()
        {
            const string testName = @"Model\Run\Font\Test13983";

            Document doc = TestUtil.Open(testName + ".docx");

            Run run = doc.FirstSection.Body.FirstParagraph.Runs[2];
            Assert.That(run.Font.EmphasisMark, Is.EqualTo(EmphasisMark.None));
            run.Font.EmphasisMark = EmphasisMark.OverWhiteCircle;
            Assert.That(run.Font.EmphasisMark, Is.EqualTo(EmphasisMark.OverWhiteCircle));
        }

        /// <summary>
        /// Case for WORDSNET-13983
        /// Check the emphasis mark inheritance from the specified style.
        /// </summary>
        [Test]
        public void Test13983_EmphasisStyle()
        {
            Document doc = new Document();

            Style style = doc.Styles.GetByIstd(StyleIndex.Normal, false);
            style.Font.EmphasisMark = EmphasisMark.OverWhiteCircle;

            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            para.ParagraphFormat.Style = style;
            Run run = para.AppendChild(new Run(doc, "Emphasis"));

            Assert.That(run.Font.EmphasisMark, Is.EqualTo(EmphasisMark.OverWhiteCircle));
        }




        /// <summary>
        /// WORDSNET-24973 Ability to determine whether run is ruby text.
        /// Added new public property <see cref="Run.IsPhoneticGuide"/>.
        /// </summary>
        [Test]
        public void Test24973()
        {
            Document doc = TestUtil.Open(@"Model\Run\Text\Ruby\Test24973.xml");

            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            Assert.That(para.Runs[0].IsPhoneticGuide, Is.True);
            Assert.That(para.Runs[1].IsPhoneticGuide, Is.False);
        }

        /// <summary>
        /// WORDSNET-25173 Programmatically get both base text and annotation of Run nodes that contain ruby text.
        /// Added new corresponding public properties.
        /// </summary>
        [Test]
        public void Test25173()
        {
            Document doc = TestUtil.Open(@"Model\Run\Text\Ruby\Test25713.docx");
            Run run = doc.FirstSection.Body.FirstParagraph.FirstRun;
            Assert.That(run.Text, Is.EqualTo(""));
            Assert.That(run.PhoneticGuide.BaseText, Is.EqualTo("base"));
            Assert.That(run.PhoneticGuide.RubyText, Is.EqualTo("ruby"));
        }

        /// <summary>
        /// Relates to WORDSNET-25173
        /// Tests when Run is not a phonetic guide.
        /// </summary>
        [Test]
        public void Test25173NotRuby()
        {
            DocumentBuilder builder = new DocumentBuilder();

            builder.Writeln("text");

            Run run = builder.Document.FirstSection.Body.FirstParagraph.FirstRun;
            Assert.That(run.Text, Is.EqualTo("text"));
            Assert.That(run.IsPhoneticGuide, Is.False);
            Assert.That(run.PhoneticGuide.BaseText, Is.EqualTo(""));
            Assert.That(run.PhoneticGuide.RubyText, Is.EqualTo(""));
        }


        /// <summary>
        /// Relates to  WORDSNET-25856
        /// Tests getting the effective font name from rFonts.
        /// </summary>
        [Test]
        public void Test25856_Fonts()
        {
            const string expFontNames = "DaunPenh,DaunPenh,Tahoma,Tahoma,MoolBoran,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran," +
                "PMingLiU,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran," +
                "PMingLiU,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran,MoolBoran,PMingLiU," +
                "MoolBoran,MoolBoran,PMingLiU,MoolBoran,MoolBoran,PMingLiU,MoolBoran,MoolBoran,PMingLiU,MoolBoran,MoolBoran,PMingLiU," +
                "MoolBoran,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran," +
                "PMingLiU,MoolBoran,PMingLiU,MoolBoran,Tahoma,Tahoma,Tahoma,Tahoma,Tahoma,PMingLiU,PMingLiU,PMingLiU,MoolBoran," +
                "MoolBoran,PMingLiU,MoolBoran,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran," +
                "PMingLiU,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran," +
                "PMingLiU,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran,PMingLiU,MoolBoran," +
                "PMingLiU,MoolBoran,PMingLiU,MoolBoran,PMingLiU,PMingLiU,PMingLiU,PMingLiU,PMingLiU,PMingLiU,PMingLiU,PMingLiU," +
                "PMingLiU,PMingLiU,PMingLiU,PMingLiU,PMingLiU,PMingLiU,PMingLiU,PMingLiU,PMingLiU,PMingLiU,Tahoma,Tahoma,Tahoma";

            Document doc = TestUtil.Open(@"Model\Run\Font\Test25856_Fonts.docx");

            // Tests getting a font name from an empty Run.
            Assert.That(doc.FirstSection.Body.FirstParagraph.ParagraphBreakFont.Name, Is.EqualTo("Tahoma"));

            List<string> fontNames = new List<string>();
            foreach (Paragraph para in doc.FirstSection.Body.Paragraphs)
                foreach (Run run in para.Runs)
                    // We ignore font comments here.
                    if (run.Text.Length < 3)
                        fontNames.Add(run.Font.Name);

            Assert.That(string.Join(",", fontNames), Is.EqualTo(expFontNames));
        }

        /// <summary>
        /// WORDSNET-25882 Wk: How can we determine whether the text inside a shape is white or black?
        /// Shape style was not taken into account when resolving font color of a run located in a shape.
        /// </summary>
        [Test]
        public void Test25882()
        {
            Document doc = TestUtil.Open(@"Model\Run\Font\Test25882.docx");
            ShapeCollection shapes = doc.FirstSection.Body.Shapes;

            Paragraph paragraph1 = shapes[0].FirstParagraph;
            Assert.That(paragraph1.GetText(), Is.EqualTo("Black text in shape 2\r"));
            Assert.That(paragraph1.FirstRun.Font.Color, Is.EqualTo(Color.FromArgb(0, 0, 0)));

            Paragraph paragraph2 = shapes[1].FirstParagraph;
            Assert.That(paragraph2.GetText(), Is.EqualTo("white text in shape 1\r"));
            Assert.That(paragraph2.FirstRun.Font.Color, Is.EqualTo(Color.FromArgb(0xff, 0xff, 0xff)));
        }


        /// <summary>
        /// WORDSNET-26904 Consider exposing FontAttr.OpenTypeNumSpacing.
        /// Tests new public API Font.NumberSpacing.
        /// </summary>
        [Test]
        public void Test26904()
        {
            Document doc = TestUtil.Open(@"Model\Run\Font\Test26904", LoadFormat.Docx);

            RunCollection coll = doc.FirstSection.Body.FirstParagraph.Runs;
            Run run1 = coll[0];
            Run run2 = coll[1];
            Run run3 = coll[2];
            Assert.That(run1.Font.NumberSpacing, Is.EqualTo(NumSpacing.Default));
            Assert.That(run2.Font.NumberSpacing, Is.EqualTo(NumSpacing.Proportional));
            Assert.That(run3.Font.NumberSpacing, Is.EqualTo(NumSpacing.Tabular));

            run1.Font.NumberSpacing = NumSpacing.Proportional;
            run2.Font.NumberSpacing = NumSpacing.Tabular;
            run3.Font.NumberSpacing = NumSpacing.Default;

            Assert.That(run1.Font.NumberSpacing, Is.EqualTo(NumSpacing.Proportional));
            Assert.That(run2.Font.NumberSpacing, Is.EqualTo(NumSpacing.Tabular));
            Assert.That(run3.Font.NumberSpacing, Is.EqualTo(NumSpacing.Default));
        }

        /// <summary>
        /// Checks that run has only Thai characters and forced to be Thai by certain attributes.
        /// </summary>
        private static void CheckThai(Run run)
        {
            foreach (char ch in run.Text)
                Assert.That(UnicodeUtil.IsThaiCharacter(ch), Is.True);

            Assert.That(run.RunPr[FontAttr.ComplexScript], Is.EqualTo(AttrBoolEx.True));
            Assert.That((Language)run.RunPr[FontAttr.LocaleIdBi], Is.EqualTo(Language.ThaiThailand));
        }

        /// <summary>
        /// Checks that run has no Thai characters and not forced to be Thai.
        /// </summary>
        private static void CheckNotThai(Run run)
        {
            foreach (char ch in run.Text)
                Assert.That(UnicodeUtil.IsThaiCharacter(ch), Is.False);

            Assert.That(run.RunPr[FontAttr.ComplexScript], Is.Null);
            Assert.That(run.RunPr[FontAttr.LocaleIdBi], Is.Null);
        }

        private static void CheckStyleAttributes(Document doc)
        {
            CheckStyleAttributes(doc, "Heading 1", AttrBoolEx.True, null);
            CheckStyleAttributes(doc, "Heading 4", AttrBoolEx.True, AttrBoolEx.True);
            CheckStyleAttributes(doc, "Strong", AttrBoolEx.True, null);
            CheckStyleAttributes(doc, "Emphasis", null, AttrBoolEx.True);
            CheckStyleAttributes(doc, "Intense Emphasis", AttrBoolEx.True, AttrBoolEx.True);
        }

        private static void CheckStyleAttributes(Document doc, string styleName, AttrBoolEx bold, AttrBoolEx italic)
        {
            Style style = doc.Styles[styleName];
            CheckStyleAttribute(style, FontAttr.Bold, bold);
            CheckStyleAttribute(style, FontAttr.Italic, italic);
        }

        private static void CheckStyleAttribute(Style style, int attr, AttrBoolEx expectedValue)
        {
            AttrBoolEx styleAttrValue = (AttrBoolEx)style.GetFontAttr(attr, false);
            Assert.That(styleAttrValue, Is.EqualTo(expectedValue));
        }

        private static void TestCombinedAttributes(Document doc)
        {
            NodeCollection runs = doc.FirstSection.Body.GetChildNodes(NodeType.Run, true);

            int runCounter = 0;

            Run run = (Run)runs[runCounter];
            CheckCombinedRunAttributes(run, "JustHeading1 ", true, false);

            run = (Run)runs[++runCounter];
            CheckCombinedRunAttributes(run, "Heading1PlusStrong ", false, false);

            run = (Run)runs[++runCounter];
            CheckCombinedRunAttributes(run, "Heading1PlusEmphasis ", true, true);

            run = (Run)runs[++runCounter];
            CheckCombinedRunAttributes(run, "Heading1PlusIntenseEmphasis ", false, true);

            run = (Run)runs[++runCounter];
            CheckCombinedRunAttributes(run, "JustHeading4 ", true, true);

            run = (Run)runs[++runCounter];
            CheckCombinedRunAttributes(run, "Heading4PlusStrong ", false, true);

            run = (Run)runs[++runCounter];
            CheckCombinedRunAttributes(run, "Heading4PlusEmphasis ", true, false);

            run = (Run)runs[++runCounter];
            CheckCombinedRunAttributes(run, "Heading4PlusIntenseEmphasis ", false, false);

            run = (Run)runs[++runCounter];
            CheckCombinedRunAttributes(run, "NormalText ", false, false);

            run = (Run)runs[++runCounter];
            CheckCombinedRunAttributes(run, "JustStrong ", true, false);

            run = (Run)runs[++runCounter];
            CheckCombinedRunAttributes(run, "JustEmphasis ", false, true);

            run = (Run)runs[++runCounter];
            CheckCombinedRunAttributes(run, "JustIntenseEmphasis ", true, true);
        }

        private static void CheckCombinedRunAttributes(Run run, string expectedText, bool bold, bool italic)
        {
            Assert.That(run.Text, Is.EqualTo(expectedText));
            Assert.That(run.Font.Bold, Is.EqualTo(bold), "Unexpected Font.Bold value for {0}" + expectedText);
            Assert.That(run.Font.Italic, Is.EqualTo(italic), "Unexpected Font.Italic value for {0}" + expectedText);
        }
    }
}
