// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 03/04/2015 by Roman Korchagin

using System;
using Aspose.Words.Notes;
using Aspose.Words.Saving;
using Aspose.Words.Settings;
using NUnit.Framework;

namespace Aspose.Words.Tests.Unified
{
    /// <summary>
    /// Unified for the document properties (the DOP structure).
    /// </summary>
    [TestFixture]
    public class TestDop : UnifiedTestsBase
    {
        [Test]
        public void TestNew()
        {
            Document doc = new Document();
            DocPr pr = doc.DocPr;

            // RK Can add other properties to test.

            Assert.That(pr.CompatibilityOptions.UICompat97To2003, Is.EqualTo(false));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestDefault(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Dop\TestDefault", lf, sf);
            DocPr pr = doc.DocPr;

            Assert.That(pr.PrintFormsData, Is.EqualTo(false));
            Assert.That(pr.TrackRevisions, Is.EqualTo(false));
            Assert.That(pr.DefaultTabStop, Is.EqualTo(1440));
            Assert.That(pr.EvenAndOddHeaders, Is.EqualTo(true));
            Assert.That(pr.ShowOutlineLevels, Is.EqualTo(9));

            // RK These seem to be DOC properties only.
            if (sf == SaveFormat.Doc)
            {
                Assert.That(pr.ShowHidden, Is.EqualTo(true));
                Assert.That(pr.ShowFieldResults, Is.EqualTo(true));
            }

            Assert.That(pr.DocumentProtection.Edit, Is.EqualTo(ProtectionType.NoProtection));
            Assert.That(pr.DocumentProtection.Enforcement, Is.EqualTo(false));
            Assert.That(pr.DocumentProtection.Formatting, Is.EqualTo(false));
            Assert.That(pr.DocumentProtection.SelectFormFieldsOnly, Is.EqualTo(false));

            FootnoteOptions footnoteOptions = doc.FootnoteOptions;
            Assert.That(footnoteOptions.Position, Is.EqualTo(FootnotePosition.BottomOfPage));
            Assert.That(footnoteOptions.NumberStyle, Is.EqualTo(NumberStyle.Arabic));
            Assert.That(footnoteOptions.StartNumber, Is.EqualTo(1));
            Assert.That(footnoteOptions.RestartRule, Is.EqualTo(FootnoteNumberingRule.Continuous));

            EndnoteOptions endnoteOptions = doc.EndnoteOptions;
            Assert.That(endnoteOptions.Position, Is.EqualTo(EndnotePosition.EndOfDocument));
            Assert.That(endnoteOptions.NumberStyle, Is.EqualTo(NumberStyle.LowercaseRoman));
            Assert.That(endnoteOptions.StartNumber, Is.EqualTo(1));
            Assert.That(endnoteOptions.RestartRule, Is.EqualTo(FootnoteNumberingRule.Continuous));

            Assert.That(doc.ViewOptions.DisplayBackgroundShape, Is.EqualTo(false));
            Assert.That(doc.ViewOptions.DoNotDisplayPageBoundaries, Is.EqualTo(false));

            Assert.That(pr.StyleLockQuickFormatSet, Is.EqualTo(false));
            Assert.That(pr.StyleLockTheme, Is.EqualTo(false));
            Assert.That(pr.StylePaneFormatFilterSettings.Data, Is.EqualTo(0x3f01));
            Assert.That(pr.StylePaneSortMethod, Is.EqualTo(StylePaneSortMethod.Default));
            Assert.That(pr.CompatibilityOptions.UICompat97To2003, Is.EqualTo(true));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestRevMarks(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Dop\TestRevMarks", lf, sf);
            DocPr pr = doc.DocPr;

            Assert.That(pr.TrackRevisions, Is.EqualTo(true));
            Assert.That(pr.ShowMarkup, Is.EqualTo(true));
            Assert.That(pr.ShowAnnotations, Is.EqualTo(true));
            Assert.That(pr.ShowFormatting, Is.EqualTo(true));
            Assert.That(pr.ShowInsertionsDeletions, Is.EqualTo(true));
            Assert.That(pr.ShowRevisions, Is.EqualTo(true));

            // This seems to be DOC only property.
            if (sf == SaveFormat.Doc)
                Assert.That(pr.PrintRevisions, Is.EqualTo(true));

            doc.TrackRevisions = false;

            doc = TestUtil.SaveOpen(doc, @"Model\Dop\TestRevMarks Off", lf, sf);
            Assert.That(doc.TrackRevisions, Is.EqualTo(false));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestShowGrammarErrors(LoadFormat lf, SaveFormat sf)
        {
            // RTF format hasn't got 'HideGrammaticalErrors' option.
            if (sf != SaveFormat.Rtf)
            {
                Document doc = TestUtil.OpenSaveOpen(@"Model\Dop\TestShowGrammarErrors", lf, sf);
                Assert.That(doc.ShowGrammaticalErrors, Is.True);
                Assert.That(doc.DocPr.HideGrammaticalErrors, Is.False);

                doc.ShowGrammaticalErrors = false;
                doc = TestUtil.SaveOpen(doc, @"Model\Dop\TestShowGrammarErrors Off", lf, sf);
                Assert.That(doc.ShowGrammaticalErrors, Is.False);
                Assert.That(doc.DocPr.HideGrammaticalErrors, Is.True);
            }
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestShowSpellingErrors(LoadFormat lf, SaveFormat sf)
        {
            // RTF format hasn't got 'HideSpellingErrors' option.
            if (sf != SaveFormat.Rtf)
            {
                Document doc = TestUtil.OpenSaveOpen(@"Model\Dop\TestShowSpellingErrors", lf, sf);
                Assert.That(doc.ShowSpellingErrors, Is.True);
                Assert.That(doc.DocPr.HideSpellingErrors, Is.False);

                doc.ShowSpellingErrors = false;
                doc = TestUtil.SaveOpen(doc, @"Model\Dop\TestShowSpellingErrors Off", lf, sf);
                Assert.That(doc.ShowSpellingErrors, Is.False);
                Assert.That(doc.DocPr.HideSpellingErrors, Is.True);
            }
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestProtect(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Dop\TestProtectRevisions", lf, sf);

            DocPr pr = doc.DocPr;
            Assert.That(pr.DocumentProtection.Edit, Is.EqualTo(ProtectionType.AllowOnlyRevisions));
            Assert.That(pr.DocumentProtection.Enforcement, Is.EqualTo(true));

            doc = TestUtil.OpenSaveOpen(@"Model\Dop\TestProtectComments", lf, sf);
            pr = doc.DocPr;
            Assert.That(pr.DocumentProtection.Edit, Is.EqualTo(ProtectionType.AllowOnlyComments));
            Assert.That(pr.DocumentProtection.Enforcement, Is.EqualTo(true));

            doc = TestUtil.OpenSaveOpen(@"Model\Dop\TestProtectFormFields", lf, sf);
            pr = doc.DocPr;
            Assert.That(pr.DocumentProtection.Edit, Is.EqualTo(ProtectionType.AllowOnlyFormFields));
            Assert.That(pr.DocumentProtection.Enforcement, Is.EqualTo(true));

            // Seems to be a DOC property only.
            if (sf == SaveFormat.Doc)
                Assert.That(pr.DocumentProtection.SelectFormFieldsOnly, Is.EqualTo(true));

            doc = TestUtil.OpenSaveOpen(@"Model\Dop\TestProtectReadOnly", lf, sf);
            pr = doc.DocPr;
            Assert.That(pr.DocumentProtection.Edit, Is.EqualTo(ProtectionType.ReadOnly));
            Assert.That(pr.DocumentProtection.Enforcement, Is.EqualTo(true));

            doc = TestUtil.OpenSaveOpen(@"Model\Dop\TestProtectionPassword", lf, sf);
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestZoom(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Dop\TestZoom75", lf, sf);
            Assert.That(doc.ViewOptions.ZoomPercent, Is.EqualTo(75));
            Assert.That(doc.ViewOptions.ZoomType, Is.EqualTo(ZoomType.None));

            doc = TestUtil.OpenSaveOpen(@"Model\Dop\TestZoomPageWidth", lf, sf);
            Assert.That(doc.ViewOptions.ZoomType, Is.EqualTo(ZoomType.PageWidth));

            doc = TestUtil.OpenSaveOpen(@"Model\Dop\TestZoomTextWidth", lf, sf);
            Assert.That(doc.ViewOptions.ZoomType, Is.EqualTo(ZoomType.TextFit));

            doc = TestUtil.OpenSaveOpen(@"Model\Dop\TestZoomWholePage", lf, sf);
            Assert.That(doc.ViewOptions.ZoomType, Is.EqualTo(ZoomType.FullPage));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestView(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Dop\TestViewNormal", lf, sf);
            Assert.That(doc.ViewOptions.ViewType, Is.EqualTo(ViewType.Normal));

            doc = TestUtil.OpenSaveOpen(@"Model\Dop\TestViewPageLayout", lf, sf);
            Assert.That(doc.ViewOptions.ViewType, Is.EqualTo(ViewType.PageLayout));

            doc = TestUtil.OpenSaveOpen(@"Model\Dop\TestViewOutline", lf, sf);
            Assert.That(doc.ViewOptions.ViewType, Is.EqualTo(ViewType.Outline));

            doc = TestUtil.OpenSaveOpen(@"Model\Dop\TestViewWeb", lf, sf);
            Assert.That(doc.ViewOptions.ViewType, Is.EqualTo(ViewType.Web));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestViewOptions(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Dop\TestViewOptions", lf, sf);

            Assert.That(doc.ViewOptions.DisplayBackgroundShape, Is.EqualTo(true));
            Assert.That(doc.ViewOptions.DoNotDisplayPageBoundaries, Is.EqualTo(true));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestFootnoteEndnote(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Dop\TestFootnoteEndnote", lf, sf);

            Assert.That(doc.FootnoteOptions.Position, Is.EqualTo(FootnotePosition.BeneathText));
            Assert.That(doc.FootnoteOptions.NumberStyle, Is.EqualTo(NumberStyle.LowercaseLetter));
            Assert.That(doc.FootnoteOptions.StartNumber, Is.EqualTo(2));
            Assert.That(doc.FootnoteOptions.RestartRule, Is.EqualTo(FootnoteNumberingRule.Continuous));

            Assert.That(doc.EndnoteOptions.Position, Is.EqualTo(EndnotePosition.EndOfSection));
            Assert.That(doc.EndnoteOptions.NumberStyle, Is.EqualTo(NumberStyle.UppercaseRoman));
            Assert.That(doc.EndnoteOptions.StartNumber, Is.EqualTo(1));
            Assert.That(doc.EndnoteOptions.RestartRule, Is.EqualTo(FootnoteNumberingRule.RestartSection));
        }

        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestCompatibilityOptions(LoadFormat lf, SaveFormat sf)
        {
            Document doc = TestUtil.OpenSaveOpen(@"Model\Dop\TestCompatibilityOptions", lf, sf);
            DoTestCompatibilityOptions(doc.DocPr.CompatibilityOptions);
        }

        private static void DoTestCompatibilityOptions(CompatibilityOptions co)
        {
            Assert.That(co.SpaceForUL, Is.EqualTo(false));
            Assert.That(co.AdjustLineHeightInTable, Is.EqualTo(false));
            Assert.That(co.AlignTablesRowByRow, Is.EqualTo(true));
            Assert.That(co.LayoutTableRowsApart, Is.EqualTo(true));
            Assert.That(co.GrowAutofit, Is.EqualTo(true));
            Assert.That(co.AutoSpaceLikeWord95, Is.EqualTo(false));
            Assert.That(co.BalanceSingleByteDoubleByteWidth, Is.EqualTo(true));
            Assert.That(co.UseSingleBorderforContiguousCells, Is.EqualTo(false));
            Assert.That(co.DoNotLeaveBackslashAlone, Is.EqualTo(false));
            Assert.That(co.WPJustification, Is.EqualTo(false));
            Assert.That(co.NoTabHangInd, Is.EqualTo(false));
            Assert.That(co.NoSpaceRaiseLower, Is.EqualTo(false));
            Assert.That(co.NoLeading, Is.EqualTo(false));
            Assert.That(co.DoNotWrapTextWithPunct, Is.EqualTo(true));
            Assert.That(co.NoColumnBalance, Is.EqualTo(false));
            Assert.That(co.TransparentMetafiles, Is.EqualTo(false));
            Assert.That(co.NoExtraLineSpacing, Is.EqualTo(false));
            Assert.That(co.DoNotExpandShiftReturn, Is.EqualTo(false));
            Assert.That(co.DoNotUseEastAsianBreakRules, Is.EqualTo(true));
            Assert.That(co.DoNotUseHTMLParagraphAutoSpacing, Is.EqualTo(true));
            Assert.That(co.UlTrailSpace, Is.EqualTo(false));
            Assert.That(co.SpacingInWholePoints, Is.EqualTo(false));
            Assert.That(co.ForgetLastTabAlignment, Is.EqualTo(true));
            Assert.That(co.ShapeLayoutLikeWW8, Is.EqualTo(true));
            Assert.That(co.FootnoteLayoutLikeWW8, Is.EqualTo(true));
            Assert.That(co.LayoutRawTableWidth, Is.EqualTo(true));
            Assert.That(co.LineWrapLikeWord6, Is.EqualTo(false));
            Assert.That(co.PrintBodyTextBeforeHeader, Is.EqualTo(false));
            Assert.That(co.PrintColBlack, Is.EqualTo(false));
            Assert.That(co.SelectFldWithFirstOrLastChar, Is.EqualTo(true));
            Assert.That(co.WPSpaceWidth, Is.EqualTo(false));
            Assert.That(co.ShowBreaksInFrames, Is.EqualTo(false));
            Assert.That(co.SubFontBySize, Is.EqualTo(false));
            Assert.That(co.SuppressBottomSpacing, Is.EqualTo(false));
            Assert.That(co.SuppressTopSpacing, Is.EqualTo(false));
            Assert.That(co.SuppressSpacingAtTopOfPage, Is.EqualTo(false));
            Assert.That(co.SuppressSpBfAfterPgBrk, Is.EqualTo(false));
            Assert.That(co.SwapBordersFacingPgs, Is.EqualTo(true));
            Assert.That(co.ConvMailMergeEsc, Is.EqualTo(false));
            Assert.That(co.TruncateFontHeightsLikeWP6, Is.EqualTo(false));
            Assert.That(co.MWSmallCaps, Is.EqualTo(true));
            Assert.That(co.ApplyBreakingRules, Is.EqualTo(false));
            Assert.That(co.UsePrinterMetrics, Is.EqualTo(false));
            Assert.That(co.UseWord2002TableStyleRules, Is.EqualTo(true));
            Assert.That(co.DoNotSuppressParagraphBorders, Is.EqualTo(false));
            Assert.That(co.UseWord97LineBreakRules, Is.EqualTo(true));
            Assert.That(co.WrapTrailSpaces, Is.EqualTo(false));
        }



        /// <summary>
        /// WORDSNET-9847 Track Formatting option is not preserved during open/save a document
        /// </summary>
        /// <remarks>
        /// AM. There is bad issue with binary DOC format. I found that Word ignores DOP2007 i.e DoNotTrackFormatting, DoNotTrackMoves,
        /// StylePaneSort if nFib value is less than Word2007. I adopted DocPrFiler to this logic.
        /// Currently we write Word2003 nFib value for our files exported in binary DOC format.
        ///
        /// Sometimes we need to switch to Word2007 but lets wait till customer complains.
        /// </remarks>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestJira9847(LoadFormat lf, SaveFormat sf)
        {
            bool isWordML = (lf == LoadFormat.WordML) || (sf == SaveFormat.WordML);
            bool isDoc = (lf == LoadFormat.Doc) || (sf == SaveFormat.Doc);
            UnifiedScenario scenario = TestUtil.GetUnifiedScenario(lf, sf) | UnifiedScenario.NoGold;

            if (isWordML)
            {
                // This option is missed in WordML.
                return;
            }

            // Test option is switched on.
            Document doc = TestUtil.Open(@"Model\Dop\TestJira9847A", lf);
            Assert.That(doc.DocPr.DoNotTrackFormatting, Is.True);

            if (!isDoc)
            {
                doc = TestUtil.SaveOpen(doc, @"Model\Dop\TestJira9847A", scenario);
                Assert.That(doc.DocPr.DoNotTrackFormatting, Is.True);
            }

            // Test option is switched off.
            doc = TestUtil.Open(@"Model\Dop\TestJira9847B", lf);
            Assert.That(doc.DocPr.DoNotTrackFormatting, Is.False);

            if (!isDoc)
            {
                doc = TestUtil.SaveOpen(doc, @"Model\Dop\TestJira9847B", scenario);
                Assert.That(doc.DocPr.DoNotTrackFormatting, Is.False);
            }

            // Test non default style pane sort method.
            doc = TestUtil.Open(@"Model\Dop\TestJira9847C", lf);
            Assert.That(doc.DocPr.StylePaneSortMethod, Is.EqualTo(StylePaneSortMethod.BasedOn));

            if (!isDoc)
            {
                doc = TestUtil.SaveOpen(doc, @"Model\Dop\TestJira9847C", scenario);
                Assert.That(doc.DocPr.StylePaneSortMethod, Is.EqualTo(StylePaneSortMethod.BasedOn));
            }
        }

        /// <summary>
        /// WORDSNET-10473 Implement load/save w:hideSpellingErrors into DOM for relevant formats.
        /// Here is the test for all formats, except the 'rtf', which is tested in TestUnifiedWarnings.TestJira10473().
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestJira10473(LoadFormat lf, SaveFormat sf)
        {
            UnifiedScenario scenario = TestUtil.GetUnifiedScenario(lf, sf) | UnifiedScenario.NoGold;

            Document doc = new Document();

            // By default MSW doesn't hide spelling errors.
            Assert.That(doc.DocPr.HideSpellingErrors, Is.False);

            // Checks disabled property, after resaving remained disabled.
            doc = TestUtil.SaveOpen(doc, @"Model\Dop\TestJira10473", scenario);
            Assert.That(doc.DocPr.HideSpellingErrors, Is.False);

            // Checks enabled property, after resaving remained enabled.
            if (sf != SaveFormat.Rtf)
            {
                doc.DocPr.HideSpellingErrors = true;
                doc = TestUtil.SaveOpen(doc, @"Model\Dop\TestJira10473", scenario);
                Assert.That(doc.DocPr.HideSpellingErrors, Is.True);
            }
        }

        /// <summary>
        /// WORDSNET-10474 Implement load/save w:hideGrammaticalErrors into DOM for relevant formats.
        /// Here is the test for all formats, except the 'rtf', which is tested in TestUnifiedWarnings.TestJira10474().
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestJira10474(LoadFormat lf, SaveFormat sf)
        {
            UnifiedScenario scenario = TestUtil.GetUnifiedScenario(lf, sf) | UnifiedScenario.NoGold;

            Document doc = new Document();

            // By default MSW doesn't hide grammatical errors.
            Assert.That(doc.DocPr.HideGrammaticalErrors, Is.False);

            // Checks disabled property, after resaving remained disabled.
            doc = TestUtil.SaveOpen(doc, @"Model\Dop\TestJira10474", scenario);
            Assert.That(doc.DocPr.HideGrammaticalErrors, Is.False);

            // Checks enabled property, after resaving remained enabled.
            if (sf != SaveFormat.Rtf)
            {
                doc.DocPr.HideGrammaticalErrors = true;
                doc = TestUtil.SaveOpen(doc, @"Model\Dop\TestJira10474", scenario);
                Assert.That(doc.DocPr.HideGrammaticalErrors, Is.True);
            }
        }

        /// <summary>
        /// Tests loading/saving <see cref="Document.HyphenationOptions"/> properties.
        /// </summary>
        // FOSS: only Docx survives — Doc/WordML/Rtf/Odt save were removed.
        [TestCase(SaveFormat.Docx, true, 5, 720, false)]
        public void TestHyphenationOptions(SaveFormat saveFormat, bool autoHyphenation,
            int consecutiveHyphenLimit, int hyphenationZone, bool hyphenateCaps)
        {
            Document doc = new Document();

            HyphenationOptions options = doc.HyphenationOptions;
            options.AutoHyphenation = autoHyphenation;
            options.ConsecutiveHyphenLimit = consecutiveHyphenLimit;
            options.HyphenationZone = hyphenationZone;
            options.HyphenateCaps = hyphenateCaps;

            Document outDoc = TestUtil.SaveOpen(doc, @"Model\Dop\TestHyphenationOptions",
                SaveOptions.CreateSaveOptions(saveFormat), false);

            options = outDoc.HyphenationOptions;
            Assert.That(options.AutoHyphenation, Is.EqualTo(autoHyphenation), "AutoHyphenation property value is wrong.");
            Assert.That(options.ConsecutiveHyphenLimit, Is.EqualTo(consecutiveHyphenLimit), "ConsecutiveHyphenLimit property value is wrong.");
            Assert.That(options.HyphenationZone, Is.EqualTo(hyphenationZone), "HyphenationZone property value is wrong.");
            Assert.That(options.HyphenateCaps, Is.EqualTo(hyphenateCaps), "HyphenateCaps property value is wrong.");
        }

        /// <summary>
        /// Tests that an exception is raised on setting wrong value to the
        /// <see cref="HyphenationOptions.ConsecutiveHyphenLimit"/> property.
        /// </summary>
        [Test]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void TestValidationOfConsecutiveHyphenLimit()
        {
            Document doc = new Document();
            doc.HyphenationOptions.ConsecutiveHyphenLimit = -1;
        }

        /// <summary>
        /// Tests that an exception is raised on setting wrong value to the
        /// <see cref="HyphenationOptions.HyphenationZone"/> property.
        /// </summary>
        [Test]
        [ExpectedException(typeof (ArgumentOutOfRangeException))]
        public void TestValidationOfHyphenationZone()
        {
            Document doc = new Document();
            doc.HyphenationOptions.HyphenationZone = 0;
        }

        // FOSS: TestJira13764 removed — RTF load/save is gone and its third input is a customer .rtf.
        // FOSS: TestJira13596B removed — it reads DisplayBackgroundShape via binary-.doc nFib/FIB behavior.

         private static void CheckProtection(Document doc, ProtectionType type, bool formatting, bool enforcement)
        {
            Assert.That(doc.DocumentProtection.Edit, Is.EqualTo(type));
            Assert.That(doc.DocumentProtection.Formatting, Is.EqualTo(formatting));
            Assert.That(doc.DocumentProtection.Enforcement, Is.EqualTo(enforcement));
        }

        /// <summary>
        /// WORDSNET-14109 Support of the following document settings: clickAndTypeStyle, defaultTableStyle,
        /// doNotAutoCompressPictures, updateFields, forceUpgrade, removeDateAndTime, w15:chartTrackingRefBased,
        /// w15:docId, w14:discardImageEditingData, w14:defaultImageDpi.
        /// </summary>
        /// <remarks>
        /// Not all settings are supported by DOC, RTF and WML formats now.
        /// </remarks>
        // FOSS: only the Docx2Docx scenario survives — Doc/Rtf/WordML load+save were removed.
        [TestCase(UnifiedScenario.Docx2Docx)]
        public void TestJira14109(UnifiedScenario scenario)
        {
            Document doc = TestUtil.SaveOpen(GenerateTestDocForJira14109(), @"Model\Dop\TestJira14109",
                scenario | UnifiedScenario.NoGold);

            Assert.That(doc.DocPr.ClickTypeParaStyleIstd, Is.EqualTo(StyleIndex.Heading1));
            Style style = doc.Styles.GetByName("MyTable", false);
            Assert.That(style, IsNot.Null());
            Assert.That(doc.DocPr.DefaultTableStyleIstd, Is.EqualTo(style.Istd));
            // Some options are not supported in some formats.
            if (scenario != UnifiedScenario.Rtf2Rtf && scenario != UnifiedScenario.Wml2Wml)
            {
                Assert.That(doc.DocPr.DoNotAutoCompressPictures, Is.EqualTo(true));
                Assert.That(doc.DocPr.RemoveDateAndTime, Is.EqualTo(true));

                if (scenario != UnifiedScenario.Doc2Doc)
                {
                    Assert.That(doc.DocPr.UpdateFields, Is.EqualTo(true));
                    Assert.That(doc.DocPr.ForceUpgrade, Is.EqualTo(true));
                    Assert.That(doc.DocPr.ChartTrackingRefBased, Is.EqualTo(true));
                    Assert.That(doc.DocPr.DocumentSetId, Is.EqualTo("{D3633DB6-ADDC-42C4-B64B-CF101F95ED34}"));
                    Assert.That(doc.DocPr.DiscardImageEditingData, Is.EqualTo(true));
                    Assert.That(doc.DocPr.DefaultImageDpi, Is.EqualTo(200));
                }
            }
        }

        /// <summary>
        /// Generates a test document with settings, which support is added on implementing WORDSNET-14109
        /// </summary>
        private static Document GenerateTestDocForJira14109()
        {
            Document doc = new Document();
            doc.ComplianceInfo = new OoxmlComplianceInfo();
            doc.ComplianceInfo.IsDocxExtensions = true;
            doc.CompatibilityOptions.DoNotUseIndentAsNumberingTabStop = false; // for DOC format to write as Word2007

            Assert.That(doc.DocPr.ClickTypeParaStyleIstd, Is.EqualTo(DocPr.ClickTypeParaStyleIstdDefault));
            doc.Styles.GetByIstd(StyleIndex.Heading1, true); // This adds the style into the style collection.
            doc.DocPr.ClickTypeParaStyleIstd = StyleIndex.Heading1;

            Assert.That(doc.DocPr.DefaultTableStyleIstd, Is.EqualTo(DocPr.DefaultTableStyleIstdDefault));
            Style style = doc.Styles.Add(StyleType.Paragraph, "MyTable");
            style.BaseStyleName = "Heading 4";
            doc.DocPr.DefaultTableStyleIstd = style.Istd;

            Assert.That(doc.DocPr.DoNotAutoCompressPictures, Is.False);
            doc.DocPr.DoNotAutoCompressPictures = true;

            Assert.That(doc.DocPr.RemoveDateAndTime, Is.False);
            doc.DocPr.RemoveDateAndTime = true;

            Assert.That(doc.DocPr.UpdateFields, Is.False);
            doc.DocPr.UpdateFields = true;

            Assert.That(doc.DocPr.ForceUpgrade, Is.False);
            doc.DocPr.ForceUpgrade = true;

            Assert.That(doc.DocPr.ChartTrackingRefBased, Is.False);
            doc.DocPr.ChartTrackingRefBased = true;

            Assert.That(doc.DocPr.DocumentSetId, Is.Null);
            doc.DocPr.DocumentSetId = "{D3633DB6-ADDC-42C4-B64B-CF101F95ED34}";

            Assert.That(doc.DocPr.DiscardImageEditingData, Is.False);
            doc.DocPr.DiscardImageEditingData = true;

            Assert.That(doc.DocPr.DefaultImageDpi, Is.EqualTo(0));
            doc.DocPr.DefaultImageDpi = 200;

            Assert.That(doc.DocPr.DocId, Is.Null);
            doc.DocPr.DocId = "62376D8B";

            return doc;
        }

        // FOSS: TestWarningIfSettingNotSupported removed — it only verified data-loss warnings when
        // downgrading to Doc/Rtf/WordML, all of which were removed. No surviving format exercises it.

        /// <summary>
        /// WORDSNET-21349 The w14:docId has been removed after re-saving.
        /// The new DocId document property has been added.
        /// </summary>
        [Test]
        public void Test21349()
        {
            Document doc = TestUtil.Open(@"Model\Dop\Test21349.docx");
            OoxmlSaveOptions options = new OoxmlSaveOptions(SaveFormat.Docx);
            options.SetTestMode();
            options.WriteW14DocId = true;
            doc = TestUtil.SaveOpen(doc, @"Model\Dop\Test21349.docx", options, false);

            Assert.That(doc.DocPr.DocId, Is.EqualTo("62376D8B"));
        }

        /// <summary>
        /// Test that RemovePersonalInformation flag is correctly imported and exported.
        /// </summary>
        [Test]
        [TestCaseSource("DefaultTestScenarios")]
        public void UnifiedTestJira16914(LoadFormat lf, SaveFormat sf)
        {
            UnifiedScenario scenario = TestUtil.GetUnifiedScenario(lf, sf) | UnifiedScenario.NoGold;

            // Set option.
            Document doc = TestUtil.Open(@"Model\Dop\TestJira16914 false", lf);
            Assert.That(doc.RemovePersonalInformation, Is.False);

            doc.RemovePersonalInformation = true;

            doc = TestUtil.SaveOpen(doc, @"Model\Dop\TestJira16914", scenario);
            Assert.That(doc.RemovePersonalInformation, Is.True);

            // Remove option.
            doc = TestUtil.Open(@"Model\Dop\TestJira16914 true", lf);
            Assert.That(doc.RemovePersonalInformation, Is.True);

            doc.RemovePersonalInformation = false;

            doc = TestUtil.SaveOpen(doc, @"Model\Dop\TestJira16914", scenario);
            Assert.That(doc.RemovePersonalInformation, Is.False);
        }

    }
}
