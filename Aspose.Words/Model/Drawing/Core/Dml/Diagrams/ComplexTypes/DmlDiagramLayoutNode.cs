// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml.Diagrams.SimpleTypes;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.2.19 layoutNode (Layout Node)
    /// </summary>
    internal class DmlDiagramLayoutNode : DmlDiagramLayoutNodeContentItem
    {
        internal override void Accept(IDmlDiagramLayoutNodeContentItemVisitor visitor)
        {
            visitor.VisitLayoutNode(this);
        }

        internal override DmlDiagramLayoutNodeContentItemType ContentItemType
        {
            get { return DmlDiagramLayoutNodeContentItemType.LayoutNode; }
        }

        internal string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        internal string StyleLabel
        {
            get { return mStyleLabel; }
            set { mStyleLabel = value; }
        }

        internal DmlChildOrder ChildOrder
        {
            get { return mChildOrder; }
            set { mChildOrder = value; }
        }

        internal string MoveWith
        {
            get { return mMoveWith; }
            set { mMoveWith = value; }
        }

        internal List<DmlDiagramLayoutNodeContentItem> Content
        {
            get { return mContent; }
            set { mContent = value; }
        }

        internal override DmlDiagramLayoutNodeContentItem DeepCopy()
        {
            DmlDiagramLayoutNode copy = (DmlDiagramLayoutNode)MemberwiseClone();
            
            copy.Content = new List<DmlDiagramLayoutNodeContentItem>(Content.Count);
            foreach (DmlDiagramLayoutNodeContentItem item in Content)
                copy.Content.Add(item.DeepCopy());
            
            return copy;
        }

        private string mName;
        private string mStyleLabel;
        private DmlChildOrder mChildOrder = DmlChildOrder.Bottom;
        private string mMoveWith;
        private List<DmlDiagramLayoutNodeContentItem> mContent;
    }
}
