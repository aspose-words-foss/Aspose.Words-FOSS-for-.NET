// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2013 by Konstantin Kornilov

using System.Collections.Generic;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.2.25 ruleLst (Rule List)
    /// </summary>
    internal class DmlNumericRuleList : DmlDiagramLayoutNodeContentItem
    {
        internal override void Accept(IDmlDiagramLayoutNodeContentItemVisitor visitor)
        {
            visitor.VisitRuleList(this);
        }

        internal override DmlDiagramLayoutNodeContentItemType ContentItemType
        {
            get { return DmlDiagramLayoutNodeContentItemType.NumericRuleList; }
        }

        internal List<DmlNumericRule> Values
        {
            get { return mValues; }
            set { mValues = value; }
        }
        
        public static DmlNumericRuleList Empty { get; }

        static DmlNumericRuleList()
        {
            Empty = new DmlNumericRuleList();
            Empty.Values = new List<DmlNumericRule>();
        }

        internal override DmlDiagramLayoutNodeContentItem DeepCopy()
        {
            DmlNumericRuleList copy = (DmlNumericRuleList)MemberwiseClone();

            copy.Values = new List<DmlNumericRule>(Values.Count);
            foreach (DmlNumericRule value in Values)
                copy.Values.Add(value.DeepCopy());

            return copy;
        }

        private List<DmlNumericRule> mValues;
    }
}
