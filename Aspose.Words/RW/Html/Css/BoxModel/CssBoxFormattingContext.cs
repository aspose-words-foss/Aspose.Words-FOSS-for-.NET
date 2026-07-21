// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 24/03/2014 by Victor Chebotok

namespace Aspose.Words.RW.Html.Css
{
    /// <summary>
    /// Represents a CSS box formatting context. If boxes belong to the same formatting context, their adjacent vertical margins
    /// can collapse.
    /// </summary>
    /// <remarks>
    /// We have to manually collapse adjacent vertical margins of boxes in cases where MS Word do not conform to CSS rules.
    /// For example, in CSS floating tables do not affect collapsing of margins of elements around them, but MS Word do not
    /// collapse the margins. Currently, our CSS box model creates additional formatting contexts only for floating tables.
    /// These contexts can be nested.
    /// </remarks>
    internal class CssBoxFormattingContext
    {
        internal CssBoxFormattingContext(CssBox rootBox, bool isFullWidthBox)
        {
            Debug.Assert(rootBox != null);
            mRootBox = rootBox;
            mIsFullWidthBox = isFullWidthBox;
        }

        internal CssBox RootBox
        {
            get { return mRootBox; }
        }

        /// <summary>
        /// Gets a value indicating whether the enclosing box of this formatting context has 100% width (is full-width).
        /// </summary>
        /// <remarks>
        /// A full-width floating box cannot have other boxes next to its left and right sides. Instead, adjacent boxes
        /// are pushed below the full-width box.
        /// </remarks>
        internal bool IsFullWidthBox
        {
            get { return mIsFullWidthBox; }
        }

        internal CssBoxMarginManualCollapsing CurrentBoxTopMarginManualCollapsing
        {
            get { return mCurrentBoxTopMarginManualCollapsing; }
            set { mCurrentBoxTopMarginManualCollapsing = value; }
        }

        internal double PreviousBoxBottomMargin
        {
            get { return mPreviousBoxBottomMargin; }
            set { mPreviousBoxBottomMargin = value; }
        }

        internal double CurrentBoxBottomMargin
        {
            get { return mCurrentBoxBottomMargin; }
            set { mCurrentBoxBottomMargin = value; }
        }

        internal CssLength EmptyBlockMarginBefore { get; set; }

        private readonly CssBox mRootBox;

        private readonly bool mIsFullWidthBox;

        private CssBoxMarginManualCollapsing mCurrentBoxTopMarginManualCollapsing;

        private double mPreviousBoxBottomMargin;

        private double mCurrentBoxBottomMargin;
    }
}
