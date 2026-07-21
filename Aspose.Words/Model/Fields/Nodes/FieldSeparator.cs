// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2005 by Roman Korchagin

using Aspose.Words.Drawing.Core;

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents a Word field separator that separates the field code from the field result.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p><see cref="FieldSeparator"/> is an inline-level node and represented
    /// by the <see cref="ControlChar.FieldSeparatorChar"/> control character in the document.</p>
    /// 
    /// <p><see cref="FieldSeparator"/> can only be a child of <see cref="Paragraph"/>.</p>
    /// 
    /// <include file='..\..\Docs\Text.xml' path='Topics/Topic[@name="Field.Common"]/*'/>
    /// </remarks>
    public class FieldSeparator : FieldChar
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        internal FieldSeparator(DocumentBase doc, RunPr runPr, FieldType type) :
            base(doc, ControlChar.FieldSeparatorChar, runPr, type)
        {
            OleObject = null;  // To satisfy Java final.
        }

        /// <summary>
        /// Initializes a new instance of this class with associated OleObject.
        /// 
        /// </summary>
        internal FieldSeparator(DocumentBase doc, RunPr runPr, FieldType type, OleObject oleObject) :
            base(doc, ControlChar.FieldSeparatorChar, runPr, type)
        {
            OleObject = oleObject;
        }

        /// <summary>
        /// Returns <see cref="Aspose.Words.NodeType.FieldSeparator"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.FieldSeparator; }
        }

        /// <summary>
        /// Data of a linked OLE object.
        /// This is only set for LINK fields that link data using text, rtf or HTML format.
        /// All other OLE objects are represented in the model as a Shape object.
        /// 
        /// It is interesting to note that it does not really store the actual data of an OLE object,
        /// it stores some "proxy" or whatever and in theory, MS Word can regenerate this if we don't write it.
        /// 
        /// MS Word WordML does not store this.
        /// MS Word DOC stores this.
        /// MS Word RTF probably needs it because it stores some data.
        /// 
        /// It would be nice if we can get rid of this, but if I don't write it to DOC,
        /// the user has to do manual update for the object in MS Word before he can work with it.
        /// 
        /// So for the time being, lets keep this in the model.
        /// </summary>
        internal OleObject OleObject { get; }

        /// <summary>
        /// Accepts a visitor.
        /// </summary>
        /// <remarks>
        /// <p>Calls <see cref="DocumentVisitor.VisitFieldSeparator"/>.</p>
        /// <p>For more info see the Visitor design pattern.</p>
        /// </remarks>
        /// <param name="visitor">The visitor that will visit the node.</param>
        /// <returns><b>False</b> if the visitor requested the enumeration to stop.</returns>
        public override bool Accept(DocumentVisitor visitor)
        {
            return VisitorActionToBool(visitor.VisitFieldSeparator(this));
        }
    }
}
