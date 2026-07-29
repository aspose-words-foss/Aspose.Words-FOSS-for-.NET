// Copyright (c) 2001-2026 Aspose Pty Ltd. All Rights Reserved.
// 30/05/2017 by Anatoliy Sidorenko

using NUnit.Framework;

namespace Aspose.JavaMs.Tests.System.Number
{
    [TestFixture]
    public class TestFormatLong
    {
        [Test]
        public void TestBasic()
        {
            long longValue = 0x1234567890ABCDEF;
            Assert.That(longValue.ToString(), Is.EqualTo("1311768467294899695"));
            Assert.That(longValue.ToString("D2"), Is.EqualTo("1311768467294899695"));
            Assert.That(longValue.ToString("d2"), Is.EqualTo("1311768467294899695"));
            Assert.That(longValue.ToString("D1"), Is.EqualTo("1311768467294899695"));
            Assert.That(longValue.ToString("d1"), Is.EqualTo("1311768467294899695"));
            Assert.That(longValue.ToString("X"), Is.EqualTo("1234567890ABCDEF"));
            Assert.That(longValue.ToString("x"), Is.EqualTo("1234567890abcdef"));
            Assert.That(longValue.ToString("X1"), Is.EqualTo("1234567890ABCDEF"));
            Assert.That(longValue.ToString("x1"), Is.EqualTo("1234567890abcdef"));
            Assert.That(longValue.ToString("G"), Is.EqualTo("1311768467294899695"));
            Assert.That(longValue.ToString("g"), Is.EqualTo("1311768467294899695"));
        }

        [Test]
        public void TestMinusSign()
        {
            long longValue = -31568343242576;
            Assert.That(longValue.ToString("D2"), Is.EqualTo("-31568343242576"));
            Assert.That(longValue.ToString("d2"), Is.EqualTo("-31568343242576"));
            Assert.That(longValue.ToString("D1"), Is.EqualTo("-31568343242576"));
            Assert.That(longValue.ToString("d1"), Is.EqualTo("-31568343242576"));
            Assert.That(longValue.ToString("D"), Is.EqualTo("-31568343242576"));
            Assert.That(longValue.ToString("d"), Is.EqualTo("-31568343242576"));
            Assert.That(longValue.ToString("G"), Is.EqualTo("-31568343242576"));
            Assert.That(longValue.ToString("g"), Is.EqualTo("-31568343242576"));
        }

        [Test]
        public void TestZeroPad()
        {
            long longValue = 31568343242576;
            long longValue1 = -31568343242576;
            Assert.That(longValue.ToString("D7"), Is.EqualTo("31568343242576"));
            Assert.That(longValue.ToString("d7"), Is.EqualTo("31568343242576"));
            Assert.That(longValue1.ToString("d7"), Is.EqualTo("-31568343242576"));

            Assert.That(longValue.ToString("D17"), Is.EqualTo("00031568343242576"));
            Assert.That(longValue.ToString("d17"), Is.EqualTo("00031568343242576"));
            Assert.That(longValue1.ToString("d17"), Is.EqualTo("-00031568343242576"));

            Assert.That(longValue.ToString("X5"), Is.EqualTo("1CB613E29750"));
            Assert.That(longValue.ToString("x5"), Is.EqualTo("1cb613e29750"));
            Assert.That(longValue1.ToString("x5"), Is.EqualTo("ffffe349ec1d68b0"));
        }

        [Test]
        public void TestlongArray()
        {
            Assert.That(longArray[3].ToString(), Is.EqualTo("-9223372036854775709"));
            Assert.That(longArray[4].ToString("d7"), Is.EqualTo("9223372036854775806"));

            Assert.That(longArray[2].ToString("X2"), Is.EqualTo("8000000000000009"));
            Assert.That(longArray[4].ToString("x"), Is.EqualTo("7ffffffffffffffe"));
        }

        [Test]
        public void TestBorder()
        {
            Assert.That(longArray[0].ToString(), Is.EqualTo("9223372036854775807"));
            Assert.That(longArray[1].ToString(), Is.EqualTo("-9223372036854775808"));
            Assert.That(longArray[2].ToString(), Is.EqualTo("-9223372036854775799"));
            Assert.That(longArray[3].ToString(), Is.EqualTo("-9223372036854775709"));
            Assert.That(longArray[4].ToString(), Is.EqualTo("9223372036854775806"));
            Assert.That(longArray[5].ToString(), Is.EqualTo("9223372036854775797"));
            Assert.That(longArray[6].ToString(), Is.EqualTo("9223372036854775707"));
            Assert.That(longArray[7].ToString(), Is.EqualTo("-9223372036854775808"));
            Assert.That(longArray[8].ToString(), Is.EqualTo("-9223372036854775807"));
            Assert.That(longArray[9].ToString(), Is.EqualTo("-9223372036854775798"));
            Assert.That(longArray[10].ToString(), Is.EqualTo("-9223372036854775708"));
            Assert.That(longArray[11].ToString(), Is.EqualTo("9223372036854775807"));
            Assert.That(longArray[12].ToString(), Is.EqualTo("9223372036854775798"));
            Assert.That(longArray[13].ToString(), Is.EqualTo("9223372036854775708"));
        }

        [Test]
        public void TestMax()
        {
            Assert.That(maxValue.ToString(), Is.EqualTo("9223372036854775807"));
            Assert.That(maxValue.ToString("D"), Is.EqualTo("9223372036854775807"));
            Assert.That(maxValue.ToString("d"), Is.EqualTo("9223372036854775807"));
            Assert.That(maxValue.ToString("G"), Is.EqualTo("9223372036854775807"));
            Assert.That(maxValue.ToString("g"), Is.EqualTo("9223372036854775807"));

            Assert.That(maxValue.ToString("D1"), Is.EqualTo("9223372036854775807"));
            Assert.That(maxValue.ToString("d2"), Is.EqualTo("9223372036854775807"));
            Assert.That(maxValue.ToString("D21"), Is.EqualTo("009223372036854775807"));

            Assert.That(maxValue.ToString("X"), Is.EqualTo("7FFFFFFFFFFFFFFF"));
            Assert.That(maxValue.ToString("x"), Is.EqualTo("7fffffffffffffff"));
        }

        [Test]
        public void TestMin()
        {
            Assert.That(minValue.ToString(), Is.EqualTo("-9223372036854775808"));
            Assert.That(minValue.ToString("D"), Is.EqualTo("-9223372036854775808"));
            Assert.That(minValue.ToString("d"), Is.EqualTo("-9223372036854775808"));
            Assert.That(minValue.ToString("G"), Is.EqualTo("-9223372036854775808"));
            Assert.That(minValue.ToString("g"), Is.EqualTo("-9223372036854775808"));

            Assert.That(minValue.ToString("D1"), Is.EqualTo("-9223372036854775808"));
            Assert.That(minValue.ToString("d2"), Is.EqualTo("-9223372036854775808"));
            Assert.That(minValue.ToString("D21"), Is.EqualTo("-009223372036854775808"));

            Assert.That(minValue.ToString("X"), Is.EqualTo("8000000000000000"));
            Assert.That(minValue.ToString("x"), Is.EqualTo("8000000000000000"));
        }

        private static long maxValue = long.MaxValue;
        private static long minValue = long.MinValue;

        private static long[] longArray =
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
