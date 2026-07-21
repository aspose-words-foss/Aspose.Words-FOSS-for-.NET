// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2005 by Roman Korchagin

using System;
using System.Collections.Generic;
using System.Text;
using Aspose.JavaAttributes;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.Drawing;
using Aspose.Words.Fields;
using Aspose.Words.Markup;
using Aspose.Words.Math;
using Aspose.Words.Notes;
using Aspose.Words.RW.Factories;
using Aspose.Words.Saving;
using Aspose.Words.Tables;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Words
{
    /// <summary>
    /// Base class for all nodes of a Word document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/aspose-words-document-object-model/">Aspose.Words Document Object Model (DOM)</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>A document is represented as a tree of nodes, similar to DOM or XmlDocument.</p>
    /// <p>For more info see the Composite design pattern.</p>
    /// <p>The <see cref="Node"/> class:</p>
    /// <list type="bullet">
    /// <item>Defines the child node interface.</item>
    /// <item>Defines the interface for visiting nodes.</item>
    /// <item>Provides default cloning capability.</item>
    /// <item>Implements parent node and owner document mechanisms.</item>
    /// <item>Implements access to sibling nodes.</item>
    /// </list>
    /// </remarks>
    public abstract class Node
    {
        /// <summary>
        /// Ctor for creating special nodes that never need to know the document.
        /// </summary>
        protected Node()
        {
        }

        /// <summary>
        /// Ctor. This is the normal ctor to use.
        /// </summary>
        /// <param name="doc"></param>
        protected Node(DocumentBase doc) : this()
        {
            SetDocument(doc);
        }

#if CPLUSPLUS
        /// <summary>
        /// Destructor for Node.
        /// </summary>
        /// <dev>
        /// Removes node from the DocumentBase.HangingNodesCollection.
        /// </dev>
        [CodePorting.Translator.Cs2Cpp.CppSkipDefinition(false)]
        ~Node()
        {
        }
#endif

        /// <summary>
        /// Gets the type of this node.
        /// </summary>
        public abstract NodeType NodeType
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get;
        }

        /// <summary>
        /// Gets the immediate parent of this node.
        /// </summary>
        /// <remarks>
        /// <p>If a node has just been created and not yet added to the tree,
        /// or if it has been removed from the tree, the parent is <c>null</c>.</p>
        /// </remarks>
        public CompositeNode ParentNode
        {
            get
            {
                //Parent can be null for the document node for example.
                if (mParentNode == null)
                    return null;

                // When a node is part of the tree, it knows its parent node.
                // But a node can exist outside of a tree as well, in this case the parent is null.
                // Even if the node has no parent, it always belongs to a document (knows its owner document).
                // This is implemented using  the NullNode approach stolen from XmlDocument.

                //If the parent is the special null node, return simple null to the user.
                //As a performance optimization I don't use Document.NullNode anymore.
                if (mParentNode.NodeType == NodeType.Null)
                    return null;

                return (CompositeNode)mParentNode;
            }
        }

        /// <summary>
        /// Returns first non-Markup parent of this node.
        /// </summary>
        internal CompositeNode FirstNonMarkupParentNode
        {
            get
            {
                // Recurse until find a non markup.
                return NodeUtil.IsMarkupNode(ParentNode) ? ParentNode.FirstNonMarkupParentNode : ParentNode;
            }
        }

        /// <summary>
        /// Returns first non-Markup parent and non-Office Math parent of this node.
        /// </summary>
        /// <remarks>
        /// DV Initial name I chose was FirstNonMarkupNonOfficeMathParentNode, but then I thought that we might potentially have other node types
        /// that should be skipped during traverse operations.
        /// </remarks>
        internal CompositeNode FirstMeaningfulParentNode
        {
            get
            {
                // Recurse until find a meaningful node.
                return NodeUtil.IsMarkupNode(ParentNode) || NodeUtil.IsOfficeMath(ParentNode) ?
                    ParentNode.FirstMeaningfulParentNode :
                    ParentNode;
            }
        }

        /// <summary>
        /// Gets the document to which this node belongs.
        /// </summary>
        /// <remarks>
        /// <p>The node always belongs to a document even if it has just been created
        /// and not yet added to the tree, or if it has been removed from the tree.</p>
        /// </remarks>
        public virtual DocumentBase Document
        {
            [System.Diagnostics.DebuggerStepThrough]
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get
            {
                return (mParentNode != null)
                    ? mParentNode.Document
                    : null;
            }
        }

        /// <summary>
        /// Returns the main (not glossary) owner document.
        /// </summary>
        internal Document FetchDocument()
        {
            DocumentBase doc = Document;
            if (doc is Document)
                return (Document)doc;

            throw new InvalidOperationException("This operation requires the node to be inside the main document.");
        }

        /// <summary>
        /// Returns the main (not glossary) owner document.
        /// in case of GlossaryDocument returns its main document.
        /// </summary>
        internal Document FetchDocumentOrGlossaryMain()
        {
            DocumentBase doc = Document;
            if (doc is Document)
                return (Document) doc;

            return ((GlossaryDocument)doc).MainDocument;
        }

        /// <summary>
        /// Gets the node immediately preceding this node.
        /// </summary>
        /// <remarks>
        /// If there is no preceding node, a <c>null</c> is returned.
        /// </remarks>
        public Node PreviousSibling
        {
            get { return PrevNode; }
        }

        /// <summary>
        /// Gets the non-cross structure annotation node immediately preceding this node.
        /// </summary>
        /// <remarks>
        /// If there is no such node, a <c>null</c> is returned.
        /// </remarks>
        internal Node PreviousNonAnnotationSibling
        {
            get
            {
                Node node = this;
                do
                {
                    node = node.PreviousSibling;
                }
                while ((node != null) && NodeUtil.IsCrossStructureAnnotation(node));

                return node;
            }
        }

        /// <summary>
        /// Gets the last non-markup node <see cref="NodeUtil.IsMarkupNode(Words.NodeType)"/>
        /// which goes before the current node in a depth-first traverse order. Can go up the tree.
        /// </summary>
        /// <remarks>
        /// The current node and its descendants are not considered.
        /// The siblings before the current node and their descendants are searched for non-markups.
        /// If nothing found among siblings and below, it is assumed that the current node may be already deep inside a tree consisting of markup composites.
        /// So the search continues from the current node's parent while it remains a markup node.
        ///
        /// The search is limited in two ways:
        /// It does not go inside the current node. So if it contains non-markup nodes, they will not be returned.
        /// It goes to the parent node only if the parent is a markup.
        /// So if the current node is inside a non-markup node, it is similar to PreviousSibling, but it looks inside previous siblings if they are markups.
        /// </remarks>
        internal Node PreviousNonMarkupNodeLimited
        {
            get
            {
                Node previousNonMarkup = NodeUtil.GetLastNonMarkupNodeInDepthFirstTraverse(PreviousSibling, false);

                if (previousNonMarkup == null)
                {
                    // No non-markups nodes found among siblings before the current node and below.
                    if (NodeUtil.IsMarkupNode(ParentNode))
                        previousNonMarkup = ParentNode.PreviousNonMarkupNodeLimited;
                }

                return previousNonMarkup;
            }
        }

        /// <summary>
        /// Gets the next non-markup node <see cref="NodeUtil.IsMarkupNode(Words.NodeType)"/>
        /// which goes after the current node in a depth-first traverse order. Can go up the tree.
        /// </summary>
        /// <remarks>
        /// The current node and its descendants are not considered.
        /// The siblings after the current node and their descendants are searched for non-markups.
        /// If nothing found among siblings and below, it is assumed that the current node may be already deep inside a tree consisting of markup composites.
        /// So the search continues from the current node's parent while it remains a markup node.
        ///
        /// The search is limited in two ways:
        /// It does not go inside the current node. So if it contains non-markup nodes, they will not be returned.
        /// It goes to the parent node only if the parent is a markup.
        /// So if the current node is inside a non-markup node, it is similar to NextSibling, but it looks inside if next siblings if they are markups.
        /// </remarks>
        internal Node NextNonMarkupNodeLimited
        {
            get
            {
                Node nextNonMarkup = NodeUtil.GetFirstNonMarkupNodeInDepthFirstTraverse(NextSibling, false);

                if(nextNonMarkup == null)
                {
                    // No non-markups nodes found among siblings after the current node and below.
                    if (NodeUtil.IsMarkupNode(ParentNode))
                        nextNonMarkup = ParentNode.NextNonMarkupNodeLimited;
                }

                return nextNonMarkup;
            }
        }

        /// <summary>
        /// Gets the last non-markup composite node (<see cref="NodeUtil.IsMarkupNode(Words.NodeType)"/>)
        /// which goes before the current node in a depth-first traverse order. Can go up the tree.
        /// </summary>
        /// <remarks>
        /// The current node and its descendants are not considered.
        /// The siblings before the current node and their descendants are searched for non-markups and composite.
        /// If nothing found among siblings and below, it is assumed that the current node may be already deep
        /// inside a tree consisting of markup composites.
        /// So the search continues from the current node's parent while it remains a markup node.
        ///
        /// The search is limited in two ways:
        /// It does not go inside the current node. So if it contains non-markup nodes, they will not be returned.
        /// It goes to the parent node only if the parent is a markup.
        /// So if the current node is inside a non-markup node, it is similar to PreviousSibling with additional checking
        /// for composite, but it looks inside previous siblings if they are markups.
        /// </remarks>
        internal CompositeNode PreviousNonMarkupCompositeLimited
        {
            get
            {
                Node node = this;
                do
                {
                    node = node.PreviousNonMarkupNodeLimited;
                }
                while ((node != null) && !node.IsComposite);

                return (CompositeNode)node;
            }
        }

        /// <summary>
        /// Gets the next non-markup composite node (<see cref="NodeUtil.IsMarkupNode(Words.NodeType)"/>)
        /// which goes after the current node in a depth-first traverse order. Can go up the tree.
        /// </summary>
        /// <remarks>
        /// The current node and its descendants are not considered.
        /// The siblings after the current node and their descendants are searched for non-markups and composite.
        /// If nothing found among siblings and below, it is assumed that the current node may be already deep
        /// inside a tree consisting of markup composites.
        /// So the search continues from the current node's parent while it remains a markup node.
        ///
        /// The search is limited in two ways:
        /// It does not go inside the current node. So if it contains non-markup nodes, they will not be returned.
        /// It goes to the parent node only if the parent is a markup.
        /// So if the current node is inside a non-markup node, it is similar to NextSibling with additional checking
        /// for composite, but it looks inside if next siblings if they are markups.
        /// </remarks>
        internal CompositeNode NextNonMarkupCompositeLimited
        {
            get
            {
                Node node = this;
                do
                {
                    node = node.NextNonMarkupNodeLimited;
                }
                while ((node != null) && !node.IsComposite);

                return (CompositeNode)node;
            }
        }

        /// <summary>
        /// Gets the node immediately following this node.
        /// </summary>
        /// <remarks>
        /// If there is no next node, a <c>null</c> is returned.
        /// </remarks>
        public Node NextSibling
        {
            get { return NextNode; }
        }

        /// <summary>
        /// Gets the non-cross structure annotation node immediately following this node.
        /// </summary>
        /// <remarks>
        /// If there is no such node, a <c>null</c> is returned.
        /// </remarks>
        internal Node NextNonAnnotationSibling
        {
            get
            {
                Node node = this;
                do
                {
                    node = node.NextSibling;
                }
                while ((node != null) && NodeUtil.IsCrossStructureAnnotation(node));

                return node;
            }
        }

        /// <summary>
        /// Returns <c>true</c> if this node can contain other nodes.
        /// </summary>
        /// <value>This method returns <c>false</c> as <see cref="Node"/> cannot have child nodes.</value>
        public virtual bool IsComposite
        {
            get { return false; }
        }

        /// <summary>
        /// Returns a <see cref="Aspose.Words.Range"/> object that represents the portion of a document that is contained in this node.
        /// </summary>
        public Range Range
        {
            get { return new Range(this); }
        }

        internal bool IsLastChild
        {
            get { return (ParentNode == null) || (this == ParentNode.LastChild); }
        }

        internal bool IsFirstChild
        {
            get { return (ParentNode == null) || (this == ParentNode.FirstChild); }
        }

        internal bool IsFirstNonZeroLengthChild
        {
            get
            {
                Node node = PreviousSibling;
                while (node != null)
                {
                    if (!NodeUtil.IsZln(node))
                        return false;

                    node = node.PreviousSibling;
                }

                return true;
            }
        }

        internal bool IsLastNonZeroLengthChild
        {
            get
            {
                Node node = NextSibling;
                while (node != null)
                {
                    if (!NodeUtil.IsZln(node))
                        return false;

                    node = node.NextSibling;
                }

                return true;
            }
        }

        /// <summary>
        /// Next node in the linked chain of nodes. The same as the <see cref="NextSibling"/> but has setter.
        /// </summary>
        [CppOverrideAccessModifier(AccessModifiers.Public)]
        internal Node NextNode
        {
            get { return mNextNode; }
            set { mNextNode = value; }
        }

        /// <summary>
        /// Previous node in the linked chain of nodes. The same as the <see cref="PreviousSibling"/> but has setter.
        /// </summary>
        [CppOverrideAccessModifier(AccessModifiers.Public)]
        internal Node PrevNode
        {
            get { return mPrevNode; }
            set { mPrevNode = value; }
        }

        /// <summary>
        /// Returns the next node or parent if it does not exist.
        /// </summary>
        internal Node NextOrParent
        {
            get { return NextNode != null ? NextNode : ParentNode; }
        }

        /// <summary>
        /// Gets a level of the node (inline, block etc).
        /// </summary>
        internal NodeLevel NodeLevel
        {
            get
            {
                switch (NodeType)
                {
                    case NodeType.Document:
                        return NodeLevel.Document;
                    case NodeType.Section:
                        return NodeLevel.Section;
                    case NodeType.Body:
                    case NodeType.HeaderFooter:
                        return NodeLevel.SectionStory;
                    default:
                        bool isInlineLevel = NodeUtil.IsInlineLevelNode(this);
                        bool isBlockLevel = NodeUtil.IsBlockLevelNode(this);
                        bool isCellLevel = NodeUtil.IsCellLevelNode(this);
                        bool isRowLevel = NodeUtil.IsRowLevelNode(this);
                        Node parent = ParentNode;
                        NodeLevel sdtParentLevel = (parent != null) && (parent.NodeType == NodeType.StructuredDocumentTag)
                            ? ParentNode.NodeLevel
                            : NodeLevel.Other;
                        NodeType parentNodeType = (parent != null) ? parent.NodeType : NodeType.Null;

                        if (isRowLevel &&
                            (
                                !(isInlineLevel || isBlockLevel || isCellLevel) ||
                                (sdtParentLevel == NodeLevel.Row) ||
                                (parentNodeType == NodeType.Table)
                            ))
                            return NodeLevel.Row;

                        if (isCellLevel &&
                            (
                                !(isInlineLevel || isBlockLevel) ||
                                (sdtParentLevel == NodeLevel.Cell) ||
                                (parentNodeType == NodeType.Row)
                            ))
                            return NodeLevel.Cell;

                        if (isBlockLevel &&
                            (
                                !isInlineLevel ||
                                (sdtParentLevel == NodeLevel.Block) ||
                                ((parent != null) && (parent.NodeLevel != NodeLevel.Block) &&
                                    (sdtParentLevel != NodeLevel.Inline) && (parentNodeType != NodeType.SmartTag) &&
                                    (parentNodeType != NodeType.OfficeMath))
                            ))
                            return NodeLevel.Block;

                        if (isInlineLevel)
                            return NodeLevel.Inline;

                        return NodeLevel.Other;
                }
            }
        }

        /// <summary>
        /// Returns the start linear position of this node.
        /// This is not public because it is global in the tree and therefore not really
        /// meaningful to the user. For example, position in body can depend on whether
        /// header node is before the body or node or after. So its internal.
        /// </summary>
        internal int GetStart()
        {
            if (ParentNode == null)
                return 0;

            int localStart = 0;
            for (Node node = ParentNode.FirstChild; node != this; node = node.NextSibling)
                localStart += node.GetTextLength();

            return ParentNode.GetStart() + localStart;
        }

        /// <summary>
        /// Returns the end linear position of this node.
        /// </summary>
        internal int GetEnd()
        {
            return GetStart() + GetTextLength();
        }

        /// <summary>
        /// Returns <c>true</c> if the position is greater than equal start and less than end.
        /// </summary>
        internal bool Contains(int position)
        {
            int start = GetStart();
            int end = start + GetTextLength();
            return ((position >= start) && (position < end));
        }

        /// <summary>
        /// Returns this node if it contains the position, otherwise <c>null</c>.
        /// </summary>
        [JavaThrows(true)]
        internal virtual Node GetNodeFromPos(int position)
        {
            return (Contains(position)) ? this : null;
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Node.Clone(bool)"]/*'/>
        /// <dev>
        /// You should use the <see cref="Clone(bool, INodeCloningListener)"/> overload when
        /// the node cloning is a part of another (usually composite) node cloning process.
        /// And you should pass down the <see cref="INodeCloningListener"/> parameter in this case.
        /// </dev>
        public Node Clone(bool isCloneChildren)
        {
            return Clone(isCloneChildren, new CommentIdGenerator(this));
        }

        internal virtual Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            // This creates a node of the correct class and bitwise copies all fields.
            Node lhs = (Node)MemberwiseClone();

            // Make the node parentless, but belonging to the document by referencing the document's special null node.
            // The if condition is required for a Document object to work as it has null parent.
            if (mParentNode != null)
                lhs.mParentNode = Document.NullNode;

            //And will have no siblings.
            lhs.mNextNode = null;
            lhs.mPrevNode = null;

            if (cloningListener != null)
                cloningListener.NotifyNodeCloned(this, lhs);

            return lhs;
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Node.Accept"]/*'/>
        [JavaThrows(true)]
        public abstract bool Accept(DocumentVisitor visitor);

        /// <summary>
        /// A utility function that all derived classes can call to convert a visitor action
        /// into a boolean value when visiting the end of the node.
        /// </summary>
        protected static bool VisitorActionToBool(VisitorAction action)
        {
            switch (action)
            {
                case VisitorAction.Continue:
                case VisitorAction.SkipThisNode:
                    return true;
                case VisitorAction.Stop:
                    return false;
                default:
                    throw new InvalidOperationException("Unknown visitor action.");
            }
        }

        /// <summary>
        /// Gets the text of this node and of all its children.
        /// </summary>
        ///<remarks>
        ///<p>The returned string includes all control and special characters as described in <see cref="ControlChar"/>.</p>
        ///</remarks>
        public virtual string GetText()
        {
            return string.Empty;
        }

        /// <summary>
        /// Puts the text of this node and of all its children to the specified string builder.
        /// </summary>
        internal virtual void GetTextToBuilder(StringBuilder builder)
        {
            builder.Append(GetText());
        }

#if !CPLUSPLUS
        /// <summary>
        /// Gets the first ancestor of the specified object type.
        /// </summary>
        /// <param name="ancestorType">The object type of the ancestor to retrieve.</param>
        /// <returns>The ancestor of the specified type or <c>null</c> if no ancestor of this type was found.</returns>
        /// <remarks>
        /// <p>The ancestor type matches if it is equal to <paramref name="ancestorType"/> or derived from <paramref name="ancestorType"/>.</p>
        /// </remarks>
        public CompositeNode GetAncestor(Type ancestorType)
        {
            CompositeNode curParent = ParentNode;
            while (curParent != null)
            {
                if (ancestorType.IsInstanceOfType(curParent))
                    return curParent;

                curParent = curParent.ParentNode;
            }

            return null;
        }
#else
        public T GetAncestorOf<T>() where T : CompositeNode
        {
            Node curParent = ParentNode as Node;
            while (curParent != null)
            {
                var parent = curParent as T;
                if (parent != null)
                    return parent;

                curParent = curParent.ParentNode as Node;
            }
            return null;
        }
#endif

        internal IStory GetAncestorIStory()
        {
            Node curParent = ParentNode;

            while (curParent != null)
            {
                IStory parent = curParent as IStory;
                if (parent != null)
                    return parent;

                curParent = curParent.ParentNode;
            }

            return null;
        }

        /// <summary>
        /// Gets the first ancestor of the specified <see cref="Aspose.Words.NodeType"/>.
        /// </summary>
        /// <param name="ancestorType">The node type of the ancestor to retrieve.</param>
        /// <returns>The ancestor of the specified type or <c>null</c> if no ancestor of this type was found.</returns>
        public CompositeNode GetAncestor(NodeType ancestorType)
        {
            CompositeNode curParent = ParentNode;
            while (curParent != null)
            {
                if (curParent.NodeType == ancestorType)
                    return curParent;

                curParent = curParent.ParentNode;
            }
            return null;
        }

        /// <summary>
        /// Removes itself from the parent.
        /// </summary>
        public void Remove()
        {
            if (ParentNode == null)
                throw new InvalidOperationException("Cannot remove because there is no parent.");
            ParentNode.RemoveChild(this);
        }

        /// <summary>
        /// Setting parent node to null actually sets it to reference the <c>null</c>
        /// node of the document so we still know the document.
        /// </summary>
        [CppOverrideAccessModifier(AccessModifiers.Public)]
        internal void SetParent(Node parentNode)
        {
            mParentNode = (parentNode != null) ? parentNode : Document.NullNode;
#if CPLUSPLUS // alexnosk: Workaround for hanging nodes. Must be removed when CsToCppPorter.MemoryManagement.BindLifetime is used where required.
            // Remove node from document's hanging nodes if it is added into the document.
            // Add node to hanging nodes if it is removed from document.
            if (parentNode != null)
                Document.RemoveHangingNode(this);
            else
                Document.AddHangingNode(this);
#endif
        }

        /// <summary>
        /// Makes the node belong to a different document and have no parent. Use with care.
        /// </summary>
        internal void SetDocument(DocumentBase doc)
        {
            // RK I guess doc can be null when creating a document node itself.
            mParentNode = (doc != null) ? doc.NullNode : null;
#if CPLUSPLUS // alexnosk: Workaround for hanging nodes. Must be removed when CsToCppPorter.MemoryManagement.BindLifetime is used where required.
            if (doc != null)
                doc.AddHangingNode(this);
#endif
        }

        /// <summary>
        /// Returns <c>true</c> if the specified node is an ancestor of this node.
        /// </summary>
        [CppOverrideAccessModifier(AccessModifiers.Public)]
        internal bool IsAncestorNode(Node node)
        {
            if (node == null)
                throw new ArgumentNullException("node");

            Node n = ParentNode;
            while (n != null && n != this)
            {
                if (n == node)
                    return true;
                n = n.ParentNode;
            }
            return false;
        }

        /// <summary>
        /// Returns top most ancestor of the node.
        /// </summary>
        internal Node GetTopmostAncestor()
        {
            // Return Document if node was deleted.
            if (ParentNode == null)
                return Document;

            Node node = this;
            while (node.ParentNode != null)
                node = node.ParentNode;

            return node;
        }

        /// <summary>
        /// Gets the length of the text of this node.
        /// Override in descendant classes if you can make it faster than using GetText().Length.
        /// </summary>
        internal virtual int GetTextLength()
        {
            return GetText().Length;
        }

        /// <summary>
        /// Gets next node according to the pre-order tree traversal algorithm.
        /// </summary>
        /// <param name="rootNode">The top node (limit) of traversal.</param>
        /// <returns>Next node in pre-order order. Null if reached the <paramref name="rootNode"/>.</returns>
        public Node NextPreOrder(Node rootNode)
        {
            //For preorder walking, first try its child
            Node retNode = (IsComposite) ? ((CompositeNode)this).FirstChild : null;

            if (retNode == null)
            {
                //If no child, the next node forward will the be the NextSibling of this node
                //or of the first ancestor which has NextSibling so, first loop to find out such
                //an ancestor until no more ancestor or the ancestor is the rootNode.
                retNode = this;
                while ((retNode != null) &&
                    (retNode != rootNode) &&
                    (retNode.NextSibling == null))
                {
                    retNode = retNode.ParentNode;
                }

                //Then if such an ancestor exists, set the retNode to its NextSibling.
                if ((retNode != null) && (retNode != rootNode))
                    retNode = retNode.NextSibling;
            }

            //If reached the rootNode, consider having walked through the whole tree.
            if (retNode == rootNode)
                retNode = null;

            return retNode;
        }

        /// <summary>
        /// Gets the previous node according to the pre-order tree traversal algorithm.
        /// </summary>
        /// <param name="rootNode">The top node (limit) of traversal.</param>
        /// <returns>Previous node in pre-order order. Null if reached the <paramref name="rootNode"/>.</returns>
        public Node PreviousPreOrder(Node rootNode)
        {
            //For preorder walking, the previous node will be the right-most node
            //in the tree of PreviousSibling of the curNode.

            Node retNode = PreviousSibling;
            //So if the previous sibling is not null, go through the tree down to find the right-most node.
            while (retNode != null)
            {
                Node lastChild = (retNode.IsComposite) ? ((CompositeNode)retNode).LastChild : null;

                if (lastChild == null)
                    break;

                retNode = lastChild;
            }

            //If no previous sibling, the previous node will be the curNode's parentNode
            if (retNode == null)
                retNode = ParentNode;

            //If the final retNode is rootNode, consider having walked through the tree.
            if (retNode == rootNode)
                retNode = null;

            return retNode;
        }

        /// <summary>
        /// Returns first sibling node of the specified type after this node.
        /// If there is no such node returns <c>null</c>.
        /// </summary>
        internal Node NextSiblingOfType(NodeType nodeType)
        {
            Node node = NextSibling;
            while (null != node && nodeType != node.NodeType)
                node = node.NextSibling;
            return node;
        }

        /// <summary>
        /// Returns first sibling node of the specified type before this node.
        /// If there is no such node returns <c>null</c>.
        /// </summary>
        internal Node PreviousSiblingOfType(NodeType nodeType)
        {
            Node node = PreviousSibling;
            while (null != node && nodeType != node.NodeType)
                node = node.PreviousSibling;
            return node;
        }

        /// <summary>
        /// Gets the next node specified type according to the pre-order tree traversal algorithm.
        /// </summary>
        /// <param name="rootNode">The top node (limit) of traversal.</param>
        /// <param name="nodeType">Type of node to search.</param>
        /// <returns>Next node in pre-order order. Null if reached the rootNode.</returns>
        internal Node NextPreOrderOfType(Node rootNode, NodeType nodeType)
        {
            Node nextNode = NextPreOrder(rootNode);
            while (nextNode != null)
            {
                if (nextNode.NodeType == nodeType)
                    return nextNode;

                nextNode = nextNode.NextPreOrder(rootNode);
            }

            return null;
        }

        /// <summary>
        /// Gets the previous node specified type according to the pre-order tree traversal algorithm.
        /// </summary>
        /// <param name="rootNode">The top node (limit) of traversal.</param>
        /// <param name="nodeType">Type of node to search.</param>
        /// <returns>Previous node in pre-order order. Null if reached the <paramref name="rootNode"/>.</returns>
        internal Node PreviousPreOrderOfType(Node rootNode, NodeType nodeType)
        {
            Node node = PreviousPreOrder(rootNode);
            while ((node != null) && (node.NodeType != nodeType))
            {
                node = node.PreviousPreOrder(rootNode);
            }
            return node;
        }

        /// <summary>
        /// Returns the next or previous sibling of this node.
        /// </summary>
        internal Node GetNearestSibling(bool isNext)
        {
            return isNext ? NextSibling : PreviousSibling;
        }

        /// <summary>
        /// Inserts the specified node immediately after this node.
        /// </summary>
        /// <param name="node">The <see cref="Node"/> to insert.</param>
        internal Node InsertNext(Node node)
        {
            return ParentNode.InsertAfter(node, this);
        }

        /// <summary>
        /// Inserts nodes starting from the start node up to, but not including the end node after this node.
        /// </summary>
        /// <param name="start">Node to start moving from.</param>
        /// <param name="end">Can be <c>null</c> to indicate move all from start to the end.</param>
        internal void InsertNext(Node start, Node end)
        {
            ParentNode.InsertAfter(start, end, this);
        }

        /// <summary>
        /// Inserts the specified node immediately before this node.
        /// </summary>
        /// <param name="node">The <see cref="Node"/> to insert.</param>
        internal Node InsertPrevious(Node node)
        {
            return ParentNode.InsertBefore(node, this);
        }

        /// <summary>
        /// Inserts nodes starting from the start node up to, but not including the end node before this node.
        /// </summary>
        /// <param name="start">Node to start moving from.</param>
        /// <param name="end">Can be <c>null</c> to indicate move all from start to the end.</param>
        internal void InsertPrevious(Node start, Node end)
        {
            ParentNode.InsertBefore(start, end, this);
        }

        /// <summary>
        /// Exports the content of the node into a string in the specified format.
        /// </summary>
        /// <returns>The content of the node in the specified format.</returns>
        public string ToString(SaveFormat saveFormat)
        {
            IDocumentFragmentWriter fragmentWriter = FragmentWriterFactory.CreateFragmentWriter(saveFormat);
            return fragmentWriter.SaveToString(this);
        }

        /// <summary>
        /// Exports the content of the node into a string using the specified save options.
        /// </summary>
        /// <param name="saveOptions">Specifies the options that control how the node is saved.</param>
        /// <returns>The content of the node in the specified format.</returns>
        public string ToString(SaveOptions saveOptions)
        {
            IDocumentFragmentWriter fragmentWriter = FragmentWriterFactory.CreateFragmentWriter(saveOptions);
            return fragmentWriter.SaveToString(this);
        }

        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private Node mParentNode;
        private Node mNextNode;
        [CodePorting.Translator.Cs2Cpp.CppWeakPtr]
        private Node mPrevNode;

        /// <summary>
        /// Returns ancestor of the node of the specified type or story ancestor.
        /// This method doesn't ascend story masters.
        /// </summary>
        /// <param name="nodeType">Ancestor node type. If <see cref="Words.NodeType.Any"/> then stops at story node,
        /// <seealso cref="NodeUtil.IsStoryNodeType"/>.</param>
        /// <returns>Ancestor node of the specified type, or story ancestor (if requested), or Null if not found.</returns>
        internal Node GetStoryAncestor(NodeType nodeType)
        {
            for (Node node = ParentNode; null != node; node = node.ParentNode)
            {
                if (node.NodeType == nodeType)
                    return node;
                if (NodeUtil.IsStoryNodeType(node))
                {
                    if (nodeType != NodeType.Any)
                        break;
                    return node;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a <see cref="DocumentPosition"/> that is based on the previous sibling of this node or,
        /// if the sibling is null, on the parent of this node.
        /// </summary>
        /// <returns></returns>
        internal DocumentPosition GetBeforeDocumentPosition()
        {
            Node previousSibling = PreviousSibling;

            return (previousSibling != null) ?
                DocumentPosition.CreatePositionAfter(previousSibling) :
                DocumentPosition.CreatePositionBefore(ParentNode);
        }

        /// <summary>
        /// Returns a <see cref="DocumentPosition"/> that is based on the next sibling of this node or,
        /// if the sibling is null, on the parent of this node.
        /// </summary>
        /// <returns></returns>
        internal DocumentPosition GetAfterDocumentPosition()
        {
            Node nextSibling = NextSibling;

            return (nextSibling != null) ?
                DocumentPosition.CreatePositionBefore(nextSibling) :
                DocumentPosition.CreatePositionAfter(ParentNode);
        }

        /// <summary>
        /// Calculates if this node is above given node in a document tree flow.
        /// </summary>
        /// <remarks>
        /// The result is computed by finding a common ancestor and looking whose ancestor comes first.
        /// </remarks>
        internal bool IsAbove(Node node)
        {
            return NodeAIsAboveNodeB(this, node);
        }

        /// <summary>
        /// Determines whether this node is removed from a <see cref="Document"/>
        /// </summary>
        internal bool IsRemoved
        {
            get { return GetAncestor(NodeType.Document) == null; }
        }

        /// <summary>
        /// Gets a common ancestor of the specified nodes.
        /// </summary>
        internal static Node GetCommonAncestor(Node nodeA, Node nodeB)
        {
            if (nodeA == nodeB)
                return nodeA;
            if ((nodeA == null) || (nodeB == null))
                return null;

            Stack<Node> aAncestors = GetAncestors(nodeA);
            Stack<Node> bAncestors = GetAncestors(nodeB);

            return FindTheLastMatchingObject(aAncestors, bAncestors);
        }

        /// <summary>
        /// Calculates if the first node is above the second node in a document tree flow.
        /// </summary>
        /// <remarks>
        /// The result is computed by finding a common ancestor and looking whose ancestor comes first.
        /// </remarks>
        internal static bool NodeAIsAboveNodeB(Node a, Node b)
        {
            if ((a == null) || (b == null))
                return false;

            Stack<Node> aAncestors = GetAncestors(a);
            Stack<Node> bAncestors = GetAncestors(b);

            Node lastCommonAncestor = FindTheLastMatchingObject(aAncestors, bAncestors);

            if (lastCommonAncestor == null)
            {
                // The nodes are from different trees.
                return false;
            }
            else if ((aAncestors.Count == 0) || (bAncestors.Count == 0))
            {
                // a is an ancestor of b or vice versa.
                bool aIsAboveB = (bAncestors.Count > 0);
                return aIsAboveB;
            }
            else
            {
                // Whose ancestor is the first in the last common ancestor's children, is above.
                object aAncestor = aAncestors.Peek();
                object bAncestor = bAncestors.Peek();

                foreach (Node possibleAncestor in ((CompositeNode)lastCommonAncestor).GetChildNodes(NodeType.Any, false))
                {
                    if (possibleAncestor == aAncestor)
                        return true;
                    if (possibleAncestor == bAncestor)
                        return false;
                }
                // Actually, we should never get here, but the compiler does not know about it.
                return false;
            }
        }

        /// <summary>
        /// Gets a stack of ancestors of the given node with the root on top.
        /// </summary>
        private static Stack<Node> GetAncestors(Node node)
        {
            Stack<Node> ancestorStack = new Stack<Node>();

            Node currentNode = node;
            while (currentNode != null)
            {
                ancestorStack.Push(currentNode);
                currentNode = currentNode.ParentNode;
            }
            return ancestorStack;
        }

        /// <summary>
        /// Going from top of the stacks, finds the last object that is the same in two stacks.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static Node FindTheLastMatchingObject(Stack<Node> a, Stack<Node> b)
        {
            Node lastMatchingObject = null;
            while (a.Peek() == b.Peek())
            {
                lastMatchingObject = a.Pop();
                b.Pop();

                if ((a.Count == 0) || (b.Count == 0))
                    break;
            }
            return lastMatchingObject;
        }

        /// <summary>
        /// A utility method that converts a node type enum value into a user friendly string.
        /// </summary>
        public static string NodeTypeToString(NodeType nodeType)
        {
            switch (nodeType)
            {
                case NodeType.Any: return "Any";
                case NodeType.Document: return "Document";
                case NodeType.Section: return "Section";
                case NodeType.Body: return "Body";
                case NodeType.HeaderFooter: return "HeaderFooter";
                case NodeType.Table: return "Table";
                case NodeType.Row: return "Row";
                case NodeType.Cell: return "Cell";
                case NodeType.Paragraph: return "Paragraph";
                case NodeType.BookmarkStart: return "BookmarkStart";
                case NodeType.BookmarkEnd: return "BookmarkEnd";
                case NodeType.GroupShape: return "GroupShape";
                case NodeType.Shape: return "Shape";
                case NodeType.Comment: return "Comment";
                case NodeType.Footnote: return "Footnote";
                case NodeType.Run: return "Run";
                case NodeType.FieldStart: return "FieldStart";
                case NodeType.FieldSeparator: return "FieldSeparator";
                case NodeType.FieldEnd: return "FieldEnd";
                case NodeType.FormField: return "FormField";
                case NodeType.SpecialChar: return "SpecialChar";
                case NodeType.SmartTag: return "SmartTag";
                case NodeType.StructuredDocumentTag: return "StructuredDocumentTag";
                case NodeType.StructuredDocumentTagRangeStart: return "StructuredDocumentTagRangeStart";
                case NodeType.StructuredDocumentTagRangeEnd: return "StructuredDocumentTagRangeEnd";
                case NodeType.GlossaryDocument: return "GlossaryDocument";
                case NodeType.BuildingBlock: return "BuildingBlock";
                case NodeType.CommentRangeStart: return "CommentRangeStart";
                case NodeType.CommentRangeEnd: return "CommentRangeEnd";
                case NodeType.MoveFromRangeStart: return "MoveFromRangeStart";
                case NodeType.MoveFromRangeEnd: return "MoveFromRangeEnd";
                case NodeType.MoveToRangeStart: return "MoveToRangeStart";
                case NodeType.MoveToRangeEnd: return "MoveToRangeEnd";
                case NodeType.EditableRangeStart: return "EditableRangeStart";
                case NodeType.EditableRangeEnd: return "EditableRangeEnd";
                case NodeType.OfficeMath: return "OfficeMath";
                case NodeType.SubDocument: return "SubDocument";
                case NodeType.System: return "System";
                case NodeType.Null: return "Null";
                default: return "Unknown node type.";
            }
        }

        /// <summary>
        /// Specifies custom node identifier.
        /// </summary>
        /// <remarks>
        /// <p>Default is zero.</p>
        /// <p>This identifier can be set and used arbitrarily. For example, as a key to get external data.</p>
        /// <p>Important note, specified value is not saved to an output file and exists only during the node lifetime.</p>
        /// </remarks>
        public int CustomNodeId { get; set; }

        /// <summary>
        /// Returns Node Id, used for debugging and testing.
        /// </summary>
        [CodePorting.Translator.Cs2Cpp.CppConstMethod]
        internal string GetNodeId()
        {
            StringBuilder sb = new StringBuilder();

            for (Node node = this; node != null && node.ParentNode != null; node = node.ParentNode)
            {
                int index = 0;
                for (Node n = node.ParentNode.FirstChild; n != null && n != node; n = n.NextSibling)
                    index++;
                sb.AppendFormat("{0}{1}", (node == this ? "" : "."), index);
            }

            return sb.ToString().Trim();
        }

#if DEBUG
        public override string ToString()
        {
            return string.Format("{0} {1}", NodeTypeToString(NodeType), GetNodeId()).Trim();
        }

        public virtual void dd()
        {
            Debug.WriteLine(ToString());
        }
#elif JAVA
        public override string ToString()
        {
            // WORDSJAVA-2613 return implementation of toString method
            return string.Format("{0} {1}", NodeTypeToString(NodeType), GetNodeId()).Trim();
        }
#endif

        /// <summary>
        /// Returns first sibling node of the specified types after this node.
        /// If there is no such node returns <c>null</c>.
        /// </summary>
        internal Node NextSiblingOfTypes(params NodeType[] nodeTypes)
        {
            Debug.Assert(nodeTypes.Length > 0);

            Node node = NextSibling;
            while ((node != null) && !node.HasType(nodeTypes))
                node = node.NextSibling;
            return node;
        }

        /// <summary>
        /// Returns first sibling node of the specified types before this node.
        /// If there is no such node returns <c>null</c>.
        /// </summary>
        internal Node PreviousSiblingOfTypes(params NodeType[] nodeTypes)
        {
            Debug.Assert(nodeTypes.Length > 0);

            Node node = PreviousSibling;
            while ((node != null) && !node.HasType(nodeTypes))
                node = node.PreviousSibling;
            return node;
        }

        /// <summary>
        /// Returns <c>true</c>, if node has one of the specified types.
        /// </summary>
        private bool HasType(params NodeType[] nodeType)
        {
            foreach (NodeType type in nodeType)
            {
                if (NodeType == type)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Represents a class that generates IDs for cloned comment nodes.
        /// </summary>
        private class CommentIdGenerator : INodeCloningListener
        {
            internal CommentIdGenerator(Node cloningNode)
            {
                mCloningNode = cloningNode;
            }

            public void NotifyNodeCloned(Node source, Node clone)
            {
                // Do nothing if Clone is called for a comment or comment range node. Can replace Id if all comment
                // nodes are children of the cloning node.
                if (source == mCloningNode)
                    return;

                if ((clone.NodeType != NodeType.Comment) &&
                    (clone.NodeType != NodeType.CommentRangeStart) &&
                    (clone.NodeType != NodeType.CommentRangeEnd))
                {
                    return;
                }

                INodeWithAnnotationId annotation = (INodeWithAnnotationId)clone;

                if (mCommentIdMap == null)
                    mCommentIdMap = new Dictionary<int,int>();

                int newId;
                if (!mCommentIdMap.TryGetValue(annotation.IdInternal, out newId))
                {
                    newId = clone.Document.GetNextAnnotationId();
                    mCommentIdMap.Add(annotation.IdInternal, newId);
                }

                annotation.IdInternal = newId;

                int parentNewId;
                if (mCommentIdMap.TryGetValue(annotation.ParentIdInternal, out parentNewId))
                    annotation.ParentIdInternal = parentNewId;
            }

            private readonly Node mCloningNode;
            private Dictionary<int,int> mCommentIdMap;
        }
    }
}
