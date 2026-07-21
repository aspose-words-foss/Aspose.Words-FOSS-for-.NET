// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 18/01/2008 by Roman Korchagin

using System;
using Aspose.JavaAttributes;
using Aspose.Words.Fields;
using Aspose.Words.Notes;

namespace Aspose.Words.Markup
{
    /// <summary>
    /// This element specifies the presence of a smart tag around one or more inline structures
    /// (runs, images, fields,etc.) within a paragraph.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-content-control-sdt/">Structured Document Tags or Content Control</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <para>Smart tags is a kind of custom XML markup. Smart tags provide a facility for embedding
    /// customer-defined semantics into the document via the ability to provide a basic namespace/name
    /// for a run or set of runs within a document.</para>
    ///
    /// <p><see cref="SmartTag"/> can be a child of a <see cref="Paragraph"/> or
    /// another <see cref="SmartTag"/> node.</p>
    ///
    /// <p>The complete list of child nodes that can occur inside a smart tag consists of
    /// <see cref="BookmarkStart"/>, <see cref="BookmarkEnd"/>,
    /// <see cref="FieldStart"/>, <see cref="FieldSeparator"/>, <see cref="FieldEnd"/>, <see cref="FormField"/>,
    /// <see cref="Comment"/>, <see cref="Footnote"/>,
    /// <see cref="Run"/>, <see cref="SpecialChar"/>,
    /// <see cref="Aspose.Words.Drawing.Shape"/>, <see cref="Aspose.Words.Drawing.GroupShape"/>,
    /// <see cref="CommentRangeStart"/>,
    /// <see cref="CommentRangeEnd"/>,
    /// <see cref="SmartTag"/>.</p>
    /// </remarks>
    [JavaGenericArguments("CompositeNode<Node>")]
    public class SmartTag : CompositeNode, IMarkupNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SmartTag"/> class.
        /// </summary>
        /// <remarks>
        /// <para>When you create a new node, you need to specify a document to which the node belongs.
        /// A node cannot exist without a document because it depends on the document-wide structures
        /// such as lists and styles. Although a node always belongs to a document, a node might or might
        /// not be a part of the document tree.</para>
        ///
        /// <para>When a node is created, it belongs to a document, but is not yet part of the document tree
        /// and <see cref="Node.ParentNode"/> is null. To insert a node into the document, use the
        /// <see cref="CompositeNode.InsertAfter{T}(T, Node)"/> or <see cref="CompositeNode.InsertBefore{T}(T, Node)"/>
        /// methods on the parent node.</para>
        /// </remarks>
        /// <param name="doc">The owner document.</param>
        public SmartTag(DocumentBase doc) : base(doc)
        {
            mProperties = new CustomXmlPropertyCollection();
        }

        /// <summary>
        /// Returns <see cref="NodeType.SmartTag"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.SmartTag; }
        }

        /// <summary>
        /// Swaps all data (excluding child nodes) of this smart tag with another smart tag.
        /// </summary>
        internal void Swap(SmartTag rhs)
        {
            string tempElement = mElement;
            mElement = rhs.mElement;
            rhs.mElement = tempElement;

            string tempUri = mUri;
            mUri = rhs.mUri;
            rhs.mUri = tempUri;

            CustomXmlPropertyCollection tempProperties = mProperties;
            mProperties = rhs.mProperties;
            rhs.mProperties = tempProperties;
        }

        internal override Node Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            SmartTag lhs = (SmartTag)base.Clone(isCloneChildren, cloningListener);
            lhs.mProperties = mProperties.Clone();

            // Don't need to clone name and uri because they are strings (immutable).

            return lhs;
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Node.Accept"]/*'/>
        /// <remarks>
        /// Calls <see cref="DocumentVisitor.VisitSmartTagStart"/>, then calls <see cref="Node.Accept"/> for all
        /// child nodes of the smart tag and calls <see cref="DocumentVisitor.VisitSmartTagEnd"/> at the end.
        /// </remarks>
        public override bool Accept(DocumentVisitor visitor)
        {
            return AcceptCore(visitor);
        }

        /// <summary>
        /// Accepts a visitor for visiting the start of the SmartTag.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptStart(DocumentVisitor visitor)
        {
            return visitor.VisitSmartTagStart(this);
        }

        /// <summary>
        /// Accepts a visitor for visiting the end of the SmartTag.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptEnd(DocumentVisitor visitor)
        {
            return visitor.VisitSmartTagEnd(this);
        }

        /// <summary>
        /// Allows to insert inline-level elements or bookmarks.
        /// </summary>
        internal override bool CanInsert(Node newChild)
        {
            return NodeUtil.IsInlineLevelNode(newChild);
        }

        /// <summary>
        /// Specifies the name of the smart tag within the document.
        /// </summary>
        /// <remarks>
        /// <para>Cannot be <c>null</c>.</para>
        /// <para>Default is empty string.</para>
        /// </remarks>
        public string Element
        {
            [CodePorting.Translator.Cs2Cpp.CppConstMethod]
            get { return mElement; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "Element");
                mElement = value;
            }
        }

        /// <summary>
        /// Specifies the namespace URI of the smart tag.
        /// </summary>
        /// <remarks>
        /// <para>Cannot be <c>null</c>.</para>
        /// <para>Default is empty string.</para>
        /// </remarks>
        public string Uri
        {
            get { return mUri; }
            set
            {
                ArgumentUtil.CheckNotNull(value, "Uri");
                mUri = value;
            }
        }

        /// <summary>
        /// A collection of the smart tag properties.
        /// </summary>
        /// <remarks>
        /// <para>Cannot be <c>null</c>.</para>
        /// </remarks>
        public CustomXmlPropertyCollection Properties
        {
            get { return mProperties; }
        }

        MarkupLevel IMarkupNode.Level_IMarkupNode
        {
            get { return MarkupLevel.Inline; }
        }

#if DEBUG
        public override string ToString()
        {
            return String.Format("{0} {1}", base.ToString(), Element);
        }
#endif

        private string mElement = "";
        private string mUri = "";
        private CustomXmlPropertyCollection mProperties;

    }
}
