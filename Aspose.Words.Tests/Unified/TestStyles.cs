// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/05/2005 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Aspose.Drawing;
using Aspose.Words.Drawing.Core.Dml.Themes;
using Aspose.Words.Loading;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using Aspose.Words.Tables;
using Aspose.Words.Validation;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified tests for styles that do not fall into any more specific category.
    /// </summary>
    [TestFixture]
    public class TestStyles : UnifiedTestsBase
    {


        /// <summary>
        /// category - Style
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBuiltin2000(LoadFormat lf, SaveFormat sf)
        {
            CheckBuiltin(TestUtil.OpenSaveOpen(@"Model\Style\TestBuiltin2000", lf, sf), 12, 16);
        }

        /// <summary>
        /// category - Style
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBuiltin2003(LoadFormat lf, SaveFormat sf)
        {
            CheckBuiltin(TestUtil.OpenSaveOpen(@"Model\Style\TestBuiltin2003", lf, sf), 12, 16);
        }

        /// <summary>
        /// category - Style
        /// </summary>
        [Test]
        public void TestBuiltin2007()
        {
            Document doc = TestUtil.Open(@"Model\Style\TestBuiltin2007.docx");
            foreach (Style style in doc.Styles)
                Debug.WriteLine(style.Name + ": " + style.StyleIdentifier);
        }

        /// <summary>
        /// Basic check of some font and paragraph properties of built in styles.
        /// </summary>
        private static void CheckBuiltin(Document doc, double normalFontSize, double h1FontSize)
        {
            StyleCollection styles = doc.Styles;

            Style normal = styles["Normal"];
            Assert.That(normal.Type, Is.EqualTo(StyleType.Paragraph));
            Assert.That(normal.Name, Is.EqualTo("Normal"));
            Assert.That(normal.BaseStyleName, Is.EqualTo(""));
            Assert.That(normal.NextParagraphStyleName, Is.EqualTo("Normal"));
            Assert.That(normal.Font.Name, Is.EqualTo("Times New Roman"));
            Assert.That(normal.Font.Size, Is.EqualTo(normalFontSize));
            Assert.That(normal.ParagraphFormat.KeepWithNext, Is.EqualTo(false));
            Assert.That(normal.BuiltIn, Is.EqualTo(true));

            Style h1 = styles["Heading 1"];
            Assert.That(h1.BaseStyleName, Is.EqualTo("Normal"));
            Assert.That(h1.NextParagraphStyleName, Is.EqualTo("Normal"));
            Assert.That(h1.Font.Name, Is.EqualTo("Arial"));
            Assert.That(h1.Font.Size, Is.EqualTo(h1FontSize));
            Assert.That(h1.ParagraphFormat.KeepWithNext, Is.EqualTo(true));

            Style hyperlink = styles["Hyperlink"];
            Assert.That(hyperlink.Type, Is.EqualTo(StyleType.Character));
            Assert.That(hyperlink.Font.Color.ToArgb(), Is.EqualTo(Color.Blue.ToArgb()));
        }

        /// <summary>
        /// Test how styles with aliases are loaded and saved.
        ///
        /// category - Style
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestAlias(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Style\TestAlias", lf, sf);

            StyleCollection styles = doc.Styles;

            Style normal = styles["Normal"];
            Assert.That(normal.Name, Is.EqualTo("Normal"));
            Assert.That(normal, IsNot.Null());

            Style normalAlias = styles["NormalA"];
            Assert.That(normalAlias, IsNot.Null());

            Assert.That(normalAlias, Is.EqualTo(normal));
        }

        /// <summary>
        /// category - Style
        /// </summary>
        [Test]
        public void TestSetNextStyle()
        {
            Document doc = new Document();

            // Requesting a built in style that does not yet exist,
            // created it automatically, but that's tested elsewhere.

            Style style = doc.Styles["Heading 1"];
            Assert.That(style.NextParagraphStyleName, Is.EqualTo("Normal"));    //Check before we change
            style.NextParagraphStyleName = "Heading 2";
            Assert.That(style.NextParagraphStyleName, Is.EqualTo("Heading 2"));    //Check after we change
            Assert.That(style.NextIstd, Is.EqualTo(StyleIndex.Heading2));
        }

        /// <summary>
        /// category - Style
        /// </summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Cannot find style 'XXX'.")]
        public void TestSetNextStyleNotFound()
        {
            Document doc = new Document();
            Style style = doc.Styles["Heading 1"];
            style.NextParagraphStyleName = "XXX";
        }

        /// <summary>
        /// category - Style
        /// </summary>
        [Test]
        public void TestSetNextStyleEmpty()
        {
            try
            {
                Document doc = new Document();
                Style style = doc.Styles["Heading 1"];
                style.NextParagraphStyleName = "";
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.That(e.Message.IndexOf("The argument cannot be null or empty string.") >= 0, Is.True);
            }
        }

        /// <summary>
        /// category - Style
        /// </summary>
        [Test]
        public void TestSetNextStyleNull()
        {
            try
            {
                Document doc = new Document();
                Style style = doc.Styles["Heading 1"];
                style.NextParagraphStyleName = null;
                Assert.Fail();
            }
            catch (ArgumentException e)
            {
                Assert.That(e.Message.IndexOf("The argument cannot be null or empty string.") >= 0, Is.True);
            }
        }

        /// <summary>
        /// Check setting base style succeeds.
        ///
        /// category - Style
        /// </summary>
        [Test]
        public void TestSetBaseStyle()
        {
            Document doc = new Document();
            StyleCollection styles = doc.Styles;
            Style h1 = styles["Heading 1"];
            h1.BaseStyleName = "";
            Assert.That(h1.BaseStyleName, Is.EqualTo(""));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "The built-in styles Normal and Default Paragraph Font cannot be based on any style.")]
        public void TestSetBaseStyleOfNormalStyle()
        {
            Document doc = new Document();
            StyleCollection styles = doc.Styles;
            Style normal = styles["Normal"];
            normal.BaseStyleName = "";
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Cannot find style 'AAA'.")]
        public void TestSetBaseStyleNotFound()
        {
            Document doc = new Document();
            StyleCollection styles = doc.Styles;
            Style h1 = styles["Heading 1"];
            h1.BaseStyleName = "AAA";
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "This operation will create a circular reference between styles.")]
        public void TestSetBaseStyleCircularSelf()
        {
            Document doc = new Document();
            StyleCollection styles = doc.Styles;
            styles["Heading 1"].BaseStyleName = "Heading 1";
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "This operation will create a circular reference between styles.")]
        public void TestSetBaseStyleCircularSelfCustom()
        {
            Document doc = new Document();
            Style style = doc.Styles.Add (StyleType.Paragraph, "TOC 1");
            style.BaseStyleName = "TOC 1";
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "This operation will create a circular reference between styles.")]
        public void TestSetBaseStyleCircularSimple()
        {
            Document doc = new Document();
            StyleCollection styles = doc.Styles;
            styles["Heading 1"].BaseStyleName = "Heading 2";
            styles["Heading 2"].BaseStyleName = "Heading 1";
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "This operation will create a circular reference between styles.")]
        public void TestSetBaseStyleCircularComplex()
        {
            Document doc = new Document();
            StyleCollection styles = doc.Styles;
            styles["Heading 2"].BaseStyleName = "Heading 1";
            styles["Heading 3"].BaseStyleName = "Heading 2";
            styles["Heading 1"].BaseStyleName = "Heading 3";
        }

        /// <summary>
        /// Check that a simple value inheritance works in styles for Font and ParagraphFormat.
        ///
        /// category - Style
        /// </summary>
        [Test]
        public void TestSimpleInheritance()
        {
            Document doc = new Document();
            StyleCollection styles = doc.Styles;

            Style aaa = styles.Add(StyleType.Paragraph, "AAA");
            Assert.That(aaa.BaseStyleName, Is.EqualTo(""));

            //Check default attribute values are visible.
            Assert.That(aaa.Font.Size, Is.EqualTo(10d));
            Assert.That(aaa.ParagraphFormat.SpaceBefore, Is.EqualTo(0d));

            //Base the style on another style and check the value is visible.
            aaa.BaseStyleName = "Heading 1";
            // FOSS RK I don't know why the value reported now is 14, whereas it was 16.
            // It obviously has to do with the migration from Blank.doc to Blank.docx, but check back later.
            // Space before went from 12 to 24 as part of that too.
            Assert.That(aaa.Font.Size, Is.EqualTo(14d));
            Assert.That(aaa.ParagraphFormat.SpaceBefore, Is.EqualTo(24d));

            //Change the attr value in the base style and check it propagates to the derived style.
            styles["Heading 1"].Font.Size = 8;
            Assert.That(aaa.Font.Size, Is.EqualTo(8d));
            styles["Heading 1"].ParagraphFormat.SpaceBefore = 11;
            Assert.That(aaa.ParagraphFormat.SpaceBefore, Is.EqualTo(11d));

            //Change the attr value in the derived style and check it does not affect the parent style.
            aaa.Font.Size = 4;
            Assert.That(aaa.Font.Size, Is.EqualTo(4d));
            aaa.ParagraphFormat.SpaceBefore = 10;
            Assert.That(aaa.ParagraphFormat.SpaceBefore, Is.EqualTo(10d));
            Assert.That(styles["Heading 1"].Font.Size, Is.EqualTo(8d));
        }

        /// <summary>
        /// Checks how toggle value for bold etc inheritance works in paragraph styles.
        ///
        /// category - Style
        /// </summary>
        [Test]
        public void TestToggleInheritance()
        {
            Document doc = new Document();
            StyleCollection styles = doc.Styles;

            Style normal = styles["Normal"];

            Style h1 = styles["Heading 1"];
            h1.RunPr.Bold = AttrBoolEx.Toggle;

            Style h2 = styles["Heading 2"];
            h2.RunPr.Bold = AttrBoolEx.Toggle;

            Assert.That(normal.Font.Bold, Is.EqualTo(false));    // Default value works okay.
            Assert.That(h1.Font.Bold, Is.EqualTo(true));        // Toggle once value works okay.
            Assert.That(h2.Font.Bold, Is.EqualTo(true));        // Toggle once value works okay.

            h2.BaseStyleName = "Heading 1";
            Assert.That(h2.Font.Bold, Is.EqualTo(false));        // Now this is a toggle twice value and works okay.

            h1.Font.Bold = false;                        // Change the value in the parent.
            Assert.That(h1.Font.Bold, Is.EqualTo(false));        // Explicit value works okay.
            Assert.That(h2.Font.Bold, Is.EqualTo(true));        // Toggle once value works okay.
        }

        /// <summary>
        /// Test how a complex attribute Font.Border on style is inherited and instantiated on demand.
        ///
        /// category - Style
        /// </summary>
        [Test]
        public void TestFontBorderOnStyleInheritance()
        {
            Document doc = new Document();
            StyleCollection styles = doc.Styles;

            Style normal = styles["Normal"];
            Style h1 = styles["Heading 1"];
            Style h2 = styles["Heading 2"];

            //Test defaults as created. This instantiates a new Border object in the "inherit from parent" state.
            Assert.That(normal.Font.Border.LineStyle, Is.EqualTo(LineStyle.None));

            //Change the border in H1. This changes the Border in H1 from inherited to explicit state.
            h1.Font.Border.LineStyle = LineStyle.Single;
            //Make sure it was changed in H1, but not changed in the parent.
            Assert.That(h1.Font.Border.LineStyle, Is.EqualTo(LineStyle.Single));
            Assert.That(normal.Font.Border.LineStyle, Is.EqualTo(LineStyle.None));

            Assert.That(h2.Font.Border.LineStyle, Is.EqualTo(LineStyle.None));
            //Make H2 based on H1 and observe H2 inheriting the Border from H1.
            h2.BaseStyleName = "Heading 1";
            Assert.That(h2.Font.Border.LineStyle, Is.EqualTo(LineStyle.Single));
            //Change Border in H1 once again and observe it is still inherited in H2.
            h1.Font.Border.LineStyle = LineStyle.Double;
            Assert.That(h2.Font.Border.LineStyle, Is.EqualTo(LineStyle.Double));
        }

        /// <summary>
        /// Test how a complex attribute Font.Border on style is cloned from the parent
        /// when it is modified from inherited to explicit.
        ///
        /// category - Style
        /// </summary>
        [Test]
        public void TestFontBorderOnStyleCloneBeforeModify()
        {
            Document doc = new Document();
            StyleCollection styles = doc.Styles;

            Style normal = styles["Normal"];
            Style h1 = styles["Heading 1"];

            //Let's say we have a border on the base style.
            normal.Font.Border.LineStyle = LineStyle.Double;
            normal.Font.Border.LineWidth = 10;

            //This request actually instantiates an inherited Border on H1,
            //but no physical copying of the parent border takes place yet.
            Assert.That(h1.Font.Border.LineWidth, Is.EqualTo(10d));

            //This later helps to see that the border has not been copied to H1 yet.
            normal.Font.Border.LineWidth = 5;

            //Change one of the border properties on the child style.
            //This causes the Border to copy from the base to the child style.
            h1.Font.Border.LineStyle = LineStyle.Single;
            Assert.That(h1.Font.Border.LineWidth, Is.EqualTo(5d));

            //Now change a property in the parent style and observe that it no longer has effect on the child border.
            normal.Font.Border.LineWidth = 2;
            Assert.That(h1.Font.Border.LineWidth, Is.EqualTo(5d));
        }

        /// <summary>
        /// Shows there is a difference when obtaining font attributes from character as opposed to paragraph style.
        ///
        /// category - Style
        /// </summary>
        [Test]
        public void TestFontOnCharVsParaStyle()
        {
            Document doc = new Document();
            StyleCollection styles = doc.Styles;

            Style p = styles[StyleIdentifier.Normal];
            Style c = styles[StyleIdentifier.DefaultParagraphFont];

            //For public access, both char and para styles will resolve using default values.
            Assert.That(12d, Is.EqualTo(p.Font.Size));
            Assert.That(10d, Is.EqualTo(c.Font.Size));

            //However for internal purposes, para style will use default values, but char styles will not.
            //This resolves up all base styles and a default value.
            Assert.That(p.GetFontAttr(FontAttr.Size, true), Is.EqualTo(12 * 2));
            //This resolves all base styles, but not a default value.
            Assert.That(c.GetFontAttr(FontAttr.Size, false), Is.EqualTo(null));
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBlank(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Style\TestBlank", lf, sf);
            Paragraph para = doc.FirstSection.Body.FirstParagraph;
            Assert.That(para.ParagraphBreakFont.Size, Is.EqualTo(11.0));
        }

        /// <summary>
        /// Test that expanding font and paragraph formatting works.
        ///
        /// category - Style
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestExpandStyleFormatting(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Style\TestExpandStyleFormatting", lf, sf);

            ParaPr paraPr = new ParaPr();
            doc.Styles["Heading 1"].ExpandParaPr(paraPr, ParaPrExpandFlags.Normal);

            ParagraphFormat pf = new ParagraphFormat(paraPr, null);
            Assert.That(pf.Alignment, Is.EqualTo(ParagraphAlignment.Right));    //This is expanded from Normal
            Assert.That(pf.SpaceBefore, Is.EqualTo(12d));                        //This is overridden in Heading 1
        }



        [Test]
        public void TestNoFontInListStyle()
        {
            Document doc = TestUtil.Open(@"Model\Style\TestListStyle.docx");
            Style style = doc.Styles[StyleIdentifier.OutlineList1];
            // This throws because it is a list style.
            Assert.That(style.Font, Is.EqualTo(null));
        }

        [Test]
        public void TestNoListInParagraphStyle()
        {
            Document doc = new Document();
            Style style = doc.Styles[StyleIdentifier.Normal];
            // This throws because it is not a list style.
            Assert.That(style.List, Is.EqualTo(null));
        }


        /// <summary>
        /// Test style id generation.
        ///
        /// category - style collection?
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void TestWmlStyleIdGeneration(LoadFormat lf, SaveFormat sf)
        {
            TestUtil.OpenSaveOpen(@"Model\Style\TestWmlStyleIdGeneration", lf, sf);
        }

        /// <summary>
        /// category - Style
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestBehaviorProperties(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Style\TestBehaviorProperties", lf, sf);

            CheckBehaviorProperties(doc, "Normal", false, 0, false, false, true, false, false, lf, sf);
            CheckBehaviorProperties(doc, "Locked", true, 0, false, false, true, false, false, lf, sf);
            CheckBehaviorProperties(doc, "UI Priority 99", false, 98, false, false, true, false, false, lf, sf);
            CheckBehaviorProperties(doc, "UI Priority Last", false, 99, false, false, true, false, false, lf, sf);
            CheckBehaviorProperties(doc, "Semi Hidden", false, 0, true, false, true, false, false, lf, sf);
            CheckBehaviorProperties(doc, "Unhide When Used", false, 0, false, true, true, false, false, lf, sf);
            CheckBehaviorProperties(doc, "Auto Redefine", false, 0, false, false, true, false, true, lf, sf);
            CheckBehaviorProperties(doc, "No Quick Format", false, 0, false, false, false, false, false, lf, sf);
        }

        private static void CheckBehaviorProperties(
            Document doc,
            string styleName,
            bool isLocked,
            int uiPriority,
            bool isSemiHidden,
            bool isUnhideWhenUsed,
            bool isQuickFormat,
            bool isHidden,
            bool isAutoRedefine,
            LoadFormat lf,
            SaveFormat sf)
        {
            switch (TestUtil.GetUnifiedScenario(lf, sf))
            {
                case UnifiedScenario.Doc2Doc:
                case UnifiedScenario.Doc2Docx:
                case UnifiedScenario.Doc2Rtf:
                case UnifiedScenario.Docx2DocxNoGold:
                case UnifiedScenario.Rtf2RtfNoGold:
                    break;

                case UnifiedScenario.Doc2Wml:
                case UnifiedScenario.Wml2WmlNoGold:
                    // Exclude WML. It has no support for QuickFormat, SemiHidden, UnhideWhenUsed and UiPriority.
                    return;

                default:
                    throw new InvalidOperationException("Unknown file format.");
            }

            Style style = doc.Styles[styleName];
            Assert.That(style.Locked, Is.EqualTo(isLocked));
            Assert.That(style.Priority, Is.EqualTo(uiPriority));
            Assert.That(style.SemiHidden, Is.EqualTo(isSemiHidden));
            Assert.That(style.UnhideWhenUsed, Is.EqualTo(isUnhideWhenUsed));
            Assert.That(style.IsQuickStyle, Is.EqualTo(isQuickFormat));
            Assert.That(style.Hidden, Is.EqualTo(isHidden));
            Assert.That(style.AutomaticallyUpdate, Is.EqualTo(isAutoRedefine));
        }

        /// <summary>
        /// category - latent styles
        /// </summary>
        [Test]
        public void TestLatentStylesClone()
        {
            Document doc = new Document();
            LatentStyles latentStyles = doc.Styles.LatentStyles;
            LatentStyle latentStyle = new LatentStyle(StyleIdentifier.Hyperlink, true, false, false, 9, true);
            latentStyles.Add(latentStyle);

            Document clonedDoc = doc.Clone();
            Assert.That(doc.Styles.LatentStyles != clonedDoc.Styles.LatentStyles, Is.True);
            LatentStyles clonedLatentStyles = clonedDoc.Styles.LatentStyles;
            Assert.That(latentStyles != clonedLatentStyles, Is.True);
            Assert.That(clonedLatentStyles.Count, Is.EqualTo(latentStyles.Count));
            LatentStyle clonedLatentStyle = clonedLatentStyles[StyleIdentifier.Hyperlink];
            Assert.That(clonedLatentStyle.UIPriority, Is.EqualTo(9));
            Assert.That(latentStyle != clonedLatentStyle, Is.True);
        }

        /// <summary>
        /// category - latent styles
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestLatentStyles(LoadFormat lf, SaveFormat sf)
        {
            // UI related style features are missing in WML format.
            if (sf == SaveFormat.WordML)
                return;

            Document doc = TestUtil.OpenSaveOpen(@"Model\Style\TestLatentStyles", lf, sf);

            LatentStyles ls = doc.Styles.LatentStyles;

            // Check defaults.
            Assert.That(ls.KnownStylesCount, Is.EqualTo(267));
            Assert.That(ls.DefaultLockedState, Is.EqualTo(false));
            Assert.That(ls.DefaultQuickFormat, Is.EqualTo(false));
            Assert.That(ls.DefaultSemiHidden, Is.EqualTo(true));
            Assert.That(ls.DefaultUIPriority, Is.EqualTo(99));
            Assert.That(ls.DefaultUnhideWhenUsed, Is.EqualTo(true));

            switch (TestUtil.GetUnifiedScenario(lf, sf))
            {
                case UnifiedScenario.Doc2Doc:
                case UnifiedScenario.Doc2Docx:
                case UnifiedScenario.Doc2Rtf:
                case UnifiedScenario.Docx2DocxNoGold:
                case UnifiedScenario.Rtf2RtfNoGold:
                case UnifiedScenario.DocxDml2DocxDml:
                    // Check some lsds.
                    Assert.That(ls.Count, Is.EqualTo(137));
                    CheckLatentStyle(ls, StyleIdentifier.Normal, false, true, false, 0, false);
                    CheckLatentStyle(ls, StyleIdentifier.MediumGrid1Accent6, false, false, false, 67, false);
                    Assert.That(ls[StyleIdentifier.Hyperlink], Is.EqualTo(null));
                    break;
                default:
                    throw new InvalidOperationException("Unknown file format.");
            }
        }

        private static void CheckLatentStyle(LatentStyles ls, StyleIdentifier sti, bool locked, bool quickFormat, bool semiHidden, int uiPriority, bool unhideWhenUsed)
        {
            LatentStyle latentStyle = ls[sti];
            Assert.That(latentStyle.Locked, Is.EqualTo(locked));
            Assert.That(latentStyle.UIPriority, Is.EqualTo(uiPriority));
            Assert.That(latentStyle.SemiHidden, Is.EqualTo(semiHidden));
            Assert.That(latentStyle.UnhideWhenUsed, Is.EqualTo(unhideWhenUsed));
            Assert.That(latentStyle.QuickStyle, Is.EqualTo(quickFormat));
        }

        [Test]
        public void TestClonedFromStyleIdentifier()
        {
            Document doc = new Document();

            Style hyperlinkStyle = doc.Styles[StyleIdentifier.Hyperlink];
            Style clonedHyperlinkStyle = hyperlinkStyle.Clone();

            Assert.That(clonedHyperlinkStyle.ClonedFromStyleIdentifier, Is.EqualTo(StyleIdentifier.Hyperlink));
        }

        /// <summary>
        /// Test creating document from scratch and checking BuildIn style.
        /// Expect Word 2003 styles by default.
        /// </summary>
        [Test]
        public void TestCreateDocumentFromScratch()
        {
            Document doc = new Document();

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToDocumentEnd();
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;
            builder.Writeln("Heading 1");
            TestStyle2003(doc);
        }

        /// <summary>
        /// Test importing BuildIn styles upon appending document.
        /// Expect Word 2007 styles in case of UseDestinationStyles specified. See WORDSNET-9188
        /// </summary>
        [Test]
        public void TestStyleImport()
        {
            Document dstDoc = new Document();
            Document srcDoc = TestUtil.Open(@"Model\Style\TestJira3826StyleImport.docx");

            dstDoc.AppendDocument(srcDoc, ImportFormatMode.UseDestinationStyles);

            dstDoc = TestUtil.SaveOpen(dstDoc, @"Model\Style\TestJira3826StyleImport.docx");
            TestStyle2007(dstDoc, dstDoc.LastSection.Body.FirstParagraph);
        }

        /// <summary>
        /// Test importing Docx to Doc.
        /// </summary>
        [Test]
        public void TestStyleImportDocxToDoc()
        {
            Document doc = TestUtil.Open(@"Model\Style\TestJira3826StyleImport.docx");

            // FOSS No gold check (the Doc-era gold differs from the Docx roundtrip); TestStyle2007 is the point.
            doc = TestUtil.SaveOpen(doc, @"Model\Style\TestJira3826StyleImport.docx", (SaveOptions)null, false);
            TestStyle2007(doc, doc.FirstSection.Body.FirstParagraph);
        }

        /// <summary>
        /// WORDSNET-6467 Appending documents into empty (new Document()) stretches the contents of one page into multiple pages.
        /// andrnosk: The problem occurred because paragraphs (without directly specified spacing) inside tables inherited formatting from Defaults,
        /// but Table style was ignored. Fixed by expanding table style properties into paragraph.
        /// </summary>
        [Test]
        public void TestJira6467()
        {
            Document dstDoc = new Document();
            dstDoc.RemoveAllChildren();
            Document srcDoc = TestUtil.Open(@"Model\Style\TestJira6467.docx");

            dstDoc.AppendDocument(srcDoc, ImportFormatMode.KeepSourceFormatting);

            dstDoc = TestUtil.SaveOpen(dstDoc, @"Model\Style\TestJira6467.docx");

            Paragraph paragraph = dstDoc.FirstSection.Body.Tables[0].Rows[0].Cells[0].FirstParagraph;
            Assert.That(paragraph.ParagraphFormat.SpaceBefore, Is.EqualTo(0));
            Assert.That(paragraph.ParagraphFormat.SpaceAfter, Is.EqualTo(0));
            Assert.That(paragraph.ParagraphFormat.LineSpacingRule, Is.EqualTo(LineSpacingRule.Multiple));
            Assert.That(paragraph.ParagraphFormat.LineSpacing, Is.EqualTo(12.0d));
        }

        /// <summary>
        /// Check whether it is Word 2003 style.
        /// </summary>
        private static void TestStyle2003(Document dstDoc)
        {
            Assert.That(dstDoc.LastSection.Body.FirstParagraph.ParagraphFormat.StyleIdentifier, Is.EqualTo(StyleIdentifier.Heading1));
            // FOSS RK Arial 16 became Cambria 14 after I migrated from Blank.doc to Blank.docx. Check later.
            Assert.That(dstDoc.Styles[StyleIdentifier.Heading1].Font.Name, Is.EqualTo("Cambria"));
            Assert.That(dstDoc.Styles[StyleIdentifier.Heading1].Font.Size, Is.EqualTo(14.0d));
        }

        /// <summary>
        /// Check whether it is Word 2007 style.
        /// </summary>
        private static void TestStyle2007(Document doc, Paragraph para)
        {
            Assert.That(para.ParagraphFormat.StyleIdentifier, Is.EqualTo(StyleIdentifier.Heading1));
            Assert.That(doc.Styles[StyleIdentifier.Heading1].Font.Name, Is.EqualTo("Cambria"));
            Assert.That(doc.Styles[StyleIdentifier.Heading1].Font.Size, Is.EqualTo(14.0d));
        }

        /// <summary>
        /// Test loading of all formats and checking BuildIn style.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestJira3826(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Style\TestJira3826", lf);

            DocumentBuilder builder = new DocumentBuilder(doc);
            builder.MoveToDocumentEnd();
            builder.ParagraphFormat.StyleIdentifier = StyleIdentifier.Heading1;
            builder.Writeln("Heading 1");

            StyleCollection styles = doc.Styles;
            Style h1 = styles["Heading 1"];

            TestUtil.SaveOpen(doc, @"Model\Style\TestJira3826", lf, sf);

            switch (TestUtil.GetUnifiedScenario(lf, sf))
            {
                case UnifiedScenario.Doc2Doc:
                case UnifiedScenario.Doc2Docx:
                case UnifiedScenario.Doc2Rtf:
                case UnifiedScenario.Doc2Wml:
                case UnifiedScenario.Wml2WmlNoGold:
                case UnifiedScenario.Rtf2RtfNoGold:
                    {
                        Assert.That(h1.Font.Name, Is.EqualTo("Arial"));
                        Assert.That(h1.Font.Size, Is.EqualTo(16));

                        ComplexFontName arial = ComplexFontName.FromName("Arial");
                        Assert.That(h1.RunPr[FontAttr.NameAscii], Is.EqualTo(arial));
                        Assert.That(h1.RunPr[FontAttr.NameBi], Is.EqualTo(arial));
                        Assert.That(h1.RunPr[FontAttr.NameFarEast], Is.Null);
                        Assert.That(h1.RunPr[FontAttr.NameOther], Is.EqualTo(arial));
                        break;
                    }
                case UnifiedScenario.Docx2DocxNoGold:
                    {
                        // NameAscii, NameBi, NameFarEast, NameOther are null,
                        // and ThemeAscii, ThemeBi, ThemeEastAsia, ThemeOther not null, it means font is specified through theme.
                        Assert.That(h1.RunPr[FontAttr.NameAscii], Is.EqualTo(ComplexFontName.FromTheme(ThemeFontCore.MajorHAnsi)));
                        Assert.That(h1.RunPr[FontAttr.NameBi], Is.EqualTo(ComplexFontName.FromTheme(ThemeFontCore.MajorBidi)));
                        Assert.That(h1.RunPr[FontAttr.NameFarEast], Is.EqualTo(ComplexFontName.FromTheme(ThemeFontCore.MajorEastAsia)));
                        Assert.That(h1.RunPr[FontAttr.NameOther], Is.EqualTo(ComplexFontName.FromTheme(ThemeFontCore.MajorHAnsi)));
                        break;
                    }
                default:
                    throw new InvalidOperationException("Unknown file format.");
            }
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRemoveParagraphStyle(LoadFormat lf, SaveFormat sf)
        {
            const string fileName = @"Model\Style\RemoveParagraphStyle";
            Document doc = TestUtil.Open(fileName, lf);

            Style oldStyle1 = doc.Styles["Style1"];
            // Style2 is a child of Style1.
            Style oldStyle2 = doc.Styles["Style2"];
            // Check all paragraph and run properties after collapse.
            if (lf == LoadFormat.Doc)
            {
                Assert.That(oldStyle1.RunPr.Bold, Is.EqualTo(AttrBoolEx.Toggle));
                Assert.That(oldStyle2.RunPr.Italic, Is.EqualTo(AttrBoolEx.Toggle));
            }
            else
            {
                Assert.That(oldStyle1.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));
                Assert.That(oldStyle2.RunPr.Italic, Is.EqualTo(AttrBoolEx.True));
            }

            Assert.That(oldStyle1.RunPr.Underline, Is.EqualTo(Underline.Single));
            Assert.That((bool)oldStyle1.ParaPr[ParaAttr.AddSpaceBetweenFarEastAndAlpha], Is.False);
            Assert.That((bool)oldStyle1.ParaPr[ParaAttr.AddSpaceBetweenFarEastAndDigit], Is.False);
            Assert.That(oldStyle1.ParaPr.LeftIndent, Is.EqualTo(1008));

            // These properties were inherited from Style1.
            Assert.That(oldStyle2.ParaPr[ParaAttr.AddSpaceBetweenFarEastAndAlpha], Is.Null);
            Assert.That(oldStyle2.ParaPr[ParaAttr.AddSpaceBetweenFarEastAndDigit], Is.Null);
            Assert.That(oldStyle2.RunPr.Bold, Is.EqualTo(AttrBoolEx.False));

            doc.Styles.Remove(oldStyle1.Name);

            doc = TestUtil.SaveOpen(doc, fileName, lf, sf);
            // Check that Style1 doesn't exist in StyleCollection.
            Assert.That(doc.Styles["Style1"], Is.Null);

            Style newStyle2 = doc.Styles["Style2"];

            // Check all properties which were inherited from Style1.
            Assert.That(newStyle2.RunPr.Bold.ToBool(), Is.True);
            Assert.That(newStyle2.RunPr.Underline, Is.EqualTo(Underline.Single));
            Assert.That(newStyle2.RunPr.Italic.ToBool(), Is.True);
            Assert.That((bool)newStyle2.ParaPr[ParaAttr.AddSpaceBetweenFarEastAndAlpha], Is.False);
            Assert.That((bool)newStyle2.ParaPr[ParaAttr.AddSpaceBetweenFarEastAndDigit], Is.False);
            Assert.That(newStyle2.ParaPr.LeftIndent, Is.EqualTo(1008));

        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRemoveCharacterStyle(LoadFormat lf, SaveFormat sf)
        {
            const string fileName = @"Model\Style\RemoveCharacterStyle";

            Document doc = TestUtil.Open(fileName, lf);
            Style oldStyle2 = doc.Styles["Style2"];
            Style oldStyle3 = doc.Styles["Style3"];

            Assert.That(oldStyle2.Font.Subscript, Is.True);
            Assert.That(oldStyle2.RunPr.NameAscii, Is.EqualTo("Arial"));

            Assert.That(oldStyle3.Font.Subscript, Is.True);
            Assert.That(oldStyle3.RunPr.NameAscii, Is.EqualTo("Times New Roman"));
            Assert.That(oldStyle3.RunPr.Underline, Is.EqualTo(Underline.Single));

            doc.Styles.Remove(oldStyle2.Name);

            doc = TestUtil.SaveOpen(doc, fileName, lf, sf);

            // Check that Style2 doesn't exist in StyleCollection.
            Assert.That(doc.Styles["Style2"], Is.Null);

            Style newStyle3 = doc.Styles["Style3"];

            // Check all properties which were inherited from Style2.
            Assert.That(newStyle3.RunPr.NameAscii, Is.EqualTo("Arial"));
            Assert.That(newStyle3.Font.Subscript, Is.True);
            Assert.That(newStyle3.RunPr.Underline, Is.EqualTo(Underline.Single));

        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRemoveTableStyle(LoadFormat lf, SaveFormat sf)
        {
            if ((lf == LoadFormat.Rtf) || (sf == SaveFormat.Rtf))
            {
                Debug.WriteLine("Table styles are not implemented in Rtf.");
                return;
            }

            const string fileName = @"Model\Style\RemoveTableStyle";

            Document doc = TestUtil.Open(fileName, lf);

            TableStyle tableStyle1 = (TableStyle)doc.Styles["Style1"];
            Assert.That(tableStyle1.CellPr.Shading.BackgroundPatternColorInternal.Equals(new DrColor(0xff, 0x9B, 0xBB, 0x59)), Is.True);
            ConditionalStyle oldConditionalStyle = tableStyle1.ConditionalStyles.FirstRow;
            Assert.That(oldConditionalStyle.RunPr.Bold.ToBool(), Is.True);

            doc.Styles.Remove("Style1");
            doc = TestUtil.SaveOpen(doc, fileName, lf, sf);

            // Check that Style doesn't exist in StyleCollection.
            Assert.That(doc.Styles["Style1"], Is.Null);

            TableStyle newTableStyle = (TableStyle) doc.Styles["Style2"];

            // Check all properties which were inherited from Style1.
            Assert.That(newTableStyle.CellPr.Shading.BackgroundPatternColorInternal.Equals(new DrColor(0xff, 0x9B, 0xBB, 0x59)), Is.True);
            ConditionalStyle newConditionalStyle = tableStyle1.ConditionalStyles.FirstRow;
            Assert.That(newConditionalStyle.RunPr.Bold.ToBool(), Is.True);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRemoveListStyle(LoadFormat lf, SaveFormat sf)
        {
            const string fileName = @"Model\Style\RemoveListStyle";

            Document doc = TestUtil.Open(fileName, lf);

            Style listStyle = doc.Styles["Style1"];
            Paragraph paragraph = (Paragraph)doc.GetNodeById("0.0.0");
            ParagraphFormat pf = paragraph.ParagraphFormat;

            // The paragraph refers to a list that references the list style.
            Assert.That(pf.ListId, Is.EqualTo(2));
            Assert.That(paragraph.ListFormat.List.IsListStyleReference, Is.EqualTo(true));
            Assert.That(paragraph.ListFormat.List.IsListStyleDefinition, Is.EqualTo(false));
            Assert.That(paragraph.ListFormat.List.Style, Is.EqualTo(listStyle));

            doc.Styles.Remove(listStyle.Name);

            // Check that Style1 doesn't exist in StyleCollection.
            Assert.That(doc.Styles["Style1"], Is.Null);
            Assert.That(paragraph.ParaPr.ListId, Is.EqualTo(0));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCopyCharacterStyle(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Style\CopyCharacterStyle", lf);
            Style oldStyle1 = doc.Styles["Style1"];
            // Style2 is a child of Style1.
            Style oldStyle2 = doc.Styles["Style2"];
            // Style3 is a child of Style2.
            Style oldStyle3 = doc.Styles["Style3"];

            // Check all paragraph and run properties after collapse.
            // These properties were inherited from Style1 and Style2.
            if (lf == LoadFormat.Doc)
            {
                Assert.That(oldStyle1.RunPr.Italic, Is.EqualTo(AttrBoolEx.Toggle));
                Assert.That(oldStyle2.RunPr.Bold, Is.EqualTo(AttrBoolEx.Toggle));
            }
            else
            {
                Assert.That(oldStyle1.RunPr.Italic, Is.EqualTo(AttrBoolEx.True));
                Assert.That(oldStyle2.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));
            }

            Assert.That(oldStyle3.RunPr.Name, Is.EqualTo("Andalus"));

            Document blank = new Document();
            blank.Styles.AddCopy(oldStyle3);
            Style newStyle3 = blank.Styles["Style3"];

            // Check all properties which were inherited from Style1 and Style2.
            if (lf == LoadFormat.Doc)
            {
                // IS. We should rewrite AttrBoolEx because we have Toggle and Same only in Doc format.
                // I set Italic in Style1 and  set Bold in Style2.
                // Problem occurs because in Doc format Bold and Italic are set to Toggle in dependencies styles
                // and these properties are set to Toggle in Style3 but it's incorrect.
                Assert.That(newStyle3.RunPr.Italic, Is.EqualTo(AttrBoolEx.False));
                Assert.That(newStyle3.RunPr.Bold, Is.EqualTo(AttrBoolEx.False));
            }
            else
            {
                Assert.That(newStyle3.RunPr.Italic, Is.EqualTo(AttrBoolEx.True));
                Assert.That(newStyle3.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));
            }

            Assert.That(newStyle3.RunPr.Name, Is.EqualTo("Andalus"));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCopyParagraphStyle(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.Open(@"Model\Style\CopyParagraphStyle", lf);

            Style oldStyle1 = doc.Styles["Style1"];
            // Style2 is a child of Style1.
            Style oldStyle2 = doc.Styles["Style2"];
            // Style3 is a child of Style2.
            Style oldStyle3 = doc.Styles["Style3"];

            // Check all paragraph and run properties after collapse.
            // These properties were inherited from Style1 and Style2.
            Assert.That(oldStyle1.ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.Center));
            Assert.That(oldStyle1.RunPr.Bold, Is.EqualTo((lf == LoadFormat.Doc) ? AttrBoolEx.Toggle : AttrBoolEx.True));
            Assert.That(oldStyle2.ParaPr.FirstLineIndent, Is.EqualTo(720));

            Assert.That(oldStyle3.ParaPr.LeftIndent, Is.EqualTo(1440));

            Document blank = new Document();
            blank.Styles.AddCopy(oldStyle3);
            Style newStyle3 = blank.Styles["Style3"];

            // Check all properties which were inherited from Style1 and Style2.
            Assert.That(newStyle3.ParaPr.Alignment, Is.EqualTo(ParagraphAlignment.Center));
            Assert.That(newStyle3.RunPr.Bold, Is.EqualTo(AttrBoolEx.True));
            Assert.That(newStyle3.ParaPr.FirstLineIndent, Is.EqualTo(720));
            Assert.That(newStyle3.ParaPr.LeftIndent, Is.EqualTo(1440));
        }


        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCopyTableStyle(LoadFormat lf, SaveFormat sf)
        {
            if (lf == LoadFormat.Rtf || sf == SaveFormat.Rtf)
            {
                Debug.WriteLine("Table styles are not implemented in Rtf.");
                return;
            }

            Document doc = TestUtil.Open(@"Model\Style\CopyTableStyle", lf);

            TableStyle oldTableStyle = (TableStyle)doc.Styles.GetByName("Table 3D effects 1", false);
            Assert.That(oldTableStyle.Name, Is.EqualTo("Table 3D effects 1"));
            Assert.That(oldTableStyle.CellPr.Shading.ForegroundPatternColorInternal, Is.EqualTo(DrColor.FromArgb(0xc0, 0xc0, 0xc0)));
            ConditionalStyle oldConditionalStyle = oldTableStyle.ConditionalStyles.FirstRow;
            Assert.That(oldConditionalStyle.RunPr.Color.Equals(new DrColor(0xff, 0x80, 0x00, 0x80)), Is.True);

            Document blank = new Document();
            blank.Styles.AddCopy(oldTableStyle);
            TableStyle newTableStyle = (TableStyle)doc.Styles["Table 3D effects 1"];
            Assert.That(oldTableStyle.Name, Is.EqualTo("Table 3D effects 1"));
            Assert.That(newTableStyle.CellPr.Shading.ForegroundPatternColorInternal, Is.EqualTo(DrColor.FromArgb(0xc0, 0xc0, 0xc0)));
            ConditionalStyle conditionalStyle = newTableStyle.ConditionalStyles.FirstRow;
            Assert.That(conditionalStyle.RunPr.Color.Equals(new DrColor(0xff, 0x80, 0x00, 0x80)), Is.True);
        }

        /// <summary>
        /// WORDSNET-4782 Paragraph SpaceAfter/SpaceBefore is incorrect.
        /// andrnosk: If parent style is null we need to check style ParaPr defaults before getting global defaults.
        /// The same we already do for style RunPr defaults.
        /// </summary>
        [Test]
        public void TestJira4782()
        {
            Document doc = TestUtil.Open(@"Model\Style\TestJira4782.docx");

            ParagraphFormat dirParagraphFormat = doc.FirstSection.Body.FirstParagraph.ParagraphFormat;
            ParagraphFormat styleParagraphFormat = doc.Styles[StyleIdentifier.Title].ParagraphFormat;

            Assert.That(dirParagraphFormat.SpaceAfter, Is.EqualTo(15));
            Assert.That(dirParagraphFormat.SpaceBefore, Is.EqualTo(18));
            Assert.That(styleParagraphFormat.SpaceAfter, Is.EqualTo(15));
            Assert.That(styleParagraphFormat.SpaceBefore, Is.EqualTo(18));
        }



        /// <summary>
        /// WORDSNET-2304 Consider adding an ability to remove style from the document.
        /// Tests that attributes from base style being deleted are expanded into child styles.
        /// </summary>
        [Test]
        public void TestJira2304_Expanding()
        {
            Document doc = TestUtil.Open(@"Model\Style\TestJira2304_Expanding.docx");

            Assert.That(doc.Styles["Style1"], IsNot.Null());

            Assert.That(doc.Styles["Style2"].RunPr[FontAttr.Bold], Is.Null);
            Assert.That(doc.Styles["Style2"].RunPr[FontAttr.Italic], Is.Null);

            doc.Styles["Style1"].Remove();

            // Style is removed.
            Assert.That(doc.Styles["Style1"], Is.Null);

            // Child style got attributes from removed style.
            Assert.That(doc.Styles["Style2"].RunPr[FontAttr.Bold], Is.EqualTo(AttrBoolEx.True));
            Assert.That(doc.Styles["Style2"].RunPr[FontAttr.Italic], Is.EqualTo(AttrBoolEx.True));
        }

        /// <summary>
        /// WORDSNET-2304 Consider adding an ability to remove style from the document.
        /// Tests that if linked style is being deleted both styles are deleted.
        /// </summary>
        [Test]
        public void TestJira2304_Linked()
        {
            Document doc = TestUtil.Open(@"Model\Style\TestJira2304_Linked.docx");

            Style style = doc.Styles["Style3"];
            Assert.That(style.GetLinkedStyle(), IsNot.Null());

            doc.Styles["Style3 Char"].Remove();

            // Both styles are removed.
            Assert.That(doc.Styles["Style3"], Is.Null);
            Assert.That(doc.Styles["Style3 Char"], Is.Null);
        }

        /// <summary>
        /// WORDSNET-2304 Consider adding an ability to remove style from the document.
        /// Tests that style for following paragraph (NextIstd) is set properly upon style deletion.
        /// </summary>
        [Test]
        public void TestJira2304_NextIstd()
        {
            Document doc = TestUtil.Open(@"Model\Style\TestJira2304_NextIstd.docx");

            Style style1 = doc.Styles["Style1"];

            TestStyle(doc, "Style2", "Style1", "Style1");
            TestStyle(doc, "Style3", "Normal", "Style1");
            TestStyle(doc, "Style4", "Style1", "Style1");
            TestStyle(doc, "Style5", "Style2", "Style1");

            style1.Remove();

            // Verify links to base and following styles are updated properly.
            TestStyle(doc, "Style2", "Normal", "Style2");
            TestStyle(doc, "Style3", "Normal", "Normal");
            TestStyle(doc, "Style4", "Normal", "Style4");
            TestStyle(doc, "Style5", "Style2", "Normal");
        }

        /// <summary>
        /// WORDSNET-2304 Consider adding an ability to remove style from the document.
        /// Tests how document model is updated.
        /// </summary>
        [Test]
        public void TestJira2304_DocumentModel()
        {
            Document doc = TestUtil.Open(@"Model\Style\TestJira2304_DocumentModel.docx");

            // CharacterTest changes is tracked.
            Assert.That(doc.Revisions.Count, Is.EqualTo(1));

            ParaPr paraPr = doc.FirstSection.Body.Paragraphs[0].ParaPr;
            Assert.That(paraPr.Istd, Is.EqualTo(0x0f));

            RunPr runPr = doc.FirstSection.Body.Paragraphs[1].Runs[1].RunPr;
            Assert.That(runPr.Istd, Is.EqualTo(0x10));

            TablePr tablePr = ((Row)doc.GetChild(NodeType.Row, 0, true)).TablePr;
            Assert.That(tablePr.Istd, Is.EqualTo(0x12));

            // Remove paragraph styles.
            doc.Styles["ParagraphTest"].Remove();
            Assert.That(paraPr[ParaAttr.Istd], Is.Null);

            // Remove character style. Style is also removed from Document.Revisions collection.
            doc.Styles["CharacterTest"].Remove();
            Assert.That(runPr[FontAttr.Istd], Is.Null);
            Assert.That(doc.Revisions.Count, Is.EqualTo(0));

            // Remove table style.
            doc.Styles["TableTest"].Remove();
            Assert.That(tablePr.Istd, Is.EqualTo(0x0b));
        }


        /// <summary>
        /// WORDSNET-8850 doc.Styles[StyleIdentifier.NoteHeading] does not create style in the document.
        /// NoteHeading seems to be deprecated builtin style, it is presented in DOC but absent in DOCX.
        /// Special processing for this style is added to perform unified behavior.
        /// </summary>
        [Test]
        public void TestJira8850()
        {
            Document doc = new Document();
            Style noteHeading = doc.Styles.GetBySti(StyleIdentifier.NoteHeading, true);

            // Verify that "Note Heading" style is created.
            Assert.That(noteHeading, IsNot.Null());
            Assert.That(noteHeading.BuiltIn, Is.True);

            // Verify that linked "Note Heading Char" is also created.
            Style noteHeadingChar = doc.Styles.GetByIstd(noteHeading.LinkedIstd, true);
            Assert.That(noteHeadingChar.Name, Is.EqualTo("Note Heading Char"));
            Assert.That(noteHeadingChar.BuiltIn, Is.False);
        }

        /// <summary>
        /// WORDSNET-10695 /list label attrs/ Docx to Pdf conversion issue with numbering style
        /// Word ignores any formatting specified in "Default Paragraph Font".
        /// </summary>
        [Test]
        public void TestJira10695()
        {
            Document doc = TestUtil.Open(@"Model\Style\TestJira10695.docx");
            Paragraph p = doc.FirstSection.Body.FirstParagraph;
            RunPr runPr = p.GetExpandedParagraphBreakRunPr(RunPrExpandFlags.Layout);
            Assert.That(runPr.Bold, Is.EqualTo(AttrBoolEx.True));
        }

        /// <summary>
        /// WORDSNET-10693 Paragraph spacing is lost after re-saving the Docx
        /// "type" attribute missing in style definition (we have Paragraph by default) and Table Normal becomes StyleType.Paragraph after loading.
        /// </summary>
        [Test]
        public void TestJira10693()
        {
            Document doc = TestUtil.Open(@"Model\Style\TestJira10693.docx");

            TableStyle tableNormal = (TableStyle)doc.Styles.GetByIstd(StyleIndex.TableNormal, false);
            Assert.That(tableNormal, IsNot.Null());

            Assert.That(tableNormal.Type, Is.EqualTo(StyleType.Table));
        }



        /// <summary>
        /// WORDSNET-10872 Provide Style.Aliases property
        /// </summary>
        [Test]
        public void TestJira10872_AliasesDoNotExist()
        {
            Document doc = new Document();
            Style style = doc.Styles["Normal"];
            Assert.That(style.Aliases, Is.Empty);
        }

        /// <summary>
        /// WORDSNET-10872 Provide Style.Aliases property
        /// </summary>
        [Test]
        public void TestJira10872_AliasesExist()
        {
            const string testName = @"Model\Style\TestJira10872.docx";
            Document doc = TestUtil.Open(testName);
            Style style = doc.Styles["NewStyle"];
            Assert.That(style.Aliases, Is.EqualTo(new string[] {"New-Style", "NouveauStyle", "N-S"}));
        }




        /// <summary>
        /// Relates to WORDSNET-4079
        /// Checks how default paragraph formatting is accessed.
        /// </summary>
        [Test]
        public void TestJira4079ParagraphFormat()
        {
            const string testName = @"Model\Style\TestJira4079 ParagraphFormat.docx";

            Document doc = TestUtil.Open(testName);

            Assert.That(doc.Styles.DefaultParagraphFormat.SpaceAfter, Is.EqualTo(8));

            doc.Styles.DefaultParagraphFormat.SpaceAfter = 20;
            doc = TestUtil.SaveOpen(doc, testName, SaveOptions.CreateSaveOptions(SaveFormat.Docx), false);
            Assert.That(doc.Styles.DefaultParagraphFormat.SpaceAfter, Is.EqualTo(20));
        }




        /// <summary>
        /// WORDSNET-14467 Custom font formatting applied via Template gets lost in PDF.
        /// In addition to cloning 'defaults' into 'Normal' style,
        /// need to expand 'Normal' properties when updating styles from template.
        /// </summary>
        [Test]
        public void TestJira14467()
        {
            Document doc = TestUtil.Open(@"Model\Style\TestJira14467.docx");
            doc.AttachedTemplate = TestUtil.BuildTestFileName(@"Model\Style\TestJira14467.dotx");

            TestUtil.ExecuteValidator(doc, SaveFormat.Markdown);

            TestJira14467Core(doc);
        }

        /// <summary>
        /// WORDSNET-14429 Document based on a template loses normal style after saving with Aspose.Words.
        /// This issue duplicates WORDSNET-14467
        /// </summary>
        [Test]
        public void TestJira14429()
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Style\TestJira14467", UnifiedScenario.Docx2DocxNoGold);
            doc.AttachedTemplate = TestUtil.BuildTestFileName(@"Model\Style\NonExistentTemplateFile");
            doc = TestUtil.SaveOpen(doc, @"Model\Style\TestJira14429", UnifiedScenario.Docx2DocxNoGold);

            TestJira14467Core(doc);
        }

        /// <summary>
        /// Tests links to base and following styles.
        /// </summary>
        private static void TestStyle(Document doc, string styleName, string basedOnName, string followingByName)
        {
            Style style = doc.Styles.GetByName(styleName, false);

            Assert.That(style.BaseStyleName, Is.EqualTo(basedOnName));
            Assert.That(style.NextParagraphStyleName, Is.EqualTo(followingByName));
        }

        /// <summary>
        /// Core method for testing WORDSNET-14467 and WORDSNET-14429
        /// Checks some significant for these tests properties of 'Normal' style.
        /// </summary>
        private static void TestJira14467Core(Document doc)
        {
            Assert.That(doc.DocPr.LinkStyles, Is.True);

            Style normal = doc.Styles.GetBySti(StyleIdentifier.Normal, false);

            // Check run attributes.
            RunPr runPr = normal.RunPr;
            ComplexFontName officeFont = ComplexFontName.FromName("Aldhabi");
            // These are from 'Normal'. Defaults have these attributes too, but with some other values.
            Assert.That(runPr[FontAttr.NameAscii], Is.EqualTo(officeFont));
            Assert.That(runPr[FontAttr.Color], Is.EqualTo(DrColor.FromArgb(0xFF, 0x0, 0x0)));
            // These are from defaults. 'Normal' does not contain these attributes at all.
            Assert.That(runPr[FontAttr.Size], Is.EqualTo(200));
            Assert.That(runPr[FontAttr.Bold], Is.EqualTo(AttrBoolEx.True));
            // 'Italic' is set in 'Normal' of source document, but it is not defined in template at all.
            Assert.That(runPr.Contains(FontAttr.Italic), Is.False);

            // Check paragraph attributes.
            LineSpacing lineSpacing = (LineSpacing)normal.ParaPr[ParaAttr.LineSpacing];
            // 300 is in 'Normal', 259 is in defaults.
            Assert.That(lineSpacing.Value, Is.EqualTo(300));
        }

        /// <summary>
        /// WORDSNET-15274 Read-only access to style properties caused that the Style.Equals method returned
        /// <c>false</c> for identical styles.
        /// </summary>
        [Test]
        public void TestJira15274()
        {
            Document doc = new Document();

            Style style1 = doc.Styles.Add(StyleType.Paragraph, "my1");
            Style style2 = doc.Styles.Add(StyleType.Paragraph, "my2");

            // Call properties to create and add them to internal attribute collection.
            Assert.That(style1.ParagraphFormat.Shading, IsNot.Null());
            Assert.That(style1.ParagraphFormat.Borders[BorderType.Left], IsNot.Null());
            // It is not fixed for the ParagraphFormat.TabStops property yet.

            Assert.That(style1.Equals(style2), Is.True);
        }


        /// <summary>
        /// WORDSNET-16374 Table row is rendered on previous page in output PDF.
        /// Word uses font size value from default paragraph style when setting “overrideTableStyleFontSizeAndJustification”
        /// is switched off, paragraph is located into the table, font size in the default paragraph style is 12 pt and actual
        /// size value before expanding direct formatting from style is 10 pt. Emulate the same behavior to fix the problem.
        /// </summary>
        [Test]
        public void TestJira16374()
        {
            Document doc = TestUtil.Open(@"Model\Style\TestJira16374.docx");
            TableCollection tables = doc.FirstSection.Body.Tables;

            CheckExpandedSizeVal(tables[0].FirstRow.FirstCell.FirstParagraph, FontAttr.Size, 26);
            CheckExpandedSizeVal(tables[1].FirstRow.FirstCell.FirstParagraph, FontAttr.Size, 24);
            CheckExpandedSizeVal(tables[2].FirstRow.FirstCell.FirstParagraph, FontAttr.Size, 22);
            CheckExpandedSizeVal(tables[3].FirstRow.FirstCell.FirstParagraph, FontAttr.Size, 24);
            CheckExpandedSizeVal(tables[4].FirstRow.FirstCell.FirstParagraph, FontAttr.Size, 18);
            CheckExpandedSizeVal(tables[5].FirstRow.FirstCell.FirstParagraph, FontAttr.Size, 24);
            CheckExpandedSizeVal(tables[6].FirstRow.FirstCell.FirstParagraph, FontAttr.Size, 40);
        }

        /// <summary>
        /// WORDSNET-19927 Incorrect font size in output PDF.
        /// Iso29500ComplianceEnforcer (incorrectly) changed unspecified value of compatibility
        /// setting “overrideTableStyleFontSizeAndJustification”.
        /// </summary>
        [Test]
        public void Test19927()
        {
            Document doc = TestUtil.Open(@"Model\Style\Test19927.docx");
            Paragraph para = doc.FirstSection.Body.Tables[0].FirstRow.FirstCell.FirstParagraph;

            CustomCompatibilitySetting setting =
                doc.CompatibilityOptions.CustomCompatibilitySettings["overrideTableStyleFontSizeAndJustification"];

            Assert.That(setting, Is.Null);
            CheckExpandedSizeVal(para, FontAttr.Size, 40);

            TestUtil.ExecuteValidator(doc, SaveFormat.Markdown);

            setting = doc.CompatibilityOptions.CustomCompatibilitySettings["overrideTableStyleFontSizeAndJustification"];
            Assert.That(setting, IsNot.Null());
            Assert.That(setting.Value, Is.EqualTo("0"));

            // Verify, that the resulting font size remained the same.
            CheckExpandedSizeVal(para, FontAttr.Size, 40);
        }

        /// <summary>
        /// Relates with WORDSNET-16374
        /// Checks cases with sizes of the "rtl" content, while expanding properties of runs.
        /// </summary>
        [Test]
        public void TestJira16374Bidi()
        {
            Document doc = TestUtil.Open(@"Model\Style\TestJira16374Bidi.docx");
            TableCollection tables = doc.FirstSection.Body.Tables;

            CheckExpandedSizeVal(tables[0].FirstRow.FirstCell.FirstParagraph, FontAttr.SizeBi, 26);
            CheckExpandedSizeVal(tables[1].FirstRow.FirstCell.FirstParagraph, FontAttr.SizeBi, 24);
            CheckExpandedSizeVal(tables[2].FirstRow.FirstCell.FirstParagraph, FontAttr.SizeBi, 22);
            CheckExpandedSizeVal(tables[3].FirstRow.FirstCell.FirstParagraph, FontAttr.SizeBi, 24);
            CheckExpandedSizeVal(tables[4].FirstRow.FirstCell.FirstParagraph, FontAttr.SizeBi, 18);
            CheckExpandedSizeVal(tables[5].FirstRow.FirstCell.FirstParagraph, FontAttr.SizeBi, 24);
            CheckExpandedSizeVal(tables[6].FirstRow.FirstCell.FirstParagraph, FontAttr.SizeBi, 40);
        }





        /// <summary>
        /// WORDSNET-18557 Shading.BackgroundPatternColor returns incorrect cell's background color
        /// We need to update color from theme on style import.
        /// </summary>
        [Test]
        public void Test18557()
        {
            Document doc = TestUtil.Open(@"Model\Style\Test18557.docx");

            Table table = (Table)doc.GetChildNodes(NodeType.Table, true)[0];
            table.Style = doc.Styles[StyleIdentifier.DarkListAccent3];

            Cell cell = (Cell)doc.GetChildNodes(NodeType.Cell, true)[8];

            Assert.That(cell.CellFormat.Shading.BackgroundPatternColor.G, Is.EqualTo(0x7B));
            Assert.That(cell.CellFormat.Shading.BackgroundPatternColor.R, Is.EqualTo(0x7B));
            Assert.That(cell.CellFormat.Shading.BackgroundPatternColor.B, Is.EqualTo(0x7B));
        }





        /// <summary>
        /// Tests how document with huge set of styles exceeding Word limit are written to Word native document formats.
        /// </summary>
        [TestCase(SaveFormat.Docx, WarningSource.Docx, 4082)]
        public void TestStyleLimit(SaveFormat sf, WarningSource warningSource, int expectedCount)
        {
            Document doc = CreateStyleLimitDocument();

            WarningInfoCollection warnings = new WarningInfoCollection();
            doc.WarningCallback = warnings;

            // Check some edge styles before save.
            Assert.That(doc.Styles.Count, Is.EqualTo(4204));
            CheckStyle(doc.Styles, 4015, 4025, 4115, 4030);
            CheckStyle(doc.Styles, 4079, 4089, 4179, 4094);
            CheckStyle(doc.Styles, 4084, 4094, 4184, 4099);

            UnifiedScenario uc = TestUtil.GetUnifiedScenario(LoadFormat.Docx, sf);

            // WML writer has unrelated roundtrip issue, exclude from gold check.
            if (sf == SaveFormat.WordML)
                uc |= UnifiedScenario.NoGold;

            doc = TestUtil.SaveOpen(doc, @"Model\Style\TestStyleLimit", uc);

            // Check the same style set after roundtrip.
            Assert.That(doc.Styles.Count, Is.EqualTo(expectedCount));

            // "Followed by" feature is not supported in ODT format.
            CheckStyle(doc.Styles, 4015, 4025, StyleIndex.Nil, (sf == SaveFormat.Odt) ? 4015 : 4030);
            CheckStyle(doc.Styles, 4079, 4089, StyleIndex.Nil, 4079);
            CheckStyle(doc.Styles, 4084, StyleIndex.Nil, StyleIndex.Nil, 4084);

            Assert.That(TestUtil.ContainsWarning(warnings, WarningType.DataLoss, warningSource,
                "Style 'MyParagraphStyle 4094' has style index unsupported by target format."), Is.True);

            Assert.That(TestUtil.ContainsWarning(warnings, WarningType.DataLoss, warningSource,
                "Style 'MyLinkedStyle 4115' has style index unsupported by target format."), Is.True);
        }


        /// <summary>
        /// WORDSNET-21785 Remove styles from Style Gallery.
        /// Added public method ClearQuickStyleGallery().
        /// </summary>
        [Test]
        public void Test21785()
        {
            Document doc = new Document();
            doc.Styles.ClearQuickStyleGallery();
            foreach (Style style in doc.Styles)
                Assert.That(style.IsQuickStyle, Is.False);

            for (int i = 0; i < doc.Styles.LatentStyles.Count; i++)
            {
                LatentStyle lse = doc.Styles.LatentStyles[i];
                Assert.That(lse.QuickStyle, Is.False);
            }

            // Gold check, check that Quick Style Gallery is empty if modified.
            TestUtil.SaveOpen(doc, @"Model\Style\Test21785.docx", SaveOptions.CreateSaveOptions(SaveFormat.Docx));
        }






        /// <summary>
        /// WORDSNET-24686 Make Style.AutomaticallyUpdate property public.
        /// Tests the public API of AutomaticallyUpdate in Style.
        /// </summary>
        [Test]
        public void Test24686()
        {
            Document doc = new Document();
            Style style = doc.Styles.Add(StyleType.Paragraph, "Redefined");
            style.BaseStyleName = "Normal";

            Assert.That(style.AutomaticallyUpdate, Is.False);
            style.AutomaticallyUpdate = true;
            Assert.That(style.AutomaticallyUpdate, Is.True);

            doc = TestUtil.SaveOpen(doc, @"Model\Style\Test24686", UnifiedScenario.Docx2DocxNoGold);

            style = doc.Styles.GetByName("Redefined", false);
            Assert.That(style.AutomaticallyUpdate, Is.True);
        }


        /// <summary>
        /// WORDSNET-23972 Provide an ability to get/set styles priority.
        /// Checks a new public API.
        /// </summary>
        [Test]
        public void Test23972()
        {
            Document doc = TestUtil.Open(@"Model\Style\Test23972.docx");
            Style styleTitle = doc.Styles[StyleIdentifier.Subtitle];
            Assert.That(styleTitle.Priority, Is.EqualTo(11));

            styleTitle.Priority = 9;
            doc = TestUtil.SaveOpen(doc, @"Model\Style\Test23972.docx");
            styleTitle = doc.Styles[StyleIdentifier.Subtitle];
            Assert.That(styleTitle.Priority, Is.EqualTo(9));
        }

        /// <summary>
        /// WORDSNET-23973 Provide an ability to show/hide styles.
        /// Checks a new public API.
        /// </summary>
        [Test]
        public void Test23973()
        {
            Document doc = TestUtil.Open(@"Model\Style\Test23973.docx");
            Style styleTitle = doc.Styles[StyleIdentifier.Title];
            Assert.That(styleTitle.SemiHidden, Is.EqualTo(false));
            Assert.That(styleTitle.UnhideWhenUsed, Is.EqualTo(false));

            styleTitle.SemiHidden = true;
            styleTitle.UnhideWhenUsed = true;
            doc = TestUtil.SaveOpen(doc, @"Model\Style\Test23973.docx");
            styleTitle = doc.Styles[StyleIdentifier.Title];
            Assert.That(styleTitle.SemiHidden, Is.EqualTo(true));
            Assert.That(styleTitle.UnhideWhenUsed, Is.EqualTo(true));
        }

        /// <summary>
        /// WORDSNET-24691 Allow setting LinkedStyleName property.
        /// Checks a new public API.
        /// </summary>
        [Test]
        public void Test24691()
        {
            const string fileName = @"Model\Style\Test24691";
            Document doc = TestUtil.Open(fileName, LoadFormat.Docx);
            // Checking the linked style values set via MS Word VBA.
            Style styleHeading1 = doc.Styles[StyleIdentifier.Heading1];
            Style styleHeading1Char1 = doc.Styles["Heading 1 Char"];
            Style styleHeading1Char2 = doc.Styles["Heading 1 Char1"];
            Assert.That(styleHeading1.LinkedStyleName, Is.EqualTo("Heading 1 Char"));
            Assert.That(styleHeading1Char1.LinkedStyleName, Is.EqualTo("Heading 1"));
            Assert.That(styleHeading1Char2.LinkedStyleName, Is.EqualTo(string.Empty));

            styleHeading1.LinkedStyleName = "Heading 1 Char1";
            // Checking the linked style values set via the paragraph style.
            doc = TestUtil.SaveOpen(doc, fileName, UnifiedScenario.Docx2DocxNoGold);
            styleHeading1 = doc.Styles[StyleIdentifier.Heading1];
            styleHeading1Char1 = doc.Styles["Heading 1 Char"];
            styleHeading1Char2 = doc.Styles["Heading 1 Char1"];
            Assert.That(styleHeading1.LinkedStyleName, Is.EqualTo("Heading 1 Char1"));
            Assert.That(styleHeading1Char1.LinkedStyleName, Is.EqualTo(string.Empty));
            Assert.That(styleHeading1Char2.LinkedStyleName, Is.EqualTo("Heading 1"));

            styleHeading1Char1.LinkedStyleName = "Heading 1";
            // Checking the linked style values set via the character style.
            doc = TestUtil.SaveOpen(doc, fileName, UnifiedScenario.Docx2DocxNoGold);
            styleHeading1 = doc.Styles[StyleIdentifier.Heading1];
            styleHeading1Char1 = doc.Styles["Heading 1 Char"];
            styleHeading1Char2 = doc.Styles["Heading 1 Char1"];
            Assert.That(styleHeading1.LinkedStyleName, Is.EqualTo("Heading 1 Char"));
            Assert.That(styleHeading1Char1.LinkedStyleName, Is.EqualTo("Heading 1"));
            Assert.That(styleHeading1Char2.LinkedStyleName, Is.EqualTo(string.Empty));

            // The check of unlinking.
            styleHeading1.LinkedStyleName = string.Empty;
            Assert.That(styleHeading1.LinkedIstd, Is.EqualTo(StyleIndex.Nil));
            Assert.That(styleHeading1Char1.LinkedIstd, Is.EqualTo(StyleIndex.Nil));
            Assert.That(styleHeading1Char2.LinkedIstd, Is.EqualTo(StyleIndex.Nil));
        }

        /// <summary>
        /// Related to WORDSNET-24691
        /// Tests the case when LinkedStyleName is null.
        /// </summary>
        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void Test24691_Null()
        {
            Document doc = TestUtil.Open(@"Model\Style\Test24691.docx");
            doc.Styles[StyleIdentifier.Heading1].LinkedStyleName = null;
        }

        /// <summary>
        /// Related to WORDSNET-24691
        /// Tests the case when LinkedStyleName is assigned to a table style.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Only the paragraph and the character styles can be linked.")]
        public void Test24691_StyleType()
        {
            Document doc = TestUtil.Open(@"Model\Style\Test24691.docx");
            doc.Styles[StyleIdentifier.TableNormal].LinkedStyleName = "Heading 1 Char1";
        }

        /// <summary>
        /// Related to WORDSNET-24691
        /// Tests the case when a list style is assigned as LinkedStyleName.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Only the paragraph and the character styles can be linked.")]
        public void Test24691_LinkedStyleType()
        {
            Document doc = TestUtil.Open(@"Model\Style\Test24691.docx");
            doc.Styles[StyleIdentifier.Heading1].LinkedStyleName = doc.Styles[StyleIdentifier.List].Name;
        }

        /// <summary>
        /// Related to WORDSNET-24691
        /// Tests the case when DefaultParagraphFont is assigned as LinkedStyleName.
        /// </summary>
        [Test, ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Default paragraph font cannot be linked style.")]
        public void Test24691_DefaultParagraphFont()
        {
            Document doc = TestUtil.Open(@"Model\Style\Test24691.docx");
            doc.Styles[StyleIdentifier.Heading1].LinkedStyleName = doc.Styles[StyleIdentifier.DefaultParagraphFont].Name;
        }

        /// <summary>
        /// WORDSNET-28303 Because of a bug, Aspose.Words couldn't instantiate the built-in "Macro Text" by name.
        /// </summary>
        [Test]
        public void Test28303()
        {
            Document doc = new Document();
            Style style = doc.Styles["Macro Text"];

            Assert.That(style, IsNot.Null());
            Assert.That(style.BuiltIn, Is.True);
            Assert.That(style.StyleIdentifier, Is.EqualTo(StyleIdentifier.Macro));
        }

        /// <summary>
        /// Expands attributes of the first run for specified paragraph and checks calculated size attribute value.
        /// </summary>
        private static void CheckExpandedSizeVal(Paragraph para, int sizeKey, int sizeValue)
        {
            Style style = para.Document.Styles.GetByIstd(para.ParaPr.Istd, false);
            Assert.That(style.StyleIdentifier, Is.EqualTo(StyleIdentifier.Normal));
            Assert.That(style.RunPr[sizeKey], Is.EqualTo(24));

            RunPr dstRunPr = para.FirstRun.GetExpandedRunPr(RunPrExpandFlags.Normal);
            Assert.That(dstRunPr[sizeKey], Is.EqualTo(sizeValue));
        }

        /// <summary>
        /// Creates Document object with many styles exceeding Word style limit.
        /// </summary>
        /// <returns></returns>
        private static Document CreateStyleLimitDocument()
        {
            Document doc = new Document();
            DocumentBuilder builder = new DocumentBuilder(doc);

            builder.Font.Style = doc.Styles[StyleIdentifier.DefaultParagraphFont];
            builder.ParagraphFormat.Style = doc.Styles[StyleIdentifier.Normal];

            const int istdBase = 15;
            for (int i = 0; i < 4000; i++)
            {
                Style style = doc.Styles.Add(StyleType.Character, string.Format("MyCharStyle {0:D4}", istdBase + i));

                style.RunPr.SetAttr(FontAttr.Color, DrColor.FromArgb(i % 255, i % 255, i % 255));
                builder.Font.Style = style;
                builder.Write((i % 10).ToString());
            }

            builder.Writeln();

            for (int i = 0; i < 100; i++)
            {
                Style style = doc.Styles.Add(StyleType.Paragraph, string.Format("MyParagraphStyle {0:D4}", istdBase + 4000 + i));
                style.ParaPr.SetAttr(ParaAttr.LeftIndent, i * 10);


                builder.ParagraphFormat.Style = style;
                builder.Writeln(string.Format("Line {0}", i));

                style.BasedOnIstd = (i < 90 ? style.Istd + 10 : StyleIndex.Nil);
                style.NextIstd = (i < 85 ? style.Istd + 15 : style.Istd);
            }

            // Make few character styles linked to paragraph styles.
            for (int i = 0; i < 100; i++)
            {
                Style style = doc.Styles.Add(StyleType.Character, string.Format("MyLinkedStyle {0:D4}", istdBase + 4000 + 100 + i));
                // Link to paragraph styles generated in above step.
                Style paraStyle = doc.Styles.GetByName(string.Format("MyParagraphStyle {0:D4}", istdBase + 4000 + i), false);

                paraStyle.LinkedIstd = style.Istd;
                style.LinkedIstd = paraStyle.Istd;

                style.RunPr.SetAttr(FontAttr.Color, DrColor.FromArgb(i % 255, 0, i % 255));
                builder.Font.Style = style;
                builder.Write((i % 10).ToString());
            }

            return doc;
        }

        /// <summary>
        /// Checks BasedOn, Linked and Next Istd of given style.
        /// </summary>
        private static void CheckStyle(StyleCollection styles, int istd, int istdBasedOn, int istdLinked, int istdNext)
        {
            Style style = styles.GetByIstd(istd, false);
            Assert.That(style.BasedOnIstd, Is.EqualTo(istdBasedOn));
            Assert.That(style.LinkedIstd, Is.EqualTo(istdLinked));
            Assert.That(style.NextIstd, Is.EqualTo(istdNext));
        }
    }
}
