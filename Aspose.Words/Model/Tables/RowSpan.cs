// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/11/2014 by Dmitry Matveenko

using System;
using System.Collections.Generic;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Represents a table row during table grid construction.
    /// </summary>
    /// <remarks>
    /// The class represents all table row attributes needed for grid construction.
    /// It also allows to update them independently of the original row attributes.
    /// If during grid construction it is eventually discovered that construction failed
    /// and the grid cannot be applied, the original row attributes remain unchanged.
    /// This allows to fall back to the "old" grid approach.
    /// </remarks>
    internal class RowSpan
    {
        internal RowSpan(Row row, bool useOriginalRowProperties)
        {
            if (row == null)
                throw new ArgumentNullException("row");

            TablePr rowPr = row.TablePr;
            Debug.Assert(rowPr != null);

            // Nrx reader employs some naive technique to set widthBefore. Use the original value from the document.
            // For binary documents, use the value returned from the reader. TestJira5658 and others.
            bool isBinaryDocRtf = DocRtfGridHandler.IsGridStoredInCellWidths(row.ParentTable);
            bool useOriginalValueForDocxWml = useOriginalRowProperties && !isBinaryDocRtf;
            PreferredWidth widthBefore = useOriginalValueForDocxWml
                ? rowPr.GetDirectWidthBeforeAfterValue(TableAttr.WidthBeforeOriginal)
                : rowPr.GetDirectWidthBeforeAfterValue(TableAttr.WidthBefore);

            int gridBefore = rowPr.GridBefore;
            // TestTableMergWidthBeforeWtGridBefore shows that widthBefore without gridBefore is ignored.
            if (gridBefore <= 0)
                widthBefore = null;
            // Preserve the original handling of Auto with a non-zero value
            else if (widthBefore != null && widthBefore.IsAuto && widthBefore.ValueRaw > 0)
                widthBefore = PreferredWidth.FromTwipsSafe(widthBefore.ValueRaw);

            mBefore = new CellSpan(gridBefore, widthBefore);
            mSpanCount = Before.GridSpan;

            PreferredWidth widthAfter = row.TablePr.GetDirectWidthBeforeAfterValue(TableAttr.WidthAfter);
            mAfter = new CellSpan(rowPr.GridAfter, widthAfter);

            CellSpan lastSpan = null;
            foreach (Cell cell in row.Cells)
            {
                CellPr cellPr = cell.CellPr;

                Debug.Assert((cellPr.GridSpan > 0) ||
                    ((cellPr.HorizontalMerge == CellMerge.Previous) && (lastSpan != null)));

                mSpanCount += cellPr.GridSpan;

                switch (cellPr.HorizontalMerge)
                {
                    case CellMerge.None:
                        // Forget "first in merge" span.
                        lastSpan = null;
                        mCells.Add(new CellSpan(cell));
                        break;
                    case CellMerge.First:
                        // Remember "first in merge" span to update span count.
                        lastSpan = new CellSpan(cell);
                        mCells.Add(lastSpan);
                        break;
                    case CellMerge.Previous:
                        if (lastSpan != null)
                        {
                            // This is normal, just increase the span count of the last span.
                            lastSpan.GridSpan += cellPr.GridSpan;
                        }
                        else
                        {
                            // The cell is merged, but the starting cell is missing.
                            // Treat it as a normal (not merged) cell.
                            mCells.Add(new CellSpan(cell));
                        }
                        break;
                    default:
                        throw new InvalidOperationException("Unexpected cell merge type.");
                }
            }
        }

        /// <summary>
        /// Represents row "grid before" and "width before" during grid construction.
        /// </summary>
        internal CellSpan Before
        {
            get { return mBefore; }
        }

        /// <summary>
        /// Update row properties for the new gridBefore value.
        /// </summary>
        /// <remarks>
        /// <see cref="Before"/> and <see cref="SpanCount"/> are updated.
        /// </remarks>
        internal void UpdateBeforeSpanCount(int newGridBefore)
        {
            int oldBeforeSpanCount = Before.GridSpan;
            if ((newGridBefore == 0) && (Before.PreferredWidth.ValueRaw > 0))
            {
                // Test25405 shows a legit case when grid before can be removed altogether with wBefore specified.
                // Well, actually MS Word does not remove gridBefore at all for the source document rtf,
                // but it treats it (and saves to .docx) as if no widthBefore is specified.
                // If wBefore = 6 is manually set for the problematic column in the re-saved docx,
                // MS Word does remove the first grid column on re-saving.
                // So the case of removing a narrow column with widthBefore is legitimate.
                // Precise imitation of MS Word logic for 25405 is a different problem.
                TableGridDebugLogger.DebugWriteLine(
                    string.Format("A non-zero widthBefore {0} is removed from a row because the first grid column is removed",
                        Before.PreferredWidth.ValueRaw));

                Before.PreferredWidth = PreferredWidth.Auto;
            }

            Before.GridSpan = newGridBefore;
            SpanCount += newGridBefore - oldBeforeSpanCount;
        }

        /// <summary>
        /// Represents row "grid after" and "width after" during grid construction.
        /// </summary>
        internal CellSpan After
        {
            get { return mAfter; }
        }

        /// <summary>
        /// Gets the total number of columns occupied by row cells and gridBefore.
        /// </summary>
        /// <remarks>
        /// Row gridAfter is not included as it is actually the number of columns missing at the end of the row.
        /// </remarks>
        internal int SpanCount
        {
            get { return mSpanCount; }
            set { mSpanCount = value; }
        }

        /// <summary>
        /// Represents row cells during grid construction.
        /// </summary>
        /// <remarks>
        /// This is actually a list of GridSpan objects.
        /// </remarks>
        internal IList<CellSpan> Cells
        {
            get { return mCells; }
        }

        internal CellSpan FirstCell
        {
            get { return mCells[0]; }
        }

        internal CellSpan LastCell
        {
            get { return mCells[mCells.Count - 1]; }
        }

        // Generated by Resharper.
        private bool Equals(RowSpan rhs)
        {
            if (!object.Equals(Before, rhs.Before))
                return false;

            if (!object.Equals(After, rhs.After))
                return false;

            if (SpanCount != rhs.SpanCount)
                return false;

            for (int i = 0; i < Cells.Count; i++)
            {
                if(!object.Equals(Cells[i], rhs.Cells[i]))
                    return false;
            }

            return true;
        }

        // Generated by Resharper.
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((RowSpan)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (mBefore != null)
                    ? mBefore.GetHashCode()
                    : 0;

                if (mAfter != null)
                    hashCode = (hashCode * 397) ^ mAfter.GetHashCode();

                foreach (CellSpan cell in mCells)
                    hashCode = (hashCode * 397) ^ cell.GetHashCode();

                return hashCode;
            }
        }

        private readonly CellSpan mBefore;
        private readonly CellSpan mAfter;
        private int mSpanCount;
        private readonly List<CellSpan> mCells = new List<CellSpan>();
    }
}
