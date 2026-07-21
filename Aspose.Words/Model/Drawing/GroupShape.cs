// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 04/07/2006 by Roman Korchagin

using System;
using Aspose.Words.Drawing.Core;
using Aspose.Words.Drawing.Core.Dml;

namespace Aspose.Words.Drawing
{
    /// <summary>
    /// Represents a group of shapes in a document.
    /// <para>To learn more, visit the <a href="https://docs.aspose.com/words/net/how-to-add-group-shape-into-a-word-document/">How to Add Group Shape into a Word Document</a> documentation article.</para>
    /// </summary>
    /// <remarks>
    /// <p>A <see cref="GroupShape"/> is a composite node and can have <see cref="Shape"/> and
    /// <see cref="GroupShape"/> nodes as children.</p>
    /// 
    /// <p>Each <see cref="GroupShape"/> defines a new coordinate system for its child shapes.
    /// The coordinate system is defined using the <see cref="ShapeBase.CoordSize"/> and 
    /// <see cref="ShapeBase.CoordOrigin"/> properties.</p>
    /// 
    /// <seealso cref="ShapeBase"/>
    /// <seealso cref="Shape"/>
    /// </remarks>
    public class GroupShape : ShapeBase
    {
        /// <summary>
        /// Creates a new group shape.
        /// </summary>
        /// <param name="doc">The owner document.</param>
        /// <remarks>
        /// <p>By default, the shape is floating and has default location and size.</p>
        /// <p>You should specify desired shape properties after you created a shape.</p>
        /// </remarks>
        public GroupShape(DocumentBase doc)
            : this(doc, ShapeMarkupLanguage.Vml)
        {
        }

        /// <summary>
        /// Creates a new group shape.
        /// </summary>
        /// <param name="doc">The owner document.</param>
        /// <param name="markupLanguage">Shape markup language: DrawingML or Vml</param>
        /// <remarks>
        /// <p>By default, the shape is floating and has default location and size.</p>
        /// <p>You should specify desired shape properties after you created a shape.</p>
        /// </remarks>
        internal GroupShape(DocumentBase doc, ShapeMarkupLanguage markupLanguage)
            : base(doc, markupLanguage)
        {
            SetShapeType(ShapeType.Group);
        }

        /// <include file='..\Docs\Text.xml' path='Topics/Topic[@name="Node.Accept"]/*'/>
        /// <remarks>
        /// Calls <see cref="DocumentVisitor.VisitGroupShapeStart"/>, then calls <see cref="Node.Accept"/> for all 
        /// child shapes of this group shape and calls <see cref="DocumentVisitor.VisitGroupShapeEnd"/> at the end.
        /// </remarks>
        public override bool Accept(DocumentVisitor visitor)
        {
            return AcceptCore(visitor);
        }

        /// <summary>
        /// Accepts a visitor for visiting the start of the GroupShape.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptStart(DocumentVisitor visitor)
        {
            return visitor.VisitGroupShapeStart(this);
        }

        /// <summary>
        /// Accepts a visitor for visiting the end of the GroupShape.
        /// </summary>
        /// <param name="visitor">The document visitor.</param>
        /// <returns>The action to be taken by the visitor.</returns>
        public override VisitorAction AcceptEnd(DocumentVisitor visitor)
        {
            return visitor.VisitGroupShapeEnd(this);
        }

        /// <summary>
        /// Can only insert other shapes and group shapes into this node.
        /// </summary>
        internal override bool CanInsert(Node newChild)
        {
            Debug.Assert(newChild != null);

            ShapeBase newChildShape = newChild as ShapeBase;

            if (newChildShape == null)
                return false;

            if (MarkupLanguage != newChildShape.MarkupLanguage)
            {
                throw new ArgumentException(
                    "Cannot insert a Shape into a GroupShape with a different markup language"
                    );
            }

            if (MarkupLanguage == ShapeMarkupLanguage.Vml)
                return CanInsertBase(newChild);

            DmlNodeType nodeType = DmlNode.DmlNodeType;
            DmlNodeType newNodeType = newChildShape.DmlNode.DmlNodeType;

            if ((nodeType != DmlNodeType.GraphicFrame) && (newNodeType != DmlNodeType.GraphicFrame))
                return CanInsertBase(newChild);

            // According to spec graphic frame can be child element of the group or canvas.
            // But actually MSW does not allow insert graphic frame into canvas.
            // Chart can contain graphic frame among user shapes. Although DML chart is initialized
            // as DML node of the Shape type and DML node property has not any restrictions,
            // so for group shape DML node can be assigned with "Chart" type.
            if (newNodeType == DmlNodeType.GraphicFrame)
                return (nodeType == DmlNodeType.WordprocessingGroupShape) ||
                       (nodeType == DmlNodeType.GroupShape) ||
                       (nodeType == DmlNodeType.LockedCanvas) ||
                       (nodeType == DmlNodeType.Chart) ||
                       (nodeType == DmlNodeType.ChartEx);

            // Generally content of the graphic frame expected object which is serialized out as xml.
            return (newNodeType == DmlNodeType.Chart) || (newNodeType == DmlNodeType.ChartEx) ||
                (newNodeType == DmlNodeType.Diagram);                       
        }

        /// <summary>
        /// Checks ability to insert node into group shape for all types of nodes
        /// exclude graphic frame shape.
        /// </summary>
        /// <param name="newChild">Node to insert.</param>
        /// <returns>True, when specified node can be inserted.</returns>
        private static bool CanInsertBase(Node newChild)
        {
            Debug.Assert(newChild != null);

            switch (newChild.NodeType)
            {
                case NodeType.GroupShape:
                case NodeType.Shape:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Returns <see cref="Aspose.Words.NodeType.GroupShape"/>.
        /// </summary>
        public override NodeType NodeType
        {
            get { return NodeType.GroupShape; }
        }

        /// <summary>
        /// Specifies how the group shape should be edited in Microsoft Word.
        /// </summary>
        internal EditAs EditAs
        {
            get { return (EditAs)FetchShapeAttrInternal(ShapeAttr.EditAs); }
            set { SetShapeAttrInternal(ShapeAttr.EditAs, value); }
        }

        internal override bool IsPercentWidthInapplicable
        {
            get { return IsInline && (MarkupLanguage != ShapeMarkupLanguage.Vml); }
        }
    }
}
