// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/11/2022 by amorozov

using System;
using Aspose.Collections.Generic;

namespace Aspose.Words
{
    /// <summary>
    /// Implements few optimization hacks to increase performance of FindReplace and Comparison features.
    /// </summary>
    internal class NodeIndex : SortedIntegerListGeneric<Node>
    {
        /// <summary>
        /// Do low-level keys shift.
        /// </summary>
        /// <remarks>
        /// Adds given value for all keys starting from given.
        /// </remarks>
        internal void Shift(int key, int len)
        {
            int index = IndexOfKey(key);

            if (index != -1)
                for (int i = index; i < Count; i++)
                    Keys[i] += len;
        }

        /// <summary>
        /// Replaces entry with new key/value pair.
        /// </summary>
        public void Replace(int key, int newKey, Node value)
        {
            int index = IndexOfKey(key);

            if(index == -1)
                throw new ArgumentOutOfRangeException("key");

            // Previous key must be less than new key.
            if ((index > 0) && (Keys[index - 1] >= newKey))
                throw new ArgumentOutOfRangeException("newKey");

            // Next key must be greater than new key.
            if ((index < Count - 1) && (Keys[index + 1] <= newKey))
                throw new ArgumentOutOfRangeException("newKey");

            Keys[index] = newKey;
            Values[index] = value;
        }
    }
}
