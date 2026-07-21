// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using Aspose.Collections.Generic;
using Aspose.Words.Drawing.Core.Dml.Diagrams.SimpleTypes;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.2.3 alg (Algorithm)
    /// </summary>
    internal class DmlAlgorithm : DmlDiagramLayoutNodeContentItem
    {
        internal override void Accept(IDmlDiagramLayoutNodeContentItemVisitor visitor)
        {
            visitor.VisitAlgorithm(this);
        }

        internal override DmlDiagramLayoutNodeContentItemType ContentItemType
        {
            get { return DmlDiagramLayoutNodeContentItemType.Algorithm; }
        }

        internal DmlAlgorithmType Type
        {
            get { return mType; }
            set { mType = value; }
        }

        internal int Revision
        {
            get { return mRevision; }
            set { mRevision = value; }
        }

        internal SortedStringListGeneric<string> Params
        {
            get { return mParams; }
        }

        private DmlAlgorithmType mType;
        private int mRevision;
        private readonly SortedStringListGeneric<string> mParams = new SortedStringListGeneric<string>();
    }
}
