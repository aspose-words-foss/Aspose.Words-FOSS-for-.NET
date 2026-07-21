// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 02/10/2008 by Roman Korchagin

using System.Collections;
using System.Collections.Generic;

namespace Aspose
{
    /// <summary>
    /// You need to use this comparer when all of the following is true:
    /// 1. You need to create a <see cref="SortedList"/> with case-sensitive string keys.
    /// 2. You will export this list to document formats that have text golds.
    /// 
    /// This comparer implements string compare using Unicode character values exactly the way strings 
    /// are sorted in Java. If you don't use this comparer, then golds in .NET and Java could be different 
    /// because .NET default string sorting algorithm is different (it looks more like case-insensitive).
    /// 
    /// <seealso cref="StringOrdinalIgnoreCaseComparer"/>
    /// </summary>
    public class StringOrdinalComparer : IComparer<string>
    {
        /// <summary>
        /// Use the singleton instance instead.
        /// </summary>
        private StringOrdinalComparer()
        {
        }

        public int Compare(string x, string y)
        {
            return string.CompareOrdinal(x, y);
        }

        /// <summary>
        /// Do not create instances of this class. Use this instance instead.
        /// </summary>
        public static readonly StringOrdinalComparer Default = new StringOrdinalComparer();
    }
}
