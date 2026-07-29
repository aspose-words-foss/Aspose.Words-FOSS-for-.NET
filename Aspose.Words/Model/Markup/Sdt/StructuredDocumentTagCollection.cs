// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 23/03/2022 by Vadim Saltykov

using System;
using System.Collections;
using System.Collections.Generic;

namespace Aspose.Words.Markup
{
    /// <summary>
    /// A collection of <see cref="IStructuredDocumentTag"/> instances that represent the structured document tags in the specified range.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-content-control-sdt/">Structured Document Tags or Content Control</a> documentation article.</para>
    /// </summary>
    public class StructuredDocumentTagCollection : IEnumerable<IStructuredDocumentTag>
    {
        internal StructuredDocumentTagCollection(Node parent)
        {
            if (parent.IsComposite)
                mStructuredDocumentTags = ((CompositeNode)parent).GetChildNodes(new NodeType[]
                    { NodeType.StructuredDocumentTag, NodeType.StructuredDocumentTagRangeStart }, true);
            else
                mStructuredDocumentTags = EmptyNodeCollection.CreateEmpty();
        }

        internal StructuredDocumentTagCollection(NodeRange nodeRange)
        {
            mStructuredDocumentTags = new NodeCollection(nodeRange.Document, new StructuredDocumentTagMatcher(nodeRange), true);
        }

        /// <summary>
        /// Returns the structured document tag by identifier.
        /// </summary>
        /// <remarks>
        /// <p>Returns null if the structured document tag with the specified identifier cannot be found.</p>
        /// </remarks>
        /// <param name="id">The structured document tag identifier.</param>
        public IStructuredDocumentTag GetById(int id)
        {
            foreach (Node node in mStructuredDocumentTags)
            {
                IStructuredDocumentTag sdt = node as IStructuredDocumentTag;
                if ((sdt != null) && (sdt.Id == id))
                    return sdt;
            }
            return null;
        }

        /// <summary>
        /// Returns the first structured document tag encountered in the collection with the specified title.
        /// </summary>
        /// <remarks>
        /// <p>Returns null if the structured document tag with the specified title cannot be found.</p>
        /// </remarks>
        /// <param name="title">The title of structured document tag.</param>
        public IStructuredDocumentTag GetByTitle(string title)
        {
            ArgumentUtil.CheckNotNull(title, "title");

            foreach (Node node in mStructuredDocumentTags)
            {
                IStructuredDocumentTag sdt = node as IStructuredDocumentTag;
                if ((sdt != null) && (sdt.Title.Equals(title, StringComparison.Ordinal)))
                    return sdt;
            }
            return null;
        }

        /// <summary>
        /// Returns the first structured document tag encountered in the collection with the specified tag.
        /// </summary>
        /// <remarks>
        /// <p>Returns null if the structured document tag with the specified tag cannot be found.</p>
        /// </remarks>
        /// <param name="tag">The tag of the structured document tag.</param>
        public IStructuredDocumentTag GetByTag(string tag)
        {
            ArgumentUtil.CheckNotNull(tag, "tag");

            foreach (Node node in mStructuredDocumentTags)
            {
                IStructuredDocumentTag sdt = node as IStructuredDocumentTag;
                if ((sdt != null) && (sdt.Tag.Equals(tag, StringComparison.Ordinal)))
                    return sdt;
            }
            return null;
        }


        /// <summary>
        /// Removes the structured document tag with the specified identifier.
        /// </summary>
        /// <param name="id">The structured document tag identifier.</param>
        public void Remove(int id)
        {
            IStructuredDocumentTag sdt = this.GetById(id);
            if (sdt == null)
                throw new ArgumentException("The specified Id is not found.");

            sdt.Node.Remove();
        }

        /// <summary>
        /// Removes a structured document tag at the specified index.
        /// </summary>
        /// <param name="index">An index into the collection.</param>
        public void RemoveAt(int index)
        {
            mStructuredDocumentTags.RemoveAt(index);
        }

        /// <summary>
        /// Returns index of structured document tag with the specified identifier.
        /// </summary>
        /// <param name="id">The structured document tag identifier.</param>
        /// <returns>Structured document tag index in this collection. Returns -1 if the structured document tag with the specified Id does not exist in the collection.</returns>
        internal int IndexOf(int id)
        {
            for (int i = 0; i < mStructuredDocumentTags.Count; i++)
            {
                if (((IStructuredDocumentTag)mStructuredDocumentTags[i]).Id == id)
                    return i;
            }

            return -1;
        }

        /// <summary>
        /// Returns the number of structured document tags in the collection.
        /// </summary>
        public int Count
        {
            get { return mStructuredDocumentTags.Count; }
        }

        /// <summary>
        /// Returns the structured document tag at the specified index.
        /// </summary>
        /// <param name="index">An index into the collection.</param>
        public IStructuredDocumentTag this[int index]
        {
            get
            {
                return (IStructuredDocumentTag)mStructuredDocumentTags[index];
            }
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        public IEnumerator<IStructuredDocumentTag> GetEnumerator()
        {
            return new StructuredDocumentTagIterator(mStructuredDocumentTags);
        }

        /// <summary>
        /// Returns an enumerator object.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new StructuredDocumentTagIterator(mStructuredDocumentTags);
        }

        /// <summary>
        /// Enumerator through items of <see cref="StructuredDocumentTagCollection"/>.
        /// </summary>
        private sealed class StructuredDocumentTagIterator : IEnumerator<IStructuredDocumentTag>
        {
            /// <summary>
            /// Ctor.
            /// </summary>
            internal StructuredDocumentTagIterator(NodeCollection structuredDocumentTags)
            {
                mSdtCollEnumerator = structuredDocumentTags.GetEnumerator();
            }

            public void Dispose()
            {
                // Nothing to dispose.
            }

            /// <summary>
            /// Gets the element in the collection at the current position of the enumerator.
            /// </summary>
            public IStructuredDocumentTag Current
            {
                get
                {
                    return (IStructuredDocumentTag)mSdtCollEnumerator.Current;
                }
            }

            /// <summary>
            /// Advances the enumerator to the next element of the collection.
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                return mSdtCollEnumerator.MoveNext();
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the collection.
            /// </summary>
            public void Reset()
            {
                ((NodeCollectionEnumerator<Node>)mSdtCollEnumerator).Reset();
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            private readonly IEnumerator<Node> mSdtCollEnumerator;
        }

        /// <summary>
        /// This is a live collection of structured document tag nodes.
        /// </summary>
        private readonly NodeCollection mStructuredDocumentTags;

        private class StructuredDocumentTagMatcher : NodeMatcher
        {

            internal StructuredDocumentTagMatcher(NodeRange range)
            {
                mNodeRange = range;
                UpdateStructuredDocumentTagCache();
            }

            /// <summary>
            /// Returns true if the specified node matches the appropriate structured document tag node.
            /// </summary>
            /// <remarks>
            /// Supports both StructuredDocumentTag and the ranged SDT. Both types of nodes are treated as the ranges
            /// with the start and the end, see the comment BookmarkMatcher.IsMatch() for more details.
            /// </remarks>
            internal override bool IsMatch(Node node)
            {
                if (mInitialChangeCount != Document.TreeChangeCount)
                    UpdateStructuredDocumentTagCache();

                if (((node.NodeType != NodeType.StructuredDocumentTag) && (node.NodeType != NodeType.StructuredDocumentTagRangeStart)) ||
                    mRangeIsEmpty)
                    return false;

                StructuredDocumentTagRangeStart sdtStart = node as StructuredDocumentTagRangeStart;
                if (sdtStart != null)
                {
                    StructuredDocumentTagRangeEnd sdtEnd = sdtStart.RangeEnd;
                    return mRangesStarts.Contains(sdtStart) && mRangesEnds.Contains(sdtEnd);
                }

                return mSdtNodes.Contains(node);
            }

            internal override bool IsSkipMarkupNodes
            {
                get { return true; }
            }

            private void UpdateStructuredDocumentTagCache()
            {
                mInitialChangeCount = Document.TreeChangeCount;

                mRangeIsEmpty = mNodeRange.IsEmpty || mNodeRange.IsVoid ||
                                mNodeRange.Start.Node.IsRemoved ||
                                mNodeRange.End.Node.IsRemoved ||
                                (NodeFinder.FindNodes(mNodeRange, NodeType.Any).Count == 0);

                if (mRangeIsEmpty)
                    return;

                mSdtNodes = NodeFinder.FindNodes(mNodeRange, NodeType.StructuredDocumentTag);

                // We add here StructuredDocumentTag which is the parent for the part of the range.
                Node parent = mNodeRange.FirstRangeNode.ParentNode;
                do
                {
                    if (parent.NodeType == NodeType.StructuredDocumentTag)
                        mSdtNodes.Add(parent);
                    parent = parent.ParentNode;
                }
                while ((parent != null) && (parent.NodeLevel != NodeLevel.SectionStory));

                NodeRange highRange = new NodeRange(Document.FirstSection.Body.FirstChild,
                    true, mNodeRange.End.Node, mNodeRange.IsEndIncluded);
                mRangesStarts = NodeFinder.FindNodes(highRange, NodeType.StructuredDocumentTagRangeStart);

                NodeRange tailRange = new NodeRange(mNodeRange.Start.Node, mNodeRange.IsStartIncluded,
                    Document.LastChild, true);
                mRangesEnds = NodeFinder.FindNodes(tailRange, NodeType.StructuredDocumentTagRangeEnd);
            }

            private Document Document
            {
                get { return mNodeRange.Document.FetchDocument(); }
            }

            private bool mRangeIsEmpty;
            private IList<Node> mSdtNodes;
            private IList<Node> mRangesStarts;
            private IList<Node> mRangesEnds;
            private int mInitialChangeCount;
            private readonly NodeRange mNodeRange;
        }
    }
}
