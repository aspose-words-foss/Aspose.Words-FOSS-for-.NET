// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/09/2011 by Roman Korchagin

using NUnit.Framework;
// kvk: This using is needed to build project with newer NUnit versions.
using List = Aspose.Words.Lists.List; 

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Tests for List Styles.
    /// </summary>
    [TestFixture]
    public class TestStyleList : UnifiedTestsBase
    {
        /// <summary>
        /// WORDSNET-1568 The problem was that DOCX and WordML do not store copies of list formatting 
        /// in the list, which is a style reference. They only store list formatting in the list, which is a 
        /// list style definition. Modified attribute resolution so we retrieve the list which is a list style definition. 
        /// 
        /// category - Style, list style
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestListStyle(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Style\TestListStyle", lf, sf);

            // Check all is set up correctly from the list style side point of view.
            Style style = doc.Styles[StyleIdentifier.OutlineList1];

            Assert.That(style.Type, Is.EqualTo(StyleType.List));
            Assert.That(style.BasedOnIstd, Is.EqualTo(StyleIndex.NoList));
            Assert.That(style.Istd, Is.EqualTo(0x0f));
            Assert.That(style.NextIstd, Is.EqualTo(0x0f));

            // Check the link from the list style to the list and back.
            Assert.That(style.ParaPr.ListId, Is.EqualTo(1));
            Assert.That(style.List.IsListStyleReference, Is.EqualTo(false));
            Assert.That(style.List.IsListStyleDefinition, Is.EqualTo(true));
            Assert.That(style.List.Style, Is.EqualTo(style));

            // Check all is set up correctly from the paragraph point of view.
            Paragraph para = (Paragraph)doc.GetChild(NodeType.Paragraph, 0, true);
            ParagraphFormat pf = para.ParagraphFormat;

            // The paragraph style is Normal, its correct.
            Assert.That(pf.StyleIdentifier, Is.EqualTo(StyleIdentifier.Normal));

            // The paragraph refers to a list that references the list style.
            Assert.That(pf.ListId, Is.EqualTo(2));
            Assert.That(para.ListFormat.List.IsListStyleReference, Is.EqualTo(true));
            Assert.That(para.ListFormat.List.IsListStyleDefinition, Is.EqualTo(false));
            Assert.That(para.ListFormat.List.Style, Is.EqualTo(style));

            // Check the list formatting makes its way on to the paragraph.
            // This happens not through the list style, but through normal list formatting that references the list style.
            Assert.That(pf.LeftIndent, Is.EqualTo(18d));
        }

        /// <summary>
        /// category - list style, import style
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestImportListStyle(LoadFormat lf, SaveFormat sf)
        {
            Document srcDoc = TestUtil.OpenSaveOpen(@"Model\Style\TestImportListStyle", lf, sf);

            // Check things are the way we expect them in the source document.
            Style style = srcDoc.Styles.GetBySti(StyleIdentifier.OutlineList1, false);
            Assert.That(style.Istd, Is.EqualTo(16));
            Assert.That(style.ParaPr.ListId, Is.EqualTo(11));

            // This list defines the style.
            List list = srcDoc.Lists.GetListByListId(11);
            Assert.That(list.IsListStyleDefinition, Is.EqualTo(true));
            Assert.That(list.Style, Is.EqualTo(style));

            // This list references the style.
            list = srcDoc.Lists.GetListByListId(12);
            Assert.That(list.IsListStyleReference, Is.EqualTo(true));
            Assert.That(list.Style, Is.EqualTo(style));


            Document dstDoc = new Document();

            // Import a paragraph that has list style formatting.
            Paragraph srcPara = srcDoc.FirstSection.Body.Paragraphs[0];
            Paragraph dstPara = (Paragraph)dstDoc.ImportNode(srcPara, true, ImportFormatMode.UseDestinationStyles);
            dstDoc.FirstSection.Body.Paragraphs.Add(dstPara);

            // The "normal" list was imported first and became the first list.
            Assert.That(dstPara.ListFormat.List.ListId, Is.EqualTo(1));
            // Word preserves source list definition identifier while importing.
            Assert.That(dstPara.ListFormat.List.ListDefId, Is.EqualTo(0x7C8F5509));

            // Check the list style was imported into the document.
            style = dstDoc.Styles.GetBySti(StyleIdentifier.OutlineList1, false);
            // Note the list style has a different istd in the destination document.
            Assert.That(style.Istd, Is.EqualTo(15));
            // Check the list style points to the proper list definition.
            Assert.That(style.List.ListId, Is.EqualTo(2));
            // Word preserves source list definition identifier while importing.
            Assert.That(style.List.ListDefId, Is.EqualTo(0x49AF0B72));

            // There should be two lists and two list definitions imported.
            Assert.That(dstDoc.Lists.Count, Is.EqualTo(2));
            Assert.That(dstDoc.Lists.ListDefCount, Is.EqualTo(2));
            Assert.That(dstDoc.Lists[0].Style, Is.EqualTo(style));
            Assert.That(dstDoc.Lists[1].Style, Is.EqualTo(style));
            Assert.That(dstDoc.Lists[0].IsListStyleDefinition, Is.EqualTo(false));
            Assert.That(dstDoc.Lists[1].IsListStyleDefinition, Is.EqualTo(true));

            // And just for fun, import another paragraph and request creating another list style.
            // Style will not be imported, because list in the destination will be reused.
            srcPara = srcDoc.FirstSection.Body.Paragraphs[1];
            dstPara = (Paragraph)dstDoc.ImportNode(srcPara, true, ImportFormatMode.KeepSourceFormatting);
            dstDoc.FirstSection.Body.Paragraphs.Add(dstPara);

            // The "normal" list was imported first.
            Assert.That(dstPara.ListFormat.List.ListId, Is.EqualTo(1));
            Assert.That(dstPara.ListFormat.List.ListDefId, Is.EqualTo(0x7C8F5509));

            // WORDSNET-14587 Do not create new style, if such style already exists in destination.
            Assert.That(dstDoc.Styles.GetByName("Normal_0", false), Is.Null);
            Assert.That(dstDoc.Styles.GetByName("1 / a / i_0", false), Is.Null);

            TestUtil.SaveOpen(dstDoc, @"Model\Style\TestImportListStyle Modified", lf, sf);
        }

        /// <summary>
        /// category - list style
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCreateListStyle(LoadFormat lf, SaveFormat sf)
        {
            Document doc = new Document();
            Style style = doc.Styles.Add(StyleType.List, "MyListStyle");

            // Check that the default list style created has the "1 / a / i" numbering like in MS Word.
            Assert.That(style.List.ListLevels[0].NumberStyle, Is.EqualTo(NumberStyle.Arabic));
            Assert.That(style.List.ListLevels[1].NumberStyle, Is.EqualTo(NumberStyle.LowercaseLetter));
            Assert.That(style.List.ListLevels[2].NumberStyle, Is.EqualTo(NumberStyle.LowercaseRoman));

            // Modify the list style to our liking.
            style.List.ListLevels[0].NumberStyle = NumberStyle.UppercaseRoman;

            // Create a list based on the list style and use it for some text.
            List list = doc.Lists.Add(style);

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.ListFormat.List = list;
            builder.ListFormat.ListLevelNumber = 0;
            builder.Writeln("Level 1");
            builder.ListFormat.ListLevelNumber = 1;
            builder.Writeln("Level 2");
            builder.ListFormat.ListLevelNumber = 0;
            builder.Writeln("Level 1");
            builder.ListFormat.RemoveNumbers();

            doc = TestUtil.SaveOpen(doc, @"Model\Style\TestCreateListStyle", lf, sf);

            Assert.That(doc.Lists.Count, Is.EqualTo(2));
            Assert.That(doc.Lists.ListDefCount, Is.EqualTo(2));
            style = doc.Styles["MyListStyle"];
            Assert.That(style.List.ListId, Is.EqualTo(1));
            Assert.That(list.ListLevels[0].NumberStyle, Is.EqualTo(NumberStyle.UppercaseRoman));
        }

    }
}
