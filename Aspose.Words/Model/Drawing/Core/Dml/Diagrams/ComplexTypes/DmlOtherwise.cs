// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/05/2013 by Konstantin Kornilov

using System.Collections.Generic;

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// 21.4.2.12 else (Else)
    /// </summary>
    internal class DmlOtherwise : DmlExtensionListSource
    {
        internal string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        internal List<DmlDiagramLayoutNodeContentItem> Content
        {
            get { return mContent; }
            set { mContent = value; }
        }

        internal DmlOtherwise DeepCopy()
        {
            DmlOtherwise copy = (DmlOtherwise)MemberwiseClone();

            copy.Content = new List<DmlDiagramLayoutNodeContentItem>(Content.Count);
            foreach (DmlDiagramLayoutNodeContentItem item in Content)
                copy.Content.Add(item.DeepCopy());

            return copy;
        }

        private string mName;
        private List<DmlDiagramLayoutNodeContentItem> mContent;
    }
}
