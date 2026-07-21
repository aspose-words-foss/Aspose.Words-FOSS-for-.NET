// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/11/2013 by Andrey Noskov

using Aspose.Words.Revisions;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a end of an moveFrom range in a Word document.
    /// </summary>
    /// <remarks>
    /// <p>A complete moveFrom range in a Word document consists of a <see cref="MoveFromRangeStart"/>
    /// and a matching <see cref="MoveFromRangeEnd"/> with the same Id.</p>
    ///
    /// <p><see cref="MoveFromRangeStart"/> and <see cref="MoveFromRangeEnd"/> are just markers inside a document
    /// that specify where the moveFrom range starts and ends.</p>
    ///
    /// <note>Currently moveFrom ranges are supported only at the inline-level, that is inside <see cref="Paragraph"/>.</note>
    /// </remarks>
    internal class MoveFromRangeEnd : MoveRangeEnd
    {
        internal MoveFromRangeEnd(DocumentBase doc, int id, DisplacedByType displacedBy)
            : base(doc, id, displacedBy)
        {
        }

        /// <summary>
        /// Corresponding MoveFromRangeStart, received by ID.
        /// </summary>
        internal MoveFromRangeStart MoveFromRangeStart
        {
            get { return (MoveFromRangeStart)MoveRangeFinder.FindMoveRangeStart(Document, Id); }
        }

        /// <summary>
        /// Accepts a visitor.
        /// </summary>
        /// <remarks>
        /// <p>Calls <see cref="DocumentVisitor.VisitMoveFromRangeEnd"/>.</p>
        /// <p>For more info see the Visitor design pattern.</p>
        /// </remarks>
        /// <param name="visitor">The visitor that will visit the node.</param>
        /// <returns>False if the visitor requested the enumeration to stop.</returns>
        public override bool Accept(DocumentVisitor visitor)
        {
            return VisitorActionToBool(visitor.VisitMoveFromRangeEnd(this));
        }

        /// <summary>
        /// Returns <see cref="Aspose.Words.NodeType.MoveFromRangeEnd"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.MoveFromRangeEnd; }
        }
    }
}
