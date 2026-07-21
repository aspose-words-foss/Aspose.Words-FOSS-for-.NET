// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using System.Collections.Generic;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.2.14 forEach (For Each)
    /// </summary>
    internal class DmlForEach : DmlDiagramLayoutNodeContentItem
    {
        internal override void Accept(IDmlDiagramLayoutNodeContentItemVisitor visitor)
        {
            visitor.VisitForEach(this);
        }

        internal override DmlDiagramLayoutNodeContentItemType ContentItemType
        {
            get { return DmlDiagramLayoutNodeContentItemType.ForEach; }
        }

        internal string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        internal string Reference
        {
            get { return mReference; }
            set { mReference = value; }
        }

        internal DmlIteratorAttributes IteratorAttributes
        {
            get { return mIteratorAttributes; }
        }

        internal List<DmlDiagramLayoutNodeContentItem> Content
        {
            get { return mContent; }
            set { mContent = value; }
        }

        internal override DmlDiagramLayoutNodeContentItem DeepCopy()
        {
            DmlForEach copy = (DmlForEach)MemberwiseClone();

            copy.Content = new List<DmlDiagramLayoutNodeContentItem>(Content.Count);
            foreach (DmlDiagramLayoutNodeContentItem item in Content)
                copy.Content.Add(item.DeepCopy());
            
            return copy;
        }

        private string mName;
        private string mReference;
        private readonly DmlIteratorAttributes mIteratorAttributes = new DmlIteratorAttributes();
        private List<DmlDiagramLayoutNodeContentItem> mContent;
    }
}
