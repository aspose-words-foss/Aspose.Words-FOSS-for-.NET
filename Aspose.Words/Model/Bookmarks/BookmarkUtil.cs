// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/12/2023 by Alexander Zhiltsov

using Aspose.Words.Tables;

namespace Aspose.Words
{
    /// <summary>
    /// Provides methods to work with bookmarks.
    /// </summary>
    internal static class BookmarkUtil
    {
        /// <summary>
        /// Gets the top left cell of area of the specified column bookmark.
        /// </summary>
        internal static Cell GetColumnBookmarkFirstCell(BookmarkStart start, BookmarkEnd end)
        {
            Table table = GetColumnBookmarkTable(start, end);
            if (table == null)
                return null;

            // A column bookmark start is usually located at the beginning of the first paragraph of the first cell of
            // a row. If there is a child table in the first cell, MS Word places a bookmark start to its first cell.
            // If there is a row-level SDT, a bookmark start is located before the top-most SDT (displaced by it).

            Row row = null;
            if (start.NodeLevel == NodeLevel.Row)
            {
                // The bookmark start is before row or row-level SDT.
                row = start.NextNonMarkupCompositeLimited as Row;
            }
            else
            {
                do
                {
                    Node node = (Node)row ?? (Node)start;
                    row = (Row)node.GetAncestor(NodeType.Row);
                }
                while ((row != null) && (row.ParentTable != table));
            }

            if (row == null)
                return null;

            int column = (start.LastColumn >= start.FirstColumn) ? start.FirstColumn : start.LastColumn;
            return (column < row.Cells.Count) ? row.Cells[column] : null;
        }

        /// <summary>
        /// Gets the bottom right cell of area of the specified column bookmark.
        /// </summary>
        internal static Cell GetColumnBookmarkLastCell(BookmarkStart start, BookmarkEnd end)
        {
            Table table = GetColumnBookmarkTable(start, end);
            if (table == null)
                return null;

            // A column bookmark end is usually located at the row level after the bookmark last row.
            // Old versions of AW placed such bookmark end to the next paragraph, i.e. to the first paragraph of the
            // first cell of the next row or to the first paragraph after the table. 

            Row row = null;
            if (!end.IsAncestorNode(table))
            {
                // The bookmark end is after the table, as the old versions of AW placed it.
                row = table.LastRow;
            }
            else
            {
                Node node = end;
                do
                {
                    Node prevNode = node.PreviousNonMarkupNodeLimited;
                    while ((prevNode != null) && NodeUtil.IsCrossStructureAnnotation(prevNode))
                        prevNode = prevNode.PreviousNonMarkupNodeLimited;

                    Row prevRow = prevNode as Row;
                    if ((prevRow != null) && (prevRow.ParentTable == table))
                    {
                        row = prevRow;
                        break;
                    }

                    node = node.ParentNode;
                }
                while ((node != null) && (node != table));
            }

            if (row == null)
                return null;

            int column = (start.LastColumn >= start.FirstColumn) ? start.LastColumn : start.FirstColumn;
            return (column < row.Cells.Count) ? row.Cells[column] : null;
        }

        /// <summary>
        /// Gets the table to which the specified column bookmark relates.
        /// </summary>
        private static Table GetColumnBookmarkTable(BookmarkStart start, BookmarkEnd end)
        {
            Node ancestor = Node.GetCommonAncestor(start, end);
            if (ancestor.NodeType == NodeType.Row)
                return ((Row)ancestor).ParentTable;

            if (ancestor.NodeType == NodeType.Table)
                return (Table)ancestor;

            // A bookmark end may be outside of a table. But a bookmark start is always located inside a bookmarked table.
            // Therefore, get the topmost ancestor table of the bookmark start, which is a descendant of the common
            // ancestor. Always take the nearest ancestor table as a fallback for unconventional bookmark nodes positions.

            Table bookmarkTable = (Table)start.GetAncestor(NodeType.Table);
            if (bookmarkTable == null)
                return null;

            while (true)
            {
                Table table = (Table)bookmarkTable.GetAncestor(NodeType.Table);
                if ((table != null) && table.IsAncestorNode(ancestor))
                    bookmarkTable = table;
                else
                    break;
            }
            
            return bookmarkTable;
        }
    }
}
