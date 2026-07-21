// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/05/2017 by Anatoliy Sidorenko

using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Number
{
    [TestFixture]
    public class TestFormatInt32
    {
        [Test]
        public void TestBasic()
        {
            int intValue = 0x1FFFFFFF;
            Assert.That(intValue.ToString(), Is.EqualTo("536870911"));
            Assert.That(intValue.ToString("D2"), Is.EqualTo("536870911"));
            Assert.That(intValue.ToString("d2"), Is.EqualTo("536870911"));
            Assert.That(intValue.ToString("D1"), Is.EqualTo("536870911"));
            Assert.That(intValue.ToString("d1"), Is.EqualTo("536870911"));
            Assert.That(intValue.ToString("X"), Is.EqualTo("1FFFFFFF"));
            Assert.That(intValue.ToString("x"), Is.EqualTo("1fffffff"));
            Assert.That(intValue.ToString("X1"), Is.EqualTo("1FFFFFFF"));
            Assert.That(intValue.ToString("x1"), Is.EqualTo("1fffffff"));
            Assert.That(intValue.ToString("G"), Is.EqualTo("536870911"));
            Assert.That(intValue.ToString("g"), Is.EqualTo("536870911"));
        }

        [Test]
        public void TestMinusSign()
        {
            int intValue = -3156876;
            Assert.That(intValue.ToString("D2"), Is.EqualTo("-3156876"));
            Assert.That(intValue.ToString("d2"), Is.EqualTo("-3156876"));
            Assert.That(intValue.ToString("D1"), Is.EqualTo("-3156876"));
            Assert.That(intValue.ToString("d1"), Is.EqualTo("-3156876"));
            Assert.That(intValue.ToString("D"), Is.EqualTo("-3156876"));
            Assert.That(intValue.ToString("d"), Is.EqualTo("-3156876"));
            Assert.That(intValue.ToString("G"), Is.EqualTo("-3156876"));
            Assert.That(intValue.ToString("g"), Is.EqualTo("-3156876"));
        }

        [Test]
        public void TestZeroPad()
        {
            int intValue = 3156876;
            int intValue1 = -3156876;
            Assert.That(intValue.ToString("D7"), Is.EqualTo("3156876"));
            Assert.That(intValue.ToString("d7"), Is.EqualTo("3156876"));
            Assert.That(intValue1.ToString("d7"), Is.EqualTo("-3156876"));

            Assert.That(intValue.ToString("D10"), Is.EqualTo("0003156876"));
            Assert.That(intValue.ToString("d10"), Is.EqualTo("0003156876"));
            Assert.That(intValue1.ToString("d10"), Is.EqualTo("-0003156876"));

            Assert.That(intValue.ToString("X5"), Is.EqualTo("302B8C"));
            Assert.That(intValue.ToString("x5"), Is.EqualTo("302b8c"));
            Assert.That(intValue1.ToString("x5"), Is.EqualTo("ffcfd474"));
        }

        [Test]
        public void TestintArray()
        {
            Assert.That(intArray[3].ToString(), Is.EqualTo("-2147483549"));
            Assert.That(intArray[4].ToString("d7"), Is.EqualTo("2147483646"));

            Assert.That(intArray[2].ToString("X2"), Is.EqualTo("80000009"));
            Assert.That(intArray[4].ToString("x"), Is.EqualTo("7ffffffe"));
        }

        [Test]
        public void TestBorder()
        {
            Assert.That(intArray[0].ToString(), Is.EqualTo("2147483647"));
            Assert.That(intArray[1].ToString(), Is.EqualTo("-2147483648"));
            Assert.That(intArray[2].ToString(), Is.EqualTo("-2147483639"));
            Assert.That(intArray[3].ToString(), Is.EqualTo("-2147483549"));
            Assert.That(intArray[4].ToString(), Is.EqualTo("2147483646"));
            Assert.That(intArray[5].ToString(), Is.EqualTo("2147483637"));
            Assert.That(intArray[6].ToString(), Is.EqualTo("2147483547"));
            Assert.That(intArray[7].ToString(), Is.EqualTo("-2147483648"));
            Assert.That(intArray[8].ToString(), Is.EqualTo("-2147483647"));
            Assert.That(intArray[9].ToString(), Is.EqualTo("-2147483638"));
            Assert.That(intArray[10].ToString(), Is.EqualTo("-2147483548"));
            Assert.That(intArray[11].ToString(), Is.EqualTo("2147483647"));
            Assert.That(intArray[12].ToString(), Is.EqualTo("2147483638"));
            Assert.That(intArray[13].ToString(), Is.EqualTo("2147483548"));
        }

        [Test]
        public void TestMax()
        {
            Assert.That(maxValue.ToString(), Is.EqualTo("2147483647"));
            Assert.That(maxValue.ToString("D"), Is.EqualTo("2147483647"));
            Assert.That(maxValue.ToString("d"), Is.EqualTo("2147483647"));
            Assert.That(maxValue.ToString("G"), Is.EqualTo("2147483647"));
            Assert.That(maxValue.ToString("g"), Is.EqualTo("2147483647"));

            Assert.That(maxValue.ToString("D1"), Is.EqualTo("2147483647"));
            Assert.That(maxValue.ToString("d2"), Is.EqualTo("2147483647"));
            Assert.That(maxValue.ToString("D12"), Is.EqualTo("002147483647"));

            Assert.That(maxValue.ToString("X"), Is.EqualTo("7FFFFFFF"));
            Assert.That(maxValue.ToString("x"), Is.EqualTo("7fffffff"));
        }

        [Test]
        public void TestMin()
        {
            Assert.That(minValue.ToString(), Is.EqualTo("-2147483648"));
            Assert.That(minValue.ToString("D"), Is.EqualTo("-2147483648"));
            Assert.That(minValue.ToString("d"), Is.EqualTo("-2147483648"));
            Assert.That(minValue.ToString("G"), Is.EqualTo("-2147483648"));
            Assert.That(minValue.ToString("g"), Is.EqualTo("-2147483648"));

            Assert.That(minValue.ToString("D1"), Is.EqualTo("-2147483648"));
            Assert.That(minValue.ToString("d2"), Is.EqualTo("-2147483648"));
            Assert.That(minValue.ToString("D12"), Is.EqualTo("-002147483648"));

            Assert.That(minValue.ToString("X"), Is.EqualTo("80000000"));
            Assert.That(minValue.ToString("x"), Is.EqualTo("80000000"));
        }

        private static int maxValue = int.MaxValue;
        private static int minValue = int.MinValue;

        private static int[] intArray =
        {
            maxValue,
            maxValue + 1,
            maxValue + 10,
            maxValue + 100,
            maxValue - 1,
            maxValue - 10,
            maxValue - 100,
            minValue,
            minValue + 1,
            minValue + 10,
            minValue + 100,
            minValue - 1,
            minValue - 10,
            minValue - 100
        };
    }
}
