// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/04/2011 by Roman Korchagin

using Aspose.Words.Tables;

namespace Aspose.Words.TableLayout
{
    /// <summary>
    /// Stores several values to pass between the methods that deal with spanned cell calculations.
    /// </summary>
    internal class SpanContext
    {
        internal SpanContext()
        {
        }

        internal int SpanIndex
        {
            get { return mSpanIndex; }
            set { mSpanIndex = value; }
        }

        internal int LastColumnIndex
        {
            get { return mLastColumnIndex; }
            set { mLastColumnIndex = value; }
        }

        internal int Span
        {
            get { return mSpan; }
            set { mSpan = value; }
        }

        internal PreferredWidth CellPreferredWidth
        {
            get { return mCellPreferredWidth; }
            set { mCellPreferredWidth = value; }
        }

        internal bool AllColumnsAreFixed
        {
            get { return mAllColumnsAreFixed; }
            set { mAllColumnsAreFixed = value; }
        }

        internal bool AllColumnsArePercent
        {
            get { return mAllColumnsArePercent; }
            set { mAllColumnsArePercent = value; }
        }

        internal bool HasAuto
        {
            get { return mHasAuto; }
            set { mHasAuto = value; }
        }

        internal double TotalPercent
        {
            get { return mTotalPercent; }
            set { mTotalPercent = value; }
        }

        internal double TotalFixed
        {
            get { return mTotalFixed; }
            set { mTotalFixed = value; }
        }

        internal LayoutWidth CellLayoutWidth
        {
            get { return mCellLayoutWidth; }
            set { mCellLayoutWidth = value; }
        }

        internal LayoutWidth SpanLayoutWidth
        {
            get { return mSpanLayoutWidth; }
            set { mSpanLayoutWidth = value; }
        }

        private int mSpan;
        private int mSpanIndex;
        private int mLastColumnIndex;
        private PreferredWidth mCellPreferredWidth;
        private bool mAllColumnsAreFixed = true;
        private bool mAllColumnsArePercent = true;
        private bool mHasAuto = false;
        private double mTotalPercent = 0;
        private double mTotalFixed = 0;
        private LayoutWidth mCellLayoutWidth;
        private LayoutWidth mSpanLayoutWidth;
    }
}
