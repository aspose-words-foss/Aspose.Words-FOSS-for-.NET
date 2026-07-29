// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/29/2014 by Alexey Noskov

using System;
using Aspose.Collections;

namespace Aspose.Words.RW.Dml
{
    internal static class DmlEnumUtil
    {
        internal static StringToIntBidirectionalMap InitHashtableWithValues(string[] keys, int[] values)
        {
            if (keys.Length != values.Length)
                throw new ArgumentException("Length of keys and values arrays must be the same.");

            StringToIntBidirectionalMap dictionary = new StringToIntBidirectionalMap();

            for (int i = 0; i < keys.Length; i++)
                dictionary.AddEntry(keys[i], values[i]);

            return dictionary;
        }
    }
}
