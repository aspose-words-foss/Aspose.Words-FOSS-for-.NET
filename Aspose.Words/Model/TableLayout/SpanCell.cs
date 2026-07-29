// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/08/2006 by Dmitry Vorobyev
using Aspose.Words.Tables;

namespace Aspose.Words.TableLayout
{
    /// <summary>
    /// Encapsulates a spanned cell information.
    /// </summary>
    internal struct SpanCell
    {
        internal SpanCell(Cell cell, int columnIndex)
        {
            Debug.Assert(cell != null);
            mCell = cell;
            mColumnIndex = columnIndex;
            
            mSpan = Extensions.GetHorizontalMergeSpan(cell);
            // The object should be created for multi-column cells only.
            Debug.Assert(mSpan > 1);
        }

        internal Cell Cell
        {
            get { return mCell; }
        }

        internal int Span
        {
            get { return mSpan; }
        }

        internal int ColumnIndex
        {
            get { return mColumnIndex; }
        }

        private readonly Cell mCell;
        private readonly int mSpan;
        private readonly int mColumnIndex;
    }
}
