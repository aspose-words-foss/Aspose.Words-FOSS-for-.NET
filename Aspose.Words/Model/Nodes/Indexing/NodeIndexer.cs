// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/08/2014 by Alexey Morozov

using System.Collections.Generic;
using System.Text;
using Aspose.Collections.Generic;

namespace Aspose.Words
{
    /// <summary>
    /// Builds document or document part text and index each node within text to provide fast access.
    /// </summary>
    internal class NodeIndexer
    {
        internal NodeIndexer(Node rootNode)
        {
            RootNode = rootNode;
        }

        internal NodeIndexer(List<Node> nodeList)
        {
            mList = nodeList;
        }

        internal NodeIndexer(NodeRange nodeRange)
        {
            mNodeRange = nodeRange;
            RootNode = null;
        }

        internal NodeIndexer(NodeRange nodeRange, Node rootNode)
        {
            mNodeRange = nodeRange;
            RootNode = rootNode;
        }

        /// <summary>
        /// Returns node which corresponds to given position.
        /// </summary>
        internal Node GetNodeByPosition(int position)
        {
            return mPositions.GetByIndex(GetCachedIndexFromPosition(position));
        }

        /// <summary>
        /// Returns start position of node which corresponds to given position.
        /// </summary>
        internal int GetNodeStartByPosition(int position)
        {
            return mPositions.GetKey(GetCachedIndexFromPosition(position));
        }

        internal void Replace(int key, int newKey, Node value)
        {
            mPositions.Replace(key, newKey, value);
            InvalidateCache();
        }

        internal void Remove(int key)
        {
            mPositions.Remove(key);
            InvalidateCache();
        }

        internal void Add(int key, Node value)
        {
            mPositions.Add(key, value);
            InvalidateCache();
        }

        internal void Update()
        {
            mSb.Length = 0;
            mText = null;
            mPositions.Clear();
            InvalidateCache();

            if (mNodeRange != null)
                UpdateRangeIndexes();
            else if (mList != null)
            {
                int position = 0;
                foreach (Node node in mList)
                {
                    RootNode = node;
                    int len = BuildIndex(node, position);
                    position += len;
                }
            }
            else
            {
                BuildIndex(RootNode, 0);
            }
        }

        /// <summary>
        /// Inserts new node into index and do fast index update.
        /// </summary>
        /// <remarks>
        /// Inserts new key/value pair with low-level keys shift.
        /// </remarks>
        internal void InsertAndUpdate(int key, Node node, string text)
        {
            mPositions.Shift(key, text.Length);
            mPositions.Add(key, node);

            mText.Insert(key, text);

            InvalidateCache();
        }

        [JavaAttributes.JavaThrows(true)]
        internal virtual IndexerAction OnNode(Node node)
        {
            return IndexerAction.None;
        }

        protected virtual string GetText(Node node)
        {
            return node.GetText();
        }

        protected virtual string GetEndText(CompositeNode node)
        {
            return node.GetEndText();
        }

        /// <summary>
        /// Adds index for a specified node.
        /// </summary>
        protected virtual void Add(int position, Node node, string text)
        {
            mSb.Append(text);
            mPositions.Add(position, node);
        }

        /// <summary>
        /// Updates indexes for range nodes.
        /// </summary>
        private void UpdateRangeIndexes()
        {
            DocumentPosition curPosition = RootNode == null
                ? mNodeRange.Start.Clone()
                : new DocumentPosition(RootNode);

            if (!mNodeRange.IsStartIncluded && (RootNode == null))
                curPosition.MoveNext(true);

            int textPosition = 0;
            Node ignoreParent = null;

            while ((curPosition.Node != mNodeRange.End.Node) || (mNodeRange.IsEndIncluded && !curPosition.IsEnd))
            {
                curPosition.Move(null, true, true, true, false, true);

                Node node = curPosition.Node;

                if (RootNode != null && !node.IsAncestorNode(RootNode))
                    break;

                if (ignoreParent != null)
                {
                    if ((node == ignoreParent) || node.IsAncestorNode(ignoreParent))
                        continue;

                    ignoreParent = null;
                }

                if (!curPosition.IsEnd)
                {
                    IndexerAction action = OnNode(node);
                    switch (action)
                    {
                        case IndexerAction.Collapse:
                            ignoreParent = node;
                            break;
                        case IndexerAction.Ignore:
                            ignoreParent = node;
                            continue;
                        case IndexerAction.Skip:
                            continue;
                    }

                    continue;
                }

                string text = node.IsComposite
                    ? GetEndText((CompositeNode)node)
                    : GetText(node);

                int textLength = text.Length;
                if (textLength > 0)
                {
                    Add(textPosition, node, text);
                    textPosition += textLength;
                }
            }
        }

        /// <summary>
        /// Cached version of <see cref="GetIndexFromPosition"/> method.
        /// </summary>
        /// <remarks>
        /// Simple trick to avoid repeated search of index when consecutive calls with the same position are made.
        /// Main purpose is to make caller code simpler.
        /// </remarks>
        private int GetCachedIndexFromPosition(int position)
        {
            if (position != mCachedPosition)
            {
                mCachedPosition = position;
                mCachedIndex = GetIndexFromPosition(position);
            }

            return mCachedIndex;
        }

        /// <summary>
        /// Performs binary search for given position.
        /// </summary>
        private int GetIndexFromPosition(int position)
        {
            //This algorithm is adopted from the .NET BinarySearch function.
            int lo = 0;
            int hi = mPositions.Count;

            while (lo <= hi)
            {
                int i = (lo + hi) >> 1;

                int start = mPositions.GetKey(i);
                int end = ((i + 1) < mPositions.Count) ? mPositions.GetKey(i + 1) : int.MaxValue;

                if (position < start)
                {
                    // Position is before this node, make next iteration to look closer to the beginning.
                    hi = i - 1;
                }
                else if (position >= end)
                {
                    // Position is after this node, make next iteration to look close to the end.
                    lo = i + 1;
                }
                else
                {
                    // Return position.
                    return i;
                }
            }

            return 0;
        }

        private int BuildIndex(Node node, int position)
        {
            IndexerAction action = OnNode(node);

            // Ignore node completely.
            if (action == IndexerAction.Ignore)
                return 0;

            // Whole node is collapsed.
            if (action == IndexerAction.Collapse)
            {
                string nodeText = GetText(node);
                Add(position, node, nodeText);
                return nodeText.Length;
            }

            if (node.IsComposite)
            {
                int compositeLength = 0;

                CompositeNode compositeNode = (CompositeNode)node;

                foreach (Node childNode in compositeNode.GetChildNodes(NodeType.Any, false))
                {
                    int length = BuildIndex(childNode, position);

                    position += length;
                    compositeLength += length;
                }

                action = OnNode(compositeNode);
                if (action != IndexerAction.Skip)
                {
                    string endText = GetEndText(compositeNode);
                    if (endText.Length > 0)
                    {
                        Add(position, compositeNode, endText);
                        compositeLength += endText.Length;
                    }
                }

                return compositeLength;
            }
            else
            {
                if (action != IndexerAction.Skip)
                {
                    string text = GetText(node);

                    if (text.Length > 0)
                        Add(position, node, text);

                    return text.Length;
                }

                return 0;
            }
        }

        private void InvalidateCache()
        {
            mCachedIndex = -1;
            mCachedPosition = -1;
        }

        internal SortedIntegerListGeneric<Node> Positions
        {
            get { return mPositions; }
        }

        /// <summary>
        /// Text being indexed by the indexer.
        /// </summary>
        internal ChunkedString Text
        {
            get
            {
                if (mText == null)
                    mText = new ChunkedString(mSb.ToString());

                return mText;
            }
        }

        internal Node RootNode { get; private set; }

        private int mCachedPosition = -1;
        private int mCachedIndex = -1;

        private readonly NodeIndex mPositions = new NodeIndex();
        private readonly StringBuilder mSb = new StringBuilder();
        private ChunkedString mText;
        private readonly List<Node> mList;
        private readonly NodeRange mNodeRange;
    }
}
