// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/11/2010 by Alexey Titov

using Aspose.Words.Drawing.Core.Dml.Scene3D;
using Aspose.Words.Drawing.Core.Dml.Transforms;

namespace Aspose.Words.Drawing.Core.Dml
{
    /// <summary>
    /// Represents a base class of node in dml tree that can contain other nodes.
    /// </summary>
    internal abstract class DmlCompositeNode : DmlFillableNode
    {
        protected override void InitializeTransform()
        {
            mTransform = new DmlGroupTransform();
        }

        public override bool HasTransparency
        {
            get
            {
                if (base.HasTransparency)
                    return true;

                foreach (ShapeBase childNode in Dml.GetChildNodes(NodeType.Any, false))
                {
                    DmlFillableNode fillable = childNode.DmlNode as DmlFillableNode;

                    if (fillable != null && fillable.HasTransparency)
                        return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Defines optional scene-level 3D properties to apply to an object.
        /// </summary>
        internal DmlScene3DProperties Scene3DProperties
        {
            get { return mScene3DProperties; }
            set { mScene3DProperties = value; }
        }
        
        /// <summary>
        /// Returns original transform set in the document.
        /// Used for writers only. In rendering use <see cref="DmlNode.Transform"/> property.
        /// </summary>
        internal DmlTransform TransformInternal
        {
            get { return mTransform; }
        }

        private DmlScene3DProperties mScene3DProperties;
    }
}