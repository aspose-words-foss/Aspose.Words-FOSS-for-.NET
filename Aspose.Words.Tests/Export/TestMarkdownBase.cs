// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/11/2019 by Ilya Navrotskiy

using Aspose.Words.Drawing;
using Aspose.Words.Lists;
using Aspose.Words.Notes;
using Aspose.Words.Tables;
using NUnit.Framework;
using List = Aspose.Words.Lists.List;

namespace Aspose.Words.Tests.Export
{
    /// <summary>
    /// The base class for testing markdown format import/export.
    /// </summary>
    public class TestMarkdownBase
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            TestUtil.SetUpTests();
        }

        /// <summary>
        /// The helper class for C++ auto-porter.
        /// </summary>
        public class ExpectedStyleName
        {
            public string StyleName { get; private set; }
            public int Count { get; private set; }

            public ExpectedStyleName(string styleName, int count)
            {
                StyleName = styleName;
                Count = count;
            }
        }

        /// <summary>
        /// Verifies styles of a specified paragraph.
        /// </summary>
        protected static void CheckStyles(Paragraph para, params string[] expectedStyleNames)
        {
            Style style = para.ParagraphStyle;
            foreach (string expectedStyleName in expectedStyleNames)
            {
                Assert.That(style.Name, Is.EqualTo(expectedStyleName));
                style = style.GetBaseStyle();
            }
        }

        /// <summary>
        /// Verifies paragraph.
        /// </summary>
        protected static void CheckParagraph(Paragraph para, string expectedText, params string[] expectedStyleNames)
        {
            Assert.That(para.GetText().StartsWith(expectedText), Is.True, string.Format("Expected:\t{0}\r\nBut was:\t{1}", expectedText, para.GetText()));
            CheckStyles(para, expectedStyleNames);
        }

        /// <summary>
        /// Verifies paragraph.
        /// </summary>
        protected static void CheckParagraph(Paragraph para, string expectedText, ExpectedStyleName[] expectedStyleNames)
        {
            Assert.That(para.GetText(), Is.EqualTo(expectedText));

            Style style = para.ParagraphStyle;
            foreach (ExpectedStyleName expected in expectedStyleNames)
            {
                for (int j = 0; j < expected.Count; j++)
                {
                    Assert.That(style.Name.StartsWith(expected.StyleName), Is.True, string.Format("Expected: {0}. But was: {1}", expected.StyleName, style.Name));
                    style = style.GetBaseStyle();
                }

            }
        }

        /// <summary>
        /// Verifies HorizontalRule.
        /// </summary>
        protected static void CheckHorizontalRule(Paragraph para, params string[] expectedStyleNames)
        {
            CheckStyles(para, expectedStyleNames);

            Assert.That(para.Count, Is.EqualTo(1));
            Assert.That(((Shape)para.FirstChild).HorizontalRule.On, Is.True);
        }

        /// <summary>
        /// Verifies list item.
        /// </summary>
        protected static void CheckListItem(Paragraph para,
            string expectedText, int expectedListLevel, string expectedLabelString, params string[] expectedStyleNames)
        {
            CheckParagraph(para, expectedText, expectedStyleNames);
            Assert.That(para.ParaPr[ParaAttr.ListLevel], Is.EqualTo(expectedListLevel));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo(expectedLabelString));
        }

        /// <summary>
        /// Verifies bullet list item.
        /// </summary>
        protected static void CheckListItem(Paragraph para,
            int expectedListLevel, string expectedLabelString, params string[] expectedStyleNames)
        {
            CheckStyles(para, expectedStyleNames);
            Assert.That(para.ParaPr[ParaAttr.ListLevel], Is.EqualTo(expectedListLevel));
            Assert.That(para.ListLabel.LabelString, Is.EqualTo(expectedLabelString));
        }

        protected static void CheckStyleList(Style style, int expectedLevel, int expectedStartAt, char expectedMarker)
        {
            Assert.That(style.ParaPr[ParaAttr.ListLevel], Is.EqualTo(expectedLevel));

            List list = style.GetListInternal();
            ListLevel listLevel = list.ListLevels[expectedLevel];
            Assert.That(listLevel.StartAt, Is.EqualTo(expectedStartAt));
            Assert.That(listLevel.NumberFormat[listLevel.NumberFormat.Length - 1], Is.EqualTo(expectedMarker));
        }

        protected static NodeCollection CheckFootnotes(CompositeNode compositeNode, params string[] expectedFootnotesText)
        {
            NodeCollection footnotes = compositeNode.GetChildNodes(NodeType.Footnote, true);
            for (int i = 0; i < footnotes.Count; i++)
                Assert.That(((Footnote)footnotes[i]).GetText(), Is.EqualTo(expectedFootnotesText[i]));

            Assert.That(footnotes.Count, Is.EqualTo(expectedFootnotesText.Length));

            return footnotes;
        }

        /// <summary>
        /// Verifies run.
        /// </summary>
        protected static void CheckRun(Run run, string expectedText, string expectedStyleName)
        {
            Assert.That(run.Text, Is.EqualTo(expectedText));
            Assert.That(run.CharacterStyle.Name, Is.EqualTo(expectedStyleName));
        }

        /// <summary>
        /// Verifies run.
        /// </summary>
        protected static void CheckRun(Run run, string expectedText, bool isExpectedBold, bool isExpectedItalic,
            bool isExpectedStrikeThrough = false)
        {
            Assert.That(run.Text, Is.EqualTo(expectedText));
            CheckRunAttr(run, isExpectedBold, isExpectedItalic, isExpectedStrikeThrough);
        }

        /// <summary>
        /// Verifies run.
        /// </summary>
        protected static void CheckRun(Run run, string expectedText, string expectedStyleName,
            bool isExpectedBold, bool isExpectedItalic)
        {
            Assert.That(run.Text, Is.EqualTo(expectedText));
            Assert.That(run.CharacterStyle.Name, Is.EqualTo(expectedStyleName));
            CheckRunAttr(run, isExpectedBold, isExpectedItalic);
        }

        /// <summary>
        /// Verifies run.
        /// </summary>
        protected static void CheckRunAttr(Run run,
            bool isExpectedBold, bool isExpectedItalic, bool isExpectedStrikeThrough = false)
        {
            Assert.That(run.RunPr.Bold.ToBool(), Is.EqualTo(isExpectedBold));
            Assert.That(run.RunPr.Italic.ToBool(), Is.EqualTo(isExpectedItalic));
            Assert.That(run.RunPr.StrikeThrough.ToBool(), Is.EqualTo(isExpectedStrikeThrough));
        }

        /// <summary>
        /// Verifies row.
        /// </summary>
        protected static void CheckRow(Row row, string [] expectedTextInCells)
        {
            Assert.That(expectedTextInCells.Length, Is.EqualTo(row.Count));
            for (int i = 0; i < row.Cells.Count; i++)
                Assert.That(row.Cells[i].GetText(), Is.EqualTo(expectedTextInCells[i]));
        }

        /// <summary>
        /// Gets first occurred paragraph with a specified text.
        /// </summary>
        protected static Paragraph GetParagraphWithText(Document doc, string text)
        {
            return TestUtil.GetParagraphWithText(doc, string.Format("...{0}...", text));
        }

        /// <summary>
        /// Gets run with a specified text.
        /// </summary>
        protected static Run GetRunWithText(Paragraph para, string text)
        {
            return TestUtil.GetRunWithText(para, string.Format("...{0}...", text));
        }

        /// <summary>
        /// Checks a shape.
        /// </summary>
        protected static void CheckShape(Shape shape, string altText, string shapeUri)
        {
            CheckShape(shape, altText, shapeUri, string.Empty);
        }

        /// <summary>
        /// Checks a shape.
        /// </summary>
        protected static void CheckShape(Shape shape, string altText, string shapeUri, string title)
        {
            Assert.That(shape.AlternativeText, Is.EqualTo(altText));
            Assert.That(shape.ImageData.SourceFullName, Is.EqualTo(shapeUri));
            Assert.That(shape.ImageData.Title, Is.EqualTo(title));
        }
    }
}
