// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using System.Collections.Generic;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.2.6 choose (Choose Element)
    /// </summary>
    internal class DmlChoose : DmlDiagramLayoutNodeContentItem
    {
        internal override void Accept(IDmlDiagramLayoutNodeContentItemVisitor visitor)
        {
            visitor.VisitChoose(this);
        }

        internal override DmlDiagramLayoutNodeContentItemType ContentItemType
        {
            get { return DmlDiagramLayoutNodeContentItemType.Choose; }
        }

        internal string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        internal List<DmlWhen> If
        {
            get { return mIf; }
            set { mIf = value; }
        }

        internal DmlOtherwise Else
        {
            get { return mElse; }
            set { mElse = value; }
        }

        internal override DmlDiagramLayoutNodeContentItem DeepCopy()
        {
            DmlChoose copy = (DmlChoose)MemberwiseClone();

            copy.If = new List<DmlWhen>(If.Count);
            foreach (DmlWhen dmlWhen in If)
                copy.If.Add(dmlWhen.DeepCopy());

            if (Else != null)
                copy.Else = Else.DeepCopy();

            return copy;
        }

        private string mName;
        private List<DmlWhen> mIf;
        private DmlOtherwise mElse;
    }
}
