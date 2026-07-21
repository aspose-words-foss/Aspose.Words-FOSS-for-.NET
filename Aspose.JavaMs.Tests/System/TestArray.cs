// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 19/01/2007 by Konstantin Sidorenko
// 21/12/2015 by Anatoliy Sidorenko

using System;
using System.Drawing;
using Aspose.JavaAttributes;
using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System
{
    [TestFixture]
    public class TestArray
    {
        //Array.IndexOf(int[], ...)

        [Test]
        public void TestIndexOfIntArray()
        {
            Assert.That(0, Is.EqualTo(Array.IndexOf(mIntArray, 0, 0, 10)));
            Assert.That(7, Is.EqualTo(Array.IndexOf(mIntArray, 0, 1, 9)));
            Assert.That(5, Is.EqualTo(Array.IndexOf(mIntArray, 5, 0, 10)));
            Assert.That(6, Is.EqualTo(Array.IndexOf(mIntArray, -1, 0, 10)));
            Assert.That(8, Is.EqualTo(Array.IndexOf(mIntArray, int.MinValue, 0, 10)));
            Assert.That(9, Is.EqualTo(Array.IndexOf(mIntArray, int.MaxValue, 0, 10)));
            Assert.That(-1, Is.EqualTo(Array.IndexOf(mIntArray, 999, 0, 10)));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestIndexOfIntArrayNull()
        {
            int[] nullArray = null;
            Array.IndexOf(nullArray, 0, 0, 10);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestIndexOfIntArrayNegativeStartIndex()
        {
            Array.IndexOf(mIntArray, 0, -1, 10);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestIndexOfIntArrayTooBigStartIndex()
        {
            Array.IndexOf(mIntArray, 0, 10, 1);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestIndexOfIntArrayNegativeCount()
        {
            Array.IndexOf(mIntArray, 0, 0, -1);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestIndexOfIntArrayTooBigCount()
        {
            Array.IndexOf(mIntArray, 0, 0, 11);
        }

        //Array.IndexOf(object[], ...)

        [Test]
        public void TestIndexOfobjectArray()
        {
            Assert.That(0, Is.EqualTo(Array.IndexOf(mObjArray, 0, 0, 10)));
            Assert.That(7, Is.EqualTo(Array.IndexOf(mObjArray, 0, 1, 9)));
            Assert.That(5, Is.EqualTo(Array.IndexOf(mObjArray, 5, 0, 10)));
            Assert.That(6, Is.EqualTo(Array.IndexOf(mObjArray, -1, 0, 10)));
            Assert.That(8, Is.EqualTo(Array.IndexOf(mObjArray, null, 0, 10)));
            Assert.That(9, Is.EqualTo(Array.IndexOf(mObjArray, int.MaxValue, 0, 10)));
            Assert.That(-1, Is.EqualTo(Array.IndexOf(mObjArray, 999, 0, 10)));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestIndexOfobjectArrayNull()
        {
            object[] nullArray = null;
            Array.IndexOf(nullArray, 0, 0, 10);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestIndexOfobjectArrayNegativeStartIndex()
        {
            Array.IndexOf(mObjArray, 0, -1, 10);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestIndexOfobjectArrayTooBigStartIndex()
        {
            Array.IndexOf(mObjArray, 0, 10, 1);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestIndexOfobjectArrayNegativeCount()
        {
            Array.IndexOf(mObjArray, 0, 0, -1);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestIndexOfobjectArrayTooBigCount()
        {
            Array.IndexOf(mObjArray, 0, 0, 11);
        }

        //Array.LastIndexOf(int[], ...)

        [Test]
        public void TestLastIndexOfIntArray()
        {
            Assert.That(7, Is.EqualTo(Array.LastIndexOf(mIntArray, 0, 9, 10)));
            Assert.That(0, Is.EqualTo(Array.LastIndexOf(mIntArray, 0, 5, 6)));
            Assert.That(5, Is.EqualTo(Array.LastIndexOf(mIntArray, 5, 9, 10)));
            Assert.That(6, Is.EqualTo(Array.LastIndexOf(mIntArray, -1, 9, 10)));
            Assert.That(8, Is.EqualTo(Array.LastIndexOf(mIntArray, int.MinValue, 9, 10)));
            Assert.That(9, Is.EqualTo(Array.LastIndexOf(mIntArray, int.MaxValue, 9, 10)));
            Assert.That(-1, Is.EqualTo(Array.LastIndexOf(mIntArray, 999, 9, 10)));
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestLastIndexOfIntArrayNull()
        {
            int[] nullArray = null;
            Array.LastIndexOf(nullArray, 0, 9, 10);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestLastIndexOfIntArrayNegativeStartIndex()
        {
            Array.LastIndexOf(mIntArray, 0, -1, 10);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestLastIndexOfIntArrayTooBigStartIndex()
        {
            Array.LastIndexOf(mIntArray, 0, 10, 1);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestLastIndexOfIntArrayNegativeCount()
        {
            Array.LastIndexOf(mIntArray, 0, 2, -1);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestLastIndexOfIntArrayTooBigCount()
        {
            Array.LastIndexOf(mIntArray, 0, 9, 11);
        }

        //Array.Reverse(int[], ...)

        [Test]
        public void TestReverceIntArray()
        {
            int[] firstArray = { 0, 1, 2, 3, 4, 5 };
            int[] reversedArray = { 5, 4, 3, 2, 1, 0 };
            int[] reversedArray2 = { 5, 1, 2, 3, 4, 0 };
            int[] reversedArray3 = { 5, 1, 2, 3, 0, 4 };

            Array.Reverse(firstArray, 0, firstArray.Length);
            Assert.That(ArrayUtil.IsArrayEqual(reversedArray, firstArray), Is.True);

            Array.Reverse(firstArray, 1, 4);
            Assert.That(ArrayUtil.IsArrayEqual(reversedArray2, firstArray), Is.True);

            Array.Reverse(firstArray, 4, 2);
            Assert.That(ArrayUtil.IsArrayEqual(reversedArray3, firstArray), Is.True);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void TestReverceIntArrayNull()
        {
            int[] nullArray = null;
            Array.Reverse(nullArray, 0, 10);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReverceIntArrayNegativeStartIndex()
        {
            Array.Reverse(mIntArray2, -1, 9);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestReverceIntArrayTooBigStartIndex()
        {
            Array.Reverse(mIntArray2, 10, 1);
        }

        [Test, ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestReverceIntArrayNegativeCount()
        {
            Array.Reverse(mIntArray2, 2, -1);
        }

        // '((Array)arrayAsObject).Clone()' is not autoportable yet.
        // C# arrays extends Array, but Java arrays extends Object. '(Array)' is autoported as '(Object[])'
        // .Net's '((Array)arrayAsObject).Clone()' should be manually ported to Java as 'msArray.clone(arrayAsObject)'.
        [Test]
        [JavaDelete("((Array)arrayAsObject).Clone() is not autoportable yet.")]
        public void TestClone()
        {
            byte[] bytes = { (byte)1, (byte)2, (byte)3 };
            char[] chars = { 'a', 'b', 'c' };
            short[] shorts = { 1, 2, 3 };
            int[] ints = { 1, 2, 3 };
            long[] longs = { 1L, 2L, 3L };
            float[] floats = { 1.0F, 2.0F, 3.0F };
            double[] doubles = { 1.0, 2.0, 3.0 };

            String[] strings = { "one", "two", "three" };
            Point[] points = { new Point(1, 1), new Point(2, 2), new Point(3, 3) };

            Object objBytes = bytes;
            Object objChars = chars;
            Object objShorts = shorts;
            Object objInts = ints;
            Object objLongs = longs;
            Object objFloats = floats;
            Object objDoubles = doubles;
            Object objStrings = strings;
            Object objPoints = points;

            // Should be manually ported to Java as 'msArray.clone(arrayAsObject)'.
            Object clonedBytes = ((Array)objBytes).Clone();
            Object clonedChars = ((Array)objChars).Clone();
            Object clonedShorts = ((Array)objShorts).Clone();
            Object clonedInts = ((Array)objInts).Clone();
            Object clonedLongs = ((Array)objLongs).Clone();
            Object clonedFloats = ((Array)objFloats).Clone();
            Object clonedDoubles = ((Array)objDoubles).Clone();
            Object clonedStrings = ((Array)objStrings).Clone();
            Point[] clonedPoints = (Point[])((Array)objPoints).Clone();

            Assert.That(ArrayUtil.IsArrayEqual(bytes, (byte[])clonedBytes), Is.True);
            Assert.That(ArrayUtil.IsArrayEqual(chars, (char[])clonedChars), Is.True);
            Assert.That(ArrayUtil.IsArrayEqual(shorts, (short[])clonedShorts), Is.True);
            Assert.That(ArrayUtil.IsArrayEqual(ints, (int[])clonedInts), Is.True);
            Assert.That(ArrayUtil.IsArrayEqual(longs, (long[])clonedLongs), Is.True);
            Assert.That(ArrayUtil.IsArrayEqual(floats, (float[])clonedFloats), Is.True);
            Assert.That(ArrayUtil.IsArrayEqual(doubles, (double[])clonedDoubles), Is.True);
            Assert.That(ArrayUtil.IsArrayEqual((Object[])strings, (Object[])clonedStrings), Is.True);
            Assert.That(ArrayUtil.IsArrayEqual(points, clonedPoints), Is.True);
        }

        [Test]
        public void TestArrayUtilContains()
        {
            byte[] src = { 0x1, 0x2, 0x3, 0x4, 0x5 };
            byte[] value1 = { 0x2, 0x3 };
            byte[] value2 = { 0x1 };
            byte[] value3 = { 0x5 };
            byte[] vrong = { 0x6, 0x7 };

            Assert.That(ArrayUtil.Contains(src, value1), Is.True);
            Assert.That(ArrayUtil.Contains(src, value2), Is.True);
            Assert.That(ArrayUtil.Contains(src, value3), Is.True);
            Assert.That(ArrayUtil.Contains(src, src), Is.True);
            Assert.That(ArrayUtil.Contains(value1, value1), Is.True);

            Assert.That(ArrayUtil.Contains(value1, src), Is.False);
            Assert.That(ArrayUtil.Contains(src, vrong), Is.False);
        }

        [Test]
        // TODO: strange test...
        public void TestArrayUtilToString()
        {
            int count = 160;
            byte[] src = new byte[count];
            for (int i = 0; i < count; i++)
                src[i] = (byte)i;

            String aStr = ArrayUtil.ArrayToStringCombined(src, 0, count);
            Debug.Write("aStr = " + aStr);
        }

        [Test, ExpectedException(typeof(ArgumentException))]
        public void TestReverceIntArrayTooBigCount()
        {
            Array.Reverse(mIntArray2, 0, 11);
        }

        /// <summary>
        /// Check that Java's msArray sorts strings using .Net's "string-sort" algorithm.
        /// </summary>
        [Test]
        public void TestJiraJ1734()
        {
            string [] list = new string[]
            {
                "a",
                "TOC",
                "A",
                "_Toc256000000"
            };

            Array.Sort(list);

            Assert.That("_Toc256000000", Is.EqualTo(list[0]));
            Assert.That("a", Is.EqualTo(list[1]));
            Assert.That("A", Is.EqualTo(list[2]));
            Assert.That("TOC", Is.EqualTo(list[3]));
        }

        [Test]
        public void TestStringArraySort()
        {
            String[] data1 = getData();
            Array.Sort(data1, StringComparer.Ordinal);
            Assert.That("2016,PersonName,Visual Studio,Windows,able,apple,la Secretaria,product,Æble,ångström,Привет,嗨", Is.EqualTo(string.Join(",", data1)));

            String[] data2 = getData();
            Array.Sort(data2, StringComparer.OrdinalIgnoreCase);
            Assert.That("2016,able,apple,la Secretaria,PersonName,product,Visual Studio,Windows,ångström,Æble,Привет,嗨", Is.EqualTo(string.Join(",", data2)));

            String[] data3 = getData();
            Array.Sort(data3, StringComparer.CurrentCulture);
            Assert.That("2016,able,Æble,ångström,apple,la Secretaria,PersonName,product,Visual Studio,Windows,Привет,嗨", Is.EqualTo(string.Join(",", data3)));

            String[] data4 = getData();
            Array.Sort(data4, StringComparer.CurrentCultureIgnoreCase);
            Assert.That("2016,able,Æble,ångström,apple,la Secretaria,PersonName,product,Visual Studio,Windows,Привет,嗨", Is.EqualTo(string.Join(",", data4)));

            String[] data5 = getData();
            Array.Sort(data5, StringComparer.InvariantCulture);
            Assert.That("2016,able,Æble,ångström,apple,la Secretaria,PersonName,product,Visual Studio,Windows,Привет,嗨", Is.EqualTo(string.Join(",", data5)));

            String[] data6 = getData();
            Array.Sort(data6, StringComparer.InvariantCultureIgnoreCase);
            Assert.That("2016,able,Æble,ångström,apple,la Secretaria,PersonName,product,Visual Studio,Windows,Привет,嗨", Is.EqualTo(string.Join(",", data6)));
        }

        [Test]
        public void TestArrayUtilConcat()
        {
            int[] result = ArrayUtil.Concat(mIntArray, mIntArray2);
            Assert.That(mIntArray.Length + mIntArray2.Length, Is.EqualTo(result.Length));
            Assert.That(mIntArray[5], Is.EqualTo(result[5]));
            Assert.That(mIntArray2[5], Is.EqualTo(result[mIntArray.Length + 5]));
        }

        [Test]
        public void TestArrayUtilConcatNull()
        {
            int[] resultInt = ArrayUtil.Concat(mIntArray, null);
            Assert.That(mIntArray, Is.EqualTo(resultInt));
            int length = ArrayUtil.ConcatLength(null, mObjArray);
            object[] resultObj = ArrayUtil.Concat(null, mObjArray, new object[length]);
            Assert.That(mObjArray, Is.EqualTo(resultObj));

            object[] resultNull = ArrayUtil.Concat<object>(null, null, null);
            Assert.That(resultNull, Is.Null);
        }

        [Test]
        public void TestArrayUtilConcatEmpty()
        {
            int[] result = ArrayUtil.Concat(mIntArray, new int[0]);
            Assert.That(mIntArray.Length, Is.EqualTo(result.Length));
            Assert.That(ArrayUtil.IsArrayEqual(result, mIntArray), Is.True);
        }

        private static String[] getData()
        {
            return new String[]
            {
                "PersonName",
                "嗨",
                "2016",
                "Привет",
                "la Secretaria",
                "product",
                "able",
                "ångström",
                "apple",
                "Æble",
                "Windows",
                "Visual Studio"
            };
        }

        private int[] mIntArray = new int[] { 0, 1, 2, 3, 4, 5, -1, 0, int.MinValue, int.MaxValue };//10 elements
        private int[] mIntArray2 = { 0, 1, 2, 3, 4, 5, -1, 0, int.MinValue, int.MaxValue };//10 elements
        private object[] mObjArray = { 0, 1, 2, 3, 4, 5, -1, 0, null, int.MaxValue };//10 elements
    }
}
