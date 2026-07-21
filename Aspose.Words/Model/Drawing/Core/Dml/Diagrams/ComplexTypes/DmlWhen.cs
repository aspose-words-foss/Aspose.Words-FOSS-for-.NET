// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using System.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml.Diagrams.SimpleTypes;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.2.15 if (If)
    /// </summary>
    internal class DmlWhen : DmlExtensionListSource
    {
        internal string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        internal DmlIteratorAttributes IteratorAttributes
        {
            get { return mIteratorAttributes; }
        }

        internal DmlFunctionType Function
        {
            get { return mFunction; }
            set { mFunction = value; }
        }

        internal DmlFunctionOperator Operator
        {
            get { return mOperator; }
            set { mOperator = value; }
        }

        internal DmlVariableType Argument
        {
            get { return mArgument; }
            set { mArgument = value; }
        }

        internal string Value
        {
            get { return mValue; }
            set { mValue = value; }
        }

        internal List<DmlDiagramLayoutNodeContentItem> Content
        {
            get { return mContent; }
            set { mContent = value; }
        }

        internal DmlWhen DeepCopy()
        {
            DmlWhen copy = (DmlWhen)MemberwiseClone();
            
            copy.Content = new List<DmlDiagramLayoutNodeContentItem>(Content.Count);
            foreach (DmlDiagramLayoutNodeContentItem item in Content)
            {
                copy.Content.Add(item.DeepCopy());
            }

            return copy;
        }

        private string mName;
        private readonly DmlIteratorAttributes mIteratorAttributes = new DmlIteratorAttributes();
        private DmlFunctionType mFunction;
        private DmlFunctionOperator mOperator;
        private DmlVariableType mArgument = DmlVariableType.None;
        private string mValue; 
        private List<DmlDiagramLayoutNodeContentItem> mContent;
    }
}
