// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2010 by Alexey Titov

using System;
using Aspose.Words.Drawing.Core.Dml.Outlines;
using Aspose.Words.Drawing.Core.Dml.Styles;
using Aspose.Words.Drawing.Core.Dml.Text;

namespace Aspose.Words.Drawing.Core.Dml
{
    /// <summary>
    /// 20.1.2.2.20 grpSp (Group shape)
    /// This element specifies a group shape that represents many shapes grouped together.
    /// This shape is to be treated just as if it were a regular shape but instead of
    /// being described by a single geometry it is made up of all the shape geometries
    /// encompassed within it. Within a group shape each of the shapes that make up the
    /// group are specified just as they normally would. The idea behind grouping elements
    /// however is that a single transform can apply to many shapes at the same time.
    /// </summary>
    internal class DmlGroupShape : DmlCompositeNode, IDmlCommonShapePrSource
    {
        internal DmlGroupShape(DmlNodeType groupShapeType)
        {
            if (groupShapeType != DmlNodeType.GroupShape
                && groupShapeType != DmlNodeType.WordprocessingGroupShape
                && groupShapeType != DmlNodeType.SpTree
                && groupShapeType != DmlNodeType.GraphicFrame)
                throw new ArgumentException("Unexpected group shape type.");

            mGroupShapeType = groupShapeType;
        }

        internal override DmlNode Clone(bool isCloneChildren, INodeCloningListener cloningListener)
        {
            DmlGroupShape lhs = (DmlGroupShape)base.Clone(isCloneChildren, cloningListener);

            if (mTextShape != null)
                lhs.mTextShape = mTextShape.Clone();

            return lhs;
        }

        internal override DmlNodeType DmlNodeType
        {
            get { return mGroupShapeType; }
        }

        internal DmlTextShape TextShape
        {
            get { return mTextShape; }
            set { mTextShape = value; }
        }

        internal BWMode BWMode
        {
            get { return mDmlBWMode; }
            set { mDmlBWMode = value; }
        }

        internal bool HasParentLockedCanvas(DmlGroupShape node)
        {
            if ((node.Parent == null) || (node.Parent.NodeType != NodeType.GroupShape))
                return false;

            DmlNode parentDmlNode = ((GroupShape)node.Parent).DmlNode;
            if (parentDmlNode == null)
                return false;

            if (parentDmlNode.DmlNodeType == DmlNodeType.LockedCanvas)
                return true;

            if (parentDmlNode.DmlNodeType == DmlNodeType.GroupShape)
                return HasParentLockedCanvas((DmlGroupShape)parentDmlNode);

            return false;
        }

        public DmlOutline Outline
        {
            get { return mOutline; }
            set { mOutline = value; }
        }

        DmlShapeStyle IDmlCommonShapePrSource.Style
        {
            get { return mStyle; }
            set { mStyle = value; }
        }

        internal bool IsParentLockedCanvas
        {
            get { return HasParentLockedCanvas(this); }
        }

        private DmlOutline mOutline;
        private DmlShapeStyle mStyle;
        private DmlTextShape mTextShape;
        private readonly DmlNodeType mGroupShapeType;
        private BWMode mDmlBWMode = (BWMode)255; // Set special value to avoid BWMode writing.
    }
}
