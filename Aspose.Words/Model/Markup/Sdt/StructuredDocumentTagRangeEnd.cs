// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 20/08/2019 by Alexey Morozov

namespace Aspose.Words.Markup
{
    /// <summary>
    /// Represents an end of <b>ranged</b> structured document tag which accepts multi-sections content.
    /// See also <see cref="StructuredDocumentTagRangeStart"/> node.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-content-control-sdt/">Structured Document Tags or Content Control</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// Can be immediate child of <see cref="Body" /> node <b>only</b>.
    /// </remarks>
    public class StructuredDocumentTagRangeEnd : Node
    {
        /// <summary>
        /// Initializes a new instance of the <b>Structured document tag range end</b> class.
        /// </summary>
        /// <param name="doc">The owner document.</param>
        /// <param name="id">Identifier of the corresponding structured document tag range start.</param> 
        public StructuredDocumentTagRangeEnd(DocumentBase doc, int id) 
            : base(doc)
        {
            Id = id;
        }

        /// <summary>
        /// Returns <see cref="NodeType.StructuredDocumentTagRangeEnd"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.StructuredDocumentTagRangeEnd; }
        }

        /// <include file='..\..\Docs\Text.xml' path='Topics/Topic[@name="Node.Accept"]/*'/>
        public override bool Accept(DocumentVisitor visitor)
        {
            return VisitorActionToBool(visitor.VisitStructuredDocumentTagRangeEnd(this));
        }

        /// <summary>
        /// Specifies a unique read-only persistent numerical Id for this <b>StructuredDocumentTagRange</b> node.
        /// Corresponding <see cref="StructuredDocumentTagRangeStart"/> node has the same <see cref="StructuredDocumentTagRangeStart.Id"/>.
        /// </summary>
        public int Id { get; private set; }

        internal void SetIdInternal(int id)
        {
            Id = id;
        }
    }
}
