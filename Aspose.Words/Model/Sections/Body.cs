// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2005 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Represents a container for the main text of a section.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/aspose-words-document-object-model/">Aspose.Words Document Object Model (DOM)</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p><see cref="Body"/> can contain <see cref="Paragraph"/> and <see cref="Tables.Table">Table</see> child nodes.</p>
    /// <p><see cref="Body"/> is a section-level node and can only be a child of <see cref="Section"/>. 
    /// There can only be one <see cref="Body"/> in a <see cref="Section"/>.</p>
    /// <p>A minimal valid <see cref="Body"/> needs to contain at least one <see cref="Paragraph"/>.</p>
    /// </remarks>
    public class Body : Story
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Body"/> class.
        /// </summary>
        /// <remarks>
        /// <p>When <see cref="Body"/> is created, it belongs to the specified document, but is not 
        /// yet part of the document and <see cref="Node.ParentNode"/> is <c>null</c>.</p>
        /// <p>To append <see cref="Body"/> to a <see cref="Section"/> use <see cref="CompositeNode.AppendChild">AppendChild</see>
        /// <see cref="CompositeNode.InsertAfter{T}(T, Node)">InsertAfter</see> or <see cref="CompositeNode.InsertBefore{T}(T, Node)">InsertBefore</see>
        /// methods.</p>
        /// </remarks>
        /// <param name="doc">The owner document.</param>
        public Body(DocumentBase doc) : base(doc, StoryType.MainText)
        {
        }

        /// <summary>
        /// Returns <see cref="NodeType.Body"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.Body; }
        }

        /// <summary>
        /// Gets the parent section of this story.
        /// </summary>
        /// <remarks>
        /// <p><see cref="ParentSection"/> is equivalent to <see cref="Node.ParentNode"/> casted to <see cref="Section"/>.</p>
        /// </remarks>
        public Section ParentSection
        {
            get { return (Section)ParentNode; }
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Node.Accept"]/*'/>
        /// <remarks>
        /// Calls <see cref="DocumentVisitor.VisitBodyStart"/>, then calls <see cref="Node.Accept"/> for all child nodes of the section
        /// and calls <see cref="DocumentVisitor.VisitBodyEnd"/> at the end.
        /// </remarks>
        public override bool Accept(DocumentVisitor visitor)
        {
            return AcceptCore(visitor);
        }

        /// <summary>
        /// StructuredDocumentTag range nodes additionally can be inserted into Body.
        /// </summary>
        internal override bool CanInsert(Node newChild)
        {
            if ((newChild.NodeType == NodeType.StructuredDocumentTagRangeStart) ||
                (newChild.NodeType == NodeType.StructuredDocumentTagRangeEnd))
                return true;

            return base.CanInsert(newChild);
        }

        /// <summary>
        /// Accepts a visitor for visiting the start of the document's body.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptStart(DocumentVisitor visitor)
        {
            return visitor.VisitBodyStart(this);
        }

        /// <summary>
        /// Accepts a visitor for visiting the end of the document's body.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptEnd(DocumentVisitor visitor)
        {
            return visitor.VisitBodyEnd(this);
        }

        /// <summary>
        /// If the last child is not a paragraph, creates and appends one empty paragraph.
        /// </summary>
        public void EnsureMinimum()
        {
            // WORDSNET-897 A table cannot be a last element in a body.
            WordUtil.EnsureNonEmptyStory(this);
        }
    }
}
