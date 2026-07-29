// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2005 by Roman Korchagin

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Xml.XPath;
using Aspose.JavaAttributes;
using Aspose.Words.Markup;
using Aspose.Words.Revisions;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Base class for nodes that can contain other nodes.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/aspose-words-document-object-model/">Aspose.Words Document Object Model (DOM)</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>A document is represented as a tree of nodes, similar to DOM or XmlDocument.</p>
    /// <p>For more info see the Composite design pattern.</p>
    /// <p>The <see cref="CompositeNode"/> class:</p>
    /// <list type="bullet">
    /// <item>Provides access to the child nodes.</item>
    /// <item>Implements Composite operations such as insert and remove children.</item>
    /// <item>Provides methods for XPath navigation.</item>
    /// </list>
    /// </remarks>
    /// <dev>
    /// Maintains a pointer to the list of children.
    ///
    /// Provides helper methods to insert or remove a whole range of nodes.
    ///
    /// Inserting and removing children are the most important features here.
    ///
    /// Public API methods for insert and remove verify the arguments well and raise the insert/delete
    /// node events to the owner document.
    ///
    /// The child nodes are stored in a doubly linked list and the composite node knows its first and last children.
    /// This approach was taken from XmlElement (with addition of doubly linking). Inserting or removing a node just
    /// means updating the PrevSibling, NextSibling and ParentNode of the appropriate nodes.
    /// </dev>
    [JavaGenericParameter("V extends Node"), JavaGenericArguments("Iterable<V>")]
    public abstract class CompositeNode : Node, IEnumerable<Node>, INodeCollection
#if !JAVA && !CPLUSPLUS  // XPath navigation is supported on Java and C++, but implementing this interface is not needed.
        , IXPathNavigable
#endif
    {
        /// <summary>
        /// Ctor for creating special nodes that never need to know the document.
        /// </summary>
        protected CompositeNode()
        {
        }

        /// <summary>
        /// Ctor to use.
        /// </summary>
        protected CompositeNode(DocumentBase doc) : base(doc)
        {
        }

        #if CPLUSPLUS
        /// <summary>
        /// Destructor for CompositeNode
        /// </summary>
        /// <dev>
        /// Fixes a stack overflow exception during large Document deconstruction
        /// Deconstruct children iterative way instead of default recursive way
        /// WORDSCPP-739 and WORDSCPP-703
        /// </dev>
        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition(false)]
        ~CompositeNode()
        {
            if (mLastChild != mFirstChild)
            {
                var current = mLastChild;
                mLastChild = null;

                while (current != mFirstChild)
                {
                    var prev = current.PrevNode;
                    prev.NextNode = null;
                    current = prev;
                }

                mFirstChild = null;
            }
        }
        #endif

        /// <summary>
        /// Removes this node from the parent document, but keeps the children inside the tree,
        /// by attaching them to direct parent of this node into the place previously occupied by this node.
        /// </summary>
        /// <remarks>
        /// This node largely makes sense for <see cref="NodeType.StructuredDocumentTag"/> and <see cref="NodeType.SmartTag"/>,
        /// because they contain children of the same level as they are <see cref="MarkupLevel"/>.
        /// </remarks>
        protected void CoreRemoveSelfOnly()
        {
            if (ParentNode == null)
                throw new InvalidOperationException("This node is not attached to any document");

            using (new SuspendMappedCustomXmlUpdateDocument(Document))
            {
                while (HasChildNodes)
                {
                    // Move all children on the same level to preserve them
                    // in the document when their parent is deleted.
                    ParentNode.InsertAfter(LastChild, this);
                }

                ParentNode.RemoveChildCore(this, true);
            }
        }

        /// <summary>
        /// Returns <c>true</c> as this node can have child nodes.
        /// </summary>
        public override bool IsComposite
        {
            get { return true; }
        }

        /// <summary>
        /// Returns <c>true</c> if this node has any child nodes.
        /// </summary>
        public bool HasChildNodes
        {
            get { return (LastChild != null); }
        }

        /// <summary>
        /// Returns <c>true</c> if this node has any descendants other than markup nodes.
        /// </summary>
        /// <remarks>
        /// Sometimes a CompositeNode that is only container for empty Markup nodes is a signal of invalid node.
        /// This was the case with Defect 25952
        /// </remarks>
        internal bool HasNonMarkupDescendants
        {
            get { return (FirstNonMarkupDescendant != null); }
        }

        /// <summary>
        /// Returns <c>true</c> if this node has only one child node.
        /// </summary>
        internal bool HasOneChildOnly
        {
            get { return HasChildNodes && (FirstChild == LastChild); }
        }

        /// <summary>
        /// Recursively checks if any of this node's children implement <see cref="IInline"/>.
        /// </summary>
        internal bool HasInlineNodes()
        {
            for (Node childNode = FirstChild; childNode != null; childNode = childNode.NextSibling)
            {
                if (childNode is IInline)
                    return true;

                CompositeNode childNodeAsComposite = childNode as CompositeNode;
                if ((childNodeAsComposite != null) && childNodeAsComposite.HasInlineNodes())
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Gets the first child of the node.
        /// </summary>
        /// <remarks>
        /// If there is no first child node, a <c>null</c> is returned.
        /// </remarks>
        public Node FirstChild
        {
            get { return mFirstChild; }
        }

        /// <summary>
        /// Gets the last child of the node.
        /// </summary>
        /// <remarks>
        /// If there is no last child node, a <c>null</c> is returned.
        /// </remarks>
        public Node LastChild
        {
            get { return mLastChild; }
        }

        /// <summary>
        /// Gets the first non-markup <see cref="NodeUtil.IsMarkupNode(Words.NodeType)"/> descendant of the node in a depth-first traverse.
        /// </summary>
        internal Node FirstNonMarkupDescendant
        {
            get { return NodeUtil.GetFirstNonMarkupNodeInDepthFirstTraverse(FirstChild, false); }
        }

        /// <summary>
        /// Gets the last non-markup <see cref="NodeUtil.IsMarkupNode(Words.NodeType)"/> descendant of the node in a depth-first traverse.
        /// </summary>
        internal Node LastNonMarkupDescendant
        {
            get { return NodeUtil.GetLastNonMarkupNodeInDepthFirstTraverse(LastChild, false); }
        }

        /// <summary>
        /// Gets the first non-markup (<see cref="NodeUtil.IsMarkupNode(Words.NodeType)"/>) composite descendant of
        /// the node in a depth-first traverse.
        /// </summary>
        internal CompositeNode FirstNonMarkupCompositeDescendant
        {
            get { return (CompositeNode)NodeUtil.GetFirstNonMarkupNodeInDepthFirstTraverse(FirstChild, true); }
        }

        /// <summary>
        /// Gets the last non-markup (<see cref="NodeUtil.IsMarkupNode(Words.NodeType)"/>) composite descendant of
        /// the node in a depth-first traverse.
        /// </summary>
        internal CompositeNode LastNonMarkupCompositeDescendant
        {
            get { return (CompositeNode)NodeUtil.GetLastNonMarkupNodeInDepthFirstTraverse(LastChild, true); }
        }

        /// <summary>
        /// Gets the first non-annotation (<see cref="NodeUtil.IsCrossStructureAnnotation(Node)"/>) child of the node.
        /// </summary>
        internal Node FirstNonAnnotationChild
        {
            get
            {
                Node result = FirstChild;
                while ((result != null) && NodeUtil.IsCrossStructureAnnotation(result))
                    result = result.NextSibling;
                return result;
            }
        }

        /// <summary>
        /// Gets the last non-annotation (<see cref="NodeUtil.IsCrossStructureAnnotation(Node)"/>) child of the node.
        /// </summary>
        internal Node LastNonAnnotationChild
        {
            get
            {
                Node result = LastChild;
                while ((result != null) && NodeUtil.IsCrossStructureAnnotation(result))
                    result = result.PreviousSibling;
                return result;
            }
        }

        /// <summary>
        /// Gets the number of immediate children of this node.
        /// </summary>
        public int Count
        {
            get
            {
                int count = 0;
                for (Node node = FirstChild; node != null; node = node.NextSibling)
                    count++;
                return count;
            }
        }

        internal override Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            CompositeNode lhs = (CompositeNode)base.Clone(isCloneChildren, cloningListener);

            lhs.mLastChild = null;
            lhs.mFirstChild = null;

            if (isCloneChildren)
            {
                for (Node srcChild = FirstChild; srcChild != null; srcChild = srcChild.NextSibling)
                    lhs.AppendChildForLoad(srcChild.Clone(true, cloningListener));
            }

            return lhs;
        }

        /// <summary>
        /// Gets the text of this node and of all its children.
        /// </summary>
        ///<remarks>
        ///<p>The returned string includes all control and special characters as described in <see cref="ControlChar"/>.</p>
        ///</remarks>
        public override string GetText()
        {
            StringBuilder builder = new StringBuilder();
            GetTextToBuilder(builder);
            return builder.ToString();
        }

        /// <summary>
        /// Puts the text of this node and of all its children to the specified string builder.
        /// </summary>
        internal override void GetTextToBuilder(StringBuilder builder)
        {
            GetChildrenTextInternal(builder);
            builder.Append(GetEndText());
        }

        /// <overloads>Returns a collection of child nodes that match the specified type.</overloads>
        /// <summary>
        /// Returns a live collection of child nodes that match the specified type.
        /// </summary>
        /// <remarks>
        /// <p>The collection of nodes returned by this method is always live.</p>
        /// </remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="CompositeNode.GetChildNodesLive"]/*'/>
        /// <param name="nodeType">Specifies the type of nodes to select.</param>
        /// <param name="isDeep"><c>true</c> to select from all child nodes recursively;
        /// <c>false</c> to select only among immediate children. </param>
        /// <returns>A live collection of child nodes of the specified type.</returns>
        public NodeCollection GetChildNodes(NodeType nodeType, bool isDeep)
        {
            return new NodeCollection(this, nodeType, isDeep);
        }

        /// <summary>
        /// Returns a live collection of child nodes that match the specified types.
        /// </summary>
        internal NodeCollection GetChildNodes(NodeType[] nodeTypes, bool isDeep)
        {
            return new NodeCollection(this, nodeTypes, isDeep);
        }

        /// <summary>
        /// Returns an Nth child node that matches the specified type.
        /// </summary>
        /// <remarks>
        /// <p>If index is out of range, a <c>null</c> is returned.</p>
        /// </remarks>
        /// <param name="nodeType">Specifies the type of the child node.</param>
        /// <param name="index">Zero based index of the child node to select.
        /// Negative indexes are also allowed and indicate access from the end,
        /// that is -1 means the last node.</param>
        /// <param name="isDeep"><c>true</c> to select from all child nodes recursively;
        /// <c>false</c> to select only among immediate children. See remarks for more info.</param>
        /// <returns>The child node that matches the criteria or <c>null</c> if no matching node is found.</returns>
        /// <remarks>Note that markup nodes (<see cref="NodeType.StructuredDocumentTag"/> and <see cref="NodeType.SmartTag "/>)
        /// are traversed even when <paramref name="isDeep"/> = <c>false</c> and <see cref="GetChild"/> is invoked for non-markup node type. For example if the first run in a para
        /// is wrapped in a <see cref="StructuredDocumentTag"/>, it will still be returned by <see cref="GetChild"/>(<see cref="NodeType.Run"/>, 0, <c>false</c>).</remarks>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "isLive",
            Justification = "Public API, obsolete.")]
        public Node GetChild(NodeType nodeType, int index, bool isDeep)
        {
            if ((index >= 0) && !isDeep)
            {
                // Getting Nth flat child from the start could be used quite often, so this
                // is a simple inline performance optimization.

                // Need to repeat count times, for example 1 time for the first element (zero index).
                int count = index + 1;

                // This is the same technique as in NodeCollection when performing a flat enumeration.
                // We need to ignore sdt nodes if the user wants nodes of a specific type.
                bool isSkipMarkupNodes = NodeUtil.IsSkipMarkupNodesInFlatEnumeration(nodeType);
                Node node = (isSkipMarkupNodes) ? FirstNonMarkupDescendant : FirstChild;
                while (node != null)
                {
                    if ((node.NodeType == nodeType) || (nodeType == NodeType.Any))
                        count--;

                    if (count == 0)
                        return node;

                    node = (isSkipMarkupNodes) ? node.NextNonMarkupNodeLimited : node.NextSibling;
                }
            }
            else
            {
                NodeCollection nodes = GetChildNodes(nodeType, isDeep);
                return nodes[index];
            }

            return null;
        }

        /// <summary>
        /// Selects a list of nodes matching the XPath expression.
        /// </summary>
        /// <remarks>
        /// <p>Only expressions with element names are supported at the moment. Expressions
        /// that use attribute names are not supported.</p>
        /// </remarks>
        /// <param name="xpath">The XPath expression.</param>
        /// <returns>A list of nodes matching the XPath query.</returns>
        public NodeList SelectNodes(string xpath)
        {
            return DocumentXPathNavigator.SelectNodes(this, xpath);
        }

        /// <summary>
        /// Selects the first <see cref="Node"/> that matches the XPath expression.
        /// </summary>
        /// <remarks>
        /// <p>Only expressions with element names are supported at the moment. Expressions
        /// that use attribute names are not supported.</p>
        /// </remarks>
        /// <param name="xpath">The XPath expression.</param>
        /// <returns>The first <see cref="Node"/> that matches the XPath query or <c>null</c> if no matching node is found.</returns>
        public Node SelectSingleNode(string xpath)
        {
            return DocumentXPathNavigator.SelectSingleNode(this, xpath);
        }

        /// <summary>
        /// Provides support for the for each style iteration over the child nodes of this node.
        /// </summary>
        [JavaGenericArguments("Iterator<V>", "NodeCollectionEnumerator<V>")]
        public IEnumerator<Node> GetEnumerator()
        {
            return new NodeCollectionEnumerator<Node>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

#if PYNET
        /// <summary>
        /// Adds the specified node to the end of the list of child nodes for this node.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="CompositeNode.InsertCommon"]/*'/>
        /// <param name="newChild">The node to add.</param>
        /// <returns>The node added.</returns>
        public Node AppendChild(Node newChild)
        {
            return AppendChild<Node>(newChild);
        }

        /// <summary>
        /// Adds the specified node to the beginning of the list of child nodes for this node.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="CompositeNode.InsertCommon"]/*'/>
        /// <param name="newChild">The node to add.</param>
        /// <returns>The node added.</returns>
        public Node PrependChild(Node newChild)
        {
            return PrependChild<Node>(newChild);
        }

        /// <summary>
        /// Inserts the specified node immediately after the specified reference node.
        /// </summary>
        /// <remarks>
        /// <p>If <paramref name="refChild"/> is <c>null</c>, inserts <paramref name="newChild"/> at the beginning of the list of child nodes.</p>
        /// </remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="CompositeNode.InsertCommon"]/*'/>
        /// <param name="newChild">The <see cref="Node"/> to insert.</param>
        /// <param name="refChild">The <see cref="Node"/> that is the reference node. The <paramref name="newChild"/> is placed after the <paramref name="refChild"/>.</param>
        /// <returns>The inserted node.</returns>
        public Node InsertAfter(Node newChild, Node refChild)
        {
            return InsertAfter<Node>(newChild, refChild);
        }

        /// <summary>
        /// Inserts the specified node immediately before the specified reference node.
        /// </summary>
        /// <remarks>
        /// <p>If <paramref name="refChild"/> is <c>null</c>, inserts <paramref name="newChild"/> at the end of the list of child nodes.</p>
        /// </remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="CompositeNode.InsertCommon"]/*'/>
        /// <param name="newChild">The <see cref="Node"/> to insert.</param>
        /// <param name="refChild">The <see cref="Node"/> that is the reference node. The <paramref name="newChild"/> is placed before this node.</param>
        /// <returns>The inserted node.</returns>
        public Node InsertBefore(Node newChild, Node refChild)
        {
            return InsertBefore<Node>(newChild, refChild);
        }

        /// <summary>
        /// Removes the specified child node.
        /// </summary>
        /// <remarks>
        /// <p>The parent of <paramref name="oldChild"/> is set to <c>null</c> after the node is removed.</p>
        /// </remarks>
        /// <param name="oldChild">The node to remove.</param>
        /// <returns>The removed node.</returns>
        public Node RemoveChild(Node oldChild)
        {
            return RemoveChild<Node>(oldChild);
        }
#endif

        /// <summary>
        /// Adds the specified node to the end of the list of child nodes for this node.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="CompositeNode.InsertCommon"]/*'/>
        /// <param name="newChild">The node to add.</param>
        /// <returns>The node added.</returns>
        /// <javaName>appendChild(com.aspose.words.Node)</javaName>
        public T AppendChild<T>(T newChild)
            where T : Node
        {
            return InsertAfter(newChild, LastChild);
        }

        /// <summary>
        /// Adds the specified node to the beginning of the list of child nodes for this node.
        /// </summary>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="CompositeNode.InsertCommon"]/*'/>
        /// <param name="newChild">The node to add.</param>
        /// <returns>The node added.</returns>
        /// <javaName>prependChild(com.aspose.words.Node)</javaName>
        public T PrependChild<T>(T newChild)
            where T : Node
        {
            return InsertBefore(newChild, FirstChild);
        }

        /// <summary>
        /// Inserts the specified node immediately after the specified reference node.
        /// </summary>
        /// <remarks>
        /// <p>If <paramref name="refChild"/> is <c>null</c>, inserts <paramref name="newChild"/> at the beginning of the list of child nodes.</p>
        /// </remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="CompositeNode.InsertCommon"]/*'/>
        /// <param name="newChild">The <see cref="Node"/> to insert.</param>
        /// <param name="refChild">The <see cref="Node"/> that is the reference node. The <paramref name="newChild"/> is placed after the <paramref name="refChild"/>.</param>
        /// <returns>The inserted node.</returns>
        /// <javaName>insertAfter(com.aspose.words.Node, com.aspose.words.Node)</javaName>
        public T InsertAfter<T>(T newChild, Node refChild)
            where T : Node
        {
            return Insert(newChild, refChild, true);
        }

        /// <summary>
        /// Inserts the specified node immediately before the specified reference node.
        /// </summary>
        /// <remarks>
        /// <p>If <paramref name="refChild"/> is <c>null</c>, inserts <paramref name="newChild"/> at the end of the list of child nodes.</p>
        /// </remarks>
        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="CompositeNode.InsertCommon"]/*'/>
        /// <param name="newChild">The <see cref="Node"/> to insert.</param>
        /// <param name="refChild">The <see cref="Node"/> that is the reference node. The <paramref name="newChild"/> is placed before this node.</param>
        /// <returns>The inserted node.</returns>
        /// <javaName>insertBefore(com.aspose.words.Node, com.aspose.words.Node)</javaName>
        public T InsertBefore<T>(T newChild, Node refChild)
            where T : Node
        {
            return Insert(newChild, refChild, false);
        }

        /// <summary>
        /// Removes the specified child node.
        /// </summary>
        /// <remarks>
        /// <p>The parent of <paramref name="oldChild"/> is set to <c>null</c> after the node is removed.</p>
        /// </remarks>
        /// <param name="oldChild">The node to remove.</param>
        /// <returns>The removed node.</returns>
        /// <javaName>removeChild(com.aspose.words.Node)</javaName>
        public T RemoveChild<T>(T oldChild)
            where T : Node
        {
            return RemoveChildCore(oldChild, false);
        }

        /// <summary>
        /// Removes the specified child node.
        /// </summary>
        private T RemoveChildCore<T>(T oldChild, bool skipUpdatingRelatedNodes)
            where T : Node
        {
            if (oldChild == null)
                throw new ArgumentNullException("oldChild");

            if (oldChild.ParentNode != this)
                throw new ArgumentException("This node is not a parent of the oldChild node.");

            DocumentBase doc = Document;

            NodeChangingArgs args = CompositeNodeHelper.DocumentBaseInternalEvent(doc, oldChild, this, null, NodeChangingAction.Remove);

            if (CompositeNodeHelper.DocumentBaseIsTrackRevisionsEnabled(doc))
            {
                if (RevisionTrackingUtil.TrackDeletion(oldChild))
                    return oldChild;
            }

            CompositeNodeHelper.DocumentBaseBeforeEvent(doc, args);

            if (!skipUpdatingRelatedNodes && NodeUtil.IsStructuredDocumentTagNode(oldChild))
                NodeUtil.ResetDisplacedAnnotationReferences(oldChild, false);

            T removedNode = RemoveNode(oldChild);

            CompositeNodeHelper.DocumentBaseAfterEvent(doc, args);

            return removedNode;
        }

        /// <summary>
        /// Removes all the child nodes of the current node.
        /// </summary>
        public void RemoveAllChildren()
        {
            NodeRemover.RemoveSameParent(FirstChild, null);
        }

        /// <summary>
        /// Removes all <see cref="SmartTag"/> descendant nodes of the current node.
        /// </summary>
        /// <remarks>This method does not remove the content of the smart tags.</remarks>
        public void RemoveSmartTags()
        {
            foreach (SmartTag smartTag in GetChildNodes(NodeType.SmartTag, true))
                smartTag.CoreRemoveSelfOnly();
        }

        /// <summary>
        /// Just appends node without performing any checks.
        /// Use only when you can be sure the tree will be valid.
        ///
        /// This does not even mark the document tree as modified so any node collections
        /// that you already have might fail to show new nodes added using this method.
        /// You should avoid using node collections if you are using this method.
        /// </summary>
        internal Node AppendChildForLoad(Node newChild)
        {
            Debug.Assert(newChild.ParentNode == null, "The node is already a child of another node.");
            // WORDSNET-4319 RTF importer creates two Body or two HeaderFooter of the same type for
            // some documents. It is not too bad because it actually deletes the dummy HeaderFooter
            // after processing and extra Body is created for an invalid document.
            // RK But I can't improve RTF importer right now so I have to comment out this check.
            // Return it back when the defect is fixed.
            // Debug.Assert(CanInsert(newChild, mLastChild, true))

            if (mLastChild == null)
            {
                // First child node to be added to the parent, it is linked to nothing.
                newChild.PrevNode = null;
                newChild.NextNode = null;
                mFirstChild = newChild;
            }
            else
            {
                newChild.PrevNode = mLastChild;
                newChild.NextNode = null;
                mLastChild.NextNode = newChild;
            }

            mLastChild = newChild;
            newChild.SetParent(this);

            return newChild;
        }

        /// <summary>
        /// Inserts nodes starting from the start node up to, but not including the end node
        /// into this node before the reference node.
        /// </summary>
        /// <param name="start">Node to start moving from.</param>
        /// <param name="end">Can be <c>null</c> to indicate move all from start to the end.</param>
        /// <param name="refNode">The nodes will be inserted before this node.</param>
        internal void InsertBefore(Node start, Node end, Node refNode)
        {
            InsertBefore(start, end, refNode, false);
        }

        /// <summary>
        /// Inserts nodes starting from the start node up to, but not including the end node
        /// into this node before the reference node.
        /// </summary>
        /// <param name="start">Node to start moving from.</param>
        /// <param name="end">Can be <c>null</c> to indicate move all from start to the end.</param>
        /// <param name="refNode">The nodes will be inserted before this node.</param>
        /// <param name="safe">Prevent insertion errors when <c>true</c>.</param>
        internal void InsertBefore(Node start, Node end, Node refNode, bool safe)
        {
            if ((end != null) && (end.ParentNode != start.ParentNode))
                throw new ArgumentException("The start and end nodes should have the same parent.");

            Node curNode = start;
            while (curNode != end)
            {
                Node nextNode = curNode.NextSibling;

                if (!safe || CanInsert(curNode))
                    InsertBefore(curNode, refNode);

                curNode = nextNode;
            }
        }

        /// <summary>
        /// Inserts nodes starting from the start node up to, but not including the end node
        /// into this node after the reference node.
        /// </summary>
        /// <param name="start">Node to start moving from.</param>
        /// <param name="end">Can be <c>null</c> to indicate move all from start to the end.</param>
        /// <param name="refNode">The nodes will be inserted after this node.</param>
        internal void InsertAfter(Node start, Node end, Node refNode)
        {
            if ((end != null) && (end.ParentNode != start.ParentNode))
                throw new ArgumentException("The start and end nodes should have the same parent.");

            Node curNode = start;
            while (curNode != end)
            {
                Node nextNode = curNode.NextSibling;
                refNode = InsertAfter(curNode, refNode);
                curNode = nextNode;
            }
        }

        /// <summary>
        /// Returns the index of the specified child node in the child node array.
        /// </summary>
        /// <remarks>
        /// Returns -1 if the node is not found in the child nodes.
        /// </remarks>
        public int IndexOf(Node child)
        {
            int index = 0;
            for (Node curChild = FirstChild; curChild != null; curChild = curChild.NextSibling)
            {
                if (curChild == child)
                    return index;
                else
                    index++;
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of a child node which contains the specified descendant node.
        /// </summary>
        /// <remarks>
        /// Returns -1 if the node is not found in the child nodes.
        /// </remarks>
        internal int IndexOfChildByDescendant(Node descendant, bool skipAnnotations)
        {
            int index = 0;
            for (Node child = FirstChild; child != null; child = child.NextSibling)
            {
                if (!skipAnnotations || !NodeUtil.IsCrossStructureAnnotation(child))
                {
                    if (NodeUtil.IsAncestorOrSelf(descendant, child))
                        return index;

                    index++;
                }
            }

            return -1;
        }

        /// <summary>
        /// Recursively tries to find the childmost node that contains the position.
        /// Returns <c>null</c> if no node that contains this position was found.
        /// </summary>
        internal override Node GetNodeFromPos(int position)
        {
            //This algorithm is adopted from the .NET BinarySearch function.
            int lo = 0;
            int hi = IndexOf(LastChild);

            while (lo <= hi)
            {
                int i = (lo + hi) >> 1;

                Node curNode = GetChild(NodeType.Any, i, false);
                int start = curNode.GetStart();
                int end = start + curNode.GetTextLength();

                if (position < start)
                {
                    //Position is before this node, make next iteration to look closer to the beginning.
                    hi = i - 1;
                }
                else if (position >= end)
                {
                    //Position is after this node, make next iteration to look close to the end.
                    lo = i + 1;
                }
                else
                {
                    //Position is between start and end, recurse into the children.
                    return curNode.GetNodeFromPos(position);
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the length of the text of the child nodes. Faster than using GetText().Length.
        /// </summary>
        internal override int GetTextLength()
        {
            int result = 0;
            for (Node srcChild = FirstChild; srcChild != null; srcChild = srcChild.NextSibling)
                result += srcChild.GetTextLength();

            result += GetEndText().Length;
            return result;
        }

        /// <summary>
        /// Override in nodes that need to add some text after the children text, for example paragraph mark.
        /// </summary>
        internal virtual string GetEndText()
        {
            return string.Empty;
        }

        /// <summary>
        /// Gets text of all children nodes without appending "end text" that some composite nodes might have.
        /// </summary>
        internal string GetChildrenText()
        {
            StringBuilder builder = new StringBuilder();
            GetChildrenTextInternal(builder);
            return builder.ToString();
        }

        /// <summary>
        /// Puts text of all children nodes into the specified string builder without appending "end text"
        /// that some composite nodes might have.
        /// </summary>
        private void GetChildrenTextInternal(StringBuilder builder)
        {
            for (Node child = FirstChild; child != null; child = child.NextSibling)
                child.GetTextToBuilder(builder);
        }

        /// <summary>
        /// Calls AcceptStart, then AcceptChildren, then AcceptEnd.
        /// </summary>
        /// <param name="visitor"></param>
        /// <returns></returns>
        protected bool AcceptCore(DocumentVisitor visitor)
        {
            switch (AcceptStart(visitor))
            {
                case VisitorAction.Continue:
                    break;
                case VisitorAction.SkipThisNode:
                    return true;
                case VisitorAction.Stop:
                    return false;
                default:
                    throw new InvalidOperationException("Unknown visitor action.");
            }

            if (!AcceptChildren(visitor))
                return false;

            return VisitorActionToBool(AcceptEnd(visitor));
        }

        /// <summary>
        /// When implemented in a derived class, calls the VisitXXXStart method of the specified document visitor.
        /// </summary>
        [JavaThrows(true)]
        public abstract VisitorAction AcceptStart(DocumentVisitor visitor);

        /// <summary>
        /// When implemented in a derived class, calls the VisitXXXEnd method of the specified document visitor.
        /// </summary>
        [JavaThrows(true)]
        public abstract VisitorAction AcceptEnd(DocumentVisitor visitor);

        /// <summary>
        /// Invokes Accept(visitor) for every child node. Returns <c>true</c> to continue enumeration.
        /// </summary>
        protected bool AcceptChildren(DocumentVisitor visitor)
        {
            // Use while "loop" instead of the usual "for" to allow the child to remove itself during visiting.
            Node child = FirstChild;
            while (child != null)
            {
                Node nextChild = child.NextSibling;

                if (!child.Accept(visitor))
                    return false;

                child = nextChild;
            }

            return true;
        }

        /// <summary>
        /// Implement in derived classes and return <c>true</c> if the node can be inserted into this node.
        /// </summary>
        internal virtual bool CanInsert(Node newChild)
        {
            return true;
        }

        /// <summary>
        /// Inserts a child before or after the reference node.
        /// Reference node can be null (like in XmlNode).
        /// If inserting after a null reference node, inserts at the beginning.
        /// If inserting before a null reference node, inserts at the end.
        /// Note that inserting before could be a bit slower than inserting after.
        /// </summary>
        internal T Insert<T>(T newChild, Node refChild, bool isAfter)
            where T : Node
        {
            if ((refChild != null) && (refChild.ParentNode != this))
                throw new ArgumentException("The reference node is not a child of this node.");

            if (newChild == null)
                throw new ArgumentException("Cannot insert a null node.");

            if (newChild == this)
                throw new ArgumentException("Cannot add a node to self.");

            if (IsAncestorNode(newChild))
                throw new ArgumentException("The newChild is an ancestor of this node.");

            if (newChild == refChild)
                throw new ArgumentException("Cannot add a node before/after itself.");

            DocumentBase thisDoc = Document;
            DocumentBase childDoc = newChild.Document;
            CompositeNodeHelper.DocumentBaseCompareException(thisDoc, childDoc);

            //System nodes (such as mail merge marker) are allowed to be inserted anywhere.
            if ((newChild.NodeType != NodeType.System) && !CanInsert(newChild))
                throw new ArgumentException("Cannot insert a node of this type at this location.");

            CompositeNode oldParent = newChild.ParentNode;
            Node oldNextSibling = newChild.NextSibling;

            //Remove the child from its current parent.
            if (oldParent != null)
            {
                // Disable revision tracking to not generate Deletion revision for this node.
                // RevisionTrackingUtil.TrackInsertion is used below to generate necessary revisions.
                using (new SuspendTrackRevisionsDocument(thisDoc))
                    oldParent.RemoveChildCore(newChild, true);
            }

            NodeChangingArgs args = CompositeNodeHelper.DocumentBaseInternalEvent(thisDoc, newChild, null, this, NodeChangingAction.Insert);
            CompositeNodeHelper.DocumentBaseBeforeEvent(thisDoc, args);

            // Insert the node into the linked list.
            if (mLastChild == null)
            {
                // This is a first child node to be added to the parent, link it to nothing.
                newChild.PrevNode = null;
                newChild.NextNode = null;
                mFirstChild = newChild;
                mLastChild = newChild;
            }
            else
            {
                if (isAfter)
                {
                    if (refChild != null)
                    {
                        // Normal insert after some node.
                        InsertAfterCore(newChild, refChild);
                    }
                    else
                    {
                        // Inserting after null means inserting before the first child.
                        InsertBeforeCore(newChild, mFirstChild);
                    }
                }
                else
                {
                    if (refChild != null)
                    {
                        InsertBeforeCore(newChild, refChild);
                    }
                    else
                    {
                        // Inserting before null means insert after the last child.
                        InsertAfterCore(newChild, mLastChild);
                    }
                }
            }

            // Make the child aware it has a parent now.
            newChild.SetParent(this);

            CompositeNodeHelper.DocumentBaseAfterEvent(thisDoc, args);

            if (CompositeNodeHelper.DocumentBaseIsTrackRevisionsEnabled(thisDoc))
                RevisionTrackingUtil.TrackInsertion(newChild, oldParent, oldNextSibling);

            return newChild;
        }

        /// <summary>
        /// Modifies the links to insert newChild after prevChild.
        /// </summary>
        [CppForceSharedApi]
        private void InsertAfterCore(Node newChild, Node prevChild)
        {
            Node nextChild = prevChild.NextNode;

            newChild.PrevNode = prevChild;
            newChild.NextNode = nextChild;
            prevChild.NextNode = newChild;
            // If added to the end, change the new last child node.
            if (nextChild == null)
                mLastChild = newChild;
            else
                nextChild.PrevNode = newChild;
        }

        /// <summary>
        /// Modifies the links to insert newChild before nextChild.
        /// </summary>
        [CppForceSharedApi]
        private void InsertBeforeCore(Node newChild, Node nextChild)
        {
            Node prevChild = nextChild.PrevNode;

            newChild.PrevNode = prevChild;
            newChild.NextNode = nextChild;
            nextChild.PrevNode = newChild;

            // In inserted to the first position, change the first child node.
            if (prevChild == null)
                mFirstChild = newChild;
            else
                prevChild.NextNode = newChild;
        }

        /// <summary>
        /// Performs real node removing by arranging all necessary node links.
        /// </summary>
        private T RemoveNode<T>(T oldChild)
            where T : Node
        {
            if (oldChild == mFirstChild)
            {
                if (mFirstChild == mLastChild)
                {
                    // The first and the only child is being deleted, set it to no child nodes at all.
                    mFirstChild = null;
                    mLastChild = null;
                }
                else
                {
                    // The first child is being deleted.
                    mFirstChild = oldChild.NextNode;
                    mFirstChild.PrevNode = null;
                }
            }
            else
            {
                // Not a first node is being deleted, set the previous node to point to next sibling of the deleted node.
                Node prevNode = oldChild.PrevNode;
                Node nextNode = oldChild.NextNode;
                prevNode.NextNode = nextNode;
                // If removing the last node, update the last node pointer.
                if (nextNode == null)
                    mLastChild = prevNode;
                else
                    nextNode.PrevNode = prevNode;
            }

            // Disconnect the removed node.
            oldChild.NextNode = null;
            oldChild.PrevNode = null;
            oldChild.SetParent(null);

            return oldChild;
        }

        /// <summary>
        /// Creates navigator which can be used to traverse and read nodes.
        /// </summary>
        /// <msonly>Remove this from Java public API.</msonly>
        [JavaDelete("XPath navigation is supported on Java, but implementing this interface is not needed.")]
        [CodePorting.Translator.Cs2Cpp.CppSkipEntity("XPath navigation is not supported on C++.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public XPathNavigator CreateNavigator()
        {
            return new DocumentXPathNavigator(this);
        }

        #region INodeCollection

        /// <summary>
        /// <para>Represents this composite node as node collection for <see cref="NodeCollectionEnumerator{T}"/>.</para>
        /// <para>Gets the next matching node in the collection forward from the specified node.</para>
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        Node INodeCollection.GetNextMatchingNode(Node curNode)
        {
            Node prevNode = curNode;
            mCurrent = (curNode == this) ? FirstChild : curNode.NextSibling;
            return prevNode;
        }

        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        [EditorBrowsable(EditorBrowsableState.Never)]
        Node INodeCollection.GetCurrentNode()
        {
            return mCurrent;
        }

        /// <summary>
        /// <para>Represents this composite node as node collection for <see cref="NodeCollectionEnumerator{T}"/>.</para>
        /// <para>Returns <c>this</c>.</para>
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        CompositeNode INodeCollection.Container
        {

            [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
            get { return this; }
        }

#endregion

        private Node mFirstChild;
        private Node mLastChild;
        private Node mCurrent;

#if DEBUG
        public override void dd()
        {
            base.dd();

            Debug.Indent();
            for (Node node = FirstChild; node != null; node = node.NextSibling)
                node.dd();
            Debug.Unindent();
        }
#endif
    }
}
