// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/09/2013 by Ivan Lyagin

using System;
using System.Collections.Generic;

namespace Aspose.Collections
{
    /// <summary>
    /// Represents a bidirectional map of string key-to-value and value-to-string key value pairs.
    /// </summary>
    public class StringToObjBidirectionalMap<TValue>
        where TValue : class
    {
        /// <summary>
        /// Initializes a case-sensitive <see cref="StringToObjBidirectionalMap{TValue}"/> instance.
        /// </summary>
        public StringToObjBidirectionalMap()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a case-sensitive or case-insensitive <see cref="StringToObjBidirectionalMap{TValue}"/> instance.
        /// </summary>
        public StringToObjBidirectionalMap(bool isCaseSensitive)
        {
            mKeyToValueDictionary = new StringToObjDictionary<TValue>(isCaseSensitive);
            mValueToKeyDictionary = new Dictionary<TValue, string>();
        }

        /// <summary>
        /// Adds corresponding string key-to-value and value-to-string key value pairs to the map.
        /// </summary>
        public void AddEntry(string key, TValue value)
        {
            mKeyToValueDictionary.Add(key, value);
            mValueToKeyDictionary.Add(value, key);
        }

        /// <summary>
        /// Returns a value associated with the given string key. Throws, if it is not found.
        /// </summary>
        public TValue GetValue(string key)
        {
            TValue value;
            if (!TryGetValue(key, out value))
                throw new InvalidOperationException("A value for the specified key is missed.");

            return value;
        }

        /// <summary>
        /// Returns a value associated with the given key. If it is not found, returns
        /// the given default value. But if the default value is <c>null</c>, throws.
        /// </summary>
        public TValue GetValue(string key, TValue defaultValue)
        {
            TValue value;
            if (TryGetValue(key, out value))
                return value;

            if (defaultValue == null)
                throw new InvalidOperationException(
                    "A value for the specified key is missed and the default value is not provided.");

            return defaultValue;
        }

        /// <summary>
        /// Returns a string key associated with the given value. Throws, if it is not found.
        /// </summary>
        public string GetKey(TValue value)
        {
            string key;
            if (!TryGetKey(value, out key))
                throw new InvalidOperationException("A key for the specified value is missed.");

            return key;
        }

        /// <summary>
        /// Returns a string key associated with the given value. If it is not found, returns
        /// the given default value. But if the default value is <c>null</c>, throws.
        /// </summary>
        public string GetKey(TValue value, string defaultKey)
        {
            string key;
            if (TryGetKey(value, out key))
                return key;

            if (defaultKey == null)
                throw new InvalidOperationException(
                    "A key for the specified value is missed and the default key value is not provided.");

            return defaultKey;
        }

        private bool TryGetValue(string key, out TValue value)
        {
            if ((key != null) && mKeyToValueDictionary.ContainsKey(key))
            {
                value = mKeyToValueDictionary[key];
                return true;
            }

            value = null;
            return false;
        }

        private bool TryGetKey(TValue value, out string key)
        {
            key = (value != null)
                ? mValueToKeyDictionary.GetValueOrNull(value)
                : null;
            return key != null;
        }

        private readonly StringToObjDictionary<TValue> mKeyToValueDictionary;
        private readonly Dictionary<TValue, string> mValueToKeyDictionary;
    }
}
