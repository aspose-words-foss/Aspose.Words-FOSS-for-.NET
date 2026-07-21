// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/09/2013 by Ivan Lyagin

using System.Collections;
using System.Collections.Generic;
using Aspose.Collections.Generic;

namespace Aspose.Collections
{
    /// <summary>
    /// Represents <see cref="ISetGeneric{T}"/> which elements are case-insensitive strings.
    /// This is a complementary class to HashSet.
    /// </summary>
    public class CaseInsensitiveHashSet : ISetGeneric<string>
    {
        /// <summary>
        /// Initializes a new, empty set object using the default initial capacity.
        /// </summary>
        public CaseInsensitiveHashSet()
        {
            mDictionary = new StringToObjDictionary<string>(false);
        }

        /// <summary>
        /// Initializes a new, empty set object using the specified initial capacity.
        /// </summary>
        public CaseInsensitiveHashSet(int capacity)
        {
            mDictionary = new StringToObjDictionary<string>(capacity, false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CaseInsensitiveHashSet"/> class that contains specified items.
        /// </summary>
        public CaseInsensitiveHashSet(params string[] items)
        {
            mDictionary = new StringToObjDictionary<string>(items.Length, false);
            foreach (string item in items)
                Add(item);
        }

        /// <summary>
        /// Adds the specified element to a set object.
        /// Returns <c>true</c> if the element is added or <c>false</c> if the element is already present.
        /// </summary>
        public bool Add(string item)
        {
            string key = GetDictionaryKey(item);
            if (mDictionary.ContainsKey(key))
                return false;

            // Value of a dictionary is used to make possible to iterate over the dictionary
            // returning null (not NullKey string) where appropriate.
            mDictionary.Add(key, item);
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
        public bool Contains(string item)
        {
            return mDictionary.ContainsKey(GetDictionaryKey(item));
        }

        /// <summary>
        /// Removes the specified element from a set object.
        /// Returns <c>true</c> if the element is successfully found and removed; otherwise, <c>false</c>.
        /// </summary>
        public bool Remove(string item)
        {
            return mDictionary.Remove(GetDictionaryKey(item));
        }

        /// <summary>
        /// Returns an enumerator that can iterate through a set object.
        /// </summary>
        public IEnumerator<string> GetEnumerator()
        {
            return mDictionary.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static string GetDictionaryKey(object item)
        {
            return (item != null)
                ? (string)item
                : NullKey;
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
        private readonly StringToObjDictionary<string> mDictionary;

        /// <summary>
        /// A key value corresponding to a null item. Actually this is a hack, but let's hope that
        /// this value will not match a real item, which can be neglected according to GUID concept.
        /// </summary>
        private const string NullKey = "{62C5CE33-64E0-4EEC-9E16-1CA57A484BD2}";
    }
}
