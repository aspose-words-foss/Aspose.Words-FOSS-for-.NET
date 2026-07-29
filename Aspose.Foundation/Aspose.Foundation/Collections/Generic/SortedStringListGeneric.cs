// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/10/2008 by Roman Korchagin

using System.Collections.Generic;

namespace Aspose.Collections.Generic
{
    /// <summary>
    /// Use this class instead of <see cref="SortedList{TKey,TValue}"/> whenever you need a sorted list with string keys.
    /// This class simplifies creation and ensures that sort order is same as in Java.
    /// </summary>
    public class SortedStringListGeneric<TValue> : SortedList<string, TValue>
    {
        /// <summary>
        /// Creates a case-sensitive sorted list.
        /// </summary>
        public SortedStringListGeneric()
            : this(true)
        {
        }

        /// <summary>
        /// Creates a sorted list with a specific comparator.
        /// </summary>
        public SortedStringListGeneric(IComparer<string> comparer) : base(comparer) { }

        /// <summary>
        /// Creates a case-sensitive or case-insensitive string list.
        /// </summary>
        public SortedStringListGeneric(bool isCaseSensitive) :
            base(((isCaseSensitive) ? (IComparer<string>)StringOrdinalComparer.Default : StringOrdinalIgnoreCaseComparer.Default))
        {
        }

        public void AddRange(SortedStringListGeneric<TValue> other)
        {
            foreach (KeyValuePair<string, TValue> item in (SortedList<string, TValue>)other)
            {
                Add(item.Key, item.Value);
            }
        }
        
        public void SetByIndex(int index, TValue value)
        {
            string key = Keys[index];
            this[key] = value;
        }
        
        public TValue GetByIndex(int index)
        {
            return Values[index];
        }

        public TValue GetSafe(string key, TValue defaultValue)
        {
            if (ContainsKey(key))
                return this[key];

            return defaultValue;
        }
        
        public string GetKey(int index)
        {
            return Keys[index];
        }
    }
}
