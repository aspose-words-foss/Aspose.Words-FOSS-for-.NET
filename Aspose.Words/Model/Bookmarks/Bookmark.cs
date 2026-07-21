// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/06/2004 by Roman Korchagin

using Aspose.Words.Fields;
using Aspose.Words.Revisions;
using Aspose.Words.Tables;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a single bookmark.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-bookmarks/">Working with Bookmarks</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p><see cref="Bookmark"/> is a "facade" object that encapsulates two nodes <see cref="BookmarkStart"/>
    /// and <see cref="BookmarkEnd"/> in a document tree and allows to work with a bookmark as a single object.</p>
    /// </remarks>
    public class Bookmark
    {
        /// <summary>
        /// Creates a <see cref="Bookmark"/> instance with defined start and lazily initialized end.
        /// </summary>
        internal Bookmark(BookmarkStart bookmarkStart)
        {
            ArgumentUtil.CheckNotNull(bookmarkStart, "bookmarkStart");
            mBookmarkStart = bookmarkStart;
        }

        /// <summary>
        /// Creates a <see cref="Bookmark"/> instance with defined start and end.
        /// </summary>
        internal Bookmark(BookmarkStart bookmarkStart, BookmarkEnd bookmarkEnd)
            : this(bookmarkStart)
        {
            ArgumentUtil.CheckNotNull(bookmarkEnd, "bookmarkEnd");
            mBookmarkEnd = bookmarkEnd;
        }

        /// <summary>
        /// Gets or sets the name of the bookmark.
        /// </summary>
        /// <remarks>
        /// Note that if you change the name of a bookmark to a name that already exists in the document,
        /// no error will be given and only the first bookmark will be stored when you save the document.
        /// </remarks>
        public string Name
        {
            get { return BookmarkStart.Name; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");

                //Need to change end name before start name since bookmark end is looked up using start name.
                BookmarkEnd.SetNameInternal(value);
                BookmarkStart.SetNameInternal(value);
            }
        }

        /// <summary>
        /// Gets or sets the text enclosed in the bookmark.
        /// </summary>
        public string Text
        {
            get
            {
                return GetText(false);
            }
            set
            {
                ArgumentUtil.CheckNotNull(value, "value");

                if (IsColumn)
                    SetColumnBookmarkText(value);
                else
                    SetTextInternal(value);
            }
        }

        private void SetTextInternal(string text)
        {
            UntangleStart();
            UntangleEnd();

            RunPr sourceRunPr = GetSourceRunPr();

            RangeBound startBound = AnnotationUtil.GetAnnotationRangeBound(BookmarkStart, true);
            // If DisplacedBy has incorrect value, just reset it for now.
            if (startBound == null)
            {
                BookmarkStart.DisplacedBy = DisplacedByType.Unspecified;
                startBound = AnnotationUtil.GetAnnotationRangeBound(BookmarkStart, true);
            }

            RangeBound endBound = AnnotationUtil.GetAnnotationRangeBound(BookmarkEnd, false);
            // If DisplacedBy has incorrect value, just reset it for now.
            if (endBound == null)
            {
                BookmarkEnd.DisplacedBy = DisplacedByType.Unspecified;
                endBound = AnnotationUtil.GetAnnotationRangeBound(BookmarkEnd, false);
            }

            Node insertPosition = BookmarkEnd;
            NodeRange nodeRange = CreateNodeRange(startBound, endBound, null);

            if ((startBound != null) && (endBound != null))
            {
                // Position before paragraph equals position at first child of paragraph, so move end into nearest
                // paragraph for correct paragraph removal and merging like in inline bookmarks mode.
                // (Bookmark end is expected to be at inline level at this case in result.)
                // Do the same with all other annotations at this position to prevent them from being removed.
                if ((endBound.Node == BookmarkEnd) &&
                    (BookmarkEnd.DisplacedBy == DisplacedByType.Unspecified) &&
                    (BookmarkEnd.NodeLevel != NodeLevel.Inline) &&
                    (endBound.Node.NextNonAnnotationSibling != startBound.Node.NextNonAnnotationSibling))
                {
                    MoveAnnotationsToNextParagraph(BookmarkEnd);
                }

                BookmarkSafeNodeRemover remover =
                    new BookmarkSafeNodeRemover(nodeRange, NodeJoinMode.JoinToNextSibling, true);
                remover.RemoveCore();
                RestoreRemovedStartEnd(startBound, endBound);
                insertPosition = PrepareTextInsertPosition();
            }

            // Insert the new text.
            // todo 0 rk document builder does not work with glossary yet
            DocumentBuilder builder = new DocumentBuilder(BookmarkStart.FetchDocument());
            builder.MoveTo(insertPosition);

            sourceRunPr = sourceRunPr.Clone();
            RevisionUtil.RemoveEditRevisions(sourceRunPr);
            builder.SetFont(sourceRunPr, false);

            InsertText(builder, text);
        }

        private static void InsertText(DocumentBuilder builder, string text)
        {
            // DocumentBuilder.Write(String) is not used for the entire text as it inserts new paragraphs after the current one,
            // but to mimic MS Word, new paragraphs should be inserted before the current one. Also special processing
            // for revision tracking is needed.

            // Convert CrLf and Lf into Crs.
            string normalizedText = WordUtil.NormalizeToWord(text);

            // If text had any Crs insert paragraphs as appropriate.
            int paragraphStart = 0;
            while (paragraphStart <= normalizedText.Length)
            {
                int paragraphEnd = normalizedText.IndexOf(ControlChar.ParagraphBreakChar, paragraphStart);
                bool isParagraphBreakCharFound = (paragraphEnd >= 0);
                if (!isParagraphBreakCharFound)
                    paragraphEnd = normalizedText.Length;

                int length = paragraphEnd - paragraphStart;
                if (length > 0)
                    builder.Write(normalizedText.Substring(paragraphStart, length));

                if (isParagraphBreakCharFound)
                    InsertParagraph(builder);

                paragraphStart = paragraphEnd + 1;
            }
        }

        private static void InsertParagraph(DocumentBuilder builder)
        {
            Document document = builder.Document;
            Paragraph currentParagraph = builder.CurrentParagraph;

            // New paragraph gets a copy of the current paragraph attributes.
            Paragraph newPara = new Paragraph(document, builder.GetParaPrCopy(), builder.GetRunPrCopy());
            RevisionUtil.RemoveEditRevisions(newPara.ParagraphBreakRunPr);

            // Add new paragraph node before the current paragraph.
            currentParagraph.InsertPrevious(newPara);

            // Move nodes before the insertion point to the new paragraph.
            using (new SuspendTrackRevisionsDocument(document))
                newPara.InsertBefore(currentParagraph.FirstChild, builder.CurrentNode, null);
        }

        private void SetColumnBookmarkText(string text)
        {
            Debug.Assert(text != null);

            // Word ignores end of cell character.
            text = text.Replace(ControlChar.Cell, "");

            // WORDSNET-24087 Take sibling Row as a parent in case when
            // bookmark is placed just inside a Table to mimic Word.
            Row row = (BookmarkStart.ParentNode.NodeType == NodeType.Table)
                ? (Row)BookmarkStart.NextSiblingOfType(NodeType.Row)
                : BookmarkStart.GetAncestor(NodeType.Row) as Row;

            if (row != null)
            {
                // Word throws an exception here. Maybe we should throw an exception too.
                if ((FirstColumn >= row.Cells.Count) && (LastColumn >= row.Cells.Count))
                    return;

                int column = LastColumn > FirstColumn ? FirstColumn : LastColumn;

                Cell cell = row.Cells[column];
                Paragraph para = cell.FirstParagraph;
                RunPr runPr = para.Runs.Count > 0 ? para.FirstRun.RunPr : new RunPr();

                PrepareCell(cell);

                if (!string.IsNullOrEmpty(text))
                {
                    Run run = new Run(BookmarkStart.Document, text, runPr);
                    para.AppendChild(run);
                }

                cell.InsertAfter(para, null);
            }
        }

        private void PrepareCell(Cell cell)
        {
            Paragraph firstPara = cell.FirstParagraph;

            // Word moves bookmarkStarts to the first paragraph from other paragraphs.
            for (int i = 1; i < cell.Paragraphs.Count; i++)
            {
                foreach (Node child in cell.Paragraphs[i])
                {
                    if (child.NodeType == NodeType.BookmarkStart)
                        firstPara.AppendChild(child);
                }
            }

            cell.GetChildNodes(NodeType.Any, false).Clear();

            // Prepare first paragraph.
            foreach (Node child in firstPara.GetChildNodes(NodeType.Any, false))
                if (!NodeUtil.IsBookmarkNode(child))
                    child.Remove();
        }

        private RangeBound GetRangeBound(bool forRangeStart)
        {
            Node bookmarkBound = forRangeStart ? (Node)BookmarkStart : BookmarkEnd;

            return IsColumn
                ? GetColumnRangeBound(forRangeStart)
                : AnnotationUtil.GetAnnotationRangeBound(bookmarkBound, forRangeStart);
        }

        private RangeBound GetColumnRangeBound(bool forRangeStart)
        {
            Debug.Assert(IsColumn);

            Cell cellBound = forRangeStart ? GetBookmarkStartCellBound() : GetBookmarkEndCellBound();

            return cellBound == null
                ? null
                : new RangeBound(cellBound, true, null, null);
        }

        private Cell GetBookmarkEndCellBound()
        {
            Cell cellBound = null;
            Row row = (Row)BookmarkEnd.GetAncestor(NodeType.Row);
            Table table = (Table)BookmarkStart.GetAncestor(NodeType.Table);

            // Make sure ancestor row does not belong to outer table.
            if ((row != null) && (row.ParentTable != table))
                row = null;

            // BookmarkEnd can be located between the rows, after the last row and in the paragraph after the table.
            if (row == null)
            {
                row = BookmarkEnd.PreviousNonAnnotationSibling as Row;

                if ((row == null) && (table != null))
                    row = table.LastRow;
            }

            if (row != null)
            {
                if ((FirstColumn >= row.Cells.Count) && (LastColumn >= row.Cells.Count))
                    return null;

                bool lastColumnOutOfRange = row.Cells.Count <= LastColumn;
                cellBound = lastColumnOutOfRange ? row.LastCell : row.Cells[LastColumn];

                Row bookmarkStartRow = BookmarkStart.GetAncestor(NodeType.Row) as Row;
                if ((bookmarkStartRow != null) && bookmarkStartRow.IsAbove(row) && BookmarkEnd.IsAbove(cellBound))
                {
                    row = row.PreviousRow;
                    cellBound = lastColumnOutOfRange ? row.LastCell : row.Cells[LastColumn];
                }
            }

            Debug.Assert(cellBound != null);
            return cellBound;
        }

        private Cell GetBookmarkStartCellBound()
        {
            Cell cellBound = null;
            Row row = BookmarkStart.GetAncestor(NodeType.Row) as Row;

            if (row != null)
            {
                if ((FirstColumn >= row.Cells.Count) && (LastColumn >= row.Cells.Count))
                    return null;

                cellBound = LastColumn < FirstColumn ? row.Cells[LastColumn] : row.Cells[FirstColumn];
            }

            Debug.Assert(cellBound != null);
            return cellBound;
        }

        /// <summary>
        /// Returns text enclosed in the bookmark.
        /// </summary>
        /// <param name="isFieldResultMode">
        /// <c>true</c> to include field results only, <c>false</c> to include all field parts that includes field code, field result
        /// and field control characters.
        /// </param>
        /// <returns></returns>
        internal string GetText(bool isFieldResultMode)
        {
            RangeBound startBound = GetRangeBound(true);
            RangeBound endBound = GetRangeBound(false);
            if ((startBound == null) || (endBound == null))
                return "";

            return NodeTextCollector.GetText(startBound.Node, startBound.IsNodeIncluded,
                endBound.Node, endBound.IsNodeIncluded, isFieldResultMode);
        }

        /// <summary>
        /// It is possible to for many bookmark starts and ends in any order to occur at one position in a document.
        /// It means that our bookmark can "entangle" some of those starts and ends both at the start and end.
        /// When we are setting (or deleting) the bookmark text, we don't want to delete starts or ends of such
        /// heighbouring bookmarks. This method tries to "untangle" bookmark starts and ends at the same position.
        ///
        /// Untangling start means moving our bookmark start to be after all bookmark starts and ends at the given position.
        /// Untangling end means moving our bookmark end to be before all bookmark starts and ends at the given position.
        /// </summary>
        private void UntangleStart()
        {
            if (BookmarkStart == null)
                return;

            // Loop forward until we reach end of a parent or a non bookmark node.
            Node curNode = BookmarkStart;
            do
            {
                curNode = curNode.NextSibling;
            }
            while ((curNode != null) && IsBookmarkNodeToSkip(curNode));

            // CurNode is now pointing to a first non-bookmark node or end of parent.
            // Move the bookmark start to be just before that node unless it is already there.
            if (BookmarkStart.NextSibling != curNode)
                BookmarkStart.ParentNode.InsertBefore(BookmarkStart, curNode);
        }

        /// <summary>
        /// This is a complete reverse of <see cref="UntangleStart"/>
        /// </summary>
        private void UntangleEnd()
        {
            Node curNode = BookmarkEnd;
            do
            {
                curNode = curNode.PreviousSibling;
            }
            while ((curNode != null) && IsBookmarkNodeToSkip(curNode));

            if (BookmarkEnd.PreviousSibling != curNode)
                BookmarkEnd.ParentNode.InsertAfter(BookmarkEnd, curNode);
        }

        /// <summary>
        /// We skip bookmark nodes that belong to different bookmarks.
        /// </summary>
        private bool IsBookmarkNodeToSkip(Node node)
        {
            IBookmarkNode otherBookmark = node as IBookmarkNode;
            return (otherBookmark != null) && !StringUtil.EqualsIgnoreCase(Name, otherBookmark.Name);
        }

        /// <summary>
        /// Gets source run properties for bookmark text.
        /// </summary>
        /// <returns></returns>
        private RunPr GetSourceRunPr()
        {
            Node node;
            Paragraph parentParagraph;
            if (mBookmarkStart.NodeLevel != NodeLevel.Inline)
            {
                parentParagraph = NodeUtil.FindNextParagraph(mBookmarkStart);
                if (parentParagraph == null)
                    return new RunPr();

                node = parentParagraph.FirstNonMarkupDescendant;
            }
            else
            {
                parentParagraph = (Paragraph)mBookmarkStart.GetAncestor(NodeType.Paragraph);
                node = mBookmarkStart.NextNonMarkupNodeLimited;
            }

            while ((node != null) && !(node is Inline))
                node = node.NextNonMarkupNodeLimited;

            if (node != null)
                return ((Inline)node).RunPr;

            return parentParagraph.ParagraphBreakRunPr;
        }

        /// <summary>
        /// Gets the node that represents the start of the bookmark.
        /// </summary>
        public BookmarkStart BookmarkStart
        {
            get { return mBookmarkStart; }
        }

        /// <summary>
        /// Gets the node that represents the end of the bookmark.
        /// </summary>
        public BookmarkEnd BookmarkEnd
        {
            get
            {
                if (mBookmarkEnd == null)
                    mBookmarkEnd = BookmarkFinder.FetchBookmarkEnd(mBookmarkStart.GetTopmostAncestor(), Name, mBookmarkStart);

                return mBookmarkEnd;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this bookmark is a table column bookmark.
        /// </summary>
        public bool IsColumn
        {
            get { return mBookmarkStart.IsColumn; }
        }

        /// <summary>
        /// Gets the zero-based index of the first column of the table column range associated with the bookmark.
        /// </summary>
        /// <remarks>
        /// Returns <b>-1</b> if this bookmark is not a table column bookmark.
        /// </remarks>
        public int FirstColumn
        {
            get { return mBookmarkStart.FirstColumn; }
        }

        /// <summary>
        /// Gets the zero-based index of the last column of the table column range associated with the bookmark.
        /// </summary>
        /// <remarks>
        /// Returns <b>-1</b> if this bookmark is not a table column bookmark.
        /// </remarks>
        public int LastColumn
        {
            get { return mBookmarkStart.LastColumn; }
        }

        /// <summary>
        /// Removes the bookmark from the document. Does not remove text inside the bookmark.
        /// </summary>
        public void Remove()
        {
            BookmarkEnd.Remove();
            BookmarkStart.Remove();
        }

        /// <summary>
        /// Gets a node range starting at <see cref="BookmarkStart"/> and ending at <see cref="BookmarkEnd"/>.
        /// Range start and end points are excluded.
        /// </summary>
        internal NodeRange GetNodeRange()
        {
            return GetNodeRange(BookmarkStart, BookmarkEnd);
        }

        /// <summary>
        /// Gets a node range between <see cref="BookmarkStart"/> and <see cref="BookmarkEnd"/> and includes sibling bookmark nodes.
        /// </summary>
        internal NodeRange GetMostWideNodeRange()
        {
            NodeRange range = GetNodeRange();

            Node start = range.Start.Node;
            if ((start.NodeType == NodeType.BookmarkStart) && !((BookmarkStart)start).IsColumn)
            {
                while ((start.PreviousSibling != null) &&
                    (start.PreviousSibling.NodeType == NodeType.BookmarkStart) &&
                    !((BookmarkStart)start.PreviousSibling).IsColumn)
                {
                    start = start.PreviousSibling;
                }
            }

            Node end = range.End.Node;
            if (end.NodeType == NodeType.BookmarkEnd)
                while (end.NextSibling != null && end.NextSibling.NodeType == NodeType.BookmarkEnd)
                    end = end.NextSibling;

            return new NodeRange(start, start.NodeType == NodeType.BookmarkStart, end, end.NodeType == NodeType.BookmarkEnd, start as BookmarkStart);
        }

        internal static NodeRange GetNodeRange(BookmarkStart bookmarkStart, BookmarkEnd bookmarkEnd)
        {
            RangeBound startBound = AnnotationUtil.GetAnnotationRangeBound(bookmarkStart, true);
            RangeBound endBound = bookmarkEnd.DisplacedBy != DisplacedByType.Unspecified
                ? AnnotationUtil.GetAnnotationRangeBound(bookmarkEnd, false)
                : new RangeBound(GetNodeRangeEndNode(bookmarkStart, bookmarkEnd), false);
            if ((startBound == null) || (endBound == null))
                return NodeRange.Void;

            NodeRange range = CreateNodeRange(startBound, endBound, bookmarkStart);

            int fieldCharCount = 0;
            foreach (Node node in range)
            {
                bool isFieldCodeEnd = false;
                switch (node.NodeType)
                {
                    case NodeType.FieldStart:
                        fieldCharCount++;
                        break;
                    case NodeType.FieldSeparator:
                        isFieldCodeEnd = true;
                        break;
                    case NodeType.FieldEnd:
                        isFieldCodeEnd = !((FieldEnd)node).HasSeparator;
                        break;
                    default:
                        // Do nothing.
                        break;
                }

                if (isFieldCodeEnd)
                {
                    if (fieldCharCount == 0)
                    {
                        // The bookmark starts within a field code. Let's complete or truncate the range from start.
                        Node startNode = GetNodeRangeStartNode(startBound, node);
                        return new NodeRange(startNode, true, endBound.Node, endBound.IsNodeIncluded, bookmarkStart);
                    }

                    fieldCharCount--;
                }
            }

            return range;
        }

        /// <summary>
        /// Creates node range for the specified bounds.
        /// </summary>
        private static NodeRange CreateNodeRange(RangeBound startBound, RangeBound endBound, BookmarkStart bookmarkStart)
        {
            return new NodeRange(startBound.Node, startBound.IsNodeIncluded,
                endBound.Node, endBound.IsNodeIncluded, bookmarkStart);
        }

        private static Node GetNodeRangeEndNode(BookmarkStart bookmarkStart, BookmarkEnd bookmarkEnd)
        {
            // If a bookmark end is at beginning of the first row or cell of a table, it is equivalent to
            // a bookmark before the table: the table should not be included in the range.
            if (((bookmarkEnd.ParentNode.NodeType != NodeType.Paragraph) ||
                    !NodeUtil.IsCellLevelNode(bookmarkEnd.ParentNode.ParentNode)) &&
                !NodeUtil.IsCellLevelNode(bookmarkEnd.ParentNode) &&
                !NodeUtil.IsRowLevelNode(bookmarkEnd.ParentNode) &&
                (bookmarkEnd.NodeLevel != NodeLevel.Row))
            {
                return bookmarkEnd;
            }

            // Move upward until:
            // - a parent which is not the first child (excluding other bookmark and comment starts/ends) itself is met or
            // - a parent which contains the specified bookmark start is met.
            for (Node endNode = bookmarkEnd; ; endNode = endNode.ParentNode) // return inside.
            {
                for (Node node = endNode.ParentNode.FirstChild; node != endNode; node = node.NextSibling)
                {
                    if ((node == bookmarkStart) || !NodeUtil.IsCrossStructureAnnotation(node))
                        return endNode;
                }
            }
        }

        /// <summary>
        /// Gets correct start node of bookmark range if the bookmark is in a field.
        /// </summary>
        private static Node GetNodeRangeStartNode(RangeBound startBound, Node fieldCodeEnd)
        {
            // If the first item of a bookmark is a field, then the bookmark's start is sometimes placed after the field's
            // start by MS Word. But we need to consider such fields either. On the other hand, if the bookmark starts
            // inside a field code, it may not occupy the whole field, so we should not consider the field in this case.
            // So let's move backward from the bookmark's start until any inline node and if this node is the field's start,
            // then consider the whole field, otherwise consider its result only. Note, that other cross-structure annotation
            // nodes should be ignored while searching.
            //
            // This can make the bookmark start fall within the result node range. However, it seems like there is nothing
            // bad in this.
            DocumentPosition position = startBound.IsNodeIncluded
                ? DocumentPosition.CreatePositionBefore(startBound.Node)
                : DocumentPosition.CreatePositionAfter(startBound.Node);
            while (position.Move(null, false, true, true, false, false))
            {
                Node node = position.Node;
                if (NodeUtil.IsInlineLevelNode(node) && !NodeUtil.IsCrossStructureAnnotation(node))
                    return (node.NodeType == NodeType.FieldStart) ? node : fieldCodeEnd;
            }

            return fieldCodeEnd;
        }

        /// <summary>
        /// If bookmark start or end are removed during removal of bookmark range, this method restores them
        /// at correct positions.
        /// </summary>
        /// <dev>
        /// The bookmark nodes may be removed because of the following. Real range of bookmarks with defined
        /// displacedByCustomXml attribute differ from range driven by bookmark position. So, bookmark nodes may
        /// be included in real bookmark range and may be removed during removing nodes of bookmark range.
        /// For example, real range of this bookmark:
        /// <sdt>
        /// <sdtContent>
        ///   <p>
        ///     <r>
        ///     </r>
        ///   </p>
        /// </sdtContent>
        /// </sdt>
        /// <bookmarkStart displacedByCustomXml = "prev" />
        /// is started just after the run. The bookmark will be removed on removing nodes of the bookmark range.
        /// We need to restore it after removing the range nodes.
        /// </dev>
        private void RestoreRemovedStartEnd(RangeBound startOfRemovedRange, RangeBound endOfRemovedRange)
        {
            if (IsNodeRemoved(BookmarkStart))
            {
                Node sdtNode = startOfRemovedRange.DisplacingNode;
                if ((sdtNode != null) && !IsNodeRemoved(sdtNode))
                {
                    if (sdtNode.NodeType == NodeType.StructuredDocumentTag)
                    {
                        // StructuredDocumentTag.
                        CompositeNode sdt = (CompositeNode)sdtNode;
                        InsertAtDisplacedSdt(BookmarkStart, sdt, startOfRemovedRange.IsAnnotationAChildOfSdt);
                    }
                    else
                    {
                        // StructuredDocumentTagRangeStart or StructuredDocumentTagRangeEnd.
                        sdtNode.ParentNode.Insert(BookmarkStart, sdtNode,
                            BookmarkStart.DisplacedBy == DisplacedByType.Prev);
                    }
                }
                else
                {
                    startOfRemovedRange.Parent.Insert(BookmarkStart, startOfRemovedRange.IsNodeIncluded ?
                        startOfRemovedRange.PreviousSibling : startOfRemovedRange.Node, true);

                    BookmarkStart.DisplacedBy = DisplacedByType.Unspecified;
                }
            }

            if (IsNodeRemoved(BookmarkEnd))
            {
                Node sdtNode = endOfRemovedRange.DisplacingNode;
                if ((sdtNode != null) && !IsNodeRemoved(sdtNode))
                {
                    if (sdtNode.NodeType == NodeType.StructuredDocumentTag)
                    {
                        // StructuredDocumentTag.
                        CompositeNode sdt = (CompositeNode)sdtNode;
                        InsertAtDisplacedSdt(BookmarkEnd, sdt, endOfRemovedRange.IsAnnotationAChildOfSdt);
                    }
                    else
                    {
                        // StructuredDocumentTagRangeStart or StructuredDocumentTagRangeEnd.
                        sdtNode.ParentNode.Insert(BookmarkEnd, sdtNode, BookmarkEnd.DisplacedBy == DisplacedByType.Prev);
                    }
                }
                else
                {
                    endOfRemovedRange.Parent.Insert(BookmarkEnd, endOfRemovedRange.IsNodeIncluded ?
                        endOfRemovedRange.NextSibling : endOfRemovedRange.Node, false);

                    BookmarkEnd.DisplacedBy = DisplacedByType.Unspecified;
                }
            }
        }

        /// <summary>
        /// Returns <c>true</c> if the specified node has been removed from a document.
        /// </summary>
        private static bool IsNodeRemoved(Node node)
        {
            return node.GetAncestor(NodeType.Document) == null;
        }

        /// <summary>
        /// Moves the specified annotation and everyone else in the same position at block level to the nearest paragraph.
        /// </summary>
        private static void MoveAnnotationsToNextParagraph(Node annotation)
        {
            Debug.Assert(NodeUtil.IsCrossStructureAnnotation(annotation));
            Debug.Assert(annotation.NodeLevel != NodeLevel.Inline);

            // Get interval of annotation nodes in this position.
            Node previousNonAnnotation = annotation.PreviousNonAnnotationSibling;
            Node startAnnotation = (previousNonAnnotation == null)
                ? annotation.ParentNode.FirstChild
                : previousNonAnnotation.NextSibling;
            Node nextNonAnnotation = annotation.NextNonAnnotationSibling;

            // Find destination node.
            CompositeNode nextNode = annotation.NextNonMarkupCompositeLimited;
            Node newPositionNode = (nextNode != null) ? AnnotationUtil.GetDeepestChild(nextNode, true) : null;
            if (newPositionNode != null)
            {
                // Insert into a composite node (e.g. a paragraph or inline SDT), but not into a shape or comment.
                if (newPositionNode.IsComposite && !NodeUtil.IsStoryNodeType(newPositionNode))
                    ((CompositeNode)newPositionNode).InsertAfter(startAnnotation, nextNonAnnotation, null);
                else
                    newPositionNode.InsertPrevious(startAnnotation, nextNonAnnotation);
            }
        }

        /// <summary>
        /// Gets a node to insert text before it on setting the <see cref="Text"/> property.
        /// Also the bookmark start or end may be moved for new text to be placed inside bookmark range
        /// (for new text to be included in result on getting from the <see cref="Text"/> property).
        /// </summary>
        private Node PrepareTextInsertPosition()
        {
            Node insertPosition = BookmarkEnd;

            // Move in accordance with DisplacedBy.
            if (BookmarkEnd.DisplacedBy != DisplacedByType.Unspecified)
            {
                RangeBound endBound = AnnotationUtil.GetAnnotationRangeBound(BookmarkEnd, false);
                if (endBound != null)
                {
                    Node endNode = endBound.Node;
                    bool isAfterEndNode = endBound.IsNodeIncluded;

                    if (endNode != BookmarkEnd)
                    {
                        bool needMoveEnd =
                            ((BookmarkEnd.DisplacedBy == DisplacedByType.Next) &&
                                (endBound.DisplacingNode != null) &&
                                !BookmarkEnd.IsAncestorNode(endBound.DisplacingNode)) ||
                            ((BookmarkEnd.DisplacedBy == DisplacedByType.Prev) &&
                                (endBound.DisplacingNode != null) &&
                                BookmarkEnd.IsAncestorNode(endBound.DisplacingNode));
                        if (needMoveEnd)
                        {
                            endNode.ParentNode.Insert(BookmarkEnd, endNode, isAfterEndNode);
                            BookmarkEnd.DisplacedBy = DisplacedByType.Unspecified;
                        }
                        else
                        {
                            insertPosition = endBound.Node;
                            if (isAfterEndNode)
                            {
                                insertPosition = insertPosition.NextOrParent;
                                //if (insertPosition == BookmarkStart)
                                //    insertPosition = insertPosition.NextOrParent;
                            }
                        }
                    }
                }
            }

            // Move bookmark start for inserting text to be after it.
            RangeBound startBound = AnnotationUtil.GetAnnotationRangeBound(BookmarkStart, true);
            if (startBound != null)
            {
                Node startNode = startBound.IsNodeIncluded ? startBound.Node : startBound.Node.NextOrParent;

                bool needMoveStart = insertPosition.IsAbove(startNode) && !startNode.IsAncestorNode(insertPosition);
                if (!needMoveStart && (insertPosition == startNode))
                {
                    needMoveStart =
                        ((BookmarkStart.DisplacedBy == DisplacedByType.Prev) &&
                                (startBound.DisplacingNode != null) &&
                                !BookmarkStart.IsAncestorNode(startBound.DisplacingNode)) ||
                        ((BookmarkStart.DisplacedBy == DisplacedByType.Next) &&
                            (startBound.DisplacingNode != null) &&
                            BookmarkStart.IsAncestorNode(startBound.DisplacingNode));
                }

                if (needMoveStart)
                {
                    if (insertPosition.IsComposite)
                        ((CompositeNode)insertPosition).AppendChild(BookmarkStart);
                    else
                        insertPosition.ParentNode.Insert(BookmarkStart, insertPosition, false);

                    BookmarkStart.DisplacedBy = DisplacedByType.Unspecified;
                }
            }

            // Move to inline level.
            if ((insertPosition.NodeType != NodeType.Paragraph) && (insertPosition.NodeLevel != NodeLevel.Inline))
            {
                Paragraph paragraph = NodeUtil.FindNextParagraph(insertPosition);
                if (paragraph == null)
                {
                    Node story = insertPosition.GetStoryAncestor(NodeType.Body);
                    if (story == null)
                        story = insertPosition.GetStoryAncestor(NodeType.Any);

                    if ((story != null) && story.IsComposite)
                    {
                        CompositeNode compositeStory = (CompositeNode)story;
                        paragraph = new Paragraph(BookmarkEnd.Document);
                        if (compositeStory.CanInsert(paragraph))
                            compositeStory.AppendChild(paragraph);
                        else
                            paragraph = null;
                    }
                }
                if (paragraph != null)
                {
                    paragraph.Insert(BookmarkEnd, null, true);
                    BookmarkEnd.DisplacedBy = DisplacedByType.Unspecified;
                }
            }

            return insertPosition;
        }

        /// <summary>
        /// Inserts the specified bookmark node at correct position relatively to its displaced SDT.
        /// </summary>
        private static void InsertAtDisplacedSdt(Node bookmarkNode, CompositeNode sdt, bool asChild)
        {
            DisplacedByType displacedBy = ((IDisplaceableByCustomXml)bookmarkNode).DisplacedByCustomXml;
            switch (displacedBy)
            {
                case DisplacedByType.Next:
                case DisplacedByType.Prev:
                {
                    if (asChild)
                        sdt.Insert(bookmarkNode, null, displacedBy == DisplacedByType.Prev);
                    else
                        sdt.ParentNode.Insert(bookmarkNode, sdt, displacedBy == DisplacedByType.Prev);
                    break;
                }
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        private readonly BookmarkStart mBookmarkStart;
        /// <summary>
        /// This is not really nice as bookmark end can be deleted from the document and this will not get updated.
        /// </summary>
        private BookmarkEnd mBookmarkEnd;

        internal const string ErrorBookmarkNotDefined = "Error! Bookmark not defined.";

        /// <summary>
        /// Maximum allowed bookmark name length.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstexpr]
        internal const int MaxNameLength = 40;

        internal const string GoBackBookmarkName = "_GoBack";
    }
}
