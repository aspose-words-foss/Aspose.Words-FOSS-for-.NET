// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2005 by Roman Korchagin

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a collection of nodes of a specific type.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/aspose-words-document-object-model/">Aspose.Words Document Object Model (DOM)</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p><see cref="NodeCollection"/> does not own the nodes it contains, rather, is just a selection of nodes
    /// of the specified type, but the nodes are stored in the tree under their respective parent nodes.</p>
    ///
    /// <p><see cref="NodeCollection"/> supports indexed access, iteration and provides add and remove methods.</p>
    ///
    /// <p>The <see cref="NodeCollection"/> collection is "live", i.e. changes to the children of the node object
    /// that it was created from are immediately reflected in the nodes returned by the <see cref="NodeCollection"/>
    /// properties and methods.</p>
    ///
    /// <p><see cref="NodeCollection"/> is returned by <see cref="CompositeNode.GetChildNodes(NodeType, bool)"/>
    /// and also serves as a base class for typed node collections such as <see cref="SectionCollection"/>,
    /// <see cref="ParagraphCollection"/> etc.</p>
    ///
    /// <p><see cref="NodeCollection"/> can be "flat" and contain only immediate children of the node it was created
    /// from, or it can be "deep" and contain all descendant children.</p>
    /// </remarks>
    [JavaGenericParameter("T extends Node"), JavaGenericArguments("Iterable<T>")]
    public class NodeCollection : INodeCollection, IEnumerable<Node>
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="container">The topmost node covered by the collection. Not included in the collection itself.</param>
        /// <param name="nodeType">Type of nodes that this collection selects.</param>
        /// <param name="isDeep">Specifies whether the collection selects all nested children or only immediate children of the container.</param>
        internal NodeCollection(CompositeNode container, NodeType nodeType, bool isDeep) :
            this(container, NodeTypeMatcher.GetMatcher(nodeType), isDeep)
        {
        }

        internal NodeCollection(CompositeNode container, NodeType[] nodeTypes, bool isDeep) :
            this(container, new NodeTypeMatcher(nodeTypes), isDeep)
        {
        }

        internal NodeCollection(CompositeNode container, NodeMatcher matcher, bool isDeep)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            if (matcher == null)
                throw new ArgumentNullException("matcher");

            mContainer = container;

            // SPEED Get the document only once.
            mDocument = container.Document;

            mMatcher = matcher;
            mIsDeep = isDeep;

            Invalidate();
        }

        /// <summary>
        /// Retrieves a node at the given index.
        /// </summary>
        /// <param name="index">An index into the collection of nodes.</param>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="NodeCollection.IndexerCommon"]/*'/>
        [JavaConvertCheckedExceptions]
        public Node this[int index]
        {
            get
            {
                CheckInvalidate();

                if (index < 0)
                {
                    //Access from the end, convert the index into positive index.
                    index = Count + index;

                    //If it is still negative, this means out of range.
                    if (index < 0)
                        return null;
                }

                //Optimization - direct hit - access of the most recently accessed element.
                if (mCurIndex == index)
                    return mCurNode;

                //Optimization - no direct hit, but most of the time we expect the user will
                //access nodes in close proximity, therefore navigate only the difference
                //between last and new indexes.
                //
                //Note the way it works for index zero: mCurIndex is reset to -1 when optimization is reset,
                //therefore 0 - (-1) results in 1, which is the correct index to retrieve the first child.
                //
                //This optimization is not my idea - I copied it from XmlElement.
                int diff = index - mCurIndex;
                Node node = GetNthMatchingNode(mCurNode, diff);

                //Save results for next optimization.
                if (node != null)
                {
                    mCurIndex = index;
                    mCurNode = node;
                }

                return node;
            }
        }

        /// <summary>
        /// Adds a node to the end of the collection.
        /// </summary>
        /// <remarks>
        /// <p>The node is inserted as a child into the node object from which the collection was created.</p>
        /// </remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="NodeCollection.InsertCommon"]/*'/>
        /// <param name="node">The node to be added to the end of the collection.</param>
        /// <exception cref="NotSupportedException">The <see cref="NodeCollection"/> is a "deep" collection.</exception>
        public void Add(Node node)
        {
            if (mIsDeep)
                throw new NotSupportedException("Adding nodes is not yet supported for deep node collections.");

            mContainer.AppendChild(node);
        }

        /// <summary>
        /// Inserts a node into the collection at the specified index.
        /// </summary>
        /// <remarks>
        /// <p>The node is inserted as a child into the node object from which the collection was created.</p>
        /// <p>If the index is equal to or greater than <see cref="Count"/>, the node is added at the end of the collection.</p>
        /// <p>If the index is negative and its absolute value is greater than <see cref="Count"/>, the node is added at the end of the collection.</p>
        /// </remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="NodeCollection.InsertCommon"]/*'/>
        /// <param name="index">The zero-based index of the node.
        /// Negative indexes are allowed and indicate access from the back of the list.
        /// For example -1 means the last node, -2 means the second before last and so on.</param>
        /// <param name="node">The node to insert.</param>
        /// <exception cref="NotSupportedException">The <see cref="NodeCollection"/> is a "deep" collection.</exception>
        public void Insert(int index, Node node)
        {
            if (mIsDeep)
                throw new NotSupportedException("Inserting nodes is not yet supported for deep node collections.");

            mContainer.InsertBefore(node, this[index]);
        }

        /// <summary>
        /// Removes the node from the collection and from the document.
        /// </summary>
        /// <param name="node">The node to remove.</param>
        public void Remove(Node node)
        {
            node.Remove();
        }

        /// <summary>
        /// Removes the node at the specified index from the collection and from the document.
        /// </summary>
        /// <param name="index">The zero-based index of the node.
        /// Negative indexes are allowed and indicate access from the back of the list.
        /// For example -1 means the last node, -2 means the second before last and so on.</param>
        public void RemoveAt(int index)
        {
            this[index].Remove();
        }

        /// <summary>
        /// Removes all nodes from this collection and from the document.
        /// </summary>
        public void Clear()
        {
            // When a node is removed, NodeCollectionEnumerator remembers the previous node visited in the collection's
            // container and so continues iterating from that node, not from the beginning of the container.
            foreach (Node node in this)
                node.Remove();
        }

        /// <summary>
        /// Determines whether a node is in the collection.
        /// </summary>
        /// <remarks>
        /// <p>This method performs a linear search; therefore, the average execution time is proportional to <see cref="Count"/>.</p>
        /// </remarks>
        /// <param name="node">The node to locate.</param>
        /// <returns><c>true</c> if item is found in the collection; otherwise, <c>false</c>.</returns>
        public bool Contains(Node node)
        {
            return (IndexOf(node) != -1);
        }

        /// <summary>
        /// Returns the zero-based index of the specified node.
        /// </summary>
        /// <param name="node">The node to locate.</param>
        /// <returns>The zero-based index of the node within the collection, if found; otherwise, -1.</returns>
        /// <remarks>
        /// <p>This method performs a linear search; therefore, the average execution time is proportional to <see cref="Count"/>.</p>
        /// </remarks>
        public int IndexOf(Node node)
        {
            int i = 0;
            foreach (Node curNode in this)
            {
                if (node == curNode)
                    return i;
                i++;
            }
            return -1;
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
            return ToList<Node>().ToArray();
        }

        /// <summary>
        /// Provides a simple "foreach" style iteration over the collection of nodes.
        /// </summary>
        /// <returns>An <ms>IEnumerator</ms><java>Iterator</java><cpp>IEnumerator</cpp>.</returns>
        [JavaGenericArguments("Iterator<T>", "NodeCollectionEnumerator<T>")]
        public IEnumerator<Node> GetEnumerator()
        {
            return new NodeCollectionEnumerator<Node>(this);
        }

        internal NodeCollectionEnumerator<TNode> GetNodeEnumerator<TNode>()
            where TNode : Node
        {
            return new NodeCollectionEnumerator<TNode>(this);
        }

        /// <summary>
        /// Provides a simple "foreach" style iteration over the collection of nodes.
        /// </summary>
        /// <returns><ms>An IEnumerator</ms><java><tt>Iterator&lt;Node&gt;</tt></java><cpp>An IEnumerator</cpp>.</returns>
        [JavaGenericArguments("Iterator<T>", "NodeCollectionEnumerator<T>")]
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Copies all nodes from the collection to a new List{Node} of nodes.
        /// Used as core of <see cref="ToArray"/> method.
        /// </summary>
        internal List<Node> ToNodeList()
        {
            return ToList<Node>();
        }

        /// <summary>
        /// Copies all nodes from the collection to a new List{T} of nodes.
        /// </summary>
        internal List<TValue> ToList<TValue>()
            where TValue : Node
        {
            List<TValue> result = new List<TValue>();
            foreach (Node node in this)
                result.Add((TValue)node);
            return result;
        }

        /// <summary>
        /// Gets the next matching node count times. If count is positive traverses forward.
        /// If negative traverses backward.
        /// </summary>
        /// <remarks>
        /// Exposed for unit-testing.
        /// </remarks>
        internal Node GetNthMatchingNode(Node startNode, int count)
        {
            //Decide whether we traverse forward of backward
            bool isForward = (count > 0);

            //Can normalize count to positive now.
            if (count < 0)
                count = -count;

            Node node = startNode;
            for (int i = 0; i < count; i++)
            {
                node = FindMatchingNode(node, isForward);
                if (node == null)
                    return null;
            }
            return node;
        }

        /// <summary>
        /// <para>Gets the next matching node in the collection forward from the specified node,
        /// see <see cref="GetMatchingNode(Node, bool)"/>.</para>
        /// </summary>
        Node INodeCollection.GetNextMatchingNode(Node curNode)
        {
            return GetMatchingNode(curNode, true);
        }

        /// <summary>
        /// Gets the next matching node in the collection forward or backward from the specified node,
        /// stores it in <see cref="mCurrent"/> - can be get by <see cref="INodeCollection.GetCurrentNode"/> . 
        /// Returns a node in the collection's container
        /// (maybe outside of the collection), visited before the found matching one.
        /// </summary>
        /// <param name="curNode">A starting node in the container (maybe outside of the collection).
        /// <c>node == this.Container</c> means search from the beginning of the collection.</param>
        /// <param name="isForward">Whether step is forward.</param>
        private Node GetMatchingNode(Node curNode, bool isForward)
        {
            Debug.Assert(curNode != null);
            mCurrent = curNode;
            Node prevVisited;

            do
            {
                prevVisited = mCurrent;
                // WORDSNET-27401 We shall not skip the markup nodes in case of the subobjects of the parent markup node.
                bool isSkipMarkupNodes = NodeUtil.IsSkipMarkupNodesInFlatEnumeration(mContainer.NodeType) &&
                                         mMatcher.IsSkipMarkupNodes;

                mCurrent =
                    (mIsDeep) ? TraverseDeep(isForward, mCurrent) :
                    (isSkipMarkupNodes) ? TraverseFlatSkipMarkupNodes(isForward, mCurrent) :
                    TraverseFlat(isForward, mCurrent);
            }
            while ((mCurrent != null) && !mMatcher.IsMatch(mCurrent));

            return prevVisited;
        }

        /// <summary>
        /// This is a copycat of the <see cref="GetMatchingNode(Node, bool)"/> method but without ref keyword.
        /// It is created to prevent creating of too many Ref objects on Java.
        /// See WORDSJAVA-2279.
        /// </summary>
        private Node FindMatchingNode(Node curNode, bool isForward)
        {
            Debug.Assert(curNode != null);

            do
            {
                curNode =
                    (mIsDeep) ? TraverseDeep(isForward, curNode) :
                    (mMatcher.IsSkipMarkupNodes) ? TraverseFlatSkipMarkupNodes(isForward, curNode) :
                    TraverseFlat(isForward, curNode);
            }
            while ((curNode != null) && !mMatcher.IsMatch(curNode));

            return curNode;
        }

        /// <summary>
        /// Deep (preorder) tree traversal.
        /// </summary>
        private Node TraverseDeep(bool isForward, Node node)
        {
            return (isForward) ? node.NextPreOrder(mContainer) : node.PreviousPreOrder(mContainer);
        }

        /// <summary>
        /// Simple flat sibling traversal.
        /// </summary>
        private Node TraverseFlat(bool isForward, Node node)
        {
            if (isForward)
                return (node == mContainer) ? mContainer.FirstChild : node.NextSibling;
            else
                return (node == mContainer) ? mContainer.LastChild : node.PreviousSibling;
        }

        /// <summary>
        /// Flat sibling traversal.
        /// Takes markup nodes into the account by reading their content and skipping them. 
        /// </summary>
        private Node TraverseFlatSkipMarkupNodes(bool isForward, Node node)
        {
            Node resultNode;

            if (isForward)
            {
                resultNode = (node == mContainer) ? mContainer.FirstNonMarkupDescendant : node.NextNonMarkupNodeLimited;
            }
            else
            {
                resultNode = (node == mContainer) ? mContainer.LastNonMarkupDescendant : node.PreviousNonMarkupNodeLimited;
            }

            // WORDSNET-15251 It's a sibling traversal, so we should stay on a descendant's line.
            if ((resultNode == mContainer.NextSibling) || (resultNode == mContainer.PreviousSibling))
                resultNode = null;

            return resultNode;
        }

        /// <summary>
        /// Invalidates a collection if the document has changed since last time the collection was accessed.
        /// </summary>
        private void CheckInvalidate()
        {
            if (mInitialChangeCount != DocumentTreeChangeCount)
                Invalidate();
        }

        /// <summary>
        /// Invalidates a collection so getting item by index next time will start without cache.
        /// </summary>
        private void Invalidate()
        {
            mInitialChangeCount = DocumentTreeChangeCount;
            mCurIndex = -1;
            mCurNode = mContainer;
            mCount = -1;
        }

        private int DocumentTreeChangeCount
        {
            get
            {
                // mDocument is null in case when mContainer is EmptyCompositeNode so let's handle this properly.
                return (mDocument != null) ? mDocument.TreeChangeCount : 0;
            }
        }

        /// <summary>
        /// Gets the number of nodes in the collection.
        /// </summary>
        [JavaThrows(false)]
        public int Count
        {
            get
            {
                CheckInvalidate();

                if (mCount == -1)
                    mCount = NodeCollectionEnumerator<Node>.GetCount(this);

                return mCount;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        Node INodeCollection.GetCurrentNode()
        {
            return mCurrent;
        }

        /// <summary>
        /// Gets a container (the root node) of the collection.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        CompositeNode INodeCollection.Container
        {
            get { return mContainer; }
        }

        private Node mCurrent;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly CompositeNode mContainer;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private readonly DocumentBase mDocument;
        private readonly bool mIsDeep;

        /// <summary>
        /// Caches the initial "version" of the DOM tree. Used to invalidate the collection.
        /// </summary>
        private int mInitialChangeCount;
        /// <summary>
        /// Part of speed optimization.
        /// -1 means the starting point for a new search round
        /// </summary>
        private int mCurIndex;
        /// <summary>
        /// Part of speed optimization.
        /// if set to rootNode, means the starting point for a new search round
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private Node mCurNode;
        /// <summary>
        /// Part of speed optimization.
        /// -1 means count needs to be calculated
        /// </summary>
        private int mCount;
        /// <summary>
        /// Matcher providing the select pattern for nodes.
        /// </summary>
        private readonly NodeMatcher mMatcher;
    }
}
