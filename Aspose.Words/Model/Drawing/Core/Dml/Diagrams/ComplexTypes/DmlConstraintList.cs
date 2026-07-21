// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2013 by Konstantin Kornilov

using System.Collections.Generic;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.2.9 constrLst (Constraint List)
    /// </summary>
    internal class DmlConstraintList : DmlDiagramLayoutNodeContentItem
    {
        internal override void Accept(IDmlDiagramLayoutNodeContentItemVisitor visitor)
        {
            visitor.VisitConstraintList(this);
        }

        internal override DmlDiagramLayoutNodeContentItemType ContentItemType
        {
            get { return DmlDiagramLayoutNodeContentItemType.ConstraintList; }
        }

        internal List<DmlConstraint> Values
        {
            get { return mValues; }
            set { mValues = value; }
        }

        public static DmlConstraintList Empty { get; }

        static DmlConstraintList()
        {
            Empty = new DmlConstraintList();
            Empty.Values = new List<DmlConstraint>();
        }

        internal override DmlDiagramLayoutNodeContentItem DeepCopy()
        {
            DmlConstraintList copy = (DmlConstraintList)MemberwiseClone();
            
            copy.Values = new List<DmlConstraint>();
            foreach (DmlConstraint constraint in Values)
                copy.Values.Add(constraint);

            return copy;
        }

        private List<DmlConstraint> mValues;
    }
}
