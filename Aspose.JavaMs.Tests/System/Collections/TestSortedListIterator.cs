// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 07/03/2009 by Konstantin Sidorenko
// 2016/09/09 by Anatoliy Sidorenko

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Collections
{
    [TestFixture]
    public class TestSortedListIterator
    {

        [Test]
        public void TestEmpty()
        {
            SortedList<string, int> list = new SortedList<string, int>();
            foreach (KeyValuePair<string, int> entry in list)
                Assert.Fail();

            list = getPopulated();
            list.Clear();
            foreach (KeyValuePair<string, int> entry in list)
                Assert.Fail();
        }

        [Test]
        public void TestIteration()
        {
            SortedList<string, int> list = getPopulated();

            int i = 0;
            foreach (KeyValuePair<string, int> entry in list)
            {
#if JAVA
                Assert.That(keys[i], Is.EqualTo(entry.Key));
                Assert.That(values[i], Is.EqualTo(entry.Value));
#else
                Assert.That(Net_keys[i], Is.EqualTo(entry.Key));
                Assert.That(Net_values[i], Is.EqualTo(entry.Value));
#endif
                i++;
            }
        }

        /**
         * Try to iterate SortedList with Map.Entry as users will do this.
         */
        [Test]
        public void TestIterationMapEntry()
        {
            SortedList<string, int> list = getPopulated();
#if JAVA
            int i = 0;
            for (java.util.Map.Entry entry : list)
            {
                msAssert.areEqual(entry.getKey(), keys[i]);
                msAssert.areEqual(entry.getValue(), values[i]);
                i++;
            }
#endif
        }

        //@todo 1 sk rework with new stringOrdinalIgnoreCaseComparer
        [Test]
        public void TestComparers()
        {
            SortedList<string, int> list = new SortedList<string, int>();
            addAll(list);
            Debug.WriteLine("default(case-sensitive): " + ListToString(list));

#if JAVA
            list = new SortedList<string, int>(String.CASE_INSENSITIVE_ORDER);
		    setAll(list);
		    Debug.WriteLine("java case-insensitive: " + ListToString(list));
#endif

            list = new SortedList<string, int>(new CaseInsensitiveInvariantComparator());
            setAll(list);
            Debug.WriteLine("Our case-insensitive comparator: " + ListToString(list));
        }

        private string ListToString(SortedList<string, int> list)
        {
            StringBuilder builder = new StringBuilder("\r\n");
            foreach (KeyValuePair<string, int> entry in list)
            {
                builder.Append(entry.Key).Append(" => ").Append(entry.Value).Append("\r\n");
            }
            return builder.ToString();
        }

        private class CaseInsensitiveInvariantComparator : IComparer<string>
        {
            public int Compare(String s1, String s2)
            {
                //Object equality and null string comparision used only in .Net. Taken from Rotor's String.Compare():
                //Our (.Net) paradigm is that null sorts less than any other string and
                //that two nulls sort as equal. (plus: two equal constant strings always stored in a single object).
                if ((Object)s1 == (Object)s2)
                    return 0;
                if (s1 == null)
                    return (-1);    // null < non-null
                if (s2 == null)
                    return (1);     // non-null > null

                int n1 = s1.Length, n2 = s2.Length;
                for (int i1 = 0, i2 = 0; i1 < n1 && i2 < n2; i1++, i2++)
                {
                    char c1 = s1[i1];
                    char c2 = s2[i2];

                    //Case-insensitive comparison of ascii chars is simple (taken from Rotor COMString::CaseInsensitiveCompHelper).
                    if ((c1 | c2) <= 0x7F)
                    {
                        // uppercase both chars.
                        if (c1 >= 'a' && c1 <= 'z')
                            c1 = char.ToUpper(c1);
                        if (c2 >= 'a' && c2 <= 'z')
                            c2 = char.ToUpper(c2);

                        //Return the (case-insensitive) difference between them.
                        if (c1 != c2)
                            return c1 - c2;
                    }
                    //Non-ascii chars comparision. Added for compatibility. Code taken from java.lang.String.CaseInsensitiveComparator.
                    else
                    {
                        if (c1 != c2)
                        {
                            c1 = char.ToUpper(c1);
                            c2 = char.ToUpper(c2);
                            if (c1 != c2)
                            {
                                c1 = char.ToLower(c1);
                                c2 = char.ToLower(c2);
                                if (c1 != c2)
                                    return c1 - c2;
                            }
                        }
                    }
                }

                return n1 - n2;
            }
        }

        private SortedList<string, int> getPopulated()
        {
            SortedList<string, int> sl = new SortedList<string, int>();
            addAll(sl);

            return sl;
        }

        private void addAll(SortedList<string, int> sl)
        {
            for (int i = 0; i < keys.Length; i++)
                sl.Add(keys[i], values[i]);
        }

        private void setAll(SortedList<string, int> sl)
        {
            for (int i = 0; i < keys.Length; i++)
                sl[keys[i]] = values[i];
        }

        private string[] keys = { "0abc", "0абв", "ABC", "Abc", "Bcd", "_abc", "_абв", "abc", "abc0", "abc_", "bcd", "bcd0", "АБВ", "Абв", "БВГ", "Бвг", "абв", "бвг" };
        private int[] values = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 };

        private string[] Net_keys = { "_abc", "_абв", "0abc", "0абв", "abc", "Abc", "ABC", "abc_", "abc0", "bcd", "Bcd", "bcd0", "абв", "Абв", "АБВ", "бвг", "Бвг", "БВГ" };
        private int[] Net_values = { 5, 6, 0, 1, 7, 3, 2, 9, 8, 10, 4, 11, 16, 13, 12, 17, 15, 14 };
    }
}
