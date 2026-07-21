// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/07/2005 by Roman Korchagin

namespace Aspose.Words.Fields
{
    /// <summary>
    /// Represents a start of a Word field in a document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/working-with-fields/">Working with Fields</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p><see cref="FieldStart"/> is an inline-level node and represented by the
    /// <see cref="ControlChar.FieldStartChar"/> control character in the document.</p>
    /// 
    /// <p><see cref="FieldStart"/> can only be a child of <see cref="Paragraph"/>.</p>
    /// 
    /// <include file='..\..\Docs\Text.xml' path='Topics/Topic[@name="Field.Common"]/*'/>
    /// </remarks>
    public class FieldStart : FieldChar
    {
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        internal FieldStart(DocumentBase doc, RunPr runPr, FieldType type) :
            base(doc, ControlChar.FieldStartChar, runPr, type)
        {
        }

        /// <summary>
        /// Returns <see cref="Aspose.Words.NodeType.FieldStart"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.FieldStart; }
        }

        /// <summary>
        /// Accepts a visitor.
        /// </summary>
        /// <remarks>
        /// <p>Calls <see cref="DocumentVisitor.VisitFieldStart"/>.</p>
        /// <p>For more info see the Visitor design pattern.</p>
        /// </remarks>
        /// <param name="visitor">The visitor that will visit the node.</param>
        /// <returns><b>False</b> if the visitor requested the enumeration to stop.</returns>
        public override bool Accept(DocumentVisitor visitor)
        {
            return VisitorActionToBool(visitor.VisitFieldStart(this));
        }

        /// <summary>
        /// Gets an extended data corresponding to this field start. Should be used for form fields' starts only.
        /// </summary>
        /// <remarks>
        /// ECMA-376 states that w:ffData element must be a child of w:fldChar element, i.e. its parent is not
        /// necessary to be a field start. But MS Word stores this data only for field starts. Moreover,
        /// manual adding of this data to a field end and document resaving via MS Word removes this data from
        /// the field end. So it makes no sense to expand this property for <see cref="FieldChar"/> class.
        /// </remarks>
        internal FormField FormField
        {
            get
            {
                Debug.Assert(FieldUtil.IsFormField(FieldType));

                return FindFormField();
            }
        }

        private FormField FindFormField()
        {
            int level = 0;
            DocumentPosition position = DocumentPosition.CreatePositionAfter(this);

            while (true)
            {
                if (!position.Move(null, true, true, true, false, false))
                    return null;

                Node node = position.Node;

                switch (node.NodeType)
                {
                    case NodeType.FieldStart:
                        level++;
                        break;

                    case NodeType.FieldSeparator:
                        if (level == 0)
                            return null;

                        break;

                    case NodeType.FieldEnd:
                        if (level == 0)
                            return null;

                        level--;
                        break;

                    case NodeType.FormField:
                        if (level == 0)
                            return (FormField)node;

                        break;

                    default:
                        // Do nothing.
                        break;
                }
            }

        }

        /// <summary>
        /// Gets custom field data which is associated with the field.
        /// </summary>
        public byte[] FieldData { get; internal set; }
    }
}
