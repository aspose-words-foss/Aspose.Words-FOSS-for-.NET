// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using Aspose.Words.Drawing.Core.Dml.Common;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.2.27 shape (Shape)
    /// </summary>
    internal class DmlDiagramShape : DmlDiagramLayoutNodeContentItem
    {
        internal override void Accept(IDmlDiagramLayoutNodeContentItemVisitor visitor)
        {
            visitor.VisitShape(this);
        }

        internal override DmlDiagramLayoutNodeContentItemType ContentItemType
        {
            get { return DmlDiagramLayoutNodeContentItemType.Shape; }
        }

        internal DmlAngle Rotation
        {
            get { return mRotation; }
            set { mRotation = value; }
        }

        internal string Type
        {
            get { return mType; }
            set { mType = value; }
        }

        internal string BlipReference
        {
            get { return mBlipReference; }
            set { mBlipReference = value; }
        }

        internal byte[] BlipData
        {
            get { return mBlipData; }
            set { mBlipData = value; }
        }

        internal int ZOrder
        {
            get { return mZOrder; }
            set { mZOrder = value; }
        }

        internal bool HideGeometry
        {
            get { return mHideGeometry; }
            set { mHideGeometry = value; }
        }

        internal bool PreventTextEditing
        {
            get { return mPreventTextEditing; }
            set { mPreventTextEditing = value; }
        }

        internal bool ImagePlaceholder
        {
            get { return mImagePlaceholder; }
            set { mImagePlaceholder = value; }
        }

        internal DmlShapeAdjust[] AdjustList
        {
            get { return mAdjustList; }
            set { mAdjustList = value; }
        }

        private DmlAngle mRotation;
        private string mType;
        private int mZOrder;
        private bool mHideGeometry;
        private bool mPreventTextEditing;
        private bool mImagePlaceholder;
        private DmlShapeAdjust[] mAdjustList;
        private string mBlipReference;
        private byte[] mBlipData;
    }
}
