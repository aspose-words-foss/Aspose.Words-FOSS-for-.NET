// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 01/11/2011 by Andrey Soldatov

using System.Collections;
using System.Collections.Generic;

namespace Aspose.Collections.Generic
{
    /// <summary>
    /// A set is a collection that contains no duplicate elements, and whose elements are in no particular order.
    ///
    /// The HashSet is a replacement of HashSet class existing in .NET 3.5 and higher. So its methods are
    /// created compatible with this class. For now, not all HashSet operations are implemented, check interfaces of .NET
    /// HashSet operations when adding new ones to this class.
    /// </summary>
    public class HashSetGeneric<T> : ISetGeneric<T>
    {
        /// <summary>
        /// Initializes a new, empty set object using the default initial capacity.
        /// </summary>
        public HashSetGeneric()
        {
            mDictionary = new Dictionary<T, bool>();
        }

        /// <summary>
        /// Initializes a new, empty set object using the specified initial capacity.
        /// </summary>
        public HashSetGeneric(int capacity)
        {
            mDictionary = new Dictionary<T, bool>(capacity);
        }

        public HashSetGeneric(IEnumerable<T> items)
        {
            mDictionary = new Dictionary<T, bool>();
            foreach (T item in items)
                Add(item);
        }

        /// <summary>
        /// Adds the specified element to a set object.
        /// Returns <c>true</c> if the element is added or <c>false</c> if the element is already present.
        /// </summary>
        public bool Add(T item)
        {
            if (mDictionary.ContainsKey(item))
                return false;

            mDictionary.Add(item, false);
            return true;
        }

        /// <summary>
        /// Removes all elements from a set object.
        /// </summary>
        public void Clear()
        {
            mDictionary.Clear();
        }

        /// <summary>
        /// Determines whether a set contains the specified element.
        /// </summary>
        public bool Contains(T item)
        {
            return mDictionary.ContainsKey(item);
        }

        /// <summary>
        /// Removes the specified element from a set object.
        /// Returns <c>true</c> if the element is successfully found and removed; otherwise, <c>false</c>.
        /// </summary>
        public bool Remove(T item)
        {
            if (!mDictionary.ContainsKey(item))
                return false;

            mDictionary.Remove(item);
            return true;
        }

        /// <summary>
        /// Returns an enumerator that can iterate through a set object.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return mDictionary.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the number of elements contained in a set object.
        /// </summary>
        public int Count
        {
            get { return mDictionary.Count; }
        }

        /// <summary>
        /// Underlying container.
        /// </summary>
        private readonly Dictionary<T, bool> mDictionary;
    }
}
