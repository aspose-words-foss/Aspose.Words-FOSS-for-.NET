// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.2.21 presOf (Presentation Of)
    /// </summary>
    internal class DmlPresentationOf : DmlDiagramLayoutNodeContentItem
    {
        internal override void Accept(IDmlDiagramLayoutNodeContentItemVisitor visitor)
        {
            visitor.VisitPresentationOf(this);
        }

        internal override DmlDiagramLayoutNodeContentItemType ContentItemType
        {
            get { return DmlDiagramLayoutNodeContentItemType.PresentationOf; }
        }

        internal DmlIteratorAttributes IteratorAttributes
        {
            get { return mIteratorAttributes; }
        }

        private readonly DmlIteratorAttributes mIteratorAttributes = new DmlIteratorAttributes();
    }
}
