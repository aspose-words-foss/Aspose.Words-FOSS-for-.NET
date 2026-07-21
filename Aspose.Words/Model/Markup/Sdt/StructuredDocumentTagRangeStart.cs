// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/08/2019 by Alexey Morozov

using System;
using System.Collections;
using System.Collections.Generic;
using Aspose.Collections.Generic;
using Aspose.Words.BuildingBlocks;
using Aspose.Words.RW.Docx.Writer;

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Represents a start of <b>ranged</b> structured document tag which accepts multi-sections content.
    /// See also <see cref="StructuredDocumentTagRangeEnd"/>.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-content-control-sdt/">Structured Document Tags or Content Control</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Can be immediate child of <see cref="Body" /> node <b>only</b>.
    /// </remarks>
    /// <dev>Implements public facade for encapsulated <see cref="StructuredDocumentTag"/> node.</dev>
    public class StructuredDocumentTagRangeStart : Node, IEnumerable<Node>, IStructuredDocumentTag
    {
        /// <summary>
        /// Initializes a new instance of the <b>Structured document tag range start</b> class.
        /// </summary>
        /// <remarks>
        /// <para>The following types of SDT can be created:</para>
        /// <list type="bullet">
        /// <item><see cref="Markup.SdtType.Checkbox"/></item>
        /// <item><see cref="Markup.SdtType.DropDownList"/></item>
        /// <item><see cref="Markup.SdtType.ComboBox"/></item>
        /// <item><see cref="Markup.SdtType.Date"/></item>
        /// <item><see cref="Markup.SdtType.BuildingBlockGallery"/></item>
        /// <item><see cref="Markup.SdtType.Group"/></item>
        /// <item><see cref="Markup.SdtType.Picture"/></item>
        /// <item><see cref="Markup.SdtType.RichText"/></item>
        /// <item><see cref="Markup.SdtType.PlainText"/></item>
        /// </list>
        /// </remarks>
        /// <param name="doc">The owner document.</param>
        /// <param name="type">Type of SDT node.</param>
        public StructuredDocumentTagRangeStart(DocumentBase doc, SdtType type)
            : this(doc, new StructuredDocumentTag(doc, type, MarkupLevel.Block))
        {
        }

        internal StructuredDocumentTagRangeStart(DocumentBase doc, StructuredDocumentTag sdt)
            : base(doc)
        {
            InternalSdt = sdt;
        }

        /// <summary>
        /// Returns a live collection of child nodes that match the specified types.
        /// </summary>
        public NodeCollection GetChildNodes(NodeType nodeType, bool isDeep)
        {
            return new NodeCollection(Document, new StructuredDocumentTagRangeMatcher(nodeType, this, isDeep), true);
        }

        internal override Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            StructuredDocumentTagRangeStart lhs = (StructuredDocumentTagRangeStart)base.Clone(isCloneChildren, cloningListener);
            lhs.InternalSdt = (StructuredDocumentTag)InternalSdt.Clone(isCloneChildren, cloningListener);

            // AM. When document is cloned we should try to preserve original Ids.
            // Otherwise range link between range start/end will be lost.
            // I don't like this, maybe it's better to handle link translation in Document.Clone()? Postpone for a while.
            lhs.InternalSdt.SetIdExplicitly(InternalSdt.Id);

            return lhs;
        }

        bool IStructuredDocumentTag.IsMultiSection
        {
            get { return true; }
        }

        Node IStructuredDocumentTag.Node
        {
            get { return this; }
        }

        /// <summary>
        /// Returns <see cref="NodeType.StructuredDocumentTagRangeStart"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.StructuredDocumentTagRangeStart; }
        }

        /// <summary>
        /// Gets all nodes between this range start node and the range end node.
        /// </summary>
        internal NodeCollection ChildNodes
        {
            get { return GetChildNodes(NodeType.Any, false); }
        }

        /// <summary>
        /// Gets the last child in the stdContent range.
        /// </summary>
        /// <remarks>
        /// If there is no last child node, a <c>null</c> is returned.
        /// </remarks>
        public Node LastChild
        {
            get
            {
                if (RangeEnd != null)
                    return RangeEnd.PreviousSibling;

                return null;
            }
        }

        /// <summary>
        /// Gets the level at which this structured document tag range start occurs in the document tree.
        /// </summary>
        public MarkupLevel Level
        {
            get { return MarkupLevel.Block; }
        }

        /// <summary>
        /// Gets type of this structured document tag.
        /// </summary>
        public SdtType SdtType
        {
            get { return InternalSdt.ControlProperties.Type; }
        }

        /// <summary>
        /// Gets or sets the color of the structured document tag.
        /// </summary>
        public System.Drawing.Color Color
        {
            get { return InternalSdt.Color; }
            set { InternalSdt.Color = value; }
        }

        /// <summary>
        /// <para>Specifies a unique read-only persistent numerical Id for this structured document tag.</para>
        /// </summary>
        /// <remarks>
        /// <para> Id attribute shall follow these rules:
        /// <list type="bullet">
        /// <item>The document shall retain structured document tag ids only if the whole document
        /// is cloned <see cref="Document.Clone()"/>.</item>
        /// <item>During <see cref="DocumentBase.ImportNode(Aspose.Words.Node,bool)"/>
        /// Id shall be retained if import does not cause conflicts with other structured document tag Ids in
        /// the target document.</item>
        /// <item>
        /// If multiple structured document tag nodes specify the same decimal number value for the Id attribute,
        /// then the first structured document tag in the document shall maintain this original Id,
        /// and all subsequent structured document tag nodes shall have new identifiers assigned to them when the document is loaded.
        /// </item>
        /// <item>During standalone structured document tag <see cref="StructuredDocumentTag.Clone"/> operation new unique ID will be
        /// generated for the cloned structured document tag node.</item>
        /// <item>
        /// If Id is not specified in the source document, then the structured document tag node shall have a new unique identifier assigned
        /// to it when the document is loaded.
        /// </item>
        /// </list>
        /// </para>
        /// </remarks>
        public int Id
        {
            get { return InternalSdt.Id; }
        }

        /// <summary>
        /// When set to <c>true</c>, this property will prohibit a user from deleting this structured document tag.
        /// </summary>
        public bool LockContentControl
        {
            get { return InternalSdt.LockContentControl; }
            set { InternalSdt.LockContentControl = value; }
        }

        /// <summary>
        /// When set to <c>true</c>, this property will prohibit a user from editing the contents of this structured document tag.
        /// </summary>
        public bool LockContents
        {
            get { return InternalSdt.LockContents; }
            set { InternalSdt.LockContents = value; }
        }

        /// <summary>
        /// <para>
        /// Specifies whether the content of this structured document tag shall be interpreted to contain
        /// placeholder text (as opposed to regular text contents within the structured document tag).
        /// </para>
        /// <para>
        /// if set to <c>true</c>, this state shall be resumed (showing placeholder text) upon opening this document.
        /// </para>
        /// </summary>
        public bool IsShowingPlaceholderText
        {
            get { return InternalSdt.IsShowingPlaceholderText; }
            set { InternalSdt.IsShowingPlaceholderText = value; }
        }

        /// <summary>
        /// Gets the <see cref="BuildingBlock"/> containing placeholder text which should be displayed when
        /// this structured document tag run contents are empty, the associated mapped XML element is empty as specified
        /// via the <see cref="XmlMapping"/> element or the <see cref="IsShowingPlaceholderText"/> element is <c>true</c>.
        /// </summary>
        /// <remarks>Can be <c>null</c>, meaning that the placeholder is not applicable for this structured document tag.</remarks>
        public BuildingBlock Placeholder
        {
            get { return InternalSdt.Placeholder; }
        }

        /// <summary>
        /// <para>Gets or sets Name of the <see cref="BuildingBlock"/> containing placeholder text.</para>
        /// </summary>
        /// <exception cref="InvalidOperationException">Throw if BuildingBlock with this name <see cref="BuildingBlock.Name"/> is not present in <see cref="Document.GlossaryDocument"/>.</exception>
        public string PlaceholderName
        {
            get { return InternalSdt.PlaceholderName; }
            set { InternalSdt.PlaceholderName = value; }
        }

        /// <summary>
        /// Specifies a tag associated with the current structured document tag node.
        /// Can not be <c>null</c>.
        /// </summary>
        /// <remarks>
        /// A tag is an arbitrary string which applications can associate with structured document
        /// tag in order to identify it without providing a visible friendly name.
        /// </remarks>
        public string Tag
        {
            get { return InternalSdt.Tag; }
            set { InternalSdt.Tag = value; }
        }

        /// <summary>
        /// Specifies the friendly name associated with this structured document tag.
        /// Can not be <c>null</c>.
        /// </summary>
        public string Title
        {
            get { return InternalSdt.Title; }
            set { InternalSdt.Title = value; }
        }

        /// <summary>
        /// Gets a string that represents the XML contained within the node in the <see cref="SaveFormat.FlatOpc"/> format.
        /// </summary>
        public string WordOpenXML
        {
            get
            {
                OpcDocumentFragmentWriter writer = new OpcDocumentFragmentWriter();
                return writer.SaveToString(this);
            }
        }

        /// <summary>
        /// Gets a string that represents the XML contained within the node in the <see cref="SaveFormat.FlatOpc"/> format.
        ///
        /// Unlike the <see cref="WordOpenXML"/> property, this method generates a stripped-down document that excludes any non-content-related parts.
        /// </summary>
        /// <dev>
        /// This is experimental API to understand uncertain needs of important customer.
        /// Later we could try rework it using Node.ToString() pattern.
        /// </dev>
        public string WordOpenXMLMinimal
        {
            get
            {
                OpcDocumentFragmentWriter writer = new OpcDocumentFragmentWriter(true);
                return writer.SaveToString(this);
            }
        }

        /// <summary>
        /// Gets or sets the appearance of the structured document tag.
        /// </summary>
        public SdtAppearance Appearance
        {
            get { return InternalSdt.Appearance; }
            set
            {
                InternalSdt.Appearance = value;

                SdtContentUpdater.UpdateNonBoundDataContent(InternalSdt);
            }
        }

        /// <summary>
        /// Gets an object that represents the mapping of this structured document tag range to XML data
        /// in a custom XML part of the current document.
        /// </summary>
        /// <remarks>
        /// You can use the <see cref="Markup.XmlMapping.SetMapping(CustomXmlPart,string,string)"/> method of this
        /// object to map a structured document tag range to XML data.
        /// </remarks>
        public XmlMapping XmlMapping
        {
            get { return InternalSdt.XmlMapping; }
        }

        /// <include file='..\..\Docs\Text.xml' path='Topics/Topic[@name="Node.Accept"]/*'/>
        public override bool Accept(DocumentVisitor visitor)
        {
            return VisitorActionToBool(visitor.VisitStructuredDocumentTagRangeStart(this));
        }

        /// <summary>
        /// Specifies end of range if the <see cref="StructuredDocumentTag"/> is a ranged structured document tag.
        /// Otherwise returns <c>null</c>.
        /// </summary>
        public StructuredDocumentTagRangeEnd RangeEnd
        {
            get
            {
                if (ParentNode == null)
                    return null;

                // Resilience check. If the node is in the air, we take the last child of the parent.
                Node endRefNode = (IsRemoved)
                    ? ParentNode.LastChild
                    : Document.LastChild;

                // Range end node must be located after range start node.
                // If there is no range start node return null;
                return RangeNodeFinder.FindRangeEnd(new NodeRange(this, false, endRefNode, true), Id);
            }
        }

        /// <summary>
        /// Provides support for the for each style iteration over the child nodes of this node.
        /// </summary>
        public IEnumerator<Node> GetEnumerator()
        {
            return new StructuredDocumentTagRangeEnumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Adds the specified node to the end of the stdContent range.
        /// </summary>
        /// <param name="newChild">The node to add.</param>
        /// <returns>The node added.</returns>
        public Node AppendChild(Node newChild)
        {
            Node refNode = (LastChild != null) ? LastChild : this;
            return refNode.InsertNext(newChild);
        }

        /// <summary>
        /// Removes all the nodes between this range start node and the range end node.
        /// </summary>
        public void RemoveAllChildren()
        {
            // Do not start the removal process without making sure that we can totally complete it.
            Node rangeEnd = RangeEnd;
            if (rangeEnd == null)
                return;

            Section startSection = (ParentNode != null) ? ((Body)ParentNode).ParentSection : null;
            Section endSection = (rangeEnd.ParentNode != null) ? ((Body)rangeEnd.ParentNode).ParentSection : null;

            // Do not throw here to better follow CompositeNode pattern (composite node does not need parent to remove its child nodes).
            if ((startSection == null) || (endSection == null))
                return;

            if (ReferenceEquals(startSection, endSection))
            {
                // Just clear content between range nodes if any exists.
                if (NextSibling != rangeEnd)
                    RemoveChildRange(NextSibling, rangeEnd.PreviousSibling);
            }
            else
            {
                // Clear first section content after range end.
                if (NextSibling != null)
                    RemoveChildRange(NextSibling, startSection.Body.LastChild);

                // Clear end section content before range end.
                if (rangeEnd.PreviousSibling != null)
                    RemoveChildRange(endSection.Body.FirstChild, rangeEnd.PreviousSibling);

                // Move remaining content from first section to end section.
                while (startSection.Body.HasChildNodes)
                    endSection.Body.InsertBefore(startSection.Body.FirstChild, rangeEnd);

                // Remove sections between range nodes including starting section.
                RemoveChildRange(startSection, endSection.PreviousSibling);
            }
        }

        /// <summary>
        /// Removes this range start and appropriate range end nodes of the structured document tag,
        /// but keeps its content inside the document tree.
        /// </summary>
        public void RemoveSelfOnly()
        {
            if (RangeEnd != null)
                RangeEnd.Remove();
            Remove();
        }

        internal void SetId(int id)
        {
            InternalSdt.SetId(id);
        }

        /// <summary>
        /// Accepts StructuredDocumentTagRangeStart and all its pseudo child nodes.
        /// </summary>
        internal bool AcceptAsCompositeNode(DocumentVisitor visitor)
        {
            if (!AcceptPseudoChildren(visitor))
                return false;

            return VisitorActionToBool(visitor.VisitStructuredDocumentTagRangeStart(this));
        }

        /// <summary>
        /// Encapsulated StructuredDocumentTag.
        /// </summary>
        /// <remarks>
        /// Customer not going to access this. Instead they should work with structured document tag facade provided by this class.
        /// </remarks>
        internal StructuredDocumentTag InternalSdt
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get
            {
                // Ensure that encapsulated node belongs to the same document as we can get another document
                // after some manipulation, for example, node cloned and appended to another document.
                // We need to consider making full implementation later instead of SDT encapsulation.
                if(mInternalSdt != null)
                    mInternalSdt.SetDocument(Document);

                return mInternalSdt;
            }

            set
            {
                mInternalSdt = value;
            }
        }

        /// <summary>
        /// Removes range between nodes including reference nodes itself.
        /// </summary>
        private static void RemoveChildRange(Node first, Node last)
        {
            Debug.Assert(ReferenceEquals(first.ParentNode, last.ParentNode));

            CompositeNode parent = first.ParentNode;

            int startIndex = parent.IndexOf(first);
            int count = parent.IndexOf(last) - startIndex + 1;

            for(int i = 0; i < count; i++)
                parent.GetChildNodes(NodeType.Any, false).RemoveAt(startIndex);
        }

        /// <summary>
        /// Accepts pseudo child nodes for StructuredDocumentTagRangeStart.
        /// </summary>
        private bool AcceptPseudoChildren(DocumentVisitor visitor)
        {
            foreach (Node node in this)
            {
                // Skip the nested StructuredDocumentTag ranges to avoid double iteration of their children.
                if ((node.NodeType == NodeType.StructuredDocumentTagRangeStart) ||
                    (node.NodeType == NodeType.StructuredDocumentTagRangeEnd))
                    continue;

                if (!node.Accept(visitor))
                    return false;
            }

            return true;
        }

        private StructuredDocumentTag mInternalSdt;

        /// <summary>
        /// Implements node matching for ranged SDT range.
        /// </summary>
        /// <dev>
        /// The implementation is straight and not very effective. Will try to improve if customer complained.
        /// </dev>
        private class StructuredDocumentTagRangeMatcher : NodeMatcher
        {
            internal StructuredDocumentTagRangeMatcher(NodeType nodeType, StructuredDocumentTagRangeStart start, bool isDeep)
            {
                mRangeStart = start;
                mNodeType = nodeType;
                mIsDeep = isDeep;
                mDoc = start.Document;

                Invalidate();
            }

            internal override bool IsMatch(Node node)
            {
                if ((mNodeType != NodeType.Any) && (node.NodeType != mNodeType))
                    return false;

                CheckInvalidate();

                return mRangeNodes.Contains(node);
            }

            internal override bool IsSkipMarkupNodes
            {
                get { return false; }
            }

            private bool RangeNodeListLocked
            {
                get { return mOpenedRangedBlocks.Count > 0; }
            }

            private void CheckInvalidate()
            {
                if (mInitialChangeCount != mDoc.TreeChangeCount)
                    Invalidate();
            }

            private void OpenRangedBlock(Node rangedBlock)
            {
                mOpenedRangedBlocks.Add(((StructuredDocumentTagRangeStart)rangedBlock).Id);
            }

            private void CloseRangedBlock(Node rangedBlock)
            {
                mOpenedRangedBlocks.Remove(((StructuredDocumentTagRangeEnd)rangedBlock).Id);
            }

            private void Invalidate()
            {
                mInitialChangeCount = mDoc.TreeChangeCount;

                mRangeNodes.Clear();
                mOpenedRangedBlocks.Clear();

                // Collect range nodes.
                Node rangeEnd = mRangeStart.RangeEnd;
                Node curNode = mRangeStart;
                while (true)
                {
                    curNode = curNode.NextPreOrder(mDoc);

                    // Nothing to enumerate or reached end of range node.
                    if ((curNode == null) || (curNode == rangeEnd))
                        break;

                    // Do not enumerate Body or Section nodes.
                    if ((curNode.NodeType == NodeType.Body) || (curNode.NodeType == NodeType.Section))
                        continue;

                    if (!mIsDeep && curNode.ParentNode.NodeType != NodeType.Body)
                        continue;

                    if (!RangeNodeListLocked)
                        mRangeNodes.Add(curNode);

                    if (mIsDeep)
                        continue;

                    // We count the nested ranged blocks here. Their content shall not be taken into account by the
                    // matcher if the isDeep flag is false.
                    if (curNode.NodeType == NodeType.StructuredDocumentTagRangeStart)
                        OpenRangedBlock(curNode);
                    else if (curNode.NodeType == NodeType.StructuredDocumentTagRangeEnd)
                        CloseRangedBlock(curNode);
                }
            }

            private readonly HashSetGeneric<int> mOpenedRangedBlocks = new HashSetGeneric<int>();
            private readonly List<Node> mRangeNodes = new List<Node>();
            private readonly StructuredDocumentTagRangeStart mRangeStart;
            private readonly DocumentBase mDoc;
            private readonly NodeType mNodeType;
            private readonly bool mIsDeep;
            private int mInitialChangeCount;
        }

        /// <summary>
        /// Helper class to locate range start/end nodes for ranged structured document tags.
        /// </summary>
        private class RangeNodeFinder : NodeFinder
        {
            private RangeNodeFinder(NodeRange range, int id) : base(range, NodeType.StructuredDocumentTagRangeEnd)
            {
                mId = id;
            }

            internal static StructuredDocumentTagRangeEnd FindRangeEnd(NodeRange range, int id)
            {
                IList<Node> nodes = new RangeNodeFinder(range, id).Find();

                // Do strict check: only ONE range end can be present.
                return (nodes.Count == 1) ? (StructuredDocumentTagRangeEnd)nodes[0] : null;
            }

            protected override bool OnNodeFinding()
            {
                return ((StructuredDocumentTagRangeEnd)CurrentNode).Id == mId;
            }

            private readonly int mId;
        }

        /// <summary>
        /// Provides for-each iteration over StructuredDocumentTag range nodes.
        /// </summary>
        private sealed class StructuredDocumentTagRangeEnumerator : IEnumerator<Node>
        {
            internal StructuredDocumentTagRangeEnumerator(StructuredDocumentTagRangeStart start)
            {
                mRangeStart = start;
                mDoc = start.Document;

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
                if (mRangeStart.RangeEnd == null)
                    return false;

                if (mCurNode == null)
                    mCurNode = mRangeStart;

                while (true)
                {
                    mCurNode = mCurNode.NextPreOrder(mDoc);

                    if (mCurNode == null)
                        break;

                    if ((mCurNode.NodeType != NodeType.Body) &&
                        (mCurNode.NodeType != NodeType.Section) &&
                        (mCurNode.ParentNode.NodeType == NodeType.Body))
                        break;
                }
                if (mCurNode == mRangeStart.RangeEnd)
                    mCurNode = null;

                return (mCurNode != null);
            }

            public void Reset()
            {
                mCurNode = null;
            }

            private Node mCurNode;
            private readonly StructuredDocumentTagRangeStart mRangeStart;
            private readonly DocumentBase mDoc;
        }
    }
}
