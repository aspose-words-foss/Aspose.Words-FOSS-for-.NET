// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/14/2010 by Roman Korchagin

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Provides for-each enumeration over a <see cref="NodeCollection"/> and <see cref="CompositeNode"/>.
    /// </summary>
    [JavaManual("Java's Jaxen XPath third-party lib calls hasNext() (MoveNext() in .Net) multiple times.")]
    internal sealed class NodeCollectionEnumerator<T> : IEnumerator<T>
        where T : Node
    {
        internal NodeCollectionEnumerator(INodeCollection nodes)
        {
            mNodes = nodes;
            Reset();
        }

        public void Dispose()
        {
            // Nothing to dispose.
        }

        public bool MoveNext()
        {
            if (mCurNode == null)
                return false;

            // MoveNext used to throw if document structure is changed, but it's convenient in many cases
            // to ignore the changes if they don't break iterating. Further, we use a trick to make possible
            // to remove the current node during iterating through NodeCollection. If the current node has
            // been deleted, we use "the next after the previous visited" node to step forward.

            if ((mCurNode != mNodes.Container) && (mCurNode.ParentNode == null))
            {
                // mCurNode here may become a node outside the node collection.
                mCurNode = mPrevNode;
            }

            // Throw only if iterating cannot be continued.
            if ((mCurNode != mNodes.Container) && (mCurNode.ParentNode == null))
                throw new InvalidOperationException("Document structure was changed.");

            // mCurNode here always becomes a node of the node collection (or null after the end).
            mPrevNode = mNodes.GetNextMatchingNode(mCurNode);
            mCurNode = mNodes.GetCurrentNode();

            return (mCurNode != null);
        }

        public void Reset()
        {
            mCurNode = mNodes.Container;
        }

        public T Current
        {
            get
            {
                if ((mCurNode == mNodes.Container) || (mCurNode == null))
                    throw new InvalidOperationException("Invalid position of the enumerator.");
                return (T)mCurNode;
            }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        // Code moved here from NodeCollection for portability: 
        // MoveNext() is hasNext() in java and doesn't move cursor. So it is better to implement 
        // this functionality inside this manually ported class (the body is slightly changed in java).
        public static int GetCount(INodeCollection collection)
        {
            int count = 0;
            using (NodeCollectionEnumerator<Node> enumerator = new NodeCollectionEnumerator<Node>(collection))
            {
                while (enumerator.MoveNext())
                    count++;
            }

            return count;
        }

        /// <summary>
        /// A collection to iterate trough.
        /// </summary>
        private readonly INodeCollection mNodes;

        /// <summary>
        /// The iterating goes through all nodes of an underlying container of the collection and stops on matching nodes.
        /// This field is the previous visited node of the container. It may be outside of the collection.
        /// </summary>
        private Node mPrevNode;

        /// <summary>
        /// The current node of the collection.
        /// Has two special values: <c>mNodes.Container</c> means "before the first element" and
        /// <c>null</c> means "after the last element"
        /// </summary>
        private Node mCurNode;
    }
}
