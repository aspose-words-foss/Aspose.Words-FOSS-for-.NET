// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 08/07/2013 by Ivan Lyagin

#if INCLUDE_FILE
using System;
using System.Collections;
using System.Collections.Generic;
using CodePorting.Translator.Cs2Cpp;

namespace Aspose.Collections
{
    /// <summary>
    /// Represents a collection of keys and values of certain types.
    /// </summary>
    /// <dev>
    /// This class was created using ROTOR sources. However, some of its behavior was changed to be more Hashtable-like.
    /// Also it was reworked considering AW specifics and Java autoportability.
    /// </dev>
    public class Dictionary<TKey, TValue>
#if STRING_KEY
#else
#if NULLABLE_KEY
        where TKey : class
#endif
#endif
#if STRING_VALUE
#else
#if NULLABLE_VALUE
        where TValue : class
#endif
#endif
    {
        /// <summary>
        /// Initializes a new instance of the Dictionary&lt;TKey, TValue&gt; class that is empty and has the default initial capacity.
#if STRING_KEY
        /// Uses case-sensitive ordinal key comparison.
#endif
        /// </summary>
        public Dictionary()
            : this(0)
        {
        }

        /// <summary>
        /// Initializes a new instance of the Dictionary&lt;TKey, TValue&gt; class that is empty and has the specified initial capacity.
#if STRING_KEY
        /// Uses case-sensitive ordinal key comparison.
#endif
        /// </summary>
        public Dictionary(int capacity)
        {
            ArgumentUtil.CheckNonNegative(capacity, "capacity");
            if (capacity > 0)
                Initialize(capacity);
        }

        /// <summary>
        /// Initializes a new instance of the Dictionary&lt;TKey, TValue&gt; class that contains elements copied from the specified 
        /// Dictionary&lt;TKey, TValue&gt;.
#if STRING_KEY
        /// Uses case-sensitive ordinal key comparison.
#endif
        /// </summary>
        public Dictionary(Dictionary<TKey, TValue> dictionary)
            : this((dictionary != null) ? dictionary.Count : 0)
        {
            ArgumentUtil.CheckNotNull(dictionary, "dictionary");

            Enumerator enumerator = dictionary.GetEnumerator();
            while (enumerator.MoveNext())
                Add(enumerator.CurrentKey, enumerator.CurrentValue);
        }
#if STRING_KEY

        /// <summary>
        /// Initializes a new instance of the Dictionary&lt;TKey, TValue&gt; class that is empty, has the default initial capacity
        /// and uses either case-sensitive or case-insensitive ordinal key comparison.
        /// </summary>
        public Dictionary(bool isCaseSensitive)
            : this()
        {
            mIgnoreCase = !isCaseSensitive;
        }

        /// <summary>
        /// Initializes a new instance of the Dictionary&lt;TKey, TValue&gt; class that is empty, has the specified initial capacity
        /// and uses either case-sensitive or case-insensitive ordinal key comparison.
        /// </summary>
        public Dictionary(int capacity, bool isCaseSensitive)
            : this(capacity)
        {
            mIgnoreCase = !isCaseSensitive;
        }

        /// <summary>
        /// Initializes a new instance of the Dictionary&lt;TKey, TValue&gt; class that contains elements copied from the specified 
        /// Dictionary&lt;TKey, TValue&gt; using either case-sensitive or case-insensitive ordinal key comparison.
        /// </summary>
        public Dictionary(Dictionary<TKey, TValue> dictionary, bool isCaseSensitive)
            : this((dictionary != null) ? dictionary.Count : 0, isCaseSensitive)
        {
            ArgumentUtil.CheckNotNull(dictionary, "dictionary");

            Enumerator enumerator = dictionary.GetEnumerator();
            while (enumerator.MoveNext())
                Add(enumerator.CurrentKey, enumerator.CurrentValue);
        }
#endif

        /// <summary>
        /// Adds the specified key and value to the Dictionary&lt;TKey, TValue&gt;.
        /// </summary>
        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        /// <summary>
        /// Removes all keys and values from the Dictionary&lt;TKey, TValue&gt;.
        /// </summary>
        public void Clear()
        {
            if (mCount == 0)
                return;

            ClearBuckets(mBuckets);

            // Set default values for entry data arrays.
            Array.Clear(mHashCodes, 0, mCount);
            Array.Clear(mNextIndexes, 0, mCount);
#if PRIMITIVE_KEY
            Array.Clear(mKeys, 0, mCount);
#else
            for (int i = 0; i < mCount; i++)
                mKeys[i] = null;
#endif

#if PRIMITIVE_VALUE
            Array.Clear(mValues, 0, mCount);
#else
            for (int i = 0; i < mCount; i++)
                mValues[i] = null;
#endif
            mFreeList = InvalidIndex;
            mCount = 0;
            mFreeCount = 0;
            mVersion++;
        }

        /// <summary>
        /// Fills the specified bucket array with default values.
        /// </summary>
        private static void ClearBuckets(int[] buckets)
        {
            int bucketCount = buckets.Length;
            for (int i = 0; i < bucketCount; i++)
                buckets[i] = InvalidIndex;
        }

        /// <summary>
        /// Determines whether the Dictionary&lt;TKey, TValue&gt; contains the specified key.
        /// </summary>
        public bool ContainsKey(TKey key)
        {
            return FindEntry(key) >= 0;
        }

        /// <summary>
        /// Determines whether the Dictionary&lt;TKey, TValue&gt; contains a specific value.
        /// </summary>
        public bool ContainsValue(TValue value)
        {
            for (int i = 0; i < mCount; i++)
            {
                if ((mHashCodes[i] >= 0) && CompareValues(mValues[i], value))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the Dictionary&lt;TKey, TValue&gt;.
        /// </summary>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Returns the index of an entry with the specified key.
        /// </summary>
        private int FindEntry(TKey key)
        {
#if NULLABLE_KEY
            CheckKeyNotNull(key);
#endif
            if (mBuckets != null)
            {
                int hashCode = GetHashCode(key);
                for (int i = mBuckets[hashCode % mBuckets.Length]; i >= 0; i = mNextIndexes[i])
                {
                    if ((mHashCodes[i] == hashCode) && CompareKeys(mKeys[i], key))
                        return i;
                }
            }

            return InvalidIndex;
        }

        /// <summary>
        /// Initializes the dictionary according to the specified capacity.
        /// </summary>
        private void Initialize(int capacity)
        {
            // Obtain the initial size.
            int size = MathUtil.GetPrimeForHashtable(capacity);

            // Initialize buckets.
            mBuckets = new int[size];
            ClearBuckets(mBuckets);

            // Initialize entry data arrays.
            mHashCodes = new int[size];
            mNextIndexes = new int[size];

#if PRIMITIVE_KEY
            mKeys = new TKey[size];
#else
            mKeys = new List<TKey>(size);
            for (int i = 0; i < size; i++)
                mKeys.Add(null);
#endif

#if PRIMITIVE_VALUE
            mValues = new TValue[size];
#else
            mValues = new List<TValue>(size);
            for (int i = 0; i < size; i++)
                mValues.Add(null);
#endif
            mFreeList = InvalidIndex;
        }

        /// <summary>
        /// Inserts the specified key and value to the Dictionary&lt;TKey, TValue&gt;. Throws, if the specified key has already
        /// been added to the dictionary and <paramref name="add"/> is set to true.
        /// </summary>
        private void Insert(TKey key, TValue value, bool add)
        {
#if NULLABLE_KEY
            CheckKeyNotNull(key);
#endif
            if (mBuckets == null)
                Initialize(0);

            // Try to find and replace an existing value with the specified key.
            int hashCode = GetHashCode(key);
            for (int i = mBuckets[hashCode % mBuckets.Length]; i >= 0; i = mNextIndexes[i])
            {
                if ((mHashCodes[i] == hashCode) && CompareKeys(mKeys[i], key))
                {
                    if (add)
                        throw new InvalidOperationException("A value with the specified key has already been added.");

                    mValues[i] = value;
                    mVersion++;
                    return;
                }
            }

            // Use a free slot (to store the value) if any. Otherwise, resize the dictionary and use a new slot.
            int index;
            if (mFreeCount > 0)
            {
                index = mFreeList;
                mFreeList = mNextIndexes[index];
                mFreeCount--;
            }
            else
            {
                if (mCount == mHashCodes.Length)
                    Resize();

                index = mCount;
                mCount++;
            }

            int bucket = hashCode % mBuckets.Length;
            mHashCodes[index] = hashCode;
            mNextIndexes[index] = mBuckets[bucket];
            mKeys[index] = key;
            mValues[index] = value;
            mBuckets[bucket] = index;
            mVersion++;
        }

        /// <summary>
        /// Resizes the dictionary.
        /// </summary>
        private void Resize()
        {
            // Obtain a new size, allocate resources and move existing data.
            int newSize = MathUtil.GetPrimeForHashtable(mCount * 2);

            int[] newBuckets = new int[newSize];
            ClearBuckets(newBuckets);

            int[] newHashCodes = new int[newSize];
            Array.Copy(mHashCodes, 0, newHashCodes, 0, mCount);

            // Do not copy indexes to next items, since they need to be recalculated anyway.
            int[] newNextIndexes = new int[newSize];

#if PRIMITIVE_KEY
            TKey[] newKeys = new TKey[newSize];
            Array.Copy(mKeys, 0, newKeys, 0, mCount);
            mKeys = newKeys;
#else
            for (int i = mCount; i < newSize; i++)
                mKeys.Add(null);
#endif

#if PRIMITIVE_VALUE
            TValue[] newValues = new TValue[newSize];
            Array.Copy(mValues, 0, newValues, 0, mCount);
            mValues = newValues;
#else
            for (int i = mCount; i < newSize; i++)
                mValues.Add(null);
#endif

            // Reform linked lists, i.e. recalculate buckets and indexes to next items.
            for (int i = 0; i < mCount; i++)
            {
                int bucket = newHashCodes[i] % newSize;
                newNextIndexes[i] = newBuckets[bucket];
                newBuckets[bucket] = i;
            }

            // Assign new values.
            mBuckets = newBuckets;
            mHashCodes = newHashCodes;
            mNextIndexes = newNextIndexes;
        }

        /// <summary>
        /// Removes the value with the specified key from the Dictionary&lt;TKey, TValue&gt;.
        /// Returns a value indicating whether the item was successfully found and removed.
        /// </summary>
        public bool Remove(TKey key)
        {
#if NULLABLE_KEY
            CheckKeyNotNull(key);
#endif
            if (mBuckets != null)
            {
                int hashCode = GetHashCode(key);
                int bucket = hashCode % mBuckets.Length;
                int last = InvalidIndex;

                for (int i = mBuckets[bucket]; i >= 0; last = i, i = mNextIndexes[i])
                {
                    if ((mHashCodes[i] == hashCode) && CompareKeys(mKeys[i], key))
                    {
                        if (last < 0)
                        {
                            mBuckets[bucket] = mNextIndexes[i];
                        }
                        else
                        {
                            mNextIndexes[last] = mNextIndexes[i];
                        }

                        mHashCodes[i] = InvalidIndex;
                        mNextIndexes[i] = mFreeList;
                        mKeys[i] = mDefaultKey;
                        mValues[i] = mDefaultValue;
                        mFreeList = i;
                        mFreeCount++;
                        mVersion++;

                        return true;
                    }
                }
            }

            return false;
        }
#if NULLABLE_KEY

        private static void CheckKeyNotNull(TKey key)
        {
            // Do not use ArgumentUtil.CheckNotNull(), as the key could be of Nullable<T> type in the future,
            // so do not introduce an unneeded boxing.
            if (key == null)
                throw new ArgumentNullException("key");
        }
#endif

        /// <summary>
        /// Returns a hash for the specified key applicable for inner calculations.
        /// </summary>
#if STRING_KEY
        private int GetHashCode(TKey key)
#else
        private static int GetHashCode(TKey key)
#endif
        {
            const int hashCodeMask = 0x7FFFFFFF; // This strips a sign bit.
#if STRING_KEY
            // If character case is not ignored, we will fall to string.GetHashCode() which is ordinal and case-sensitive.
            if (mIgnoreCase)
                return HashCodeProviderPal.GetHashCodeOrdinalIgnoreCase(key) & hashCodeMask;
#endif
#if PRIMITIVE_KEY
            return HashCodeProviderPal.GetHashCode(key) & hashCodeMask;
#else
            return key.GetHashCode() & hashCodeMask;
#endif
        }

        /// <summary>
        /// Returns a value indicating whether the specified keys are equal.
        /// </summary>
#if STRING_KEY
        private bool CompareKeys(TKey key1, TKey key2)
#else
        private static bool CompareKeys(TKey key1, TKey key2)
#endif
        {
#if STRING_KEY
            // If character case is not ignored, we will fall to string.Equals() which is ordinal and case-sensitive.
            if (mIgnoreCase)
                return StringUtil.EqualsOrdinalIgnoreCase(key1, key2);
#endif
#if PRIMITIVE_KEY
            return key1 == key2;
#else
            // A key can not be null for nullable types. Do not ensure this, simply compare. 
#if STRING_KEY
            return key1.Equals(key2, StringComparison.Ordinal);
#else
            return key1.Equals(key2);
#endif
#endif
        }

        /// <summary>
        /// Returns a value indicating whether the specified values are equal.
        /// </summary>
        private static bool CompareValues(TValue value1, TValue value2)
        {
#if PRIMITIVE_VALUE
            return (value1 == value2);
#else
#if NULLABLE_VALUE
            if (value1 == null)
                return value2 == null;
#endif
#if STRING_VALUE
            return value1.Equals(value2, StringComparison.Ordinal);
#else
            return value1.Equals(value2);
#endif
#endif
        }
#if NULLABLE_VALUE
#else

        /// <summary>
        /// Returns null substitute value (i.e. semantic analogue of null for non-nullable value types) defined 
        /// for the class. Always returns null if the type of this dictionary values is a nullable type.
        /// </summary>
        private static TValue GetNullSubstitute()
        {
            throw new NotSupportedException("GetNullSubstitutePlaceholder");
        }
#endif

        /// <summary>
        /// Returns a value indicating whether the specified value is null substitute value (i.e. semantic analogue 
        /// of null for non-nullable value types) defined for the class. See <see cref="this"/> for details.
        /// </summary>
        public static bool IsNullSubstitute(TValue value)
        {
#if NULLABLE_VALUE
            return value == null;
#else
            return CompareValues(value, gNullSubstitute);
#endif
        }

        /// <summary>
        /// Gets the number of key/value pairs contained in the Dictionary&lt;TKey, TValue&gt;.
        /// </summary>
        public int Count
        {
            get { return mCount - mFreeCount; }
        }
#if NULLABLE_KEY

        /// <summary>
        /// Gets an enumeration of the keys in the Dictionary&lt;TKey, TValue&gt;. Do not use this when the key of the dictionary
        /// is of nullable struct type as it will introduce extra boxings, use Dictionary&lt;TKey, TValue&gt;.GetEnumerator() instead.
        /// </summary>
        public IEnumerable<TKey> Keys
        {
            get
            {
                if (mKeyEnumeration == null)
                    mKeyEnumeration = new KeyEnumeration(this);

                return mKeyEnumeration;
            }
        }
#endif
#if NULLABLE_VALUE

        /// <summary>
        /// Gets an enumeration of the values in the Dictionary&lt;TKey, TValue&gt;. Do not use this when the value of the dictionary
        /// is of nullable struct type as it will introduce extra boxings, use Dictionary&lt;TKey, TValue&gt;.GetEnumerator() instead.
        /// </summary>
        public IEnumerable<TValue> Values
        {
            get
            {
                return mValueEnumeration ?? (mValueEnumeration = new ValueEnumeration(this));
            }
        }
#endif

        /// <summary>
        /// Gets or sets the value associated with the specified key. In contrast to built-in .Net class, does not throw, 
        /// if the specified key is not found while getting the value, but returns the null substitute value (i.e. semantic 
        /// analogue of null for non-nullable value types) defined for the class instead. Use <see cref="IsNullSubstitute"/> 
        /// to check whether this is the case. Alternatively, you can use <see cref="ContainsKey"/> to check whether a value 
        /// for the specified key exists in the dictionary before accessing it, but this works slower.
        /// </summary>
        public TValue this[TKey key]
        {
            get
            {
                int i = FindEntry(key);
                return (i >= 0)
                    ? mValues[i]
#if NULLABLE_VALUE
                    : null;
#else
                    : gNullSubstitute;
#endif
            }
            set
            {
                Insert(key, value, false);
            }
        }

        /// <summary>
        /// Represents an enumerator that iterates through the Dictionary&lt;TKey, TValue&gt;. In contrast to built-in .Net class,
        /// does not implement generic IEnumerator interface, since generics are still not supported in AW. Non-generic
        /// IEnumerable could be implemented, but it would lead to extra boxings while iterating, which we should reduce.
        /// By this reason you can not use instances of this class to build foreach loops.
        /// </summary>
        /// <dev>
        /// This was struct in ROTOR sources. Had to make it class, it to be autoportable, because it is mutable.
        /// </dev>
        public class Enumerator
        {
            /// <summary>
            /// Ctor.
            /// </summary>
            internal Enumerator(Dictionary<TKey, TValue> dictionary)
            {
                mDictionary = dictionary;
                mVersion = dictionary.mVersion;
                mIndex = InvalidIndex;
            }

            /// <summary>
            /// Advances the enumerator to the next element of the Dictionary&lt;TKey, TValue&gt;.
            /// </summary>
            public bool MoveNext()
            {
                EnsureVersion();

                // The initial value of index is -1.
                while (++mIndex < mDictionary.mCount)
                {
                    if (mDictionary.mHashCodes[mIndex] >= 0)
                        return true;
                }

                mIndex = InvalidIndex;
                return false;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first element in the Dictionary&lt;TKey, TValue&gt;.
            /// </summary>
            public void Reset()
            {
                EnsureVersion();
                mIndex = InvalidIndex;
            }

            /// <summary>
            /// Checks whether the dictionary was changed while enumerating, throws if it was.
            /// </summary>
            [CppConstMethod]
            private void EnsureVersion()
            {
                if (mVersion != mDictionary.mVersion)
                    throw new InvalidOperationException("The dictionary has been changed, the enumeration can not proceed.");
            }

            /// <summary>
            /// Checks whether the enumeration has been started before accessing current key or value, throws if it was not.
            /// </summary>
            [CppConstMethod]
            private void EnsureIndex()
            {
                if (mIndex == InvalidIndex)
                    throw new InvalidOperationException("An enumeration should be started before accessing current key or value.");
            }

            /// <summary>
            /// Gets the key of the item at the current position of the enumerator.
            /// </summary>
            public TKey CurrentKey
            {
                [CppConstMethod]
                get
                {
                    EnsureIndex();
                    return mDictionary.mKeys[mIndex]; 
                }
            }

            /// <summary>
            /// Gets the value of the item at the current position of the enumerator.
            /// </summary>
            public TValue CurrentValue
            {
                [CppConstMethod]
                get
                {
                    EnsureIndex();
                    return mDictionary.mValues[mIndex];
                }
            }

            private readonly Dictionary<TKey, TValue> mDictionary;
            private readonly int mVersion;
            private int mIndex;
        }
#if NULLABLE_KEY

        /// <summary>
        /// Represents the enumeration of keys in a Dictionary&lt;TKey, TValue&gt;.
        /// </summary>
        private class KeyEnumeration : IEnumerable<TKey>
        {
            /// <summary>
            /// Ctor.
            /// </summary>
            internal KeyEnumeration(Dictionary<TKey, TValue> dictionary)
            {
                mDictionary = dictionary;
            }

            IEnumerator<TKey> IEnumerable<TKey>.GetEnumerator()
            {
                return new KeyEnumerator(mDictionary);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new KeyEnumerator(mDictionary);
            }

            /// <summary>
            /// Enumerates the keys in a Dictionary&lt;TKey, TValue&gt;.
            /// </summary>
            private class KeyEnumerator : IEnumerator<TKey>
            {
                /// <summary>
                /// Ctor.
                /// </summary>
                internal KeyEnumerator(Dictionary<TKey, TValue> dictionary)
                {
                    mEnumerator = dictionary.GetEnumerator();
                }

                bool IEnumerator.MoveNext()
                {
                    return mEnumerator.MoveNext();
                }

                void IEnumerator.Reset()
                {
                    mEnumerator.Reset();
                }

                void IDisposable.Dispose()
                {
                    // Do nothing.
                }

                public TKey Current
                {
                    get { return mEnumerator.CurrentKey; }
                }

                TKey IEnumerator<TKey>.Current
                {
                    [CppConstMethod]
                    get { return Current; }
                }
 
                object IEnumerator.Current
                {
                    get { return Current; }
                }

                private readonly Enumerator mEnumerator;
            }

            private readonly Dictionary<TKey, TValue> mDictionary;
        }
#endif
#if NULLABLE_VALUE

        /// <summary>
        /// Represents the enumeration of values in a Dictionary&lt;TKey, TValue&gt;.
        /// </summary>
        private class ValueEnumeration : IEnumerable<TValue>
        {
            /// <summary>
            /// Ctor.
            /// </summary>
            internal ValueEnumeration(Dictionary<TKey, TValue> dictionary)
            {
                mDictionary = dictionary;
            }

            IEnumerator<TValue> IEnumerable<TValue>.GetEnumerator()
            {
                return new ValueEnumerator(mDictionary);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new ValueEnumerator(mDictionary);
            }

            /// <summary>
            /// Enumerates the values in a Dictionary&lt;TKey, TValue&gt;.
            /// </summary>
            private class ValueEnumerator : IEnumerator<TValue>
            {
                /// <summary>
                /// Ctor.
                /// </summary>
                internal ValueEnumerator(Dictionary<TKey, TValue> dictionary)
                {
                    mEnumerator = dictionary.GetEnumerator();
                }

                bool IEnumerator.MoveNext()
                {
                    return mEnumerator.MoveNext();
                }

                void IEnumerator.Reset()
                {
                    mEnumerator.Reset();
                }

                void IDisposable.Dispose()
                {
                    // Do nothing.
                }

                public TValue Current
                {
                    get { return mEnumerator.CurrentValue; }
                }

                TValue IEnumerator<TValue>.Current
                {
                    [CppConstMethod]
                    get { return Current; }
                }

                object IEnumerator.Current
                {
                    get { return Current; }
                }

                private readonly Enumerator mEnumerator;
            }

            private readonly Dictionary<TKey, TValue> mDictionary;
        }
#endif

        // The original ROTOR sources used Entry struct to store hash codes, indexes to next entries, keys and values.
        // However, there are no structs in Java. So to use the same by-reference semantics, it would be necessary to make
        // Entry struct to be a class in .Net either. But this would slow down the performance as it would require quite a lot
        // of time to initialize Entry instances in the array (which does not occur when dealing with structs). This also
        // would take a lot of time in Java according to my tests. So I decided to use four separate arrays for this purpose,
        // which is even faster than the original ROTOR approach in .Net. It is faster than the class approach in Java either.
        private int[] mBuckets;
        private int[] mHashCodes;
        private int[] mNextIndexes;
#if PRIMITIVE_KEY
        private TKey[] mKeys;
#else
        private List<TKey> mKeys;
#endif
#if PRIMITIVE_VALUE
        private TValue[] mValues;
#else
        private List<TValue> mValues;
#endif
        private int mCount;
        private int mVersion;
        private int mFreeList;
        private int mFreeCount;
#if STRING_KEY
        private bool mIgnoreCase; // This field is not readonly to satisfy Java final.
#endif
#if NULLABLE_KEY
        private KeyEnumeration mKeyEnumeration;
#endif
#if NULLABLE_VALUE
        private ValueEnumeration mValueEnumeration;
#endif

        // The following default values are different in .Net and Java in case of non-primitive structs.
        // However, it does not break anything, as the purpose of these values is to get rid of references
        // to instances of reference types. In other words, these values are not used in calculations.
        // The corresponding member variables are not readonly to satisfy Java final.
        // Note: Java doesn't support static generic fields.
        private readonly TKey mDefaultKey;
        private readonly TValue mDefaultValue;

        // The null substitute value (i.e. semantic analogue of null for non-nullable value types) defined 
        // for the class. Calculate once, use multiple times.
#if NULLABLE_VALUE
#else
        private static readonly TValue gNullSubstitute = GetNullSubstitute();
#endif

        private const int InvalidIndex = -1;
    }
}
#endif
