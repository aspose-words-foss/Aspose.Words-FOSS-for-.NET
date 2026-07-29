// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 14/03/2009 by Konstantin Sidorenko

using System.Collections;
using System.Collections.Generic;

namespace Aspose
{
    /// <summary>
    /// A complementary class to <see cref="StringOrdinalComparer"/>.
    /// Use this class when you need to create a <see cref="SortedList"/> with case-insensitive 
    /// string keys and you will export this list to document formats that have text golds.
    /// </summary>
    public class StringOrdinalIgnoreCaseComparer : IComparer<string>
    {
        /// <summary>
        /// Use the singleton instance instead.
        /// </summary>
        private StringOrdinalIgnoreCaseComparer()
        {
        }

        public int Compare(string x, string y)
        {
            return StringUtil.CompareOrdinalIgnoreCase(x, y);
        }

        /// <summary>
        /// Do not create instances of this class. Use this instance instead.
        /// </summary>
        public static readonly StringOrdinalIgnoreCaseComparer Default = new StringOrdinalIgnoreCaseComparer();
    }
}
