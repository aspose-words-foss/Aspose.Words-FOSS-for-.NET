// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 28/10/2015 by Dmitry Burov

using System.Text;
using Aspose.Collections;
using NUnit.Framework;

namespace Aspose.Tests.Base
{
    [TestFixture]
    public class TestSortedShortListIntegerFallback
    {
        [Test]
        public void TestAdd()
        {
            list = new SortedShortListIntegerFallback();
            AddAll();
            CheckSorting(mOrderedValues1);
        }

        [Test]
        public void TestSet()
        {
            list = new SortedShortListIntegerFallback();
            SetAll();
            CheckSorting(mOrderedValues2);
        }

        [Test]
        public void TestAddSet()
        {
            list = new SortedShortListIntegerFallback();
            AddAll();
            SetAll();
            CheckSorting(mOrderedValues2);
        }

        [Test]
        public void TestAddSetRemove()
        {
            list = new SortedShortListIntegerFallback();
            AddAll();
            SetAll();
            RemoveAll();
            CheckSorting(mOrderedValues3);
        }

        private void AddAll()
        {
            for (int i = 0; i < mKeysToAdd.Length; i++)
                list.Add(mKeysToAdd[i], mValuesToAdd[i]);
        }

        private void SetAll()
        {
            for (int i = 0; i < mKeysToSet.Length; i++)
                list[mKeysToSet[i]] = mValuesToSet[i];
        }

        private void RemoveAll()
        {
            for (int i = 0; i < mKeysToRemove.Length; i++)
                list.Remove(mKeysToRemove[i]);
        }

        private int[] GetArrayOfValues()
        {
            int[] values = new int[list.Count];
            for (int i = 0; i < list.Count; i++)
                values[i] = (int)list.GetByIndex(i);
            return values;
        }

        private void CheckSorting(int[] expectedValues)
        {
            int[] sortedValues = GetArrayOfValues();
            if (!ArrayUtil.IsArrayEqual(sortedValues, expectedValues))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("\r\nsorted\t: expected\r\n");
                sb.Append("----------:---------\r\n");
                for (int i = 0; i < expectedValues.Length; i++)
                    sb.AppendFormat("{0} -> {1} : {2}\r\n", list.GetKey(i), sortedValues[i], expectedValues[i]);

                Assert.Fail(sb.ToString());
            }
        }

        private SortedShortListIntegerFallback list;

        private int[] mKeysToAdd =
            {
                0, 20, 10, 200, 100, 2000, 1000, 20000, 10000, 200000, 100000,
                2000000, 1000000, 20000000, 10000000, 200000000, 100000000, 2000000000, 1000000000
            };

        private int[] mValuesToAdd = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18 };
        private int[] mOrderedValues1 = { 0, 2, 1, 4, 3, 6, 5, 8, 7, 10, 9, 12, 11, 14, 13, 16, 15, 18, 17 };

        private int[] mKeysToSet =
            {
                999, 0, 20, 10, 200, 100, 2000, 1000, 20000, 10000, 200000, 100000,
                999999999, 2000000, 1000000, 20000000, 10000000, 200000000, 100000000, 2000000000, 1000000000
            };

        private int[] mValuesToSet = { 19, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 20, 11, 12, 13, 14, 15, 16, 17, 18 };
        private int[] mOrderedValues2 = { 0, 2, 1, 4, 3, 19, 6, 5, 8, 7, 10, 9, 12, 11, 14, 13, 16, 15, 20, 18, 17 };

        private int[] mKeysToRemove =
            {
                20, 10, 200, 100, 2000, 1000, 20000, 10000, 200000, 100000,
                2000000, 1000000, 20000000, 10000000, 200000000, 100000000, 2000000000, 1000000000
            };

        private int[] mOrderedValues3 = { 0, 19, 20 };
    }
}
