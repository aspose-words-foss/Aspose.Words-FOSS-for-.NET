// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 29/08/2013 by Ivan Lyagin

#if INCLUDE_FILE
using System;
using Aspose.Collections;
using NUnit.Framework;
using CodePorting.Translator.Cs2Cpp;
using Aspose.JavaAttributes;

namespace Aspose.Tests.Collections.Auto
{
    /// <summary>
    /// Tests List<T> class' functionality.
    /// </summary>
    /// <remarks>
    /// Every individual test of this class corresponds to a List<T> member.
    /// See the corresponding List<T> members' descriptions for any additional information.
    /// </remarks>
    [TestFixture]
    public class TestList<T>
    {
        [Test]
        public void TestAdd()
        {
            List<T> list = new List<T>();
            list.Add(FirstItem);
            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list[0], Is.EqualTo(FirstItem));
        }

        [Test]
        public void TestAddRange()
        {
            List<T> list1 = new List<T>();
            list1.AddRange(gArray);
            Assert.That(list1.Count, Is.EqualTo(gArray.Length));
            Assert.That(list1.ToArray(), Is.EqualTo(gArray));

            List<T> list2 = new List<T>();
            list2.AddRange(list1);
            Assert.That(list2.Count, Is.EqualTo(gArray.Length));
            Assert.That(list2.ToArray(), Is.EqualTo(gArray));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestAddRangeNullList()
        {
            List<T> list = new List<T>();
            list.AddRange((List<T>)null);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestAddRangeNullArray()
        {
            List<T> list = new List<T>();
            list.AddRange((T[])null);
        }

#if COMPARABLE_ITEM
        [Test]
        public void TestSortBinarySearchFull()
        {
            T[] sortedArray = new T[gArray.Length];
            Array.Copy(gArray, sortedArray, gArray.Length);
            Array.Sort(sortedArray);

            List<T> list = new List<T>(gArray);
            list.Sort();
            Assert.That(list.ToArray(), Is.EqualTo(sortedArray));
            Assert.That(list.BinarySearch(FirstItem), Is.EqualTo(ArrayUtil.BinarySearch(sortedArray, 0, sortedArray.Length, FirstItem)));
            Assert.That(0, Is.GreaterThan(list.BinarySearch(gMissingItem)));
        }

        [Test]
        public void TestSortBinarySearchPart()
        {
            T[] sortedArray = new T[gArray.Length];
            Array.Copy(gArray, sortedArray, gArray.Length);
            Array.Sort(sortedArray, 1, sortedArray.Length - 1);

            List<T> list = new List<T>(gArray);
            list.Sort(1, list.Count - 1);
            Assert.That(list.ToArray(), Is.EqualTo(sortedArray));

            T lastItem = gArray[gArray.Length - 1];
            Assert.That(list.BinarySearch(1, list.Count - 1, lastItem), Is.EqualTo(ArrayUtil.BinarySearch(sortedArray, 1, sortedArray.Length - 1, lastItem)));
            Assert.That(0, Is.GreaterThan(list.BinarySearch(1, list.Count - 1, FirstItem)));
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSortNegativeIndex()
        {
            List<T> list = new List<T>(gArray);
            list.Sort(-1, list.Count);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSortNegativeCount()
        {
            List<T> list = new List<T>(gArray);
            list.Sort(0, -1);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestSortInvalidRange()
        {
            List<T> list = new List<T>(gArray);
            list.Sort(0, list.Count + 1);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestBinarySearchNegativeIndex()
        {
            List<T> list = new List<T>(gArray);
            list.BinarySearch(-1, list.Count, FirstItem);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestBinarySearchNegativeCount()
        {
            List<T> list = new List<T>(gArray);
            list.BinarySearch(0, -1, FirstItem);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestBinarySearchInvalidRange()
        {
            List<T> list = new List<T>(gArray);
            list.BinarySearch(0, list.Count + 1, FirstItem);
        }
#endif

        [Test]
        public void TestClear()
        {
            List<T> list = new List<T>(gArray);
            list.Add(FirstItem);
            list.Clear();
            Assert.That(list.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestContains()
        {
            List<T> list = new List<T>(gArray);
            Assert.That(list.Contains(FirstItem), Is.True);
            Assert.That(list.Contains(gMissingItem), Is.False);
        }

        [Test]
        public void TestCopyTo()
        {
            T[] expectedArray = new T[gArray.Length * 2 + 1];
            Array.Copy(gArray, expectedArray, gArray.Length);
            Array.Copy(gArray, 0, expectedArray, gArray.Length, gArray.Length);
            expectedArray[expectedArray.Length - 1] = gArray[gArray.Length - 1];

            T[] actualArray = new T[gArray.Length * 2 + 1];
            List<T> list = new List<T>(gArray);
            list.CopyTo(actualArray);
            list.CopyTo(actualArray, list.Count);
            list.CopyTo(list.Count - 1, actualArray, list.Count * 2, 1);

            Assert.That(actualArray, Is.EqualTo(expectedArray));
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCopyToNegativeIndex()
        {
            T[] array = new T[gArray.Length];
            List<T> list = new List<T>(gArray);
            list.CopyTo(-1, array, 0, list.Count);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCopyToNegativeCount()
        {
            T[] array = new T[gArray.Length];
            List<T> list = new List<T>(gArray);
            list.CopyTo(0, array, 0, -1);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestCopyToInvalidRange()
        {
            T[] array = new T[gArray.Length];
            List<T> list = new List<T>(gArray);
            list.CopyTo(0, array, 0, list.Count + 1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCopyToNullArray()
        {
            List<T> list = new List<T>(gArray);
            list.CopyTo(0, null, 0, list.Count);
        }

        [Test]
        [JavaExpectedException(typeof(ArrayIndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCopyToNegativeArrayIndex()
        {
            T[] array = new T[gArray.Length];
            List<T> list = new List<T>(gArray);
            list.CopyTo(0, array, -1, list.Count);
        }

        [Test]
        [JavaExpectedException(typeof(ArrayIndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCopyToInvalidArrayRange()
        {
            T[] array = new T[gArray.Length - 1];
            List<T> list = new List<T>(gArray);
            list.CopyTo(0, array, 0, list.Count);
        }

        [Test]
        public void TestGetRange()
        {
            List<T> list1 = new List<T>(gArray);
            List<T> list2 = list1.GetRange(1, list1.Count - 1);
            Assert.That(list2.Count, Is.EqualTo(list1.Count - 1));

            T[] expectedArray = new T[gArray.Length - 1];
            Array.Copy(gArray, 1, expectedArray, 0, gArray.Length - 1);
            Assert.That(list2.ToArray(), Is.EqualTo(expectedArray));
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetRangeNegativeIndex()
        {
            List<T> list = new List<T>(gArray);
            list.GetRange(-1, list.Count);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetRangeNegativeCount()
        {
            List<T> list = new List<T>(gArray);
            list.GetRange(0, -1);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestGetRangeInvalidRange()
        {
            List<T> list = new List<T>(gArray);
            list.GetRange(0, list.Count + 1);
        }

        [Test]
        public void TestIndexOf()
        {
            List<T> list = new List<T>(gArray);
            Assert.That(list.IndexOf(FirstItem), Is.EqualTo(0));
            Assert.That(list.IndexOf(gMissingItem), Is.EqualTo(-1));
            Assert.That(list.IndexOf(FirstItem, 1), Is.EqualTo(-1));
            Assert.That(list.IndexOf(FirstItem, 0), Is.EqualTo(0));
            Assert.That(list.IndexOf(FirstItem, 1, 1), Is.EqualTo(-1));
            Assert.That(list.IndexOf(FirstItem, 0, 0), Is.EqualTo(-1));
            Assert.That(list.IndexOf(FirstItem, 0, 1), Is.EqualTo(0));
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestIndexOfNegativeIndex()
        {
            List<T> list = new List<T>(gArray);
            list.IndexOf(gMissingItem, -1, list.Count);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestIndexOfNegativeCount()
        {
            List<T> list = new List<T>(gArray);
            list.IndexOf(gMissingItem, 0, -1);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestIndexOfInvalidRange()
        {
            List<T> list = new List<T>(gArray);
            list.IndexOf(gMissingItem, 0, list.Count + 1);
        }

        [Test]
        public void TestInsert()
        {
            T[] expectedArray = new T[gArray.Length + 3];
            expectedArray[0] = gMissingItem;
            expectedArray[1] = FirstItem;
            expectedArray[2] = gMissingItem;
            expectedArray[expectedArray.Length - 1] = gMissingItem;
            Array.Copy(gArray, 1, expectedArray, 3, gArray.Length - 1);

            List<T> list = new List<T>(gArray);
            list.Insert(1, gMissingItem);
            list.Insert(0, gMissingItem);
            list.Insert(list.Count, gMissingItem);
            Assert.That(list.Count, Is.EqualTo(gArray.Length + 3));
            Assert.That(list.ToArray(), Is.EqualTo(expectedArray));
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestInsertNegativeIndex()
        {
            List<T> list = new List<T>(gArray);
            list.Insert(-1, gMissingItem);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestInsertExceedingIndex()
        {
            List<T> list = new List<T>(gArray);
            list.Insert(list.Count + 1, gMissingItem);
        }

        [Test]
        public void TestInsertRange()
        {
            T[] expectedArray = new T[gArray.Length * 4];
            Array.Copy(gArray, expectedArray, gArray.Length);
            expectedArray[gArray.Length] = FirstItem;
            Array.Copy(gArray, 0, expectedArray, gArray.Length + 1, gArray.Length);
            Array.Copy(gArray, 1, expectedArray, gArray.Length * 2 + 1, gArray.Length - 1);
            Array.Copy(gArray, 0, expectedArray, gArray.Length * 3, gArray.Length);

            List<T> list1 = new List<T>(gArray);
            List<T> list2 = new List<T>(gArray);
            list1.InsertRange(1, list2);
            list1.InsertRange(0, list2);
            list1.InsertRange(list1.Count, list2);
            list2.InsertRange(1, gArray);
            list2.InsertRange(0, gArray);
            list2.InsertRange(list2.Count, gArray);

            Assert.That(list1.Count, Is.EqualTo(expectedArray.Length));
            Assert.That(list1.ToArray(), Is.EqualTo(expectedArray));
            Assert.That(list2.Count, Is.EqualTo(expectedArray.Length));
            Assert.That(list2.ToArray(), Is.EqualTo(expectedArray));
        }

        [Test]
        public void TestInsertRangeSelf()
        {
            T[] expectedArray = new T[gArray.Length * 2];
            expectedArray[0] = FirstItem;
            Array.Copy(gArray, 0, expectedArray, 1, gArray.Length);
            Array.Copy(gArray, 1, expectedArray, gArray.Length + 1, gArray.Length - 1);

            List<T> list = new List<T>(gArray);
            list.InsertRange(1, list);
            Assert.That(list.Count, Is.EqualTo(expectedArray.Length));
            Assert.That(list.ToArray(), Is.EqualTo(expectedArray));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestInsertRangeNullArray()
        {
            List<T> list = new List<T>(gArray);
            list.InsertRange(1, (T[])null);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestInsertRangeArrayNegativeIndex()
        {
            List<T> list = new List<T>(gArray);
            list.InsertRange(-1, gArray);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestInsertRangeArrayExceedingIndex()
        {
            List<T> list = new List<T>(gArray);
            list.InsertRange(list.Count + 1, gArray);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestInsertRangeNullList()
        {
            List<T> list = new List<T>(gArray);
            list.InsertRange(1, (List<T>)null);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestInsertRangeListNegativeIndex()
        {
            List<T> list = new List<T>(gArray);
            list.InsertRange(-1, list);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestInsertRangeListExceedingIndex()
        {
            List<T> list = new List<T>(gArray);
            list.InsertRange(list.Count + 1, list);
        }

        [Test]
        public void TestLastIndexOf()
        {
            List<T> list = new List<T>(gArray);
            int lastItemIndex = list.Count - 1;
            T lastItem = list[lastItemIndex];

            Assert.That(list.LastIndexOf(lastItem), Is.EqualTo(lastItemIndex));
            Assert.That(list.LastIndexOf(gMissingItem), Is.EqualTo(-1));
            Assert.That(list.LastIndexOf(lastItem, lastItemIndex - 1), Is.EqualTo(-1));
            Assert.That(list.LastIndexOf(lastItem, lastItemIndex), Is.EqualTo(lastItemIndex));
            Assert.That(list.LastIndexOf(lastItem, lastItemIndex - 1, 1), Is.EqualTo(-1));
            Assert.That(list.LastIndexOf(lastItem, lastItemIndex, 0), Is.EqualTo(-1));
            Assert.That(list.LastIndexOf(lastItem, lastItemIndex, 1), Is.EqualTo(lastItemIndex));
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestLastIndexOfNegativeIndex()
        {
            List<T> list = new List<T>(gArray);
            list.LastIndexOf(gMissingItem, -1, list.Count);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestLastIndexOfNegativeCount()
        {
            List<T> list = new List<T>(gArray);
            list.LastIndexOf(gMissingItem, list.Count - 1, -1);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestLastIndexOfInvalidRange()
        {
            List<T> list = new List<T>(gArray);
            list.LastIndexOf(gMissingItem, list.Count - 1, list.Count + 1);
        }

        [Test]
        public void TestRemove()
        {
            T[] expectedArray = new T[gArray.Length - 1];
            expectedArray[0] = FirstItem;
            Array.Copy(gArray, 2, expectedArray, 1, gArray.Length - 2);

            List<T> list = new List<T>(gArray);
            bool isRemoved = list.Remove(gArray[1]);
            Assert.That(isRemoved, Is.True);
            Assert.That(list.Count, Is.EqualTo(expectedArray.Length));
            Assert.That(list.ToArray(), Is.EqualTo(expectedArray));

            isRemoved = list.Remove(gMissingItem);
            Assert.That(isRemoved, Is.False);
            Assert.That(list.Count, Is.EqualTo(expectedArray.Length));
            Assert.That(list.ToArray(), Is.EqualTo(expectedArray));
        }

        [Test]
        public void TestRemoveAt()
        {
            T[] expectedArray = new T[gArray.Length - 1];
            expectedArray[0] = FirstItem;
            Array.Copy(gArray, 2, expectedArray, 1, gArray.Length - 2);

            List<T> list = new List<T>(gArray);
            list.RemoveAt(1);
            Assert.That(list.Count, Is.EqualTo(expectedArray.Length));
            Assert.That(list.ToArray(), Is.EqualTo(expectedArray));
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRemoveAtNegativeIndex()
        {
            List<T> list = new List<T>(gArray);
            list.RemoveAt(-1);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRemoveAtExceedingIndex()
        {
            List<T> list = new List<T>(gArray);
            list.RemoveAt(list.Count);
        }

        [Test]
        public void TestRemoveRange()
        {
            List<T> list = new List<T>(gArray);
            list.RemoveRange(1, list.Count - 2);
            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list[0], Is.EqualTo(FirstItem));
            Assert.That(list[1], Is.EqualTo(gArray[gArray.Length - 1]));
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRemoveRangeNegativeIndex()
        {
            List<T> list = new List<T>(gArray);
            list.RemoveRange(-1, list.Count);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRemoveRangeNegativeCount()
        {
            List<T> list = new List<T>(gArray);
            list.RemoveRange(0, -1);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestRemoveRangeInvalidRange()
        {
            List<T> list = new List<T>(gArray);
            list.RemoveRange(0, list.Count + 1);
        }

        [Test]
        public void TestReverseFull()
        {
            T[] expectedArray = new T[gArray.Length];
            Array.Copy(gArray, expectedArray, gArray.Length);
            Array.Reverse(expectedArray);

            List<T> list = new List<T>(gArray);
            list.Reverse();
            Assert.That(list.ToArray(), Is.EqualTo(expectedArray));
        }

        [Test]
        public void TestReversePart()
        {
            T[] expectedArray = new T[gArray.Length];
            Array.Copy(gArray, expectedArray, gArray.Length);
            Array.Reverse(expectedArray, 1, expectedArray.Length - 2);

            List<T> list = new List<T>(gArray);
            list.Reverse(1, list.Count - 2);
            Assert.That(list.ToArray(), Is.EqualTo(expectedArray));
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReverseNegativeIndex()
        {
            List<T> list = new List<T>(gArray);
            list.Reverse(-1, list.Count);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReverseNegativeCount()
        {
            List<T> list = new List<T>(gArray);
            list.Reverse(0, -1);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestReverseInvalidRange()
        {
            List<T> list = new List<T>(gArray);
            list.Reverse(0, list.Count + 1);
        }

        [Test]
        public void TestToArray()
        {
            List<T> list = new List<T>(gArray);
            T[] array1 = list.ToArray();
            T[] array2 = list.ToArray();
            Assert.That(array2, IsNot.SameAs(array1));
            Assert.That(array1, Is.EqualTo(gArray));
            Assert.That(array2, Is.EqualTo(gArray));

            T[] expectedArray = new T[gArray.Length - 1];
            expectedArray[0] = FirstItem;
            Array.Copy(gArray, 2, expectedArray, 1, gArray.Length - 2);

            list.RemoveAt(1);
            Assert.That(list.ToArray(), Is.EqualTo(expectedArray));

            list.Clear();
            Assert.That(list.ToArray(), Is.EqualTo(new T[0]));
        }

        [Test]
        [CppSkipEntity("C# and Java implementation specific test")]
        public void TestTrimExcess()
        {
            List<T> list = new List<T>(10);
            list.Add(gMissingItem);
            list.TrimExcess();
            Assert.That(list.Capacity, Is.EqualTo(1));
        }

        [Test]
        [CppSkipEntity("C# and Java implementation specific test")]
        public void TestCapacityGrow()
        {
            List<T> list = new List<T>();
            Assert.That(list.Capacity, Is.EqualTo(0));

            list.Add(gMissingItem);
            int capacity = list.Capacity;
            Assert.That(capacity, Is.GreaterThan(list.Count));

            while (list.Count != capacity)
                list.Add(gMissingItem);

            Assert.That(list.Capacity, Is.EqualTo(list.Count));

            list.Add(gMissingItem);
            Assert.That(list.Capacity, Is.GreaterThan(list.Count));
        }

        [Test]
        [CppSkipEntity("C# and Java implementation specific test")]
        public void TestCapacity()
        {
            List<T> list = new List<T>(10);
            Assert.That(list.Capacity, Is.EqualTo(10));
            
            list.Capacity = 5;
            Assert.That(list.Capacity, Is.EqualTo(5));
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCapacityNegative()
        {
            List<T> list = new List<T>();
            list.Capacity = -1;
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCapacityInferior()
        {
            List<T> list = new List<T>(gArray);
            list.Capacity = list.Count - 1;
        }

        [Test]
        public void TestCount()
        {
            List<T> list = new List<T>();
            Assert.That(list.Count, Is.EqualTo(0));

            list.Add(gMissingItem);
            Assert.That(list.Count, Is.EqualTo(1));

            list.Add(gMissingItem);
            Assert.That(list.Count, Is.EqualTo(2));

            list.RemoveAt(0);
            Assert.That(list.Count, Is.EqualTo(1));

            list.Clear();
            Assert.That(list.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestGetSet()
        {
            List<T> list = new List<T>();
            list.Add(gMissingItem);
            Assert.That(list[0], Is.EqualTo(gMissingItem));

            list[0] = FirstItem;
            Assert.That(list[0], Is.EqualTo(FirstItem));
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetNegativeIndex()
        {
            List<T> list = new List<T>();
            T item = list[-1];
            Assert.Fail(string.Format(string.Empty, item)); // Force the compiler not to skip the previous instruction.
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetExceedingIndex()
        {
            List<T> list = new List<T>(gArray);
            T item = list[list.Count];
            Assert.Fail(string.Format(string.Empty, item)); // Force the compiler not to skip the previous instruction.
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSetNegativeIndex()
        {
            List<T> list = new List<T>();
            list[-1] = gMissingItem;
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSetExceedingIndex()
        {
            List<T> list = new List<T>(gArray);
            list[list.Count] = gMissingItem;
        }

        [Test]
        public void TestCopyCtors()
        {
            List<T> list1 = new List<T>(gArray);
            Assert.That(list1.ToArray(), Is.EqualTo(gArray));

            List<T> list2 = new List<T>(list1);
            Assert.That(list2.ToArray(), Is.EqualTo(gArray));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestCopyCtorNullArray()
        {
            List<T> list = new List<T>((T[])null);
            Assert.Fail(string.Format(string.Empty, list)); // Force the compiler not to skip the previous instruction.
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestCopyCtorNullList()
        {
            List<T> list = new List<T>((List<T>)null);
            Assert.Fail(string.Format(string.Empty, list)); // Force the compiler not to skip the previous instruction.
        }

        /// <summary>
        /// Returns an array extensively used in this test.
        /// </summary>
        private static T[] GetArray()
        {
            throw new NotSupportedException("GetArrayPlaceholder");
        }

        /// <summary>
        /// Returns an instance of T type missing in an array returned by <see cref="GetArray"/>.
        /// </summary>
        private static T GetMissingItem()
        {
            throw new NotSupportedException("GetMissingItemPlaceholder");
        }

        /// <summary>
        /// Gets the first item of the test array.
        /// </summary>
        private static T FirstItem
        {
            get { return gArray[0]; }
        }

        // Calculate once, use multiple times.
        private static readonly T[] gArray = GetArray();
        private static readonly T gMissingItem = GetMissingItem();
    }
}
#endif
