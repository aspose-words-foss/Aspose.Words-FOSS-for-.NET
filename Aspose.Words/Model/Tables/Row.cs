// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2005 by Roman Korchagin

using Aspose.JavaAttributes;
using Aspose.Words.Markup;
using Aspose.Words.Revisions;

namespace Aspose.Words.Tables
{
    /// <summary>
    /// Represents a table row.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-tables/">Working with Tables</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p><see cref="Row"/> can only be a child of a <see cref="Table"/>.</p>
    /// <p><see cref="Row"/> can contain one or more <see cref="Cell"/> nodes.</p>
    /// <p>A minimal valid row needs to have at least one <see cref="Cell"/>.</p>
    /// </remarks>
    [JavaGenericArguments("CompositeNode<Cell>")]
    public class Row : CompositeNode, IRowAttrSource, ITrackableNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Row"/> class.
        /// </summary>
        /// <remarks>
        /// <p>When <see cref="Row"/> is created, it belongs to the specified document, but is not
        /// yet part of the document and <see cref="Node.ParentNode"/> is <c>null</c>.</p>
        /// <p>To append <see cref="Row"/> to the document use <see cref="CompositeNode.InsertAfter{T}(T, Node)"/> or <see cref="CompositeNode.InsertBefore{T}(T, Node)"/>
        /// on the table where you want the row inserted.</p>
        /// </remarks>
        /// <param name="doc">The owner document.</param>
        public Row(DocumentBase doc)
            : this(
            doc,
            TablePr.CreateMSWordLooking())  // It seems a good idea to make the table look like created in MS Word when user creates it via the public ctor.
        {
        }

        internal Row(DocumentBase doc, TablePr rowPr) : base(doc)
        {
            mTablePr = rowPr;
        }

        /// <summary>
        /// Returns <see cref="NodeType.Row"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.Row; }
        }

        /// <summary>
        /// Returns the immediate parent table of the row.
        /// </summary>
        /// <dev>
        /// Equivalent to <see cref="Node.FirstNonMarkupParentNode"/> casted to <see cref="Table"/>.
        /// </dev>
        public Table ParentTable
        {
            get { return (Table)FirstNonMarkupParentNode; }
        }

        /// <summary>
        /// True if this is the first row in a table; false otherwise.
        /// </summary>
        public bool IsFirstRow
        {
            get { return (this == ParentTable.FirstRow); }
        }

        /// <summary>
        /// True if this is the last row in a table; false otherwise.
        /// </summary>
        public bool IsLastRow
        {
            get { return (this == ParentTable.LastRow); }
        }

        /// <summary>
        /// True if this is the heading row of a table; false otherwise.
        /// </summary>
        /// <remarks>
        /// The first N rows which contains TableAttr.HeadingFormat attribute, are treated as heading
        /// rows. A conditional style with TableStyleOverrideType.FirstRow applies to these N rows.
        /// </remarks>
        internal bool IsHeadingRow
        {
            get
            {
                if (!RowFormat.HeadingFormat)
                    return false;

                return IsFirstRow || PreviousRow.IsHeadingRow;
            }
        }

        /// <summary>
        /// Gets the next <see cref="Row"/> node.
        /// </summary>
        /// <remarks>
        /// The method can be used when you need to have typed access to table rows. If a
        /// <see cref="StructuredDocumentTag"/> node is found in a table instead of a row,
        /// it is automatically traversed to get a row contained within.
        /// </remarks>
        public Row NextRow
        {
            get { return (Row)NextNonMarkupCompositeLimited; }
        }

        /// <summary>
        /// Gets the previous <see cref="Row"/> node.
        /// </summary>
        /// <remarks>
        /// The method can be used when you need to have typed access to table rows. If a
        /// <see cref="StructuredDocumentTag"/> node is found in a table instead of a row,
        /// it is automatically traversed to get a row contained within.
        /// </remarks>
        public Row PreviousRow
        {
            get { return (Row)PreviousNonMarkupCompositeLimited; }
        }

        /// <summary>
        /// Gets the index of this row in the parent collection or -1 if this row has no parent table..
        /// </summary>
        /// <remarks>
        /// Considers markup parent nodes.
        /// </remarks>
        internal int RowIndex
        {
            get
            {
                Table parentTable = ParentTable;

                // WORDSNET-28406 Multiple markup nesting prevents proper RoweIndex calculation.
                if (parentTable != null)
                {
                    for (int i = 0; i < parentTable.Rows.Count; i++)
                        if (this == parentTable.Rows[i])
                            return i;
                }

                return -1;
            }
        }

        /// <summary>
        /// Returns the first <see cref="Cell"/> in the row.
        /// </summary>
        public Cell FirstCell
        {
            // SPEED for RTF export. While the row is allowed to contain only cells this is a fast method for
            // getting first cell. However if we later allow bookmarks between cells, this has to be improved.
            // Same for LastCell.
            get { return (Cell)FirstNonMarkupCompositeDescendant; }
        }

        /// <summary>
        /// Returns the last <see cref="Cell"/> in the row.
        /// </summary>
        public Cell LastCell
        {
            get { return (Cell)LastNonMarkupCompositeDescendant; }
        }

        /// <summary>
        /// Provides typed access to the <see cref="Cell"/> child nodes of the row.
        /// </summary>
        public CellCollection Cells
        {
            get
            {
                if (mCells == null)
                    mCells = new CellCollection(this);
                return mCells;
            }
        }

        /// <summary>
        /// Provides access to the formatting properties of the row.
        /// </summary>
        public RowFormat RowFormat
        {
            get
            {
                if (mRowFormat == null)
                    mRowFormat = new RowFormat(this);
                return mRowFormat;
            }
        }

        /// <summary>
        /// Gets or sets a flag indicating whether this row is hidden or not.
        /// </summary>
        /// <remarks>
        /// Hidden row is not supported for WordML and ODT documents.
        /// </remarks>
        /// <dev>
        /// Corresponds to the 17.4.20 hidden attribute (Hidden Table Row Marker) [ISO/IEC 29500].
        /// </dev>
        public bool Hidden
        {
            get { return mTablePr.Hidden; }
            set
            {
                mTablePr.Hidden = value;
                // MS Word hides table row only if the cells contents and end-of-paragraph mark are also hidden.
                SetRowParagraphHidden(value);
            }
        }

        /// <summary>
        /// Hide or unhide all content in a row including paragraph.
        /// </summary>
        private void SetRowParagraphHidden(bool isHidden)
        {
            foreach (Cell cell in mCells)
            {
                foreach (Paragraph para in cell.Paragraphs)
                {
                    // Hide row paragraph.
                    para.ParagraphBreakRunPr.Hidden = isHidden ? AttrBoolEx.True : AttrBoolEx.False;

                    // Hide all content in the cell.
                    foreach (Run run in para.Runs)
                        run.Font.Hidden = isHidden;
                }
            }
        }

        internal TablePr TablePr
        {
            get { return mTablePr; }
            set
            {
                mTablePr = value;
            }
        }

        /// <summary>
        /// Indicates if the row has a valid cell spacing setting.
        /// </summary>
        /// <remarks>
        /// Spacing 0 and no spacing should be treated differently
        /// as tables with 0 spacing specified have layouts different from tables with no spacing.
        ///
        /// MS Word allows different spacing settings on the rows of the same tables.
        /// In the UI this can be the result of removing contents between tables with different spacing settings.
        ///
        /// Though spacing settings in oxml has type (similar to tcw),
        /// MS Word seems to disregard spacing settings with auto or pct types.
        /// This property reports false if such  spacing with a type other than Fixed is in the model.
        /// </remarks>
        internal bool HasCellSpacing
        {
            get
            {
                // Treat Auto and Pct spacing as no spacing.
                PreferredWidth spacingRaw = CellSpacingRaw;
                return (spacingRaw != null) && spacingRaw.IsFixed;
            }
        }

        /// <summary>
        /// Gets cell spacing value specified for the row, in twips.
        /// </summary>
        /// <remarks>
        /// The value is actually half of the spacing reported by MS Word UI as both MS Word and AW
        /// store half of the spacing in the model.
        ///
        /// The property should be used in conjunction with <see cref="HasCellSpacing"/>,
        /// to proper interpret 0 value.
        ///
        /// Negative spacings are interpreted as 0 spacing is specified, mimicking MS Word behavior.
        /// </remarks>
        internal int CellSpacing
        {
            get
            {
                // Treat negative spacing as 0.
                return HasCellSpacing
                    ? System.Math.Max(CellSpacingRaw.ValueTwips, 0)
                    : 0;
            }
        }

        /// <summary>
        /// Implements cell spacing attributes interpretation, the comments in <see cref="HasCellSpacing"/> and <see cref="CellSpacing"/>.
        /// </summary>
        private PreferredWidth CellSpacingRaw
        {
            get
            {
                // Do not use FetchRowAttr() or row, it reverts to defaults in absence of direct value,
                // and the defaults have 0 spacing defined for some reason (which is different from absence of the attribute).
                PreferredWidth spacingRaw = (PreferredWidth)((IRowAttrSource)this).GetDirectRowAttr(TableAttr.CellSpacing);
                // TODO consider removing the default.

                if (spacingRaw == null)
                {
                    TableStyle tableStyle = ParentTable.Style as TableStyle;
                    while(tableStyle != null)
                    {
                        spacingRaw = (PreferredWidth)((IRowAttrSource)tableStyle).GetDirectRowAttr(TableAttr.CellSpacing);
                        if (spacingRaw != null)
                            break;

                        tableStyle = tableStyle.GetBaseStyle() as TableStyle;
                    }
                }

                return spacingRaw;
            }
        }

        /// <summary>
        /// Attempts to fix invalid table style identifier.
        /// </summary>
        internal void EnsureTableStyleIsValid()
        {
            if (TablePr.Contains(TableAttr.Istd))
            {
                Style style = Document.Styles.GetByIstd(TablePr.Istd, false);

                if ((style == null) || (style.Type != StyleType.Table))
                {
                    // Use default Istd.
                    TablePr.Istd = StyleIndex.TableNormal;
                }
            }
        }

        internal override Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            Row lhs = (Row)base.Clone(isCloneChildren, cloningListener);
            lhs.mTablePr = mTablePr.Clone();
            lhs.mRowFormat = null;
            lhs.mCells = null;
            return lhs;
        }

        /// <summary>
        /// Returns true if either row or inner paragraph properties have same absolute positioning for two rows.
        /// </summary>
        /// <remarks>
        /// AM. DOC spec says:
        /// 2.4.3 Overview of Tables
        /// If neither table row specifies nondefault values for the preceding table position and wrapping properties,
        /// then two adjacent table rows of the same table depth are considered different tables if the first paragraphs
        /// of the first cells of the rows differ in any of the paragraph frame properties.
        /// </remarks>
        internal bool IsSameRowFloatingPositioning(Row rhs)
        {
            if (!TablePr.IsSameFloatingPositioning(rhs.TablePr))
                return false;

            // WORDSNET-21068 Do not compare floating of paragraphs in nested tables.
            if (!(TablePr.IsFloating || ParentTable.IsNested))
            {
                Cell cell1 = FirstCell;
                Cell cell2 = rhs.FirstCell;

                if (ArgumentUtil.BothAreNotNull(cell1, cell2))
                {
                    Paragraph para1 = (Paragraph) cell1.GetChild(NodeType.Paragraph, 0, true);
                    Paragraph para2 = (Paragraph) cell2.GetChild(NodeType.Paragraph, 0, true);

                    if (ArgumentUtil.BothAreNotNull(para1, para2))
                    {
                        ParaPr expParaPr1 = para1.GetExpandedParaPr(ParaPrExpandFlags.Layout);
                        ParaPr expParaPr2 = para2.GetExpandedParaPr(ParaPrExpandFlags.Layout);

                        return expParaPr1.IsSameFloatingPositioning(expParaPr2);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Breaks the table (that is the parent of the specified row) into two tables
        /// and moves all rows starting from the specified row into the new table.
        /// </summary>
        internal void BreakTableBefore()
        {
            Table table = ParentTable;

            // Create a new table and insert it after the existing table.
            Table newTable = new Table(table.Document);
            table.InsertNext(newTable);

            // Move all rows starting from the specified row to the new table.
            Node curRow = this;
            while (curRow != null)
            {
                Node nextRow = curRow.NextSibling;
                curRow.Remove();
                newTable.AppendChildForLoad(curRow);
                curRow = nextRow;
            }
        }


        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Node.Accept"]/*'/>
        /// <remarks>
        /// Calls <see cref="DocumentVisitor.VisitRowStart"/>, then calls <see cref="Node.Accept"/> for all child nodes of the section
        /// and calls <see cref="DocumentVisitor.VisitRowEnd"/> at the end.
        /// </remarks>
        public override bool Accept(DocumentVisitor visitor)
        {
            return AcceptCore(visitor);
        }

        /// <summary>
        /// Accepts a visitor for visiting the start of the row.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptStart(DocumentVisitor visitor)
        {
            return visitor.VisitRowStart(this);
        }

        /// <summary>
        /// Accepts a visitor for visiting the end of the row.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptEnd(DocumentVisitor visitor)
        {
            return visitor.VisitRowEnd(this);
        }

        /// <summary>
        /// Gets the text of all cells in this row including the end of row character.
        /// </summary>
        ///<remarks>
        ///<p>Returns concatenated text of all child nodes with the end of row character
        /// <see cref="ControlChar.Cell"/> appended at the end.</p>
        ///<p>The returned string includes all control and special characters as described in <see cref="ControlChar"/>.</p>
        ///</remarks>
        public override string GetText()
        {
            //Overridden here only so I can provide detailed API documentation.
            return base.GetText();
        }

        /// <summary>
        /// If the <see cref="Row"/> has no cells, creates and appends one <see cref="Cell"/>.
        /// </summary>
        public void EnsureMinimum()
        {
            Cell cell = FirstCell;
            if (cell == null)
                cell = (Cell)AppendChild(new Cell(Document));

            cell.EnsureMinimum();
        }

        /// <summary>
        /// I decided to keep MS Word-like behavior and represent end of row as \x0007,
        /// but I don't bother using \r for nested cells (that's MS Word's own hack anyway).
        ///
        /// The main reason I add end of row character is to allow to position the cursor using a flat
        /// integer position. If the end of row did not have a char - it would be impossible to distinguish
        /// from cursor inside the row at the end of it or at the beginning of the next row or paragraph.
        /// </summary>
        internal override string GetEndText()
        {
            return ControlChar.Cell;
        }

        /// <summary>
        /// Allows only cell-level elements to be inserted.
        /// </summary>
        internal override bool CanInsert(Node newChild)
        {
            return NodeUtil.IsCellLevelNode(newChild);
        }

        /// <summary>
        /// Copies <see cref="CellPr"/> from each <see cref="Cell"/> of <see cref="Cells"/> of this row
        /// to a corresponding cell in the collection passed.
        /// </summary>
        internal void CopyCellPrTo(CellCollection cells)
        {
            Debug.Assert(Cells.Count == cells.Count);
            for (int j = 0; j < cells.Count; j++)
            {
                CellPr source = Cells[j].CellPr;
                if (source != null)
                    cells[j].CellPr = source.Clone();
            }
        }

        /// <summary>
        /// Removes floating attributes from all paragraphs within the row.
        /// </summary>
        /// <remarks>
        /// AM. DOC/RTF have paragraph floating attributes which repeat corresponding floating attributes of table.
        /// If we leave them we should change them along with table attributes otherwise output is not OK.
        /// I decided just remove them upon DOC/RTF import if paragraph is in floating table, this is the way floating table stored in DOCX, so
        /// we get more unified Model regardless format.
        /// Output is seems to be good.
        /// </remarks>
        internal void RemoveParagraphFloatingAttributes()
        {
            foreach (Paragraph paragraph in GetChildNodes(NodeType.Paragraph, true))
                paragraph.ParaPr.RemoveFloatingAttrs();
        }

        #region IRowAttrSource

        object IRowAttrSource.GetDirectRowAttr(int key)
        {
            return mTablePr.GetDirectAttr(key);
        }

        object IRowAttrSource.FetchRowAttr(int key)
        {
            object value = mTablePr[key];
            return (value != null) ? value : ((IRowAttrSource)this).FetchInheritedRowAttr(key);
        }

        object IRowAttrSource.FetchInheritedRowAttr(int key)
        {
            if (ParentTable != null)
            {
                TableStyle tableStyle = ParentTable.Style as TableStyle;
                if (tableStyle != null)
                    return ((IRowAttrSource)tableStyle).FetchRowAttr(key);
            }
            return TablePr.FetchDefaultAttr(key);
        }

        void IRowAttrSource.SetRowAttr(int key, object value)
        {
            mTablePr.SetAttr(key, value);
        }

        void IRowAttrSource.ClearRowAttrs()
        {
            mTablePr.Clear();
        }

        void IRowAttrSource.ResetToDefaultAttrs()
        {
            ((IRowAttrSource)mTablePr).ResetToDefaultAttrs();
        }

        #endregion

        /// <summary>
        /// Gets a boolean value indicating if the row has the given attribute specified directly or via table style.
        /// </summary>
        internal bool HasAttribute(int key)
        {
            object value = ((IRowAttrSource)this).GetDirectRowAttr(key);
            if (value != null)
                return true;

            TableStyle tableStyle = ParentTable.Style as TableStyle;
            if (tableStyle == null)
                return false;

            value = ((IRowAttrSource)tableStyle).GetDirectRowAttr(key);

            return (value != null);
        }

        /// <summary>
        /// Removes indent for aligned row.
        /// </summary>
        /// <remarks>
        /// See WORDSNET-25666.
        /// </remarks>
        internal void ValidateAlignedIndent()
        {
            if (!Document.DocPr.CompatibilityOptions.IsWord2013OrLaterCompatible)
                return;

            if (TablePr.Alignment != TableAlignment.Left)
                TablePr.Remove(TableAttr.LeftIndent);

            if (TablePr.HasFormatRevision)
            {
                TablePr revPr = (TablePr)TablePr.FormatRevision.RevPr;

                TableAlignment revAlignment = revPr.ContainsKey(TableAttr.Alignment)
                    ? revPr.Alignment
                    : TablePr.Alignment;

                if (revAlignment != TableAlignment.Left)
                    revPr.Remove(TableAttr.LeftIndent);
            }
        }

        /// <summary>
        /// Specifies an identifier for the row that is unique within the document part.
        /// </summary>
        internal int ParaId
        {
            get { return mParaId; }
            set { mParaId = value; }
        }

        /// <summary>
        /// Specifies version identifier for the row.
        /// </summary>
        internal int TextId
        {
            get { return mTextId; }
            set { mTextId = value; }
        }

        EditRevision ITrackableNode.InsertRevision
        {
            get { return TablePr.InsertRevision; }
            set { TablePr.InsertRevision = value; }
        }

        EditRevision ITrackableNode.DeleteRevision
        {
            get { return TablePr.DeleteRevision; }
            set { TablePr.DeleteRevision = value; }
        }

        MoveRevision IMoveTrackableNode.MoveFromRevision
        {
            get { return TablePr.MoveFromRevision; }
            set { TablePr.MoveFromRevision = value; }
        }

        MoveRevision IMoveTrackableNode.MoveToRevision
        {
            get { return TablePr.MoveToRevision; }
            set { TablePr.MoveToRevision = value; }
        }

        void IMoveTrackableNode.RemoveMoveRevisions()
        {
            TablePr.Remove(RevisionAttr.MoveFromRevision);
            TablePr.Remove(RevisionAttr.MoveToRevision);
        }

        /// <summary>
        /// Specifies an identifier for the row that is unique within the document part. See [MS-DOCX] 2.4.1 paraId.
        /// </summary>
        /// <remarks>
        /// AM. See remarks in Paragraph.mParaId
        /// </remarks>
        private int mParaId;

        /// <summary>
        /// Specifies version identifier for the row.
        /// </summary>
        private int mTextId;

        private TablePr mTablePr;
        private RowFormat mRowFormat;
        private CellCollection mCells;
    }
}
