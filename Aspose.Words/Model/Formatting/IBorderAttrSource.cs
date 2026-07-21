// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/07/2005 by Roman Korchagin

using System.Collections.Generic;
using Aspose.JavaAttributes;

namespace Aspose.Words
{
    /// <summary>
    /// Objects that have one or more borders implement this interface so
    /// the Border and Borders classes can do the work of exposing the border to the public.
    ///
    /// It actually looks that GetDirectAttr and SetAttr are used by Borders only while
    /// FetchInheritedAttr is used by Border only - so it could be split into two different interfaces.
    /// </summary>
    internal interface IBorderAttrSource
    {
        /// <summary>
        /// Gets the specified border attribute specified directly on the object.
        /// </summary>
        /// <param name="key">The attribute id.</param>
        /// <returns>The Border object or null.</returns>
        object GetDirectBorderAttr(int key);

        /// <summary>
        /// Gets (throws if not found) the border attribute specified on the parent of the object.
        /// </summary>
        /// <param name="key">The attribute id.</param>
        /// <returns>The Border object.</returns>
        object FetchInheritedBorderAttr(int key);

        /// <summary>
        /// Stores the border attribute in the object.
        /// </summary>
        [JavaThrows(true)]
        void SetBorderAttr(int key, object value);

        /// <summary>
        /// Map of border types to border attribute keys.
        /// </summary>
        SortedList<BorderType, int> PossibleBorderKeys { get; }
    }
}
