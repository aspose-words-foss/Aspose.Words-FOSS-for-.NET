// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Common
{
    internal interface IDmlHierarchicalPropertyBagParentProvider
    {
        IDmlHierarchicalPropertyBag ParentBag { get; }
        IDmlHierarchicalPropertyBagParentProvider Clone();
    }
}