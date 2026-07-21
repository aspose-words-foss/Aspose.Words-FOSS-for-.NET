// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 10/02/2016 by Dmitry Burov

using System;

namespace Aspose.Collections
{
    /// <summary>
    /// This is a copy of <see cref="SortedIntegerList" /> but short[] array is used for keys instead of int[] 
    /// to reduce memory consumption. Fallback is added for cases when we try to put the key that greater than 
    /// short.MaxValue: the keys are switched from short[] to int[].
    /// See https://auckland.dynabic.com/jira/browse/WORDSJAVA-1184 and MailMergePerf.TimeMergeField()
    /// </summary>
    [System.Diagnostics.DebuggerStepThrough]
    public class SortedShortListIntegerFallback
    {
        private short[] mShortKeys;
        private int[] mKeys;
        private object[] mValues;
        private int mSize;

        // WORDSNET-21593 Reduce default capacity to optimize memory consumption.
        // WORDSNET-20975 In most cases AW is about 5% faster and takes 5-30% less memory if
        // set defaultCapacity to 2, instead of 16.
        protected virtual int DefaultCapacity
        {
            get { return 16; }
        }

        public SortedShortListIntegerFallback()
        {
            mShortKeys = null;
            mKeys = null;
            mValues = null;
        }

        public SortedShortListIntegerFallback(int initialCapacity)
        {
            if (initialCapacity < 0)
                throw new ArgumentOutOfRangeException("initialCapacity");

            mShortKeys = new short[initialCapacity];
            mKeys = null;
            mValues = new Object[initialCapacity];
        }

        private void Init()
        {
            if (mValues != null)
                return;

            mShortKeys = new short[DefaultCapacity];
            mKeys = null;
            mValues = new Object[DefaultCapacity];
        }

        private void SwitchToIntKeys()
        {
            Init();
            mKeys = new int[mShortKeys.Length];
            // Don't use System.arraycopy(shortKeys, 0, keys, 0, shortKeys.length) because it throws
            // java.lang.ArrayStoreException. shortKeys and keys are different types.
            for (int i = 0; i < mShortKeys.Length; i++)
                mKeys[i] = mShortKeys[i];

            mShortKeys = null;
        }

        public virtual void Add(int key, Object value)
        {
            Init();
            if (mKeys != null)
            {
                AddInt(key, value);
            }
            else if (key > short.MaxValue)
            {
                SwitchToIntKeys();
                AddInt(key, value);
            }
            else
            {
                AddShort((short)key, value);
            }
        }

        private void AddInt(int key, Object value)
        {
            Init();
            int i = ArrayUtil.BinarySearch(mKeys, 0, mSize, key);
            if (i >= 0)
                throw new ArgumentException("duplicate");
            InsertInt(~i, key, value);
        }

        private void AddShort(short key, Object value)
        {
            Init();
            int i = ArrayUtil.BinarySearch(mShortKeys, 0, mSize, key);
            if (i >= 0)
                throw new ArgumentException("duplicate");
            InsertShort(~i, key, value);
        }

        public int Capacity
        {
            get 
            {
                if (mValues == null)
                    return 0;

                return mKeys != null ? mKeys.Length : mShortKeys.Length; 
            }
            set
            {
                Init();
                if (mKeys != null)
                    SetCapacityInt(value);
                else
                    SetCapacityShort(value);
            }
        }

        private void SetCapacityInt(int value)
        {
            if (value != mKeys.Length)
            {
                if (value < mSize)
                    throw new ArgumentOutOfRangeException("value");

                if (value > 0)
                {
                    int[] newKeys = new int[value];
                    Object[] newValues = new Object[value];
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
                    mValues = new Object[DefaultCapacity];
                }
            }
        }

        private void SetCapacityShort(int value)
        {
            if (value != mShortKeys.Length)
            {
                if (value < mSize)
                    throw new ArgumentOutOfRangeException("value");

                if (value > 0)
                {
                    short[] newKeys = new short[value];
                    Object[] newValues = new Object[value];
                    if (mSize > 0)
                    {
                        Array.Copy(mShortKeys, 0, newKeys, 0, mSize);
                        Array.Copy(mValues, 0, newValues, 0, mSize);
                    }
                    mShortKeys = newKeys;
                    mValues = newValues;
                }
                else
                {
                    mShortKeys = new short[DefaultCapacity];
                    mValues = new Object[DefaultCapacity];
                }
            }
        }

        public virtual int Count
        {
            get { return mValues == null ? 0 : mSize; }
        }

        public virtual void Clear()
        {
            mSize = 0;

            mKeys = null;
            mShortKeys = null;
            mValues = null;
        }

        protected SortedShortListIntegerFallback CreateEmptyCopy()
        {
            if (mValues == null)
            {
                // fast copy of empty collection.
                return (SortedShortListIntegerFallback)this.MemberwiseClone();
            }

            Init();

            SortedShortListIntegerFallback sl = (SortedShortListIntegerFallback)this.MemberwiseClone();
            sl.mSize = 0;

            if (mKeys != null)
                sl.mKeys = new int[mKeys.Length];
            else
                sl.mShortKeys = new short[mShortKeys.Length];

            sl.mValues = new Object[mValues.Length];
            return sl;
        }

        public virtual bool Contains(int key)
        {
            if (mValues == null)
                return false;

            return IndexOfKey(key) >= 0;
        }

        /// <summary>
        /// Returns true if specified key is in the list.
        /// Performs binary search.
        /// </summary>
        public virtual bool ContainsKey(int key)
        {
            if (mValues == null)
                return false;

            // Yes, this is a SPEC'ed duplicate of Contains().
            return IndexOfKey(key) >= 0;
        }

        /// <summary>
        /// Returns true if at least one of specified keys is in the list.
        /// Performs binary search.
        /// </summary>
        public virtual bool ContainsAnyKey(params int[] keys)
        {
            if (mValues == null)
                return false;

            for (int i = 0; i < keys.Length; i++)
            {
                // Stop search on the first occurrence.
                if (ContainsKey(keys[i]))
                    return true;
            }

            return false;
        }

        public bool ContainsValue(Object value)
        {
            if (mValues == null)
                return false;

            return IndexOfValue(value) >= 0;
        }

        private void EnsureCapacityInt(int min)
        {
            int newCapacity = mKeys.Length == 0 ? 16 : mKeys.Length * 2;
            if (newCapacity < min) newCapacity = min;
            SetCapacityInt(newCapacity);
        }

        private void EnsureCapacityShort(int min)
        {
            int newCapacity = mShortKeys.Length == 0 ? 16 : mShortKeys.Length * 2;
            if (newCapacity < min) newCapacity = min;
            SetCapacityShort(newCapacity);
        }

        public virtual Object GetByIndex(int index)
        {
            if (mValues == null || index < 0 || index >= mSize)
                throw new ArgumentOutOfRangeException("index");
            return mValues[index];
        }

        public virtual int GetKey(int index)
        {
            if (mValues == null || index < 0 || index >= mSize)
                throw new ArgumentOutOfRangeException("index");
            return (mKeys != null) ? mKeys[index] : mShortKeys[index];
        }

        public virtual object this[int key]
        {
            get
            {
                if (mValues == null)
                    return null;

                int index = IndexOfKey(key);
                return (index >= 0) ? mValues[index] : null;
            }
            set
            {
                Init();

                if (mKeys != null)
                {
                    SetInt(key, value);
                }
                else if (key > short.MaxValue)
                {
                    SwitchToIntKeys();
                    SetInt(key, value);
                }
                else
                {
                    SetShort((short)key, value);
                }
            }
        }

        private void SetInt(int key, Object value)
        {
            Init();

            int i = ArrayUtil.BinarySearch(mKeys, 0, mSize, key);
            if (i >= 0)
            {
                mValues[i] = value;
                return;
            }
            InsertInt(~i, key, value);
        }

        private void SetShort(short key, Object value)
        {
            Init();

            int i = ArrayUtil.BinarySearch(mShortKeys, 0, mSize, key);
            if (i >= 0)
            {
                mValues[i] = value;
                return;
            }
            InsertShort(~i, key, value);
        }

        public virtual int IndexOfKey(int key)
        {
            if (mValues == null)
                return -1;

            int ret;

            if (mKeys != null)
                ret = ArrayUtil.BinarySearch(mKeys, 0, mSize, key);
            else
                ret = (key > short.MaxValue) ? -1 : ArrayUtil.BinarySearch(mShortKeys, 0, mSize, (short) key);

            return ret >=0 ? ret : -1;
        }

        public virtual int IndexOfValue(Object value)
        {
            if (mValues == null)
                return -1;

            return Array.IndexOf(mValues, value, 0, mSize);
        }

        private void InsertInt(int index, int key, Object value)
        {
            Init();

            if (mSize == mKeys.Length) EnsureCapacityInt(mSize + 1);
            if (index < mSize)
            {
                Array.Copy(mKeys, index, mKeys, index + 1, mSize - index);
                Array.Copy(mValues, index, mValues, index + 1, mSize - index);
            }
            mKeys[index] = key;
            mValues[index] = value;
            mSize++;
        }

        private void InsertShort(int index, short key, Object value)
        {
            Init();

            if (mSize == mShortKeys.Length) EnsureCapacityShort(mSize + 1);
            if (index < mSize)
            {
                Array.Copy(mShortKeys, index, mShortKeys, index + 1, mSize - index);
                Array.Copy(mValues, index, mValues, index + 1, mSize - index);
            }
            mShortKeys[index] = key;
            mValues[index] = value;
            mSize++;
        }

        public virtual void RemoveAt(int index)
        {
            if (mValues == null || index < 0 || index >= mSize)
                throw new ArgumentOutOfRangeException("index");

            mSize--;
            if (index < mSize)
            {
                if (mKeys != null)
                    Array.Copy(mKeys, index + 1, mKeys, index, mSize - index);
                else
                    Array.Copy(mShortKeys, index + 1, mShortKeys, index, mSize - index);

                Array.Copy(mValues, index + 1, mValues, index, mSize - index);
            }
            if (mKeys != null)
                mKeys[mSize] = 0;
            else
                mShortKeys[mSize] = 0;

            mValues[mSize] = null;
        }

        public virtual void Remove(int key)
        {
            int i = IndexOfKey(key);
            if (i >= 0)
                RemoveAt(i);
        }

        public virtual void SetByIndex(int index, Object value)
        {
            Init();

            if (index < 0 || index >= mSize)
                throw new ArgumentOutOfRangeException("index");
            mValues[index] = value;
        }

        public void TrimToSize()
        {
            Capacity = mSize;
        }
    }
}
