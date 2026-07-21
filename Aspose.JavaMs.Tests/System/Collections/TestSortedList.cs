// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 05/03/2009 by Konstantin Sidorenko
// 2016/07/15 by Anatoliy Sidorenko

using System;
using System.Collections;
using Aspose.JavaAttributes;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Collections
{
    [TestFixture]
    public class TestSortedList
    {
        [Test]
        public void TestCtor()
        {
            SortedList list = new SortedList();
            Assert.That(list, IsNot.Null());
            Assert.That(0, Is.EqualTo(list.Count));

            list.Add("cde", null);
            list.Add("abc", null);
            list.Add("bcd", null);

            Assert.That(3, Is.EqualTo(list.Count));
            Assert.That(2, Is.EqualTo(list.IndexOfKey("cde")));
            Assert.That(0, Is.EqualTo(list.IndexOfKey("abc")));
            Assert.That(1, Is.EqualTo(list.IndexOfKey("bcd")));
            Assert.That("abc", Is.EqualTo(list.GetKey(0)));
            Assert.That("bcd", Is.EqualTo(list.GetKey(1)));
            Assert.That("cde", Is.EqualTo(list.GetKey(2)));
        }

        [Test]
        public void TestCtor2()
        {
            SortedList list = new SortedList(100);
            Assert.That(list, IsNot.Null());
            Assert.That(0, Is.EqualTo(list.Count));

            list = new SortedList(0);
            Assert.That(list, IsNot.Null());
            Assert.That(0, Is.EqualTo(list.Count));
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCtorWrongCapacity()
        {
            SortedList list = new SortedList(-1);
            Assert.That(list, IsNot.Null()); // Just a stub to prevent an unused variable hint.
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))] 
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCtorWrongIndex()
        {
            SortedList list = new SortedList();
            object item = list.GetByIndex(-1);
            Assert.That(item, Is.Null);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCtorWrongIndex2()
        {
            SortedList list = new SortedList();
            list.GetByIndex(0);
        }

        [Test]
        public void TestGetByIndex()
        {
            SortedList list = GetPopulatedList();

            string s1;
            for (int i = 0; i < list.Count; i++)
            {
                s1 = "key_" + i;
                string s2 = "val_" + i;
                string s3 = (string)list.GetByIndex(list.IndexOfKey(s1));
                string s4 = (string)list.GetByIndex(list.IndexOfValue(s2));

                Assert.That(s4, Is.EqualTo(s3));
                Assert.That(s2, Is.EqualTo(s3));
            }

            s1 = "key_50";
            list.Remove(s1);
            int j = list.IndexOfKey(s1);
            Assert.That(-1, Is.EqualTo(j));

        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetByWrongIndex1()
        {
            SortedList list = GetPopulatedList();
            list.GetByIndex(list.Count);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetByWrongIndex2()
        {
            SortedList list = GetPopulatedList();
            list.GetByIndex(list.Count + 1);
        }

        [Test]
        public void TestClear()
        {
            SortedList list = new SortedList();
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
            SortedList list = new SortedList();
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
            SortedList list = new SortedList();
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
            SortedList list = new SortedList();
            string s1 = (string)list["No_Such_Key"];
            Assert.That(s1, Is.Null);

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
        [ExpectedException(typeof(ArgumentNullException ))]
        public void TestIndexOfNullKey()
        {
            SortedList list = new SortedList();
            list.IndexOfKey(null);
        }

        [Test]
        public void TestIndexOfKey()
        {
            SortedList list = new SortedList();
            int j = list.IndexOfKey("No_Such_Key");
            Assert.That(-1, Is.EqualTo(j));

            string s1;
            list = GetPopulatedList();
            for (int i = 0; i < list.Count; i++)
            {
                s1 = "key_" + i;
                string s2 = "val_" + i;
                string s3 = (string)list.GetByIndex(list.IndexOfKey(s1));
                string s4 = (string)list.GetByIndex(list.IndexOfValue(s2));

                Assert.That(s4, Is.EqualTo(s3));
                Assert.That(s2, Is.EqualTo(s3));
            }

            s1 = "key_50";
            list.Remove(s1);
            j = list.IndexOfKey(s1);
            Assert.That(-1, Is.EqualTo(j));
        }

        [Test]
        public void TestIndexOfValue()
        {
            SortedList list = new SortedList();
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
                string s3 = (string)list.GetByIndex(list.IndexOfKey(s1));
                string s4 = (string)list.GetByIndex(list.IndexOfValue(s2));

                Assert.That(s4, Is.EqualTo(s3));
                Assert.That(s2, Is.EqualTo(s3));
            }

            s1 = "key_50";
            list.Remove(s1);
            j = list.IndexOfValue(s1);
            Assert.That(-1, Is.EqualTo(j));
        }

        [Test]
        public void TestCount()
        {
            SortedList list = new SortedList();
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
        public void TestClone()
        {
            SortedList list = new SortedList();
            for (int i = 0; i < 10; i++)
                list.Add("Key_" + i, "string_" + i);

            SortedList list2 = (SortedList)list.Clone();
            for (int i = 0; i < 10; i++)
            {
                string s1 = "string_" + i;
                string s2 = "Key_" + i;
                string s3 = (string)list2[s2];
                Assert.That(s3, Is.EqualTo(s1));
            }

            list.Remove("Key_9");
            Assert.That(null, Is.EqualTo(list["Key_9"]));
            Assert.That("string_9", Is.EqualTo(list2["Key_9"]));

            list = new SortedList(1000);
            list2 = (SortedList)list.Clone();
            Assert.That(list2.Count, Is.EqualTo(list.Count));
        }

        [Test]
        public void TestRemove()
        {
            SortedList list = GetPopulatedList(100);
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
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestRemoveNull()
        {
            SortedList list = GetPopulatedList();
            list.Remove(null);
        }

        [Test]
        public void TestRemoveAt()
        {
            SortedList list = GetPopulatedList(100);
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
            SortedList list = GetPopulatedList(100);
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
            SortedList list = new SortedList();
            list.RemoveAt(0);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRemoveAtNegative()
        {
            SortedList list = GetPopulatedList();
            list.RemoveAt(-1);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRemoveAtMaxValue()
        {
            SortedList list = GetPopulatedList();
            list.RemoveAt(int.MaxValue);
        }

        [Test]
        public void TestTrimToSize()
        {
            SortedList list = GetPopulatedList(32);
            list.Add("key_32", "val_32");
            Assert.That(33, Is.EqualTo(list.Count));

            for (int i = 0; i < 10; i++)
                list.Remove("key_" + i);
            Assert.That(23, Is.EqualTo(list.Count));

            list.TrimToSize();
            list.Clear();
            list.TrimToSize();
        }

        [Test]
        public void TestSet()
        {
            SortedList list = new SortedList();
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
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestSetNullKey()
        {
            SortedList list = new SortedList();
            list[null] = "first value";
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetKeyAbsent()
        {
            SortedList list = new SortedList();
            list.GetKey(0);
        }

        [Test]
        public void TestGetKey()
        {
            SortedList list = new SortedList();
            list["first key"] = null;
            Assert.That(1, Is.EqualTo(list.Count));

            list.Clear();
            for (int i = 0; i < 50; i++)
                list.Add("key_" + i, "val_" + i);
            for (int i = 0; i < 50; i++)
            {
                string s1 = "key_" + i;
                string s2 = (string)list.GetKey(list.IndexOfKey(s1));
                Assert.That(s2, Is.EqualTo(s1));
            }
            Assert.That(50, Is.EqualTo(list.Count));
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
            SortedList list = new SortedList();
            Assert.That(defaultCapacity, Is.EqualTo(list.Capacity));

            list.Capacity = 3;
            Assert.That(3, Is.EqualTo(list.Capacity));
            
            list.Capacity = 0;
            Assert.That(defaultCapacity, Is.EqualTo(list.Capacity));

            list.Capacity = 5000;
            Assert.That(5000, Is.EqualTo(list.Capacity));
        }

        private SortedList GetPopulatedList()
        {
            int size = 100;
            return GetPopulatedList(size);
        }

        private SortedList GetPopulatedList(int size)
        {
            SortedList sl = new SortedList();
            for (int i = 0; i < size; i++)
                sl.Add("key_" + i, "val_" + i);

            return sl;
        }
    }
}
