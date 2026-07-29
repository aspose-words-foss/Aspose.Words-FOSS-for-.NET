// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/06/2009 by Roman Korchagin

using Aspose.JavaAttributes;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Base class for the algorithm that expands conditional formatting of table styles.
    /// </summary>
    internal abstract class CnfExpanderBase
    {
        internal void Expand(TableFormattingExpander mother, AttrCollection dstAttrs)
        {
            mMother = mother;
            mDstAttrs = dstAttrs;

            // 1. Banded rows
            if (mMother.IsRowBandNow)
                DoExpand(mMother.RowBandNow);

            // 2. Banded cols
            if (mMother.IsColBandNow)
                DoExpand(mMother.ColBandNow);

            // 3. First/last col
            if (mMother.IsFirstColNow)
                DoExpand(TableStyleOverrideType.FirstColumn);

            if (mMother.IsLastColNow)
                DoExpand(TableStyleOverrideType.LastColumn);

            // 4. First/last row
            if (mMother.IsFirstRowNow)
                DoExpand(TableStyleOverrideType.FirstRow);

            if (mMother.IsLastRowNow)
                DoExpand(TableStyleOverrideType.LastRow);

            // 5. Corner cells.
            if (mMother.IsNWCellNow)
                DoExpand(TableStyleOverrideType.TopLeftCell);

            if (mMother.IsNECellNow)
                DoExpand(TableStyleOverrideType.TopRightCell);

            if (mMother.IsSWCellNow)
                DoExpand(TableStyleOverrideType.BottomLeftCell);

            if (mMother.IsSECellNow)
                DoExpand(TableStyleOverrideType.BottomRightCell);
        }

        [JavaThrows(true)]
        protected abstract void DoExpand(TableStyleOverrideType type);

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        protected TableFormattingExpander mMother;
        protected AttrCollection mDstAttrs;
    }

    internal class CnfExpanderRunPr : CnfExpanderBase
    {
        protected override void DoExpand(TableStyleOverrideType type)
        {
            ConditionalStyle conditionalStyle = mMother.GetConditionalStyle(type);
            if ((conditionalStyle != null) && conditionalStyle.HasRunFormatting)
            {
                conditionalStyle.RunPr.ExpandTo(mDstAttrs);

                conditionalStyle.RunPr.ThemeColorInheritanceHack(mDstAttrs);
            }
        }
    }

    internal class CnfExpanderParaPr : CnfExpanderBase
    {
        protected override void DoExpand(TableStyleOverrideType type)
        {
            ConditionalStyle conditionalStyle = mMother.GetConditionalStyle(type);
            if ((conditionalStyle != null) && conditionalStyle.HasParagraphFormatting)
                conditionalStyle.ParaPr.ExpandTo(mDstAttrs);
        }
    }

    internal class CnfExpanderCellPr : CnfExpanderBase, IExpandedAttrSaver
    {
        private TableStyleOverrideType mType; 

        protected override void DoExpand(TableStyleOverrideType type)
        {
            ConditionalStyle conditionalStyle = mMother.GetConditionalStyle(type);
            if ((conditionalStyle == null) || !conditionalStyle.HasCellFormatting)
                return;

            mType = type;
            conditionalStyle.CellPr.ExpandTo(mDstAttrs, this);
        }

        /// <summary>
        /// When expanding cell properties of conditional formatting, we need to use
        /// inside horizontal and vertical borders instead of top, left, right and bottom
        /// where appropriate.
        /// </summary>
        void IExpandedAttrSaver.Save(AttrCollection dstAttrs, int key, object value)
        {
            switch (mType)
            {
                case TableStyleOverrideType.FirstRow:
                case TableStyleOverrideType.LastRow:
                {
                    if (ExpandVerticalBorders(key, value))
                        return;

                    break;
                }
                case TableStyleOverrideType.FirstColumn:
                case TableStyleOverrideType.LastColumn:
                {
                    if (ExpandInsideHorizontal(key, value))
                        return;

                    break;
                }
                case TableStyleOverrideType.OddRowBanding:
                case TableStyleOverrideType.EvenRowBanding:
                {
                    bool done = ExpandVerticalBorders(key, value);

                    // Expand horizontal borders for wide row bands.
                    if (!done && (mMother.RowBandSize > 1))
                        done = ExpandInsideAndOutsideBorders(
                            key,
                            value,
                            CellAttr.BorderHorizontal,
                            CellAttr.BorderTop,
                            CellAttr.BorderBottom,
                            mMother.IsFirstRowInBand,
                            mMother.IsLastRowInBand);

                    if (done)
                        return;

                    break;

                }
                case TableStyleOverrideType.OddColumnBanding:
                case TableStyleOverrideType.EvenColumnBanding:
                {
                    bool done = ExpandInsideHorizontal(key, value);

                    // Expand vertical borders for wide column bands.
                    if (!done && (mMother.ColBandSize > 1))
                        done = ExpandInsideAndOutsideBorders(
                            key,
                            value,
                            CellAttr.BorderVertical,
                            CellAttr.BorderLeft,
                            CellAttr.BorderRight,
                            mMother.IsFirstColInBand,
                            mMother.IsLastColInBand);

                    if (done)
                        return;

                    break;
                }
                default:
                    break;
            }

            mDstAttrs[key] = value;
        }

        private bool ExpandVerticalBorders(int key, object value)
        {
            return ExpandInsideAndOutsideBorders(
                key,
                value,
                CellAttr.BorderVertical,
                CellAttr.BorderLeft,
                CellAttr.BorderRight,
                mMother.IsFirstCol,
                mMother.IsLastCol);
        }

        private bool ExpandInsideHorizontal(int key, object value)
        {
            return ExpandInsideAndOutsideBorders(
                key,
                value,
                CellAttr.BorderHorizontal,
                CellAttr.BorderTop,
                CellAttr.BorderBottom,
                mMother.IsFirstRow,
                mMother.IsLastRow);
        }

        private bool ExpandInsideAndOutsideBorders(
            int key,
            object value,
            int keyInside,
            int keyFirst,
            int keyLast,
            bool isFirst,
            bool isLast)
        {
            if (key == keyFirst)
            {
                // The left border should only be stored in the first cell.
                if (isFirst)
                    mDstAttrs[keyFirst] = value;

                return true;
            }
            else if (key == keyLast)
            {
                // The right border should only be stored in the last cell.
                if (isLast)
                    mDstAttrs[keyLast] = value;

                return true;
            }
            else if (key == keyInside)
            {
                // The inside vertical border should become a left and/or right border.
                // If we use it for both left and right, we need to clone it.
                bool isNeedClone = false;

                if (!isLast)
                {
                    mDstAttrs[keyLast] = value;
                    isNeedClone = true;
                }

                if (!isFirst)
                {
                    object leftBorder = (isNeedClone) ? ((Border)value).Clone() : value;
                    mDstAttrs[keyFirst] = leftBorder;
                }

                return true;
            }
            else
            {
                return false;
            }
        }
    }

    internal class CnfExpanderRowPr : CnfExpanderBase
    {
        protected override void DoExpand(TableStyleOverrideType type)
        {
            ConditionalStyle conditionalStyle = mMother.GetConditionalStyle(type);
            if (conditionalStyle == null) 
                return;
            
            if (conditionalStyle.HasTableFormatting)
                conditionalStyle.TablePr.ExpandTo(mDstAttrs);

            if (conditionalStyle.HasRowFormatting)
                conditionalStyle.RowPr.ExpandTo(mDstAttrs);
        }
    }
}
