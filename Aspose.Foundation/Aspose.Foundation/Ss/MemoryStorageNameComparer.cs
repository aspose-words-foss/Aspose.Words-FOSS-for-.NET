// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 27/07/2010 by Roman Korchagin

using System;
using System.Collections.Generic;

namespace Aspose.Ss
{
    /// <summary>
    /// Structured storage elements must be sorted. 
    /// It is very important to sort "correctly" according to the structured storage 
    /// rules, otherwise the produced file will not be valid.
    /// </summary>
    internal class MemoryStorageNameComparer : IComparer<string>
    {
        [CodePorting.Translator.Cs2Cpp.CppForceSharedApi]
        int IComparer<string>.Compare(string s1, string s2)
        {

            // First compare the length, the shorter string is considered smaller.
            int result = s1.Length.CompareTo(s2.Length);
            if (result == 0)
            {
                // Now compare the name itself, but take care.

                // First of all, the compare is case insensitive, but I cannot use 
                // string.Compare because it incorrectly compares some special
                // characters like 0x01, 0x03 and 0x05 that occur in stream names.

                // Also, the underscore character is considered greater than all alpha chars.

                // The simplest solution is to convert all to upper case (not lowercase)
                // and compare ordinal values of the characters.

                // Converting to upper case is important because it ensures that the underscore
                // character is treated as greater than all alpha chars.

                // Fixed WORDSJAVA-858: OLE object cannot be edited after re saving the document.
                // gpv: "ß".ToUpper() gives "ß" in .NET, whereas "ß".ToUpper() gives "SS" in Java.
                // gpv: I use a simple comparison algorithm described in http://msdn.microsoft.com/en-us/library/dd942470.aspx, p.3.
                result = CompareOrdinalUpperCase(s1, s2);
            }
            return result;
        }

        /// <summary>
        /// Comparison algorithm of Directory Entry Names. Described in http://msdn.microsoft.com/en-us/library/dd942470.aspx, p.3.
        /// 1. For strings with the same name length, iterate through each UTF-16 code point, one at a time, from the beginning of the Unicode string.
        /// 2. For each UTF-16 code point, convert to upper-case with the Unicode Default Case Conversion Algorithm
        /// 3. Compare each upper-cased UTF-16 code point binary value.
        /// </summary>
        private static int CompareOrdinalUpperCase(string s1, string s2)
        {
            int len1 = s1.Length;
            int len2 = s2.Length;
            int n = Math.Min(len1, len2);

            int i = 0;
            while (n-- != 0)
            {
                char c1 = char.ToUpperInvariant(s1[i]);
                char c2 = char.ToUpperInvariant(s2[i]);

                if (c1 != c2)
                    return c1 - c2;

                i++;
            }

            return len1 - len2;
        }
    }
}
