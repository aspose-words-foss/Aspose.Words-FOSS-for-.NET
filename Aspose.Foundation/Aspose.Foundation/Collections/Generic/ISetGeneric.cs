// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/03/2017 by Alexey Butalov

using System.Collections.Generic;

namespace Aspose.Collections.Generic
{
    /// <summary>
    /// A set is a collection that contains no duplicate elements, and whose elements are in no particular order.
    /// 
    /// The interface is a replacement of ISet interface class provided by in .NET 4 and higher. So its methods are created
    /// compatible with this class. For now, not all ISet operations are declared, check of .NET ISet interface when adding
    /// new declarations to this interface.
    /// </summary>
    public interface ISetGeneric<T> : IEnumerable<T>
    {
        /// <summary>
        /// Adds the specified element to a set object.
        /// Returns <c>true</c> if the element is added or <c>false</c> if the element is already present.
        /// </summary>
        bool Add(T item);

        /// <summary>
        /// Removes all elements from a set object.
        /// </summary>
        void Clear();

        /// <summary>
        /// Determines whether a set contains the specified element.
        /// </summary>
        bool Contains(T item);

        /// <summary>
        /// Removes the specified element from a set object.
        /// Returns <c>true</c> if the element is successfully found and removed; otherwise, <c>false</c>.
        /// </summary>
        bool Remove(T item);

        /// <summary>
        /// Gets the number of elements contained in a set object.
        /// </summary>
        int Count { get; }
    }
}
