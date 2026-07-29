// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 22/02/2018 by Anatoly Sidorenko

using System;
using System.Collections.Generic;
using Aspose.JavaAttributes;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Collections
{
    [TestFixture]
    public class TestListGeneric
    {
        [Test]
        public void TestAdd()
        {
            List<string> list = new List<string>();
            list.Add(FirstItem);
            Assert.That(list.Count, Is.EqualTo(1));
            Assert.That(list[0], Is.EqualTo(FirstItem));
        }

        [Test]
        public void TestAddRange()
        {
            List<string> list1 = new List<string>();
            list1.AddRange(gArray);
            Assert.That(list1.Count, Is.EqualTo(gArray.Length));
            Assert.That(list1.ToArray(), Is.EqualTo(gArray));

            List<string> list2 = new List<string>();
            list2.AddRange(list1);
            Assert.That(list2.Count, Is.EqualTo(gArray.Length));
            Assert.That(list2.ToArray(), Is.EqualTo(gArray));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestAddRangeNullList()
        {
            List<string> list = new List<string>();
            list.AddRange((List<string>)null);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestAddRangeNullArray()
        {
            List<string> list = new List<string>();
            list.AddRange((string[])null);
        }

        [Test]
        public void TestSortBinarySearchFull()
        {
            string[] sortedArray = new string[gArray.Length];
            Array.Copy(gArray, sortedArray, gArray.Length);
            Array.Sort(sortedArray);

            List<string> list = new List<string>(gArray);
            list.Sort();
            Assert.That(list.ToArray(), Is.EqualTo(sortedArray));
            Assert.That(list.BinarySearch(FirstItem, StringOrdinalComparer.Default), Is.EqualTo(ArrayUtil.BinarySearch(sortedArray, 0, sortedArray.Length, FirstItem)));
            Assert.That(0, Is.GreaterThan(list.BinarySearch(gMissingItem, StringOrdinalComparer.Default)));
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSortNegativeIndex()
        {
            List<string> list = new List<string>(gArray);
            list.Sort(-1, list.Count, StringOrdinalComparer.Default);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSortNegativeCount()
        {
            List<string> list = new List<string>(gArray);
            list.Sort(0, -1, StringOrdinalComparer.Default);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentException))]
        public void TestSortInvalidRange()
        {
            List<string> list = new List<string>(gArray);
            list.Sort(0, list.Count + 1, StringOrdinalComparer.Default);
        }

        [Test]
        public void TestClear()
        {
            List<string> list = new List<string>(gArray);
            list.Add(FirstItem);
            list.Clear();
            Assert.That(list.Count, Is.EqualTo(0));
        }

        [Test]
        public void TestContains()
        {
            List<string> list = new List<string>(gArray);
            Assert.That(list.Contains(FirstItem), Is.True);
            Assert.That(list.Contains(gMissingItem), Is.False);
        }

        [Test]
        public void TestCopyTo()
        {
            string[] expectedArray = new string[gArray.Length * 2 + 1];
            Array.Copy(gArray, expectedArray, gArray.Length);
            Array.Copy(gArray, 0, expectedArray, gArray.Length, gArray.Length);
            expectedArray[expectedArray.Length - 1] = gArray[gArray.Length - 1];

            string[] actualArray = new string[gArray.Length * 2 + 1];
            List<string> list = new List<string>(gArray);
            list.CopyTo(actualArray);
            list.CopyTo(actualArray, list.Count);
            list.CopyTo(list.Count - 1, actualArray, list.Count * 2, 1);

            Assert.That(actualArray, Is.EqualTo(expectedArray));
        }

        [Test]
        [JavaExpectedException(typeof(ArrayIndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCopyToNegativeIndex()
        {
            string[] array = new string[gArray.Length];
            List<string> list = new List<string>(gArray);
            list.CopyTo(-1, array, 0, list.Count);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCopyToNegativeCount()
        {
            string[] array = new string[gArray.Length];
            List<string> list = new List<string>(gArray);
            list.CopyTo(0, array, 0, -1);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCopyToInvalidRange()
        {
            string[] array = new string[gArray.Length];
            List<string> list = new List<string>(gArray);
            list.CopyTo(0, array, 0, list.Count + 1);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestCopyToNullArray()
        {
            List<string> list = new List<string>(gArray);
            list.CopyTo(0, null, 0, list.Count);
        }

        [Test]
        [JavaExpectedException(typeof(ArrayIndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestCopyToNegativeArrayIndex()
        {
            string[] array = new string[gArray.Length];
            List<string> list = new List<string>(gArray);
            list.CopyTo(0, array, -1, list.Count);
        }

        [Test]
        [JavaExpectedException(typeof(ArrayIndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentException))]
        public void TestCopyToInvalidArrayRange()
        {
            string[] array = new string[gArray.Length - 1];
            List<string> list = new List<string>(gArray);
            list.CopyTo(0, array, 0, list.Count);
        }

        [Test]
        public void TestGetRange()
        {
            List<string> list1 = new List<string>(gArray);
            List<string> list2 = list1.GetRange(1, list1.Count - 1);
            Assert.That(list2.Count, Is.EqualTo(list1.Count - 1));

            string[] expectedArray = new string[gArray.Length - 1];
            Array.Copy(gArray, 1, expectedArray, 0, gArray.Length - 1);
            Assert.That(list2.ToArray(), Is.EqualTo(expectedArray));
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetRangeNegativeIndex()
        {
            List<string> list = new List<string>(gArray);
            list.GetRange(-1, list.Count);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetRangeNegativeCount()
        {
            List<string> list = new List<string>(gArray);
            list.GetRange(0, -1);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentException))]
        public void TestGetRangeInvalidRange()
        {
            List<string> list = new List<string>(gArray);
            list.GetRange(0, list.Count + 1);
        }

        [Test]
        public void TestIndexOf()
        {
            List<string> list = new List<string>(gArray);
            Assert.That(list.IndexOf(FirstItem), Is.EqualTo(0));
            Assert.That(list.IndexOf(gMissingItem), Is.EqualTo(-1));
        }

        [Test]
        public void TestInsert()
        {
            string[] expectedArray = new string[gArray.Length + 3];
            expectedArray[0] = gMissingItem;
            expectedArray[1] = FirstItem;
            expectedArray[2] = gMissingItem;
            expectedArray[expectedArray.Length - 1] = gMissingItem;
            Array.Copy(gArray, 1, expectedArray, 3, gArray.Length - 1);

            List<string> list = new List<string>(gArray);
            list.Insert(1, gMissingItem);
            list.Insert(0, gMissingItem);
            list.Insert(list.Count, gMissingItem);
            Assert.That(list.Count, Is.EqualTo(gArray.Length + 3));
            Assert.That(list.ToArray(), Is.EqualTo(expectedArray));
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestInsertNegativeIndex()
        {
            List<string> list = new List<string>(gArray);
            list.Insert(-1, gMissingItem);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestInsertExceedingIndex()
        {
            List<string> list = new List<string>(gArray);
            list.Insert(list.Count + 1, gMissingItem);
        }

        [Test]
        public void TestInsertRange()
        {
            string[] expectedArray = new string[gArray.Length * 4];
            Array.Copy(gArray, expectedArray, gArray.Length);
            expectedArray[gArray.Length] = FirstItem;
            Array.Copy(gArray, 0, expectedArray, gArray.Length + 1, gArray.Length);
            Array.Copy(gArray, 1, expectedArray, gArray.Length * 2 + 1, gArray.Length - 1);
            Array.Copy(gArray, 0, expectedArray, gArray.Length * 3, gArray.Length);

            List<string> list1 = new List<string>(gArray);
            List<string> list2 = new List<string>(gArray);
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
            string[] expectedArray = new string[gArray.Length * 2];
            expectedArray[0] = FirstItem;
            Array.Copy(gArray, 0, expectedArray, 1, gArray.Length);
            Array.Copy(gArray, 1, expectedArray, gArray.Length + 1, gArray.Length - 1);

            List<string> list = new List<string>(gArray);
            list.InsertRange(1, list);
            Assert.That(list.Count, Is.EqualTo(expectedArray.Length));
            Assert.That(list.ToArray(), Is.EqualTo(expectedArray));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestInsertRangeNullArray()
        {
            List<string> list = new List<string>(gArray);
            list.InsertRange(1, (string[])null);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestInsertRangeArrayNegativeIndex()
        {
            List<string> list = new List<string>(gArray);
            list.InsertRange(-1, gArray);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestInsertRangeArrayExceedingIndex()
        {
            List<string> list = new List<string>(gArray);
            list.InsertRange(list.Count + 1, gArray);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestInsertRangeNullList()
        {
            List<string> list = new List<string>(gArray);
            list.InsertRange(1, (List<string>)null);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestInsertRangeListNegativeIndex()
        {
            List<string> list = new List<string>(gArray);
            list.InsertRange(-1, list);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestInsertRangeListExceedingIndex()
        {
            List<string> list = new List<string>(gArray);
            list.InsertRange(list.Count + 1, list);
        }

        [Test]
        public void TestLastIndexOf()
        {
            List<string> list = new List<string>(gArray);
            int lastItemIndex = list.Count - 1;
            string lastItem = list[lastItemIndex];

            Assert.That(list.LastIndexOf(lastItem), Is.EqualTo(lastItemIndex));
            Assert.That(list.LastIndexOf(gMissingItem), Is.EqualTo(-1));
        }

        [Test]
        public void TestRemove()
        {
            string[] expectedArray = new string[gArray.Length - 1];
            expectedArray[0] = FirstItem;
            Array.Copy(gArray, 2, expectedArray, 1, gArray.Length - 2);

            List<string> list = new List<string>(gArray);
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
            string[] expectedArray = new string[gArray.Length - 1];
            expectedArray[0] = FirstItem;
            Array.Copy(gArray, 2, expectedArray, 1, gArray.Length - 2);

            List<string> list = new List<string>(gArray);
            list.RemoveAt(1);
            Assert.That(list.Count, Is.EqualTo(expectedArray.Length));
            Assert.That(list.ToArray(), Is.EqualTo(expectedArray));
        }

        [Test]
        [JavaExpectedException(typeof(ArrayIndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRemoveAtNegativeIndex()
        {
            List<string> list = new List<string>(gArray);
            list.RemoveAt(-1);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRemoveAtExceedingIndex()
        {
            List<string> list = new List<string>(gArray);
            list.RemoveAt(list.Count);
        }

        [Test]
        public void TestRemoveRange()
        {
            List<string> list = new List<string>(gArray);
            list.RemoveRange(1, list.Count - 2);
            Assert.That(list.Count, Is.EqualTo(2));
            Assert.That(list[0], Is.EqualTo(FirstItem));
            Assert.That(list[1], Is.EqualTo(gArray[gArray.Length - 1]));
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRemoveRangeNegativeIndex()
        {
            List<string> list = new List<string>(gArray);
            list.RemoveRange(-1, list.Count);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestRemoveRangeNegativeCount()
        {
            List<string> list = new List<string>(gArray);
            list.RemoveRange(0, -1);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentException))]
        public void TestRemoveRangeInvalidRange()
        {
            List<string> list = new List<string>(gArray);
            list.RemoveRange(0, list.Count + 1);
        }

        [Test]
        public void TestReverseFull()
        {
            string[] expectedArray = new string[gArray.Length];
            Array.Copy(gArray, expectedArray, gArray.Length);
            Array.Reverse(expectedArray);

            List<string> list = new List<string>(gArray);
            list.Reverse();
            Assert.That(list.ToArray(), Is.EqualTo(expectedArray));
        }

        [Test]
        public void TestReversePart()
        {
            string[] expectedArray = new string[gArray.Length];
            Array.Copy(gArray, expectedArray, gArray.Length);
            Array.Reverse(expectedArray, 1, expectedArray.Length - 2);

            List<string> list = new List<string>(gArray);
            list.Reverse(1, list.Count - 2);
            Assert.That(list.ToArray(), Is.EqualTo(expectedArray));
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReverseNegativeIndex()
        {
            List<string> list = new List<string>(gArray);
            list.Reverse(-1, list.Count);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReverseNegativeCount()
        {
            List<string> list = new List<string>(gArray);
            list.Reverse(0, -1);
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentException))]
        public void TestReverseInvalidRange()
        {
            List<string> list = new List<string>(gArray);
            list.Reverse(0, list.Count + 1);
        }

        [Test]
        public void TestToArray()
        {
            List<string> list = new List<string>(gArray);
            string[] array1 = list.ToArray();
            string[] array2 = list.ToArray();
            Assert.That(array2, IsNot.SameAs(array1));
            Assert.That(array1, Is.EqualTo(gArray));
            Assert.That(array2, Is.EqualTo(gArray));

            string[] expectedArray = new string[gArray.Length - 1];
            expectedArray[0] = FirstItem;
            Array.Copy(gArray, 2, expectedArray, 1, gArray.Length - 2);

            list.RemoveAt(1);
            Assert.That(list.ToArray(), Is.EqualTo(expectedArray));

            list.Clear();
            Assert.That(list.ToArray(), Is.EqualTo(new int[0]));
        }

        [Test]
        public void TestCount()
        {
            List<string> list = new List<string>();
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
            List<string> list = new List<string>();
            list.Add(gMissingItem);
            Assert.That(list[0], Is.EqualTo(gMissingItem));

            list[0] = FirstItem;
            Assert.That(list[0], Is.EqualTo(FirstItem));
        }

        [Test]
        [JavaExpectedException(typeof(ArrayIndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetNegativeIndex()
        {
            List<string> list = new List<string>();
            string item = list[-1];
            Assert.Fail(string.Format(string.Empty, item)); // Force the compiler not to skip the previous instruction.
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestGetExceedingIndex()
        {
            List<string> list = new List<string>(gArray);
            string item = list[list.Count];
            Assert.Fail(string.Format(string.Empty, item)); // Force the compiler not to skip the previous instruction.
        }

        [Test]
        [JavaExpectedException(typeof(ArrayIndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSetNegativeIndex()
        {
            List<string> list = new List<string>();
            list[-1] = gMissingItem;
        }

        [Test]
        [JavaExpectedException(typeof(IndexOutOfBoundsException))]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestSetExceedingIndex()
        {
            List<string> list = new List<string>(gArray);
            list[list.Count] = gMissingItem;
        }

        [Test]
        public void TestCopyCtors()
        {
            List<string> list1 = new List<string>(gArray);
            Assert.That(list1.ToArray(), Is.EqualTo(gArray));

            List<string> list2 = new List<string>(list1);
            Assert.That(list2.ToArray(), Is.EqualTo(gArray));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestCopyCtorNullArray()
        {
            List<string> list = new List<string>((string[])null);
            Assert.Fail(string.Format(string.Empty, list)); // Force the compiler not to skip the previous instruction.
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestCopyCtorNullList()
        {
            List<string> list = new List<string>((List<string>)null);
            Assert.Fail(string.Format(string.Empty, list)); // Force the compiler not to skip the previous instruction.
        }

        /// <summary>
        /// Returns an array extensively used in this test.
        /// </summary>
        private static string[] GetArray()
        {
            return new string[] { "6", "4", "1", "2", "5", "3" };
        }

        /// <summary>
        /// Returns an instance of string type missing in an array returned by <see cref="GetArray"/>.
        /// </summary>
        private static string GetMissingItem()
        {
            return "7";
        }

        /// <summary>
        /// Gets the first item of the test array.
        /// </summary>
        private static string FirstItem
        {
            get { return gArray[0]; }
        }

        private class StringOrdinalComparer : IComparer<string>
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
            public static readonly IComparer<string> Default = new StringOrdinalComparer();
        }

        // Calculate once, use multiple times.
        private static readonly string[] gArray = GetArray();
        private static readonly string gMissingItem = GetMissingItem();
    }
}

