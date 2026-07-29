// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 16/09/2013 by Ivan Lyagin

namespace Aspose.Collections
{
    /// <summary>
    /// Represents a bidirectional map of integer-to-integer direct and indirect value pairs.
    /// </summary>
    public class IntToIntBidirectionalMap
    {
        /// <summary>
        /// Adds corresponding integer-to-integer direct and indirect value pairs to the map.
        /// </summary>
        public void AddEntry(int value1, int value2)
        {
            mDirectMap.Add(value1, value2);
            mIndirectMap.Add(value2, value1);
        }

        /// <summary>
        /// Gets value from the direct mapping or sets value to direct mapping for the specified key
        /// and vise versa for indirect one.
        /// </summary>
        public int this[int key]
        {
            get { return TryGetValueDirect(key); }
            set 
            {
                mDirectMap[key] =  value;
                mIndirectMap[value] = key;
            }
        }

        /// <summary>
        /// Returns an integer value associated with the given integer value using direct mappings. Throws, if it is not found.
        /// </summary>
        public int GetValueDirect(int key)
        {
            return GetValue(mDirectMap, key, int.MinValue);
        }

        /// <summary>
        /// Returns an integer value associated with the given integer value using direct mappings. If it is not found, 
        /// returns the given default value. But if the default value is <see cref="int.MinValue"/>, throws.
        /// </summary>
        public int GetValueDirect(int key, int defaultValue)
        {
            return GetValue(mDirectMap, key, defaultValue);
        }

        /// <summary>
        /// Returns an integer value associated with the given integer value using direct mappings. If it is not found, 
        /// returns <see cref="int.MinValue"/>.
        /// </summary>
        public int TryGetValueDirect(int key)
        {
            return TryGetValue(mDirectMap, key);
        }

        /// <summary>
        /// Returns an integer value associated with the given integer value using indirect mappings. Throws, if it is not found.
        /// </summary>
        public int GetValueIndirect(int key)
        {
            return GetValue(mIndirectMap, key, int.MinValue);
        }

        /// <summary>
        /// Returns an integer value associated with the given integer value using indirect mappings. If it is not found, 
        /// returns the given default value. But if the default value is <see cref="int.MinValue"/>, throws.
        /// </summary>
        public int GetValueIndirect(int key, int defaultValue)
        {
            return GetValue(mIndirectMap, key, defaultValue);
        }

        /// <summary>
        /// Returns an integer value associated with the given integer value using indirect mappings. If it is not found, 
        /// returns <see cref="int.MinValue"/>.
        /// </summary>
        public int TryGetValueIndirect(int key)
        {
            return TryGetValue(mIndirectMap, key);
        }

        private static int GetValue(IntToIntDictionary map, int key, int defaultValue)
        {
            int value = TryGetValue(map, key);
            return BidirectionalMapUtil.ValidateValue(value, defaultValue);
        }

        private static int TryGetValue(IntToIntDictionary map, int key)
        {
            if (key == int.MinValue)
                return int.MinValue;

            int value = map[key];
            return IntToIntDictionary.IsNullSubstitute(value) ? int.MinValue : value;
        }

        private readonly IntToIntDictionary mDirectMap = new IntToIntDictionary();
        private readonly IntToIntDictionary mIndirectMap = new IntToIntDictionary();
    }
}
