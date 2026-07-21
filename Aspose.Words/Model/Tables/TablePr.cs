// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 17/05/2005 by Roman Korchagin

using System.Collections.Generic;
using Aspose.Drawing;
using Aspose.Words.Drawing;
using Aspose.Words.Revisions;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Collection of table row attributes that allows typed access to the values.
    /// </summary>
    internal class TablePr : WordAttrCollection, IRowAttrSource
    {
        /// <summary>
        /// Creates a table row attributes that have the values set to the values that MS Word uses when creating tables.
        /// </summary>
        internal static TablePr CreateMSWordLooking()
        {
            TablePr result = new TablePr();

            const int DefaultLeftAndRightCellPadding = 108;
            result.LeftPadding = DefaultLeftAndRightCellPadding;
            result.RightPadding = DefaultLeftAndRightCellPadding;

            result.AllowAutoFit = true;

            // Although MS Word does not actually set table preferred width to 100% on table creation, it sort of behaves that way.
            // We cannot directly emulate this behavior and hence the next best option is to set preferred width to 100% so
            // any table created by the user in AW will occupy the available width.
            result.PreferredWidth = PreferredWidth.FromPercent(100);

            // This is the default thin black border that MS Word creates.
            foreach (int key in PossibleBorderKeys.Values)
                result.SetAttr(key, new Border(LineStyle.Single, 4, DrColor.Empty));

            return result;
        }

        internal void RemoveBorders()
        {
            foreach (int key in PossibleBorderKeys.Values)
                Remove(key);
        }

        internal void RemoveShading()
        {
            Remove(TableAttr.Shading);
        }

        /// <summary>
        /// Style index.
        /// </summary>
        internal int Istd
        {
            get { return (int)FetchAttr(TableAttr.Istd); }
            set { SetAttr(TableAttr.Istd, value); }
        }

        internal int StyleRowBandSize
        {
            get { return (int)FetchAttr(TableAttr.StyleRowBandSize); }
        }

        internal int StyleColBandSize
        {
            get { return (int)FetchAttr(TableAttr.StyleColBandSize); }
        }

        /// <summary>
        /// Reverses a row alignment value.
        /// </summary>
        internal static TableAlignment ReverseAlignment(TableAlignment alignment)
        {
            switch (alignment)
            {
                case TableAlignment.Left:
                    return TableAlignment.Right;
                case TableAlignment.Right:
                    return TableAlignment.Left;
                default:
                    return alignment;
            }
        }

        /// <summary>
        /// Inverts RowBands and ColumnBands.
        /// </summary>
        internal static TableStyleOptions InvertTableBands(TableStyleOptions options)
        {
            return options ^ (TableStyleOptions.RowBands | TableStyleOptions.ColumnBands);
        }

        /// <summary>
        /// Returns true if the table properties have same absolute positioning as the specified properties.
        /// There are 11 floating table frame positioning attributes. This efficiently compares all of them if needed.
        /// </summary>
        internal bool IsSameFloatingPositioning(TablePr rhs)
        {
            bool isApo1 = IsFloating;
            bool isApo2 = rhs.IsFloating;

            // A quick way to get out if both are not absolutely positioned.
            if (!isApo1 && !isApo2)
                return true;

            // A quick way to get out if one is absolutely positioned and the other is not.
            if (isApo1 != isApo2)
                return false;

            // Now we have to compare all frame attributes thoroughly.
            if (AllowOverlap != rhs.AllowOverlap)
                return false;

            if (RelativeHorizontalPosition != rhs.RelativeHorizontalPosition)
                return false;

            if (RelativeVerticalPosition != rhs.RelativeVerticalPosition)
                return false;

            if (FrameLeft != rhs.FrameLeft)
                return false;

            if (FrameTop != rhs.FrameTop)
                return false;

            if (HorizontalAlignment != rhs.HorizontalAlignment)
                return false;

            if (VerticalAlignment != rhs.VerticalAlignment)
                return false;

            return true;
        }

        internal bool HasBorders
        {
            get
            {
                return
                    ContainsKey(TableAttr.BorderTop) ||
                    ContainsKey(TableAttr.BorderLeft) ||
                    ContainsKey(TableAttr.BorderBottom) ||
                    ContainsKey(TableAttr.BorderRight) ||
                    ContainsKey(TableAttr.BorderHorizontal) ||
                    ContainsKey(TableAttr.BorderVertical);
            }
        }

        internal Border BorderTop
        {
            get { return (Border)GetDirectAttr(TableAttr.BorderTop); }
        }

        internal Border BorderLeft
        {
            get { return (Border)GetDirectAttr(TableAttr.BorderLeft); }
        }

        internal Border BorderBottom
        {
            get { return (Border)GetDirectAttr(TableAttr.BorderBottom); }
        }

        internal Border BorderRight
        {
            get { return (Border)GetDirectAttr(TableAttr.BorderRight); }
        }

        internal Border BorderHorizontal
        {
            get { return (Border)GetDirectAttr(TableAttr.BorderHorizontal); }
        }

        internal Border BorderVertical
        {
            get { return (Border)GetDirectAttr(TableAttr.BorderVertical); }
        }

        internal Shading Shading
        {
            get { return (Shading)GetDirectAttr(TableAttr.Shading); }
            set { SetAttr(TableAttr.Shading, value); }
        }

        internal TableStyleOptions StyleOptions
        {
            get { return (TableStyleOptions)FetchAttr(TableAttr.StyleOptions); }
        }

        internal PreferredWidth PreferredWidth
        {
            get { return (PreferredWidth)FetchAttr(TableAttr.PreferredWidth); }
            set { SetAttr(TableAttr.PreferredWidth, value); }
        }

        internal bool AllowAutoFit
        {
            get { return (bool)FetchAttr(TableAttr.AllowAutoFit); }
            set { SetAttr(TableAttr.AllowAutoFit, value); }
        }

        internal TableAlignment Alignment
        {
            get { return (TableAlignment)FetchAttr(TableAttr.Alignment); }
            set { SetAttr(TableAttr.Alignment, value); }
        }

        internal bool Bidi
        {
            get { return (bool)FetchAttr(TableAttr.Bidi); }
            set { SetAttr(TableAttr.Bidi, value); }
        }

        internal int TopPadding
        {
            get { return (int)FetchAttr(TableAttr.TopPadding); }
            set { SetAttr(TableAttr.TopPadding, value); }
        }

        internal int BottomPadding
        {
            get { return (int)FetchAttr(TableAttr.BottomPadding); }
            set { SetAttr(TableAttr.BottomPadding, value); }
        }

        internal int LeftPadding
        {
            get { return (int)FetchAttr(TableAttr.LeftPadding); }
            set { SetAttr(TableAttr.LeftPadding, value); }
        }

        internal int RightPadding
        {
            get { return (int)FetchAttr(TableAttr.RightPadding); }
            set { SetAttr(TableAttr.RightPadding, value); }
        }

        /// <summary>
        /// Note this spacing must be added to both adjoining cells.
        /// </summary>
        internal PreferredWidth CellSpacing
        {
            get { return (PreferredWidth)FetchAttr(TableAttr.CellSpacing); }
        }

        internal HeightRule HeightRule
        {
            get { return ((Height)FetchAttr(TableAttr.RowHeight)).Rule; }
            set { ((Height)GetOrCreateComplexAttr(TableAttr.RowHeight)).Rule = value; }
        }

        internal int Height
        {
            get { return ((Height)FetchAttr(TableAttr.RowHeight)).Value; }
            set { ((Height)GetOrCreateComplexAttr(TableAttr.RowHeight)).Value = value; }
        }

        internal bool AllowBreakAcrossPages
        {
            get { return (bool)FetchAttr(TableAttr.AllowBreakAcrossPages); }
        }

        internal bool HeadingFormat
        {
            get { return (bool)FetchAttr(TableAttr.HeadingFormat); }
        }

        internal bool AllowOverlap
        {
            get { return (bool)FetchAttr(TableAttr.AllowOverlap); }
        }

        internal int FrameDistanceFromTop
        {
            get { return (int)FetchAttr(TableAttr.FrameDistanceFromTop); }
        }

        internal int FrameDistanceFromBottom
        {
            get { return (int)FetchAttr(TableAttr.FrameDistanceFromBottom); }
        }

        internal int FrameDistanceFromRight
        {
            get { return (int)FetchAttr(TableAttr.FrameDistanceFromRight); }
        }

        internal int FrameDistanceFromLeft
        {
            get { return (int)FetchAttr(TableAttr.FrameDistanceFromLeft); }
        }

        internal  bool HasNondefaultRelativeHorizontalPosition
        {
            get { return (ContainsKey(TableAttr.RelativeHorizontalPosition) && (RelativeHorizontalPosition != RelativeHorizontalPosition.Default)); }
        }

        /// <summary>
        /// Table Horizontal Anchor
        /// (floating table property).
        /// </summary>
        /// <remarks>should be subject to validation when exposed to public api. See ISO29500 and MS Impl Notes</remarks>
        internal RelativeHorizontalPosition RelativeHorizontalPosition
        {
            get { return (RelativeHorizontalPosition)FetchAttr(TableAttr.RelativeHorizontalPosition); }
            set { SetAttr(TableAttr.RelativeHorizontalPosition, value); }
        }

        internal bool HasNondefaultRelativeVerticalPosition
        {
            get { return (ContainsKey(TableAttr.RelativeVerticalPosition) && (RelativeVerticalPosition != RelativeVerticalPosition.TableDefault)); }
        }

        /// <summary>
        /// Table Vertical Anchor
        /// (floating table property).
        /// </summary>
        /// <remarks>should be subject to validation when exposed to public api. See ISO29500 and MS Impl Notes</remarks>
        internal RelativeVerticalPosition RelativeVerticalPosition
        {
            get { return (RelativeVerticalPosition)FetchAttr(TableAttr.RelativeVerticalPosition); }
            set { SetAttr(TableAttr.RelativeVerticalPosition, value); }
        }

        internal bool HasFrameTop
        {
            get { return ContainsKey(TableAttr.FrameTop); }
        }

        /// <summary>
        /// Absolute Vertical Distance From Anchor
        /// (floating table property).
        /// </summary>
        /// <remarks>should be subject to validation when exposed to public api. See ISO29500 and MS Impl Notes</remarks>
        internal int FrameTop
        {
            get { return (int)FetchAttr(TableAttr.FrameTop); }
            set { SetAttr(TableAttr.FrameTop, value); }
        }


        internal bool HasNondefaultVerticalAlignment
        {
            get { return (ContainsKey(TableAttr.VerticalAlignment) && (VerticalAlignment != VerticalAlignment.Default)); }
        }

        /// <summary>
        /// Relative Vertical Alignment from Anchor
        /// (floating table property).
        /// </summary>
        /// <remarks>should be subject to validation when exposed to public api. See ISO29500 and MS Impl Notes</remarks>
        internal VerticalAlignment VerticalAlignment
        {
            get { return (VerticalAlignment)FetchAttr(TableAttr.VerticalAlignment); }
        }

        internal bool HasFrameLeft
        {
            get { return ContainsKey(TableAttr.FrameLeft); }
        }

        /// <summary>
        /// Absolute Horizontal Distance From Anchor
        /// (floating table property).
        /// </summary>
        /// <remarks>should be subject to validation when exposed to public api. See ISO29500 and MS Impl Notes</remarks>
        internal int FrameLeft
        {
            get { return (int)FetchAttr(TableAttr.FrameLeft); }
            set { SetAttr(TableAttr.FrameLeft, value); }
        }

        internal bool HasNondefaultHorizontalAlignment
        {
            get { return (ContainsKey(TableAttr.HorizontalAlignment) && (HorizontalAlignment != HorizontalAlignment.Default)); }
        }

        /// <summary>
        /// Relative Horizontal Alignment From Anchor
        /// (floating table property).
        /// </summary>
        /// <remarks>should be subject to validation when exposed to public api. See ISO29500 and MS Impl Notes
        /// Dmatv: Per WORDSNET-15239, layout has a special condition to interpret default/missing horizontal alignment as Left
        /// when FrameLeft is not specified. This property has no such condition, so the reported value may differ from MS Word UI
        /// even if it matches the document xml data.
        /// </remarks>
        internal HorizontalAlignment HorizontalAlignment
        {
            get { return (HorizontalAlignment)FetchAttr(TableAttr.HorizontalAlignment); }
        }

        /// <summary>
        /// Returns true if this table should be positioned as floating frame.
        /// </summary>
        /// <remarks>
        /// DD: None of MS Specs define precise conditions for text frame to be floating,
        /// so this is experimentally derived conditions.
        ///
        /// Dmatv: MS Word ignores floating attributes for tables nested in shapes.
        /// To correctly detect if a table is floating, IsNested(table, NodeType.Shape) should be used in addition to this property.
        /// </remarks>
        internal bool IsFloating
        {
            get
            {
                // DD: The core idea of this logic is that any of the props below can have defaults, and if at least one floating-related property is present
                // then we can use defaults for other ones.
                // But if none are present or if all are defaults, then it is not a floating object.

                bool hasFloatingAttribute = HasFrameLeft ||
                    HasFrameTop ||
                    HasNondefaultRelativeHorizontalPosition ||
                    HasNondefaultHorizontalAlignment ||
                    HasNondefaultVerticalAlignment;

                // Dmatv: Per WODSNET-17857, a condition to treat "(vertical position is Page and everything else is default)" as not floating was added.
                return hasFloatingAttribute ||
                    (HasNondefaultRelativeVerticalPosition && (RelativeVerticalPosition != RelativeVerticalPosition.Page));
            }
        }

        /// <summary>
        /// Table left indent.
        /// </summary>
        internal int LeftIndent
        {
            get { return (int)FetchAttr(TableAttr.LeftIndent); }
            set { SetAttr(TableAttr.LeftIndent, value); }
        }

        /// <summary>
        /// Table grid calculated by the new algorithm.
        /// </summary>
        /// <remarks>
        /// The new algorithm currently does not support all possible cases.
        /// State property is set to Applied if the grid was calculated successfully.
        /// </remarks>
        internal TableGridAttr CalculatedGrid
        {
            get { return (TableGridAttr)GetDirectAttr(TableAttr.Sys_CalculatedTableGrid); }
            set { SetAttr(TableAttr.Sys_CalculatedTableGrid, value); }
        }

        /// <summary>
        /// "Grid before" the table row. The number of (empty) table grid columns before the first cell in the row.
        /// </summary>
        /// <remarks>
        /// Currently, it is only used by the "new" table grid calculation algorithm.
        /// </remarks>
        internal int GridBefore
        {
            get { return (int)FetchAttr(TableAttr.Sys_GridBefore); }
        }

        /// <summary>
        /// "Grid after" the table row. The number of (empty) table grid columns after the last cell in the row.
        /// </summary>
        /// <remarks>
        /// Currently, it is only used by the "new" table grid calculation algorithm.
        /// </remarks>
        internal int GridAfter
        {
            get { return (int)FetchAttr(TableAttr.Sys_GridAfter); }
        }

        /// <summary>
        /// Gets or sets <see cref="TableAttr.WidthBefore"/>
        /// </summary>
        /// <remarks>
        /// Dmatv: Returning a value with zero width instead of missing attribute is actually not correct.
        /// MS Word interprets zero and missing width before values differently.
        /// </remarks>
        internal PreferredWidth WidthBefore
        {
            get { return (PreferredWidth)FetchAttr(TableAttr.WidthBefore); }
            set { SetAttr(TableAttr.WidthBefore, value); }
        }

        /// <summary>
        /// This attribute is used to access the original "width before" stored in the document.
        /// </summary>
        /// <remarks>
        /// Dmatv: It is needed for the "new" table grid algorithm implemented by FixedGridCalculator to work.
        /// The "old" grid algorithm may replace the value read from the document based on the incorrect grid.
        /// An incorrect grid may be stored in the document,
        /// or it may be the result of a simplistic grid calculation implemented by the "old" algorithm for a missing grid.
        /// In any case, incorrect "width before" value may lead to the incorrect grid calculation.
        /// So the "new" algorithm uses this attribute to access the original value stored in the document.
        /// </remarks>
        internal PreferredWidth WidthBeforeOriginal
        {
            get { return (PreferredWidth)FetchAttr(TableAttr.WidthBeforeOriginal); }
        }

        /// <summary>
        /// Represents <see cref="TableAttr.WidthBefore"/>, but ignores values in percent units.
        /// </summary>
        /// <remarks>
        /// The property preserves older AW behavior before percent units for width before/after values were supported.
        /// It is used in the contexts where AW does not support or is not supposed to handle percent units.
        /// </remarks>
        internal int WidthBeforeTwips
        {
            get { return ((PreferredWidth)FetchAttr(TableAttr.WidthBefore)).ValueTwipsSafe; }
            set
            {
                PreferredWidth typedValue = value > 0
                    ? PreferredWidth.FromTwipsSafe(value)
                    : PreferredWidth.Auto;
                SetAttr(TableAttr.WidthBefore, typedValue);
            }
        }

        /// <summary>
        /// Gets or sets <see cref="TableAttr.WidthAfter"/>
        /// </summary>
        /// <remarks>
        /// Dmatv: Returning a value with zero width instead of missing attribute is actually not correct.
        /// MS Word interprets zero and missing width before values differently.
        /// </remarks>
        internal PreferredWidth WidthAfter
        {
            get { return (PreferredWidth)FetchAttr(TableAttr.WidthAfter); }
            set { SetAttr(TableAttr.WidthAfter, value); }
        }

        /// <summary>
        /// Represents <see cref="TableAttr.WidthAfter"/>, but ignores values in percent units.
        /// </summary>
        /// <remarks>
        /// The property preserves older AW behavior before percent units for width before/after values were supported.
        /// It is used in the contexts where AW does not support or is not supposed to handle percent units.
        /// </remarks>
        internal int WidthAfterTwips
        {
            get { return ((PreferredWidth)FetchAttr(TableAttr.WidthAfter)).ValueTwipsSafe; }
            set
            {
                PreferredWidth typedValue = value > 0
                    ? PreferredWidth.FromTwipsSafe(value)
                    : PreferredWidth.Auto;
                SetAttr(TableAttr.WidthAfter, typedValue);
            }
        }

        internal PreferredWidth GetDirectWidthBeforeAfterValue(int attributeKey)
        {
            Debug.Assert(attributeKey == TableAttr.WidthBefore || attributeKey == TableAttr.WidthBeforeOriginal || attributeKey == TableAttr.WidthAfter);
            return (PreferredWidth)GetDirectAttr(attributeKey);
        }

        /// <summary>
        /// Optional ISO29500 and up attribute. Specifies the caption for the table.
        /// </summary>
        internal string Title
        {
            get { return (string)FetchAttr(TableAttr.Title); }
            set { SetAttr(TableAttr.Title, value); }
        }

        /// <summary>
        /// Optional ISO29500 and up attribute. Specifies the description for the table.
        /// </summary>
        internal string Description
        {
            get { return (string) FetchAttr(TableAttr.Description); }
            set { SetAttr(TableAttr.Description, value); }
        }

        internal bool Hidden
        {
            get { return (bool)FetchAttr(TableAttr.Hidden); }
            set { SetAttr(TableAttr.Hidden, value); }
        }

        /// <summary>
        /// HTML related information <see cref="HtmlBlock" /> is applied to this row.
        /// </summary>
        internal int HtmlBlockId
        {
            get { return (int)FetchAttr(TableAttr.HtmlBlockId); }
            set { SetAttr(TableAttr.HtmlBlockId, value); }
        }

        /// <summary>
        /// Gets the left padding for the first cell either from the cell properties or from the table properties.
        /// </summary>
        internal int GetFirstCellLeftPadding(CellPr firstCellPr)
        {
            object firstCellLeftPadding = firstCellPr.GetDirectAttr(CellAttr.LeftPadding);
            return (firstCellLeftPadding != null) ? (int)firstCellLeftPadding : LeftPadding;
        }

        /// <summary>
        /// This method only makes sense for the first row in the table
        /// as I think the table is positioned according to the first row.
        /// Gets the left position of the table.
        /// Includes left indent, left padding, cell spacing and border width as appropriate.
        /// </summary>
        internal int GetTableLeft(CellPr firstCellPr, bool isNested)
        {
            return LeftIndent - GetDistanceFromTableLeftToText(firstCellPr, isNested);
        }

        internal int GetDistanceFromTableLeftToText(CellPr firstCellPr, bool isNested)
        {
            if (isNested)
            {
                return -GetLeftBorderWidth(firstCellPr) / 2;
            }
            else
            {
                // AM I changed calculation slightly while testing TDxaLeft values.
                // Seems it better approximates Word behavior.
                int leftPadding = GetFirstCellLeftPadding(firstCellPr);
                int fullCellSpacing = CellSpacing.ValueRaw * 2;

                int leftBorderWidth = (leftPadding == 0 || fullCellSpacing > 0) ? GetLeftBorderWidth(firstCellPr) : 0;

                if (fullCellSpacing == 0)
                    leftBorderWidth = leftBorderWidth / 2;

                // WORDSNET-23923 Looks like the Word ignores negative cell spacing while calculating distance from table to text.
                int effectiveFullCellSpacing = fullCellSpacing > 0 ? fullCellSpacing : 0;

                return leftPadding + effectiveFullCellSpacing + leftBorderWidth;
            }
        }

        private int GetLeftBorderWidth(CellPr firstCellPr)
        {
            Border leftCellBorder = firstCellPr.BorderLeft;
            if (leftCellBorder == null)
                leftCellBorder = BorderLeft;
            return (leftCellBorder != null) ? ConvertUtilCore.EightsPointToTwip(leftCellBorder.RawLineWidth) : 0;
        }

        /// <summary>
        /// Note this is only available during read and write of the attributes.
        /// </summary>
        internal CellPrCollection CellsPr
        {
            get { return (CellPrCollection)GetDirectAttr(TableAttr.Sys_Cells); }
        }

        /// <summary>
        /// Removes CellsPr from this collection and from the revision.
        /// </summary>
        internal void RemoveCellsPr()
        {
            Remove(TableAttr.Sys_Cells);

            FormatRevision rev = FormatRevision;
            if (rev != null)
                rev.RevPr.Remove(TableAttr.Sys_Cells);
        }

        /// <summary>
        /// Returns either revised or original properties depending on TablePr has format revision.
        /// Note that revised properties is NOT accepted properties and needs to be accepted to get full final property set.
        /// </summary>
        internal TablePr RevisedPr
        {
            get { return HasFormatRevision ? (TablePr)FormatRevision.RevPr : this; }
        }

        /// <summary>
        /// Returns final table properties. If there is a formatting revision applied to the table, accepted table properties
        /// are returned. Otherwise, this table properties are returned as is.
        /// </summary>
        internal TablePr FinalPr
        {
            get
            {
                if (!HasFormatRevision)
                {
                    return this;
                }

                TablePr finalPr = this.Clone();
                finalPr.AcceptFormatRevision();
                return finalPr;
            }
        }

        protected override AttrCollection GetDefaults()
        {
            return gDefaults;
        }

        internal static object FetchDefaultAttr(int key)
        {
            return gDefaults.FetchAttr(key);
        }

        #region IRowAttrSource

        object IRowAttrSource.GetDirectRowAttr(int key)
        {
            return base.GetDirectAttr(key);
        }

        object IRowAttrSource.FetchRowAttr(int key)
        {
            return base.FetchAttr(key);
        }

        object IRowAttrSource.FetchInheritedRowAttr(int key)
        {
            return base.FetchInheritedAttr(key);
        }

        void IRowAttrSource.SetRowAttr(int key, object value)
        {
            base.SetAttr(key, value);
        }

        void IRowAttrSource.ClearRowAttrs()
        {
            base.Clear();
        }

        void IRowAttrSource.ResetToDefaultAttrs()
        {
            ((IRowAttrSource)this).ClearRowAttrs();
        }

        #endregion

        static TablePr()
        {
            gDefaults = new TablePr();

            // These are complex attributes. Be careful not to modify the actual values in them.
            gDefaults.Add(TableAttr.BorderTop, new Border());
            gDefaults.Add(TableAttr.BorderLeft, new Border());
            gDefaults.Add(TableAttr.BorderBottom, new Border());
            gDefaults.Add(TableAttr.BorderRight, new Border());
            gDefaults.Add(TableAttr.BorderHorizontal, new Border());
            gDefaults.Add(TableAttr.BorderVertical, new Border());
            gDefaults.Add(TableAttr.Shading, new Shading());

            gDefaults.Add(TableAttr.Istd, StyleIndex.TableNormal);
            gDefaults.Add(TableAttr.StyleOptions, TableStyleOptions.Default);

            gDefaults.Add(TableAttr.PreferredWidth, PreferredWidth.Auto);
            gDefaults.Add(TableAttr.AllowAutoFit, false);
            gDefaults.Add(TableAttr.LeftIndent, 0);
            gDefaults.Add(TableAttr.WidthBeforeOriginal, PreferredWidth.Auto);
            gDefaults.Add(TableAttr.WidthBefore, PreferredWidth.Auto);
            gDefaults.Add(TableAttr.WidthAfter, PreferredWidth.Auto);
            gDefaults.Add(TableAttr.WidthAfterOriginal, PreferredWidth.Auto);
            gDefaults.Add(TableAttr.Alignment, TableAlignment.Left);
            gDefaults.Add(TableAttr.Bidi, false);

            gDefaults.Add(TableAttr.TopPadding, 0);
            gDefaults.Add(TableAttr.BottomPadding, 0);
            gDefaults.Add(TableAttr.LeftPadding, 0);
            gDefaults.Add(TableAttr.RightPadding, 0);

            gDefaults.Add(TableAttr.CellSpacing, PreferredWidth.ZeroTwips);

            // TODO RK MS-OI29500 2.1.179 Part 1 Section 17.4.81, trHeight (Table Row Height)
            // a. The standard states that if the hRule attribute is omitted, then its value is assumed to be auto.
            // In Word, if the hRule attribute is omitted, then its value is assumed to be atLeast.
            //
            // Auto is equivalent to (AtLeast rule with 0 value).
            gDefaults.Add(TableAttr.RowHeight, new Height(HeightRule.Auto, 0));
            gDefaults.Add(TableAttr.AllowBreakAcrossPages, true);
            gDefaults.Add(TableAttr.HeadingFormat, false);

            gDefaults.Add(TableAttr.AllowOverlap, true);
            gDefaults.Add(TableAttr.FrameDistanceFromTop, 0);
            gDefaults.Add(TableAttr.FrameDistanceFromBottom, 0);
            gDefaults.Add(TableAttr.FrameDistanceFromLeft, 0);
            gDefaults.Add(TableAttr.FrameDistanceFromRight, 0);
            gDefaults.Add(TableAttr.RelativeHorizontalPosition, RelativeHorizontalPosition.Default);
            gDefaults.Add(TableAttr.RelativeVerticalPosition, RelativeVerticalPosition.TableDefault);
            gDefaults.Add(TableAttr.FrameLeft, 0);
            gDefaults.Add(TableAttr.FrameTop, 0);
            gDefaults.Add(TableAttr.HorizontalAlignment, HorizontalAlignment.None);
            gDefaults.Add(TableAttr.VerticalAlignment, VerticalAlignment.None);

            // Defaults are zero as per the DII implementation notes.
            gDefaults.Add(TableAttr.StyleRowBandSize, 0);
            gDefaults.Add(TableAttr.StyleColBandSize, 0);

            gDefaults.Add(TableAttr.Title, "");
            gDefaults.Add(TableAttr.Description, "");

            gDefaults.Add(TableAttr.Hidden, false);
            gDefaults.Add(TableAttr.HtmlBlockId, 0);

            gDefaults.Add(TableAttr.Sys_GridBefore, 0);
            gDefaults.Add(TableAttr.Sys_GridAfter, 0);
            gDefaults.Add(TableAttr.Sys_LeftIndent97, 0);
            gDefaults.Add(TableAttr.Sys_LeftIndent97Base, 0);
            gDefaults.Add(TableAttr.Sys_Word97Logic, false);
            gDefaults.Add(TableAttr.Sys_DxaLeft, 0);
            gDefaults.Add(TableAttr.Sys_DxaGapHalf, 0);
            gDefaults.Add(TableAttr.Sys_Alignment97, TableAlignment.Left);

            PossibleBorderKeys.Add(BorderType.Top, TableAttr.BorderTop);
            PossibleBorderKeys.Add(BorderType.Left, TableAttr.BorderLeft);
            PossibleBorderKeys.Add(BorderType.Bottom, TableAttr.BorderBottom);
            PossibleBorderKeys.Add(BorderType.Right, TableAttr.BorderRight);
            PossibleBorderKeys.Add(BorderType.Horizontal, TableAttr.BorderHorizontal);
            PossibleBorderKeys.Add(BorderType.Vertical, TableAttr.BorderVertical);
        }

        private static readonly TablePr gDefaults;

        /// <summary>
        /// The key is <see cref="BorderType"/> that represents a border that can occur in a <see cref="TablePr"/> collection.
        /// The value is an <see cref="TableAttr"/> integer that represents the attribute key of that border.
        /// </summary>
        internal static readonly SortedList<BorderType, int> PossibleBorderKeys =
            new SortedList<BorderType, int>();
    }
}
