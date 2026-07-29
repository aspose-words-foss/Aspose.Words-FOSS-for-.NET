// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 26/06/2009 by Roman Korchagin

using System.Collections.Generic;
using Aspose.Words.Fields;
using Aspose.Words.Themes;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// At the moment this is a hack/experimental way to deal with table styles for exporters
    /// that do not support table styles.
    /// Say if we read from DOCX (where we parse table styles) and then save to DOC, HTML etc
    /// where we don't support table styles, we call this to expand all table style formatting
    /// and permanently modify formatting in the tables in the document.
    /// </summary>
    internal class TableFormattingExpander : DocumentVisitor
    {
        /// <summary>
        /// Restores direct attributes of nodes affected by table formatting expander.
        /// </summary>
        internal void RestoreDirectAttrs()
        {
            foreach(Node n in mAffectedNodes)
            {
                switch (n.NodeType)
                {
                    case NodeType.Row:
                        // WORDSNET-20118 Added Clone() here and below to get unique objects after the first expand/restore.
                        Row row = (Row)n;
                        TablePr savedRowAttributes = (TablePr)((Row)n).TablePr.SysDirectAttrs.Clone();
                        // Dmatv: as a side effect of restoring the attributes saved before validator,
                        // attributes set during table grid calculation are discarded.
                        // The code below works around the issue.

                        // Save direct auto-fit attributes, they may have been updated by auto-fit grid calculation in layout.
                        object[] directAutoFitRowAttrs = SaveDirectAutoFitCalculatedAttrs(row.TablePr, gAutoFitGridCalculatedRowAttrs);

                        row.TablePr = savedRowAttributes;

                        // Overwrite attributes saved before layout update with the current values.
                        RestoreDirectAutoFitCalculatedAttrs(row.TablePr, directAutoFitRowAttrs, gAutoFitGridCalculatedRowAttrs);

                        break;

                    case NodeType.Cell:
                        Cell cell = (Cell) n;
                        // Dmatv: as a side effect of restoring the attributes saved before validator,
                        // attributes set during table grid calculation are discarded.
                        // The code below works around the issue.
                        CellPr savedAttributes = (CellPr)cell.CellPr.SysDirectAttrs.Clone();

                        // Save direct auto-fit attributes, they may have been updated by auto-fit grid calculation in layout.
                        object[] directAutoFitAttrs = SaveDirectAutoFitCalculatedAttrs(cell.CellPr, gAutoFitGridCalculatedCellAttrs);

                        cell.CellPr = savedAttributes;

                        // Overwrite attributes saved before layout update with the current values.
                        RestoreDirectAutoFitCalculatedAttrs(cell.CellPr, directAutoFitAttrs, gAutoFitGridCalculatedCellAttrs);

                        break;

                    case NodeType.Paragraph:
                        ((Paragraph)n).ParaPr = (ParaPr)((Paragraph)n).ParaPr.SysDirectAttrs.Clone();
                        ((Paragraph)n).ParagraphBreakRunPr = (RunPr)((Paragraph)n).ParagraphBreakRunPr.SysDirectAttrs.Clone();
                        break;

                    case NodeType.Run:
                        ((Run)n).RunPr = (RunPr)((Run)n).RunPr.SysDirectAttrs.Clone();
                        break;

                    default:
                        break;
                }
            }
            mAffectedNodes.Clear();
        }

        /// <summary>
        /// Expands formatting in the specified table (but not in the nested tables).
        /// </summary>
        internal void Expand(Table table)
        {
            mTable = table;
            mTheme = mTable.Document.GetThemeInternal();
            table.Accept(this);
        }

        public override VisitorAction VisitFieldStart(FieldStart fieldStart)
        {
            if (fieldStart.FieldType == FieldType.FieldTOC)
                mIsInTocField = true;

            return base.VisitFieldStart(fieldStart);
        }

        public override VisitorAction VisitFieldEnd(FieldEnd fieldEnd)
        {
            if (fieldEnd.FieldType == FieldType.FieldTOC)
                mIsInTocField = false;

            return base.VisitFieldEnd(fieldEnd);
        }

        public override VisitorAction VisitTableStart(Table table)
        {
            mHeadingRowCount = 1;
            mIsBodyRowOccurred = false;

            // It could be a nested table, skip it because we just want to expand one table at a time.
            if (table != mTable)
                return VisitorAction.SkipThisNode;

            // Use the first row to get the table formatting, because in the current model, the
            // table formatting is said to be "stored" in the first row.
            Row row = table.FirstRow;
            if (row == null)
                return VisitorAction.SkipThisNode;

            // Expand table style properties that are for the whole table.
            // Use dynamic casting for safety because in invalid documents it might return a paragraph style for example.
            TableStyle tableStyle = row.Document.Styles.FetchByIstd(row.TablePr.Istd, StyleIndex.TableNormal) as TableStyle;
            if (tableStyle == null)
                return VisitorAction.SkipThisNode;

            mStyleTablePr = tableStyle.GetExpandedTablePr();
            mStyleRowPr = tableStyle.GetExpandedRowPr();
            mStyleCellPr = tableStyle.GetExpandedCellPr();
            mStyleParaPr = tableStyle.GetExpandedParaPr(ParaPrExpandFlags.Normal);
            mStyleRunPr = tableStyle.GetExpandedRunPr(RunPrExpandFlags.Normal);

            // Collect types of defined conditional styles.
            List<TableStyleOverrideType> definedOverrideTypes = new List<TableStyleOverrideType>();
            TableStyle style = tableStyle;
            while (style != null)
            {
                foreach (ConditionalStyle conditionalStyle in style.ConditionalStyles.DefinedStyles)
                {
                    if (!definedOverrideTypes.Contains(conditionalStyle.OverrideType))
                        definedOverrideTypes.Add(conditionalStyle.OverrideType);
                }

                style = style.GetBaseStyle() as TableStyle;
            }

            // Build a hashtable of conditional formatting so we can find needed table style prs fast.
            mConditionalStyles = new ConditionalStyle[gTableStyleOverrideTypeCount];
            foreach (TableStyleOverrideType type in definedOverrideTypes)
                mConditionalStyles[(int)type] = tableStyle.GetExpandedConditionalStyle(type);

            mRowBandSize = mStyleTablePr.StyleRowBandSize;
            mColBandSize = mStyleTablePr.StyleColBandSize;
            mIsHeadingRow = row.IsHeadingRow && (GetConditionalStyle(TableStyleOverrideType.FirstRow) != null);

            CalcPartsToApply(row.TablePr.StyleOptions);

            mRowIndex = 0;

            return VisitorAction.Continue;
        }

        public override VisitorAction VisitRowStart(Row row)
        {
            // Table can have more than one heading row but they must be in continuous range.
            mIsFirstRow = !mIsBodyRowOccurred && ((bool)((IRowAttrSource)row).FetchRowAttr(TableAttr.HeadingFormat) || row.IsFirstRow);
            mIsLastRow = row.IsLastRow;

            if (!IsFirstRow)
                mIsBodyRowOccurred = true;

            if (mIsFirstRow)
                mHeadingRowCount = mRowIndex + 1;

            if (mIsApplyRowBand)
            {
                mIsFirstRowInBand = IsFirstInBand(DataRowIndex, mRowBandSize);
                mIsLastRowInBand = IsLastInBand(DataRowIndex, mRowBandSize);
            }

            mCellIndex = 0;
            mIsFirstCol = false;
            mIsLastCol = false;

            TablePr newRowPr = new TablePr();

            // Whole table formatting.
            mStyleTablePr.ExpandTo(newRowPr);
            mStyleRowPr.ExpandTo(newRowPr);

            // Conditional formatting.
            mRowPrExpander.Expand(this, newRowPr);

            // It seems that this attribute is not inheritable.
            newRowPr.Remove(TableAttr.Hidden);

            // Direct formatting.
            row.TablePr.ExpandTo(newRowPr);

            newRowPr.SysDirectAttrs = row.TablePr.Clone();
            mAffectedNodes.Add(row);

            row.TablePr = newRowPr;
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitRowEnd(Row row)
        {
            mRowIndex++;
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitCellStart(Cell cell)
        {
            mIsFirstCol = cell.IsFirstCell;
            mIsLastCol = cell.IsLastCell;

            if (mIsApplyColBand)
            {
                mIsFirstColInBand = IsFirstInBand(DataColIndex, mColBandSize);
                mIsLastColInBand = IsLastInBand(DataColIndex, mColBandSize);
            }

            CellPr newCellPr = new CellPr();

            // Whole table formatting.
            mStyleCellPr.ExpandTo(newCellPr);

            // WORDSNET-28945 Override inherited cell borders by table borders.
            OverrideInheritedCellBorders(cell, newCellPr);

            // Conditional formatting.
            mCellPrExpander.Expand(this, newCellPr);

            // Direct formatting.
            cell.CellPr.ExpandTo(newCellPr);

            newCellPr.SysDirectAttrs = cell.CellPr.Clone();
            mAffectedNodes.Add(cell);

            cell.CellPr = newCellPr;
            return VisitorAction.Continue;
        }

        public override VisitorAction VisitCellEnd(Cell cell)
        {
            mCellIndex++;
            return VisitorAction.Continue;
        }

        private static bool IsFirstInBand(int index, int bandSize)
        {
            int bandIndex = index / bandSize;
            return ((bandIndex * bandSize) == index);
        }

        private static bool IsLastInBand(int index, int bandSize)
        {
            int bandIndex = index / bandSize;
            return (((bandIndex * bandSize + bandSize - 1) == index));
        }

        public override VisitorAction VisitParagraphStart(Paragraph para)
        {
            // I think paragraph formatting from the table style should apply only to the paragraphs
            // that are inside the cell and not to paragraphs that are inside comments or footnotes
            // that might occur in a table.
            if (para.FirstNonMarkupParentNode.NodeType != NodeType.Cell)
                return VisitorAction.SkipThisNode;

            ExpandParaPr(para);
            ExpandParagraphBreakRunPr(para);

            return VisitorAction.Continue;
        }

        private void ExpandParaPr(Paragraph para)
        {
            ParaPr newParaPr = new ParaPr();

            // Whole table formatting.
            mStyleParaPr.ExpandTo(newParaPr);

            // Conditional formatting.
            mParaPrExpander.Expand(this, newParaPr);

            // Save direct attrs.
            para.ParaPr.SysDirectAttrs = para.ParaPr.Clone();
            mAffectedNodes.Add(para);

            // This is the "normal" expansion procedure for a paragraph that is not in a table.
            para.ExpandParaPr(newParaPr, ParaPrExpandFlags.Normal);

            para.ParaPr = newParaPr;
        }

        /// <summary>
        /// RK It's a pity this code is almost a complete duplication of <see cref="ExpandRunPr"/> below,
        /// but at the moment it seems like too hard to refactor this while making it easier to read.
        /// </summary>
        private void ExpandParagraphBreakRunPr(Paragraph para)
        {
            RunPr newRunPr = para.Document.Styles.DefaultRunPr.Clone();

            // Remove attributes which are inherited from global defaults to reduce affected golds.
            newRunPr.RemoveGlobalDefaults();

            // Whole table formatting.
            mStyleRunPr.ExpandTo(newRunPr);

            // Conditional formatting.
            mRunPrExpander.Expand(this, newRunPr);

            // Save direct attrs. Don't add this para to ExpandedNodes array because it was added in ExpandParaPr.
            para.ParagraphBreakRunPr.SysDirectAttrs = para.ParagraphBreakRunPr.Clone();

            // WORDSNET-24396 When there is no revision in direct properties, it still can be found in
            // inherited ones. This means we should use Revised flag, otherwise we will get wrong
            // attributes from the original properties.
            RunPrExpandFlags expFlag = RunPrExpandFlags.NoTableStyle;
            if (!para.ParagraphBreakRunPr.HasFormatRevision)
                expFlag |= RunPrExpandFlags.Revised;

            // Paragraph and character styles and direct formatting.
            // Table style is already expanded so use appropriate flag.
            para.ExpandParagraphBreakRunPr(newRunPr, expFlag);

            // WORDSNET-24396 Apply formatting revision just as we do it already
            // in the related WORDSNET-23975 and WORDSNET-24237 in ExpandRunPr().
            if (newRunPr.HasFormatRevision)
            {
                RunPr revRunPr = (RunPr)newRunPr.FormatRevision.RevPr;
                para.ExpandParagraphBreakRunPr(revRunPr, expFlag | RunPrExpandFlags.Revised);
                revRunPr.Collapse(para.ParagraphBreakRunPr);
                revRunPr.RemoveIgnorableOnCollapseValues();
            }

            para.ParagraphBreakRunPr = newRunPr;
        }

        private void ExpandRunPr(Run run)
        {
            RunPr newRunPr = run.Document.Styles.DefaultRunPr.Clone();

            // Remove attributes which are inherited from global defaults to reduce affected golds.
            newRunPr.RemoveGlobalDefaults();

            // Whole table formatting.
            mStyleRunPr.ExpandTo(newRunPr);

            // Conditional formatting.
            mRunPrExpander.Expand(this, newRunPr);

            // Save direct attrs.
            run.RunPr.SysDirectAttrs = run.RunPr.Clone();
            mAffectedNodes.Add(run);

            // Paragraph and character styles and direct formatting.
            // Table style is already expanded so use appropriate flag.
            RunPrExpandFlags expFlag = RunPrExpandFlags.NoTableStyle;
            // WORDSNET-16276 Word ignores the "Hyperlink" style when the run locates inside TOC field.
            if (mIsInTocField)
                expFlag |= RunPrExpandFlags.IgnoreHyperlinkCharStyle;

            // WORDSNET-24396 When there is no revision in direct properties, it still can be found in
            // inherited ones. This means we should use Revised flag, otherwise we will get wrong
            // attributes from the original properties.
            if (!run.RunPr.HasFormatRevision)
                expFlag |= RunPrExpandFlags.Revised;

            run.ExpandRunPr(newRunPr, expFlag);

            // WORDSNET-23975 Apply formatting revision.
            if (newRunPr.HasFormatRevision)
            {
                RunPr revRunPr = (RunPr)newRunPr.FormatRevision.RevPr;
                run.ExpandRunPr(revRunPr, expFlag | RunPrExpandFlags.Revised);

                // WORDSNET-24237 Need to collapse revised properties here.
                revRunPr.Collapse(run.RunPr);
                revRunPr.RemoveIgnorableOnCollapseValues();
            }

            run.RunPr = newRunPr;
        }

        /// <summary>
        /// Calculates which parts of table the style should be applied to. Init related flag variables.
        /// </summary>
        private void CalcPartsToApply(TableStyleOptions options)
        {
            mIsApplyFirstRow = ((options & TableStyleOptions.FirstRow) != 0);
            mIsApplyLastRow = ((options & TableStyleOptions.LastRow) != 0);
            mIsApplyFirstCol = ((options & TableStyleOptions.FirstColumn) != 0);
            mIsApplyLastCol = ((options & TableStyleOptions.LastColumn) != 0);
            mIsApplyRowBand = ((options & TableStyleOptions.RowBands) != 0) && (mRowBandSize > 0);
            mIsApplyColBand = ((options & TableStyleOptions.ColumnBands) != 0) && (mColBandSize > 0);

            // WORDSNET-27004 Despite of table can have StyleLook with FirstRow, LastRow, etc turned on
            // table style itself can have no formatting defined for these conditions.
            // Word treats it as formatting is off for the condition.
            // Note we should apply corner cells as recently i.e relay on TableStyleLook.

            // First preserve "corners" look. It should be applied regardless conditional formatting are present for FirstRow, FirstCol etc.
            mIsApplyNW = mIsApplyFirstRow && mIsApplyFirstCol;
            mIsApplyNE = mIsApplyFirstRow && mIsApplyLastCol;
            mIsApplySW = mIsApplyLastRow && mIsApplyFirstCol;
            mIsApplySE = mIsApplyLastRow && mIsApplyLastCol;

            // WORDSNET-21775 If 'Use Word 2002 Table Style Rules' option is on,
            // table style for the nwCell should not be applied,
            // despite the firstRow and the firstColumn is enabled in the tblLook.
            if (mTable.Document.FetchDocumentOrGlossaryMain().CompatibilityOptions.UseWord2002TableStyleRules)
                mIsApplyNW = false;

            // Then consider presence of conditional formatting defined.
            mIsApplyFirstRow &= GetConditionalStyle(TableStyleOverrideType.FirstRow) != null;
            mIsApplyLastRow &= GetConditionalStyle(TableStyleOverrideType.LastRow) != null;
            mIsApplyFirstCol &= GetConditionalStyle(TableStyleOverrideType.FirstColumn) != null;
            mIsApplyLastCol &= GetConditionalStyle(TableStyleOverrideType.LastColumn) != null;
        }

        public override VisitorAction VisitRun(Run run)
        {
            ExpandRunPr(run);
            return VisitorAction.Continue;
        }

        /// <summary>
        /// Means that it is a first column now and we need to apply first column formatting.
        /// </summary>
        internal bool IsFirstColNow
        {
            get { return mIsApplyFirstCol && mIsFirstCol; }
        }

        internal bool IsLastColNow
        {
            get { return mIsApplyLastCol && mIsLastCol; }
        }

        internal bool IsFirstRowNow
        {
            get { return mIsApplyFirstRow && mIsFirstRow; }
        }

        internal bool IsLastRowNow
        {
            get { return mIsApplyLastRow && mIsLastRow; }
        }

        internal bool IsRowBandNow
        {
            get { return mIsApplyRowBand && !IsFirstRowNow && !IsLastRowNow; }
        }

        internal bool IsColBandNow
        {
            get { return mIsApplyColBand && !IsFirstColNow && !IsLastColNow; }
        }

        internal bool IsNWCellNow
        {
            get { return mIsApplyNW && IsFirstRow && IsFirstCol; }
        }

        internal bool IsNECellNow
        {
            get { return mIsApplyNE && IsFirstRow && IsLastCol; }
        }

        internal bool IsSWCellNow
        {
            get { return mIsApplySW && IsLastRow && IsFirstCol; }
        }

        internal bool IsSECellNow
        {
            get { return mIsApplySE && IsLastRow && IsLastCol; }
        }

        /// <summary>
        /// True when we are in the first column.
        /// </summary>
        internal bool IsFirstCol
        {
            get { return mIsFirstCol; }
        }

        internal bool IsLastCol
        {
            get { return mIsLastCol; }
        }

        internal bool IsFirstRow
        {
            get { return mIsFirstRow; }
        }

        internal bool IsLastRow
        {
            get { return mIsLastRow; }
        }


        internal bool IsFirstRowInBand
        {
            get { return mIsFirstRowInBand; }
        }

        internal bool IsLastRowInBand
        {
            get { return mIsLastRowInBand; }
        }

        internal bool IsFirstColInBand
        {
            get { return mIsFirstColInBand; }
        }

        internal bool IsLastColInBand
        {
            get { return mIsLastColInBand; }
        }


        internal int RowBandSize
        {
            get { return mRowBandSize; }
        }

        internal int ColBandSize
        {
            get { return mColBandSize; }
        }


        /// <summary>
        /// Gets the row index excluding header rows.
        /// </summary>
        private int DataRowIndex
        {
            // MS Word starts applying Odd/Even Banded Rows styles to rows located after header rows.
            // WORDSNET-20751, 21796. MS Word excludes rows marked as repeated on every new page (mIsHeadingRow) from banding
            // regardless of whether the First Row conditional style is enabled or not. Do the same.
            get { return (mIsApplyFirstRow || mIsHeadingRow) ? mRowIndex - mHeadingRowCount : mRowIndex; }
        }

        private int DataColIndex
        {
            get { return (mIsApplyFirstCol) ? mCellIndex - 1 : mCellIndex; }
        }


        internal TableStyleOverrideType RowBandNow
        {
            get
            {
                bool isOdd = MathUtil.IsOdd(DataRowIndex / mRowBandSize);
                return (isOdd) ? TableStyleOverrideType.EvenRowBanding : TableStyleOverrideType.OddRowBanding;
            }
        }

        internal TableStyleOverrideType ColBandNow
        {
            get
            {
                bool isOdd = MathUtil.IsOdd(DataColIndex / mColBandSize);
                return (isOdd) ? TableStyleOverrideType.EvenColumnBanding : TableStyleOverrideType.OddColumnBanding;
            }
        }


        internal ConditionalStyle GetConditionalStyle(TableStyleOverrideType type)
        {
            return mConditionalStyles[(int)type];
        }

        internal Theme Theme
        {
            get { return mTheme; }
        }

        /// <summary>
        /// Gets an array of attribute values with the keys from <see cref="gAutoFitGridCalculatedCellAttrs"/>
        /// </summary>
        private object[] SaveDirectAutoFitCalculatedAttrs(WordAttrCollection cellPr, int[] attrKeys)
        {
            object[] savedAttrs = new object[attrKeys.Length];

            for (int i = 0; i < attrKeys.Length; ++i)
                savedAttrs[i] = cellPr.GetDirectAttr(attrKeys[i]);

            return savedAttrs;
        }

        /// <summary>
        /// Writes the given values of cell attributes to the cell properties.
        /// </summary>
        /// <remarks>
        /// The given values must match the keys in <see cref="gAutoFitGridCalculatedCellAttrs"/>;
        /// </remarks>
        private static void RestoreDirectAutoFitCalculatedAttrs(WordAttrCollection modelAttrs, object[] savedAttrs, int[] savedAttrKeys)
        {
            Debug.Assert(savedAttrs != null && savedAttrs.Length == savedAttrKeys.Length);

            for (int i = 0; i < savedAttrKeys.Length; ++i)
            {
                int attrKey = savedAttrKeys[i];
                object attrValue = savedAttrs[i];
                if (attrValue == null)
                    modelAttrs.Remove(attrKey);
                else
                    modelAttrs.SetAttr(attrKey, attrValue);
            }
        }

        /// <summary>
        /// Overrides inherited cell borders by table borders.
        /// </summary>
        private static void OverrideInheritedCellBorders(Cell cell, CellPr newCellPr)
        {
            Row row = cell.ParentRow;

            // Directly specified table borders are backed up at this moment.
            TablePr tablePr = (TablePr)row.TablePr.SysDirectAttrs;

            OverrideInheritedCellBorder(tablePr[row.IsFirstRow ? TableAttr.BorderTop : TableAttr.BorderHorizontal], newCellPr, CellAttr.BorderTop);
            OverrideInheritedCellBorder(tablePr[row.IsLastRow ? TableAttr.BorderBottom : TableAttr.BorderHorizontal], newCellPr, CellAttr.BorderBottom);
            OverrideInheritedCellBorder(tablePr[cell.IsFirstCell ? TableAttr.BorderLeft : TableAttr.BorderVertical], newCellPr, CellAttr.BorderLeft);
            OverrideInheritedCellBorder(tablePr[cell.IsLastCell ? TableAttr.BorderRight : TableAttr.BorderVertical], newCellPr, CellAttr.BorderRight);
        }

        /// <summary>
        /// Overrides inherited cell border by according table border.
        /// </summary>
        private static void OverrideInheritedCellBorder(object tableBorder, CellPr cellPr, int cellKey)
        {
            if(tableBorder == null)
                return;

            // It looks that MS Word only overrides existing cell borders.
            Border cellBorder = (Border)cellPr[cellKey];
            if (cellBorder == null)
                return;

            cellPr[cellKey] = ((Border)tableBorder).Clone();
        }

        /// <summary>
        /// Cell attributes that may be changed during auto-fit grid calculation.
        /// </summary>
        /// <remarks>
        /// Auto-fit table grids are calculated during layout updates, because content metrics are only available during layout update.
        /// As a result of grid re-calculation, different cell grid span and cell values may be computed and applied to the model.
        /// These values should not be overwritten with values saved before layout update.
        /// </remarks>
        private static readonly int[] gAutoFitGridCalculatedCellAttrs = new int[] { CellAttr.Sys_CellSpan, CellAttr.Width, };
        private static readonly int[] gAutoFitGridCalculatedRowAttrs =
            new int[] { TableAttr.Sys_GridBefore, TableAttr.WidthBefore, TableAttr.Sys_GridAfter, TableAttr.WidthAfter, };

        private Table mTable;
        private Theme mTheme;

        private TablePr mStyleTablePr;
        private TablePr mStyleRowPr;
        private CellPr mStyleCellPr;
        private ParaPr mStyleParaPr;
        private RunPr mStyleRunPr;

        private ConditionalStyle[] mConditionalStyles;

        private int mRowBandSize;
        private int mColBandSize;

        private bool mIsApplyFirstRow;
        private bool mIsApplyLastRow;
        private bool mIsApplyFirstCol;
        private bool mIsApplyLastCol;
        private bool mIsApplyRowBand;
        private bool mIsApplyColBand;
        private bool mIsHeadingRow;

        /// <summary>
        /// Corner cells.
        /// </summary>
        private bool mIsApplyNW;
        private bool mIsApplyNE;
        private bool mIsApplySW;
        private bool mIsApplySE;

        private int mRowIndex;
        private bool mIsFirstRow;
        private bool mIsLastRow;
        private bool mIsFirstRowInBand;
        private bool mIsLastRowInBand;

        private int mCellIndex;
        private bool mIsFirstCol;
        private bool mIsLastCol;
        private bool mIsFirstColInBand;
        private bool mIsLastColInBand;

        /// <summary>
        /// Heading rows count in current table.
        /// </summary>
        private int mHeadingRowCount = 1;

        /// <summary>
        /// Indicates that at least one body row was processed.
        /// </summary>
        private bool mIsBodyRowOccurred = false;

        /// <summary>
        /// Indicates that currently the visitor running inside TOC field.
        /// </summary>
        private bool mIsInTocField;

        private readonly CnfExpanderRowPr mRowPrExpander = new CnfExpanderRowPr();
        private readonly CnfExpanderCellPr mCellPrExpander = new CnfExpanderCellPr();
        private readonly CnfExpanderParaPr mParaPrExpander = new CnfExpanderParaPr();
        private readonly CnfExpanderRunPr mRunPrExpander = new CnfExpanderRunPr();

        /// <summary>
        /// Provides quick access to nodes affected by this expander.
        /// </summary>
        private readonly List<Node> mAffectedNodes = new List<Node>();
        private static readonly int gTableStyleOverrideTypeCount =
            EnumUtilPal.GetEffectiveArrayLength(TableStyleOverrideType.OddRowBanding.GetType(), 14);
    }
}
