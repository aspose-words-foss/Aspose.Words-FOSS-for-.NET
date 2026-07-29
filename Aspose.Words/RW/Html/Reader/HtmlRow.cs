// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/11/2004 by Roman Korchagin

using System.Collections.Generic;
using Aspose.Words.RW.Html.Css;
using Aspose.Words.RW.Html.Parser;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Collection of cells during HTML table import.
    /// </summary>
    internal class HtmlRow
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="node">Table row element.</param>
        /// <param name="rowDeclarations">Row element CSS declarations.</param>
        /// <param name="rowGroup">HTML row group of the row.</param>
        internal HtmlRow(HtmlElementNode node, CssDeclarationCollection rowDeclarations, HtmlRowGroup rowGroup)
        {
            Debug.Assert(node != null);
            Debug.Assert(rowDeclarations != null);
            Debug.Assert(rowGroup != null);

            mNode = node;
            mRowGroup = rowGroup;

            mCssBorders = CssBoxBorders.CreateBorders(rowDeclarations, false);
            mMaxCellHeightInRow = double.MinValue;
            mDeclarations = rowDeclarations;
            mCells = new List<HtmlCell>();
        }

        internal void Add(HtmlCell cell)
        {
            mCells.Add(cell);
        }

        internal void RemoveLastCell()
        {
            int index = mCells.Count - 1;
            if (index >= 0)
            {
                mCells.RemoveAt(index);
            }
        }

        internal int GetColumnCountWithoutTrailingEmptyExtraCells()
        {
            int trailingEmptyCellCount = 0;
            for (int i = mCells.Count - 1; (i >= 0) && IsEmpty(mCells[i]); i--)
            {
                ++trailingEmptyCellCount;
            }

            return mCells.Count - trailingEmptyCellCount;
        }

        internal HtmlCell this[int index]
        {
            get { return mCells[index]; }
            set { mCells[index] = value; }
        }

        internal HtmlElementNode Node
        {
            get { return mNode; }
        }

        /// <summary>
        /// Maximum cell height in the row; <c>double.MinValue</c> if there are no height specified for the row's cells.
        /// </summary>
        internal double MaxCellHeightInRow
        {
            get { return mMaxCellHeightInRow; }
            set { mMaxCellHeightInRow = value; }
        }

        /// <summary>
        /// CSS borders of the table row.
        /// </summary>
        internal CssBoxBorders CssBorders
        {
            get { return mCssBorders; }
        }

        /// <summary>
        /// Gets HTML row group of the row.
        /// </summary>
        internal HtmlRowGroup RowGroup
        {
            get { return mRowGroup; }
        }

        /// <summary>
        /// CSS declarations of the table row.
        /// </summary>
        internal CssDeclarationCollection Declarations
        {
            get { return mDeclarations; }
        }

        /// <summary>
        /// Gets the number of cells in this row, as declared in the HTML document.
        /// </summary>
        internal int CellCount
        {
            get { return mCells.Count; }
        }

        /// <summary>
        /// Gets the number of cells in this row, as declared in the HTML document, but no greater than the max number
        /// of columns allowed by the document model.
        /// </summary>
        internal int ModelCellCount
        {
            get { return System.Math.Min(CellCount, Table.MaxColumns); }
        }

        private static bool IsEmpty(HtmlCell cell)
        {
            // Vertically merged cells are also treated as non-empty and are not affected by the fix applied in WORDSNET-20250
            // We've added this restriction in order to preserve behavior that had been covered by tests.
            return (cell == null) ||
                ((cell.Node == null) && (cell.VerticalMerge == CellMerge.None));
        }

        private double mMaxCellHeightInRow;
        private readonly HtmlElementNode mNode;
        private readonly CssBoxBorders mCssBorders;
        private readonly HtmlRowGroup mRowGroup;
        private readonly CssDeclarationCollection mDeclarations;
        private readonly List<HtmlCell> mCells;
    }
}
