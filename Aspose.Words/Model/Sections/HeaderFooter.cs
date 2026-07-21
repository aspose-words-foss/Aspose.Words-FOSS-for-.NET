// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2005 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Represents a container for the header or footer text of a section.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-headers-and-footers/">Working with Headers and Footers</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p><see cref="HeaderFooter"/> can contain <see cref="Paragraph"/> and <see cref="Tables.Table">Table</see> child nodes.</p>
    ///
    /// <p><see cref="HeaderFooter"/> is a section-level node and can only be a child of <see cref="Section"/>.
    /// There can only be one <see cref="HeaderFooter"/> of each <see cref="HeaderFooterType"/> in a <see cref="Section"/>.</p>
    ///
    /// <p>If <see cref="Section"/> does not have a <see cref="HeaderFooter"/> of a specific type or
    /// the <see cref="HeaderFooter"/> has no child nodes, this header/footer is considered linked to
    /// the header/footer of the same type of the previous section in Microsoft Word.</p>
    ///
    /// <p>When <see cref="HeaderFooter"/> contains at least one <see cref="Paragraph"/>, it is no longer
    /// considered linked to previous in Microsoft Word.</p>
    ///
    /// </remarks>
    public class HeaderFooter : Story
    {
        /// <summary>
        /// Creates a new header or footer of the specified type.
        /// </summary>
        /// <remarks>
        /// <p>When <see cref="HeaderFooter"/> is created, it belongs to the specified document, but is not
        /// yet part of the document and <see cref="Node.ParentNode"/> is <c>null</c>.</p>
        /// <p>To append <see cref="HeaderFooter"/> to a <see cref="Section"/> use <see cref="CompositeNode.InsertAfter{T}(T, Node)"/>, <see cref="CompositeNode.InsertBefore{T}(T, Node)"/>,
        /// or <see cref="Section.HeadersFooters"/> property and methods <see cref="NodeCollection.Add"/>, <see cref="NodeCollection.Insert"/>.</p>
        /// </remarks>
        /// <param name="doc">The owner document.</param>
        /// <param name="headerFooterType">A <see cref="HeaderFooterType"/> value
        /// that specifies the type of the header or footer.</param>
        public HeaderFooter(DocumentBase doc, HeaderFooterType headerFooterType) :
            base(doc, WordUtil.HeaderFooterTypeToStoryType(headerFooterType))
        {
        }

        /// <summary>
        /// Returns <see cref="NodeType.HeaderFooter"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.HeaderFooter; }
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

        /// <summary>
        /// Gets the type of this header/footer.
        /// </summary>
        public HeaderFooterType HeaderFooterType
        {
            get { return WordUtil.StoryTypeToHeaderFooterType(base.StoryType); }
        }

        /// <summary>
        /// True if this <see cref="HeaderFooter"/> object is a header.
        /// </summary>
        public bool IsHeader
        {
            get
            { return
                  (HeaderFooterType == HeaderFooterType.HeaderPrimary) ||
                  (HeaderFooterType == HeaderFooterType.HeaderFirst) ||
                  (HeaderFooterType == HeaderFooterType.HeaderEven);
            }
        }

        /// <summary>
        /// True if this header or footer is linked to the corresponding header or footer
        /// in the previous section.
        /// </summary>
        /// <remarks>
        /// <p>Default is <c>true</c>.</p>
        /// <p>Note, when your link a header or footer, its contents is cleared.</p>
        /// </remarks>
        public bool IsLinkedToPrevious
        {
            get
            {
                //Header footer is linked to previous when it has no child nodes (or when it does not exist).
                return (!HasChildNodes);
            }
            set
            {
                if (value)
                {
                    //If want to link to previous, just clear this header footer.
                    RemoveAllChildren();
                }
                else
                {
                    //If want to unlink from previous, need to have at least one paragraph.
                    if (!HasChildNodes)
                        AppendChild(new Paragraph(Document));
                }
            }
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Node.Accept"]/*'/>
        /// <remarks>
        /// Calls <see cref="DocumentVisitor.VisitHeaderFooterStart"/>, then calls <see cref="Node.Accept"/> for all child nodes of the section
        /// and calls <see cref="DocumentVisitor.VisitHeaderFooterEnd"/> at the end.
        /// </remarks>
        public override bool Accept(DocumentVisitor visitor)
        {
            return AcceptCore(visitor);
        }

        /// <summary>
        /// Accepts a visitor for visiting the start of the header.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptStart(DocumentVisitor visitor)
        {
            return visitor.VisitHeaderFooterStart(this);
        }

        /// <summary>
        /// Accepts a visitor for visiting the end of the header.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptEnd(DocumentVisitor visitor)
        {
            return visitor.VisitHeaderFooterEnd(this);
        }
    }
}
