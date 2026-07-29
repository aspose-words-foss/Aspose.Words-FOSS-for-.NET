// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/10/2020 by Tengiz Sharafiev

namespace Aspose.Words
{
    internal static class AttrCollectionExtension
    {
        internal static TAttrCollection Clone<TAttrCollection>(this TAttrCollection attr)
            where TAttrCollection : AttrCollection
        {
            return (TAttrCollection)attr.CloneCore();
        }
    }
}
