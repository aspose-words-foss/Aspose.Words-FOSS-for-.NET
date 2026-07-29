// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 21/11/2004 by Roman Korchagin

using Aspose.Words.RW.Html.Css;
using Aspose.Words.RW.Html.Parser;
using Aspose.Words.Tables;

namespace Aspose.Words.RW.Html.Reader
{
    /// <summary>
    /// Represents a table cell during import of an HTML table.
    /// </summary>
    internal class HtmlCell
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal HtmlCell(HtmlElementNode node, CssBoxBorders cssBorders)
            : this(cssBorders)
        {
            Debug.Assert(node != null);
            mNode = node;
        }

        /// <summary>
        /// Creates a cell that represents a merged HTML cell.
        /// </summary>
        internal HtmlCell(CssBoxBorders cssBorders)
        {
            mCssBorders = cssBorders;
            mColSpan = 1;
            mRowSpan = 1;
        }

        /// <summary>
        /// Returns the HTML TH or TD node that this cell represents. Null for merged cells.
        /// </summary>
        internal HtmlElementNode Node
        {
            get { return mNode; }
        }

        /// <summary>
        /// CSS borders of the table cell.
        /// </summary>
        internal CssBoxBorders CssBorders
        {
            get { return mCssBorders; }
        }

        /// <summary>
        /// Gets/sets the merge option that is used to create an imported cell.
        /// </summary>
        internal CellMerge HorizontalMerge
        {
            get { return mHorizontalMerge; }
            set { mHorizontalMerge = value; }
        }

        /// <summary>
        /// Gets/sets the merge option that is used to create an imported cell.
        /// </summary>
        internal CellMerge VerticalMerge
        {
            get { return mVerticalMerge; }
            set { mVerticalMerge = value; }
        }

        /// <summary>
        /// Gets/sets the first horizontal merge cell if this cell is a subsequent in the chain or null otherwise.
        /// </summary>
        internal HtmlCell HorizontalMergeFirstCell
        {
            get { return mHorizontalMergeFirstCell; }
            set { mHorizontalMergeFirstCell = value; }
        }

        /// <summary>
        /// Gets/sets the first vertical merge cell if this cell is a subsequent in the chain or null otherwise.
        /// </summary>
        internal HtmlCell VerticalMergeFirstCell
        {
            get { return mVerticalMergeFirstCell; }
            set { mVerticalMergeFirstCell = value; }
        }

        /// <summary>
        /// The number of cols (adjacent cells to the right) this cell is to span. Default is one.
        /// </summary>
        internal int ColSpan
        {
            get { return mColSpan; }
            set
            {
                Debug.Assert(value >= 1);
                mColSpan = value;
            }
        }

        /// <summary>
        /// The number of COL elements the cell is to span. Default is one.
        /// </summary>
        /// <remarks>
        /// In most cases this value equals <see cref="ColSpan"/> but unlike <see cref="ColSpan"/> this value doesn't get
        /// decreased on last cells in a table row that span extra empty cells.
        /// </remarks>
        internal int UnrestrictedColSpan { get; set; }

        /// <summary>
        /// The number of rows the cell is to span. Default is one.
        /// </summary>
        internal int RowSpan
        {
            get { return mRowSpan; }
            set
            {
                Debug.Assert(value >= 1);
                mRowSpan = value;
            }
        }

        /// <summary>
        /// Gets/sets the corresponding model cell.
        /// </summary>
        internal Cell ModelCell
        {
            get { return mModelCell; }
            set { mModelCell = value; }
        }

        private readonly HtmlElementNode mNode;
        private readonly CssBoxBorders mCssBorders;
        private CellMerge mHorizontalMerge;
        private CellMerge mVerticalMerge;
        private HtmlCell mHorizontalMergeFirstCell;
        private HtmlCell mVerticalMergeFirstCell;
        private int mColSpan;
        private int mRowSpan;
        private Cell mModelCell;
    }
}
