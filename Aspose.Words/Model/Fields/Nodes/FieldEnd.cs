// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2005 by Roman Korchagin

using Aspose.Words.Revisions;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents an end of a Word field in a document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p><see cref="FieldEnd"/> is an inline-level node and represented
    /// by the <see cref="ControlChar.FieldEndChar"/> control character in the document.</p>
    ///
    /// <p><see cref="FieldEnd"/> can only be a child of <see cref="Paragraph"/>.</p>
    ///
    /// <include file='..\..\Docs\Text.xml' path='Topics/Topic[@name="Field.Common"]/*'/>
    /// </remarks>
    public class FieldEnd : FieldChar
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="doc">The owner document.</param>
        /// <param name="runPr">Run attributes of the field character.</param>
        /// <param name="type">The type of the field.</param>
        /// <param name="hasSeparator">Indicates whether this field has a field separator or not.
        /// This MUST be set properly, otherwise some code might give exceptions.</param>
        internal FieldEnd(DocumentBase doc, RunPr runPr, FieldType type, bool hasSeparator) :
            base(doc, ControlChar.FieldEndChar, runPr, type)
        {
            HasSeparator = hasSeparator;
        }

        /// <summary>
        /// Returns <see cref="Aspose.Words.NodeType.FieldEnd"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.FieldEnd; }
        }

        /// <summary>
        /// Returns <c>true</c> if this field has a separator.
        /// </summary>
        public bool HasSeparator { get; private set; }

        internal FieldNumberRevision NumberRevision
        {
            get { return RunPr.NumberRevision; }
            set { RunPr.NumberRevision = value; }
        }

        internal void SetHasSeparator(bool value)
        {
            HasSeparator = value;
        }

        /// <summary>
        /// Accepts a visitor.
        /// </summary>
        /// <remarks>
        /// <p>Calls <see cref="DocumentVisitor.VisitFieldEnd"/>.</p>
        /// <p>For more info see the Visitor design pattern.</p>
        /// </remarks>
        /// <param name="visitor">The visitor that will visit the node.</param>
        /// <returns><b>False</b> if the visitor requested the enumeration to stop.</returns>
        public override bool Accept(DocumentVisitor visitor)
        {
            return VisitorActionToBool(visitor.VisitFieldEnd(this));
        }
    }
}
