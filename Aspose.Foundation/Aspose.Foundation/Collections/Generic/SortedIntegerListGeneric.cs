// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/06/2018 by Alexey Butalov

using System;

namespace Aspose.Collections.Generic
{
    /// <summary>
    /// This is an equivalent of .NET's <see cref="System.Collections.Generic.SortedList{TKey,TValue}" />
    /// where TKey is int taken from Rotor source, extended for performance benefits.
    /// This class is used to prevent performance degradation in Java ported code because Java uses
    /// boxing and unboxing for value types used as generic parameters.
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class SortedIntegerListGeneric<T>
    {
        public SortedIntegerListGeneric()
        {
            mKeys = new int[DefaultCapacity];
            mValues = new object[DefaultCapacity];
        }

        public SortedIntegerListGeneric(int initialCapacity)
        {
            if (initialCapacity < 0)
                throw new ArgumentOutOfRangeException("initialCapacity");
            mKeys = new int[initialCapacity];
            mValues = new object[initialCapacity];
        }

        public SortedIntegerListGeneric(SortedIntegerListGeneric<T> sortedIntegerList)
        {
            mKeys = new int[sortedIntegerList.Capacity];
            mValues = new object[sortedIntegerList.Capacity];
            mSize = sortedIntegerList.Count;

            Array.Copy(sortedIntegerList.mKeys, mKeys, mKeys.Length);
            Array.Copy(sortedIntegerList.mValues, mValues, mValues.Length);
        }

        public void Add(int key, T value)
        {
            int i = ArrayUtil.BinarySearch(mKeys, 0, mSize, key);
            if (i >= 0)
                throw new ArgumentException("duplicate");
            Insert(~i, key, value);
        }

        /// <summary>
        /// Appends to end of sorted list. Make sure that you add keys in sorted order.
        /// </summary>
        /// <remarks>
        /// AM. Sometimes we add keys sorted by nature. Using this method reduces binary search performed.
        /// </remarks>
        public void Append(int key, T value)
        {
            if ((mSize > 0) && (key <= mKeys[mSize - 1]))
                throw new InvalidOperationException("Key value should be greater than max key value in list.");

            Insert(mSize, key, value);
        }

        public void Clear()
        {
            mSize = 0;
            mKeys = new int[DefaultCapacity];
            mValues = new object[DefaultCapacity];
        }

        public bool Contains(int key)
        {
            return IndexOfKey(key) >= 0;
        }

        /// <summary>
        /// Returns true if specified key is in the list.
        /// Performs binary search.
        /// </summary>
        public bool ContainsKey(int key)
        {
            // Yes, this is a SPEC'ed duplicate of Contains().
            return IndexOfKey(key) >= 0;
        }

        /// <summary>
        /// Returns value if found; or <c>defaultValue</c> otherwise.
        /// </summary>
        public T GetSafe(int key, T defaultValue)
        {
            int index = IndexOfKey(key);
            if (index >= 0)
                return (T)mValues[index];
            else
                return defaultValue;
        }

        public bool ContainsValue(T value)
        {
            return IndexOfValue(value) >= 0;
        }

        public T GetByIndex(int index)
        {
            if (index < 0 || index >= mSize)
                throw new ArgumentOutOfRangeException("index");
            return (T)mValues[index];
        }

        public int GetKey(int index)
        {
            if (index < 0 || index >= mSize)
                throw new ArgumentOutOfRangeException("index");
            return mKeys[index];
        }

        public int IndexOfKey(int key)
        {
            int ret = ArrayUtil.BinarySearch(mKeys, 0, mSize, key);
            return ret >= 0 ? ret : -1;
        }

        public int IndexOfValue(T value)
        {
            return Array.IndexOf(mValues, value, 0, mSize);
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= mSize)
                throw new ArgumentOutOfRangeException("index");
            mSize--;
            if (index < mSize)
            {
                Array.Copy(mKeys, index + 1, mKeys, index, mSize - index);
                Array.Copy(mValues, index + 1, mValues, index, mSize - index);
            }
            mKeys[mSize] = 0;
            mValues[mSize] = mDefaultValue;
        }

        public void Remove(int key)
        {
            int i = IndexOfKey(key);
            if (i >= 0)
                RemoveAt(i);
        }

        public override int GetHashCode()
        {
            int hashCode = 0;
            for (int i = 0; i < Count; i++)
            {
                hashCode = (hashCode * 397) ^ mKeys[i].GetHashCode();
                if (mValues[i] != null)
                    hashCode = (hashCode * 397) ^ mValues[i].GetHashCode();
            }

            return hashCode;
        }

        public int Capacity
        {
            get { return mKeys.Length; }
            set
            {
                if (value != mKeys.Length)
                {
                    if (value < mSize)
                        throw new ArgumentOutOfRangeException("value");
                    if (value > 0)
                    {
                        int[] newKeys = new int[value];
                        object[] newValues = new object[value];
                        if (mSize > 0)
                        {
                            Array.Copy(mKeys, 0, newKeys, 0, mSize);
                            Array.Copy(mValues, 0, newValues, 0, mSize);
                        }
                        mKeys = newKeys;
                        mValues = newValues;
                    }
                    else
                    {
                        mKeys = new int[DefaultCapacity];
                        mValues = new object[DefaultCapacity];
                    }
                }
            }
        }

        public int Count
        {
            get { return mSize; }
        }

        public T this[int key]
        {
            get
            {
                // In Java mDefaultValue = null and it works incorrect with value types like int and bool
                return GetSafe(key, mDefaultValue);
            }
            set
            {
                int i = ArrayUtil.BinarySearch(mKeys, 0, mSize, key);
                if (i >= 0)
                {
                    mValues[i] = value;
                    return;
                }
                Insert(~i, key, value);
            }
        }

        protected int[] Keys
        {
            get { return mKeys; }
        }

        protected object[] Values
        {
            get { return mValues; }
        }

        private void EnsureCapacity(int min)
        {
            int newCapacity = (mKeys.Length == 0)
                ? DefaultCapacity
                : mKeys.Length * 2;
            if (newCapacity < min)
                newCapacity = min;
            Capacity = newCapacity;
        }

        private void Insert(int index, int key, T value)
        {
            if (mSize == mKeys.Length)
                EnsureCapacity(mSize + 1);

            if (index < mSize)
            {
                Array.Copy(mKeys, index, mKeys, index + 1, mSize - index);
                Array.Copy(mValues, index, mValues, index + 1, mSize - index);
            }
            mKeys[index] = key;
            mValues[index] = value;
            mSize++;
        }

        private int[] mKeys;
        // Java-changed: was T[] but we can't instantiate generic arrays on Java.
        private object[] mValues;
        private int mSize;

#if !JAVA
        private readonly T mDefaultValue = default(T);
#else
        private T mDefaultValue = null;
#endif

        private const int DefaultCapacity = 16;
    }
}
