// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 11/08/2006 by Roman Korchagin

namespace Aspose.Words
{
    /// <summary>
    /// Attributes whose full value is a combination of the parent and child attribute values
    /// need to implement this interface.
    ///
    /// At the moment only tab stops collection use this.
    /// </summary>
    internal interface IExpandableAttr : IComplexAttr
    {
        /// <summary>
        /// Called when expanding tab stops to full formatting.
        ///
        /// Implementations need to create a new attribute that is a combination
        /// of the value of the parent attribute and this attribute.
        /// </summary>
        /// <param name="parentAttr">The parent attribute to combine with.
        /// Can be null as parent does not have to have tab stops always.</param>
        /// <returns>The combined attribute value.</returns>
        IExpandableAttr Expand(IExpandableAttr parentAttr);

        /// <summary>
        /// Called when collapsing tab stops during RTF import.
        ///
        /// Implementations need to modify this attribute by comparing it with the parent attribute.
        /// </summary>
        /// <param name="parentAttr">The parent attribute.</param>
        void Collapse(IExpandableAttr parentAttr);
    }
}
