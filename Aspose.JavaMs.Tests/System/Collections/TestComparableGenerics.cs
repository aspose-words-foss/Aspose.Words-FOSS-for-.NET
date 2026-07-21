// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/01/2018 by Anatoly Sidorenko

using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Collections
{
    [TestFixture]
    public class TestComparableGenerics
    {
        [Test]
        public void TestArraySort()
        {
            List<CompareObject> compareArrayList = GetCompareArrayList();
            CompareObject[] compareArray = compareArrayList.ToArray();
            Array.Sort(compareArray);
            string result = "";
            for (int i = 0; i < compareArray.Length; i++)
                result = result + compareArray[i].Code;

            Assert.That("7 6 4 3 2 ", Is.EqualTo(result));

            Array.Sort(compareArray, new ReverseObjectComparer());
            result = "";
            for (int i = 0; i < compareArray.Length; i++)
                result = result + compareArray[i].Code;

            Assert.That("2 3 4 6 7 ", Is.EqualTo(result));
        }

        [Test]
        public void TestListSort()
        {
            List<CompareObject> compareList = GetCompareList();
            compareList.Sort();
            string result = "";
            for (int i = 0; i < compareList.Count; i++)
                result = result + compareList[i].Code;

            Assert.That("7 6 4 3 2 ", Is.EqualTo(result));

            compareList.Sort(new ReverseObjectComparer());
            result = "";
            for (int i = 0; i < compareList.Count; i++)
                result = result + compareList[i].Code;

            Assert.That("2 3 4 6 7 ", Is.EqualTo(result));
        }
        
        private List<CompareObject> GetCompareArrayList()
        {
            List<CompareObject> newArray = new List<CompareObject>();
            foreach (int index in mIndexes)
                newArray.Add(new CompareObject(index));

            return newArray;
        }

        private List<CompareObject> GetCompareList()
        {
            List<CompareObject> newList = new List<CompareObject>();
            foreach (int index in mIndexes)
                newList.Add(new CompareObject(index));

            return newList;
        }


        private int[] mIndexes = { 3, 4, 2, 6, 7 };

        private class ReverseObjectComparer : IComparer<CompareObject>
        {
            public int Compare(CompareObject x, CompareObject y)
            {
                return -x.CompareTo(y);
            }
        }

        private class CompareObject : IComparable<CompareObject>
        {
            internal CompareObject(int index)
            {
                mCode = index + " ";
                mIndex = index;
            }

            public int CompareTo(CompareObject obj)
            {
                if (obj == null)
                    return 1;
                
                return - Index.CompareTo(obj.Index);
            }

            internal string Code
            {
                get { return mCode; }
            }

            internal int Index
            {
                get { return mIndex; }
            }

            private readonly string mCode;
            private readonly int mIndex;
        }
    }
}
