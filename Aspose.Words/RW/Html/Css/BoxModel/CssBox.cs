// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/10/2013 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// A box of the CSS box model.
    /// </summary>
    internal class CssBox
    {
        internal CssBox()
            : this(gZeroMargin, gZeroMargin, gZeroMargin, gZeroMargin, false, false, true, false, true)
        {
            // Empty constructor.
        }

        internal CssBox(CssBoxMargin left,
            CssBoxMargin right,
            CssBoxMargin top,
            CssBoxMargin bottom,
            bool startsWithImplicitBox,
            bool endsWithImplicitBox,
            bool bottomMarginIsCollapsible,
            bool isInsideTableCell,
            bool canContainImplicitTextBoxes)
        {
            mLeftMargin = left;
            mRightMargin = right;
            mTopMargin = top;
            mBottomMargin = bottom;
            mChildTopMarginCollapsing = CssBoxMarginCollapsing.Collapse;
            mChildBottomMarginCollapsing = CssBoxMarginCollapsing.Collapse;
            mStartsWithImplicitBox = startsWithImplicitBox;
            mEndsWithImplicitBox = endsWithImplicitBox;
            mBottomMarginIsCollapsible = bottomMarginIsCollapsible;
            mIsInsideTableCell = isInsideTableCell;
            mCanContainImplicitTextBoxes = canContainImplicitTextBoxes;
        }

        internal CssBoxMarginCollapsing ChildTopMarginCollapsing
        {
            get { return mChildTopMarginCollapsing; }
            set { mChildTopMarginCollapsing = value; }
        }

        internal CssBoxMarginCollapsing ChildBottomMarginCollapsing
        {
            get { return mChildBottomMarginCollapsing; }
            set { mChildBottomMarginCollapsing = value; }
        }

        internal bool BottomMarginIsCollapsible
        {
            get { return mBottomMarginIsCollapsible; }
        }

        internal bool StartsWithImplicitBox
        {
            get { return mStartsWithImplicitBox; }
        }

        internal bool EndsWithImplicitBox
        {
            get { return mEndsWithImplicitBox; }
        }

        internal bool IsInsideTableCell
        {
            get { return mIsInsideTableCell; }
        }

        internal bool CanContainImplicitTextBoxes
        {
            get { return mCanContainImplicitTextBoxes; }
        }

        internal bool IsEmptyBlock { get; set; }

        internal CssBoxMargin GetMargin(CssBoxMarginSide side)
        {
            switch (side)
            {
                case CssBoxMarginSide.Left:
                    return mLeftMargin;
                case CssBoxMarginSide.Right:
                    return mRightMargin;
                case CssBoxMarginSide.Top:
                    return mTopMargin;
                case CssBoxMarginSide.Bottom:
                    return mBottomMargin;
                default:
                    Debug.Assert(false);
                    return gZeroMargin;
            }
        }

        private static readonly CssBoxMargin gZeroMargin = CssBoxMargin.Create(
            CssLength.ZeroDefault,
            CssBoxMarginCollapsing.Collapse,
            true);

        private CssBoxMarginCollapsing mChildTopMarginCollapsing;

        private CssBoxMarginCollapsing mChildBottomMarginCollapsing;

        private readonly bool mStartsWithImplicitBox;

        private readonly bool mEndsWithImplicitBox;

        private readonly bool mBottomMarginIsCollapsible;

        private readonly CssBoxMargin mLeftMargin;

        private readonly CssBoxMargin mRightMargin;

        private readonly CssBoxMargin mTopMargin;

        private readonly CssBoxMargin mBottomMargin;

        private readonly bool mIsInsideTableCell;

        private readonly bool mCanContainImplicitTextBoxes;

#if DEBUG
        internal CssDisplayType DisplayType;
        internal string Name;
#endif
    }
}
