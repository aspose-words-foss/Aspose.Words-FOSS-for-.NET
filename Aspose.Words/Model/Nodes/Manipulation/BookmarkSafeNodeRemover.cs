// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/08/2021 by Ilya Navrotskiy

using Aspose.Collections.Generic;

namespace Aspose.Words
{
    /// <summary>
    /// Node remover with safe bookmark processing.
    /// </summary>
    /// <remarks>
    /// It does not remove incomplete bookmarks inside removing range. 'Incomplete' means bookmark has either only
    /// its Start or End (but not both of them) within the range.
    /// </remarks>
    internal class BookmarkSafeNodeRemover : NodeRemover
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="joinMode"></param>
        /// <param name="processBoundBlockAnnotationAsInline">If <c>true</c>, when the range has block level annotation
        /// nodes at bounds, removal gives same result as when the annotation nodes are moved to inline level.</param>
        internal BookmarkSafeNodeRemover(NodeRange range, NodeJoinMode joinMode, bool processBoundBlockAnnotationAsInline)
            : base(range, joinMode, processBoundBlockAnnotationAsInline)
        {
        }

        /// <summary>
        /// Adds node to the collection of nodes pending removal.
        /// </summary>
        /// <remarks>
        /// We mimic Word behavior and do not remove bookmarks that has either only Start or End within range being removed.
        /// </remarks>
        protected override void AddNodeToRemove(Node node)
        {
            // We need to process possible bookmarks inside composite node being removed.
            if (node.IsComposite)
            {
                // We will move bookmarks out of the composite node into its parent.
                if (node.ParentNode != null)
                {
                    NodeCollection bookmarks = ((CompositeNode)node).GetChildNodes(gBookmarkTypes, true);
                    for (int i = bookmarks.Count - 1; i >= 0; i--)
                    {
                        Node bookmark = bookmarks[i];
                        if (node.ParentNode.CanInsert(bookmark))
                            node.InsertNext(bookmark);
                    }
                }
            }
            else
            {
                // Remember incomplete bookmark names.
                IBookmarkNode bookmark = CurrentNode as IBookmarkNode;
                if (bookmark != null)
                {
                    if (mIncompleteBookmarkNames.Contains(bookmark.Name))
                        mIncompleteBookmarkNames.Remove(bookmark.Name);
                    else
                        mIncompleteBookmarkNames.Add(bookmark.Name);
                }
            }

            base.AddNodeToRemove(node);
        }

        /// <summary>
        /// Removes nodes pending removal.
        /// </summary>
        protected override void RemoveNodes()
        {
            foreach (Node node in NodesToRemove)
            {
                IBookmarkNode bookmark = node as IBookmarkNode;
                if ((bookmark != null) && mIncompleteBookmarkNames.Contains(bookmark.Name))
                {
                    // If we here, then we are about to remove bookmark that has only either Start, or End node.
                    // Word does not allow to remove such incomplete bookmark range. However, there can be situation
                    // when missing Start or End is neighboring to the removing Range. In this case Word 'extends'
                    // the range and successfully removes both: End and Start.
                    IBookmarkNode bookmarkToRemove = (node.NodeType == NodeType.BookmarkStart)
                        ? GetNeighboringBookmark(Range.End.Node, bookmark.Name, true)
                        : GetNeighboringBookmark(Range.Start.Node, bookmark.Name, false);

                    if (bookmarkToRemove == null)
                        continue;

                    ((Node)bookmarkToRemove).Remove();
                }

                node.Remove();
            }
        }

        /// <summary>
        /// Returns bookmark node with a specified name neighboring to a specified node,
        /// or <c>null</c>, if there is no such bookmark.
        /// </summary>
        /// <param name="node">The node that is neighboring to the bookmark being searched.</param>
        /// <param name="name">The name of searching bookmark.</param>
        /// <param name="isForward">If <c>true</c>, then iterates forward from a specified node.</param>
        private static IBookmarkNode GetNeighboringBookmark(Node node, string name, bool isForward)
        {
            IBookmarkNode bookmark = (isForward) ? node.NextSibling as IBookmarkNode : node.PreviousSibling as IBookmarkNode;
            while (bookmark != null)
            {
                if (bookmark.Name == name)
                    return bookmark;

                bookmark = (isForward)
                    ? ((Node)bookmark).NextSibling as IBookmarkNode
                    : ((Node)bookmark).PreviousSibling as IBookmarkNode;
            }

            return null;
        }

        /// <summary>
        /// The collection of bookmark names that were occured only once.
        /// </summary>
        private readonly HashSetGeneric<string> mIncompleteBookmarkNames = new HashSetGeneric<string>();

        private static readonly NodeType[] gBookmarkTypes = { NodeType.BookmarkStart, NodeType.BookmarkEnd };
    }
}
