// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/07/2011 by Alexey Titov

namespace Aspose.Words.Drawing.Core.Dml.Common
{
    internal interface IDmlHierarchicalPropertyBag
    {
        object GetProperty(int key);
        [CodePorting.Translator.Cs2Cpp.CppConstMethod()]
        object GetDirectProperty(int key);
        bool IsPropertySpecified(int key);
        void SetProperty(int key, object value);
        void Remove(int key);

        /// <summary>
        /// Removes all items from the property bag.
        /// </summary>
        void RemoveAll();

        IDmlHierarchicalPropertyBagParentProvider ParentBagProvider { get; set; }
        IDmlHierarchicalPropertyBag ExtensionProperties { get; set; }
        IDmlHierarchicalPropertyBag Clone();

        /// <summary>
        /// Returns a flag indicating whether any property of this bag has value different than in the parent bag.
        /// </summary>
        /// <param name="attributesToIgnore">Properties/attributes that should be excluded from the checking.</param>
        /// <param name="parent">If defined, this parent is used in comparison instead of a bag of
        /// <see cref="ParentBagProvider"/>.</param>
        /// <returns>A flag indicating whether any property has non-default value.</returns>
        bool HasNonDefaultFormatting(int[] attributesToIgnore, IDmlHierarchicalPropertyBag parent);

        int Count { get; }
    }
}
