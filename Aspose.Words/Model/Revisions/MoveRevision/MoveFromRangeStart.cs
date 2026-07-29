// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 12/11/2013 by Andrey Noskov

using Aspose.Words.Revisions;

namespace Aspose.Words
{
    /// <summary>
    /// Represents a start of an moveFrom range in a Word document.
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
    internal class MoveFromRangeStart : MoveRangeStart
    {
        internal MoveFromRangeStart(
            DocumentBase doc,
            int id,
            string author,
            string name,
            System.DateTime dateTime,
            DisplacedByType displacedBy)
            : base(doc, id, author, name, dateTime, displacedBy)
        {
        }

        /// <summary>
        /// Returns <see cref="Aspose.Words.NodeType.MoveFromRangeStart"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.MoveFromRangeStart; }
        }

        /// <summary>
        /// Accepts a visitor.
        /// </summary>
        /// <remarks>
        /// <p>Calls <see cref="DocumentVisitor.VisitMoveFromRangeStart"/>.</p>
        /// <p>For more info see the Visitor design pattern.</p>
        /// </remarks>
        /// <param name="visitor">The visitor that will visit the node.</param>
        /// <returns>False if the visitor requested the enumeration to stop.</returns>
        public override bool Accept(DocumentVisitor visitor)
        {
            return VisitorActionToBool(visitor.VisitMoveFromRangeStart(this));
        }
    }
}
