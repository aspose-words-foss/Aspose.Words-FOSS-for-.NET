// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/11/2020 by Edward Voronov

using System.Collections.Generic;
using Aspose.Collections.Generic;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Stores and restores bookmark nodes in field result range during field update.
    /// </summary>
    internal class FieldResultBookmarsPreserver
    {
        internal FieldResultBookmarsPreserver(Field field)
        {
            mField = field;

            Store();
        }

        private void Store()
        {
            if (!mField.HasSeparator)
                return;

            NodeRange oldFieldResultRange = mField.GetFieldResultRange();
            using (NodeEnumerator enumerator = new NodeEnumerator(oldFieldResultRange))
            {
                while (enumerator.MoveToNextNode())
                {
                    MarkLeadingBookmarkNodes(enumerator.CurrentNode);
                    MarkMiddleBookmarkNodes(enumerator.CurrentNode);

                    StoreBookmarkNode(enumerator.CurrentNode);
                }

                MarkTrailingBookmarkNodes();
            }

            Sanitize();
        }

        internal void Restore()
        {
            HashSetGeneric<string> skipBookmarkNames = ExtractFieldResultBookmarkNames();

            for (int index = 0; index < mNodes.Count; index++)
            {
                FieldResultBookmarkNode node = mNodes[index];

                if (skipBookmarkNames.Contains(node.BookmarkName))
                    continue;

                if ((node.Position == Position.Middle) || (node.Position == Position.Trailing))
                    mField.End.InsertPrevious(node.Node);
            }

            for (int index = mNodes.Count - 1; index >= 0; index--)
            {
                FieldResultBookmarkNode node = mNodes[index];

                if (skipBookmarkNames.Contains(node.BookmarkName))
                    continue;

                if (node.Position == Position.Leading)
                    mField.Separator.InsertNext(node.Node);
            }
        }

        private void StoreBookmarkNode(Node node)
        {
            if (!NodeUtil.IsBookmarkNode(node))
                return;

            if (((IBookmarkNode)node).Name == Bookmark.GoBackBookmarkName)
                return;

            mNodes.Add(new FieldResultBookmarkNode(node));
        }

        private void MarkLeadingBookmarkNodes(Node node)
        {
            if (!mIsLeading)
                return;

            if (NodeUtil.IsCrossStructureAnnotation(node))
                return;

            mIsLeading = false;

            MarkUnknownAs(Position.Leading);
        }

        private void MarkMiddleBookmarkNodes(Node node)
        {
            if (mIsLeading)
                return;

            if (NodeUtil.IsCrossStructureAnnotation(node))
                return;

            MarkUnknownAs(Position.Middle);
        }

        private void MarkTrailingBookmarkNodes()
        {
            MarkUnknownAs(Position.Trailing);
        }

        private void MarkUnknownAs(Position position)
        {
            foreach (FieldResultBookmarkNode node in mNodes)
            {
                if (node.Position != Position.Unknown)
                    continue;

                node.Position = position;
            }
        }

        private void Sanitize()
        {
            List<FieldResultBookmarkNode> unpairedNodes = new List<FieldResultBookmarkNode>();
            for (int index = mNodes.Count - 1; index >= 0; index--)
            {
                FieldResultBookmarkNode fieldResultBookmarkNode = mNodes[index];
                if (fieldResultBookmarkNode.Position != Position.Middle)
                    continue;

                FieldResultBookmarkNode pair = FindByBookmarkName(fieldResultBookmarkNode.BookmarkName, unpairedNodes);
                if (pair == null)
                {
                    unpairedNodes.Add(fieldResultBookmarkNode);
                }
                else
                {
                    unpairedNodes.Remove(pair);
                    mNodes.Remove(pair);
                    mNodes.Remove(fieldResultBookmarkNode);
                }
            }
        }

        private HashSetGeneric<string> ExtractFieldResultBookmarkNames()
        {
            NodeRange newFieldResultRange = mField.GetFieldResultRange();
            IList<Node> bookmarkNodes = NodeFinder.FindNodes(newFieldResultRange, NodeType.BookmarkStart, NodeType.BookmarkEnd);

            HashSetGeneric<string> unpairedBookmarks = new HashSetGeneric<string>();
            HashSetGeneric<string> completeBookmarks = new HashSetGeneric<string>();

            foreach (Node node in bookmarkNodes)
            {
                string bookmarkName = ((IBookmarkNode)node).Name;
                string nameLowerInvariant = bookmarkName.ToLowerInvariant();
                if (unpairedBookmarks.Contains(nameLowerInvariant))
                {
                    unpairedBookmarks.Remove(nameLowerInvariant);
                    completeBookmarks.Add(bookmarkName);
                }
                else
                {
                    unpairedBookmarks.Add(nameLowerInvariant);
                }
            }

            return completeBookmarks;
        }

        private static FieldResultBookmarkNode FindByBookmarkName(string name, IEnumerable<FieldResultBookmarkNode> nodes)
        {
            foreach (FieldResultBookmarkNode node in nodes)
            {
                if (node.BookmarkName == name)
                    return node;
            }

            return null;
        }

        private readonly IList<FieldResultBookmarkNode> mNodes = new List<FieldResultBookmarkNode>();
        private bool mIsLeading = true;
        private readonly Field mField;

        private enum Position
        {
            Unknown,
            Leading,
            Middle,
            Trailing
        }

        private class FieldResultBookmarkNode
        {
            internal FieldResultBookmarkNode(Node node)
            {
                Node = node;
                Position = Position.Unknown;
                BookmarkName = ((IBookmarkNode)node).Name;
            }

            internal Node Node { get; }
            internal string BookmarkName { get; }
            internal Position Position { get; set; }
        }
    }
}
