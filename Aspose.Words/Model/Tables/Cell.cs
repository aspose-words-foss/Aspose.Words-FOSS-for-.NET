// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2005 by Roman Korchagin

using Aspose.JavaAttributes;
using Aspose.Words.Markup;
using Aspose.Words.Revisions;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Represents a table cell.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-tables/">Working with Tables</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p><see cref="Cell"/> can only be a child of a <see cref="Row"/>.</p>
    /// <p><see cref="Cell"/> can contain block-level nodes <see cref="Paragraph"/> and <see cref="Table"/>.</p>
    /// <p>A minimal valid cell needs to have at least one <see cref="Paragraph"/>.</p>
    /// </remarks>
    [JavaGenericArguments("CompositeNode<Node>")]
    public class Cell : CompositeNode, ICellAttrSource, ITrackableNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Cell"/> class.
        /// </summary>
        /// <remarks>
        /// <p>When <see cref="Cell"/> is created, it belongs to the specified document, but is not
        /// yet part of the document and <see cref="Node.ParentNode"/> is <c>null</c>.</p>
        /// <p>To append <see cref="Cell"/> to the document use <see cref="CompositeNode.InsertAfter{T}(T, Node)"/> or <see cref="CompositeNode.InsertBefore{T}(T, Node)"/>
        /// on the row where you want the cell inserted.</p>
        /// </remarks>
        /// <param name="doc">The owner document.</param>
        public Cell(DocumentBase doc) : this(doc, new CellPr())
        {
        }

        internal Cell(DocumentBase doc, CellPr cellPr) : base(doc)
        {
            mCellPr = cellPr;
        }

        /// <summary>
        /// Returns <see cref="NodeType.Cell"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.Cell; }
        }

        /// <summary>
        /// Returns a table this cell belongs to.
        /// </summary>
        internal Table ParentTable
        {
            get { return ParentRow.ParentTable; }
        }

        /// <summary>
        /// Gets the next <see cref="Cell"/> node.
        /// </summary>
        /// <remarks>
        /// The method can be used when you need to have typed access to cells of a <see cref="Row"/>. If a
        /// <see cref="StructuredDocumentTag"/> node is found in a row instead of a cell, it is automatically
        /// traversed to get a cell contained within.
        /// </remarks>
        public Cell NextCell
        {
            get { return (Cell)NextNonMarkupCompositeLimited; }
        }

        /// <summary>
        /// Gets the previous <see cref="Cell"/> node.
        /// </summary>
        /// <remarks>
        /// The method can be used when you need to have typed access to cells of a <see cref="Row"/>. If a
        /// <see cref="StructuredDocumentTag"/> node is found in a row instead of a cell, it is automatically
        /// traversed to get a cell contained within.
        /// </remarks>
        public Cell PreviousCell
        {
            get { return (Cell)PreviousNonMarkupCompositeLimited; }
        }

        /// <summary>
        /// Returns the parent row of the cell.
        /// </summary>
        /// <dev>
        /// Equivalent to <see cref="Node.FirstNonMarkupParentNode"/> casted to <see cref="Row"/>.
        /// </dev>
        public Row ParentRow
        {
            get { return (Row)FirstNonMarkupParentNode; }
        }

        /// <summary>
        /// Gets the first paragraph among the immediate children.
        /// </summary>
        public Paragraph FirstParagraph
        {
            get { return (Paragraph)GetChild(NodeType.Paragraph, 0, false); }
        }

        /// <summary>
        /// Gets the last paragraph among the immediate children.
        /// </summary>
        public Paragraph LastParagraph
        {
            get { return (Paragraph)GetChild(NodeType.Paragraph, -1, false); }
        }

        /// <summary>
        /// True if this is the first cell inside a row; false otherwise.
        /// </summary>
        public bool IsFirstCell
        {
            get { return ((ParentRow != null) && (this == ParentRow.FirstCell)); }
        }

        /// <summary>
        /// True if this is the last cell inside a row; false otherwise.
        /// </summary>
        public bool IsLastCell
        {
            get { return ((ParentRow != null) && (this == ParentRow.LastCell)); }
        }

        /// <summary>
        /// Gets the column index of this cell or -1 if this cell has no parent row.
        /// </summary>
        /// <remarks>
        /// Considers markup parent nodes.
        /// </remarks>
        internal int ColumnIndex
        {
            get
            {
                Row parentRow = ParentRow;
                return (parentRow != null) ? parentRow.IndexOfChildByDescendant(this, true) : -1;
            }
        }
        
        /// <summary>
        /// Gets the row index of this cell or -1 if this cell has no parent row.
        /// </summary>
        internal int RowIndex
        {
            get { return (ParentRow != null) ? ParentRow.RowIndex : -1; }
        }

        /// <summary>
        /// Provides access to the formatting properties of the cell.
        /// </summary>
        public CellFormat CellFormat
        {
            get
            {
                if (mCellFormat == null)
                    mCellFormat = new CellFormat(this);
                return mCellFormat;
            }
        }

        /// <summary>
        /// Gets a collection of paragraphs that are immediate children of the cell.
        /// </summary>
        public ParagraphCollection Paragraphs
        {
            get
            {
                if (mParagraphs == null)
                    mParagraphs = new ParagraphCollection(this);
                return mParagraphs;
            }
        }

        /// <summary>
        /// Gets a collection of tables that are immediate children of the cell.
        /// </summary>
        public TableCollection Tables
        {
            get
            {
                if (mTables == null)
                    mTables = new TableCollection(this);
                return mTables;
            }
        }

        /// <summary>
        /// Gets the collection of attributes specified on this cell directly.
        /// </summary>
        internal CellPr CellPr
        {
            get { return mCellPr; }
            set
            {
                mCellPr = value;
            }
        }

        internal override Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            Cell lhs = (Cell)base.Clone(isCloneChildren, cloningListener);
            CellPr cellPr =  mCellPr.Clone();
            lhs.mCellPr = cellPr;
            lhs.mCellFormat = null;
            lhs.mParagraphs = null;
            lhs.mTables = null;
            return lhs;
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Node.Accept"]/*'/>
        /// <remarks>
        /// Calls <see cref="DocumentVisitor.VisitCellStart"/>, then calls <see cref="Node.Accept"/> for all child nodes of the section
        /// and calls <see cref="DocumentVisitor.VisitCellEnd"/> at the end.
        /// </remarks>
        public override bool Accept(DocumentVisitor visitor)
        {
            return AcceptCore(visitor);
        }


        /// <summary>
        /// Accepts a visitor for visiting the start of the cell.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptStart(DocumentVisitor visitor)
        {
            return visitor.VisitCellStart(this);
        }

        /// <summary>
        /// Accepts a visitor for visiting the end of the cell.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptEnd(DocumentVisitor visitor)
        {
            return visitor.VisitCellEnd(this);
        }
        
        /// <summary>
        /// If the last child is not a paragraph, creates and appends one empty paragraph.
        /// </summary>
        public void EnsureMinimum()
        {
            // WORDSNET-3381 Last node in a cell cannot be a table, it must be a paragraph.
            WordUtil.EnsureNonEmptyStory(this);
        }

        /// <summary>
        /// Allows to insert block level elements.
        /// </summary>
        internal override bool CanInsert(Node newChild)
        {
            return NodeUtil.IsBlockLevelNode(newChild);
        }

        /// <summary>
        /// Gets the column index of this cell, ignoring cells with <see cref="CellMerge.Previous"/> merge state.
        /// </summary>
        internal int GetMergedColumnIndex()
        {
            Row parentRow = ParentRow;
            if (parentRow == null)
                return -1;

            int index = -1;

            for (Cell cell = parentRow.FirstCell; cell != null; cell = cell.NextCell)
            {
                if (cell.CellFormat.HorizontalMerge != CellMerge.Previous)
                    index++;

                if (cell == this)
                    return index;
            }

            return -1;
        }

        #region ICellAttrSource

        /// <summary>
        /// ICellAttrSource.
        /// </summary>
        object ICellAttrSource.GetDirectCellAttr(int key)
        {
            return mCellPr.GetDirectAttr(key);
        }

        /// <summary>
        /// ICellAttrSource.
        /// </summary>
        object ICellAttrSource.FetchCellAttr(int key)
        {
            // SPEED for export to formats that require fully calculated values.
            // Gets the fully calculated value of the attribute.
            // For example, the border could be defined on the cell or inherited from the table or defaults.
            // Note that if the border is inherited, this will return the parent value, so don't modify it.
            object value = mCellPr.GetDirectAttr(key);
            if (value == null)
            {
                // The attribute is not defined on the cell, return the inherited value.
                return ((ICellAttrSource)this).FetchInheritedCellAttr(key);
            }
            else
            {
                IComplexAttr complexAttr = value as IComplexAttr;
                if ((complexAttr != null) && (complexAttr.IsInheritedComplexAttr))
                {
                    // The attribute is complex and it is inherited, return the inherited value.
                    return ((ICellAttrSource)this).FetchInheritedCellAttr(key);
                }
                else
                {
                    return value;
                }
            }
        }

        /// <summary>
        /// ICellAttrSource.
        /// </summary>
        object ICellAttrSource.FetchInheritedCellAttr(int key)
        {
            object value = GetInheritedCellAttr(key);

            if (value != null)
                return value;

            // WORDSNET-1348, ParentRow could be null if the cell is not added to a row.
            if (ParentRow != null)
            {
                // When requesting a cell border or padding, retrieve them from the table row.
                // Interesting to note that while cell borders can inherit from the table,
                // shading does not inherit. If shading is not defined for the cell,
                // it means no shading, and we should not use the table default shading.
                IRowAttrSource rowAttrs = ParentRow;
                switch (key)
                {
                    case CellAttr.TopPadding:
                        return rowAttrs.FetchRowAttr(TableAttr.TopPadding);
                    case CellAttr.LeftPadding:
                        return rowAttrs.FetchRowAttr(TableAttr.LeftPadding);
                    case CellAttr.BottomPadding:
                        return rowAttrs.FetchRowAttr(TableAttr.BottomPadding);
                    case CellAttr.RightPadding:
                        return rowAttrs.FetchRowAttr(TableAttr.RightPadding);
                    case CellAttr.BorderTop:
                        return rowAttrs.FetchRowAttr(ParentRow.IsFirstRow ? TableAttr.BorderTop : TableAttr.BorderHorizontal);
                    case CellAttr.BorderLeft:
                        return rowAttrs.FetchRowAttr(IsFirstCell ? TableAttr.BorderLeft : TableAttr.BorderVertical);
                    case CellAttr.BorderBottom:
                        return rowAttrs.FetchRowAttr(ParentRow.IsLastRow ? TableAttr.BorderBottom : TableAttr.BorderHorizontal);
                    case CellAttr.BorderRight:
                        return rowAttrs.FetchRowAttr(IsLastCell ? TableAttr.BorderRight : TableAttr.BorderVertical);
                    default:
                        break;
                }
            }

            // Return global default value.
            return CellPr.FetchDefaultAttr(key);
        }

        void ICellAttrSource.SetCellAttr(int key, object value)
        {
            mCellPr.SetAttr(key, value);
        }

        void ICellAttrSource.ClearCellAttrs()
        {
            mCellPr.Clear();
        }

        #endregion

        /// <summary>
        /// Gets inherited cell attribute. 
        /// Does not fetch global defaults.
        /// Does not inherit table borders or paddings.
        /// </summary>
        /// <remarks>
        /// AM. It seems that we need some flags for ICellAttrSource.FetchCellAttr similar to RunPrExpandFlags then 
        /// we can remove this method and replace it with FetchCellAttr.
        /// Lets postpone this for a while.
        /// </remarks>
        internal object GetInheritedCellAttr(int key)
        {
            // WORDSNET-16039 For cloned rows ParentRow is not null, but ParentTable is null,
            // if row not yet added to table. Therefore, need to check both.
            if ((ParentRow == null) || (ParentTable == null))
                return null;

            TableStyle tableStyle = ParentTable.Style as TableStyle;
            return (tableStyle != null) ? tableStyle.GetCellAttr(key, this) : null;
        }

        EditRevision ITrackableNode.InsertRevision
        {
            get { return CellPr.InsertRevision; }
            set { CellPr.InsertRevision = value; }
        }

        EditRevision ITrackableNode.DeleteRevision
        {
            get { return CellPr.DeleteRevision; }
            set { CellPr.DeleteRevision = value; }
        }

        MoveRevision IMoveTrackableNode.MoveFromRevision
        {
            get { return CellPr.MoveFromRevision; }
            set { CellPr.MoveFromRevision = value; }
        }

        MoveRevision IMoveTrackableNode.MoveToRevision
        {
            get { return CellPr.MoveToRevision; }
            set { CellPr.MoveToRevision = value; }
        }

        void IMoveTrackableNode.RemoveMoveRevisions()
        {
            CellPr.Remove(RevisionAttr.MoveFromRevision);
            CellPr.Remove(RevisionAttr.MoveToRevision);
        }

        /// <summary>
        /// Creates and returns a fully expanded collection of attributes for this cell.
        /// </summary>
        internal CellPr GetExpandedCellPr(bool expandTableStyle)
        {
            CellPr cellPr = new CellPr();

            // RK I think only borders and padding are inherited from the table.
            ExpandAttr(CellAttr.BorderLeft, cellPr);
            ExpandAttr(CellAttr.BorderRight, cellPr);
            ExpandAttr(CellAttr.BorderTop, cellPr);
            ExpandAttr(CellAttr.BorderBottom, cellPr);

            ExpandAttr(CellAttr.LeftPadding, cellPr);
            ExpandAttr(CellAttr.RightPadding, cellPr);
            ExpandAttr(CellAttr.TopPadding, cellPr);
            ExpandAttr(CellAttr.BottomPadding, cellPr);

            // Expand conditional formatting.
            if (expandTableStyle)
            {
                TableStyle tableStyle = ParentTable.Style as TableStyle;
                if (tableStyle != null)
                    tableStyle.ExpandCellPr(this, cellPr);
            }

            mCellPr.ExpandTo(cellPr);

            return cellPr;
        }

        private void ExpandAttr(int key, CellPr dstCellPr)
        {
            dstCellPr.SetAttr(key, ((ICellAttrSource)this).FetchInheritedCellAttr(key));
        }

        private CellPr mCellPr;
        private CellFormat mCellFormat;
        private ParagraphCollection mParagraphs;
        private TableCollection mTables;
    }
}
