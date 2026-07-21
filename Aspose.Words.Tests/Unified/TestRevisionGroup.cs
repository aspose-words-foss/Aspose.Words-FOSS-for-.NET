// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/07/2016 by Alexey Morozov

using System;
using System.IO;
using System.Linq;
using Aspose.Common;
using Aspose.TestFx;
using Aspose.Words.Revisions;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Tests related to revision group collection feature.
    /// </summary>
    [TestFixture]
    public class TestRevisionGroup
    {
        [SetUp]
        public void SetStandardCulture()
        {
            SystemPal.SaveCulture();
            SystemPal.SetStandardCulture();
        }

        [TearDown]
        public void RevertToOldCulture()
        {
            SystemPal.RestoreCulture();
        }

        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// Tests that for deleted table only text of first cell is collected to visible nodes.
        /// </summary>
        [Test]
        public void TestDeletedTable()
        {
            Document doc = TestUtil.Open(@"Model\Revision\RevisionGroups\TestDeletedTable.docx");
            RevisionGroupCollection groups = doc.Revisions.Groups;
            Assert.That(groups.Count, Is.EqualTo(1));
            Assert.That(groups[0].VisibleText, Is.EqualTo("1"));
        }

        /// <summary>
        /// Tests that for two adjacent deleted table only text of first cell of first table is collected.
        /// </summary>
        [Test]
        public void TestDeletedTwoTables()
        {
            Document doc = TestUtil.Open(@"Model\Revision\RevisionGroups\TestDeletedTwoTables.docx");

            RevisionGroupCollection groups = doc.Revisions.Groups;
            Assert.That(groups.Count, Is.EqualTo(1));
            Assert.That(groups[0].VisibleText, Is.EqualTo("rt\rAbc\rcde\rxyz"));
        }

        /// <summary>
        /// Tests how inline shape deletion is collected.
        /// </summary>
        [Test]
        public void TestDeletedInlineShape()
        {
            Document doc = TestUtil.Open(@"Model\Revision\RevisionGroups\TestDeletedInlineShape.docx");

            RevisionGroupCollection groups = doc.Revisions.Groups;
            Assert.That(groups.Count, Is.EqualTo(1));
            Assert.That(groups[0].VisibleNodes[0].NodeType, Is.EqualTo(NodeType.Shape));
        }

        /// <summary>
        /// Tests how section break deletion is collected.
        /// </summary>
        [Test]
        public void TestDeletedSectionBreakMark()
        {
            Document doc = TestUtil.Open(@"Model\Revision\RevisionGroups\TestDeletedSectionBreakMark.docx");

            RevisionGroupCollection groups = doc.Revisions.Groups;
            Assert.That(groups.Count, Is.EqualTo(1));
            Assert.That(groups[0].VisibleText, Is.EqualTo("t page\r\fSecond pa"));
        }

        /// <summary>
        /// Tests how paragraph break deletion is collected when last paragraph break is not included.
        /// </summary>
        [Test]
        public void TestDeletedParagraphBreakMark()
        {
            Document doc = TestUtil.Open(@"Model\Revision\RevisionGroups\TestDeletedParagraphBreakMark.docx");

            RevisionGroupCollection groups = doc.Revisions.Groups;
            Assert.That(groups.Count, Is.EqualTo(2));

            Assert.That(groups[0].VisibleText, Is.EqualTo("st\rSe"));
        }

        /// <summary>
        /// Tests how paragraph break deletion is collected when last paragraph break is included.
        /// </summary>
        [Test]
        public void TestDeletedParagraphBreakMark2()
        {
            Document doc = TestUtil.Open(@"Model\Revision\RevisionGroups\TestDeletedParagraphBreakMark2.docx");

            RevisionGroupCollection groups = doc.Revisions.Groups;
            Assert.That(groups.Count, Is.EqualTo(1));

            Assert.That(groups[0].VisibleText, Is.EqualTo("First\rSecond\r"));
        }

        /// <summary>
        /// Tests how few revision groups are collected.
        /// </summary>
        [Test]
        public void TestFewGroups()
        {
            TestRevisionGroups(@"Model\Revision\RevisionGroups\TestFewGroups.docx",
                new string[] { "mer hav", "g iss", "e with an RTF do", "ume", "t, on importing docume", "M custom style im" });
        }


        /// <summary>
        /// Tests format string for various borders and combination of borders.
        /// </summary>
        [Test]
        public void TestBorders()
        {
            TestRevisionGroups(@"Model\Revision\RevisionGroups\TestBorders.docx", new string[]
                {
                    "Border: : (Dotted, Green, 0.5 pt Line width)",
                    "Border: Box: (Single solid line, Background 2, 0.5 pt Line width)",
                    "Border: Left: (Single solid line, Auto, 0.5 pt Line width)",
                    "Border: Right: (Single solid line, Auto, 0.5 pt Line width)",
                    "Border: Left: (Single solid line, Auto, 0.5 pt Line width), Right: (Single solid line, Auto, 0.5 pt Line width)",
                    "Border: Between: (Single solid line, Auto, 0.5 pt Line width), Bar: (Single solid line, Auto, 0.5 pt Line width), Box: (Single solid line, Auto, 0.5 pt Line width)",
                    "Border: Between: (Single solid line, Auto, 0.5 pt Line width)",
                    "Border: Right: (Dashed (large gap), Custom Color(RGB(38,154,110)), 4.5 pt Line width)",
                    "Border: Top: (Triple solid lines, Orange, 0.5 pt Line width), Bottom: (Dash dot (stroked), Orange, 3 pt Line width), Left: (Dashed (large gap), Dark Red, 4.5 pt Line width), Right: (Double wavy, Custom Color(RGB(59,23,73)), 0.75 pt Line width)",
                    "Border: Left: (Triple solid lines, Accent 1, 0.5 pt Line width), Right: (Triple solid lines, Accent 1, 0.5 pt Line width)",
                    "Border: Box: (Shadowed Dash dot dot, Accent 6, 1.5 pt Line width)"
                });
        }

        [Test]
        public void TestShading()
        {
            TestRevisionGroups(@"Model\Revision\RevisionGroups\TestShading.docx", new string[]{
                "Pattern: Solid (100%) (Accent 6)",
                "Pattern: Dk Horizontal (Accent 2 Foreground, Auto Background)",
                "Pattern: Lt Trellis (Dark Red Foreground, Auto Background)",
                "Pattern: Clear (Accent 4)",
                "Pattern: 62.5% (Accent 1 Foreground, Accent 1 Background)",
                "Pattern: Dk Grid (Accent 4 Foreground, Accent 4 Background)",
                "Pattern: Lt Grid (Custom Color(RGB(204,102,255)) Foreground, Yellow Background)",
                "Pattern: 12.5% (Background 1 Foreground, Background 1 Background)" });
        }

        [Test]
        public void TestSectionFormat()
        {
            TestRevisionGroups(@"Model\Revision\RevisionGroups\TestSectionFormat.docx",
                "Left: 2 cm, Right: 2 cm, Top: 3 cm, Bottom: 1.5 cm, Width: 29.7 cm, Height: 21 cm");
        }

        [Test]
        public void TestUnderline()
        {
            TestRevisionGroups(@"Model\Revision\RevisionGroups\TestUnderline.docx", new string[]
                { "Underline", "Double underline", "Thick underline", "Dotted underline",
                  "Dashed underline", "Dot-dash underline", "Dot-dot-dash underline", "Wave underline",
                  "Underline, Underline color: Accent 4", "Wave double underline" });
        }

        [Test]
        public void TestCrossSections()
        {
            TestRevisionGroups(@"Model\Revision\RevisionGroups\TestCrossSections.docx", "Font color: Blue");
        }

        [Test]
        public void TestLineSpacing()
        {
            TestRevisionGroups(@"Model\Revision\RevisionGroups\TestLineSpacing.docx",
                new string[] {"Line spacing: Single", "Line spacing: 1.5 lines", "Line spacing: Exactly 12 pt", "Line spacing: At least 30 pt"});
        }


        /// <summary>
        /// Test revision text for paragraph formatting.
        /// </summary>
        [Test]
        public void TestParagraphFormat()
        {
            TestRevisionGroups(@"Model\Revision\RevisionGroups\TestParagraphFormat.docx", new string[]
                {
                    "Centered",
                    "Right, Indent: Left: 1 cm, Right: 1 cm, Space Before: 6 pt, After: 12 pt, Line spacing: Double, " +
                    "Keep with next, Keep lines together, Border: Box: (Dashed (small gap), Accent 5, 6 pt Line width), Pattern: 20% (Auto Foreground, Accent 4 Background)"
                });
        }

        /// <summary>
        /// Test revision text for paragraph formatting.
        /// </summary>
        [Test]
        public void TestParagraphBools()
        {
            TestRevisionGroups(@"Model\Revision\RevisionGroups\TestParagraphBools.docx", new string[]
                {
                    "Automatically adjust right indent when grid is defined, " +
                    "Add space between paragraphs of the same style, " +
                    "No widow/orphan control, " +
                    "No page break before, " +
                    "Don't keep with next, " +
                    "Don't keep lines together, " +
                    "Don't suppress line numbers, " +
                    "Hyphenate, " +
                    "Don't use Asian rules to control first and last character, " +
                    "Allow text to wrap in the middle of a word, " +
                    "Don't allow hanging punctuation, " +
                    "Don't adjust space between Latin and Asian text, " +
                    "Don't adjust space between Asian text and numbers, " +
                    "Snap to grid, " +
                    "Not Don't swap indents on facing pages, " +
                    "Compress initial punctuation",

                    "Allow text to wrap in the middle of a word",
                    "No page break before, Allow text to wrap in the middle of a word, Don't allow hanging punctuation"
                });
        }
        /// <summary>
        /// Tests revision text for various text effects.
        /// </summary>
        [Test]
        public void TestEffects()
        {
            TestRevisionGroups(@"Model\Revision\RevisionGroups\TestEffects.docx",
                "Font: Bold, Font color: Accent 4, Text Outline, Glow, Bevel");
        }

        [Test]
        public void TestRunFormat1()
        {
            TestRevisionGroups(@"Model\Revision\RevisionGroups\TestRunFormat1.docx", new string[]
                {
                    "Font: Bold, Underline, Font color: Purple, Strikethrough, Superscript, Expanded by 1 pt, Raised by 5 pt, Kern at 8 pt, Border: : (Single solid line, Auto, 0.5 pt Line width), Highlight",
                    "Font: Bold, Italic, Complex Script Font: Bold, Italic"
                });
        }

        [Test]
        public void TestRunFormat2()
        {
            // There is a strange issue with Highlight. Paragraph break has no highlight set but Word doesn't show separate group for it.
            TestRevisionGroups(@"Model\Revision\RevisionGroups\TestRunFormat2.docx", new string[]
                {
                    "Font: Arial Rounded MT Bold, 24 pt, Bold, Italic, Underline, Font color: Accent 4, Complex Script Font: 24 pt, Double strikethrough, Subscript, Small caps, Highlight, Text Outline, Reflection",
                    "Font: Arial Rounded MT Bold, 24 pt, Bold, Italic, Underline, Font color: Accent 4, Complex Script Font: 24 pt, Double strikethrough, Subscript, Small caps, Text Outline, Reflection"
                });
        }

        [Test]
        public void TestRunFormat3()
        {
            TestRevisionGroups(@"Model\Revision\RevisionGroups\TestRunFormat3.docx",
                new string[]
                {
                    "Font: Bold, Italic, Underline, Font color: Purple, Complex Script Font: Italic, Strikethrough, Subscript",
                    "Font: (Default) Times New Roman, Complex Script Font: Times New Roman",
                    "Underline",
                    "Font: Italic, Underline",
                    "Font: Italic, Strikethrough",
                    "Strikethrough",
                    "Strikethrough, Superscript",
                    "Superscript",
                    "Font color: Accent 4, Superscript",
                    // Word doesn't show this balloon for some reason.
                    "Font color: Accent 4",
                    "Font color: Accent 4, Subscript",
                    // Word doesn't show this balloon for some reason.
                    "Font color: Accent 4",
                    "Font: 28 pt, Font color: Accent 4, Complex Script Font: 28 pt",
                    "Font: 28 pt, Complex Script Font: 28 pt",
                    "Font: 28 pt, Complex Script Font: 28 pt, Double strikethrough",
                    "Double strikethrough",
                    "Font: 26 pt, Dot-dot-dash underline, Complex Script Font: 26 pt",
                    "Font: 26 pt, Dot-dot-dash underline, Underline color: Custom Color(RGB(214,0,147)), Complex Script Font: 26 pt",
                    "Dot-dot-dash underline, Underline color: Accent 1",
                    "Small caps", "Character scale: 50%",
                    "Character scale: 50%, Expanded by 1 pt, Kern at 11 pt",
                    "Raised by 6 pt, Kern at 11 pt",
                    "Font: Italic, Raised by 6 pt",
                    "Font: Bold, Italic",
                    "Font: Consolas",
                    "Font: Consolas, Italic, Font color: Accent 4, Subscript",
                    "Font: Consolas",
                });
        }

        [Test]
        public void TestColors()
        {
            TestRevisionGroups(@"Model\Revision\RevisionGroups\TestColors.docx", new string[]
                {
                    "Font color: Dark Red",
                    "Font color: Yellow",
                    "Font color: Green",
                    "Font color: Light Green",
                    "Font color: Accent 6",
                    "Font color: Text 1",
                    "Font color: Accent 3",
                    "Font color: Background 1",
                    "Font color: Accent 6",
                    "Font color: Custom Color(RGB(220,169,68))",
                    "Font color: Dark Blue",
                    "Font color: Purple", "Text Fill"
                });
        }

        [Test]
        public void TestFormattedDeleted()
        {
            TestRevisionGroups(@"Model\Revision\RevisionGroups\TestFormattedDeleted.docx",
                new string[] { "formatted ", "Font: 20 pt, Bold, Italic" });
        }

        /// <summary>
        /// Document is taken from WORDSNET-10988 forum links.
        /// </summary>
        [Test]
        public void TestJira10988A()
        {
            TestRevisionGroups(@"Model\Revision\RevisionGroups\TestJira10988A.docx",
                new string[] { "Font: Verdana, 15 pt, Bold" });
        }

        [Test]
        public void TestJira10988B()
        {
            // AM. 'Complex Script Font: 18pt' label is shown only if any complex script language (Arabic for example) is
            // in Office Language Preferences list.
            // Actually I don't know how to handle this since Language preferences is load time option but
            // revision group is runtime feature.
            //
            // Maybe we need a parallel property, something like RevisionGroup.ComplexScriptText.

            TestRevisionGroups(@"Model\Revision\RevisionGroups\TestJira10988B.docx",
                new string[] { "Font: 18 pt, Bold, Complex Script Font: 18 pt", "Underline", "Font: Italic", "Centered", "Right" });
        }




        [Test]
        public void TestIsFormatRevisionFlag()
        {
            TestRevisionGroups(@"Model\Revision\TestIsFormatRevisionFlag.docx", new string[]{"Font: Italic", "Heading 2" });
        }



        [Test]
        public void TestHighlight()
        {
            TestRevisionGroups(@"Model\Revision\RevisionGroups\TestHighlight.docx",
                new string[] { "Font color: Dark Blue, Highlight", "Font color: Dark Blue" });
        }

        /// <summary>
        /// Tests that table formatting change is correctly formatted.
        /// </summary>
        [Test]
        public void TestTableFormatted()
        {
            TestRevisionGroups(@"Model\Revision\RevisionGroups\TestTableFormatted.docx", "Table");
        }

        /// <summary>
        /// Tests that theme fonts are correctly formatted.
        /// </summary>
        [Test]
        public void TestThemeFonts()
        {
            TestRevisionGroups(@"Model\Revision\RevisionGroups\TestThemeFonts.docx", "Font: +Headings (Calibri Light)");
        }


        /// <summary>
        /// Exception is thrown on layout.
        /// </summary>
        [Test]
        public void TestStyleRevisions()
        {
            TestRevisionGroups(@"Model\Revision\TestStyleRevisions.docx", new string[]
                {
                    "Emphasis: Font: Bold, Font color: Accent 1, Border: : (Dashed (small gap), Auto, 0.5 pt Line width)",
                    "Heading 2: , Space Before: 4 pt, After: 0 pt, Line spacing: Double",
                    "T",
                    "t",
                    "Font: Bold",
                    "Border: Bottom: (Single solid line, Accent 1, 0.5 pt Line width)"
                });
        }

        /// <summary>
        /// Tests list revision.
        /// </summary>
        /// <remarks>
        /// AM. This series of tests (TestJira5738-1, 2, etc.) is good for testing List related formatting.
        /// </remarks>
        [Test, Ignore("List format issue, postpone for a while.")]
        public void TestJira5738_1()
        {
            TestRevisionGroups(@"ImportDocx\TestJira5738-1.docx", new string[]
                {
                    "Normal, No bullets or numbering",
                    "List Paragraph, Numbered + Level: 1 + Numbering Style: I, II, III, ... + Start at: 1 + Alignment: Right + Aligned at: 0.64 cm + Indent at: 1.27 cm",
                    "Numbered + Level: 1 + Numbering Style: 1, 2, 3, ... + Start at: 1 + Alignment: Left + Aligned at: 0.64 cm + Indent at: 1.27 cm"
                });
        }


        /// <summary>
        /// WORDSNET-15506 Support of move revisions.
        /// </summary>
        [Test]
        public void TestMoveRevisions()
        {
            TestRevisionGroups(@"Model\Revision\RevisionGroups\TestMoveRevisionGroups.docx", new string[]
                {
                    "Test\r",
                    "Test\rTest\r",
                    "Row 5",
                    " ",
                    "Test\rTest\r",
                    "Inserted",
                    "Test Test Test\rTest\r",
                    "Moved Test Moved\rTest\rTest\rMoved Simple Moved\rMoved Simple",
                    "Test Test Test\rTest\r",
                    "Moved Test Moved\rMoved Simple Moved\rMoved Simple ",
                    "Row 1",
                    "Row 1"
                });
        }





        /// <summary>
        /// WORDSNET-24018 Revisions reports no groups while there is a group.
        /// Checking for a special case where Row or Cell is revision.ParentNode for the table revision.
        /// </summary>
        [Test]
        public void Test24018()
        {
            Document doc = TestUtil.Open(@"Model\Revision\RevisionGroups\Test24018.docx");

            RevisionGroup group = doc.Revisions.Groups[0];
            foreach (Revision revision in doc.Revisions)
                Assert.That(revision.Group, Is.EqualTo(group));
        }


        /// <summary>
        /// Related with WORDSNET-24237
        /// Checks that a row splits one revision group.
        /// </summary>
        [Test]
        public void Test24237Rows()
        {
            TestRevisionGroups(
                @"Model\Revision\RevisionGroups\Test24237Rows.docx",
                new string[]
                {
                    "Font: Bold, Complex Script Font: Bold",
                    "Font: Bold, Complex Script Font: Bold"
                });
        }

        /// <summary>
        /// Related with WORDSNET-24237
        /// Checks that end table does not slit one revision group.
        /// However the Word splits a revision group at this case. Skip it for a while.
        /// </summary>
        [Test]
        public void Test24237Rows2()
        {
            TestRevisionGroups(
                @"Model\Revision\RevisionGroups\Test24237Rows2.docx",
                new string[]
                {
                    "Font: Bold, Complex Script Font: Bold",
                });
        }





        /// <summary>
        /// WORDSNET-28093 Number of revision returned by Aspose.Words does not match MS Word (test_129.docx).
        /// Seems MS Word ignores complex script related attributes for paragraph break character.
        /// </summary>
        [Test]
        public void Test28093()
        {
            Document doc = TestUtil.Open(@"Model\Revision\RevisionGroups\Test28093.docx");

            RevisionGroupCollection groups = doc.Revisions.Groups;
            Assert.That(groups.Count, Is.EqualTo(4));

            CheckGroup(groups[0], "-", "ipsum ");
            CheckGroup(groups[1], "+", " This is added in target file");
            CheckGroup(groups[2], "*", "Font: 9 pt, Italic, Underline");
            CheckGroup(groups[3], "*", "Font: Italic");
        }




        /// <summary>
        /// WORDSNET-28098 Number of revision returned by Aspose.Words does not match MS Word (test_144.docx).
        /// ColumnSpacing is incorrectly read.
        /// </summary>
        [Test]
        public void Test28098()
        {
            Document doc = TestUtil.Open(@"Model\Revision\RevisionGroups\Test28098.docx");

            RevisionGroupCollection groups = doc.Revisions.Groups;
            Assert.That(CountRevisionType(groups, RevisionType.Deletion), Is.EqualTo(1));
            Assert.That(CountRevisionType(groups, RevisionType.Insertion), Is.EqualTo(1));
            Assert.That(CountRevisionType(groups, RevisionType.FormatChange), Is.EqualTo(10));
        }

        /// <summary>
        /// Additional test for WORDSNET-28098.
        /// Original document is slightly modified to have section format revision.
        /// </summary>
        [Test]
        public void Test28098A()
        {
            Document doc = TestUtil.Open(@"Model\Revision\RevisionGroups\Test28098A.docx");

            SectPr sectPr = doc.FirstSection.SectPr;
            Assert.That(sectPr.HasFormatRevision, Is.True);
            Assert.That(sectPr[SectAttr.ColumnsSpacing], Is.EqualTo(720));
            Assert.That(sectPr.FormatRevision.RevPr[SectAttr.ColumnsSpacing], Is.EqualTo(820));

            Assert.That(CountRevisionType(doc.Revisions.Groups, RevisionType.FormatChange), Is.EqualTo(11));
        }











        /// <summary>
        /// Relates to WORDSNET-28091.
        /// Simplified document, no DML fallback.
        /// </summary>
        [Test]
        public void Test28091B()
        {
            Document doc = TestUtil.Open(@"Model\Revision\RevisionGroups\Test28091B.docx");

            // MS Word: 2+, 2-, 0*
            RevisionGroupCollection groups = doc.Revisions.Groups;
            Assert.That(groups.Count, Is.EqualTo(4));

            CheckGroup(groups[0], "-", "3Introduction...");
            CheckGroup(groups[1], "+", "1Introduction...");
            CheckGroup(groups[2], "+", "1Introduction...");
            CheckGroup(groups[3], "-", "3Introduction...");
        }


        /// <summary>
        /// WORDSNET-28956 Revision group calculated incorrectly.
        /// Break group when story type is changed.
        /// </summary>
        [Test]
        public void Test28956()
        {
            Document doc = TestUtil.Open(@"Model\Revision\RevisionGroups\Test28956.docx");
            Assert.That(doc.Revisions.Groups.Count, Is.EqualTo(5));
        }

        /// <summary>
        /// Returns true if document has format revisions.
        /// </summary>
        private static bool HasFormatRevisions(Document doc)
        {
            foreach (Revision revision in doc.Revisions)
                if (revision.RevisionType == RevisionType.FormatChange)
                    return true;

            return false;
        }

        /// <summary>
        /// Tests how document builds revision groups collection.
        /// </summary>
        private static void TestRevisionGroups(string file, string balloon)
        {
            TestRevisionGroups(file, new string[] { balloon });
        }

        /// <summary>
        /// Tests how document builds revision groups collection.
        /// </summary>
        private static void TestRevisionGroups(string file, string[] balloons)
        {
            Document doc = TestUtil.Open(file);
            TestRevisionGroups(doc, balloons);
        }

        /// <summary>
        /// Tests how document builds revision groups collection.
        /// </summary>
        private static void TestRevisionGroups(Document doc, string[] balloons)
        {
            Assert.That(doc.Revisions.Groups.Count, Is.EqualTo(balloons.Length));

            for (int i = 0; i < balloons.Length; i++)
                Assert.That(doc.Revisions.Groups[i].Text, Is.EqualTo(balloons[i]));
        }

        private static void CheckGroup(RevisionGroup group, string revisionType, string text)
        {
            switch (revisionType)
            {
                case "+":
                case "-":
                    Assert.That(group.EditRevision.Type, Is.EqualTo((revisionType == "-" ? EditRevisionType.Deletion : EditRevisionType.Insertion)));
                    break;

                case "*":
                    Assert.That(group.RevisionType, Is.EqualTo(RevisionType.FormatChange));
                    break;

                case "#":
                    Assert.That(group.RevisionType, Is.EqualTo(RevisionType.StyleDefinitionChange));
                    break;
            }

            if (text.EndsWith("..."))
            {
                string partialText = text.Substring(0, text.Length - 3);
                Assert.That(group.Text.StartsWith(partialText), Is.True, string.Format("Text should start with '{0}'", partialText));
            }
            else if (text.StartsWith("..."))
            {
                string partialText = text.Substring(3);
                Assert.That(group.Text.EndsWith(partialText), Is.True, string.Format("Text should end with '{0}'", partialText));
            }
            else
                Assert.That(group.Text, Is.EqualTo(text));
        }

        private static int CountRevisionType(RevisionGroupCollection groups, RevisionType type)
        {
            int count = 0;
            foreach (RevisionGroup g in groups)
            {
                if (g.RevisionType == type)
                    count++;
            }
            return count;
        }
    }
}
