// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/06/2016 by Dmitry Vorobyev

using System.Collections.Generic;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Implements deferred removing of bookmarks from TOC field results, when they are used by PAGEREF fields in another TOC field results.
    /// </summary>
    internal class TocEntriesBookmarkDeferredRemover
    {
        internal void RegisterBookmark(string bookmarkName)
        {
            mBookmarkNames.Add(bookmarkName);
        }

        internal void FreezeBookmarks(Field field)
        {
            mCurrentFieldBookmarks = null;
            if (field.Type != FieldType.FieldTOC)
                return;

            NodeRange fieldResultRange = new NodeRange(field.Separator, field.End);
            mCurrentFieldBookmarks = BookmarkFinder.Find(fieldResultRange, mBookmarkNames);
        }

        internal void RestoreFrozenBookmarks(Field field)
        {
            if (mCurrentFieldBookmarks == null || mCurrentFieldBookmarks.Length == 0)
                return;

            field.EnsureSeparator(false);

            Paragraph firstParagraph = (Paragraph)field.Separator.GetAncestor(NodeType.Paragraph);

            for (int i = mCurrentFieldBookmarks.Length - 1; i >= 0; i--)
            {
                BookmarkWithParagraphOffset bookmark = mCurrentFieldBookmarks[i];

                Paragraph paragraph = GetParagraph(firstParagraph, bookmark.ParagraphOffset);

                Node bookmarkNode = bookmark.Node;
                paragraph.InsertAfter(bookmarkNode, (paragraph == firstParagraph) ? field.Separator : null);

                mBookmarkNodes.Add(bookmarkNode);
            }
        }

        internal void RemoveFrozenBookmarks()
        {
            foreach (Node node in mBookmarkNodes)
            {
                if (node.ParentNode != null)
                    node.Remove();
            }
        }

        private static Paragraph GetParagraph(Paragraph paragraph, int offset)
        {
            Paragraph result = paragraph;

            for (int i = 0; i < offset; i++)
            {
                if (result.NextNonAnnotationSibling == null)
                    break;

                result = (Paragraph)result.NextNonAnnotationSibling;
            }

            return result;
        }

        private BookmarkWithParagraphOffset[] mCurrentFieldBookmarks;

        private readonly List<string> mBookmarkNames = new List<string>();
        private readonly List<Node> mBookmarkNodes = new List<Node>();

        /// <summary>
        /// Finds bookmarks with particular names in specified range.
        /// </summary>
        private class BookmarkFinder : NodeFinder
        {
            internal static BookmarkWithParagraphOffset[] Find(NodeRange range, IList<string> expectedBookmarkNames)
            {
                using (BookmarkFinder finder = new BookmarkFinder(range, expectedBookmarkNames))
                {
                    IList<Node> nodes = finder.Find();
                    Debug.Assert(nodes.Count == finder.mOffsets.Count);

                    BookmarkWithParagraphOffset[] result = new BookmarkWithParagraphOffset[nodes.Count];

                    for (int i = 0; i < result.Length; i++)
                    {
                        Node node = nodes[i];
                        int offset = finder.mOffsets[i];

                        result[i] = new BookmarkWithParagraphOffset(node, offset);
                    }

                    return result;
                }
            }

            private BookmarkFinder(NodeRange range, IList<string> expectedBookmarkNames)
                : base(range, NodeType.BookmarkStart, NodeType.BookmarkEnd)
            {
                mExpectedBookmarkNames = expectedBookmarkNames;
                mFirstParagraph = (Paragraph)range.Start.Node.GetAncestor(NodeType.Paragraph);
            }

            protected override bool OnNodeFinding()
            {
                IBookmarkNode bookmarkNode = (IBookmarkNode)CurrentNode;

                if (mExpectedBookmarkNames.IndexOf(bookmarkNode.Name) == -1)
                    return false;

                Paragraph bookmarkParagraph = (Paragraph)CurrentNode.GetAncestor(NodeType.Paragraph);
                Debug.Assert(mFirstParagraph.ParentNode == bookmarkParagraph.ParentNode);

                int offset = 0;
                Paragraph paragraph = bookmarkParagraph;
                while (paragraph != mFirstParagraph)
                {
                    paragraph = (Paragraph)paragraph.PreviousSiblingOfType(NodeType.Paragraph);
                    offset++;
                }

                mOffsets.Add(offset);

                return true;
            }

            private readonly IList<string> mExpectedBookmarkNames;
            private readonly Paragraph mFirstParagraph;
            private readonly List<int> mOffsets = new List<int>();
        }

        private class BookmarkWithParagraphOffset
        {
            public BookmarkWithParagraphOffset(Node node, int paragraphOffset)
            {
                Node = node;
                ParagraphOffset = paragraphOffset;
            }

            internal Node Node { get; }

            internal int ParagraphOffset { get; }
        }
    }
}
