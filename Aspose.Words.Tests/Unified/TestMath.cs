// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/02/2011 by Denis Darkin

using System;
using System.Text;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Loading;
using Aspose.Words.Math;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Test suite for Office Math feature.
    /// </summary>
    [TestFixture]
    public class TestMath : UnifiedTestsBase
    {
        [SetUp]
        public void EachTestSetUp()
        {
            RandomUtil.Reset();
        }

        /// <summary>
        /// Test that bookmarks inside Office Math subtree are roundtripped in/out of the model.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestMathBookmarks(LoadFormat lf, SaveFormat sf)
        {
            LoadOptions lo = new LoadOptions();
            lo.ConvertShapeToOfficeMath = true;

            Document doc = CheckAndLoadDocument(@"Model\Math\TestMathBookmarks", lf, sf, lo);

            OfficeMath node = GetOfficeMathTestNode(doc);
            Assert.That(node.MathObjectType, Is.EqualTo(MathObjectType.NAry));

            node = (OfficeMath)node.GetChildNodes(NodeType.Any, false)[0];
            Assert.That(node.MathObjectType, Is.EqualTo(MathObjectType.SubscriptPart));

            node = (OfficeMath)node.GetChildNodes(NodeType.Any, false)[0];
            Assert.That(node.MathObjectType, Is.EqualTo(MathObjectType.Array));

            node = (OfficeMath)node.GetChildNodes(NodeType.Any, false)[1];
            Assert.That(node.MathObjectType, Is.EqualTo(MathObjectType.Argument));

            Assert.That(node.GetChildNodes(NodeType.Any, false)[1].NodeType, Is.EqualTo(NodeType.BookmarkStart));
            Assert.That(node.GetChildNodes(NodeType.Any, false)[2].NodeType, Is.EqualTo(NodeType.Run));
            Assert.That(node.GetChildNodes(NodeType.Any, false)[3].NodeType, Is.EqualTo(NodeType.BookmarkEnd));
        }


        /// <summary>
        /// Test document-wide math properties.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestMathProperties(LoadFormat lf, SaveFormat sf)
        {
            //WML format doesn't support default math properties.
            if (sf == SaveFormat.WordML)
                return;

            const string testName = @"Model\Math\TestMathProperties";

            Document doc = new Document();
            MathProperties mathPr = doc.DocPr.MathProperties;
            mathPr.BreakOnBinary = MathBreakOnBinary.Repeat;
            mathPr.BreakOnBinarySubtraction = MathBreakOnBinarySubtraction.PlusMinus;
            mathPr.DefaultFont = "Times New Roman";
            mathPr.DefaultJustification = OfficeMathJustification.Right;
            mathPr.IntegralLimitLocation = MathLimitLocation.UnderOver;
            mathPr.NaryLimitLocation = MathLimitLocation.SubscriptSuperscript;
            mathPr.LeftMargin = 10;
            mathPr.RightMargin = 20;
            mathPr.InterEquationSpacing = 30;
            mathPr.IntraEquationSpacing = 40;
            mathPr.PostParagraphSpacing = 50;
            mathPr.PreParagraphSpacing = 60;
            mathPr.IsSmallFraction = true;
            mathPr.UseDisplayMathDefaults = false;

            MathProperties test = TestUtil.SaveOpen(doc, testName, lf, sf).DocPr.MathProperties;

            if (lf != LoadFormat.Doc)
            {
                Assert.That(test.BreakOnBinary, Is.EqualTo(MathBreakOnBinary.Repeat));
                Assert.That(test.BreakOnBinarySubtraction, Is.EqualTo(MathBreakOnBinarySubtraction.PlusMinus));
                Assert.That(test.DefaultFont, Is.EqualTo("Times New Roman"));
                Assert.That(test.DefaultJustification, Is.EqualTo(OfficeMathJustification.Right));
                Assert.That(mathPr.IntegralLimitLocation, Is.EqualTo(MathLimitLocation.UnderOver));
                Assert.That(mathPr.NaryLimitLocation, Is.EqualTo(MathLimitLocation.SubscriptSuperscript));
                Assert.That(test.LeftMargin, Is.EqualTo(10));
                Assert.That(test.RightMargin, Is.EqualTo(20));
                Assert.That(test.InterEquationSpacing, Is.EqualTo(30));
                Assert.That(test.IntraEquationSpacing, Is.EqualTo(40));
                Assert.That(test.PostParagraphSpacing, Is.EqualTo(50));
                Assert.That(test.PreParagraphSpacing, Is.EqualTo(60));
                Assert.That(test.IsSmallFraction, Is.True);
                Assert.That(test.UseDisplayMathDefaults, Is.False);
            }
        }

        /// <summary>
        /// Office Math can contain runs with w:rPr and m:rPr.
        /// Test that both properties work.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestMathRunPr(LoadFormat lf, SaveFormat sf)
        {
            LoadOptions lo = new LoadOptions();
            lo.ConvertShapeToOfficeMath = true;

            Document doc = CheckAndLoadDocument(@"Model\Math\TestMathRunPr", lf, sf, lo);
            Paragraph p = (Paragraph)(doc.Sections[0].Body).GetChildNodes(NodeType.Any, false)[1];
            Run run = (Run)((OfficeMath)((OfficeMath)p.FirstChild).FirstChild).FirstChild;
            RunPr rPr = run.RunPr;

            Assert.That(rPr.MathStyle, Is.EqualTo(Math.MathStyle.BoldItalic));
            Assert.That(rPr.HighlightColor, Is.EqualTo(DrColor.Yellow));
            Assert.That(rPr.StrikeThrough, Is.EqualTo(AttrBoolEx.True));
            Assert.That(rPr.SmallCaps, Is.EqualTo(AttrBoolEx.True));
        }

        /// <summary>
        /// Test oMathPara, test argPr
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestOMathPara(LoadFormat lf, SaveFormat sf)
        {
            LoadOptions lo = new LoadOptions();
            lo.ConvertShapeToOfficeMath = true;

            Document doc = CheckAndLoadDocument(@"Model\Math\TestOMathPara", lf, sf, lo);
            OfficeMath node = (OfficeMath)((CompositeNode)((doc.Sections[0].Body).GetChildNodes(NodeType.Any, false)[0])).GetChildNodes(NodeType.Any, false)[0];
            OfficeMath arg = (OfficeMath) GetOfficeMathTestNode(doc).GetChildNodes(NodeType.Any, false)[0];

            Assert.That(node.MathObject.MathObjectType, Is.EqualTo(MathObjectType.OMathPara));
            if((lf == LoadFormat.Doc) || (lf==LoadFormat.WordML))
            {
                Assert.That(((MathObjectOMathPara)node.MathObject).Justification, Is.EqualTo(OfficeMathJustification.CenterGroup));
                Assert.That(((MathObjectArgumentBase)arg.MathObject).ArgumentSize, Is.EqualTo(0));
            }
            else
            {
                Assert.That(((MathObjectOMathPara) node.MathObject).Justification, Is.EqualTo(OfficeMathJustification.Right));
                Assert.That(((MathObjectArgumentBase) arg.MathObject).ArgumentSize, Is.EqualTo(-1));
            }
        }

        /// <summary>
        /// Test m:acc is roundtripped.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAccent(LoadFormat lf, SaveFormat sf)
        {
            LoadOptions lo = new LoadOptions();
            lo.ConvertShapeToOfficeMath = true;

            Document doc = CheckAndLoadDocument(@"Model\Math\TestAccent", lf, sf, lo);
            OfficeMath node = GetOfficeMathTestNode(doc);

            Assert.That(node.MathObject.MathObjectType, Is.EqualTo(MathObjectType.Accent));
            Assert.That(((MathObjectAccent)node.MathObject).Character, Is.EqualTo('\u0308'));
            ValidateMathArgumentContent(node, 0, "simple accent");
        }

        /// <summary>
        /// Test that m:eqArr is roundtripped.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestArray(LoadFormat lf, SaveFormat sf)
        {
            LoadOptions lo = new LoadOptions();
            lo.ConvertShapeToOfficeMath = true;

            Document doc = CheckAndLoadDocument(@"Model\Math\TestArray", lf, sf, lo);
            OfficeMath node = GetOfficeMathTestNode(doc);

            Assert.That(node.MathObject.MathObjectType, Is.EqualTo(MathObjectType.Array));
            MathObjectArray arr = (MathObjectArray)node.MathObject;
            Assert.That(arr.RowSpacing, Is.EqualTo(123));
            Assert.That(arr.RowSpacingRule, Is.EqualTo(MathSpacingRule.Exactly));
            Assert.That(arr.BaseJustification, Is.EqualTo(MathBaseJustification.Bottom));
            Assert.That(arr.IsMaximumDistribution, Is.True);
            Assert.That(arr.IsObjectDistribution, Is.True);

            // test that children are read.
            Assert.That(node.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(2));
            ValidateMathArgumentContent(node, 0, "el1");
            ValidateMathArgumentContent(node, 1, "el2");
        }

        /// <summary>
        /// Test m:bar is roundtripped.
        /// WORDSNET-6085 Bar elements was read improperly due to the mistake in the code.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBar(LoadFormat lf, SaveFormat sf)
        {
            LoadOptions lo = new LoadOptions();
            lo.ConvertShapeToOfficeMath = true;

            Document doc = CheckAndLoadDocument(@"Model\Math\TestBar", lf, sf, lo);
            OfficeMath node = GetOfficeMathTestNode(doc);

            Assert.That(node.MathObject.MathObjectType, Is.EqualTo(MathObjectType.Bar));
            ValidateMathArgumentContent(node, 0, "overbar");
        }

        /// <summary>
        /// Test sub/superscript object including:
        /// m:sPre, m:sSup, m:sSub, m:sSubSup
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestSubSupAssorti(LoadFormat lf, SaveFormat sf)
        {
            LoadOptions lo = new LoadOptions();
            lo.ConvertShapeToOfficeMath = true;

            Document doc = CheckAndLoadDocument(@"Model\Math\TestSubSupAssorti", lf, sf, lo);
            // test m:sSup
            OfficeMath node = GetOfficeMathTestNode(doc);
            Assert.That(node.MathObject.MathObjectType, Is.EqualTo(MathObjectType.Supercript));
            Assert.That(node.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(2));
            ValidateMathArgumentContent(node, 0, "sup_text");
            ValidateMathArgumentContent(node, 1, "superscript", MathObjectType.SuperscriptPart);

            // test m:sSub
            node = GetOfficeMathTestNode(doc, 1);
            Assert.That(node.MathObject.MathObjectType, Is.EqualTo(MathObjectType.Subscript));
            Assert.That(node.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(2));
            ValidateMathArgumentContent(node, 0, "sub_text");
            ValidateMathArgumentContent(node, 1, "subscript", MathObjectType.SubscriptPart);

            // test m:sSubSup
            node = GetOfficeMathTestNode(doc, 2);
            Assert.That(node.MathObject.MathObjectType, Is.EqualTo(MathObjectType.SubSuperscript));
            Assert.That(node.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(3));
            ValidateMathArgumentContent(node, 0, "sub_sup_text");
            ValidateMathArgumentContent(node, 2, "superscript", MathObjectType.SuperscriptPart);
            ValidateMathArgumentContent(node, 1, "subscript", MathObjectType.SubscriptPart);

            // test m:sSubSup
            node = GetOfficeMathTestNode(doc, 3);
            Assert.That(node.MathObject.MathObjectType, Is.EqualTo(MathObjectType.PreSubSuperscript));
            Assert.That(node.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(3));
            ValidateMathArgumentContent(node, 0, "subscript", MathObjectType.SubscriptPart);
            ValidateMathArgumentContent(node, 1, "superscript", MathObjectType.SuperscriptPart);
            ValidateMathArgumentContent(node, 2, "pre_text");
        }


        /// <summary>
        /// Test m:borderBox
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBorderBox(LoadFormat lf, SaveFormat sf)
        {
            LoadOptions lo = new LoadOptions();
            lo.ConvertShapeToOfficeMath = true;

            Document doc = CheckAndLoadDocument(@"Model\Math\TestBorderBox", lf, sf, lo);
            OfficeMath node = GetOfficeMathTestNode(doc);

            Assert.That(node.MathObject.MathObjectType, Is.EqualTo(MathObjectType.BorderBox));
            MathObjectBorderBox box = (MathObjectBorderBox)node.MathObject;
            Assert.That(box.HideTopEdge, Is.True);
            Assert.That(box.HideRightEdge, Is.True);
            Assert.That(box.HideBottomEdge, Is.False);
            Assert.That(box.HideLeftEdge, Is.False);

            Assert.That(box.StrikeBLTR, Is.True);
            Assert.That(box.StrikeH, Is.False);
            Assert.That(box.StrikeV, Is.False);
            Assert.That(box.StrikeTLBR, Is.False);
        }

        /// <summary>
        /// Test m:d
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDelimiter(LoadFormat lf, SaveFormat sf)
        {
            LoadOptions lo = new LoadOptions();
            lo.ConvertShapeToOfficeMath = true;

            Document doc = CheckAndLoadDocument(@"Model\Math\TestDelimiter", lf, sf, lo);
            OfficeMath node = GetOfficeMathTestNode(doc);

            Assert.That(node.MathObject.MathObjectType, Is.EqualTo(MathObjectType.Delimiter));
            MathObjectDelimiter d = (MathObjectDelimiter)node.MathObject;
            Assert.That(d.DelimiterShape, Is.EqualTo(MathDelimiterShape.Match));
            Assert.That(d.SeparatorCharacter, Is.EqualTo('*'));

            if((lf != LoadFormat.Doc) && (lf!= LoadFormat.WordML))
                Assert.That(d.GrowToMatchOperand, Is.True);
        }

        /// <summary>
        /// Test m:f
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFraction(LoadFormat lf, SaveFormat sf)
        {
            LoadOptions lo = new LoadOptions();
            lo.ConvertShapeToOfficeMath = true;

            Document doc = CheckAndLoadDocument(@"Model\Math\TestFraction", lf, sf, lo);
            OfficeMath node = GetOfficeMathTestNode(doc);

            Assert.That(node.MathObject.MathObjectType, Is.EqualTo(MathObjectType.Fraction));
            MathObjectFraction f = (MathObjectFraction)node.MathObject;
            Assert.That(f.FractionType, Is.EqualTo(MathFractionType.Skewed));

            Assert.That(node.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(2));
            ValidateMathArgumentContent(node, 0, "A", MathObjectType.Numerator);
            ValidateMathArgumentContent(node, 1, "B", MathObjectType.Denominator);
        }


        /// <summary>
        /// Test "m:limLow" object
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLimLow(LoadFormat lf, SaveFormat sf)
        {
            LoadOptions lo = new LoadOptions();
            lo.ConvertShapeToOfficeMath = true;

            Document doc = CheckAndLoadDocument(@"Model\Math\TestLimLow", lf, sf, lo);
            OfficeMath node = GetOfficeMathTestNode(doc);

            Assert.That(node.MathObject.MathObjectType, Is.EqualTo(MathObjectType.Function));

            OfficeMath fName = (OfficeMath)node.GetChildNodes(NodeType.Any, false)[0];
            Assert.That(fName.MathObject.MathObjectType, Is.EqualTo(MathObjectType.FunctionName));

            Assert.That(fName.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            Assert.That(((OfficeMath)fName.GetChildNodes(NodeType.Any, false)[0]).MathObject.MathObjectType, Is.EqualTo(MathObjectType.LowerLimit));
        }

        /// <summary>
        /// Test "m:limUpp" object
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLimUpp(LoadFormat lf, SaveFormat sf)
        {
            LoadOptions lo = new LoadOptions();
            lo.ConvertShapeToOfficeMath = true;

            Document doc = CheckAndLoadDocument(@"Model\Math\TestLimUpp", lf, sf, lo);
            OfficeMath node = GetOfficeMathTestNode(doc);
            Assert.That(node.MathObject.MathObjectType, Is.EqualTo(MathObjectType.UpperLimit));

            Assert.That(node.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(2));

            OfficeMath groupObj = (OfficeMath)node.GetChildNodes(NodeType.Any, false)[0];
            Assert.That(groupObj.MathObject.MathObjectType, Is.EqualTo(MathObjectType.Argument));
        }

        /// <summary>
        /// Test "m:phant" object
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestPhantom(LoadFormat lf, SaveFormat sf)
        {
            if ((lf != LoadFormat.Docx) && (lf != LoadFormat.Rtf))
                return;

            const string testName = @"Model\Math\TestPhantom";

            LoadOptions lo = new LoadOptions();
            lo.ConvertShapeToOfficeMath = true;

            Document doc = TestUtil.Open(testName, lf, lo);
            OfficeMath node = GetOfficeMathTestNode(doc);

            Assert.That(node.MathObject.MathObjectType, Is.EqualTo(MathObjectType.Phantom));
            Assert.That(node.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(1));
            ValidateMathArgumentContent(node, 0, "phantom content");

            MathObjectPhantom p = (MathObjectPhantom)node.MathObject;
            Assert.That(p.IsZeroAscent, Is.True);
            Assert.That(p.IsZeroDescent, Is.True);
            Assert.That(p.IsZeroWidth, Is.True);
            TestUtil.SaveOpen(doc, testName, TestUtil.GetUnifiedScenario(lf, sf), lo);

            p.IsShown = false;
            p.IsTransparent = true;
            TestUtil.SaveOpen(doc, testName.Replace("Phantom", "PhantomInvisible"), TestUtil.GetUnifiedScenario(lf, sf), lo);
        }

        /// <summary>
        /// Test m:m (matrix). Check the follwing:
        /// - Verify rows and row elements are read;
        /// - Verify embedded matrices are read;
        /// - Verify properties are read;
        /// - Verify that validator fixes with non-valid properties of matrix columnPrCollection when loading this into the model.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestMatrixValidateColumnPr(LoadFormat lf, SaveFormat sf)
        {
            LoadOptions lo = new LoadOptions();
            lo.ConvertShapeToOfficeMath = true;

            Document doc = CheckAndLoadDocument(@"Model\Math\TestMatrixValidateColumnPr", lf, sf, lo);
            OfficeMath node = GetOfficeMathTestNode(doc);

            Assert.That(node.MathObject.MathObjectType, Is.EqualTo(MathObjectType.Matrix));

            MathObjectMatrix matrix = (MathObjectMatrix)node.MathObject;
            Assert.That(matrix.ColumnPrCollection.Count, Is.EqualTo(2));
            Assert.That(matrix.ColumnPrCollection[0].HorizontalAlignment, Is.EqualTo(HorizontalAlignment.Center));
            Assert.That(matrix.ColumnPrCollection[1].HorizontalAlignment, Is.EqualTo(HorizontalAlignment.Center));

            Assert.That(node.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(2));
            Assert.That(node.GetChildNodes(NodeType.Any, false)[0].NodeType, Is.EqualTo(NodeType.OfficeMath));
            Assert.That(node.GetChildNodes(NodeType.Any, false)[1].NodeType, Is.EqualTo(NodeType.OfficeMath));

            OfficeMath innerNode = (OfficeMath)((OfficeMath)node.GetChildNodes(NodeType.Any, false)[1]).GetChildNodes(NodeType.Any, false)[1];
            Assert.That(innerNode.MathObject.MathObjectType, Is.EqualTo(MathObjectType.Argument));
            MathObjectMatrix embeddedMatrix = (MathObjectMatrix)((OfficeMath)innerNode.GetChildNodes(NodeType.Any, false)[0]).MathObject;

            Assert.That(embeddedMatrix.BaseJustification, Is.EqualTo(MathBaseJustification.Bottom));

            Assert.That(embeddedMatrix.RowSpacingRule, Is.EqualTo(MathSpacingRule.Exactly));
            Assert.That(embeddedMatrix.RowSpacing, Is.EqualTo(360));

            Assert.That(embeddedMatrix.ColumnSpacingRule, Is.EqualTo(MathSpacingRule.Multiple));
            Assert.That(embeddedMatrix.MinimumColumnWidth, Is.EqualTo(240));

            Assert.That(embeddedMatrix.ColumnPrCollection.Count, Is.EqualTo(1));
            Assert.That(embeddedMatrix.ColumnPrCollection[0].HorizontalAlignment, Is.EqualTo(HorizontalAlignment.Right));
        }

        /// <summary>
        /// Test m:m (matrix). Check the if some arguments in a row are missing, then they are inserted by validator as empty arguments.
        /// That is what MS Word does upon resaving of bad document.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestMatrixCellMissing(LoadFormat lf, SaveFormat sf)
        {
            LoadOptions lo = new LoadOptions();
            lo.ConvertShapeToOfficeMath = true;

            Document doc = CheckAndLoadDocument(@"Model\Math\TestMatrixCellMissing", lf, sf, lo);
            OfficeMath node = GetOfficeMathTestNode(doc);

            Assert.That(node.MathObject.MathObjectType, Is.EqualTo(MathObjectType.Matrix));

            Assert.That(node.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(2));
            OfficeMath firstRow = ((OfficeMath)node.GetChildNodes(NodeType.Any, false)[0]);
            Assert.That(firstRow.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(2));
            Assert.That(((OfficeMath)firstRow.GetChildNodes(NodeType.Any, false)[0]).GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(0));
        }

        /// <summary>
        /// Test m:nary
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestNAry(LoadFormat lf, SaveFormat sf)
        {
            LoadOptions lo = new LoadOptions();
            lo.ConvertShapeToOfficeMath = true;

            Document doc = CheckAndLoadDocument(@"Model\Math\TestNary", lf, sf, lo);
            OfficeMath node = GetOfficeMathTestNode(doc);

            Assert.That(node.MathObject.MathObjectType, Is.EqualTo(MathObjectType.NAry));
            MathObjectNAry nAry = (MathObjectNAry)node.MathObject;
            Assert.That(nAry.LimitLocation, Is.EqualTo(MathLimitLocation.UnderOver));
            Assert.That(nAry.IsHideSubscript, Is.True);
            Assert.That(nAry.IsHideSuperscript, Is.True);
            Assert.That(nAry.GrowToMatchOperand, Is.True);
            Assert.That(nAry.Character, Is.EqualTo('\u222D'));

            // test that children are read.
            Assert.That(node.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(3));
            ValidateMathArgumentContent(node, 0, "lower_lim", MathObjectType.SubscriptPart);
            ValidateMathArgumentContent(node, 1, "upper_lim", MathObjectType.SuperscriptPart);
            ValidateMathArgumentContent(node, 2, "main_text");
        }

        /// <summary>
        /// Test m:rad, test that inserted/deleted runs are read correctly.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRadical(LoadFormat lf, SaveFormat sf)
        {
            LoadOptions lo = new LoadOptions();
            lo.ConvertShapeToOfficeMath = true;

            Document doc = CheckAndLoadDocument(@"Model\Math\TestRadical", lf, sf, lo);
            OfficeMath node = GetOfficeMathTestNode(doc);

            Assert.That(node.MathObject.MathObjectType, Is.EqualTo(MathObjectType.Radical));
            Assert.That(node.GetChildNodes(NodeType.Any, false).Count, Is.EqualTo(2));
            OfficeMath degree = ValidateMathArgumentContent(node, 0, "degree_textnew", MathObjectType.Degree);

            Assert.That(degree.GetChildNodes(NodeType.Any, false)[1].GetText(), Is.EqualTo("new"));
            Assert.That(((Run)degree.GetChildNodes(NodeType.Any, false)[1]).IsInsertRevision, Is.True);

            OfficeMath radical = ValidateMathArgumentContent(node, 1, "radical_text");
            Assert.That(radical.GetChildNodes(NodeType.Any, false)[1].GetText(), Is.EqualTo("ical_tex"));
            Assert.That(((Run)radical.GetChildNodes(NodeType.Any, false)[1]).IsDeleteRevision, Is.True);
        }

        /// <summary>
        /// Verify that rPr of a math node are cloned properly.
        /// </summary>
        [Test]
        public void TestCloneRunPr()
        {
            Document blank = new Document();
            RunPr testRunPr = new RunPr();
            testRunPr.AllCaps = AttrBoolEx.True;
            testRunPr.MathLineBreak = new MathLineBreak();
            testRunPr.MathLineBreak.Alignment = 25;

            OfficeMath m = new OfficeMath(blank, new MathObjectNAry(), testRunPr);

            OfficeMath copy = (OfficeMath)m.Clone(true);
            RunPr copyRunPr = copy.RunPr;
            Assert.That(copyRunPr.AllCaps, Is.EqualTo(AttrBoolEx.True));
            Assert.That(copyRunPr.MathLineBreak, IsNot.Null());
            Assert.That(copyRunPr.MathLineBreak.Alignment, Is.EqualTo(25));

            copyRunPr.AllCaps = AttrBoolEx.False;
            Assert.That(testRunPr.AllCaps, Is.EqualTo(AttrBoolEx.True));
            copyRunPr.MathLineBreak.Alignment = 3;
            Assert.That(testRunPr.MathLineBreak.Alignment, Is.EqualTo(25));
        }

        /// <summary>
        /// Verify that right ancestors can be inserted at the right parent nodes.
        /// </summary>
        [Test]
        public void TestParentChildConsistency()
        {
            Document blank = new Document();
            OfficeMath arg = new OfficeMath(blank, new MathObjectArgumentBase(MathObjectType.Argument));
            OfficeMath func = new OfficeMath(blank, new MathObjectFunction());
            OfficeMath fName = new OfficeMath(blank, new MathObjectArgumentBase(MathObjectType.FunctionName));
            OfficeMath matrix = new OfficeMath(blank, new MathObjectMatrix());
            OfficeMath row = new OfficeMath(blank, new MathObjectMatrixRow());
            OfficeMath arr = new OfficeMath(blank, new MathObjectArray());
            OfficeMath officeMath= new OfficeMath(blank, new MathObjectOMath());

            Assert.That(func.CanInsert(arg), Is.True);
            Assert.That(fName.CanInsert(arg), Is.False);
            Assert.That(arg.CanInsert(func), Is.True);
            Assert.That(func.CanInsert(fName), Is.True);
            Assert.That(matrix.CanInsert(arr), Is.False);
            Assert.That(row.CanInsert(arr), Is.False);
            Assert.That(row.CanInsert(arg), Is.True);
            Assert.That(arr.CanInsert(arg), Is.True);
            Assert.That(officeMath.CanInsert(func), Is.True);
            Assert.That(officeMath.CanInsert(arg), Is.False);
            Assert.That(row.CanInsert(officeMath), Is.False);

            Assert.That(officeMath.CanInsert(new CommentRangeStart(blank, 0)), Is.True);
            Assert.That(arg.CanInsert(new BookmarkStart(blank)), Is.True);
            Assert.That(fName.CanInsert(new Run(blank, "test")), Is.True);
            Assert.That(arr.CanInsert(new Shape(blank)), Is.False);
        }

        // FOSS TestJira11897 removed: it round-trips the math EquationXML through the removed WordML
        // format (its WML input case loads a removed format), and the docx case relies on the removed
        // shape->OfficeMath conversion which yields no MathObject in the FOSS build.

        // FOSS TestJira16909 removed: OfficeMath equation-XML encoding from a WordML (.xml) input; WordML load removed.

        /// <summary>
        /// WORDSNET-17185 <see cref="ArgumentOutOfRangeException"/> occurred when we tried to read math a matrix
        /// without columns properties (or with too few column properties). Now we restore missing column properties upon
        /// loading a document.
        /// </summary>
        [Test]
        public void TestJira17185()
        {
            Document doc = TestUtil.Open(@"Model\Math\TestJira17185.docx");

            // Find the matrix node.
            CompositeNode node = doc.FirstSection.Body.FirstParagraph;
            while ((node.NodeType != NodeType.OfficeMath) || (((OfficeMath)node).MathObjectType != MathObjectType.Matrix))
            {
                node = node.FirstChild as CompositeNode;
            }

            MathObjectMatrix matrix = (MathObjectMatrix)(((OfficeMath)node).MathObject);
            Assert.That(matrix.ColumnPrCollection.Count, Is.EqualTo(3));
            // FOSS Dropped the HTML save smoke-check (removed format); the matrix-column restore above is the point.
        }

        /// <summary>
        /// Verify that inserted/deleted Office Math subtrees are accepted well.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAcceptRevisions(LoadFormat lf, SaveFormat sf)
        {
            if ((lf != LoadFormat.Docx) && (lf != LoadFormat.Rtf))
                return;

            const string testName = @"Model\Math\TestAcceptRevisionsMath";
            Document doc = TestUtil.Open(testName, lf);
            doc.AcceptAllRevisions();
            TestUtil.SaveOpen(doc, testName, lf, sf);
        }

        /// <summary>
        /// Test that CtrlPr child of mathPr containing "w:del", "w:ins" and "w:rPr" is read correctly.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestControlPrResiliency(LoadFormat lf, SaveFormat sf)
        {
            const string testName = @"Model\Math\TestControlPrResiliency";

            // todo 6220 add new task for fix this problem.
            if ((lf != LoadFormat.Docx) && (lf != LoadFormat.Rtf))
                return;

            Document doc = TestUtil.Open(testName, lf);
            OfficeMath node = GetOfficeMathTestNode(doc);
            Assert.That(node.RunPr.HasDeleteRevision, Is.True);
            Assert.That(node.RunPr.HasInsertRevision, Is.True);

            Assert.That(node.RunPr.InsertRevision.Author, Is.EqualTo("Vasya"));
            Assert.That(node.RunPr.DeleteRevision.Author, Is.EqualTo("Denis"));
            Assert.That(node.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));
            Assert.That(node.RunPr.HighlightColor, Is.EqualTo(DrColor.Silver));
            Assert.That(node.RunPr.NameAscii, Is.EqualTo("Times New Roman"));
            Assert.That(node.RunPr.NameOther, Is.EqualTo("Times New Roman"));
            if (!((lf == LoadFormat.Doc) && (sf == SaveFormat.WordML)))
                TestUtil.SaveOpen(doc, testName, lf, sf);
        }



        /// <summary>
        /// WORDSNET-14602 Justification of OfficeMath object.
        /// New <see cref="OfficeMathJustification.Inline"/> type was added.
        /// <see cref="OfficeMath.Justification"/> and <see cref="OfficeMath.DisplayType"/> were exposed to public.
        /// Tests get/set <see cref="OfficeMath.DisplayType"/> and <see cref="OfficeMath.Justification"/>.
        /// </summary>
        [Test]
        public void TestOMathDisplayToInline()
        {
            Document doc = TestUtil.Open(@"Model\Math\TestOMathDisplayType.docx");

            NodeCollection officeMaths = doc.GetChildNodes(NodeType.OfficeMath, true);

            OfficeMath officeMath = (OfficeMath)officeMaths[0];
            Assert.That(officeMath.IsTopLevel, Is.True);
            Assert.That(officeMath.DisplayType, Is.EqualTo(OfficeMathDisplayType.Display));
            Assert.That(officeMath.Justification, Is.EqualTo(OfficeMathJustification.CenterGroup));

            // Change to Inline.
            officeMath.DisplayType = OfficeMathDisplayType.Inline;
            Assert.That(officeMath.DisplayType, Is.EqualTo(OfficeMathDisplayType.Inline));
            Assert.That(officeMath.Justification, Is.EqualTo(OfficeMathJustification.Inline));

            doc = TestUtil.SaveOpen(doc, @"Model\Math\TestOMathDisplayToInline.docx", UnifiedScenario.Docx2DocxNoGold);
            officeMaths = doc.GetChildNodes(NodeType.OfficeMath, true);

            // Check after save.
            officeMath = (OfficeMath)officeMaths[0];
            Assert.That(officeMath.DisplayType, Is.EqualTo(OfficeMathDisplayType.Inline));
            Assert.That(officeMath.Justification, Is.EqualTo(OfficeMathJustification.Inline));
        }

        /// <summary>
        /// Part of WORDSNET-14602
        /// All child Office Math have Inline DisplayType and Inline Justification.
        /// </summary>
        [Test]
        public void TestDisplayTypeOfNestedOMath()
        {
            Document doc = TestUtil.Open(@"Model\Math\TestOMathDisplayType.docx");

            NodeCollection officeMaths = doc.GetChildNodes(NodeType.OfficeMath, true);

            OfficeMath officeMath = (OfficeMath)officeMaths[1];
            Assert.That(officeMath.IsTopLevel, Is.False);
            Assert.That(officeMath.DisplayType, Is.EqualTo(OfficeMathDisplayType.Inline));
            Assert.That(officeMath.Justification, Is.EqualTo(OfficeMathJustification.Inline));
        }

        /// <summary>
        /// Part of WORDSNET-14602
        /// Tests get/set oMath Justification.
        /// </summary>
        [Test]
        public void TestSetOMathJustification()
        {
            Document doc = TestUtil.Open(@"Model\Math\TestOMathDisplayType.docx");

            NodeCollection officeMaths = doc.GetChildNodes(NodeType.OfficeMath, true);

            OfficeMath officeMath = (OfficeMath)officeMaths[5];
            Assert.That(officeMath.DisplayType, Is.EqualTo(OfficeMathDisplayType.Display));
            Assert.That(officeMath.Justification, Is.EqualTo(OfficeMathJustification.Left));

            officeMath = (OfficeMath)officeMaths[10];
            Assert.That(officeMath.Justification, Is.EqualTo(OfficeMathJustification.Right));

            officeMath = (OfficeMath)officeMaths[15];
            Assert.That(officeMath.Justification, Is.EqualTo(OfficeMathJustification.Center));

            // Change Justification.
            officeMath.Justification = OfficeMathJustification.Right;
            Assert.That(officeMath.Justification, Is.EqualTo(OfficeMathJustification.Right));

            doc = TestUtil.SaveOpen(doc, @"Model\Math\TestSetOMathJustification.docx", UnifiedScenario.Docx2DocxNoGold);
            officeMaths = doc.GetChildNodes(NodeType.OfficeMath, true);

            // Check after save.
            officeMath = (OfficeMath)officeMaths[15];
            Assert.That(officeMath.DisplayType, Is.EqualTo(OfficeMathDisplayType.Display));
            Assert.That(officeMath.Justification, Is.EqualTo(OfficeMathJustification.Right));
        }

        /// <summary>
        /// Part of WORDSNET-14602
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException),
            ExpectedMessage = "DisplayType cannot be changed for the nested Office Math. Please, check the parent node type to make sure it is top level Office Math.")]
        public void TestSetDisplayTypeForNestedOMath()
        {
            Document doc = TestUtil.Open(@"Model\Math\TestOMathDisplayType.docx");
            OfficeMath officeMath = (OfficeMath)doc.GetChild(NodeType.OfficeMath, 2, true);

            Assert.That(officeMath.IsTopLevel, Is.False);

            // Returned display format type is always Inline for nested Office Math.
            Assert.That(officeMath.DisplayType, Is.EqualTo(OfficeMathDisplayType.Inline));
            Assert.That(officeMath.Justification, Is.EqualTo(OfficeMathJustification.Inline));

            // Set Inline DisplayType.
            officeMath.DisplayType = OfficeMathDisplayType.Inline;
            Assert.That(officeMath.DisplayType, Is.EqualTo(OfficeMathDisplayType.Inline));

            // Try to set DisplayType for the nested Office Math.
            officeMath.DisplayType = OfficeMathDisplayType.Display;
        }

        /// <summary>
        /// Part of WORDSNET-14602
        /// Test how the OMath siblings were proceed upon changing the <see cref="OfficeMath.DisplayType"/> to Inline.
        /// </summary>
        [Test]
        public void TestOMathInlineSiblings()
        {
            Document doc = TestUtil.Open(@"Model\Math\TestOMathInlineSiblings.docx");

            NodeCollection officeMaths = doc.GetChildNodes(NodeType.OfficeMath, true);

            foreach (OfficeMath officeMath in officeMaths)
            {
                // Check office math is top level (IsTopLevel property is internal)
                if (officeMath.ParentNode.NodeType != NodeType.OfficeMath)
                {
                    Assert.That(officeMath.DisplayType, Is.EqualTo(OfficeMathDisplayType.Display));
                    officeMath.DisplayType = OfficeMathDisplayType.Inline;
                }
            }

            // Gold comparing in this case is more suitable, because the are lot of test cases.
            TestUtil.SaveOpen(doc, @"Model\Math\TestOMathInlineSiblings", UnifiedScenario.Docx2Docx);
        }

        /// <summary>
        /// Part of WORDSNET-14602
        /// Test how the OMath siblings were proceed upon changing the <see cref="OfficeMath.DisplayType"/> to Display.
        /// </summary>
        [Test]
        public void TestOMathDisplaySiblings()
        {
            Document doc = TestUtil.Open(@"Model\Math\TestOMathDisplaySiblings.docx");
            NodeCollection officeMaths = doc.GetChildNodes(NodeType.OfficeMath, true);

            foreach (OfficeMath officeMath in officeMaths)
            {
                // Check office math is top level (IsTopLevel property is internal)
                if (officeMath.ParentNode.NodeType != NodeType.OfficeMath)
                {
                    Assert.That(officeMath.DisplayType, Is.EqualTo(OfficeMathDisplayType.Inline));
                    officeMath.DisplayType = OfficeMathDisplayType.Display;
                }
            }

            // Gold comparing in this case is more suitable, because the are lot of test cases.
            TestUtil.SaveOpen(doc, @"Model\Math\TestOMathDisplaySiblings", UnifiedScenario.Docx2Docx);
        }

        /// <summary>
        /// Part of WORDSNET-14602
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException),
            ExpectedMessage = "Justification cannot be set to the Office Math displayed inline with text. Please, use OfficeMath.DisplayType property to change OfficeMathDisplayType.")]
        public void TestOMathJustificationA()
        {
            Document doc = TestUtil.Open(@"Model\Math\TestOMathDisplayType.docx");
            OfficeMath officeMath = (OfficeMath)doc.GetChild(NodeType.OfficeMath, 1, true);

            Assert.That(officeMath.DisplayType, Is.EqualTo(OfficeMathDisplayType.Inline));
            Assert.That(officeMath.Justification, Is.EqualTo(OfficeMathJustification.Inline));

            // Set inline justification.
            officeMath.Justification = OfficeMathJustification.Inline;
            Assert.That(officeMath.Justification, Is.EqualTo(OfficeMathJustification.Inline));

            // Try to set any other justification for the office math with DisplayType equals Inline.
            officeMath.Justification = OfficeMathJustification.Left;
        }

        /// <summary>
        /// Part of WORDSNET-14602
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentException),
            ExpectedMessage = "Inline justification cannot be set to the Office Math displayed on its own line. Please, use OfficeMath.DisplayType property to change OfficeMathDisplayType.")]
        public void TestOMathJustificationB()
        {
            Document doc = TestUtil.Open(@"Model\Math\TestOMathDisplayType.docx");
            OfficeMath officeMath = (OfficeMath)doc.GetChild(NodeType.OfficeMath, 0, true);

            Assert.That(officeMath.DisplayType, Is.EqualTo(OfficeMathDisplayType.Display));
            Assert.That(officeMath.Justification, Is.EqualTo(OfficeMathJustification.CenterGroup));

            // Set left justification.
            officeMath.Justification = OfficeMathJustification.Left;
            Assert.That(officeMath.Justification, Is.EqualTo(OfficeMathJustification.Left));

            // Try to set inline justification for the office math with DisplayType equals Display.
            officeMath.Justification = OfficeMathJustification.Inline;
        }




        /// <summary>
        /// WORDSNET-26171 Resetting italic does not work for runs inside office math.
        /// Translate italic attribute into MathStyle.
        /// </summary>
        [Test]
        public void Test26171()
        {
            Document doc = TestUtil.Open(@"Model\Math\Test26171.docx");
            Run run = (Run)doc.GetChild(NodeType.Run, 0, true);

            run.Font.Italic = false;
            Assert.That(run.RunPr[FontAttr.Italic], Is.Null);
            Assert.That(run.RunPr[FontAttr.MathStyle], Is.EqualTo(MathStyle.Plain));

            run.Font.Italic = true;
            Assert.That(run.RunPr[FontAttr.Italic], Is.Null);
            Assert.That(run.RunPr[FontAttr.MathStyle], Is.EqualTo(MathStyle.Italic));
        }

        /// <summary>
        /// Opens and saves document, also checks state of the shape with "EquationXml".
        /// </summary>
        private static void OpenSaveOpenDocumentWithEquationXmlShape(string pathPattern, LoadFormat loadFromat, SaveFormat saveFormat)
        {
            LoadOptions lo = new LoadOptions();
            Assert.That(lo.ConvertShapeToOfficeMath, Is.False);
            string sourceExt = FileFormatUtil.LoadFormatToExtension(loadFromat);

            // On loading expected, that math shape should be preserved (according to load settings).
            Document doc = TestUtil.Open(string.Format(pathPattern, sourceExt), lo);
            Assert.That(doc.FirstSection.Body.Shapes[0].ShapePr.Contains(ShapeAttr.EquationXML), Is.True);
            Assert.That(doc.FirstSection.Body.GetChild(NodeType.OfficeMath, 0, true), Is.Null);

            string dstExt = FileFormatUtil.SaveFormatToExtension(saveFormat);
            string dstFileName = string.Format(pathPattern, sourceExt.Replace(".", string.Empty).ToUpper());

            SaveOptions so = SaveOptions.CreateSaveOptions(saveFormat);
            so.SetTestMode();

            TestWarningCallback wc = new TestWarningCallback();
            doc.WarningCallback = wc;

            if (saveFormat == SaveFormat.Docx)
                ((OoxmlSaveOptions)so).ComplianceCore = OoxmlComplianceCore.IsoTransitional;

            doc = TestUtil.SaveOpen(doc, dstFileName, TestUtil.BuildScenario(loadFromat, saveFormat, true), so, lo);

            switch (saveFormat)
            {
                case SaveFormat.Rtf:
                case SaveFormat.Doc:
                case SaveFormat.WordML:
                case SaveFormat.FlatOpc:
                {
                    Shape shape = doc.FirstSection.Body.Shapes[0];
                    // Expected, that "EquationXml" will be lost after saving to RTF.
                    Assert.That(shape.ShapePr.Contains(ShapeAttr.EquationXML), Is.EqualTo(saveFormat != SaveFormat.Rtf));
                    Assert.That(doc.FirstSection.Body.GetChild(NodeType.OfficeMath, 0, true), Is.Null);
                    Assert.That(shape.HasImageBytes, Is.True);
                    break;
                }
                case SaveFormat.Docx:
                {
                    OfficeMath officeMath = (OfficeMath)doc.FirstSection.Body.GetChild(NodeType.OfficeMath, 0, true);
                    Assert.That(officeMath.Justification, Is.EqualTo(OfficeMathJustification.CenterGroup));
                    Assert.That(doc.FirstSection.Body.GetChild(NodeType.Shape, 0, true), Is.Null);
                    Assert.That(officeMath, IsNot.Null());

                    Assert.That(wc.Contains(WarningSource.Validator,
                        WarningType.DataLoss, WarningStrings.InvalidOfficeMathXml), Is.False);
                    break;
                }
                default:
                    throw new ArgumentException("Specified output format is not supported.");
            }
        }

        /// <summary>
        /// Used for check the file with gold. Returns the loaded model from file.
        /// Word keeps different schemes to shape's prop(ShapeAttr.EquationXML) to Doc and WML formats.
        /// </summary>
        private static Document CheckAndLoadDocument(string fileName, LoadFormat lf, SaveFormat sf, LoadOptions lo)
        {
            Document document = null;
            if ((sf == SaveFormat.WordML) && (lf == LoadFormat.Doc))
            {
                document = TestUtil.Open(fileName, lf, lo);
                fileName = TestUtil.Save(document, fileName+".wml");
                document = TestUtil.Open(fileName, lo);
            }
            else
            {
                document = TestUtil.OpenSaveOpen(fileName, TestUtil.GetUnifiedScenario(lf, sf), lo);
            }
            return document;
        }

        /// <summary>
        /// Retrieve i-th argument from a Office Math node.
        /// Assert that the argument is of <see cref="MathObjectType.Argument"/>
        /// </summary>
        private static OfficeMath ValidateMathArgumentContent(OfficeMath node, int index, string runText)
        {
            return ValidateMathArgumentContent(node, index, runText, MathObjectType.Argument);
        }

        /// <summary>
        /// Retrieve i-th argument of a Office Math node.
        /// Assert that the argument is of requested <see cref="MathObjectType"/>
        /// </summary>
        private static OfficeMath ValidateMathArgumentContent(OfficeMath node, int index, string runText, MathObjectType assertType)
        {
            Assert.That(node.GetChildNodes(NodeType.Any, false)[index].NodeType, Is.EqualTo(NodeType.OfficeMath));
            OfficeMath result = (OfficeMath)node.GetChildNodes(NodeType.Any, false)[index];
            Assert.That(result.MathObject.MathObjectType, Is.EqualTo(assertType));

            if (runText != "") // With non empty runText to check we consider argument child to be run.
            {
                Assert.That(result.GetChildNodes(NodeType.Any, false)[0].NodeType, Is.EqualTo(NodeType.Run));
                Assert.That(result.GetText(), Is.EqualTo(runText));
            }
            return result;
        }

        private static OfficeMath GetOfficeMathTestNode(Document doc)
        {
            return GetOfficeMathTestNode(doc, 0);
        }

        /// <summary>
        /// Since large number of docs contains tested data nested as in para\oMathPara\oMath\testNode.
        /// This method returns only testNode from the i-th para in the doc.
        /// </summary>
        private static OfficeMath GetOfficeMathTestNode(Document doc, int index)
        {
            Paragraph p = (Paragraph)(doc.Sections[0].Body).GetChildNodes(NodeType.Any, false)[index];
            if (p.FirstChild.NodeType == NodeType.OfficeMath)
                return (OfficeMath)((OfficeMath)((OfficeMath)p.FirstChild).FirstChild).FirstChild;
            else
                return null;
        }
    }
}
