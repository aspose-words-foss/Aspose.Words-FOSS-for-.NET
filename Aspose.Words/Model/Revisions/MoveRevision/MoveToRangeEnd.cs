// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/11/2013 by Andrey Noskov

using Aspose.Words.Revisions;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a end of an moveTo range in a Word document.
    /// </summary>
    /// <remarks>
    /// <p>A complete moveTo range in a Word document consists of a <see cref="MoveToRangeStart"/>
    /// and a matching <see cref="MoveToRangeEnd"/> with the same Id.</p>
    ///
    /// <p><see cref="MoveToRangeStart"/> and <see cref="MoveToRangeEnd"/> are just markers inside a document
    /// that specify where the moveTo range starts and ends.</p>
    ///
    /// <note>Currently moveTo ranges are supported only at the inline-level, that is inside <see cref="Paragraph"/>.</note>
    /// </remarks>
    internal class MoveToRangeEnd : MoveRangeEnd
    {
        internal MoveToRangeEnd(DocumentBase doc, int id, DisplacedByType displacedBy)
            : base(doc, id, displacedBy)
        {
        }

        /// <summary>
        /// Corresponding MoveToRangeStart, received by ID.
        /// </summary>
        internal MoveToRangeStart MoveToRangeStart
        {
            get { return (MoveToRangeStart)MoveRangeFinder.FindMoveRangeStart(Document, Id); }
        }

        /// <summary>
        /// Accepts a visitor.
        /// </summary>
        /// <remarks>
        /// <p>Calls <see cref="DocumentVisitor.VisitMoveToRangeEnd"/>.</p>
        /// <p>For more info see the Visitor design pattern.</p>
        /// </remarks>
        /// <param name="visitor">The visitor that will visit the node.</param>
        /// <returns>False if the visitor requested the enumeration to stop.</returns>
        public override bool Accept(DocumentVisitor visitor)
        {
            return VisitorActionToBool(visitor.VisitMoveToRangeEnd(this));
        }

        /// <summary>
        /// Returns <see cref="Aspose.Words.NodeType.MoveToRangeEnd"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.MoveToRangeEnd; }
        }
    }
}
