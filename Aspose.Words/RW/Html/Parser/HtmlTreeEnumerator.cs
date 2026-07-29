// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/05/2019 by Victor Chebotok

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// Enumerates nodes of a HTML sub-tree in the depth-first manner.
    /// </summary>
    /// <remarks>
    /// Each element node is visited twice: before and after enumerating its children. Each text node is visited only once.
    /// </remarks>
    internal class HtmlTreeEnumerator
    {
        /// <summary>
        /// Creates an instance of the enumerator.
        /// </summary>
        /// <param name="root">
        /// The root element of the sub-tree whose nodes will be enumerated.
        /// </param>
        internal HtmlTreeEnumerator(HtmlElementNode root)
        {
            Debug.Assert(root != null);

            mRoot = root;
            mIsStart = true;
        }

        /// <summary>
        /// Moves the enumerator to the next node of the sub-tree in the depth-first manner.
        /// </summary>
        /// <returns>
        /// <c>true</c> - the move is successful; there is another node to enumerate in the sub-tree.
        /// <c>false</c> - enumeration of the sub-tree has been completed; no more nodes to enumerate.
        /// </returns>
        internal bool MoveNext()
        {
            // No current node. Either the enumeration hasn't been started yet or it has already been completed.
            if (mCurrent == null)
            {
                if (mIsStart)
                {
                    // Not started. Start the enumeration.
                    mCurrent = mRoot;
                    mIsStart = true;
                    // Reset the flag if it was set before the enumeration started.
                    mSkipCurrentSubTree = false;
                    return true;
                }
                else
                {
                    // Completed.
                    return false;
                }
            }

            // If we're moving forward (deeper) and there can be children to visit.
            if (mIsStart && !mSkipCurrentSubTree && (mCurrent is HtmlElementNode))
            {
                HtmlNodeCollection children = ((HtmlElementNode)mCurrent).Children;
                if (children.Count > 0)
                {
                    // There are children. Let's move forward and visit the first child.
                    mCurrent = children[0];
                }
                else
                {
                    // The current element has no children. We start moving backwards and re-visit the element
                    // for the second time.
                    mIsStart = false;
                }
            }
            // We have completed (or skipped) enumeration of a sub-tree and are moving right and backward.
            else
            {
                // The last move that completes the enumeration. We have visited the whole sub-tree and returned to
                // the root node.
                if (mCurrent == mRoot)
                {
                    mCurrent = null;

                }
                // Not the last move. There are more nodes to visit in the sub-tree.
                else
                {
                    // Visit the next sibling if there is one.
                    if (mCurrent.NextSibling != null)
                    {
                        mCurrent = mCurrent.NextSibling;
                        mIsStart = true;
                    }
                    // If there are no more siblings, re-visit the parent element for the second time. We have enumerated all of
                    // its children.
                    else
                    {
                        mCurrent = mCurrent.Parent;
                        mIsStart = false;
                    }
                }
            }

            // The current node has changed, which means we have a new sub-tree to visit. We always visit sub-trees unless
            // we're explicitly told not to.
            mSkipCurrentSubTree = false;

            return mCurrent != null;
        }

        /// <summary>
        /// Instructs the enumerator to step over children of the current node on the next move and not to visit the current
        /// node for the second time. Doesn't actually move the enumerator.
        /// </summary>
        /// <remarks>
        /// Only has effect when the current node is an element and <see cref="IsStart"/> is "true". When there actually are
        /// children that hasn't been visited yet.
        /// </remarks>
        internal void DontEnumerateCurrentSubTree()
        {
            mSkipCurrentSubTree = true;
        }

        /// <summary>
        /// The current node being visited by the enumerator. <c>null</c> before enumeration starts and after it ends.
        /// </summary>
        internal HtmlNode Current
        {
            get { return mCurrent; }
        }

        /// <summary>
        /// Indicates whether it is the first time the current element node is being visited. Always <c>true</c> for text nodes.
        /// </summary>
        /// <remarks>
        /// Each element node is visited twice. On the second visit the value of this property is <c>false</c>.
        /// </remarks>
        internal bool IsStart
        {
            get { return mIsStart; }
        }

        /// <summary>
        /// The root element of the HTML sub-tree being enumerated.
        /// </summary>
        private readonly HtmlNode mRoot;

        /// <summary>
        /// The HTML node currently being visited by the enumerator.
        /// </summary>
        private HtmlNode mCurrent;

        /// <summary>
        /// Indicates whether the current node is being visited for the first time.
        /// <c>true</c> - first time, <c>false</c> - second time.
        /// </summary>
        /// <remarks>
        /// This flag is also used to decide whether the enumeration has ended. We have processed the whole sub-tree if we
        /// are visiting its root node for the second time.
        /// </remarks>
        private bool mIsStart;

        /// <summary>
        /// Indicates whether children of the current node must be stepped over on the next move.
        /// </summary>
        private bool mSkipCurrentSubTree;
    }
}
