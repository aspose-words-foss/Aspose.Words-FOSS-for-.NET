// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/06/2009 by Roman Korchagin

using System;
using System.Collections.Generic;
using Aspose.Words.Tables;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a table style.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-tables/">Working with Tables</a> documentation article.</para>
    /// </summary>
    /// <dev>
    /// It is interesting to note that all attribute collections in the style are not supposed
    /// to contain attributes that reference another style. E.g. TablePr of the style should not
    /// contain Istd of a table style. It is actually a good thing. The only problem is that we
    /// do not validate it and maybe reading from some formats could leave Istd in these collections.
    /// </dev>
    public class TableStyle : Style, ICellAttrSource, IRowAttrSource, IBorderAttrSource, IShadingAttrSource
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal TableStyle() : base(StyleType.Table)
        {
        }

        /// <summary>
        /// See <see cref="Style.Clone"/> for more info.
        /// </summary>
        internal override Style Clone()
        {
            TableStyle lhs = (TableStyle)base.Clone();

            CloneProperties(this, lhs);

            return lhs;
        }

        /// <summary>
        /// Clones table style properties.
        /// </summary>
        internal static void CloneProperties(TableStyle src, TableStyle dst)
        {
            if (src.TablePr != null)
                dst.TablePr = src.TablePr.Clone();

            if (src.RowPr != null)
                dst.RowPr = src.RowPr.Clone();

            if (src.CellPr != null)
                dst.CellPr = src.CellPr.Clone();

            if (src.mConditionalStyles != null)
            {
                dst.mConditionalStyles = new ConditionalStyleCollection(dst);
                foreach (ConditionalStyle conditionalStyle in src.mConditionalStyles.DefinedStyles)
                    dst.ConditionalStyles.Add(conditionalStyle.Clone());
            }
        }

        internal void AddConditionalStyle(ConditionalStyle conditionalStyle)
        {
            if (!ConditionalStyles.ContainsTableStyleOverride(conditionalStyle.OverrideType))
                ConditionalStyles.Add(conditionalStyle);
        }

        internal void ClearConditionalStyles()
        {
            if (mConditionalStyles != null)
                mConditionalStyles.Clear();
        }

        internal TablePr GetExpandedTablePr()
        {
            TablePr dstTablePr = new TablePr();
            ExpandTablePr(dstTablePr);
            return dstTablePr;
        }

        /// <summary>
        /// Expands table style paragraph formatting including conditional formatting.
        /// </summary>
        internal void ExpandParaPr(Cell cell, ParaPr dstParaPr)
        {
            TableStyle baseStyle = (TableStyle)GetBaseStyle();
            if (baseStyle != null)
                baseStyle.ExpandParaPr(cell, dstParaPr);

            // Expand whole formatting first.
            ParaPr.ExpandTo(dstParaPr);

            // Expand conditional formatting.
            ConditionalStyle conditionalStyle = NextTableStylePrDesc(null);
            while (conditionalStyle != null)
            {
                if ((conditionalStyle.ParaPr != null) && IsApplicable(cell, conditionalStyle))
                    conditionalStyle.ParaPr.ExpandTo(dstParaPr);

                conditionalStyle = NextTableStylePrDesc(conditionalStyle);
            }
        }

        /// <summary>
        /// Expands table style font formatting including conditional formatting.
        /// </summary>
        internal void ExpandRunPr(Cell cell, RunPr dstRunPr)
        {
            TableStyle baseStyle = (TableStyle)GetBaseStyle();
            if (baseStyle != null)
                baseStyle.ExpandRunPr(cell, dstRunPr);

            // Expand whole formatting first.
            RunPr.ExpandTo(dstRunPr);

            // Expand conditional formatting.
            ConditionalStyle conditionalStyle = NextTableStylePrDesc(null);
            while (conditionalStyle != null)
            {
                if ((conditionalStyle.RunPr != null) && IsApplicable(cell, conditionalStyle))
                    conditionalStyle.RunPr.ExpandTo(dstRunPr);

                conditionalStyle = NextTableStylePrDesc(conditionalStyle);
            }
        }

        private ConditionalStyle NextTableStylePrDesc(ConditionalStyle tableStylePr)
        {
            int startFrom = (tableStylePr == null) ? 0 : Array.IndexOf(gFetchOrderDesc, tableStylePr.OverrideType) + 1;

            // Look for first occurred item in fetch order.
            for (int i = startFrom; i < gFetchOrderDesc.Length; i++)
            {
                if (ConditionalStyles.ContainsTableStyleOverride(gFetchOrderDesc[i]))
                    return ConditionalStyles[gFetchOrderDesc[i]];
            }

            return null;
        }

        private void ExpandTablePr(TablePr dstTablePr)
        {
            TableStyle baseStyle = (TableStyle)GetBaseStyle();
            if (baseStyle != null)
                baseStyle.ExpandTablePr(dstTablePr);

            if (mTablePr != null)
                mTablePr.ExpandTo(dstTablePr);
        }

        internal TablePr GetExpandedRowPr()
        {
            TablePr dstRowPr = new TablePr();
            ExpandRowPr(dstRowPr);
            return dstRowPr;
        }

        private void ExpandRowPr(TablePr dstRowPr)
        {
            TableStyle baseStyle = (TableStyle)GetBaseStyle();
            if (baseStyle != null)
                baseStyle.ExpandRowPr(dstRowPr);

            if (mRowPr != null)
                mRowPr.ExpandTo(dstRowPr);
        }

        internal CellPr GetExpandedCellPr()
        {
            CellPr dstCellPr = new CellPr();
            ExpandCellPr(dstCellPr);
            return dstCellPr;
        }

        /// <summary>
        /// Returns expanded conditional formatting of given type.
        /// </summary>
        internal ConditionalStyle GetExpandedConditionalStyle(TableStyleOverrideType type)
        {
            TableStyle baseStyle = (TableStyle)GetBaseStyle();

            ConditionalStyle expandedConditionalStyle = null;

            if (baseStyle != null)
                expandedConditionalStyle = baseStyle.GetExpandedConditionalStyle(type);

            if (expandedConditionalStyle == null)
                expandedConditionalStyle = new ConditionalStyle(type);

            if (ConditionalStyles.ContainsTableStyleOverride(type))
            {
                ConditionalStyle thisConditionalStyle = ConditionalStyles[type];
                if (thisConditionalStyle.HasRunFormatting)
                {
                    thisConditionalStyle.RunPr.ExpandTo(expandedConditionalStyle.RunPr);
                    thisConditionalStyle.RunPr.ThemeColorInheritanceHack(expandedConditionalStyle.RunPr);
                }

                if (thisConditionalStyle.HasCellFormatting)
                    thisConditionalStyle.CellPr.ExpandTo(expandedConditionalStyle.CellPr);

                if (thisConditionalStyle.HasParagraphFormatting)
                    thisConditionalStyle.ParaPr.ExpandTo(expandedConditionalStyle.ParaPr);

                if (thisConditionalStyle.HasTableFormatting)
                    thisConditionalStyle.TablePr.ExpandTo(expandedConditionalStyle.TablePr);

                if (thisConditionalStyle.HasRowFormatting)
                    thisConditionalStyle.RowPr.ExpandTo(expandedConditionalStyle.RowPr);
            }

            return expandedConditionalStyle;
        }

        internal void ExpandCellPr(Cell cell, CellPr dstCellPr)
        {
            ExpandCellPr(dstCellPr);

            // Expand conditional formatting.
            ConditionalStyle conditionalStyle = NextTableStylePrDesc(null);
            while (conditionalStyle != null)
            {
                if ((conditionalStyle.CellPr != null) && IsApplicable(cell, conditionalStyle))
                    conditionalStyle.CellPr.ExpandTo(dstCellPr);

                conditionalStyle = NextTableStylePrDesc(conditionalStyle);
            }
        }

        private void ExpandCellPr(CellPr dstCellPr)
        {
            TableStyle baseStyle = (TableStyle)GetBaseStyle();
            if (baseStyle != null)
                baseStyle.ExpandCellPr(dstCellPr);

            if (mCellPr != null)
                mCellPr.ExpandTo(dstCellPr);
        }

        #region ICellAttrSource

        object ICellAttrSource.GetDirectCellAttr(int key)
        {
            // AM. Word ignores borders defined in table style CellPr
            if (CellPr.PossibleBorderKeys.ContainsValue(key))
                return null;

            return mCellPr[key];
        }

        object ICellAttrSource.FetchCellAttr(int key)
        {
            object value = mCellPr[key];
            return (value != null) ? value : ((ICellAttrSource)this).FetchInheritedCellAttr(key);
        }

        object ICellAttrSource.FetchInheritedCellAttr(int key)
        {
            TableStyle baseStyle = GetBaseStyle() as TableStyle;

            return (baseStyle != null) ? ((ICellAttrSource)baseStyle).FetchCellAttr(key) : CellPr.FetchDefaultAttr(key);
        }

        void ICellAttrSource.SetCellAttr(int key, object value)
        {
            mCellPr.SetAttr(key, value);
        }

        void ICellAttrSource.ClearCellAttrs()
        {
            mCellPr.Clear();
        }

        #endregion

        #region IRowAttrSource

        object IRowAttrSource.GetDirectRowAttr(int key)
        {
            // WORDSNET-12776
            // At the moment we don't have interface to get value from TablePr. And we use IRowAttrSource to get values from TablePr.
            // Added RowPr to get all style attribute.
            object value = mRowPr[key];
            return (value != null) ? value : mTablePr[key];
        }

        object IRowAttrSource.FetchRowAttr(int key)
        {
            object value = ((IRowAttrSource)this).GetDirectRowAttr(key);
            return (value != null) ? value : ((IRowAttrSource)this).FetchInheritedRowAttr(key);
        }

        object IRowAttrSource.FetchInheritedRowAttr(int key)
        {
            TableStyle baseStyle = GetBaseStyle() as TableStyle;
            return (baseStyle != null) ? ((IRowAttrSource)baseStyle).FetchRowAttr(key) : TablePr.FetchDefaultAttr(key);
        }

        void IRowAttrSource.SetRowAttr(int key, object value)
        {
            mTablePr[key] = value;
        }

        void IRowAttrSource.ClearRowAttrs()
        {
            // WORDSNET-12776
            // Added clearing both objects, because we get values from them.
            mRowPr.Clear();
            mTablePr.Clear();
        }

        void IRowAttrSource.ResetToDefaultAttrs()
        {
            // WORDSNET-12776
            // Added clearing both objects, because we get values from them.
            mRowPr.Clear();
            mTablePr.Clear();
        }

        #endregion

        #region IBorderAttrSource

        object IBorderAttrSource.GetDirectBorderAttr(int key)
        {
            return mTablePr[key];
        }

        object IBorderAttrSource.FetchInheritedBorderAttr(int key)
        {
            TableStyle baseStyle = GetBaseStyle() as TableStyle;
            if (baseStyle != null)
            {
                object value = ((IBorderAttrSource)baseStyle).GetDirectBorderAttr(key);
                if (value != null)
                    return value;

                return ((IBorderAttrSource)baseStyle).FetchInheritedBorderAttr(key);
            }

            return TablePr.FetchDefaultAttr(key);
        }

        void IBorderAttrSource.SetBorderAttr(int key, object value)
        {
            mTablePr.SetAttr(key, value);
        }

        SortedList<BorderType, int> IBorderAttrSource.PossibleBorderKeys
        {
            get { return TablePr.PossibleBorderKeys; }
        }

        #endregion

        #region IShadingAttrSource

        object IShadingAttrSource.FetchInheritedShadingAttr(int key)
        {
            TableStyle baseStyle = GetBaseStyle() as TableStyle;
            if (baseStyle != null)
            {
                object value = baseStyle.CellPr[key];
                if (value != null)
                    return value;

                return ((IShadingAttrSource)baseStyle).FetchInheritedShadingAttr(key);
            }

            return CellPr.FetchDefaultAttr(key);
        }

        #endregion

        /// <summary>
        /// Gets or sets a flag indicating whether text in a table row is allowed to split across a page break.
        /// </summary>
        /// <remarks>
        /// The default value is <c>true</c>.
        /// </remarks>
        public bool AllowBreakAcrossPages
        {
            get { return (bool)((IRowAttrSource)this).FetchRowAttr(TableAttr.AllowBreakAcrossPages); }
            set
            {
                mRowPr.SetAttr(TableAttr.AllowBreakAcrossPages, value);
                mTablePr.Remove(TableAttr.AllowBreakAcrossPages); // Word stores the attribute in trPr.
            }
        }

        /// <summary>
        /// Gets the collection of default cell borders for the style.
        /// </summary>
        public BorderCollection Borders
        {
            get
            {
                if (mBorders == null)
                    mBorders = new BorderCollection(this);
                return mBorders;
            }
        }

        /// <summary>
        /// Gets or sets the amount of space (in points) to add to the left of the contents of table cells.
        /// </summary>
        public double LeftPadding
        {
            get { return FetchPadding(TableAttr.LeftPadding, CellAttr.LeftPadding); }
            set { mTablePr.SetAttr(TableAttr.LeftPadding, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Gets or sets the amount of space (in points) to add to the right of the contents of table cells.
        /// </summary>
        public double RightPadding
        {
            get { return FetchPadding(TableAttr.RightPadding, CellAttr.RightPadding); }
            set { mTablePr.SetAttr(TableAttr.RightPadding, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Gets or sets the amount of space (in points) to add above the contents of table cells.
        /// </summary>
        public double TopPadding
        {
            get { return FetchPadding(TableAttr.TopPadding, CellAttr.TopPadding); }
            set { mTablePr.SetAttr(TableAttr.TopPadding, ConvertUtilCore.PointToTwip(value)); }
        }

        /// <summary>
        /// Gets or sets the amount of space (in points) to add below the contents of table cells.
        /// </summary>
        public double BottomPadding
        {
            get { return FetchPadding(TableAttr.BottomPadding, CellAttr.BottomPadding); }
            set { mTablePr.SetAttr(TableAttr.BottomPadding, ConvertUtilCore.PointToTwip(value)); }
        }

        // WORDSJAVA-2491 check both the Table and Cell attributes to find Paddings.
        private double FetchPadding(int tablePaddingAttr, int cellPaddingAttr)
        {
            if ((int)((IRowAttrSource)this).FetchRowAttr(tablePaddingAttr) != 0)
                return ConvertUtilCore.TwipToPoint((int)((IRowAttrSource)this).FetchRowAttr(tablePaddingAttr));
            else
                return ConvertUtilCore.TwipToPoint((int)((ICellAttrSource)this).FetchCellAttr(cellPaddingAttr));
        }

        /// <summary>
        /// Specifies the alignment for the table style.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="TableAlignment.Left"/>.
        /// </remarks>
        public TableAlignment Alignment
        {
            get { return (TableAlignment)((IRowAttrSource)this).FetchRowAttr(TableAttr.Alignment); }
            set
            {
                // Word stores the attribute in tblPr and trPr.
                mRowPr.SetAttr(TableAttr.Alignment, value);
                mTablePr.SetAttr(TableAttr.Alignment, value);
            }
        }

        /// <summary>
        /// Gets or sets the amount of space (in points) between the cells.
        /// </summary>
        public double CellSpacing
        {
            get
            {
                PreferredWidth cellSpacing = (PreferredWidth)((IRowAttrSource)this).FetchRowAttr(TableAttr.CellSpacing);
                return ConvertUtilCore.TwipToPoint(cellSpacing.ValueRaw);
            }
            set
            {
                // Word stores the attribute in tblPr and trPr.
                mRowPr.SetAttr(TableAttr.CellSpacing, PreferredWidth.FromPointsSafe(value));
                mTablePr.SetAttr(TableAttr.CellSpacing, PreferredWidth.FromPointsSafe(value));
            }
        }

        /// <summary>
        /// Gets or sets the value that represents the left indent of a table.
        /// </summary>
        public double LeftIndent
        {
            get { return ConvertUtilCore.TwipToPoint((int)((IRowAttrSource)this).FetchRowAttr(TableAttr.LeftIndent)); }
            set
            {
                mTablePr.SetAttr(TableAttr.LeftIndent, ConvertUtilCore.PointToTwip(value));
                mRowPr.Remove(TableAttr.LeftIndent); // Word stores the attribute in tblPr.
            }
        }

        /// <summary>
        /// Gets a <see cref="Words.Shading"/> object that refers to the shading formatting for table cells.
        /// </summary>
        public Shading Shading
        {
            get
            {
                Shading shading = (Shading)mCellPr[CellAttr.Shading];
                if (shading == null)
                {
                    TableStyle parent = this;
                    shading = new Shading(parent, CellAttr.Shading);
                    shading = CodePorting.Translator.Cs2Cpp.MemoryManagement.ExtendLifetime(shading, parent);
                    mCellPr.SetAttr(CellAttr.Shading, shading);
                }
                return shading;
            }
        }

        /// <summary>
        /// Specifies the vertical alignment for the cells.
        /// </summary>
        /// <remarks>
        /// The default value is <see cref="CellVerticalAlignment.Top"/>.
        /// </remarks>
        /// <dev>
        /// AM. We copy Word VBA here and do not expose CellFormat object.
        /// I think the reason is that CellFormat contains properties such as VerticalMerge which is not applicable
        /// to table style.
        /// </dev>
        public CellVerticalAlignment VerticalAlignment
        {
            get { return (CellVerticalAlignment)((ICellAttrSource)this).FetchCellAttr(CellAttr.VerticalAlignment); }
            set { mCellPr.SetAttr(CellAttr.VerticalAlignment, value); }
        }

        /// <summary>
        /// Gets or sets a number of rows to include in the banding when the style specifies odd/even row banding.
        /// </summary>
        public int RowStripe
        {
            get { return (int)((IRowAttrSource)this).FetchRowAttr(TableAttr.StyleRowBandSize); }
            set
            {
                mTablePr.SetAttr(TableAttr.StyleRowBandSize, value);
                mRowPr.Remove(TableAttr.StyleRowBandSize); // Word stores the attribute in tblPr.
            }
        }

        /// <summary>
        /// Gets or sets a number of columns to include in the banding when the style specifies odd/even columns banding.
        /// </summary>
        public int ColumnStripe
        {
            get { return (int)((IRowAttrSource)this).FetchRowAttr(TableAttr.StyleColBandSize); }
            set
            {
                mTablePr.SetAttr(TableAttr.StyleColBandSize, value);
                mRowPr.Remove(TableAttr.StyleColBandSize); // Word stores the attribute in tblPr.
            }
        }

        /// <summary>
        /// Collection of conditional styles that may be defined for this table style.
        /// </summary>
        public ConditionalStyleCollection ConditionalStyles
        {
            get
            {
                if (mConditionalStyles == null)
                    mConditionalStyles = new ConditionalStyleCollection(this);
                return mConditionalStyles;
            }
        }

        /// <summary>
        /// 2.7.5.4        Style Table Properties
        /// </summary>
        internal TablePr TablePr
        {
            get { return mTablePr; }
            set { mTablePr = value; }
        }

        /// <summary>
        /// 2.7.5.11    Style Table Row Properties
        /// </summary>
        internal TablePr RowPr
        {
            get { return mRowPr; }
            set { mRowPr = value; }
        }

        /// <summary>
        /// 2.7.5.8        Style Table Cell Properties
        /// </summary>
        internal CellPr CellPr
        {
            get { return mCellPr; }
            set { mCellPr = value; }
        }

        /// <summary>
        /// Returns attributes collection of a conditional style of the given type and PrType.
        /// Performs creation of collection if they don't exist.
        /// </summary>
        /// <returns>CellPr attribute collection.</returns>
        internal WordAttrCollection FetchConditionalStylePr(TableStyleOverrideType type, AttrCollectionType prType)
        {
            ConditionalStyle conditionalStyle = ConditionalStyles[type];

            switch (prType)
            {
                case AttrCollectionType.CellPr:
                    return conditionalStyle.CellPr;
                case AttrCollectionType.ParaPr:
                    return conditionalStyle.ParaPr;
                case AttrCollectionType.RunPr:
                    return conditionalStyle.RunPr;
                default:
                    throw new InvalidOperationException("Unknown AttrCollectionType value.");
            }
        }

        /// <summary>
        /// Returns true if there is at least one formatting attribute specified in this table style.
        /// </summary>
        internal override bool HasFormatting()
        {
            if (base.HasFormatting())
                return true;

            if (mTablePr.Count > 0)
                return true;
            if (mRowPr.Count > 0)
                return true;
            if (mCellPr.Count > 0)
                return true;

            if (HasConditionalFormatting)
                return true;

            return false;
        }

        /// <summary>
        /// Returns attribute value defined in this style considering conditional formatting.
        /// Does not fetch global defaults.
        /// Does not inherit table borders or paddings.
        /// </summary>
        internal object GetCellAttr(int key, Cell cell)
        {
            return GetAttr(key, cell, AttrCollectionType.CellPr);
        }

        /// <summary>
        /// Returns attribute value defined in this style considering conditional formatting.
        /// Does not fetch global defaults.
        /// </summary>
        internal object GetRunAttr(int key, Cell cell)
        {
            return GetAttr(key, cell, AttrCollectionType.RunPr);
        }

        /// <summary>
        /// Returns attribute value defined in this style considering conditional formatting.
        /// Does not fetch global defaults.
        /// </summary>
        internal object GetParaAttr(int key, Cell cell)
        {
            return GetAttr(key, cell, AttrCollectionType.ParaPr);
        }

        /// <summary>
        /// Returns attribute value defined in this style considering conditional formatting.
        /// Does not fetch global defaults.
        /// Does not inherit table borders or paddings.
        /// </summary>
        private object GetAttr(int key, Cell cell, AttrCollectionType type)
        {
            object value = null;
            WordAttrCollection pr;

            // Try conditional formatting first.
            if (HasConditionalFormatting)
            {
                ConditionalStyle conditionalStyle = NextConditionalStyle(null);
                while (conditionalStyle != null)
                {
                    pr = conditionalStyle.GetCollection(type);

                    if ((pr != null) && IsApplicable(cell, conditionalStyle))
                    {
                        // Before get property we might need to transform key.
                        // AM. Actually this is needed for cell border keys only but
                        // for simplicity reason lets do it always.
                        int newKey = TransformCellBorderKey(conditionalStyle.OverrideType, cell, key);

                        value = pr[newKey];

                        if (value != null)
                            return value;
                    }

                    conditionalStyle = NextConditionalStyle(conditionalStyle);
                }
            }

            // Get whole table formatting if no conditional formatting was found.
            pr = GetCollection(type);
            if (pr != null)
                value = pr[key];

            if (value != null)
                return value;

            TableStyle baseStyle = GetBaseStyle() as TableStyle;
            return (baseStyle != null) ? baseStyle.GetAttr(key, cell, type) : null;
        }

        /// <summary>
        /// Iterates through defined conditional styles in certain order.
        /// </summary>
        /// <remarks>
        /// If null is passed as parameter, the first conditional style is returned
        /// otherwise next one is returned or null if this is the last conditional style.
        /// </remarks>
        private ConditionalStyle NextConditionalStyle(ConditionalStyle conditionalStyle)
        {
            int startFrom = (conditionalStyle == null) ? 0 : Array.IndexOf(gFetchOrder, conditionalStyle.OverrideType) + 1;

            // Look for first occurred item in fetch order.
            for (int i = startFrom; i < gFetchOrder.Length; i++)
            {
                if (ConditionalStyles.ContainsTableStyleOverride(gFetchOrder[i]))
                    return ConditionalStyles[gFetchOrder[i]];
            }

            return null;
        }

        /// <summary>
        /// Checks if the conditional style can be applied to the given cell.
        /// </summary>
        internal bool IsApplicable(Cell cell, ConditionalStyle conditionalStyle)
        {
            Row row = cell.ParentRow;

            if ((row == null) || (row.TablePr == null))
                return false;

            bool isFirstRow = row.IsFirstRow || row.IsHeadingRow;
            bool isLastRow = row.IsLastRow;
            bool isFirstCell = cell.IsFirstCell;
            bool isLastCell = cell.IsLastCell;
            bool hasHeadingRow = row.ParentTable.FirstRow.IsHeadingRow;

            TableStyleOptions options = row.TablePr.StyleOptions;
            bool isApplyFirstRow = (options & TableStyleOptions.FirstRow) != 0;
            bool isApplyLastRow = (options & TableStyleOptions.LastRow) != 0;
            bool isApplyFirstCol = (options & TableStyleOptions.FirstColumn) != 0;
            bool isApplyLastCol = (options & TableStyleOptions.LastColumn) != 0;
            bool isApplyRowBand = (options & TableStyleOptions.RowBands) != 0;
            bool isApplyColBand = (options & TableStyleOptions.ColumnBands) != 0;

            int rowBandSize = (TablePr.StyleRowBandSize > 0) ? TablePr.StyleRowBandSize : 1;
            int colBandSize = (TablePr.StyleColBandSize > 0) ? TablePr.StyleColBandSize : 1;

            // Table has first row/column only if table style specifies conditional formatting for the first row.
            // WORDSNET-20751, 21796. MS Word excludes rows marked as repeated on every new page (hasHeadingRow) from banding
            // regardless of whether the First Row conditional style is enabled or not. Do the same.
            bool hasFirstRow = (isApplyFirstRow || hasHeadingRow) &&
                ConditionalStyles.ContainsTableStyleOverride(TableStyleOverrideType.FirstRow);
            bool hasFirstCol = isApplyFirstCol &&
                ConditionalStyles.ContainsTableStyleOverride(TableStyleOverrideType.FirstColumn);

            bool isOddRowBand = false, isOddColBand = false;
            TableStyleOverrideType conditionalStyleType = conditionalStyle.OverrideType;

            // WORDSNET-15746 Skip calculation of indexes of row and cell to avoid affect to performance.
            switch (conditionalStyleType)
            {
                case TableStyleOverrideType.OddRowBanding:
                case TableStyleOverrideType.EvenRowBanding:
                    isOddRowBand = MathUtil.IsOdd((row.RowIndex + (hasFirstRow ? 0 : 1)) / rowBandSize);
                    break;
                case TableStyleOverrideType.OddColumnBanding:
                case TableStyleOverrideType.EvenColumnBanding:
                    isOddColBand = MathUtil.IsOdd((cell.ColumnIndex + (hasFirstCol ? 0 : 1)) / colBandSize);
                    break;
                default:
                    break;
            }

            switch (conditionalStyleType)
            {
                case TableStyleOverrideType.BottomRightCell: return isApplyLastRow && isApplyLastCol && isLastRow && isLastCell;
                case TableStyleOverrideType.BottomLeftCell: return isApplyLastRow && isApplyFirstCol && isLastRow && isFirstCell;
                case TableStyleOverrideType.TopRightCell: return isApplyFirstRow && isApplyLastCol && isFirstRow && isLastCell;
                case TableStyleOverrideType.TopLeftCell: return isApplyFirstRow && isApplyFirstCol && isFirstRow && isFirstCell;
                case TableStyleOverrideType.LastRow: return isApplyLastRow && isLastRow;
                case TableStyleOverrideType.FirstRow: return isApplyFirstRow && isFirstRow;
                case TableStyleOverrideType.LastColumn: return isApplyLastCol && isLastCell;
                case TableStyleOverrideType.FirstColumn: return isApplyFirstCol && isFirstCell;
                case TableStyleOverrideType.OddRowBanding: return isApplyRowBand && isOddRowBand && (!isLastRow || !isApplyLastRow);
                case TableStyleOverrideType.EvenRowBanding: return isApplyRowBand && !isOddRowBand && (!isLastRow || !isApplyLastRow);
                case TableStyleOverrideType.OddColumnBanding: return isApplyColBand && isOddColBand;
                case TableStyleOverrideType.EvenColumnBanding: return isApplyColBand && !isOddColBand;
                default:
                    throw new InvalidOperationException("Unexpected TableStyleOverrideType value.");
            }
        }

        /// <summary>
        /// Returns <c>true</c> if any conditional formatting is defined in this style.
        /// </summary>
        internal bool HasConditionalFormatting
        {
            get
            {
                if (mConditionalStyles != null)
                {
                    foreach (ConditionalStyle conditionalStyle in mConditionalStyles.DefinedStyles)
                    {
                        if (conditionalStyle.HasFormatting)
                            return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Converts cell border key if needed.
        /// </summary>
        /// <remarks>
        /// For certain TableStyleOverrideTypes we need to apply horizontal/vertical borders instead
        /// of left/right borders if cell is not first/last in the row/column.
        /// </remarks>
        private static int TransformCellBorderKey(TableStyleOverrideType type, Cell cell, int key)
        {
            int newKey = key;
            switch (type)
            {
                case TableStyleOverrideType.FirstRow:
                case TableStyleOverrideType.LastRow:
                case TableStyleOverrideType.OddRowBanding:
                case TableStyleOverrideType.EvenRowBanding:
                    if ((key == CellAttr.BorderRight) && !cell.IsLastCell)
                        newKey = CellAttr.BorderVertical;
                    else if ((key == CellAttr.BorderLeft) && !cell.IsFirstCell)
                        newKey = CellAttr.BorderVertical;
                    break;

                case TableStyleOverrideType.FirstColumn:
                case TableStyleOverrideType.LastColumn:
                case TableStyleOverrideType.OddColumnBanding:
                case TableStyleOverrideType.EvenColumnBanding:
                    if ((key == CellAttr.BorderBottom) && !cell.ParentRow.IsLastRow)
                        newKey = CellAttr.BorderHorizontal;
                    else if ((key == CellAttr.BorderTop) && !cell.ParentRow.IsFirstRow)
                        newKey = CellAttr.BorderHorizontal;
                    break;

                default:
                    break;
            }

            return newKey;
        }

        /// <summary>
        /// Returns collection of given type.
        /// </summary>
        private WordAttrCollection GetCollection(AttrCollectionType type)
        {
            switch (type)
            {
                case AttrCollectionType.RunPr:
                    return RunPr;
                case AttrCollectionType.ParaPr:
                    return ParaPr;
                case AttrCollectionType.CellPr:
                    return CellPr;
                default:
                    throw new InvalidOperationException("Unexpected AttrCollectionType value.");
            }
        }

        private TablePr mTablePr = new TablePr();
        private TablePr mRowPr = new TablePr();
        private CellPr mCellPr = new CellPr();

        private BorderCollection mBorders;
        private ConditionalStyleCollection mConditionalStyles;

        /// <summary>
        /// Specifies order for attribute fetching.
        /// </summary>
        private static readonly TableStyleOverrideType[] gFetchOrder = new TableStyleOverrideType[]
                                                             {
                                                                 TableStyleOverrideType.BottomRightCell,
                                                                 TableStyleOverrideType.BottomLeftCell,
                                                                 TableStyleOverrideType.TopRightCell,
                                                                 TableStyleOverrideType.TopLeftCell,
                                                                 TableStyleOverrideType.LastRow,
                                                                 TableStyleOverrideType.FirstRow,
                                                                 TableStyleOverrideType.LastColumn,
                                                                 TableStyleOverrideType.FirstColumn,
                                                                 TableStyleOverrideType.OddRowBanding,
                                                                 TableStyleOverrideType.EvenRowBanding,
                                                                 TableStyleOverrideType.OddColumnBanding,
                                                                 TableStyleOverrideType.EvenColumnBanding
                                                             };

        private static readonly TableStyleOverrideType[] gFetchOrderDesc = new TableStyleOverrideType[]
        {
            TableStyleOverrideType.EvenColumnBanding,
            TableStyleOverrideType.OddColumnBanding,
            TableStyleOverrideType.EvenRowBanding,
            TableStyleOverrideType.OddRowBanding,
            TableStyleOverrideType.FirstColumn,
            TableStyleOverrideType.LastColumn,
            TableStyleOverrideType.FirstRow,
            TableStyleOverrideType.LastRow,
            TableStyleOverrideType.TopLeftCell,
            TableStyleOverrideType.TopRightCell,
            TableStyleOverrideType.BottomLeftCell,
            TableStyleOverrideType.BottomRightCell,
        };
    }
}
