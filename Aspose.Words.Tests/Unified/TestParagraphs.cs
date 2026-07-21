// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using System.Text.RegularExpressions;
using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Loading;
using Aspose.Words.Replacing;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using NUnit.Framework;

//TODO 3 Cannot properly read fast saved tab positions. See TestTabPositionsFastSaved.doc.

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for paragraphs.
    /// </summary>
    [TestFixture]
    public class TestParagraphs : UnifiedTestsBase
    {
        /// <summary>
        /// Test borders on ParagraphFormat are working and being inherited properly.
        /// </summary>
        [Test]
        public void TestParagraphBordersInheritance()
        {
            Document doc = new Document();
            Style normal = doc.Styles["Normal"];
            Style h1 = doc.Styles["Heading 1"];

            Paragraph para = (Paragraph)doc.SelectSingleNode("//Paragraph[1]");
            para.ParagraphFormat.StyleName = "Heading 1";

            //The borders are not visible although the count is there.
            Assert.That(para.ParagraphFormat.Borders.Count, Is.EqualTo(6));
            Assert.That(para.ParagraphFormat.Borders[BorderType.Top].LineWidth, Is.EqualTo(0d));

            //Set some borders in the normal style.
            normal.ParagraphFormat.Borders[BorderType.Top].LineWidth = 1;
            normal.ParagraphFormat.Borders[BorderType.Left].LineWidth = 2;
            //Set some borders in Heading 1
            h1.ParagraphFormat.Borders[BorderType.Right].LineWidth = 4;

            //Check they are inherited from the styles.
            Assert.That(para.ParagraphFormat.Borders[BorderType.Top].LineWidth, Is.EqualTo(1d));
            Assert.That(para.ParagraphFormat.Borders[BorderType.Left].LineWidth, Is.EqualTo(2d));
            Assert.That(para.ParagraphFormat.Borders[BorderType.Right].LineWidth, Is.EqualTo(4d));

            //Make Heading 1 override a border specified in Normal and check it takes precedence in inheritance.
            h1.ParagraphFormat.Borders[BorderType.Left].LineWidth = 3;
            Assert.That(para.ParagraphFormat.Borders[BorderType.Left].LineWidth, Is.EqualTo(3d));

            //Set some borders on the paragraph directly and make sure they take precedence.
            para.ParagraphFormat.Borders[BorderType.Left].LineWidth = 5;
            Assert.That(para.ParagraphFormat.Borders[BorderType.Left].LineWidth, Is.EqualTo(5d));

            //A simple check to make sure all complex attributes were copied
            Assert.That(normal.ParagraphFormat.Borders[BorderType.Left], IsNot.SameAs(h1.ParagraphFormat.Borders[BorderType.Left]));
            Assert.That(h1.ParagraphFormat.Borders[BorderType.Left], IsNot.SameAs(para.ParagraphFormat.Borders[BorderType.Left]));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestParagraphInOneRun(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Para\TestParagraphInOneRun", lf, sf);
            Assert.That(doc.SelectNodes("//Run").Count, Is.EqualTo(1));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCustomColor(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Para\TestCustomColor", lf, sf);

            Paragraph para = (Paragraph)doc.SelectSingleNode("//Paragraph[2]");
            ParagraphFormat pf = para.ParagraphFormat;

            //Test shading with custom colors is set
            Assert.That(pf.Shading.ForegroundPatternColorInternal, Is.EqualTo(DrColor.FromArgb(255, 0, 0)));
            Assert.That(pf.Shading.BackgroundPatternColorInternal, Is.EqualTo(DrColor.FromArgb(250, 150, 50)));

            Assert.That(pf.Shading.Texture, Is.EqualTo(TextureIndex.Texture10Percent));
        }

        /// <summary>
        /// Test on a paragraph style that has no tab positions a few tab positions are
        /// added using direct formatting.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAddTabPositions(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Para\TestTabPositions", lf, sf);

            Paragraph para = (Paragraph)doc.SelectSingleNode("//Paragraph[1]");
            TabStopCollection tabStops = para.ParagraphFormat.TabStops;

            Assert.That(tabStops.Count, Is.EqualTo(3));
            Assert.That(tabStops.GetPositionTwipsByIndex(0), Is.EqualTo(1440));    //1"
            Assert.That(tabStops.GetPositionTwipsByIndex(1), Is.EqualTo(2880));    //2"
            Assert.That(tabStops.GetPositionTwipsByIndex(2), Is.EqualTo(5760));    //4"
            Assert.That(tabStops[2].Alignment, Is.EqualTo(TabAlignment.Right));
            Assert.That(tabStops[2].Leader, Is.EqualTo(TabLeader.Dots));
        }

        /// <summary>
        /// Some positions were on the style and some added and moved using direct formatting.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestStyleAndDirectTabPositions(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Para\TestTabPositions", lf, sf);

            //These are the tab stops on the style
            Style style = doc.Styles["RKStyle1"];
            TabStopCollection tabStops = style.ParagraphFormat.TabStops;
            Assert.That(tabStops.Count, Is.EqualTo(2));

            Assert.That(tabStops.GetPositionTwipsByIndex(0), Is.EqualTo(360));        //add 0.25"
            Assert.That(tabStops[0].Alignment, Is.EqualTo(TabAlignment.Left));

            Assert.That(tabStops.GetPositionTwipsByIndex(1), Is.EqualTo(2880));    //add 2"
            Assert.That(tabStops[1].Alignment, Is.EqualTo(TabAlignment.Left));


            //These are the tab stops on the paragraph
            Paragraph para = (Paragraph)doc.SelectSingleNode("//Paragraph[2]");
            tabStops = para.ParagraphFormat.TabStops;
            Assert.That(tabStops.Count, Is.EqualTo(3));

            Assert.That(tabStops.GetPositionTwipsByIndex(0), Is.EqualTo(2160));    //add 1.5"
            Assert.That(tabStops[0].Alignment, Is.EqualTo(TabAlignment.Left));

            Assert.That(tabStops.GetPositionTwipsByIndex(1), Is.EqualTo(2880));    //delete 2"
            Assert.That(tabStops[1].Alignment, Is.EqualTo(TabAlignment.Clear));

            Assert.That(tabStops.GetPositionTwipsByIndex(2), Is.EqualTo(7200));    //add 5"
            Assert.That(tabStops[2].Alignment, Is.EqualTo(TabAlignment.Left));
        }





        [Test]
        public void TestSetParagraphStyle()
        {
            Document doc = new Document();
            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            Assert.That(doc.Styles["Normal"], Is.EqualTo(para.ParagraphFormat.Style));
            para.ParagraphFormat.Style = doc.Styles["Heading 1"];
            Assert.That(doc.Styles["Heading 1"], Is.EqualTo(para.ParagraphFormat.Style));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestSetParagraphStyleNull()
        {
            Document doc = new Document();
            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            para.ParagraphFormat.Style = null;
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "This style belongs to a different document.")]
        public void TestSetParagraphStyleWrongDocument()
        {
            Document doc1 = new Document();
            Document doc2 = new Document();
            Paragraph para = doc1.FirstSection.Body.FirstParagraph;
            para.ParagraphFormat.Style = doc2.Styles["Normal"];
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "This style is not a paragraph style.")]
        public void TestSetParagraphStyleWrongStyleType()
        {
            Document doc = new Document();
            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            para.ParagraphFormat.Style = doc.Styles[StyleIdentifier.DefaultParagraphFont];
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDirection(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Para\TestDirection", lf, sf);
            CheckDirection(doc, 0, TextOrientation.Horizontal);
            CheckDirection(doc, 2, TextOrientation.Upward);
            CheckDirection(doc, 4, TextOrientation.VerticalFarEast);
            CheckDirection(doc, 6, TextOrientation.Downward);
            CheckDirection(doc, 8, TextOrientation.HorizontalRotatedFarEast);
        }

        private static void CheckDirection(Document doc, int paraIndex, TextOrientation orientation)
        {
            Paragraph para = (Paragraph)doc.FirstSection.Body.GetChild(NodeType.Paragraph, paraIndex, true);
            Assert.That(orientation, Is.EqualTo(para.ParagraphFormat.FrameTextOrientation));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAlignment(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestAlignment", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAlignmentAndFirstIndent(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestAlignmentAndFirstIndent", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAlignmentAndFirstIndentBig(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestAlignmentAndFirstIndentBig", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAlignmentAndFirstIndentNegative(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestAlignmentAndFirstIndentNegative", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAlignmentAndLeftIndent(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestAlignmentAndLeftIndent", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAlignmentWithTabs(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestAlignmentWithTabs", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAll(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestAll", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBorders(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestBorders", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestConforming(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestConforming", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestConformingBetween(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestConformingBetween", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDefaultPara(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestDefaultPara", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDropCap(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestDropCap", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFrame(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestFrame", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestHeadings(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestHeadings", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestIndentAndBarTab(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestIndentAndBarTab", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestIndentAndTab(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestIndentAndTab", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestIndents(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestIndents", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestJustify(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestJustify", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestJustifyWithBreaks(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestJustifyWithBreaks", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestJustifyWithTabs(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestJustifyWithTabs", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestKeeps(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestKeeps", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLineSpacing(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestLineSpacing", lf, sf);
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLineSpacingRule(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestLineSpacingRule", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLineSpacings(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestLineSpacings", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLineStyle(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestLineStyle", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestList(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestList", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestPageNumberInFrame(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestPageNumberInFrame", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestParaAlignment(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestParaAlignment", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestParaBlanks(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestParaBlanks", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestParaCustomTabs(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestParaCustomTabs", lf, sf);
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevision(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestRevision", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSpacing(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestSpacing", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestStyle(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestStyle", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTable(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestTable", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTabs(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestTabs", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTabStopWrap(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Para\TestTabStopWrap", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAutoColorOnShading(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaBorder\TestAutoColorOnShading", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBorder2(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaBorder\TestBorder2", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBorderWithShadow(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaBorder\TestBorderWithShadow", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBorderAll(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaBorder\TestBorderAll", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestColumnBreaks(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaBorder\TestColumnBreaks", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestComplex(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaBorder\TestComplex", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestConformingAtColumnEnd(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaBorder\TestConformingAtColumnEnd", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestConformingBorder(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaBorder\TestConformingBorder", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestConformingDistance(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaBorder\TestConformingDistance", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestConformingDouble(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaBorder\TestConformingDouble", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestConformingTriple(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaBorder\TestConformingTriple", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestConformingTripleCombo(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaBorder\TestConformingTripleCombo", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFitsColumn(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaBorder\TestFitsColumn", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSectionBreaks(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaBorder\TestSectionBreaks", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSpansColumn(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaBorder\TestSpansColumn", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestVariousProperties(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaBorder\TestVariousProperties", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestVerticalBorderMerged(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaBorder\TestVerticalBorderMerged", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestKeepTogether(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaKeeps\TestKeepTogether", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestKeepWithNext(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaKeeps\TestKeepWithNext", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestKeepWithNext2(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaKeeps\TestKeepWithNext2", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestTough2(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaKeeps\TestTough2", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestWidowOrphan(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaKeeps\TestWidowOrphan", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestWidowOrphan2(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaKeeps\TestWidowOrphan2", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBeforeAfter(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaSpacing\TestBeforeAfter", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBeforeAfterAuto(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaSpacing\TestBeforeAfterAuto", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBeforeAfterZeroes(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaSpacing\TestBeforeAfterZeroes", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLineSpacingAtLeast(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaSpacing\TestLineSpacingAtLeast", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLineSpacingExactly(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaSpacing\TestLineSpacingExactly", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLineSpacingExactlyWithBorders(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaSpacing\TestLineSpacingExactlyWithBorders", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLineSpacingMultiple(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaSpacing\TestLineSpacingMultiple", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestNoSpacingBetweenSameStyle(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\ParaSpacing\TestNoSpacingBetweenSameStyle", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAsiaProps(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Para\TestAsiaProps", lf);
            // Verify attributes read properly.
            VerifyFeValues(doc.GetNodeById("0.0.0"), true, true);
            VerifyFeValues(doc.GetNodeById("1.0.0"), true, false);
            VerifyFeValues(doc.GetNodeById("2.0.0"), false, true);
            VerifyFeValues(doc.GetNodeById("3.0.0"), false, false);

            // So TRUE value comes from defaults i.e collection doesn't contain corresponding direct attribute.
            VerifyFePresence(doc.GetNodeById("0.0.0"), false, false);
            VerifyFePresence(doc.GetNodeById("1.0.0"), false, true);
            VerifyFePresence(doc.GetNodeById("2.0.0"), true, false);
            VerifyFePresence(doc.GetNodeById("3.0.0"), true, true);

            doc = TestUtil.SaveOpen(doc, @"Model\Para\TestAsiaProps", lf, sf);
            // Verify attributes saved properly.
            VerifyFeValues(doc.GetNodeById("0.0.0"), true, true);
            VerifyFeValues(doc.GetNodeById("1.0.0"), true, false);
            VerifyFeValues(doc.GetNodeById("2.0.0"), false, true);
            VerifyFeValues(doc.GetNodeById("3.0.0"), false, false);
        }


        /// <summary>
        /// WORDSNET-6540 Paragraph Mirror Indenting Issue.
        /// andrnosk: Added mechanism to read/write MirrorIndents in Doc/Docx.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestMirrorIndents(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Para\TestMirrorIndents", lf, sf);
            Paragraph paragraph = (Paragraph)doc.GetNodeById("0.0.0");

            switch (TestUtil.GetUnifiedScenario(lf, sf))
            {
                case UnifiedScenario.Doc2Doc:
                case UnifiedScenario.Doc2Docx:
                case UnifiedScenario.Docx2DocxNoGold:
                case UnifiedScenario.Doc2Rtf:
                case UnifiedScenario.Rtf2RtfNoGold:
                    Assert.That((bool)paragraph.ParaPr[ParaAttr.MirrorIndents], Is.True);
                    break;
                case UnifiedScenario.Doc2Wml:
                case UnifiedScenario.Wml2WmlNoGold:
                    Assert.That(paragraph.ParaPr[ParaAttr.MirrorIndents], Is.Null);
                    break;
                default:
                    throw new InvalidOperationException("Unknown file format.");
            }
        }

        /// <summary>
        /// WORDSNET-7533 Two Paragraph should display on single line during Doc to Rtf conversion.
        /// Added SpecialHidden (Style separator) attribute reading/writing to RTF.
        /// </summary>
        /// <remarks>
        /// AM. Additionally I found that Word doesn't like NonBreakingSpace written as \~ keyword and displays them incorrectly.
        /// Fixed here and verified by Gold.
        /// </remarks>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedJira7533(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Para\TestJira7533", lf, sf);

            RunPr paraBreakRunPr = ((Paragraph)doc.GetNodeById("1.2.0")).ParagraphBreakRunPr;

            // Special case for DOC's Toggle value.
            Assert.That(paraBreakRunPr.GetDirectAttr(FontAttr.SpecialHidden), Is.EqualTo((TestUtil.GetUnifiedScenario(lf, sf) == UnifiedScenario.Doc2Doc) ? AttrBoolEx.Toggle : AttrBoolEx.True));
        }


        /// <summary>
        /// Verify values of FarEast spacing.
        /// </summary>
        private static void VerifyFeValues(Node paragraph, bool farEastAndAlpha, bool farEastAndDigit)
        {
            Assert.That(((Paragraph)paragraph).ParagraphFormat.AddSpaceBetweenFarEastAndAlpha, Is.EqualTo(farEastAndAlpha));
            Assert.That(((Paragraph)paragraph).ParagraphFormat.AddSpaceBetweenFarEastAndDigit, Is.EqualTo(farEastAndDigit));
        }

        /// <summary>
        /// Verify that property collection contains FarEast spacing attributes.
        /// </summary>
        private static void VerifyFePresence(Node paragraph, bool farEastAndAlpha, bool farEastAndDigit)
        {
            Assert.That(((Paragraph)paragraph).ParaPr.Contains(ParaAttr.AddSpaceBetweenFarEastAndAlpha), Is.EqualTo(farEastAndAlpha));

            Assert.That(((Paragraph)paragraph).ParaPr.Contains(ParaAttr.AddSpaceBetweenFarEastAndDigit), Is.EqualTo(farEastAndDigit));
        }

        /// <summary>
        /// WORDSNET-5937 While converting doc to rtf, paragraph left indent value is incorrect.
        /// Unit spacing/indent support.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestUnits(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Para\TestUnits", lf);

            Assert.That(doc.FirstSection.SectPr.GridType, Is.EqualTo(SectionLayoutMode.LineGrid));
            Assert.That(doc.FirstSection.SectPr.LinePitch, Is.EqualTo(582));
            ParaPr paraPr = doc.FirstSection.Body.FirstParagraph.ParaPr;

            // 2 line before, no after
            Assert.That(paraPr[ParaAttr.SpaceBeforeUnits], Is.EqualTo(200));
            Assert.That(paraPr[ParaAttr.SpaceAfterUnits], Is.Null);

            Assert.That(paraPr.SpaceBefore, Is.EqualTo(1164));
            Assert.That(paraPr.SpaceAfter, Is.EqualTo(0));

            // 1 ch left and 2 ch right.
            Assert.That(paraPr[ParaAttr.LeftIndentUnits], Is.EqualTo(100));
            Assert.That(paraPr[ParaAttr.RightIndentUnits], Is.EqualTo(200));

            // These attributes present in original file but are updated in DocumentValidator anyway.
            Assert.That(paraPr.LeftIndent, Is.EqualTo(200));
            Assert.That(paraPr.RightIndent, Is.EqualTo(400));

            doc = TestUtil.SaveOpen(doc, @"Model\Para\TestUnits", lf, sf);
            paraPr = doc.FirstSection.Body.FirstParagraph.ParaPr;
            // Units are preserved after open/save.
            Assert.That(paraPr[ParaAttr.SpaceBeforeUnits], Is.EqualTo(200));
            Assert.That(paraPr[ParaAttr.SpaceAfterUnits], Is.Null);
            Assert.That(paraPr[ParaAttr.LeftIndentUnits], Is.EqualTo(100));
            Assert.That(paraPr.GetDirectAttr(ParaAttr.RightIndentUnits), Is.EqualTo(200));

            // Twips indents are updated properly and the same as in original document.
            Assert.That(paraPr.LeftIndent, Is.EqualTo(200));
            Assert.That(paraPr.RightIndent, Is.EqualTo(400));

            // Modify Normal Style and DocumentGrid line pitch.
            Style normal = doc.Styles.GetBySti(StyleIdentifier.Normal, true);
            normal.RunPr.Size = 96; // 48 pt.
            doc.FirstSection.SectPr.LinePitch = 360;

            // Twips values are changed after open/save.
            doc = TestUtil.SaveOpen(doc, @"Model\Para\TestUnits Modified", lf, sf);
            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            paraPr = para.ParaPr;

            Assert.That(paraPr[ParaAttr.SpaceBeforeUnits], Is.EqualTo(200));                   // the same.
            Assert.That(paraPr.SpaceBefore, Is.EqualTo(720));                                  // was 1164.

            Assert.That(paraPr.GetDirectAttr(ParaAttr.LeftIndentUnits), Is.EqualTo(100));      // the same.
            Assert.That(paraPr.LeftIndent, Is.EqualTo(960));                                   // was 100.

            // Test public setter removes units.
            Assert.That(paraPr[ParaAttr.SpaceBeforeUnits], IsNot.Null());
            para.ParagraphFormat.SpaceBefore = 100;
            Assert.That(paraPr[ParaAttr.SpaceBeforeUnits], Is.Null);

            Assert.That(paraPr[ParaAttr.LeftIndentUnits], IsNot.Null());
            para.ParagraphFormat.LeftIndent = 100;
            Assert.That(paraPr[ParaAttr.LeftIndentUnits], Is.Null);
        }

        /// <summary>
        /// WORDSNET-6998 Support suppressOverlap paragraph attribute.
        /// For details see:
        ///  1. DOCX - part 1 reference c059575_ISO_IEC_29500-1_2011 chapter 17.3.1.36 suppressOverlap (Prevent Text Frames From Overlapping),
        ///  2. DOC - MS-DOC chapter 2.6.2 Paragraph Properties,
        ///  3. Rtf - Word2007RTFSpec1.9.1 chapter Positioned Objects and Frames.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFramesSuppressOverlap(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Para\TestFramesSuppressOverlap", lf, sf);
            Assert.That(((Paragraph)doc.GetNodeById("5.0.0")).ParaPr.FrameSuppressOverlap, Is.True);
            Assert.That(((Paragraph)doc.GetNodeById("3.0.0")).ParaPr.FrameSuppressOverlap, Is.False);
        }

        /// <summary>
        /// WORDSNET-5375 Div elements are not supported in the model.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestHtmlBlock5375(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Para\TestJira5375", lf, sf);

            UnifiedScenario scenario = TestUtil.GetUnifiedScenario(lf, sf);
            switch (scenario)
            {
                case UnifiedScenario.Docx2DocxNoGold:
                case UnifiedScenario.Wml2WmlNoGold:
                    Assert.That(doc.HtmlBlockCollection.Count, Is.EqualTo(1));
                    break;
                default:
                    Assert.That(doc.HtmlBlockCollection.Count, Is.EqualTo(2));
                    break;
            }

            Paragraph para = (Paragraph)doc.GetChild(NodeType.Paragraph, 1, true);
            int htmlBlockId = (int)para.ParaPr.GetDirectAttr(ParaAttr.HtmlBlockId);

            HtmlBlock htmlBlock = doc.HtmlBlockCollection.GetHtmlBlockById(htmlBlockId);
            Assert.That(htmlBlock.HtmlBlockType, Is.EqualTo(HtmlBlockType.Div));
            Assert.That(htmlBlock.ParaPr.GetDirectAttr(ParaAttr.HtmlMarginLeft), Is.EqualTo(720));
            Assert.That(htmlBlock.ParaPr.Contains(ParaAttr.HtmlMarginRight), Is.False);

            // Verify expanding.
            Assert.That(para.ParaPr.Contains(ParaAttr.LeftIndent), Is.False);
            ParaPr expParaPr = para.GetExpandedParaPr(ParaPrExpandFlags.Layout);
            Assert.That(expParaPr.GetDirectAttr(ParaAttr.HtmlMarginLeft), Is.EqualTo(720));
            Assert.That(expParaPr.LeftIndent, Is.EqualTo(0));
        }



        /// <summary>
        /// Relates to WORDSNET-7898 Going to collect here various attribute collections
        /// to quickly test that ParaPr.IsFloating works correctly.
        /// </summary>
        [Test]
        public void TestParaPrIsFloating()
        {
            ParaPr paraPr = new ParaPr();

            // Insane test.
            Assert.That(paraPr.IsFloating, Is.False);

            // TestJira7898.doc
            paraPr.Clear();
            paraPr.SetAttr(ParaAttr.FrameRelativeHorizontalPosition, RelativeHorizontalPosition.Column);
            paraPr.SetAttr(ParaAttr.FrameRelativeVerticalPosition, RelativeVerticalPosition.Page);
            Assert.That(paraPr.IsFloating, Is.False);

            // TestJira7720.docx
            paraPr.Clear();
            paraPr.SetAttr(ParaAttr.FrameRelativeHorizontalPosition, RelativeHorizontalPosition.Column);
            paraPr.SetAttr(ParaAttr.FrameRelativeVerticalPosition, RelativeVerticalPosition.TextFrameDefault);
            paraPr.SetAttr(ParaAttr.FrameWrapType, WrapType.Square);
            Assert.That(paraPr.IsFloating, Is.True);

            // TestDropCap.doc
            paraPr.Clear();
            paraPr.SetAttr(ParaAttr.FrameRelativeHorizontalPosition, RelativeHorizontalPosition.Column);
            paraPr.SetAttr(ParaAttr.FrameRelativeVerticalPosition, RelativeVerticalPosition.TextFrameDefault);
            paraPr.SetAttr(ParaAttr.FrameWrapType, WrapType.Square);
            paraPr.SetAttr(ParaAttr.FrameHorizontalDistanceFromText, 80);
            Assert.That(paraPr.IsFloating, Is.True);

            // TestFloaterWrappedPositions.docx
            paraPr.Clear();
            paraPr.SetAttr(ParaAttr.FrameLeft, 1081);
            paraPr.SetAttr(ParaAttr.FrameTop, 433);
            paraPr.SetAttr(ParaAttr.FrameWidth, 2894);
            paraPr.SetAttr(ParaAttr.FrameRelativeHorizontalPosition, RelativeHorizontalPosition.Page);
            paraPr.SetAttr(ParaAttr.FrameRelativeVerticalPosition, RelativeVerticalPosition.TextFrameDefault);
            paraPr.SetAttr(ParaAttr.FrameWrapType, WrapType.Square);
            paraPr.SetAttr(ParaAttr.FrameVerticalDistanceFromText, 187);
            paraPr.SetAttr(ParaAttr.FrameHorizontalDistanceFromText, 187);
            Assert.That(paraPr.IsFloating, Is.True);
        }


        /// <summary>
        /// WORDSNET-5641 Indent of the second line of list item is incorrect after rendering.
        /// TabStop expanding was fixed recently and I think additional test is not superfluous due to issue complexity.
        /// </summary>
        [Test]
        public void TestJira5641()
        {
            Document doc = TestUtil.Open(@"Model\Para\TestJira5641.docx");
            Node[] nodes = doc.GetChildNodes(NodeType.Paragraph, true).ToArray();

            Paragraph para2 = (Paragraph)nodes[1];
            ParaPr paraPr2 = para2.GetExpandedParaPr(ParaPrExpandFlags.DocumentDefaults);

            TabStopCollection tabStops = paraPr2.TabStops;
            Assert.That(tabStops.Count, Is.EqualTo(2));

            CheckTabStop(tabStops[0], TabAlignment.List, 10.8);
            CheckTabStop(tabStops[1], TabAlignment.Clear, 28.8);
        }



        /// <summary>
        /// WORDSNET-7279 providing a JoinRunsWithSameFormatting method that works on Paragraph level
        /// </summary>
        [Test]
        public void TestJira7279()
        {
            Document doc = new Document();
            Paragraph p1 = new Paragraph(doc);
            Paragraph p2 = new Paragraph(doc);

            p1.AppendChild(new Run(doc, "Join1"));
            p1.AppendChild(new Run(doc, "Join2"));
            p1.AppendChild(new Run(doc, "Join3"));

            p2.AppendChild(new Run(doc, "Join1"));
            p2.AppendChild(new Run(doc, "Join2"));
            p2.AppendChild(new Run(doc, "Join3"));

            // All p1 runs different, thus no joins have to be performed.
            p1.Runs[0].RunPr.SetAttr(FontAttr.Size, 72);
            p1.Runs[1].RunPr.SetAttr(FontAttr.Bold, AttrBoolEx.True);
            p1.Runs[2].RunPr.SetAttr(FontAttr.Color, DrColor.DarkCyan);
            Assert.That(p1.JoinRunsWithSameFormatting(), Is.EqualTo(0));

            // All runs in p2 paragraph have same formatting, thus 2 joins have to be performed
            Assert.That(p2.JoinRunsWithSameFormatting(), Is.EqualTo(2));
        }

        private static void CheckTabStop(TabStop tab, TabAlignment alignment, double position)
        {
            Assert.That(tab.Alignment, Is.EqualTo(alignment));
            Assert.That(tab.Position, Is.EqualTo(position));
        }

        /// <summary>
        /// WORDSNET-8657 DropCapPostion property is not working.
        /// The problem occurs because it was not enough to set DropCapPosition to none,
        /// we also have to concatenate DropCap frame paragraph with original paragraph and restore the run formatting.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSetDropCapPositionToNone(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Para\TesJira8657", lf);
            NodeCollection paragraphs = doc.FirstSection.Body.Paragraphs;
            Assert.That(paragraphs.Count, Is.EqualTo(11));

            Paragraph parWithDrop = doc.FirstSection.Body.Paragraphs[4];
            Assert.That(parWithDrop.ParagraphFormat.DropCapPosition, Is.EqualTo(DropCapPosition.Normal));
            Assert.That(parWithDrop.FirstRun.Font.Size, Is.EqualTo(83.0d));

            // Set DropCapPosition to None for all paragraphs inside document.
            foreach (Paragraph para in paragraphs)
                para.ParagraphFormat.DropCapPosition = DropCapPosition.None;

            doc = TestUtil.SaveOpen(doc, @"Model\Para\TesJira8657", TestUtil.GetUnifiedScenario(lf, sf) | UnifiedScenario.NoGold);
            paragraphs = doc.FirstSection.Body.Paragraphs;
            Assert.That(paragraphs.Count, Is.EqualTo(8));

            parWithDrop = doc.FirstSection.Body.Paragraphs[3];
            Assert.That(parWithDrop.ParagraphFormat.DropCapPosition, Is.EqualTo(DropCapPosition.None));
            Assert.That(parWithDrop.FirstRun.Font.Size, Is.EqualTo(11.0d));
        }


        /// <summary>
        /// Relates to WORDSNET-10090
        /// Tests how ParaPr.IsFloating works on various combination of frame attributes.
        /// </summary>
        [Test]
        public void TestIsFloating()
        {
            // <w:framePr w:xAlign="left" w:yAlign="center" />
            VerifyIsFloating(@"Model\Para\TestIsFloating A.xml", true);

            // <w:framePr w:xAlign="left" w:yAlign="inline" />
            VerifyIsFloating(@"Model\Para\TestIsFloating B.xml", false);

            // Both FrameTop and FrameLeft are 0. Paragraph is not floating.
            // <w:framePr w:x="0" w:y="0" />
            VerifyIsFloating(@"Model\Para\TestIsFloating C.xml", false);

            // FrameTop is greater than 0. Paragraph is floating.
            // <w:framePr w:x="0" w:y="1" />
            VerifyIsFloating(@"Model\Para\TestIsFloating D.xml", true);

            // Only FrameWidth is defined. Paragraph is floating.
            // <w:framePr w:w="1701" />
            VerifyIsFloating(@"Model\Para\TestIsFloating E.xml", true);

            // Non default RelativeHorizontalPosition. Paragraph is floating.
            //<w:framePr w:hAnchor="page" />
            VerifyIsFloating(@"Model\Para\TestIsFloating F.xml", true);

            // Zero FrameWidth. Paragraph is not floating.
            //<w:framePr w:w="0" />
            VerifyIsFloating(@"Model\Para\TestIsFloating G.xml", false);

            // Default RelativeHorizontalPosition. Paragraph is not floating.
            // <w:framePr w:hAnchor="text" />
            VerifyIsFloating(@"Model\Para\TestIsFloating H.xml", false);

            // Only DistanceToText is specified. Paragraph is not floating.
            // <w:framePr w:hSpace="57" />
            VerifyIsFloating(@"Model\Para\TestIsFloating I.xml", false);

            // RelativeVerticalPosition is Page. Paragraph is not floating.
            // <w:framePr w:hAnchor="text" w:vAnchor="page" />
            VerifyIsFloating(@"Model\Para\TestIsFloating J.xml", false);

            // RelativeVerticalPosition is Margin. Paragraph is not floating.
            // <w:framePr w:hAnchor="text" w:vAnchor="margin" />
            VerifyIsFloating(@"Model\Para\TestIsFloating K.xml", false);

            // RelativeVerticalPosition is Text. Paragraph is floating.
            // <w:framePr w:hAnchor="text" w:vAnchor="text" />
            VerifyIsFloating(@"Model\Para\TestIsFloating L.xml", true);

            // Only RelativeVerticalPosition Text is specified. Paragraph is floating.
            // <w:framePr w:vAnchor="text" />
            VerifyIsFloating(@"Model\Para\TestIsFloating M.xml", true);
        }


        /// <summary>
        /// Relates to WORDSNET-10090
        /// </summary>
        [Test]
        public void TestJira8883()
        {
            Document doc = TestUtil.Open(@"Model\Para\TestJira8883.docx");
            Paragraph p = doc.FirstSection.Body.Paragraphs[0];
            Assert.That(p.GetExpandedParaPr(ParaPrExpandFlags.Layout).IsFloating, Is.True);
        }



        /// <summary>
        /// WORDSNET-4870 Paragraph alignment “Justify Low” is changed to “Justified” after open/save.
        /// </summary>
        /// <remarks>
        /// AM. I think we are not ready to expose Arabic alignment to public, probably we even need separate build for it.
        /// That's why I decided to fix roundtrip in this strange way.
        /// As a result document is roundtriped properly but customer has no access to Arabic alignment types.
        /// </remarks>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestJira4870(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Para\TestJira4870", TestUtil.GetUnifiedScenario(lf, sf) | UnifiedScenario.NoGold);
            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;

            Assert.That(paras[0].ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.Left));
            Assert.That(paras[1].ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.Center));
            Assert.That(paras[2].ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.Right));
            Assert.That(paras[3].ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.Justify));
            Assert.That(paras[4].ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.Distributed));
            Assert.That(paras[5].ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.ArabicLowKashida));
            Assert.That(paras[6].ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.ArabicMediumKashida));
            Assert.That(paras[7].ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.ArabicHighKashida));
            Assert.That(paras[8].ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.ThaiDistributed));
        }








        /// <summary>
        /// Relates to WORDSNET-12849 Checks that units conversion is correct for all paragraphs.
        /// </summary>
        [Test]
        public void TestJira12849A()
        {
            Document srcDoc = TestUtil.Open(@"Model\Para\TestUnits ms.docx");
            Document dstDoc = TestUtil.Open(@"Model\Para\TestUnits ms.docx");

            // Update indents in one of document.
            TestUtil.ExecuteValidator(dstDoc, SaveFormat.Docx);   // FOSS: was Pdf.

            // Verify indents are not changed.
            for (int i = 0; i < srcDoc.FirstSection.Body.Paragraphs.Count; i++)
            {
                ParaPr srcParaPr = srcDoc.FirstSection.Body.Paragraphs[i].ParaPr;
                ParaPr dstParaPr = dstDoc.FirstSection.Body.Paragraphs[i].ParaPr;

                // Check all indent attributes.
                Assert.That(dstParaPr.FetchAttr(ParaAttr.FirstLineIndent), Is.EqualTo(srcParaPr.FetchAttr(ParaAttr.FirstLineIndent)));
                Assert.That(dstParaPr.FetchAttr(ParaAttr.LeftIndent), Is.EqualTo(srcParaPr.FetchAttr(ParaAttr.LeftIndent)));
                Assert.That(dstParaPr.FetchAttr(ParaAttr.RightIndent), Is.EqualTo(srcParaPr.FetchAttr(ParaAttr.RightIndent)));

                // We don't update units but check them all as well.
                Assert.That(dstParaPr.FetchAttr(ParaAttr.FirstLineIndentUnits), Is.EqualTo(srcParaPr.FetchAttr(ParaAttr.FirstLineIndentUnits)));
                Assert.That(dstParaPr.FetchAttr(ParaAttr.LeftIndentUnits), Is.EqualTo(srcParaPr.FetchAttr(ParaAttr.LeftIndentUnits)));
                Assert.That(dstParaPr.FetchAttr(ParaAttr.RightIndentUnits), Is.EqualTo(srcParaPr.FetchAttr(ParaAttr.RightIndentUnits)));
            }
        }

        /// <summary>
        /// WORDSNET-14113 Add feature to support Frames in Aspose.Words
        /// Frame properties are exposed public.
        /// </summary>
        [Test]
        public void TestJira14113()
        {
            Document doc = TestUtil.Open(@"Model\Para\TestJira14113.xml");

            Style normal = doc.Styles["Normal"];
            Assert.That(normal.FrameFormat.HorizontalPosition, Is.EqualTo(0.0d));
            Assert.That(normal.FrameFormat.VerticalPosition, Is.EqualTo(0.0d));
            Assert.That(normal.FrameFormat.IsFrame, Is.False);

            Style framed = doc.Styles["Framed"];
            Assert.That(framed.FrameFormat.HorizontalPosition, Is.EqualTo(0.0d));
            Assert.That(framed.FrameFormat.Width, Is.EqualTo(85.05).Within(.01));
            Assert.That(framed.FrameFormat.IsFrame, Is.True);

            ParagraphCollection paras = doc.FirstSection.Body.Paragraphs;
            Assert.That(paras[0].FrameFormat.IsFrame, Is.False);
            Assert.That(paras[1].FrameFormat.IsFrame, Is.True);
            Assert.That(paras[2].FrameFormat.IsFrame, Is.True);
        }


        /// <summary>
        /// WORDSNET-14123 Unsupported elements that are used for simultaneously editing of a document
        /// Added roundtrip for paraId and textId attributes.
        /// </summary>
        [Test]
        public void TestJira14123A()
        {
            const string testName = @"Model\Para\TestJira14123A.docx";

            Document doc = TestUtil.Open(testName);
            OoxmlSaveOptions so = (OoxmlSaveOptions)SaveOptions.CreateSaveOptions(SaveFormat.Docx);
            so.WriteExtendedIds = true;
            doc = TestUtil.SaveOpen(doc, testName, UnifiedScenario.Docx2DocxNoGold, so);

            Paragraph p = doc.FirstSection.Body.FirstParagraph;
            Assert.That(p.ParaId, Is.EqualTo(0x07891758));
            Assert.That(p.TextId, Is.EqualTo(0x77777777));
        }

        /// <summary>
        /// Relates to WORDSNET-14123 Tests that textId and paraId is preserved for rows.
        /// </summary>
        [Test]
        public void TestJira14123B()
        {
            const string testName = @"Model\Para\TestJira14123B.docx";

            Document doc = TestUtil.Open(testName);
            OoxmlSaveOptions so = (OoxmlSaveOptions)SaveOptions.CreateSaveOptions(SaveFormat.Docx);
            so.WriteExtendedIds = true;
            doc = TestUtil.SaveOpen(doc, testName, UnifiedScenario.Docx2DocxNoGold, so);

            Row row = doc.FirstSection.Body.Tables[0].FirstRow;
            Assert.That(row.ParaId, Is.EqualTo(0x111bfe40));
            Assert.That(row.TextId, Is.EqualTo(0x77777777));
        }

        // FOSS: TestJira15260 removed — it loaded a .dot (removed format) and asserted a layout-computed
        // expansion (GetExpandedParaPr(Layout)); both .dot load and the layout engine are gone.

        /// <summary>
        /// WORDSNET-18436 Provide API to identify Style Separator Paragraph.
        /// <see cref="Paragraph.BreakIsStyleSeparator"/> public property was introduced.
        /// </summary>
        [Test]
        public void TestJira18436()
        {
            const string testName = @"Model\Para\TestJira18436.docx";

            Document doc = TestUtil.Open(testName);
            Paragraph p = doc.FirstSection.Body.FirstParagraph;
            Assert.That(p.BreakIsStyleSeparator, Is.True);
        }


        /// <summary>
        /// WORDSNET-19313 DOC to Fixed page format conversion issue with page header
        /// Any inline including shapes should be considered in twips per unit calculation.
        /// </summary>
        [Test]
        public void Test19313()
        {
            Document doc = TestUtil.Open(@"Model\Para\Test19313.docx");

            TestUtil.ExecuteValidator(doc, SaveFormat.Docx);

            ParaPr paraPr = doc.FirstSection.Body.FirstParagraph.ParaPr;
            Assert.That(paraPr[ParaAttr.FirstLineIndentUnits], Is.EqualTo(-1000));

            Assert.That(paraPr[ParaAttr.LeftIndent], Is.EqualTo(1600));
            Assert.That(paraPr[ParaAttr.FirstLineIndent], Is.EqualTo(-1600));
        }





        /// <summary>
        /// WORDSNET-19747 Add feature to set/get paragraph property "Snap to grid when document grid is defined"
        /// SnapToGrid Font/ParagraphFormat property was added.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTest19747(LoadFormat lf, SaveFormat sf)
        {
            //Check that property is reading well
            Document doc = TestUtil.Open(@"Model\Para\Test19747", lf);
            CheckSnapToGrid(doc, false);

            //Check that property is reading well after save
            doc = TestUtil.SaveOpen(doc, @"Model\Para\Test19747", TestUtil.GetUnifiedScenario(lf, sf) | UnifiedScenario.NoGold);
            CheckSnapToGrid(doc, false);

            //Check that property is reading well after setting values and save
            ToggleSnapToGrid(doc);
            doc = TestUtil.SaveOpen(doc, @"Model\Para\Test19747", TestUtil.GetUnifiedScenario(lf, sf) | UnifiedScenario.NoGold);
            CheckSnapToGrid(doc, true);
        }

        /// <summary>
        /// WORDSNET-19752 SpaceBefore and SpaceAfter return incorrect value for MS Word 2007 document
        /// Added unit conversion to DocumentPostLoader.
        /// </summary>
        [Test]
        public void Test19752()
        {
            Document doc = TestUtil.Open(@"Model\Para\Test19752.docx");

            ParagraphFormat format = doc.FirstSection.Body.FirstParagraph.ParagraphFormat;
            Assert.That(format.LeftIndent, Is.EqualTo(10.5));
            Assert.That(format.RightIndent, Is.EqualTo(21));
            Assert.That(format.SpaceBefore, Is.EqualTo(15.6));
            Assert.That(format.SpaceAfter, Is.EqualTo(31.2));
        }

        /// <summary>
        /// Relates to WORDSNET-19752
        /// Tests customer documents.
        /// </summary>
        [Test]
        public void Test19752A()
        {
            Document doc = TestUtil.Open(@"Model\Para\Test19752_2007.docx");
            Assert.That(doc.FirstSection.Body.FirstParagraph.ParaPr[ParaAttr.SpaceBefore], Is.EqualTo(312));

            doc = TestUtil.Open(@"Model\Para\Test19752_2016.docx");
            Assert.That(doc.FirstSection.Body.FirstParagraph.ParaPr[ParaAttr.SpaceBefore], Is.EqualTo(312));
        }

        /// <summary>
        /// WORDSNET-20001 Provide API to get set Word for Chinese specific Paragraph formatting
        /// Added unit spacings and indents to public API.
        /// </summary>
        [Test]
        public void Test20001()
        {
            Document doc = TestUtil.Open(@"Model\Para\Test20001.docx");
            ParagraphFormat format = doc.FirstSection.Body.FirstParagraph.ParagraphFormat;

            // Check preconditions.
            Assert.That(format.LineUnitBefore, Is.EqualTo(1));
            Assert.That(format.LineUnitAfter, Is.EqualTo(2));
            Assert.That(format.CharacterUnitLeftIndent, Is.EqualTo(1));
            Assert.That(format.CharacterUnitRightIndent, Is.EqualTo(2));
            Assert.That(format.CharacterUnitFirstLineIndent, Is.EqualTo(0));

            Assert.That(format.SpaceBefore, Is.EqualTo(15.6));
            Assert.That(format.SpaceAfter, Is.EqualTo(31.2).Within(0.01));
            Assert.That(format.LeftIndent, Is.EqualTo(10.5));
            Assert.That(format.RightIndent, Is.EqualTo(21));
            Assert.That(format.FirstLineIndent, Is.EqualTo(0));

            // Set units.
            format.LineUnitBefore = 10;
            Assert.That(format.LineUnitBefore, Is.EqualTo(10));
            Assert.That(format.SpaceBefore, Is.EqualTo(156));

            format.LineUnitAfter = 20;
            Assert.That(format.LineUnitAfter, Is.EqualTo(20));
            Assert.That(format.SpaceAfter, Is.EqualTo(312));

            format.CharacterUnitLeftIndent = 30;
            Assert.That(format.CharacterUnitLeftIndent, Is.EqualTo(30));
            Assert.That(format.LeftIndent, Is.EqualTo(315));

            format.CharacterUnitRightIndent = 40;
            Assert.That(format.CharacterUnitRightIndent, Is.EqualTo(40));
            Assert.That(format.RightIndent, Is.EqualTo(420));

            format.CharacterUnitFirstLineIndent = 50;
            Assert.That(format.CharacterUnitFirstLineIndent, Is.EqualTo(50));
            Assert.That(format.FirstLineIndent, Is.EqualTo(525));
        }

        /// <summary>
        /// Changed type for CharacterUnitLeftIndent, CharacterUnitRightIndent, CharacterUnitFirstLineIndent,
        /// LineUnitBefore and LineUnitAfter.
        /// </summary>
        [Test]
        public void Test20001Double()
        {
            Document doc = TestUtil.Open(@"Model\Para\Test20001.docx");
            ParagraphFormat format = doc.FirstSection.Body.FirstParagraph.ParagraphFormat;

            format.CharacterUnitLeftIndent = 0.5;
            format.CharacterUnitRightIndent = 1.5;
            format.CharacterUnitFirstLineIndent = 2.5;
            format.LineUnitBefore = 3.5;
            format.LineUnitAfter = 4.5;

            doc = TestUtil.SaveOpen(doc, @"Model\Para\Test20001.docx");

            format = doc.FirstSection.Body.FirstParagraph.ParagraphFormat;
            Assert.That(format.CharacterUnitLeftIndent, Is.EqualTo(0.5));
            Assert.That(format.CharacterUnitRightIndent, Is.EqualTo(1.5));
            Assert.That(format.CharacterUnitFirstLineIndent, Is.EqualTo(2.5));
            Assert.That(format.LineUnitBefore, Is.EqualTo(3.5));
            Assert.That(format.LineUnitAfter, Is.EqualTo(4.5));
        }

        /// <summary>
        /// WORDSNET-21040 Cell text is rendered at right side in output PDF.
        /// CharacterUnitLeftIndent and CharacterUnitFirstLineIndent should be ignored while expanding
        /// paragraph attributes with ListId explicitly set to 0. Some details can be also found
        /// in 17.9.18 numId (Numbering Definition Instance Reference) of ISO29500-1.
        /// </summary>
        // FOSS: only Docx survives — Pdf/Rtf/WordML save were removed.
        [TestCase(SaveFormat.Docx)]
        public void Test21040(SaveFormat sf)
        {
            const string testFileName = @"Model\Para\Test21040";
            Document doc = TestUtil.Open(testFileName, LoadFormat.Docx);

            // The problematic paragraph.
            Paragraph para = doc.FirstSection.Body.Tables[0].FirstRow.FirstCell.FirstParagraph;
            ParaPr paraPrExp = para.GetExpandedParaPr(ParaPrExpandFlags.Normal);
            Assert.That(paraPrExp[ParaAttr.LeftIndentUnits], Is.Null);
            Assert.That(paraPrExp[ParaAttr.FirstLineIndentUnits], Is.Null);

            Assert.That(para.ParagraphFormat.CharacterUnitLeftIndent, Is.EqualTo(0));
            Assert.That(para.ParagraphFormat.CharacterUnitFirstLineIndent, Is.EqualTo(0));

                string outFileName = string.Format("{0}{1}", testFileName, FileFormatUtil.SaveFormatToExtension(sf));
                TestUtil.Save(doc, outFileName, SaveOptions.CreateSaveOptions(sf), true);
        }




        /// <summary>
        /// WORDSNET-26172 Add public API to get/set MirrorIndents paragraph property.
        /// Tests MirrorIndents public API in ParagraphFormat.
        /// </summary>
        [Test]
        public void Test26172()
        {
            const string testFile = @"Model\Para\Test26172";
            Document doc = TestUtil.Open(testFile, LoadFormat.Docx);
            ParagraphFormat paraFormat1 = doc.FirstSection.Body.Paragraphs[0].ParagraphFormat;
            ParagraphFormat paraFormat2 = doc.FirstSection.Body.Paragraphs[1].ParagraphFormat;

            Assert.That(paraFormat1.MirrorIndents, Is.False);
            Assert.That(paraFormat2.MirrorIndents, Is.True);

            paraFormat1.MirrorIndents = true;
            paraFormat2.MirrorIndents = false;

            doc = TestUtil.SaveOpen(doc, testFile, UnifiedScenario.Docx2DocxNoGold);
            paraFormat1 = doc.FirstSection.Body.Paragraphs[0].ParagraphFormat;
            paraFormat2 = doc.FirstSection.Body.Paragraphs[1].ParagraphFormat;

            Assert.That(paraFormat1.MirrorIndents, Is.True);
            Assert.That(paraFormat2.MirrorIndents, Is.False);
        }

        /// <summary>
        /// WORDSNET-25493 Table row is moved to the next page after rendering.
        /// Resolved per WORDSNET-26551, just added test.
        /// </summary>
        [Test]
        public void Test25493()
        {
            Document doc = TestUtil.Open(@"Model\Para\Test25493.docx");
            Paragraph para = doc.FirstSection.HeadersFooters[HeaderFooterType.FooterPrimary].FirstParagraph;

            Assert.That(para.ParaPr[ParaAttr.SpaceBeforeUnits], Is.EqualTo(2900));
            Assert.That(para.ParaPr[ParaAttr.SpaceBefore], Is.EqualTo(6960));
        }









        /// <summary>
        /// Change false to true and true to false for the SnapToGrid property
        /// of the test document content.
        /// </summary>
        private static void ToggleSnapToGrid(Document doc)
        {
            for (int i = 0; i < 4; i++)
            {
                Paragraph para = doc.FirstSection.Body.Paragraphs[i];
                para.ParagraphFormat.SnapToGrid = !para.ParagraphFormat.SnapToGrid;
                para.FirstRun.Font.SnapToGrid = !para.FirstRun.Font.SnapToGrid;
            }
        }

        /// <summary>
        /// Check SnapToGrid values for the test document content.
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="isPropertyToggled"></param>
        private static void CheckSnapToGrid(Document doc, bool isPropertyToggled)
        {
            //The SnapToGrid property has the following distribution when isPropertyToggled is false
            //When isPropertyToggled is true, true and false values are swapped
            //
            //      | PARA  | FONT  |
            // Par1 | true  | true  |
            // Par2 | false | false |
            // Par3 | true  | false |
            // Par4 | false | true  |

            bool toggledTrue = isPropertyToggled ? false : true;
            bool toggledFalse = isPropertyToggled ? true : false;

            Paragraph par1 = doc.FirstSection.Body.Paragraphs[0];
            Paragraph par2 = doc.FirstSection.Body.Paragraphs[1];
            Paragraph par3 = doc.FirstSection.Body.Paragraphs[2];
            Paragraph par4 = doc.FirstSection.Body.Paragraphs[3];

            Assert.That(par1.ParagraphFormat.SnapToGrid, Is.EqualTo(toggledTrue));
            Assert.That(par2.ParagraphFormat.SnapToGrid, Is.EqualTo(toggledFalse));
            Assert.That(par3.ParagraphFormat.SnapToGrid, Is.EqualTo(toggledTrue));
            Assert.That(par4.ParagraphFormat.SnapToGrid, Is.EqualTo(toggledFalse));

            Assert.That(par1.FirstRun.Font.SnapToGrid, Is.EqualTo(toggledTrue));
            Assert.That(par2.FirstRun.Font.SnapToGrid, Is.EqualTo(toggledFalse));
            Assert.That(par3.FirstRun.Font.SnapToGrid, Is.EqualTo(toggledFalse));
            Assert.That(par4.FirstRun.Font.SnapToGrid, Is.EqualTo(toggledTrue));

        }

        /// <summary>
        /// Helper method. Verifies ParaPr.IsFloating result for fully resolved first paragraph in given document.
        /// </summary>
        private static void VerifyIsFloating(string fileName, bool isFloating)
        {
            Document doc = TestUtil.Open(fileName);
            Paragraph p = doc.FirstSection.Body.Paragraphs[0];
            Assert.That(p.GetExpandedParaPr(ParaPrExpandFlags.Layout).IsFloating, Is.EqualTo(isFloating));
        }
    }
}
