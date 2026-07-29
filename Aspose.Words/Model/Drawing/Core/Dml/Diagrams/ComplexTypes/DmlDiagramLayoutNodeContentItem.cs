// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/10/2013 by Konstantin Kornilov

namespace Aspose.Words.Drawing.Core.Dml.Diagrams.ComplexTypes
{
    /// <summary>
    /// Base class for layout node content items.
    /// </summary>
    internal abstract class DmlDiagramLayoutNodeContentItem : DmlExtensionListSource
    {
        internal abstract void Accept(IDmlDiagramLayoutNodeContentItemVisitor visitor);

        internal abstract DmlDiagramLayoutNodeContentItemType ContentItemType { get; }

        internal virtual DmlDiagramLayoutNodeContentItem DeepCopy()
        {
            return (DmlDiagramLayoutNodeContentItem) MemberwiseClone();
        }
    }
}
