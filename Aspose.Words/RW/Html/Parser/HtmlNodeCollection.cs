// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 13/04/2013 by Victor Chebotok

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.RW.Html.Parser
{
    /// <summary>
    /// Collection of HTML tree nodes, that represents children of a parent node.
    /// </summary>
    /// <remarks>
    /// Order of items is preserved. Adjacent text nodes are concatenated.
    /// </remarks>
    internal class HtmlNodeCollection : IEnumerable<HtmlNode>
    {
        /// <summary>
        /// Constructor. Initializes a new instance of the class.
        /// </summary>
        /// <param name="parent">The parent node. <c>null</c> if the node is a root and has no parent.</param>
        internal HtmlNodeCollection(HtmlElementNode parent)
        {
            mItems = new List<HtmlNode>();
            mParent = parent;
        }

        /// <summary>
        /// Adds the given node to the end of the collection.
        /// </summary>
        /// <param name="node">The node that will be added.</param>
        /// <remarks>
        /// If the last node of the collection and the given node are both text nodes, then the given node will not be added, 
        /// but the last node of the collection will be replaced by a new node containing the concatenated text
        /// of the last and the given node.
        /// </remarks>
        internal void Add(HtmlNode node)
        {
            Debug.Assert(node != null);
            Debug.Assert(node != mParent);

            // Concatenate text.
            if ((node is HtmlTextNode) && (mItems.Count > 0) && (mItems[mItems.Count - 1] is HtmlTextNode))
            {
                HtmlTextNode lastNode = (HtmlTextNode)mItems[mItems.Count - 1];
                string text = lastNode.Text + ((HtmlTextNode)node).Text;
                Remove(lastNode);
                Add(new HtmlTextNode(text));
            }
            // Append node.
            else
            {
                node.Parent = mParent;

                HtmlNode previousSibling;
                if (mItems.Count > 0)
                {
                    previousSibling = mItems[mItems.Count - 1];
                    previousSibling.NextSibling = node;
                }
                else
                {
                    previousSibling = null;
                }
                node.PreviousSibling = previousSibling;

                node.NextSibling = null;

                mItems.Add(node);
            }

            mVersion++;
        }

        /// <summary>
        /// Effectively moves nodes from the specified range to this collection.
        /// </summary>
        /// <param name="range">A collection nodes to be moved from.</param>
        internal void MoveRange(HtmlNodeCollection range)
        {
            int newCount = mItems.Count + range.mItems.Count;
            ListUtil.EnsureCapacity(mItems, newCount);

            foreach (HtmlNode node in range)
                Add(node);

            range.ClearCore();
        }

        /// <summary>
        /// Inserts the node to the specified position in the collection.
        /// </summary>
        /// <param name="node">The node that will be inserted.</param>
        /// <param name="index">The position to which the node will be inserted.</param>
        /// <remarks>
        /// After the insertion adjacent text nodes are replaced with one node containing concatenated text of these nodes.
        /// </remarks>
        internal void Insert(int index, HtmlNode node)
        {
            Debug.Assert(node != null);
            Debug.Assert(node != mParent);
            Debug.Assert(mItems.IndexOf(node) < 0);
            Debug.Assert(index >= 0);
            Debug.Assert(index < mItems.Count);

            // Concatenate with the previous text node.
            if ((node is HtmlTextNode) && (index > 0) && (mItems[index - 1] is HtmlTextNode))
            {
                HtmlTextNode precedingTextNode = (HtmlTextNode)mItems[index - 1];
                Remove(precedingTextNode);
                string text = precedingTextNode.Text + ((HtmlTextNode)node).Text;
                Insert(index - 1, new HtmlTextNode(text));
            }
            // Concatenate with the previous text node.
            else if ((node is HtmlTextNode) && (mItems[index] is HtmlTextNode))
            {
                HtmlTextNode successiveTextNode = (HtmlTextNode)mItems[index];
                Remove(successiveTextNode);
                string text = ((HtmlTextNode)node).Text + successiveTextNode.Text;
                Insert(index, new HtmlTextNode(text));
            }
            // No concatenation is required, just insert the node.
            else
            {
                node.Parent = mParent;

                HtmlNode previousSibling;
                if (index > 0)
                {
                    previousSibling = mItems[index - 1];
                    previousSibling.NextSibling = node;
                }
                else
                {
                    previousSibling = null;
                }
                node.PreviousSibling = previousSibling;

                node.NextSibling = mItems[index];
                node.NextSibling.PreviousSibling = node;

                mItems.Insert(index, node);
            }

            mVersion++;
        }

        /// <summary>
        /// Removes the node from the collection.
        /// </summary>
        /// <param name="node">The node that will be removed.</param>
        internal void Remove(HtmlNode node)
        {
            Debug.Assert(mItems.IndexOf(node) >= 0);

            node.Parent = null;

            if (node.PreviousSibling != null)
            {
                node.PreviousSibling.NextSibling = node.NextSibling;
            }

            if (node.NextSibling != null)
            {
                node.NextSibling.PreviousSibling = node.PreviousSibling;
            }

            node.PreviousSibling = null;
            node.NextSibling = null;

            mItems.Remove(node);
            mVersion++;
        }

        /// <summary>
        /// Removes all nodes from the collection.
        /// </summary>
        internal void Clear()
        {
            foreach (HtmlNode node in mItems)
            {
                node.Parent = null;
                node.PreviousSibling = null;
                node.NextSibling = null;
            }
            ClearCore();
        }

        private void ClearCore()
        {
            mItems.Clear();
            mVersion++;
        }

        /// <summary>
        /// Gets the index of the specified node in the collection.
        /// </summary>
        /// <param name="node">The node whose index will be returned.</param>
        /// <returns>
        /// The index of the specified node, or -1 if the node is not in the collection.
        /// </returns>
        internal int IndexOf(HtmlNode node)
        {
            return mItems.IndexOf(node);
        }

        public IEnumerator<HtmlNode> GetEnumerator()
        {
            return mItems.GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator that iterates over all instances of <see cref="HtmlNode"/> in the collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the number of elements actually contained in the collection.
        /// </summary>
        internal int Count
        {
            get { return mItems.Count; }
        }

        /// <summary>
        /// Collection version. The marker increases every time when the collection is modified.
        /// Used for performance improvement.
        /// </summary>
        internal long Version
        {
            get { return mVersion; }
        }

        /// <summary>
        /// Gets the node at the specified index.
        /// </summary>
        internal HtmlNode this[int index]
        {
            get { return mItems[index]; }
        }

        private readonly List<HtmlNode> mItems;

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly HtmlElementNode mParent;

        /// <summary>
        /// Collection version. The marker increases every time when the collection is modified.
        /// Used for performance improvement.
        /// </summary>
        /// <remarks>
        /// Version starts from 1 so the default zero value can be considered an 'unknown' version that differs from any
        /// known version of the collection.
        /// </remarks>
        private long mVersion = 1;
    }
}
