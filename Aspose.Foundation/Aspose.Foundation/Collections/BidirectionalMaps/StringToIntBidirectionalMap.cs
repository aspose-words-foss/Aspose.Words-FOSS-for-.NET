// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/09/2013 by Ivan Lyagin

namespace Aspose.Collections
{
    /// <summary>
    /// Represents a bidirectional map of string-to-integer and integer-to-string value pairs.
    /// </summary>
    public class StringToIntBidirectionalMap
    {
        /// <summary>
        /// Initializes a case-sensitive <see cref="StringToIntBidirectionalMap"/> instance.
        /// </summary>
        public StringToIntBidirectionalMap()
            : this(true)
        {
        }

        /// <summary>
        /// Initializes a case-sensitive or case-insensitive <see cref="StringToIntBidirectionalMap"/> instance.
        /// </summary>
        public StringToIntBidirectionalMap(bool isCaseSensitive)
        {
            mFirstDictionary = new StringToIntDictionary(isCaseSensitive);
            mSecondDictionary = new IntToObjDictionary<string>();
        }

        /// <summary>
        /// Adds corresponding string-to-integer and integer-to-string value pairs to the map.
        /// </summary>
        public void AddEntry(string stringValue, int int32Value)
        {
            mFirstDictionary.Add(stringValue, int32Value);
            mSecondDictionary.Add(int32Value, stringValue);
        }

        /// <summary>
        /// Adds corresponding string-to-integer and integer-to-string value pairs to the map.
        /// </summary>
        public void AddEntry(int int32Value, string stringValue)
        {
            AddEntry(stringValue, int32Value);
        }

        /// <summary>
        /// Returns an integer value associated with the given string value. Throws, if it is not found.
        /// </summary>
        public int GetValue(string key)
        {
            return GetValue(key, NullSubstitute);
        }

        /// <summary>
        /// Returns an integer value associated with the given string value. If it is not found, returns
        /// the given default value. But if the default value is equal to <see cref="int.MinValue"/>, throws.
        /// </summary>
        public int GetValue(string key, int defaultValue)
        {
            int value = TryGetValue(key);
            return BidirectionalMapUtil.ValidateValue(value, defaultValue);
        }

        /// <summary>
        /// Returns an integer value associated with the given string value. If it is not found, returns
        /// the null substitute value. Use the <see cref="IsNullSubstitute"/> method to check the returned value.
        /// </summary>
        public int TryGetValue(string key)
        {
            if (key == null)
                return NullSubstitute;

            int value = mFirstDictionary[key];
            return StringToIntDictionary.IsNullSubstitute(value) ? NullSubstitute : value;
        }

        /// <summary>
        /// Returns a string value associated with the given integer value. Throws, if it is not found.
        /// </summary>
        public string GetValue(int key)
        {
            return GetValue(key, null);
        }

        /// <summary>
        /// Returns a string value associated with the given integer value. If it is not found, returns
        /// the given default value. But if the default value is <b>null</b>, throws.
        /// </summary>
        public string GetValue(int key, string defaultValue)
        {
            string value = TryGetValue(key);
            return BidirectionalMapUtil.ValidateValue(value, defaultValue);
        }

        /// <summary>
        /// Returns a string value associated with the given integer value. If it is not found, returns
        /// <c>null</c>.
        /// </summary>
        public string TryGetValue(int key)
        {
            if (IsNullSubstitute(key))
                return null;

            return (mSecondDictionary.ContainsKey(key))
                ? mSecondDictionary[key]
                : null;
        }

        /// <summary>
        /// Returns a value indicating whether the specified value is null substitute value (i.e. semantic analogue 
        /// of null for non-nullable value types) defined for the class. 
        /// </summary>
        public static bool IsNullSubstitute(int value)
        {
            return value == NullSubstitute;
        }

        /// <summary>
        /// Gets a number of entries in this map.
        /// </summary>
        public int Count
        {
            get { return mFirstDictionary.Count; }
        }

        /// <summary>
        /// Shows if <see cref="StringToIntBidirectionalMap"/> contains an integer value.
        /// </summary>
        public bool ContainsValue(int value) 
        {
            return mFirstDictionary.ContainsValue(value);
        }
        
        private readonly StringToIntDictionary mFirstDictionary;
        private readonly IntToObjDictionary<string> mSecondDictionary;

        /// <summary>
        /// The null substitute value (i.e. semantic analogue of null for non-nullable value types) defined
        /// for the class.
        /// </summary>
        private const int NullSubstitute = int.MinValue;
    }
}
