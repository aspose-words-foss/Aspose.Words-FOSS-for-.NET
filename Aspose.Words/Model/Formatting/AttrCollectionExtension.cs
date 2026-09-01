// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 15/10/2020 by Tengiz Sharafiev

namespace Aspose.Words
{
    internal static class AttrCollectionExtension
    {
        /// <summary>
        /// Creates a deep copy of the collection.
        ///
        /// Attributes that implement the IAttr interface are deep copied,
        /// but value types are value copied.
        /// </summary>
        internal static TAttrCollection Clone<TAttrCollection>(this TAttrCollection attr)
            where TAttrCollection : AttrCollection
        {
            return (TAttrCollection)attr.CloneCore();
        }

        /// <summary>
        /// Sets attribute value.
        /// </summary>
        internal static TAttrCollection WithKeyValue<TAttrCollection>(this TAttrCollection attr, int key, object value)
            where TAttrCollection : AttrCollection
        {
            attr[key] = value;
            return attr;
        }
    }
}
