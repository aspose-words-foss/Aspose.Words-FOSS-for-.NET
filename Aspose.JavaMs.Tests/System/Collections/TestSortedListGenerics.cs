// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 06/03/2009 by Konstantin Sidorenko
// 2016/07/21 by Anatoliy Sidorenko

using System;
using System.Collections.Generic;
using Aspose.JavaAttributes;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Collections
{
    [TestFixture]
    // In .Net generic SortedList do not support getByIndex(int), getKey(int) and clone()
    public class TestSortedListGenerics
    {
        [Test]
        public void TestCtor()
        {

            SortedList<string, string> list = new SortedList<string, string>();
            Assert.That(list, IsNot.Null());
            Assert.That(0, Is.EqualTo(list.Count));

            list.Add("cde", null);
            list.Add("abc", null);
            list.Add("bcd", null);

            Assert.That(3, Is.EqualTo(list.Count));
            Assert.That(2, Is.EqualTo(list.IndexOfKey("cde")));
            Assert.That(0, Is.EqualTo(list.IndexOfKey("abc")));
            Assert.That(1, Is.EqualTo(list.IndexOfKey("bcd")));
        }

        [Test]
        public void TestCtor2()
        {
            SortedList<string, string> list = new SortedList<string, string>(100);
            Assert.That(list, IsNot.Null());
            Assert.That(0, Is.EqualTo(list.Count));

            list = new SortedList<string, string>(0);
            Assert.That(list, IsNot.Null());
            Assert.That(0, Is.EqualTo(list.Count));
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCtorWrongCapacity()
        {
            SortedList<string, string> list = new SortedList<string, string>(-1);
            Assert.That(list, IsNot.Null()); // Just to prevent unused variable hint.
        }

        [Test]
        public void TestClear()
        {
            SortedList<string, string> list = new SortedList<string, string>();
            Assert.That(0, Is.EqualTo(list.Count));

            list.Clear();
            Assert.That(0, Is.EqualTo(list.Count));

            list = GetPopulatedList();
            Assert.That(100, Is.EqualTo(list.Count));

            list.Clear();
            Assert.That(0, Is.EqualTo(list.Count));
        }

        [Test]
        public void TestContainsKey()
        {
            SortedList<string, string> list = new SortedList<string, string>();
            bool contains = list.ContainsKey("No_Such_Key");
            Assert.That(false, Is.EqualTo(contains));

            string s1;
            list = GetPopulatedList();
            for (int i = 0; i < list.Count; i++)
            {
                s1 = "key_" + i;
                contains = list.ContainsKey(s1);
                Assert.That(true, Is.EqualTo(contains));
            }

            s1 = "key_50";
            list.Remove(s1);
            contains = list.ContainsKey(s1);
            Assert.That(false, Is.EqualTo(contains));
        }

        [Test]
        public void TestContainsValue()
        {
            SortedList<string, string> list = new SortedList<string, string>();
            bool contains = list.ContainsValue("No_Such_Value");
            Assert.That(false, Is.EqualTo(contains));

            string s1;
            list = GetPopulatedList();
            for (int i = 0; i < list.Count; i++)
            {
                s1 = "val_" + i;
                contains = list.ContainsValue(s1);
                Assert.That(true, Is.EqualTo(contains));
            }

            s1 = "key_50";
            string s2 = "val_50";
            list.Remove(s1);
            contains = list.ContainsValue(s2);
            Assert.That(false, Is.EqualTo(contains));
        }

        [Test]
        public void TestGet()
        {
            string s1;
            SortedList<string, string> list = new SortedList<string, string>();
            try
            {
                s1 = list["No_Such_Key"];
                Assert.That(s1, Is.Null);
            }
            catch (KeyNotFoundException)
            {
                // In .Net generic SortedList throws KeyNotFoundException.
                // Java throws nothing, just returns null.
            }


            list = GetPopulatedList();
            for (int i = 0; i < list.Count; i++)
            {
                s1 = "key_" + i;
                string s2 = "val_" + i;
                string s3 = (string)list[s1];
                Assert.That(s3, Is.EqualTo(s2));
            }

            s1 = "key_50";
            list.Remove(s1);
            bool contains = list.ContainsKey(s1);
            Assert.That(false, Is.EqualTo(contains));
        }

        [Test]
        [JavaExpectedException(typeof(NullPointerException))]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestIndexOfNullKey()
        {
            SortedList<string, string> list = new SortedList<string, string>();
            list.IndexOfKey(null);
        }

        [Test]
        public void TestIndexOfKey()
        {
            SortedList<string, string> list = new SortedList<string, string>();
            int j = list.IndexOfKey("No_Such_Key");
            Assert.That(-1, Is.EqualTo(j));

            string s1;
            list = GetPopulatedList();
            for (int i = 0; i < list.Count; i++)
            {
                s1 = "key_" + i;
                string s2 = "val_" + i;

                Assert.That(list.IndexOfValue(s2), Is.EqualTo(list.IndexOfKey(s1)));
            }

            s1 = "key_50";
            list.Remove(s1);
            j = list.IndexOfKey(s1);
            Assert.That(-1, Is.EqualTo(j));
        }

        [Test]
        public void TestIndexOfValue()
        {
            SortedList<string, string> list = new SortedList<string, string>();
            int j = list.IndexOfValue(null);
            Assert.That(-1, Is.EqualTo(j));
            j = list.IndexOfValue("No_Such_Value");
            Assert.That(-1, Is.EqualTo(j));

            list.Add("Key_0", null);
            j = list.IndexOfValue(null);
            Assert.That(0, Is.EqualTo(j));

            list.Add("Key_1", "Val_Same");
            list.Add("Key_2", "Val_Same");
            j = list.IndexOfValue("Val_Same");
            Assert.That(1, Is.EqualTo(j));

            string s1;
            list = GetPopulatedList();
            for (int i = 0; i < list.Count; i++)
            {
                s1 = "key_" + i;
                string s2 = "val_" + i;

                Assert.That(list.IndexOfValue(s2), Is.EqualTo(list.IndexOfKey(s1)));
            }

            s1 = "key_50";
            list.Remove(s1);
            j = list.IndexOfValue(s1);
            Assert.That(-1, Is.EqualTo(j));
        }

        [Test]
        public void TestCount()
        {
            SortedList<string, string> list = new SortedList<string, string>();
            Assert.That(0, Is.EqualTo(list.Count));

            list["first key"] = null;
            Assert.That(1, Is.EqualTo(list.Count));

            list["first key"] = "first value";
            Assert.That(1, Is.EqualTo(list.Count));

            list.Clear();
            Assert.That(0, Is.EqualTo(list.Count));

            list = GetPopulatedList(50);
            Assert.That(50, Is.EqualTo(list.Count));

            for (int i = 0; i < 100; i++)
                list["key_" + i] = "val_" + i;
            Assert.That(100, Is.EqualTo(list.Count));

            list.Clear();
            Assert.That(0, Is.EqualTo(list.Count));
        }

        [Test]
        public void TestRemove()
        {
            SortedList<string, string> list = GetPopulatedList(100);
            Assert.That(100, Is.EqualTo(list.Count));

            for (int i = 0; i < 100; i++)
            {
                string s1 = "key_" + i;
                list.Remove(s1);
                int j = 100 - (i + 1);
                Assert.That(j, Is.EqualTo(list.Count));
            }

            list.Remove("No_Such_Key");//shouldn't throw
        }

        [Test]
        [JavaExpectedException(typeof(NullPointerException))]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestRemoveNull()
        {
            SortedList<string, string> list = GetPopulatedList();
            list.Remove(null);
        }

        [Test]
        public void TestRemoveAt()
        {
            SortedList<string, string> list = GetPopulatedList(100);
            Assert.That(100, Is.EqualTo(list.Count));

            for (int i = 0; i < 100; i++)
            {
                list.RemoveAt(99 - i);
                int j = 100 - (i + 1);
                Assert.That(j, Is.EqualTo(list.Count));
            }
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRemoveAtEmpty()
        {
            SortedList<string, string> list = GetPopulatedList(100);
            Assert.That(100, Is.EqualTo(list.Count));

            for (int i = 0; i < 100; i++)
            {
                list.RemoveAt(99 - i);
                int j = 100 - (i + 1);
                Assert.That(j, Is.EqualTo(list.Count));
            }

            list.RemoveAt(0);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRemoveAtEmpty2()
        {
            SortedList<string, string> list = new SortedList<string, string>();
            list.RemoveAt(0);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRemoveAtNegative()
        {
            SortedList<string, string> list = GetPopulatedList();
            list.RemoveAt(-1);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRemoveAtMaxValue()
        {
            SortedList<string, string> list = GetPopulatedList();
            list.RemoveAt(int.MaxValue);
        }

        [Test]
        public void TestTrimToSize()
        {
            SortedList<string, string> list = GetPopulatedList(32);
            list.Add("key_32", "val_32");
            Assert.That(33, Is.EqualTo(list.Count));

            for (int i = 0; i < 10; i++)
                list.Remove("key_" + i);
            Assert.That(23, Is.EqualTo(list.Count));

            list.TrimExcess();
            list.Clear();
            list.TrimExcess();
        }

        [Test]
        public void TestSet()
        {
            SortedList<string, string> list = new SortedList<string, string>();
            list["first key"] = null;
            Assert.That(1, Is.EqualTo(list.Count));

            list["0"] = "first value";
            Assert.That(2, Is.EqualTo(list.Count));

            Assert.That(list.ContainsKey("first key"), Is.True);

            list["first key"] = "first value";
            Assert.That("first value", Is.EqualTo(list["first key"]));

            list["first key"] = "second value";
            Assert.That(2, Is.EqualTo(list.Count));
            Assert.That("second value", Is.EqualTo(list["first key"]));

            list.Clear();
            for (int i = 0; i < 50; i++)
                list.Add("key_" + i, "val_" + i);
            for (int i = 0; i < 50; i++)
                list["key_" + i] = "new_val_" + i;
            Assert.That(50, Is.EqualTo(list.Count));
            for (int i = 0; i < 50; i++)
                Assert.That("new_val_" + i, Is.EqualTo(list["key_" + i]));
        }

        [Test]
        [JavaExpectedException(typeof(NullPointerException))]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetNullKey()
        {
            SortedList<string, string> list = new SortedList<string, string>();
            list[null] = "first value";
        }

        [Test]
        public void TestGetCapacity()
        {
            //JAVA-changed default capacity from 0 to 16. 
            int defaultCapacity =
#if JAVA
            16;     
#else
            0;
#endif
            SortedList<string, string> list = new SortedList<string, string>();
            Assert.That(defaultCapacity, Is.EqualTo(list.Capacity));

            list.Capacity = 3;
            Assert.That(3, Is.EqualTo(list.Capacity));

            list.Capacity = 0;
            Assert.That(defaultCapacity, Is.EqualTo(list.Capacity));//JAVA-changed since default capacity in java is 16.

            list.Capacity = 5000;
            Assert.That(5000, Is.EqualTo(list.Capacity));
        }

        private SortedList<string, string> GetPopulatedList()
        {
            int size = 100;
            return GetPopulatedList(size);
        }

        private static SortedList<string, string> GetPopulatedList(int size)
        {
            SortedList<string, string> sl = new SortedList<string, string>();
            for (int i = 0; i < size; i++)
                sl.Add("key_" + i, "val_" + i);
            return sl;
        }
    }
}
