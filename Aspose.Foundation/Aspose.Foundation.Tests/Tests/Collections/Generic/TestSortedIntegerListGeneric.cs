// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 25/06/2018 by Alexey Butalov

using System.Text;
using Aspose.Collections.Generic;
using NUnit.Framework;

namespace Aspose.Tests.Collections.Generic
{
    [TestFixture]
    public class TestSortedIntegerListGeneric
    {
        [Test]
        public void TestAdd()
        {
            SortedIntegerListGeneric<string> list = new SortedIntegerListGeneric<string>();
            AddAll(list);
            CheckSorting(list, mOrderedValues1);
        }

        [Test]
        public void TestSet()
        {
            SortedIntegerListGeneric<string> list = new SortedIntegerListGeneric<string>();
            SetAll(list);
            CheckSorting(list, mOrderedValues2);
        }

        [Test]
        public void TestAddSet()
        {
            SortedIntegerListGeneric<string> list = new SortedIntegerListGeneric<string>();
            AddAll(list);
            SetAll(list);
            CheckSorting(list, mOrderedValues2);
        }

        [Test]
        public void TestAddSetRemove()
        {
            SortedIntegerListGeneric<string> list = new SortedIntegerListGeneric<string>();
            AddAll(list);
            SetAll(list);
            RemoveAll(list);
            CheckSorting(list, mOrderedValues3);
        }

        private void AddAll(SortedIntegerListGeneric<string> list)
        {
            for (int i = 0; i < mKeysToAdd.Length; i++)
                list.Add(mKeysToAdd[i], mValuesToAdd[i]);
        }

        private void SetAll(SortedIntegerListGeneric<string> list)
        {
            for (int i = 0; i < mKeysToSet.Length; i++)
                list[mKeysToSet[i]] = mValuesToSet[i];
        }

        private void RemoveAll(SortedIntegerListGeneric<string> list)
        {
            for (int i = 0; i < mKeysToRemove.Length; i++)
                list.Remove(mKeysToRemove[i]);
        }

        private static string[] GetArrayOfValues(SortedIntegerListGeneric<string> list)
        {
            string[] values = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
                values[i] = list.GetByIndex(i);
            return values;
        }

        private static void CheckSorting(SortedIntegerListGeneric<string> list, string[] expectedValues)
        {
            string[] sortedValues = GetArrayOfValues(list);
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

        private readonly int[] mKeysToAdd =
        {
            0, 20, 10, 200, 100, 2000, 1000, 20000, 10000, 200000, 100000,
            2000000, 1000000, 20000000, 10000000, 200000000, 100000000, 2000000000, 1000000000
        };

        private readonly string[] mValuesToAdd =
        {
            "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18"
        };

        private readonly string[] mOrderedValues1 =
        {
            "0", "2", "1", "4", "3", "6", "5", "8", "7", "10", "9", "12", "11", "14", "13", "16", "15", "18", "17"
        };

        private readonly int[] mKeysToSet =
        {
            999, 0, 20, 10, 200, 100, 2000, 1000, 20000, 10000, 200000, 100000,
            999999999, 2000000, 1000000, 20000000, 10000000, 200000000, 100000000, 2000000000, 1000000000
        };

        private readonly string[] mValuesToSet =
        {
            "19", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "20", "11", "12", "13", "14", "15", "16", "17", "18"
        };

        private readonly string[] mOrderedValues2 =
        {
            "0", "2", "1", "4", "3", "19", "6", "5", "8", "7", "10", "9", "12", "11", "14", "13", "16", "15", "20", "18", "17"
        };

        private readonly int[] mKeysToRemove =
        {
            20, 10, 200, 100, 2000, 1000, 20000, 10000, 200000, 100000,
            2000000, 1000000, 20000000, 10000000, 200000000, 100000000, 2000000000, 1000000000
        };

        private readonly string[] mOrderedValues3 = {"0", "19", "20"};
    }
}
