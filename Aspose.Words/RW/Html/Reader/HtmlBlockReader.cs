// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/01/2022 by Artem Shabarshin

using System.Collections.Generic;
using Aspose.Collections;
using Aspose.Collections.Generic;
using Aspose.Common;
using Aspose.Drawing;
using Aspose.Words.RW.Html.Css;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Creates <see cref="HtmlBlock"/> nodes from 'div', 'body' and 'blockquote' HTML elements and uses them to store element's
    /// borders and margins.
    /// </summary>
    internal class HtmlBlockReader
    {
        /// <param name="disabled">
        /// Indicates whether the HTML block reader is disabled.
        /// If it is <c>true</c> then HTML blocks are not created.
        /// </param>
        /// <param name="applyFormattingAsMsWord">
        /// Indicates whether we should stick to MS Word's rules when applying CSS formatting to document model.
        /// </param>
        internal HtmlBlockReader(bool disabled, bool applyFormattingAsMsWord)
        {
            mDisabled = disabled;
            mApplyFormattingAsMsWord = applyFormattingAsMsWord;
        }

        /// <summary>
        /// Initializes the HTML block reader.
        /// </summary>
        internal void Init(DocumentBuilder builder)
        {
            Debug.Assert(builder != null);

            mBuilder = builder;

            Debug.Assert(mBuilder.CurrentParagraph != null);
            Paragraph currentParagraph = mBuilder.CurrentParagraph;

            bool isSectionEmpty = IsCurrentSectionEmpty;

            // Do nothing if the first section of the document is empty
            // and the current paragraph is inside the body.
            if (isSectionEmpty &&
                (currentParagraph.ParentSection == mBuilder.Document.FirstSection) &&
                !mApplyFormattingAsMsWord)
            {
                return;
            }

            mInsertHtmlTargetParagraph = currentParagraph;

            if (mDisabled)
                return;

            // Take into account Id's of already created HTML blocks.
            for (int i = 0; i < mInsertHtmlTargetParagraph.Document.HtmlBlockCollection.Count; ++i)
            {
                mGeneratedIds.Add(mBuilder.Document.HtmlBlockCollection.GetHtmlBlockByIndex(i).Id, null);
            }

            if (!mApplyFormattingAsMsWord)
            {

                // Сreate an empty intermediate HTML block (mimic MS Word behavior).
                mEmptyInitialHtmlBlock = CreateHtmlBlock();

                bool isHeaderFooterEmpty = (mInsertHtmlTargetParagraph.Count == 0) &&
                    (mInsertHtmlTargetParagraph.ParentStory != null) &&
                    (mInsertHtmlTargetParagraph.ParentStory.NodeType == NodeType.HeaderFooter) &&
                    (mInsertHtmlTargetParagraph.ParentStory.Count == 1);

                // If a new HTML block is being inserted into a table or an empty section or an empty header or footer
                // then MS Word marks an empty intermediate HTML block as 'body'
                // and doesn't connect a new HTML block with the parent HTML block.
                if (mInsertHtmlTargetParagraph.IsInCell || isSectionEmpty || isHeaderFooterEmpty)
                {
                    mEmptyInitialHtmlBlock.HtmlBlockType = HtmlBlockType.Body;
                }
                else
                {
                    mEmptyInitialHtmlBlock.ParentId = mInsertHtmlTargetParagraph.ParaPr.HtmlBlockId;
                }
                // If the HTML document is being inserted into the current paragraph then this
                // paragraph should be enclosed by the empty intermediate HTML block.
                if (mBuilder.IsAtStartOfParagraph)
                    mInsertHtmlTargetParagraph.ParaPr.HtmlBlockId = mEmptyInitialHtmlBlock.Id;
            }

            if (mApplyFormattingAsMsWord)
            {
                mInsertHtmlTargetParagraph = null;
            }
            else
            {
                // The initial HTML block doesn't have any background colors.
                RememberOpenedHtmlBlock(mEmptyInitialHtmlBlock, new CssBackgroundColor((CssDeclaration)null));
            }
        }

        /// <summary>
        /// Starts a new HTML block.
        /// </summary>
        /// <remarks>
        /// Returns <c>true</c> if HTML block was in fact started.
        /// </remarks>
        internal bool StartHtmlBlock(
            string elementName,
            CssDeclarationCollection elementDeclarations,
            CssDisplayType elementDisplayType)
        {
            if (!CreatesHtmlBlock(elementName, elementDeclarations))
                return false;

            HtmlBlock htmlBlock = CreateHtmlBlock();
            htmlBlock.HtmlBlockType = GetHtmlBlockType(elementName);

            if (IsInsideHtmlBlock)
            {
                HtmlBlock parentHtmlBlock = mOpenedHtmlBlocksStack.Peek();
                htmlBlock.ParentId = parentHtmlBlock.Id;

                // If the inserted HTML block is 'body' then an empty intermediate HTML block should be removed.
                if (htmlBlock.HtmlBlockType == HtmlBlockType.Body)
                {
                    htmlBlock.ParentId = parentHtmlBlock.ParentId;
                    ForgetOpenedHtmlBlock();
                    mEmptyInitialHtmlBlock = null;

                    // If the HTML document with 'body' tag was inserted into already created 'body' HTML block
                    // then the child HTML block should be created as 'div' HTML block.
                    if (HasParentBodyHtmlBlock())
                        htmlBlock.HtmlBlockType = HtmlBlockType.Div;
                }
            }
            RememberOpenedHtmlBlock(htmlBlock, new CssBackgroundColor(elementDeclarations));

            SetBorder(elementDeclarations, htmlBlock.ParaPr);

            SetMargin(elementDeclarations, htmlBlock.ParaPr);

            // WORDSNET-21063 We set the HTML block only for a block-level element.
            // In the case inserting the HTML document into non-empty paragraph
            // an element with non-block display type may create the HTML block or for this paragraph
            // or for the next paragraph. It depends on the inner elements. The HTML block will be set
            // during updating the corresponding paragraph.
            if (elementDisplayType == CssDisplayType.Block)
            {
                mBuilder.CurrentParagraph.ParaPr.HtmlBlockId = htmlBlock.Id;
                // If we set the HTML block for the paragraph into which the HTML document will be inserted
                // then we should allow to change any HTML block properties during updating this paragraph.
                if (mInsertHtmlTargetParagraph == mBuilder.CurrentParagraph)
                    mInsertHtmlTargetParagraph = null;
            }

            return true;
        }

        /// <summary>
        /// Ends current HTML block.
        /// </summary>
        internal void EndHtmlBlock(
            string elementName,
            CssDeclarationCollection elementDeclarations,
            HtmlBidiTextArranger bidiTextArranger)
        {
            if (!CreatesHtmlBlock(elementName, elementDeclarations))
                return;

            Debug.Assert(IsInsideHtmlBlock);
            HtmlBlock htmlBlock = ForgetOpenedHtmlBlock();
            Paragraph paragraph = mBuilder.CurrentParagraph;
            bool htmlBlockIsUsed = false;

            // Check all adjacent paragraphs sharing the same HTML block ID.
            while (paragraph.ParaPr.HtmlBlockId == htmlBlock.Id)
            {
                // Remove the HTML block from an empty paragraph if it is not before a table
                // and doesn't have any text in the bidi text arranger.
                // The bidi text arranger may contain a text if the HTML block
                // was started from inline `div` or `blockquote` elements.
                if (paragraph.IsEmptyOrContainsOnlyCrossAnnotation &&
                    ((bidiTextArranger == null) || bidiTextArranger.IsEmpty) &&
                    ((paragraph.PreviousSibling == null) || (paragraph.PreviousSibling.NodeType != NodeType.Table)) &&
                    // WORDSNET-22805 Empty paragraphs created from HTML breaks shouldn't split the HTML block.
                    !mEmptyInHtmlParagraphs.Contains(paragraph))
                {
                    paragraph.ParaPr.Remove(ParaAttr.HtmlBlockId);
                }
                else
                {
                    htmlBlockIsUsed = true;
                }

                if ((paragraph.PreviousSibling == null) || (paragraph.PreviousSibling.NodeType != NodeType.Paragraph))
                {
                    break;
                }

                paragraph = (Paragraph)paragraph.PreviousSibling;
            }

            if (htmlBlockIsUsed)
            {
                AddHtmlBlock(htmlBlock);
                if (htmlBlock.ParentId != 0)
                {
                    AddParentHtmlBlock(htmlBlock);
                }
            }
        }

        /// <summary>
        /// Updates paragraph properties if it is inside <see cref="HtmlBlock"/>.
        /// If the paragraph was created from 'body' or 'div' all borders and margins
        /// should be stored in the corresponding HTML block.
        /// </summary>
        /// <param name="removePropertiesAppliedToHtmlBlock">
        /// Indicates whether need to remove paragraph properties if they were applied to the corresponding HTML block.
        /// </param>
        internal void UpdateCurrentParagraph(bool removePropertiesAppliedToHtmlBlock)
        {
            // The empty initial HTML block should be added only during inserting HTML document.
            // In case of reading altChunk the initial HTML block will be added only together with a new HTML block.
            if (mEmptyInitialHtmlBlock != null && !mApplyFormattingAsMsWord)
            {
                AddHtmlBlock(mEmptyInitialHtmlBlock);
                mEmptyInitialHtmlBlock = null;
            }

            Paragraph paragraph = mBuilder.CurrentParagraph;

            // Don't set the HTML block properties for a paragraph inside a table.
            if (paragraph.IsInCell)
                return;

            // Don't set the HTML block properties for a paragraph after or into which the HTML document is being inserted.
            if (paragraph == mInsertHtmlTargetParagraph)
                return;

            // If a paragraph is being inserted into an existing HTML block then this paragraph should be part of this block
            // regardless of the HTML block reader's disabled status.
            if (mDisabled && (mInsertHtmlTargetParagraph != null) && (mInsertHtmlTargetParagraph.ParaPr.HtmlBlockId != 0))
            {
                paragraph.ParaPr.HtmlBlockId = mInsertHtmlTargetParagraph.ParaPr.HtmlBlockId;
            }

            if (mDisabled)
                return;

            if (IsInsideHtmlBlock)
            {
                HtmlBlock htmlBlock = mOpenedHtmlBlocksStack.Peek();
                UpdateParagraph(paragraph, htmlBlock, removePropertiesAppliedToHtmlBlock);
                UpdateBackgroundColor(paragraph);
            }
            else
            {
                // The paragraph arranger copies the HTML block Id to a new inserted paragraph.
                // At this case we should remove the HTML block manually
                // if the current paragraph is outside of any HTML blocks.
                paragraph.ParaPr.Remove(ParaAttr.HtmlBlockId);
            }
        }

        /// <summary>
        /// Marks the specified paragraph as empty in the HTML document.
        /// </summary>
        internal void MarkParagraphAsEmptyInHtml(Paragraph paragraph)
        {
            if (mDisabled)
                return;

            mEmptyInHtmlParagraphs.Add(paragraph);
        }

        /// <summary>
        /// Updates table properties if it is inside <see cref="HtmlBlock"/>.
        /// </summary>
        internal void UpdateTable(Table table)
        {
            if (mDisabled)
                return;

            if (IsInsideHtmlBlock && (table.FirstRow != null))
            {
                HtmlBlock htmlBlock = mOpenedHtmlBlocksStack.Peek();

                // Set the HTML block for all table rows.
                foreach (Row row in table.Rows)
                {
                    row.TablePr.HtmlBlockId = htmlBlock.Id;
                }

                Paragraph paragraphAfterTable = table.NextSibling as Paragraph;
                if (paragraphAfterTable != null)
                {
                    UpdateParagraph(paragraphAfterTable, htmlBlock, false);
                }
            }
        }

        /// <summary>
        /// Inserts a hidden empty paragraph that prevents the table from concatenating with the previous table in MS Word.
        /// </summary>
        internal void InsertSeparatorParagraphBeforeTable(Table table)
        {
            if (table == null)
                return;

            Table prevTable = table.PreviousNonAnnotationSibling as Table;
            if (prevTable == null)
                return;

            // WORDSNET-24029 In certain cases HTML blocks prevent tables from concatenating and MS Word doesn't
            // write a separator paragraph.
            if (!MsWordSeparatesTables(prevTable, table))
                return;

            // Insert a hidden empty separator paragraph.
            Paragraph para = new Paragraph(mBuilder.Document);
            para.ParagraphBreakRunPr.Hidden = AttrBoolEx.True;
            prevTable.InsertNext(para);

            // If the second table is inside a HTML block, place the separator paragraph in that block too.
            // This is what MS Word does.
            int tableHtmlBlockId = (table.FirstRow != null)
                ? table.FirstRow.TablePr.HtmlBlockId
                : 0;
            if (tableHtmlBlockId != 0)
            {
                para.ParaPr.HtmlBlockId = tableHtmlBlockId;
            }
        }

        /// <summary>
        /// Indicates whether current HTML element creates a HTML block in the document model.
        /// </summary>
        internal bool CreatesHtmlBlock(string elementName, CssDeclarationCollection elementDeclarations)
        {
            return !mDisabled && CssUtil.CreatesHtmlBlock(elementName, elementDeclarations, mApplyFormattingAsMsWord);
        }

        private bool IsInsideHtmlBlock
        {
            get { return mOpenedHtmlBlocksStack.Count > 0; }
        }

        private bool IsCurrentSectionEmpty
        {
            get
            {
                // WORDSNET-22838 MS Word considers a paragraph with the cross annotations only as an empty.
                // At this case it doesn't add an empty intermediate HTML block. Mimic this behavior.
                return mBuilder.CurrentParagraph.IsEmptyOrContainsOnlyCrossAnnotation &&
                (mBuilder.CurrentParagraph.ParentStory != null) &&
                (mBuilder.CurrentParagraph.ParentStory.NodeType == NodeType.Body) &&
                (mBuilder.CurrentParagraph.ParentStory.Count == 1);
            }
        }

        private HtmlBlock CreateHtmlBlock()
        {
            HtmlBlock htmlBlock = new HtmlBlock(GenerateNextId());
            return htmlBlock;
        }

        private static void UpdateParagraph(
            Paragraph paragraph,
            HtmlBlock htmlBlock,
            bool removePropertiesAppliedToHtmlBlock)
        {
            paragraph.ParaPr.SetAttr(ParaAttr.HtmlBlockId, htmlBlock.Id);

            // If a new HTML block was started then the current paragraph and HTML block
            // share the same properties. Remove duplicates.
            if (removePropertiesAppliedToHtmlBlock)
            {
                paragraph.ParaPr.Remove(ParaAttr.BorderLeft);
                paragraph.ParaPr.Remove(ParaAttr.BorderRight);
                paragraph.ParaPr.Remove(ParaAttr.BorderTop);
                paragraph.ParaPr.Remove(ParaAttr.BorderBottom);
                paragraph.ParaPr.Remove(ParaAttr.LeftIndent);
                paragraph.ParaPr.Remove(ParaAttr.RightIndent);
                // MS Word sets direct spacing for a paragraph if it is not a list item. Mimic this behavior.
                if (!paragraph.IsListItem)
                {
                    paragraph.ParaPr.Remove(ParaAttr.SpaceBefore);
                    paragraph.ParaPr.Remove(ParaAttr.SpaceAfter);
                }
            }
        }

        private int GenerateNextId()
        {
            // MS Word uses positive random integers for HTML block ID values.
            // HTML block collection may contain already created HTML blocks.
            int resultingId;
            do
            {
                resultingId = System.Math.Abs(RandomUtil.NewGuid().GetHashCode());
            }
            while ((resultingId == 0) ||
                mGeneratedIds.ContainsKey(resultingId));

            mGeneratedIds.Add(resultingId, null);
            return resultingId;
        }

        /// <summary>
        /// Makes sure all parent blocks of the specified HTML block are also added to the collection.
        /// </summary>
        private void AddParentHtmlBlock(HtmlBlock htmlBlock)
        {
            HtmlBlock currentHtmlBlock = htmlBlock;
            // Check all preceding HTML blocks.
            if (currentHtmlBlock.ParentId != 0)
            {
                foreach (HtmlBlock htmlBlockId in mOpenedHtmlBlocksStack)
                {
                    if (GetHtmlBlockById(htmlBlockId.Id) == null)
                    {
                        AddHtmlBlock(htmlBlockId);
                    }
                    if (htmlBlockId.ParentId == 0)
                        break;
                }
            }
        }

        private bool HasParentBodyHtmlBlock()
        {
            if ((mInsertHtmlTargetParagraph == null) ||
                (mInsertHtmlTargetParagraph.ParaPr.HtmlBlockId == 0))
            {
                return false;
            }

            HtmlBlock currentHtmlBlock = GetHtmlBlockById(mInsertHtmlTargetParagraph.ParaPr.HtmlBlockId);
            while (currentHtmlBlock != null)
            {
                if (currentHtmlBlock.HtmlBlockType == HtmlBlockType.Body)
                    return true;

                currentHtmlBlock = GetHtmlBlockById(currentHtmlBlock.ParentId);
            }
            return false;
        }

        private HtmlBlock GetHtmlBlockById(int id)
        {
            return mBuilder.Document.HtmlBlockCollection.GetHtmlBlockById(id);
        }

        private void AddHtmlBlock(HtmlBlock htmlBlock)
        {
            mBuilder.Document.HtmlBlockCollection.Add(htmlBlock);
        }

        /// <summary>
        /// Enumerates HTML blocks that contain the specified table and return their IDs in a stack. The top of the stack is
        /// the outermost block's ID.
        /// </summary>
        private IntStack EnumerateContainingHtmlBlocks(Table table)
        {
            IntStack blockIds = new IntStack();
            int blockId = (table.FirstRow != null)
                ? table.FirstRow.TablePr.HtmlBlockId
                : 0;
            for (HtmlBlock htmlBlock = FindHtmlBlock(blockId); htmlBlock != null; htmlBlock = FindHtmlBlock(blockId))
            {
                blockIds.Push(blockId);
                blockId = htmlBlock.ParentId;
            }
            return blockIds;
        }

        /// <summary>
        /// Returns a value indicating whether MS Word adds an empty hidden separator paragraph between the specified adjacent
        /// tables in order to prevent their rows from concatenating.
        /// </summary>
        private bool MsWordSeparatesTables(Table table1, Table table2)
        {
            IntStack table1Ids = EnumerateContainingHtmlBlocks(table1);
            IntStack table2Ids = EnumerateContainingHtmlBlocks(table2);

            if ((table1Ids.Count == 0) && (table2Ids.Count == 0))
            {
                // Adjacent tables that are not enclosed by HTML blocks are always separated by an empty span.
                return true;
            }

            // MS Word demonstrates counter-intuitive behavior when it comes to separating tables in HTML blocks. Tables are
            // separated if the second one is more deeply nested in HTML blocks, or if both tables are nested equally deep
            // but share a common HTML block. If the first table is more deeply nested than the second one, MS Word doesn't
            // separate them.
            bool haveCommonBlocks =
                (table1Ids.Count > 0) &&
                (table2Ids.Count > 0) &&
                (table1Ids.Peek() == table2Ids.Peek());

            return (table2Ids.Count > table1Ids.Count) ||
                ((table2Ids.Count == table1Ids.Count) && haveCommonBlocks);
        }

        /// <summary>
        /// Searches for a HTML block by its ID both among blocks that are already stored in the document and among active
        /// open blocks that haven't been added to the document yet.
        /// </summary>
        private HtmlBlock FindHtmlBlock(int blockId)
        {
            if (blockId == 0)
                return null;

            HtmlBlock knownBlock = GetHtmlBlockById(blockId);
            if (knownBlock != null)
                return knownBlock;

            foreach (HtmlBlock openBlock in mOpenedHtmlBlocksStack)
            {
                if (openBlock.Id == blockId)
                    return openBlock;
            }

            return null;
        }

        private void UpdateBackgroundColor(Paragraph paragraph)
        {
            Debug.Assert(mOpenedHtmlBlockBackgroundColors.Count > 0);
            mOpenedHtmlBlockBackgroundColors.Peek().ToShading(paragraph.ParagraphFormat.Shading);
        }

        private static void SetBorder(
            CssDeclarationCollection elementDeclarations,
            ParaPr paraPr)
        {
            foreach (BorderType borderType in gBorderTypes)
            {
                SetBorder(elementDeclarations, paraPr, borderType);
            }
        }

        private static void SetBorder(
            CssDeclarationCollection elementDeclarations,
            ParaPr paraPr,
            BorderType borderType)
        {
            Border border;
            CssBorder cssBorder = CssBorder.CreateHtmlBlockBorder(elementDeclarations, borderType);

            if ((cssBorder.LineStyle != LineStyle.None) && (cssBorder.ColorInternal != DrColor.Transparent))
            {
                ApplyCSSBorderStyle(elementDeclarations, paraPr, cssBorder, borderType);
            }
            else
            {
                // Set border color if border declaration is not defined but border color is present.
                string borderColorProperty;
                switch (borderType)
                {
                    case BorderType.Left:
                        borderColorProperty = "border-left-color";
                        break;
                    case BorderType.Right:
                        borderColorProperty = "border-right-color";
                        break;
                    case BorderType.Top:
                        borderColorProperty = "border-top-color";
                        break;
                    case BorderType.Bottom:
                        borderColorProperty = "border-bottom-color";
                        break;
                    default:
                        return;
                }
                DrColor borderColor = elementDeclarations.GetColor(borderColorProperty);
                if (borderColor != null)
                {
                    border = new Border();
                    border.ColorInternal = borderColor;
                    SetBorder(paraPr, border, borderType);
                }
            }
        }

        private static void SetBorder(
            ParaPr paraPr,
            Border border,
            BorderType borderType)
        {
            switch (borderType)
            {
                case BorderType.Left:
                    paraPr.SetAttr(ParaAttr.BorderLeft, border);
                    break;
                case BorderType.Right:
                    paraPr.SetAttr(ParaAttr.BorderRight, border);
                    break;
                case BorderType.Top:
                    paraPr.SetAttr(ParaAttr.BorderTop, border);
                    break;
                case BorderType.Bottom:
                    paraPr.SetAttr(ParaAttr.BorderBottom, border);
                    break;
                default:
                    // Unsupported border type. Nothing to do.
                    break;
            }
        }

        private static void ApplyCSSBorderStyle(
            CssDeclarationCollection elementDeclarations,
            ParaPr paraPr,
            CssBorder cssBorder,
            BorderType borderType)
        {
            Border border;
            string paddingAttribute;
            switch (borderType)
            {
                case BorderType.Left:
                    border = paraPr.BorderLeft;
                    paddingAttribute = "padding-left";
                    break;
                case BorderType.Right:
                    border = paraPr.BorderRight;
                    paddingAttribute = "padding-right";
                    break;
                case BorderType.Top:
                    border = paraPr.BorderTop;
                    paddingAttribute = "padding-top";
                    break;
                case BorderType.Bottom:
                    border = paraPr.BorderBottom;
                    paddingAttribute = "padding-bottom";
                    break;
                default:
                    // Unsupported border type. Nothing to do.
                    return;
            }

            if (Border.Empty.Equals(border))
            {
                border = new Border();
                SetBorder(paraPr, border, borderType);
            }

            border.LineStyleInternal = cssBorder.LineStyle;
            border.ColorInternal = cssBorder.ColorInternal;
            border.SetLineWidthSafe(ConvertToMswBorderWidth(cssBorder.LineWidth));

            // If the border has a line style then padding value should be set to distance of the border from text.
            if (border.LineStyle != LineStyle.None)
            {
                CssDeclaration paddingDeclaration = elementDeclarations[paddingAttribute];
                if (paddingDeclaration == null)
                    return;

                CssPropertyValue paddingValue = paddingDeclaration.Value;
                if (paddingValue.FirstValue.ValueType == CssValueType.Length)
                {
                    CssLengthValue lengthValue = (CssLengthValue)paddingValue.FirstValue;
                    int distanceFromText = DoublePal.RoundToIntUp(lengthValue.GetLength(CssUnit.Pt));

                    // WORDSNET-25528 Do not throw exception, if distance from text exceeds maximum value.
                    border.SetDistanceFromTextSafe(distanceFromText);
                }
            }
        }

        private static void SetMargin(
            CssDeclarationCollection elementDeclarations,
            ParaPr paraPr)
        {
            SetMargin(elementDeclarations, "margin-left", ParaAttr.HtmlMarginLeft, paraPr);
            SetMargin(elementDeclarations, "margin-right", ParaAttr.HtmlMarginRight, paraPr);
            SetMargin(elementDeclarations, "margin-top", ParaAttr.HtmlMarginTop, paraPr);
            SetMargin(elementDeclarations, "margin-bottom", ParaAttr.HtmlMarginBottom, paraPr);
        }

        private static void SetMargin(
            CssDeclarationCollection elementDeclarations,
            string propertyName,
            int paraAttr,
            ParaPr paraPr)
        {
            double marginValueInTwips;
            if (elementDeclarations.GetIdentifier(propertyName) == "auto")
            {
                // The 'auto' margin value should be converted to zero.
                marginValueInTwips = 0;
            }
            else
            {
                double marginValueInPoints = elementDeclarations.GetLength(propertyName);
                if (MathUtil.IsMinValue(marginValueInPoints))
                {
                    // MS Word uses the percentage value as the absolute margin value in twips.
                    // The maximum allowed value is 500%.
                    marginValueInTwips = System.Math.Min(500, elementDeclarations.GetPercentage(propertyName));
                }
                else
                {
                    // According to ECMA-376-1:2016 17.15.2.26 marLeft (Left Margin for HTML div).
                    marginValueInTwips = ConvertUtilCore.PointToTwip(marginValueInPoints);
                }
            }
            if (!MathUtil.IsMinValue(marginValueInTwips))
            {
                // Top and Bottom margins should be positive or zero.
                bool acceptNegativeValues = (propertyName != "margin-top") && (propertyName != "margin-bottom");

                if (MathUtil.IsGreaterOrEqual(marginValueInTwips, 0) || acceptNegativeValues)
                {
                    // MS Word limits the max value for the margins:
                    // 21600 twips for the left and the right.
                    // 18928 twips for the up and the down.
                    int marginLimit = (propertyName == "margin-left") || (propertyName == "margin-right")
                        ? 21600
                        : 18928;
                    paraPr.SetAttr(paraAttr, System.Math.Min(marginLimit, MathUtil.DoubleToInt(marginValueInTwips)));
                }
            }
        }

        private static HtmlBlockType GetHtmlBlockType(string elementName)
        {
            switch (elementName)
            {
                case "body":
                    return HtmlBlockType.Body;
                case "blockquote":
                    return HtmlBlockType.BlockQuote;
                default:
                    return HtmlBlockType.Div;
            }
        }

        /// <summary>
        /// Returns a MS Word border width that is the closest to the specified value.
        /// </summary>
        private static double ConvertToMswBorderWidth(double borderWidth)
        {
            int borderWidthInEighthPoints = ConvertUtilCore.PointToEightsPoint(borderWidth);

            if (borderWidthInEighthPoints <= gMswBorderWidths[0])
            {
                return ConvertUtilCore.EightsPointToPoint(gMswBorderWidths[0]);
            }
            for (int i = 0; i < (gMswBorderWidths.Length - 1); i++)
            {
                int midpoint = (gMswBorderWidths[i] + gMswBorderWidths[i + 1]) / 2;
                // Return the greater value if the original width is right in the middle between two values.
                if (borderWidthInEighthPoints < midpoint)
                {
                    return ConvertUtilCore.EightsPointToPoint(gMswBorderWidths[i]);
                }
            }
            return ConvertUtilCore.EightsPointToPoint(gMswBorderWidths[gMswBorderWidths.Length - 1]);
        }

        private void RememberOpenedHtmlBlock(HtmlBlock htmlBlock, CssBackgroundColor backgroundColor)
        {
            mOpenedHtmlBlocksStack.Push(htmlBlock);
            mOpenedHtmlBlockBackgroundColors.Push(backgroundColor);
        }

        private HtmlBlock ForgetOpenedHtmlBlock()
        {
            HtmlBlock htmlBlock = mOpenedHtmlBlocksStack.Pop();
            mOpenedHtmlBlockBackgroundColors.Pop();
            return htmlBlock;
        }

        /// <summary>
        /// MS Word uses only the following border width values when importing HTML blocks. In 1/8th of a point.
        /// </summary>
        private static readonly int[] gMswBorderWidths = { 2, 4, 6, 8, 12, 18, 24, 36, 48 };

        /// <summary>
        /// The paragraph into which the HTML document is being inserted.
        /// </summary>
        private Paragraph mInsertHtmlTargetParagraph;

        private DocumentBuilder mBuilder;

        private HtmlBlock mEmptyInitialHtmlBlock;

        /// <summary>
        /// A flag indicating whether creation of HTML blocks is in fact disabled and the HTML block reader is "idle".
        /// </summary>
        /// <remarks>
        /// If creation of HTML blocks is disabled, the HTML block reader is still called by the outer code but it doesn't do
        /// any useful job. We've introduced this "idle" mode in order to simplify the outer logic and make it unconditional
        /// by moving all condition checks inside the HTML block reader.
        /// </remarks>
        private readonly bool mDisabled;

        /// <summary>
        /// Indicates whether we should stick to MS Word's rules when applying CSS formatting to document model.
        /// </summary>
        private readonly bool mApplyFormattingAsMsWord;

        /// <summary>
        /// A stack of HTML blocks that are opened at the moment.
        /// </summary>
        /// <remarks>
        /// Used to resolve nested HTML blocks.
        /// </remarks>
        private readonly Stack<HtmlBlock> mOpenedHtmlBlocksStack = new Stack<HtmlBlock>();

        /// <summary>
        /// A stack of CSS background colors that are connected with the opened HTML block.
        /// </summary>
        private readonly Stack<CssBackgroundColor> mOpenedHtmlBlockBackgroundColors = new Stack<CssBackgroundColor>();

        /// <summary>
        /// Generates unique IDs for created HTML blocks.
        /// </summary>
        /// <remarks>
        /// We use this dictionary as a hash set of integers and are only interested in item keys. Item values are ignored.
        /// We can't use the HTML block collection instead of dictionary because not all opened HTML blocks may be added to the collection.
        /// </remarks>
        private readonly IntToObjDictionary<object> mGeneratedIds = new IntToObjDictionary<object>();

        /// <summary>
        /// A set of <see cref="Paragraph"/> nodes that correspond to empty paragraphs in the HTML document.
        /// </summary>
        private readonly HashSetGeneric<Paragraph> mEmptyInHtmlParagraphs = new HashSetGeneric<Paragraph>();

        private static readonly BorderType[] gBorderTypes = new BorderType[]
        {
            BorderType.Top,
            BorderType.Right,
            BorderType.Bottom,
            BorderType.Left
        };
    }
}
