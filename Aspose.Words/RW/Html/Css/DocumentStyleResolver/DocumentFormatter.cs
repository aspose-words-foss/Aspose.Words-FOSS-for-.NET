// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/12/2016 by Victor Chebotok

using Aspose.Bidi;
using Aspose.Words.Drawing;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Revisions;
using Aspose.Words.RW.HtmlCommon;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Applies CSS formatting to nodes of the document model.
    /// </summary>
    internal class DocumentFormatter : ITableFormatter
    {
        /// <summary>
        /// Applies calculated CSS styles to a model table.
        /// </summary>
        /// <param name="table">A model table.</param>
        /// <param name="topCaptionCount">Number of caption rows at the top of the table.</param>
        /// <param name="bottomCaptionCount">Number of caption rows at the bottom of the table.</param>
        /// <param name="appliedLeftBorder">Left CSS border of the table.</param>
        /// <param name="appliedRightBorder">Right CSS border of the table.</param>
        /// <remarks>Table captions are imported as table rows. Top captions become leading table rows and bottom captions
        /// become trailing rows. To distinguish 'normal' table rows from caption rows we need to know how many top and bottom
        /// caption rows the table contains.</remarks>
        public void ToTable(
            Table table,
            int topCaptionCount,
            int bottomCaptionCount,
            CssBorder appliedLeftBorder,
            CssBorder appliedRightBorder)
        {
            if (mCssStyleTracker.IsEmpty)
                return;

            Style style = mStyleFormatter.GetModelStyle(StyleType.Table);
            if (style != null)
            {
                foreach (Row row in table.Rows)
                    row.TablePr.SetAttr(TableAttr.Istd, style.Istd);
            }

            // WORDSNET-12135 Table Style does not import into output document while using InsertHTML.
            // In this case table uses an already existed document style, so apply this style to the table completely.
            if ((style != null) && !style.BuiltIn)
            {
                table.StyleOptions = TableStyleOptions.FirstRow | TableStyleOptions.FirstColumn | TableStyleOptions.RowBands |
                    TableStyleOptions.LastColumn | TableStyleOptions.LastRow;
            }

            HtmlTableFormater.ToTable(
                table,
                topCaptionCount,
                bottomCaptionCount,
                appliedLeftBorder,
                appliedRightBorder,
                mCssStyleTracker);
        }

        /// <summary>
        /// Creates an instance of this class that uses MS Word formatting rules.
        /// </summary>
        internal DocumentFormatter(
            Document document,
            CssStyleTracker styleTracker,
            bool isLoadingHtmlAltChunk)
        {
            Debug.Assert(document != null);
            Debug.Assert(styleTracker != null);

            mDocument = document;
            mCssStyleTracker = styleTracker;
            mParagraphFormatter = new ParagraphFormatterWordRules();
            mFontFormatter = new FontFormatterWordRules(mDocument.FontProvider, isLoadingHtmlAltChunk);
            mStyleFormatter = new StyleFormatterWordRules(
                mDocument,
                mCssStyleTracker,
                mParagraphFormatter,
                mFontFormatter);
            mSectionFormatter = new SectionFormatterWordRules(
                mDocument,
                mCssStyleTracker);
            mRemoveDuplicateDirectFormatting = true;
        }

        /// <summary>
        /// Creates an instance of this class that uses more HTML and CSS-conformant formatting rules.
        /// These rules differ from what MS Word uses.
        /// </summary>
        internal DocumentFormatter(
            Document document,
            int defaultParagraphIstd,
            int defaultFontIstd,
            CssUserAgentFormatting omittedUserAgentFormatting,
            CssStyleTracker styleTracker,
            bool useHtmlBlocks,
            CssFontFaceProvider cssFontFaceProvider)
        {
            Debug.Assert(document != null);
            Debug.Assert(styleTracker != null);

            mDocument = document;
            mCssStyleTracker = styleTracker;
            bool applyUserAgentStyles = (omittedUserAgentFormatting & CssUserAgentFormatting.Font) == 0;
            mFontFormatter = new FontFormatterHtmlRules(applyUserAgentStyles, cssFontFaceProvider, document.FontProvider);
            mParagraphFormatter = new ParagraphFormatterHtmlRules(useHtmlBlocks);
            mStyleFormatter = new StyleFormatterHtmlRules(
                mDocument,
                mCssStyleTracker,
                defaultParagraphIstd,
                defaultFontIstd,
                mFontFormatter,
                mParagraphFormatter);
            mSectionFormatter = new SectionFormatterHtmlRules(
                mDocument,
                mCssStyleTracker);
            mRemoveDuplicateDirectFormatting = false;
        }

        internal void UpdateDocumentWideFormatting()
        {
            mStyleFormatter.UpdatePredefinedStyles();
        }

        internal void UpdateDefaultSectionProperties(Section section)
        {
            mSectionFormatter.UpdateDefaultSectionProperties(section);
        }

        internal void PostUpdateDocumentWideFormatting()
        {
            mStyleFormatter.PostUpdateStyles();
        }

        internal void PushElement(IHtmlElementProvider element, bool updateCounters)
        {
            mCssStyleTracker.PushElement(element, updateCounters);
        }

        internal void PopElement()
        {
            mCssStyleTracker.PopElement();
        }

        internal bool SwitchToPart(HtmlElementPart part, bool updateCounters)
        {
            return mCssStyleTracker.SwitchToPart(part, updateCounters);
        }

        internal string GetGeneratedContent()
        {
            return mCssStyleTracker.GetGeneratedContent();
        }

        internal int GetListLevelCount()
        {
            return mCssStyleTracker.GetListLevelCount();
        }

        internal GeneratedContentListLabelInfo GetListLabelInfo(int skipListLevelsCount, int currentListLevelNumber)
        {
            return mCssStyleTracker.GetListLabelInfo(skipListLevelsCount, currentListLevelNumber);
        }

        internal bool IsBlockLevelElement()
        {
            return mCssStyleTracker.IsBlockLevelElement();
        }

        internal CssDeclarationCollection ElementDeclarations
        {
            get { return mCssStyleTracker.ElementDeclarations; }
        }

        internal IHtmlElementProvider CurrentElement
        {
            get { return mCssStyleTracker.CurrentElement; }
        }

        internal BidiLevelList GetActiveBidiLevels()
        {
            return mCssStyleTracker.GetActiveBidiLevels();
        }

        internal bool CurrentElementHasChildren
        {
            get { return mCssStyleTracker.CurrentElementHasChildren; }
        }

        internal void HandleSpanStart()
        {
            mStyleFormatter.HandleSpanStart();
        }

        internal void HandleSpanEnd()
        {
            mStyleFormatter.HandleSpanEnd();
        }

        internal void HandleParagraphStart()
        {
            mStyleFormatter.HandleParagraphStart();
        }

        internal bool ParentElementIsBlockLevel()
        {
            return mCssStyleTracker.ParentElementIsBlockLevel();
        }

        internal CssDisplayType ParentElementDisplayType()
        {
            return mCssStyleTracker.ParentElementDisplayType();
        }

        internal bool IsPreformatted()
        {
            return mCssStyleTracker.IsPreformatted();
        }

        internal bool IsPreformattedWithLine()
        {
            return mCssStyleTracker.IsPreformattedWithLine();
        }

        internal bool IsPreformattedWithWrap()
        {
            return mCssStyleTracker.IsPreformattedWithWrap();
        }

        internal bool IsBlockRtl()
        {
            return mCssStyleTracker.IsBlockRtl();
        }

        internal int GetCounterValue(string counterName)
        {
            return mCssStyleTracker.GetCounterValue(counterName);
        }

        /// <summary>
        /// Applies calculated CSS styles to a model paragraph style.
        /// </summary>
        /// <param name="paragraph">Paragraph to format.</param>
        internal void ToParagraphFormat(Paragraph paragraph)
        {
            Debug.Assert(paragraph != null);
            if (mCssStyleTracker.IsEmpty)
                return;

            mCssStyleTracker.PushImplicitDiv();

            ApplyParagraphStyleAndFormatting(paragraph.ParagraphFormat);

            if (mRemoveDuplicateDirectFormatting)
            {
                paragraph.RemoveDuplicateDirectFormatting(RevisionsView.Original);
            }

            mCssStyleTracker.PopImplicitElement();
        }

        /// <summary>
        /// Applies calculated CSS styles to a model font style.
        /// </summary>
        internal void ToFont(Font font, Style paragraphStyle)
        {
            // Also think about font size adjustment here.
            // We should correct size at the very end of collecting style information or even after particular node is parsed.
            // This is unlikely that we get such a style in a style sheet.

            Debug.Assert(font != null);
            if (mCssStyleTracker.IsEmpty)
                return;

            mCssStyleTracker.PushImplicitSpan();

            if (mRemoveDuplicateDirectFormatting)
            {
                RunPr newRunPr = new RunPr();
                Font newFont = Font.MakeFont(newRunPr, mDocument);

                // Needed to make Font.Style work on underlying levels.
                Style existingFontStyle = font.Style;
                if (existingFontStyle.StyleIdentifier != StyleIdentifier.DefaultParagraphFont)
                {
                    newFont.Istd = existingFontStyle.Istd;
                }

                ApplyFontStyleAndFormatting(newFont);

                // Apply only formatting that don't duplicate formatting from other levels (styles, document defaults, etc.)

                RunPr existingRunPr = paragraphStyle.GetExpandedRunPr(RunPrExpandFlags.DocumentDefaults);
                newFont.Style.ExpandRunPr(existingRunPr, RunPrExpandFlags.Normal);

                for (int i = 0; i < newRunPr.Count; i++)
                {
                    int key = newRunPr.GetKey(i);
                    object existingValue = existingRunPr.FetchAttr(key);
                    object newValue = newRunPr[key];
                    // Style istd is always copied here. It's checked for duplicates elsewhere.
                    if ((key == FontAttr.Istd) ||
                        (existingValue == null) ||
                        (!newValue.Equals(existingValue)))
                    {
                        font.Parent.SetRunAttr(key, newValue);
                    }
                }
            }
            else
            {
                ApplyFontStyleAndFormatting(font);
            }

            mCssStyleTracker.PopImplicitElement();
        }

        /// <summary>
        /// Applies CSS to the specified section.
        /// </summary>
        /// <param name="section">A model section.</param>
        internal void ToSection(Section section)
        {
            mSectionFormatter.Format(section);
        }

        internal CssDeclarationCollection GetAllPageDeclarations()
        {
            return mCssStyleTracker.GetAllPageDeclarations();
        }

        /// <summary>
        /// Applies calculated CSS styles to a horizontal rule shape.
        /// </summary>
        internal void ToHorizontalRule(Shape horizontalRuleShape)
        {
            Debug.Assert(horizontalRuleShape != null);
            if (mCssStyleTracker.IsEmpty)
                return;

            CssHRuleNoShadeStyleConverter.ToHorizontalRule(mCssStyleTracker.ElementDeclarations, horizontalRuleShape);
            CssHRuleAlignmentStyleConverter.ToHorizontalRule(mCssStyleTracker.ElementDeclarations, horizontalRuleShape);
            CssHRuleHeightStyleConverter.ToHorizontalRule(mCssStyleTracker.ElementDeclarations, horizontalRuleShape);
            foreach (CssDeclaration declaration in mCssStyleTracker.ElementDeclarations)
                ((CssComputedDeclaration)declaration).ToHorizontalRule(horizontalRuleShape);
        }

        /// <summary>
        /// Applies calculated CSS styles to a shape.
        /// </summary>
        internal void ToShape(Shape shape)
        {
            CssBorderStyleConverter.ToShape(mCssStyleTracker.ElementDeclarations, shape);
            foreach (CssDeclaration declaration in mCssStyleTracker.ElementDeclarations)
                ((CssComputedDeclaration)declaration).ToShape(shape);
            CssPositionStyleConverter.ToShape(mCssStyleTracker.ElementDeclarations, shape, mCssStyleTracker.BoxModel);
        }

        /// <summary>
        /// Applies calculated CSS styles to a document.
        /// </summary>
        internal void ToDocument(Document document, byte[] backgroundImageBytes)
        {
            foreach (CssDeclaration declaration in mCssStyleTracker.ElementDeclarations)
                ((CssComputedDeclaration)declaration).ToDocument(document);

            ApplyBackgroundImage(document, backgroundImageBytes, mCssStyleTracker.ElementDeclarations["background-image"]);
        }

        /// <summary>
        /// Determines whether the following elements are wrapped in a paragraph:
        /// svg, img, object, embed, textarea, iframe, input, select.
        /// </summary>
        internal bool IsDisplayedAsBlock()
        {
            switch (ElementDisplayType)
            {
                case CssDisplayType.Block:
                case CssDisplayType.ListItem:
                case CssDisplayType.Table:
                case CssDisplayType.TableCaption:
                    return true;
                default:
                    return false;
            }
        }

        internal SizeD GetEffectiveParentElementSize()
        {
            return mCssStyleTracker.GetEffectiveParentElementSize();
        }

        internal LineBreakClear GetLineBreakClear()
        {
            return mCssStyleTracker.GetLineBreakClear();
        }

        internal CssBoxModel BoxModel
        {
            get { return mCssStyleTracker.BoxModel; }
        }

        internal Style StyleWithoutDeclarations
        {
            get { return mStyleFormatter.StyleWithoutDeclarations; }
        }

        internal CssDeclarationCollection BeforePseudoElementDeclarations
        {
            get { return mCssStyleTracker.BeforePseudoElementDeclarations; }
        }

        internal HtmlElementDisplayState ElementDisplayState
        {
            get { return mCssStyleTracker.ElementDisplayState; }
        }

        internal CssDisplayType ElementDisplayType
        {
            get { return mCssStyleTracker.ElementDisplayType; }
        }

        internal EditRevision ElementInsertionRevision
        {
            get { return mCssStyleTracker.ElementInsertionRevision; }
        }

        internal EditRevision ElementDeletionRevision
        {
            get { return mCssStyleTracker.ElementDeletionRevision; }
        }

        private void ApplyFontStyleAndFormatting(Font font)
        {
            Style style = mStyleFormatter.GetModelStyle(StyleType.Character);
            if ((style != null) && (!ReferenceEquals(font.Style, style)))
            {
                font.Style = style;
            }

            mFontFormatter.Format(font, mCssStyleTracker);
        }

        private void ApplyParagraphStyleAndFormatting(ParagraphFormat pf)
        {
            Style style = mStyleFormatter.GetModelStyle(StyleType.Paragraph);
            if ((style != null) && (!ReferenceEquals(pf.Style, style)))
            {
                pf.Style = style;
            }

            mParagraphFormatter.Format(pf, mCssStyleTracker);
        }

        /// <summary>
        /// Applies 'background-image' CSS property to a document.
        /// </summary>
        private static void ApplyBackgroundImage(
            Document document,
            byte[] backgroundImageBytes,
            CssDeclaration backgroundImageDeclaration)
        {
            if (backgroundImageDeclaration == null)
                return;

            Debug.Assert(backgroundImageDeclaration.Value.Count == 1);

            CssValue backgroundImageValue = backgroundImageDeclaration.Value.FirstValue;
            if (backgroundImageValue.ValueType == CssValueType.Uri)
            {
                // WORDSNET-24088 Some image formats are not fully supported by the document model in all scenarios.
                // For example, there may be issues with GIF images upon rendering or saving to some document formats.
                // As a workaround, we currently convert such image formats to PNG upon loading. We plan, however, to get rid of
                // that conversion and store such images as is in the model, because MS Word supports that. We're going to
                // rework this code after support for additional formats improves (see WORDSNET-24146).
                backgroundImageBytes = HtmlImageUtil.GetSupportedImageBytes(backgroundImageBytes);
                if (backgroundImageBytes != null)
                {
                    if (document.BackgroundShape == null)
                        document.BackgroundShape = new Shape(document, ShapeType.Rectangle);

                    document.BackgroundShape.FillCore.SetImageBytes(backgroundImageBytes);
                    document.BackgroundShape.FillCore.FillType = FillTypeCore.Texture;
                }
            }
            else if (backgroundImageValue.Equals(CssValue.None))
            {
                // "background-image" has a priority over "background-color" and these properties both use DocumentBase.BackgroundShape
                // so we should eliminate background image only if background color was not previously set.
                if ((document.BackgroundShape == null) || !document.BackgroundShape.Filled)
                    document.SetBackgroundShapeSafe(null);
            }
        }

        private readonly Document mDocument;
        private readonly CssStyleTracker mCssStyleTracker;
        private readonly StyleFormatter mStyleFormatter;
        private readonly SectionFormatter mSectionFormatter;
        private readonly FontFormatter mFontFormatter;
        private readonly ParagraphFormatter mParagraphFormatter;

        /// <summary>
        /// Indicates whether direct formatting should be removed if it duplicates inherited formatting.
        /// </summary>
        private readonly bool mRemoveDuplicateDirectFormatting;
    }
}
