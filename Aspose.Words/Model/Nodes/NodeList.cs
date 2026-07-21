// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2005 by Roman Korchagin
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.XPath;
using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a collection of nodes matching an XPath query executed using the <see cref="CompositeNode.SelectNodes"/> method.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/aspose-words-document-object-model/">Aspose.Words Document Object Model (DOM)</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p><see cref="NodeList"/> is returned by <see cref="CompositeNode.SelectNodes"/> and contains a collection
    /// of nodes matching the XPath query.</p>
    /// <p><see cref="NodeList"/> supports indexed access and iteration.</p>
    /// <note>Treat the <see cref="NodeList"/> collection as a "snapshot" collection. <see cref="NodeList"/> starts
    /// as a "live" collection because the nodes are not actually retrieved when the XPath query is run.
    /// The nodes are only retrieved upon access and at this time the node and all nodes that precede
    /// it are cached forming a "snapshot" collection.</note>
    /// </remarks>
    [JavaManual("In Java this works with Jaxen Xpath engine.")]
    [CodePorting.Translator.Cs2Cpp.CppSkipEntity("XPath navigation is supported on C++ yet.")]
    public class NodeList : IEnumerable<Node>
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        internal NodeList(XPathNodeIterator iterator)
        {
            mIterator = iterator;
            mCachedNodes = new List<Node>();
            mIsAllCached = false;
        }

        /// <summary>
        /// Retrieves a node at the given index.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="NodeCollection.IndexerCommon"]/*'/>
        /// <param name="index">An index into the list of nodes.</param>
        public Node this[int index]
        {
            get
            {
                if (index < 0)
                {
                    //Access from the end, convert the index into positive index.
                    index = Count + index;

                    //If still out of range, return null as per interface requirements.
                    if (index < 0)
                        return null;
                }

                //Read more items into cache if needed.
                if (index >= mCachedNodes.Count)
                    CacheUntil(index);

                //If still out of range, return null as per interface requirements.
                if (index >= mCachedNodes.Count)
                    return null;

                return mCachedNodes[index];
            }
        }

        /// <summary>
        /// Gets the number of nodes in the list.
        /// </summary>
        public int Count
        {
            get
            {
                CacheUntil(Int32.MaxValue);
                return mCachedNodes.Count;
            }
        }

        /// <summary>
        /// Copies all nodes from the collection to a new array of nodes.
        /// </summary>
        /// <remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="NodeCollection.ToArrayCommon"]/*'/>
        /// </remarks>
        /// <returns>An array of nodes.</returns>
        public Node[] ToArray()
        {
            int count = this.Count;
            Node[] result = new Node[count];
            for (int i = 0; i < count; i++)
                result[i] = this[i];

            return result;
        }

        /// <summary>
        /// Provides a simple "foreach" style iteration over the collection of nodes.
        /// </summary>
        /// <returns>An <ms>IEnumerator</ms><java>Iterator</java><cpp>IEnumerator</cpp>.</returns>
        public IEnumerator<Node> GetEnumerator()
        {
            return new NodeListEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Reads nodes from XPathNodeIterator into our cache up to the specified index.
        /// </summary>
        internal void CacheUntil(int index)
        {
            if (mIsAllCached)
                return;

            while (index >= mCachedNodes.Count)
            {
                if (mIterator.MoveNext())
                {
                    DocumentXPathNavigator nav = (DocumentXPathNavigator)mIterator.Current;
                    Node node = nav.CurrentNode;
                    if (node != null)
                        mCachedNodes.Add(node);
                }
                else
                {
                    mIsAllCached = true;
                    break;
                }
            }
        }

        /// <summary>
        /// Provides for-each iteration over NodeList.
        /// </summary>
        private sealed class NodeListEnumerator : IEnumerator<Node>
        {
            internal NodeListEnumerator(NodeList nodes)
            {
                mNodes = nodes;
                Reset();
            }

            public void Dispose()
            {
                // Nothing to dispose.
            }

            public Node Current
            {
                get { return mCurNode; }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                mIndex++;

                //Read one past for delete-node case (this is not my code, later test this case).
                mNodes.CacheUntil(mIndex + 1);

                mCurNode = mNodes[mIndex];

                return (mCurNode != null);
            }

            public void Reset()
            {
                mIndex = -1;
                mCurNode = null;
            }

            private readonly NodeList mNodes;
            private int mIndex;
            private Node mCurNode;
        }

        private readonly XPathNodeIterator mIterator;
        private readonly List<Node> mCachedNodes;
        private bool mIsAllCached;
    }
}
