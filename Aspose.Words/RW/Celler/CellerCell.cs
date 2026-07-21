// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/02/2005 by Roman Korchagin

using Aspose.Words.Tables;

namespace Aspose.Words.RW.Celler
{
    /// <summary>
    /// Represents a table cell in the celler table model. See CellerTable for more info.
    /// </summary>
    internal class CellerCell
    {
        /// <summary>
        /// Redesigned so the caller is to deal with meaningful function names, not constructors with different intentions.
        /// </summary>
        private CellerCell(CellPr cellPr, Cell cellNode)
        {
            mCellPr = cellPr;
            mCellNode = cellNode;
        }

        /// <summary>
        /// Creates a cell from a specified TC.
        /// Use this creator for cells that correspond to cells in the document.
        /// </summary>
        internal static CellerCell CreateNormalCell(CellPr cellPr, Cell cellNode)
        {
            // TODO 2 I don't like this. Clone because we will resolve default borders and don't want to modify original document.
            return new CellerCell(cellPr.Clone(), cellNode);
        }

        /// <summary>
        /// Creates a horizontally merged cell from the specified TC.
        /// Use this creator for "extra" cells that resulted from the celler algorithm.
        /// </summary>
        internal static CellerCell CreateHorizontallyMergedCell(CellPr cellPr, Cell cellNode)
        {
            CellerCell cell = CreateNormalCell(cellPr, cellNode);

            CellPr cellPrDest = cell.mCellPr;
            cellPrDest.HorizontalMerge = CellMerge.Previous;

            // Borders not cloned in the above constructor if they are in the inherited mode.
            // Another thing that looks like a hack and hard to explain why.
            cellPrDest.BorderTop = cellPr.BorderTop;
            cellPrDest.BorderLeft = cellPr.BorderLeft;
            cellPrDest.BorderBottom = cellPr.BorderBottom;
            cellPrDest.BorderRight = cellPr.BorderRight;
            cellPrDest.BorderDiagonalDown = cellPr.BorderDiagonalDown;
            cellPrDest.BorderDiagonalUp = cellPr.BorderDiagonalUp;

            return cell;
        }

        /// <summary>
        /// Creates an empty pad cell.
        /// Use this creator for cells that are used on the left and right of a row as padding.
        /// </summary>
        internal static CellerCell CreatePadCell()
        {
            return new CellerCell(new CellPr(), null);
        }

        internal CellPr CellPr
        {
            get { return mCellPr; }
        }

        internal Cell CellNode
        {
            get { return mCellNode; }
        }

        /// <summary>
        /// We can have some pad cells at the beginning and end of a row so we can align rows of the table vertically.
        /// </summary>
        internal bool IsPad
        {
            get { return mCellNode == null; }
        }

        internal int ColSpan
        {
            get { return mColSpan; }
            set { mColSpan = value; }
        }

        internal int RowSpan
        {
            get { return mRowSpan; }
            set { mRowSpan = value; }
        }

        /// <summary>
        /// Row span value excluding merged rows.
        /// </summary>
        internal int UnmergedRowSpan
        {
            get { return mUnmergedRowSpan; }
            set { mUnmergedRowSpan = value; }
        }

        private readonly CellPr mCellPr;
        private int mColSpan = 1;
        private int mRowSpan = 1;
        private int mUnmergedRowSpan = 1;
        private readonly Cell mCellNode;
    }
}
