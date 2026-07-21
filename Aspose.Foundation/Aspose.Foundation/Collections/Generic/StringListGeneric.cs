// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/05/2024 by Ilya Navrotskiy

using System;
using System.Collections;
using System.Collections.Generic;

namespace Aspose.Collections.Generic
{
    /// <summary>
    /// Represents generic unsorted list of strings mapped to <see cref="TValue"/>.
    /// </summary>
    /// <remarks>
    /// This is implementation based on <see cref="Dictionary{TKey,TValue}"/> cache,
    /// so that the reading from the list is very fast in case of unique string keys (O(1)).
    /// The string keys can be non-unique. In this case the cache is not applicable.
    /// </remarks>
    public class StringListGeneric<TValue> : IEnumerable<KeyValuePair<string, TValue>>
    {
        /// <summary>
        /// Creates instance of a <see cref="StringListGeneric{TValue}"/> class.
        /// </summary>
        public StringListGeneric()
        {
            mStringList = new List<string>();
            mValuesList = new List<TValue>();

            mCache = new Dictionary<string, TValue>();
        }

        /// <summary>
        /// Creates instance of <see cref="StringListGeneric{TValue}"/> from specified arrays.
        /// </summary>
        public StringListGeneric(string[] keys, TValue[] values)
        {
            ArgumentUtil.CheckNotNull(keys, "keys");
            ArgumentUtil.CheckNotNull(values, "values");

            if (keys.Length != values.Length)
                throw new ArgumentException("Keys length does not equal to values length.");

            mStringList = new List<string>(keys.Length);
            mValuesList = new List<TValue>(values.Length);
            mCache = new Dictionary<string, TValue>(keys.Length);

            for (int i = 0; i < keys.Length; i++)
            {
                string key = keys[i];
                mStringList.Add(key);

                TValue value = values[i];
                mValuesList.Add(value);

                mCache[key] = value;
            }
        }

        /// <summary>
        /// Creates instance from other <see cref="StringListGeneric{TValue}"/> collection.
        /// </summary>
        public StringListGeneric(StringListGeneric<TValue> other) : this()
        {
            ArgumentUtil.CheckNotNull(other, "other");

            for (int i = 0; i < other.Count; i++)
            {
                string key = other.mStringList[i];
                TValue value = other.mValuesList[i];

                mStringList.Add(key);
                mValuesList.Add(value);
                mCache[key] = value;
            }
        }

        /// <summary>
        /// Returns value at a specified index.
        /// </summary>
        public TValue GetByIndex(int index)
        {
            return mValuesList[index];
        }

        /// <summary>
        /// Removes key and value at a specified index.
        /// </summary>
        public void RemoveAt(int index)
        {
            string key = mStringList[index];

            mStringList.RemoveAt(index);
            mValuesList.RemoveAt(index);

            mCache.Remove(key);
        }

        /// <summary>
        /// Returns the zero-based index of the first occurrence of a key in the <see cref="StringListGeneric{TValue}"/>.
        /// </summary>
        public int IndexOfKey(string key)
        {
            return mStringList.IndexOf(key);
        }

        /// <summary>
        /// Determines whether an element with a specified key is in the <see cref="StringListGeneric{TValue}"/>.
        /// </summary>
        public bool ContainsKey(string key)
        {
            if (IsValidCache)
                return mCache.ContainsKey(key);

            return (mStringList.IndexOf(key) != -1);
        }

        /// <summary>
        /// Adds element with a specified key and value to the end of the <see cref="StringListGeneric{TValue}"/>.
        /// </summary>
        public void Add(string key, TValue value)
        {
            mStringList.Add(key);
            mValuesList.Add(value);

            mCache[key] = value;
        }

        /// <summary>
        /// Removes last occurence of element with a specified key from the <see cref="StringListGeneric{TValue}"/>.
        /// </summary>
        public void Remove(string key)
        {
            // Fast check if all keys in the list are unique and the key does not exist.
            if (IsValidCache && !mCache.ContainsKey(key))
                return;

            int index = mStringList.LastIndexOf(key);
            if (index == -1)
                return;

            mStringList.RemoveAt(index);
            mValuesList.RemoveAt(index);

            mCache.Remove(key);
        }

        /// <summary>
        /// Returns value corresponded to a last occurence of a specified key from the <see cref="StringListGeneric{TValue}"/> safely.
        /// </summary>
        /// <remarks>
        /// Does not throw if the specified key is not within the collection, but returns a specified default value.
        /// </remarks>
        public TValue GetValueSafe(string key, TValue defaultValue)
        {
            TValue value;
            if (mCache.TryGetValue(key, out value))
                return value;

            // The cache is valid and value is not found. In this case we return the default value.
            if (IsValidCache)
                return defaultValue;

            // The cache is invalid. Lets get value directly from the unsorted list collection.
            int index = mStringList.LastIndexOf(key);
            if (index == -1)
                return defaultValue;

            return mValuesList[index];
        }

        /// <summary>
        /// Returns key at a specified index.
        /// </summary>
        public string GetKey(int index)
        {
            return mStringList[index];
        }

        /// <summary>
        /// Sorts list by key.
        /// </summary>
        public StringListGeneric<TValue> Sort()
        {
            for (int i = 0; i < mStringList.Count - 1; i++)
            {
                for (int j = i + 1; j < mStringList.Count; j++)
                {
                    int comparision = string.Compare(mStringList[i], mStringList[j], StringComparison.Ordinal);
                    if (comparision > 0)
                    {
                        Swap(mStringList, i, j);
                        Swap(mValuesList, i, j);
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Swaps two values in <see cref="StringListGeneric{TValue}"/>.
        /// </summary>
        private static void Swap<T>(IList<T> list, int indexA, int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        #region IEnumerable implementation

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<KeyValuePair<string, TValue>> GetEnumerator()
        {
            return new StringListEnumerator<TValue>(this);
        }

        #endregion

        /// <summary>
        /// Gets or sets the value associated with the specified string key.
        /// </summary>
        public object this[string key]
        {
            get
            {
                TValue value;
                if (mCache.TryGetValue(key, out value))
                    return value;

                // The cache is valid and value is not found.
                if (IsValidCache)
                    return null;

                // The cache is invalid. Lets get value directly from the unsorted list collection.
                int index = mStringList.LastIndexOf(key);
                if (index == -1)
                    return null;

                return mValuesList[index];
            }
            set
            {
                // Fast insert in case of all keys in the list are unique (i.e., the cache is valid).
                if (IsValidCache && !mCache.ContainsKey(key))
                {
                    mStringList.Add(key);
                    mValuesList.Add((TValue)value);
                }
                else
                {
                    int index = mStringList.LastIndexOf(key);
                    if (index == -1)
                    {
                        mStringList.Add(key);
                        mValuesList.Add((TValue)value);
                    }
                    else
                    {
                        mValuesList[index] = (TValue)value;
                    }
                }

                mCache[key] = (TValue)value;
            }
        }

        public int Count
        {
            get { return mStringList.Count; }
        }

        /// <summary>
        /// Returns true if the cache <see cref="mCache"/> is valid.
        /// </summary>
        private bool IsValidCache
        {
            get { return (mCache.Count == mStringList.Count); }
        }

        /// <summary>
        /// The collection of string keys in <see cref="StringListGeneric{TValue}"/>.
        /// </summary>
        private readonly List<string> mStringList;

        /// <summary>
        /// The collection of <see cref="TValue"/> objects in <see cref="StringListGeneric{TValue}"/>.
        /// </summary>
        private readonly List<TValue> mValuesList;

        /// <summary>
        /// The cache for fast access to <see cref="StringListGeneric{TValue}"/> when its keys are unique.
        /// </summary>
        private readonly Dictionary<string, TValue> mCache;
    }
}
